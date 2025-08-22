namespace Vogelhochzeit.Models;

public class Photo
{
    public string Id { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; }
    public long FileSize { get; set; }
    public string? Description { get; set; }
    public string? Alt { get; set; }
    public bool IsLoaded { get; set; } = true;
    
    public string FormattedFileSize
    {
        get
        {
            if (FileSize < 1024)
            {
                return $"{FileSize} B";
            }
                
            if (FileSize < 1024 * 1024)
            {
                return $"{FileSize / 1024:F1} KB";
            }

            return $"{FileSize / (1024 * 1024):F1} MB";
        }
    }
}
