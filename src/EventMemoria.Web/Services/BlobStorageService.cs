using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EventMemoria.Web.Common.Constants;
using EventMemoria.Web.Common.Settings;
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

    public async Task<PagedResult<Photo>> GetPhotosPagedAsync(int page = 1, int pageSize = 24)
    {
        try
        {
            var allMediaItems = new List<(BlobItem blob, BlobClient client, bool isVideo)>();

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
        var mediaTypeFromTag = blobItem.Tags.FirstOrDefault(x => x.Key == ApplicationConstants.ImageTags.MediaType).Value;
        var actualIsVideo = isVideo || mediaTypeFromTag == "Video";

        return new ()
        {
            Id = Guid.NewGuid().ToString(),
            FileName = blobItem.Name,
            Url = blobClient.Uri.ToString(),
            UploadDate = blobItem.Properties.LastModified?.DateTime ?? DateTime.MinValue,
            FileSize = blobItem.Properties.ContentLength ?? 0,
            Author = blobItem.Tags.FirstOrDefault(x => x.Key == ApplicationConstants.ImageTags.Author).Value,
            MediaType = actualIsVideo ? MediaType.Video : MediaType.Image,
            ThumbnailUrl = actualIsVideo ? blobClient.Uri.ToString() : null
        };
    }
}
