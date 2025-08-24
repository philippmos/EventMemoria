namespace Vogelhochzeit.Common.Constants;

public static class ApplicationConstants
{
    public static class FileUpload
    {
        public const long MaxFileSizeInBytes = 50 * 1024 * 1024; // 50MB
        public const int MaxFileCount = 50;
    }

    public static class ImageTags
    {
        public const string Author = "Author";
        public const string FileName = "FileName";
        public const string UploadedAt = "UploadedAt";
    }

    public static class Pagination
    {
        public const int DefaultPageSize = 24;
        public const int MaxPageSize = 100;
        public const int VirtualizationOverscanCount = 3;
    }

    public static class PhotoGrid
    {
        public const int MinPhotosPerRow = 2;
        public const int MaxPhotosPerRow = 12;

        public static readonly Dictionary<int, int> PhotosPerRowHeights = new()
        {
            { 2, 400 },
            { 4, 300 },
            { 6, 250 },
            { 8, 200 },
            { 12, 150 }
        };

        public static readonly Dictionary<int, string> PhotosPerRowWidths = new()
        {
            { 2, "calc(50% - 9px)" },
            { 4, "calc(25% - 9px)" },
            { 6, "calc(16.666% - 10px)" },
            { 8, "calc(12.5% - 10.5px)" },
            { 12, "calc(8.333% - 11px)" }
        };
    }

    public static class Ui
    {
        public const string GridContainerHeight = "70vh";
        public const int PhotoCardOffset = 60;
        public const int FileNameTruncateLength = 15;
    }

    public static class UserPreferences
    {
        public const string StorageKey = "author_name";
    }
}
