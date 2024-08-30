using System.Text.Json;
using BewerbungMasterApp.Models;
using BewerbungMasterApp.Interfaces;

namespace BewerbungMasterApp.Services
{
    public partial class FileManagementService : IFileManagementService
    {
        private readonly string _jobAppDocsPath;
        private readonly string _userDirectoryPath;

        public FileManagementService(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Console.WriteLine("Initializing FileManagementService...");

            _jobAppDocsPath = Path.Combine(environment.WebRootPath, "JobAppDocs");
            _userDirectoryPath = Path.Combine(environment.WebRootPath, configuration["UserDirectoryPath"]?.Trim() ??
                throw new InvalidOperationException("User directory path is not configured properly."));

            if (string.IsNullOrWhiteSpace(_userDirectoryPath))
                throw new InvalidOperationException("User directory path cannot be null or empty.");
        }

        public void InitializeJobAppDocsDirectory()
        {
            if (string.IsNullOrWhiteSpace(_jobAppDocsPath))
                throw new InvalidOperationException("JobAppDocs path is invalid.");

            if (Directory.Exists(_jobAppDocsPath))
            {
                Directory.Delete(_jobAppDocsPath, true);
                Console.WriteLine($"Cleared existing directory: {_jobAppDocsPath}");
            }

            Directory.CreateDirectory(_jobAppDocsPath);
            Console.WriteLine($"Created new directory: {_jobAppDocsPath}");
        }
    }
}
