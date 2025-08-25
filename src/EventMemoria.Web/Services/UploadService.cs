using EventMemoria.Web.Common.Constants;
using EventMemoria.Web.Helpers;
using EventMemoria.Web.Models;
using EventMemoria.Web.Services.Interfaces;
using ImageMagick;
using Microsoft.AspNetCore.Components.Forms;

namespace EventMemoria.Web.Services;

public class UploadService(
    IStorageService storageService,
    ILogger<UploadService> logger) : IUploadService
{
    public async Task<UploadResult> ProcessFileAsync(IBrowserFile file, string? userName)
    {
        try
        {
            using var fileStream = file.OpenReadStream(ApplicationConstants.FileUpload.MaxFileSizeInBytes);
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            
            var fileBytes = memoryStream.ToArray();
            
            await CreateAndUploadThumbnailAsync(fileBytes, file.Name, file.ContentType, userName);
            
            using var uploadStream = new MemoryStream(fileBytes);
            await storageService.UploadFullSizeAsync(uploadStream, file.Name, file.ContentType, userName);

            return UploadResult.Success(file.Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing file {FileName}", file.Name);
            return UploadResult.Failure(file.Name, ex.Message);
        }
    }

    private async Task CreateAndUploadThumbnailAsync(byte[] fileBytes, string fileName, string? contentType, string? userName)
    {
        try
        {
            using var image = new MagickImage(fileBytes);
            image.Resize(700, 700);
            image.Quality = 85;

            var thumbnailFormat = MagickFormat.Jpeg;

            using var thumbnailStream = new MemoryStream();
            image.Write(thumbnailStream, thumbnailFormat);
            var thumbnailBytes = thumbnailStream.ToArray();

            using var uploadStream = new MemoryStream(thumbnailBytes);

            var thumbnailFileName = $"{PhotoHelper.GetFileNameWithoutExtension(fileName)}.{thumbnailFormat.ToString().ToLower()}";
            await storageService.UploadThumbnailAsync(uploadStream, thumbnailFileName, contentType, userName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating thumbnail for {FileName}", fileName);
        }
    }
}
