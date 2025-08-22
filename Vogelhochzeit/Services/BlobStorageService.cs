using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using Vogelhochzeit.Common.Settings;
using Vogelhochzeit.Models;

namespace Vogelhochzeit.Services;

public class BlobStorageService(
    BlobServiceClient blobServiceClient,
    IOptions<ImageOptions> imageOptions,
    ILogger<BlobStorageService> logger) : IStorageService
{
    private readonly string _containerName = imageOptions.Value.ContainerName;

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string? contentType = null)
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            
            var uniqueFileName = Guid.NewGuid().ToString();
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
                    { "Author", "Philipp Moser" },
                    { "FileName", fileName },
                    { "UploadedAt", DateTime.UtcNow.ToString("o") }
                },
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

    public async Task<PagedResult<Photo>> GetPhotosPagedAsync(int page = 1, int pageSize = 24, string? prefix = null)
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            
            if (!await containerClient.ExistsAsync())
            {
                return new PagedResult<Photo>();
            }

            var allBlobs = new List<BlobItem>();
            
            await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: prefix, traits: BlobTraits.Metadata))
            {
                if (IsImageFile(blobItem.Name))
                {
                    allBlobs.Add(blobItem);
                }
            }

            allBlobs = allBlobs.OrderByDescending(b => b.Properties.LastModified).ToList();

            var totalCount = allBlobs.Count;
            var skip = (page - 1) * pageSize;
            var pagedBlobs = allBlobs.Skip(skip).Take(pageSize).ToList();

            var photos = new List<Photo>();
            
            foreach (var blobItem in pagedBlobs)
            {
                var blobClient = containerClient.GetBlobClient(blobItem.Name);
                var photo = new Photo
                {
                    Id = Guid.NewGuid().ToString(),
                    FileName = blobItem.Name,
                    Url = blobClient.Uri.ToString(),
                    UploadDate = blobItem.Properties.LastModified?.DateTime ?? DateTime.MinValue,
                    FileSize = blobItem.Properties.ContentLength ?? 0
                };
                photos.Add(photo);
            }

            logger.LogInformation("Retrieved page {Page} with {Count} photos from container {ContainerName}. Total: {TotalCount}", 
                page, photos.Count, _containerName, totalCount);

            return new PagedResult<Photo>(photos, page, pageSize, totalCount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting paged photos from container {ContainerName}", _containerName);
            throw;
        }
    }

    private bool IsImageFile(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return imageOptions.Value.AllowedFileTypes.Contains(extension);
    }
}
