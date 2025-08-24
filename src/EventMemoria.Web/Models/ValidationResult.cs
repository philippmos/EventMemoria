namespace EventMemoria.Web.Models;

public record ValidationResult(bool IsValid, string ErrorMessage = "")
{
    public static ValidationResult Success() => new(true);
    public static ValidationResult Failure(string errorMessage) => new(false, errorMessage);
}
