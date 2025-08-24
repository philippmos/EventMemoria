using Vogelhochzeit.Models;

namespace Vogelhochzeit.Services.Interfaces;

public interface IStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string? contentType = null, string? author = null);
    Task<PagedResult<Photo>> GetPhotosPagedAsync(int page = 1, int pageSize = 24);
}
