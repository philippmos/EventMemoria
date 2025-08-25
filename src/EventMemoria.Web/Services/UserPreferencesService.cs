using Blazored.LocalStorage;
using EventMemoria.Web.Common.Constants;
using EventMemoria.Web.Helpers;
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
            var storageValue = await localStorage.GetItemAsync<string>(ApplicationConstants.UserPreferences.StorageKey);

            return storageValue is null
                ? null
                : SanitizingHelper.SanitizeValue(storageValue);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving user name from storage");
            return null;
        }
    }

    public async Task<string?> SetUserNameAsync(string userName)
    {
        try
        {
            var sanitizedUserName = SanitizingHelper.SanitizeValue(userName);
            await localStorage.SetItemAsync(ApplicationConstants.UserPreferences.StorageKey, sanitizedUserName);
            return sanitizedUserName;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error setting user name to storage: {Error}", ex.Message);
            return null;
        }
    }
}
