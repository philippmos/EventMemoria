namespace EventMemoria.Web.Models;

public record UploadResult(bool IsSuccess, string FileName)
{
    public static UploadResult Success(string fileName) => new(true, fileName);
    public static UploadResult Failure(string fileName) => new(false, fileName);
}
