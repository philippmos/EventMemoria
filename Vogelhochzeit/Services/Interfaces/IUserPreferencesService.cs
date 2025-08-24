namespace Vogelhochzeit.Services.Interfaces;

public interface IUserPreferencesService
{
    Task<string?> GetUserNameAsync();
    Task SetUserNameAsync(string userName);
}
