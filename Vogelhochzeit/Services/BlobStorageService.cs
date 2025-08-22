using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Vogelhochzeit.Services;

public class BlobStorageService(BlobServiceClient blobServiceClient, ILogger<BlobStorageService> logger) : IStorageService
{
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string containerName, string? contentType = null)
    {
        try
        {
            var containerClient = await GetOrCreateContainerAsync(containerName);
            
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var blobClient = containerClient.GetBlobClient(uniqueFileName);

            var blobHttpHeaders = new BlobHttpHeaders();
            if (!string.IsNullOrEmpty(contentType))
            {
                blobHttpHeaders.ContentType = contentType;
            }

            var uploadOptions = new BlobUploadOptions
            {
                HttpHeaders = blobHttpHeaders
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

    public async Task<Stream> DownloadFileAsync(string blobName, string containerName)
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
            {
                throw new FileNotFoundException($"Blob {blobName} not found");
            }

            var response = await blobClient.DownloadStreamingAsync();
            
            logger.LogInformation("File {BlobName} successfully downloaded", blobName);
            
            return response.Value.Content;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error downloading file {BlobName}", blobName);
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(string blobName, string containerName)
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var response = await blobClient.DeleteIfExistsAsync();
            
            if (response.Value)
            {
                logger.LogInformation("File {BlobName} successfully deleted", blobName);
            }
            else
            {
                logger.LogWarning("File {BlobName} not found or already deleted", blobName);
            }
            
            return response.Value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting file {BlobName}", blobName);
            throw;
        }
    }

    public async Task<List<string>> ListFilesAsync(string containerName, string? prefix = null)
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            
            if (!await containerClient.ExistsAsync())
            {
                return new List<string>();
            }

            var blobs = new List<string>();
            
            await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: prefix))
            {
                blobs.Add(blobItem.Name);
            }

            logger.LogInformation("Found files in container {ContainerName}: {Count}", containerName, blobs.Count);
            
            return blobs;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error listing files in container {ContainerName}", containerName);
            throw;
        }
    }

    public async Task<bool> FileExistsAsync(string blobName, string containerName)
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var response = await blobClient.ExistsAsync();
            return response.Value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking file existence {BlobName}", blobName);
            throw;
        }
    }

    public async Task<string> GetFileUrlAsync(string blobName, string containerName)
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
            {
                throw new FileNotFoundException($"Blob {blobName} not found");
            }

            return blobClient.Uri.ToString();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting file URL {BlobName}", blobName);
            throw;
        }
    }

    private async Task<BlobContainerClient> GetOrCreateContainerAsync(string containerName)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);
        
        return containerClient;
    }
}
