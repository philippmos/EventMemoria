using EventMemoria.Web.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace EventMemoria.Web.Services.Interfaces;

public interface IFileValidationService
{
    ValidationResult ValidateFile(IBrowserFile file);
    ValidationResult ValidateFiles(IEnumerable<IBrowserFile> files);
}
