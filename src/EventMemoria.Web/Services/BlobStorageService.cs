using System.Web;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EventMemoria.Web.Common.Constants;
using EventMemoria.Web.Common.Settings;
using EventMemoria.Web.Extensions;
using EventMemoria.Web.Helpers;
using EventMemoria.Web.Models;
using EventMemoria.Web.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace EventMemoria.Web.Services;

public class BlobStorageService(
    BlobServiceClient blobServiceClient,
    IOptions<PhotoOptions> photoOptions,
    ILogger<BlobStorageService> logger) : IStorageService
{
    public async Task<string> UploadThumbnailAsync(Stream fileStream, string fileName, string? contentType = null, string? author = null)
    {
        return await UploadFileAsync(
            photoOptions.Value.StorageContainer.Thumbnails,
            fileStream,
            fileName,
            contentType,
            author);
    }

    public async Task<string> UploadFullSizeAsync(Stream fileStream, string fileName, string? contentType = null, string? author = null)
    {
        return await UploadFileAsync(
            photoOptions.Value.StorageContainer.FullSize,
            fileStream,
            fileName,
            contentType,
            author);
    }

    public async Task<string> UploadVideoAsync(Stream fileStream, string fileName, string? contentType = null, string? author = null)
    {
        return await UploadFileAsync(
            photoOptions.Value.StorageContainer.Videos,
            fileStream,
            fileName,
            contentType,
            author,
            isVideo: true);
    }

    private async Task<string> UploadFileAsync(string containerName, Stream fileStream, string fileName, string? contentType = null, string? author = null, bool isVideo = false)
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var uniqueFileName = $"{Guid.NewGuid()}{MediaHelper.GetFileExtension(fileName)}";
            var blobClient = containerClient.GetBlobClient(uniqueFileName);

            var blobHttpHeaders = new BlobHttpHeaders();
            if (!string.IsNullOrEmpty(contentType))
            {
                blobHttpHeaders.ContentType = contentType;
            }

            var mediaType = isVideo ? "Video" : "Image";

            var uploadOptions = new BlobUploadOptions
            {
                HttpHeaders = blobHttpHeaders,
                Tags = new Dictionary<string, string>
                {
                    { ApplicationConstants.ImageTags.Author, SanitizingHelper.SanitizeValue(author ?? "Anonymous") },
                    { ApplicationConstants.ImageTags.FileName, SanitizingHelper.SanitizeValue(fileName) },
                    { ApplicationConstants.ImageTags.UploadedAt, DateTime.UtcNow.ToString("o") },
                    { ApplicationConstants.ImageTags.MediaType, mediaType }
                }
            };

            await blobClient.UploadAsync(fileStream, uploadOptions);

            logger.LogInformation("{MediaType} {FileName} successfully uploaded as {BlobName}", mediaType, fileName, uniqueFileName);

            return uniqueFileName;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error uploading file {FileName}", fileName);
            throw;
        }
    }

    public async Task<PagedResult<Photo>> GetPhotosPagedAsync(int page = 1, int pageSize = 24, string? folderName = null)
    {
        try
        {
            var allMediaItems = new List<(BlobItem blob, BlobClient client, bool isVideo)>();

            if (!string.IsNullOrWhiteSpace(folderName))
            {
                await LoadMediaItemsFromFolderAsync(folderName, allMediaItems);
            }
            else
            {
                await LoadMediaItemsFromContainersAsync(allMediaItems);
            }

            allMediaItems = allMediaItems
                .OrderByDescending(item => item.blob.Properties.LastModified)
                .ToList();

            var totalCount = allMediaItems.Count;
            var skip = (page - 1) * pageSize;
            var pagedItems = allMediaItems.Skip(skip).Take(pageSize).ToList();

            var photos = new List<Photo>();

            foreach (var (blobItem, blobClient, isVideo) in pagedItems)
            {
                var photo = CreatePhotoFromBlobItem(blobItem, blobClient, isVideo);
                photos.Add(photo);
            }

            logger.LogInformation("Retrieved page {Page} with {Count} media items (Images: {ImageCount}, Videos: {VideoCount}). Total: {TotalCount}",
                page, photos.Count, 
                photos.Count(p => p.MediaType == MediaType.Image),
                photos.Count(p => p.MediaType == MediaType.Video),
                totalCount);

            return new PagedResult<Photo>(photos, page, pageSize, totalCount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting paged media items");
            throw;
        }
    }

    private static Photo CreatePhotoFromBlobItem(BlobItem blobItem, BlobClient blobClient, bool isVideo)
    {
        var mediaTypeFromTag = "Image";

        try
        {
            mediaTypeFromTag = blobItem.Tags.FirstOrDefault(x => x.Key == ApplicationConstants.ImageTags.MediaType).Value;
        }
        catch (Exception) { }

        var actualIsVideo = isVideo || mediaTypeFromTag == "Video";

        return new ()
        {
            Id = Guid.NewGuid().ToString(),
            FileName = blobItem.Name,
            Url = blobClient.Uri.ToString(),
            UploadDate = blobItem.Properties.LastModified?.DateTime ?? DateTime.MinValue,
            FileSize = blobItem.Properties.ContentLength ?? 0,
            Author = blobItem.Tags?.FirstOrDefault(x => x.Key == ApplicationConstants.ImageTags.Author).Value ?? string.Empty,
            MediaType = actualIsVideo ? MediaType.Video : MediaType.Image,
            ThumbnailUrl = actualIsVideo ? blobClient.Uri.ToString() : null
        };
    }

    public async Task<IEnumerable<string>> GetGallerySubFoldersAsync()
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(photoOptions.Value.StorageContainer.Gallery);

            if (!await containerClient.ExistsAsync())
            {
                logger.LogWarning("Container prod-gallery does not exist when listing top-level folders");
                return [];
            }

            var folderNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            await foreach (var item in containerClient.GetBlobsByHierarchyAsync(delimiter: "/"))
            {
                if (!item.IsPrefix || string.IsNullOrWhiteSpace(item.Prefix))
                {
                    continue;
                }

                var prefix = item.Prefix.TrimEnd('/');

                if (!string.IsNullOrEmpty(prefix))
                {
                    folderNames.Add(prefix);
                }
            }

            var orderedFolders = folderNames
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            logger.LogInformation("Retrieved {Count} top-level folders from gallery-container", orderedFolders.Count);

            return orderedFolders;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error listing top-level folders from gallery-container");
            throw;
        }
    }

    private async Task LoadMediaItemsFromFolderAsync(string folderName, List<(BlobItem blob, BlobClient client, bool isVideo)> allMediaItems)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(photoOptions.Value.StorageContainer.Gallery);

        if (await containerClient.ExistsAsync())
        {
            var prefix = $"{folderName.TrimEnd('/')}/";

            await foreach (var item in containerClient.GetBlobsByHierarchyAsync(prefix: prefix, delimiter: "/", traits: BlobTraits.All))
            {
                if (item is null || item.IsPrefix)
                {
                    continue;
                }

                var blobItem = item.Blob;
                var blobClient = containerClient.GetBlobClient(blobItem.Name);
                allMediaItems.Add((blobItem, blobClient, false));
            }
        }
    }

    private async Task LoadMediaItemsFromContainersAsync(List<(BlobItem blob, BlobClient client, bool isVideo)> allMediaItems)
    {
        var thumbnailContainer = photoOptions.Value.StorageContainer.Thumbnails;
        var thumbnailContainerClient = blobServiceClient.GetBlobContainerClient(thumbnailContainer);
        if (await thumbnailContainerClient.ExistsAsync())
        {
            await foreach (var blobItem in thumbnailContainerClient.GetBlobsAsync(traits: BlobTraits.All))
            {
                var blobClient = thumbnailContainerClient.GetBlobClient(blobItem.Name);
                allMediaItems.Add((blobItem, blobClient, false));
            }
        }

        var videosContainer = photoOptions.Value.StorageContainer.Videos;
        var videosContainerClient = blobServiceClient.GetBlobContainerClient(videosContainer);
        if (await videosContainerClient.ExistsAsync())
        {
            await foreach (var blobItem in videosContainerClient.GetBlobsAsync(traits: BlobTraits.All))
            {
                var blobClient = videosContainerClient.GetBlobClient(blobItem.Name);
                allMediaItems.Add((blobItem, blobClient, true));
            }
        }
    }
}
