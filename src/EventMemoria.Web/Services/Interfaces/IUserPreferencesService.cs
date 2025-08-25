namespace EventMemoria.Web.Services.Interfaces;

public interface IUserPreferencesService
{
    Task<string?> GetUserNameAsync();
    Task<string?> SetUserNameAsync(string userName);
}
