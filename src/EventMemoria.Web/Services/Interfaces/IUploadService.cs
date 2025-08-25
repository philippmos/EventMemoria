using EventMemoria.Web.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace EventMemoria.Web.Services.Interfaces;

public interface IUploadService
{
    Task<UploadResult> ProcessFileAsync(IBrowserFile file, string? userName);
}
