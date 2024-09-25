using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using BewerbungMasterApp.Models;

namespace BewerbungMasterApp.Services
{
    public partial class JsonService : IJsonService
    {
        private readonly ILogger<JsonService> _logger;
        private readonly string _jobDataFilePath;
        private readonly string _userDataFilePath;
        private readonly string _jobAppContentFilePath;
        private readonly JsonSerializerOptions _jsonOptions;

        public JsonService(ILogger<JsonService> logger, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _logger = logger;
            var userDirectoryPath = configuration["UserDirectoryPath"] ?? throw new InvalidOperationException("UserDirectoryPath configuration is missing");
            var jobDataFileName = configuration["JobDataFile"] ?? throw new InvalidOperationException("JobDataFile configuration is missing");
            var userDataFileName = configuration["UserDataFile"] ?? throw new InvalidOperationException("UserDataFile configuration is missing");
            var jobAppContentFileName = configuration["JobAppContentFile"] ?? throw new InvalidOperationException("JobAppContentFile configuration is missing");

            var fullUserDirectoryPath = Path.Combine(environment.WebRootPath, userDirectoryPath);

            _jobDataFilePath = Path.Combine(fullUserDirectoryPath, Path.GetFileName(jobDataFileName));
            _userDataFilePath = Path.Combine(fullUserDirectoryPath, userDataFileName);
            _jobAppContentFilePath = Path.Combine(fullUserDirectoryPath, jobAppContentFileName);

            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                PropertyNameCaseInsensitive = true
            };

            _logger.LogInformation($"JobDataFilePath: {_jobDataFilePath}");
            _logger.LogInformation($"UserDataFilePath: {_userDataFilePath}");
            _logger.LogInformation($"JobAppContentFilePath: {_jobAppContentFilePath}");
        }

        private async Task<List<T>> ReadJsonFileAsync<T>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return [];
            }

            var jsonString = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<List<T>>(jsonString, _jsonOptions) ?? [];
        }

        private async Task WriteJsonFileAsync<T>(string filePath, List<T> items)
        {
            var jsonString = JsonSerializer.Serialize(items, _jsonOptions);
            await File.WriteAllTextAsync(filePath, jsonString);
        }

        public async Task<List<T>> GetAllAsync<T>()
        {
            try
            {
                return await ReadJsonFileAsync<T>(_jobDataFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading JSON file");
                throw;
            }
        }

        public async Task<T> GetByIdAsync<T>(Guid id) where T : class, new()
        {
            var items = await ReadJsonFileAsync<T>(_jobDataFilePath);
            return items.Find(item => (item as dynamic).Id == id) ?? new T();
        }

        public async Task<T> AddAsync<T>(T item) where T : class, new()
        {
            var items = await ReadJsonFileAsync<T>(_jobDataFilePath);
            items.Add(item);
            await WriteJsonFileAsync(_jobDataFilePath, items);
            return item;
        }

        public async Task<T> AddAsFirstAsync<T>(T item) where T : class, new()
        {
            var items = await ReadJsonFileAsync<T>(_jobDataFilePath);
            items.Insert(0, item);
            await WriteJsonFileAsync(_jobDataFilePath, items);
            return item;
        }

        public async Task<T> UpdateAsync<T>(T item) where T : class, new()
        {
            var items = await ReadJsonFileAsync<T>(_jobDataFilePath);
            var index = items.FindIndex(x => (x as dynamic).Id == (item as dynamic).Id);
            if (index != -1)
            {
                items[index] = item;
                await WriteJsonFileAsync(_jobDataFilePath, items);
                return item;
            }
            throw new KeyNotFoundException($"Item with ID {(item as dynamic).Id} not found.");
        }

        public async Task UpdateAllAsync<T>(List<T> items) where T : class, new()
        {
            try
            {
                await WriteJsonFileAsync(_jobDataFilePath, items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating all items in JSON file");
                throw;
            }
        }

        public async Task<bool> DeleteAsync<T>(Guid id) where T : class, new()
        {
            var items = await ReadJsonFileAsync<T>(_jobDataFilePath);
            var removed = items.RemoveAll(x => (x as dynamic).Id == id);
            if (removed > 0)
            {
                await WriteJsonFileAsync(_jobDataFilePath, items);
                return true;
            }
            return false;
        }

        public async Task<List<JobAppContent>> GetAllJobAppContentsAsync()
        {
            try
            {
                var contents = await ReadJsonFileAsync<JobAppContent>(_jobAppContentFilePath);
                _logger.LogInformation($"Loaded {contents.Count} JobAppContents");
                return contents;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading JobAppContents");
                return new List<JobAppContent>();
            }
        }

        public async Task<JobAppContent?> GetJobAppContentByNameAsync(string name)
        {
            var contents = await GetAllJobAppContentsAsync();
            return contents.FirstOrDefault(c => c.Name == name);
        }
    }
}