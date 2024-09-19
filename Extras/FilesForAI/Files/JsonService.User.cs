using System.Text.Json;
using BewerbungMasterApp.Models;

namespace BewerbungMasterApp.Services
{
    public partial class JsonService
    {
        // User-specific methods
        public async Task<User> GetUserDataAsync()
        {
            if (!File.Exists(_userDataFilePath))
            {
                throw new FileNotFoundException($"User data file not found: {_userDataFilePath}");
            }

            var userJson = await File.ReadAllTextAsync(_userDataFilePath);
            return JsonSerializer.Deserialize<User>(userJson, _jsonOptions)
                ?? throw new InvalidOperationException("User data could not be loaded.");
        }

        public async Task UpdateUserDataAsync(User user)
        {
            var jsonString = JsonSerializer.Serialize(user, _jsonOptions);
            await File.WriteAllTextAsync(_userDataFilePath, jsonString);
        }
    }
}