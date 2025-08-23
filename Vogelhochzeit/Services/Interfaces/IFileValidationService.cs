using Microsoft.AspNetCore.Components.Forms;
using Vogelhochzeit.Models;

namespace Vogelhochzeit.Services.Interfaces;

public interface IFileValidationService
{
    ValidationResult ValidateFile(IBrowserFile file);
    ValidationResult ValidateFiles(IEnumerable<IBrowserFile> files);
}
