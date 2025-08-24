using Blazored.LocalStorage;
using EventMemoria.Web.Common.Constants;
using EventMemoria.Web.Services.Interfaces;

namespace EventMemoria.Web.Services;

public class UserPreferencesService(
    ILocalStorageService localStorage,
    ILogger<UserPreferencesService> logger) : IUserPreferencesService
{
    public async Task<string?> GetUserNameAsync()
    {
        try
        {
            return await localStorage.GetItemAsync<string>(ApplicationConstants.UserPreferences.StorageKey);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving user name from storage");
            return null;
        }
    }

    public async Task SetUserNameAsync(string userName)
    {
        try
        {
            await localStorage.SetItemAsync(ApplicationConstants.UserPreferences.StorageKey, userName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error setting user name to storage: {Error}", ex.Message);
        }
    }
}
