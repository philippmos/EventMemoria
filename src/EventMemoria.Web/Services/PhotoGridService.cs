using EventMemoria.Web.Common.Constants;
using EventMemoria.Web.Models;
using EventMemoria.Web.Services.Interfaces;

namespace EventMemoria.Web.Services;

public class PhotoGridService : IPhotoGridService
{
    public List<List<Photo>> CreatePhotoRows(IReadOnlyList<Photo> photos, int photosPerRow)
    {
        var rows = new List<List<Photo>>();

        for (var i = 0; i < photos.Count; i += photosPerRow)
        {
            var rowPhotos = photos.Skip(i).Take(photosPerRow).ToList();
            rows.Add(rowPhotos);
        }

        return rows;
    }

    public int GetRowHeight(int photosPerRow)
        => GetConfiguration(photosPerRow)?.Height ?? 250;

    public string GetPhotoContainerStyle(int photosPerRow)
    {
        var width = GetConfiguration(photosPerRow)?.Width;
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
        => photosPerRow
            is >= ApplicationConstants.PhotoGrid.MinPhotosPerRow
            and <= ApplicationConstants.PhotoGrid.MaxPhotosPerRow;

    private static PhotoGridConfiguration? GetConfiguration(int photosPerRow)
        => ApplicationConstants.PhotoGrid.Configuration.FirstOrDefault(x => x.Rows == photosPerRow);
}
