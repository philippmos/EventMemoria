using EventMemoria.Web.Common.Constants;
using EventMemoria.Web.Common.Settings;
using EventMemoria.Web.Models;
using EventMemoria.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;

namespace EventMemoria.Web.Services;

public class FileValidationService(IOptions<PhotoOptions> photoOptions) : IFileValidationService
{
    public ValidationResult ValidateFile(IBrowserFile? file)
    {
        if (file == null)
        {
            return ValidationResult.Failure("Datei ist null");
        }

        if (file.Size > ApplicationConstants.FileUpload.MaxFileSizeInBytes)
        {
            return ValidationResult.Failure($"Datei '{file.Name}' ist zu groß (max. {ApplicationConstants.FileUpload.MaxFileSizeInBytes / (1024 * 1024)} MB)");
        }

        var extension = Path.GetExtension(file.Name).ToLowerInvariant();

        if (!photoOptions.Value.AllowedFileTypes.Contains(extension))
        {
            return ValidationResult.Failure($"Dateityp '{extension}' wird nicht unterstützt");
        }

        return !file.ContentType.StartsWith("image/")
            ? ValidationResult.Failure($"Datei '{file.Name}' ist kein gültiges Bild")
            : ValidationResult.Success();
    }

    public ValidationResult ValidateMaxFileCount(IEnumerable<IBrowserFile> files)
    {
        var fileList = files.ToList();

        return fileList.Count > ApplicationConstants.FileUpload.MaxFileCount
            ? ValidationResult.Failure($"Zu viele Dateien (max. {ApplicationConstants.FileUpload.MaxFileCount} auf einmal).")
            : ValidationResult.Success();
    }
}
