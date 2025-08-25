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


    private async Task<string> UploadFileAsync(string containerName, Stream fileStream, string fileName, string? contentType = null, string? author = null)
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var uniqueFileName = $"{Guid.NewGuid()}{PhotoHelper.GetFileExtension(fileName)}";
            var blobClient = containerClient.GetBlobClient(uniqueFileName);

            var blobHttpHeaders = new BlobHttpHeaders();
            if (!string.IsNullOrEmpty(contentType))
            {
                blobHttpHeaders.ContentType = contentType;
            }

            var uploadOptions = new BlobUploadOptions
            {
                HttpHeaders = blobHttpHeaders,
                Tags = new Dictionary<string, string>
                {
                    { ApplicationConstants.ImageTags.Author, SanitizingHelper.SanitizeValue(author ?? "Anonymous") },
                    { ApplicationConstants.ImageTags.FileName, SanitizingHelper.SanitizeValue(fileName) },
                    { ApplicationConstants.ImageTags.UploadedAt, DateTime.UtcNow.ToString("o") }
                }
            };

            await blobClient.UploadAsync(fileStream, uploadOptions);

            logger.LogInformation("File {FileName} successfully uploaded as {BlobName}", fileName, uniqueFileName);

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
        var thumbnailContainer = photoOptions.Value.StorageContainer.Thumbnails;

        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(thumbnailContainer);

            if (!await containerClient.ExistsAsync())
            {
                return new PagedResult<Photo>();
            }

            var allBlobs = new List<BlobItem>();

            await foreach (var blobItem in containerClient.GetBlobsAsync(traits: BlobTraits.All))
            {
                allBlobs.Add(blobItem);
            }

            allBlobs = allBlobs.OrderByDescending(b => b.Properties.LastModified).ToList();

            var totalCount = allBlobs.Count;
            var skip = (page - 1) * pageSize;
            var pagedBlobs = allBlobs.Skip(skip).Take(pageSize).ToList();

            var photos = new List<Photo>();

            foreach (var blobItem in pagedBlobs)
            {
                var blobClient = containerClient.GetBlobClient(blobItem.Name);
                var photo = CreatePhotoFromBlobItem(blobItem, blobClient);
                photos.Add(photo);
            }

            logger.LogInformation("Retrieved page {Page} with {Count} photos from container {ContainerName}. Total: {TotalCount}",
                page, photos.Count, thumbnailContainer, totalCount);

            return new PagedResult<Photo>(photos, page, pageSize, totalCount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting paged photos from container {ContainerName}", thumbnailContainer);
            throw;
        }
    }

    private static Photo CreatePhotoFromBlobItem(BlobItem blobItem, BlobClient blobClient)
    {
        return new Photo
        {
            Id = Guid.NewGuid().ToString(),
            FileName = blobItem.Name,
            Url = blobClient.Uri.ToString(),
            UploadDate = blobItem.Properties.LastModified?.DateTime ?? DateTime.MinValue,
            FileSize = blobItem.Properties.ContentLength ?? 0,
            Author = blobItem.Tags.FirstOrDefault(x => x.Key == ApplicationConstants.ImageTags.Author).Value
        };
    }
}
