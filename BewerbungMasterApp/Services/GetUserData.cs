using System.Text.Json;
using BewerbungMasterApp.Models;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace BewerbungMasterApp.Services
{
    public static class GetUserData
    {
        public static async Task<User> GetUserDataAsync(string userDirectoryPath)
        {
            if (string.IsNullOrWhiteSpace(userDirectoryPath))
                throw new InvalidOperationException("User directory path is invalid.");

            var userJsonPath = Path.Combine(userDirectoryPath, "user.json");

            if (!File.Exists(userJsonPath))
                throw new FileNotFoundException($"User data file not found: {userJsonPath}");

            var userJson = await File.ReadAllTextAsync(userJsonPath);

            // Configure JsonSerializer options to handle special characters correctly
            JsonSerializerOptions jsonSerializerOptions = new()
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All), // Ensure all characters are encoded properly
                PropertyNameCaseInsensitive = true // Optional, depending on your JSON structure
            };
            JsonSerializerOptions options = jsonSerializerOptions;

            return JsonSerializer.Deserialize<User>(userJson, options) ?? throw new InvalidOperationException("User data could not be loaded.");
        }
    }
}
