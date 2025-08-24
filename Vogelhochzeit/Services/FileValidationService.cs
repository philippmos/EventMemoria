using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using Vogelhochzeit.Common.Constants;
using Vogelhochzeit.Common.Settings;
using Vogelhochzeit.Models;
using Vogelhochzeit.Services.Interfaces;

namespace Vogelhochzeit.Services;

public class FileValidationService(IOptions<PhotoOptions> photoOptions) : IFileValidationService
{
    public ValidationResult ValidateFile(IBrowserFile file)
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

        if (!file.ContentType.StartsWith("image/"))
        {
            return ValidationResult.Failure($"Datei '{file.Name}' ist kein gültiges Bild");
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidateFiles(IEnumerable<IBrowserFile> files)
    {
        var fileList = files.ToList();

        if (fileList.Count > ApplicationConstants.FileUpload.MaxFileCount)
        {
            return ValidationResult.Failure($"Zu viele Dateien (max. {ApplicationConstants.FileUpload.MaxFileCount})");
        }

        var errors = new List<string>();

        foreach (var file in fileList)
        {
            var result = ValidateFile(file);
            if (!result.IsValid)
            {
                errors.Add(result.ErrorMessage);
            }
        }

        return errors.Any()
            ? ValidationResult.Failure(string.Join(", ", errors))
            : ValidationResult.Success();
    }
}
