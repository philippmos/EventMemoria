using Vogelhochzeit.Common.Constants;
using Vogelhochzeit.Models;
using Vogelhochzeit.Services.Interfaces;

namespace Vogelhochzeit.Services;

public class PhotoGridService : IPhotoGridService
{
    public List<List<Photo>> CreatePhotoRows(IReadOnlyList<Photo> photos, int photosPerRow)
    {
        var rows = new List<List<Photo>>();

        for (int i = 0; i < photos.Count; i += photosPerRow)
        {
            var rowPhotos = photos.Skip(i).Take(photosPerRow).ToList();
            rows.Add(rowPhotos);
        }

        return rows;
    }

    public int GetRowHeight(int photosPerRow)
        => ApplicationConstants.PhotoGrid.PhotosPerRowHeights.GetValueOrDefault(photosPerRow, 250);

    public string GetPhotoContainerStyle(int photosPerRow)
    {
        var width = ApplicationConstants.PhotoGrid.PhotosPerRowWidths.GetValueOrDefault(photosPerRow, "calc(16.666% - 10px)");
        return $"width: {width}; min-width: {width}; flex-shrink: 0;";
    }

    public string GetShortFileName(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return string.Empty;
        }

        var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

        return nameWithoutExtension.Length > ApplicationConstants.Ui.FileNameTruncateLength
            ? $"{nameWithoutExtension[..ApplicationConstants.Ui.FileNameTruncateLength]}..."
            : nameWithoutExtension;
    }

    public bool IsValidPhotosPerRow(int photosPerRow)
        => photosPerRow >= ApplicationConstants.PhotoGrid.MinPhotosPerRow
        && photosPerRow <= ApplicationConstants.PhotoGrid.MaxPhotosPerRow;
}
