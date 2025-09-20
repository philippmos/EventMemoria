using System.Threading.Tasks;
using EventMemoria.Web.Common.Constants;
using EventMemoria.Web.Common.Settings;
using EventMemoria.Web.Helpers;
using EventMemoria.Web.Models;
using EventMemoria.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;

namespace EventMemoria.Web.Services;

public class FileValidationService(
    IOptions<PhotoOptions> photoOptions,
    IFeatureManager featureManager) : IFileValidationService
{
    public async Task<ValidationResult> ValidateFileAsync(IBrowserFile? file)
    {
        if (file == null)
        {
            return ValidationResult.Failure("Datei ist null");
        }

        var extension = Path.GetExtension(file.Name).ToLowerInvariant();
        var isImage = MediaHelper.IsImageFile(file.ContentType);
        var isVideo = MediaHelper.IsVideoFile(file.ContentType);

        if (isVideo && !await featureManager.IsEnabledAsync(FeatureFlags.EnableVideoUpload))
        {
            return ValidationResult.Failure("Der Video-Upload ist derzeit nicht verfügbar");
        }

        if (!isImage && !isVideo)
        {
            return ValidationResult.Failure($"Dateityp '{extension}' wird nicht unterstützt. Erlaubte Formate: Bilder und Videos");
        }

        var maxSize = isVideo ? ApplicationConstants.FileUpload.MaxVideoFileSizeInBytes : ApplicationConstants.FileUpload.MaxFileSizeInBytes;
        if (file.Size > maxSize)
        {
            var maxSizeMb = maxSize / (1024 * 1024);
            var fileType = isVideo ? "Video" : "Bild";
            return ValidationResult.Failure($"{fileType} '{file.Name}' ist zu groß (max. {maxSizeMb} MB)");
        }

        if (photoOptions.Value.AllowedFileTypes.Any() &&
            !photoOptions.Value.AllowedFileTypes.Contains(extension))
        {
            return ValidationResult.Failure($"Dateityp '{extension}' wird nicht unterstützt");
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidateMaxFileCount(IEnumerable<IBrowserFile> files)
    {
        var fileList = files.ToList();

        return fileList.Count > ApplicationConstants.FileUpload.MaxFileCount
            ? ValidationResult.Failure($"Zu viele Dateien (max. {ApplicationConstants.FileUpload.MaxFileCount} auf einmal).")
            : ValidationResult.Success();
    }
}
