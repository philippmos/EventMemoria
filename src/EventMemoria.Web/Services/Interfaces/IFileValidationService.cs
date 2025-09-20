using EventMemoria.Web.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace EventMemoria.Web.Services.Interfaces;

public interface IFileValidationService
{
    Task<ValidationResult> ValidateFileAsync(IBrowserFile file);
    ValidationResult ValidateMaxFileCount(IEnumerable<IBrowserFile> files);
}
