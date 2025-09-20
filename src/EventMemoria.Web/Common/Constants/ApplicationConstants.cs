using ImageMagick;

namespace EventMemoria.Web.Common.Constants;

public static class ApplicationConstants
{
    public static class FileUpload
    {
        public const long MaxFileSizeInBytes = 100 * 1024 * 1024; // 100MB
        public const long MaxVideoFileSizeInBytes = 1 * 1024 * 1024 * 1024; // 1GB
        public const int MaxFileCountNative = 1000;
        public const int MaxFileCount = 100;

        public static class Thumbnail
        {
            public const int ResizeWidth = 700;
            public const int ResizeHeight = 700;
            public const int Quality = 85;
            public const MagickFormat Format = MagickFormat.Jpeg;
        }
    }

    public static class ImageTags
    {
        public const string Author = "Author";
        public const string FileName = "FileName";
        public const string UploadedAt = "UploadedAt";
        public const string MediaType = "MediaType";
    }

    public static class Pagination
    {
        public const int DefaultPageSize = 24;
        public const int MaxPageSize = 500;
        public const int VirtualizationOverscanCount = 3;
    }

    public static class PhotoGrid
    {
        public const int MinPhotosPerRow = 2;
        public const int MaxPhotosPerRow = 12;

        public static readonly List<PhotoGridConfiguration> Configuration =
        [
            new (2, 400, "calc(50% - 9px)" ),
            new (4, 300, "calc(25% - 9px)" ),
            new (6, 250, "calc(16.666% - 10px)" ),
            new (8, 200, "calc(12.5% - 10.5px)" ),
            new (12, 150, "calc(8.333% - 11px)" )
        ];
    }

    public static class Ui
    {
        public const int PhotoCardOffset = 60;
        public const int FileNameTruncateLength = 15;
        public const string DateFormat = "dd.MM.yyyy HH:mm";
    }

    public static class UserPreferences
    {
        public const string StorageKey = "author_name";
        public const int UserNameMinLength = 3;
    }
}

public record PhotoGridConfiguration(int Rows, int Height, string Width);
