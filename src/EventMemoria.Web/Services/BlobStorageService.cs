using System.Text.RegularExpressions;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EventMemoria.Web.Common.Constants;
using EventMemoria.Web.Common.Settings;
using EventMemoria.Web.Models;
using EventMemoria.Web.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace EventMemoria.Web.Services;

public class BlobStorageService(
    BlobServiceClient blobServiceClient,
    IOptions<PhotoOptions> photoOptions,
    ILogger<BlobStorageService> logger) : IStorageService
{
    private readonly string _containerName = photoOptions.Value.ContainerName;

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string? contentType = null, string? author = null)
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            var uniqueFileName = $"{Guid.NewGuid()}{GetFileExtension(fileName)}";
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
                    { ApplicationConstants.ImageTags.Author, SanitizeTagValue(author ?? "Anonymous") },
                    { ApplicationConstants.ImageTags.FileName, SanitizeTagValue(fileName) },
                    { ApplicationConstants.ImageTags.UploadedAt, DateTime.UtcNow.ToString("o") }
                }
            };

            await blobClient.UploadAsync(fileStream, uploadOptions);

            logger.LogInformation("File {FileName} successfully uploaded as {BlobName} by {Author}", fileName, uniqueFileName, author ?? "Anonymous");

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
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

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
                page, photos.Count, _containerName, totalCount);

            return new PagedResult<Photo>(photos, page, pageSize, totalCount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting paged photos from container {ContainerName}", _containerName);
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

    private static string GetFileExtension(string fileName)
        => Path.GetExtension(fileName).ToLowerInvariant();


    private static string SanitizeTagValue(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "Unknown";
        }

        var sanitized = Regex.Replace(value, @"[^a-zA-Z0-9\s\+\-\.\/:=_]", "_");

        sanitized = sanitized.Trim();

        if (string.IsNullOrEmpty(sanitized))
        {
            sanitized = "Unknown";
        }

        if (sanitized.Length > 256)
        {
            sanitized = sanitized[..256];
        }

        sanitized = sanitized.TrimEnd();

        return sanitized;
    }
}
