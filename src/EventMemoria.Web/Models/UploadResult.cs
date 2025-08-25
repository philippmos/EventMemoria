namespace EventMemoria.Web.Models;

public record UploadResult(bool IsSuccess, string FileName, string ErrorMessage = "")
{
    public static UploadResult Success(string fileName) => new(true, fileName);
    public static UploadResult Failure(string fileName, string errorMessage) => new(false, fileName, errorMessage);
}
