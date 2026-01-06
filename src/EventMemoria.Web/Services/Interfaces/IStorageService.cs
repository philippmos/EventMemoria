using EventMemoria.Web.Models;

namespace EventMemoria.Web.Services.Interfaces;

public interface IStorageService
{
    Task<string> UploadThumbnailAsync(Stream fileStream, string fileName, string? contentType = null, string? author = null);
    Task<string> UploadFullSizeAsync(Stream fileStream, string fileName, string? contentType = null, string? author = null);
    Task<string> UploadVideoAsync(Stream fileStream, string fileName, string? contentType = null, string? author = null);
    Task<PagedResult<Photo>> GetPhotosPagedAsync(int page = 1, int pageSize = 24, string? folderName = null);
    Task<IEnumerable<string>> GetGallerySubFoldersAsync();
}
