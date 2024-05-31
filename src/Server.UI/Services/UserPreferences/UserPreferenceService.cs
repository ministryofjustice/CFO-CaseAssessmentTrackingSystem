using System.Security.Cryptography;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Cfo.Cats.Server.UI.Services.UserPreferences;

public class UserPreferencesService : IUserPreferencesService
{
    private const string Key = "userPreferences";
    private readonly ProtectedLocalStorage localStorage;

    public UserPreferencesService(ProtectedLocalStorage localStorage)
    {
        this.localStorage = localStorage;
    }

    public async Task SaveUserPreferences(UserPreferences userPreferences)
    {
        await localStorage.SetAsync(Key, userPreferences);
    }

    public async Task<UserPreferences> LoadUserPreferences()
    {
        try
        {
            var result = await localStorage.GetAsync<UserPreferences>(Key);
            if (result is { Success: true, Value: not null })
            {
                return result.Value;
            }

            return new UserPreferences();
        }
        catch (CryptographicException)
        {
            await localStorage.DeleteAsync(Key);
            return new UserPreferences();
        }
        catch (Exception)
        {
            return new UserPreferences();
        }
    }
}
