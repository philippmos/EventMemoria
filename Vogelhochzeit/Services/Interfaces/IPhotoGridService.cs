using Vogelhochzeit.Models;

namespace Vogelhochzeit.Services.Interfaces;

public interface IPhotoGridService
{
    List<List<Photo>> CreatePhotoRows(IReadOnlyList<Photo> photos, int photosPerRow);
    int GetRowHeight(int photosPerRow);
    string GetPhotoContainerStyle(int photosPerRow);
    string GetShortFileName(string fileName);
    bool IsValidPhotosPerRow(int photosPerRow);
}
