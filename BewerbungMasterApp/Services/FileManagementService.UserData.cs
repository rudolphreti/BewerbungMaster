using System.Text.Json;
using BewerbungMasterApp.Interfaces;
using BewerbungMasterApp.Models;

namespace BewerbungMasterApp.Services
{
    public partial class FileManagementService : IFileManagementService
    {
        private static async Task<User> LoadUserDataAsync(string userDirectoryPath)
        {
            if (string.IsNullOrWhiteSpace(userDirectoryPath))
                throw new InvalidOperationException("User directory path is invalid.");

            var userJsonPath = Path.Combine(userDirectoryPath, "user.json");

            if (!File.Exists(userJsonPath))
                throw new FileNotFoundException($"User data file not found: {userJsonPath}");

            var userJson = await File.ReadAllTextAsync(userJsonPath);
            return JsonSerializer.Deserialize<User>(userJson) ?? throw new InvalidOperationException("User data could not be loaded.");
        }
    }
}
