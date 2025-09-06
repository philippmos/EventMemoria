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
            await using var fileStream = file.OpenReadStream(ApplicationConstants.FileUpload.MaxFileSizeInBytes);
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
            image.Resize(
                ApplicationConstants.FileUpload.Thumbnail.ResizeWidth,
                ApplicationConstants.FileUpload.Thumbnail.ResizeHeight);
            image.Quality = ApplicationConstants.FileUpload.Thumbnail.Quality;

            using var thumbnailStream = new MemoryStream();
            await image.WriteAsync(thumbnailStream, ApplicationConstants.FileUpload.Thumbnail.Format);
            var thumbnailBytes = thumbnailStream.ToArray();

            using var uploadStream = new MemoryStream(thumbnailBytes);

            var thumbnailFileName = $"{PhotoHelper.GetFileNameWithoutExtension(fileName)}.{ApplicationConstants.FileUpload.Thumbnail.Format.ToString().ToLower()}";
            await storageService.UploadThumbnailAsync(uploadStream, thumbnailFileName, contentType, userName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating thumbnail for {FileName}", fileName);
        }
    }
}
