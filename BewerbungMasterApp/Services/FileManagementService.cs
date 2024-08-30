using System.Text.Json;
using BewerbungMasterApp.Models;
using BewerbungMasterApp.Interfaces;

namespace BewerbungMasterApp.Services
{
    public class FileManagementService : IFileManagementService
    {
        private readonly string _jobAppDocsPath;
        private readonly string _userDirectoryPath;

        public FileManagementService(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Console.WriteLine("Initializing FileManagementService..."); 
            

            // Use WebRootPath to get the absolute path to the wwwroot folder
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

            // Clear the JobAppDocs directory
            if (Directory.Exists(_jobAppDocsPath))
            {
                Directory.Delete(_jobAppDocsPath, true);
                Console.WriteLine($"Cleared existing directory: {_jobAppDocsPath}");
            }
            Directory.CreateDirectory(_jobAppDocsPath);
            Console.WriteLine($"Created new directory: {_jobAppDocsPath}");
        }

        public async Task GenerateJobApplicationFoldersAsync(List<JobApplication> jobApplications)
        {
            if (string.IsNullOrWhiteSpace(_userDirectoryPath))
                throw new InvalidOperationException("User directory path is invalid.");

            var userJsonPath = Path.Combine(_userDirectoryPath, "user.json");

            Console.WriteLine($"Looking for user data file at: {userJsonPath}");

            if (!File.Exists(userJsonPath))
                throw new FileNotFoundException($"User data file not found: {userJsonPath}");

            var userJson = await File.ReadAllTextAsync(userJsonPath);
            var user = JsonSerializer.Deserialize<User>(userJson) ?? throw new InvalidOperationException("User data could not be loaded.");

            Console.WriteLine($"Loaded user data successfully from {userJsonPath}");

            // List of unique folder names
            List<string> uniqueFolderNames = [];

            Console.WriteLine($"Total job applications to process: {jobApplications.Count}");

            for (int i = 0; i < jobApplications.Count; i++)
            {
                var application = jobApplications[i];
                string subFolderName = CleanName($"{application.Company}_{application.Position}");

                Console.WriteLine($"Processing JSON object {i + 1}: {application.Company} - {application.Position}");

                // Ensure the folder name is unique within the list
                subFolderName = EnsureUniqueFolderName(subFolderName, uniqueFolderNames);

                uniqueFolderNames.Add(subFolderName);

                Console.WriteLine($"Created unique folder name: {subFolderName}");
            }

            Console.WriteLine($"Creating directories based on unique folder names...");

            // Now create the actual folders based on the list of unique names
            foreach (var folderName in uniqueFolderNames)
            {
                string targetDirectoryPath = Path.Combine(_jobAppDocsPath, folderName);

                Directory.CreateDirectory(targetDirectoryPath);
                Console.WriteLine($"Created directory: {targetDirectoryPath}");

                // Create subfolder "CV_LAP_separated" in each job application folder
                string cvLapSubFolderPath = Path.Combine(targetDirectoryPath, "CV_LAP_separated");
                Directory.CreateDirectory(cvLapSubFolderPath);
                Console.WriteLine($"Path name: {Path.Combine(targetDirectoryPath, "CV_LAP_separated")}");
            }

            Console.WriteLine($"Successfully created {uniqueFolderNames.Count} folders for {jobApplications.Count} applications.");
        }

        private static string EnsureUniqueFolderName(string folderName, List<string> existingFolderNames)
        {
            int count = 1;
            string uniqueFolderName = folderName;

            // Add increment to the folder name if it already exists in the list
            while (existingFolderNames.Contains(uniqueFolderName))
            {
                uniqueFolderName = $"{folderName}_{count}";
                count++;
            }

            return uniqueFolderName;
        }

        private static string CleanName(string fileName)
        {
            // Trim leading and trailing whitespace
            fileName = fileName.Trim();

            // Replace invalid characters with an underscore
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                fileName = fileName.Replace(c, '_');
            }

            // Truncate the file name if it exceeds 100 characters
            if (fileName.Length > 100)
            {
                fileName = fileName[..100];
            }

            return fileName;
        }

    }
}
