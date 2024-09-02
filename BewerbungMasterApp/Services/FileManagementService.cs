using BewerbungMasterApp.Interfaces;
using BewerbungMasterApp.Models;

namespace BewerbungMasterApp.Services
{
    public partial class FileManagementService : IFileManagementService
    {
        private readonly string _jobAppDocsPath;
        private readonly string _userDirectoryPath;

        public string JobAppDocsPath => _jobAppDocsPath; // don't understand; unit test need it
        public string UserDirectoryPath => _userDirectoryPath; // don't understand; unit test need it

        public FileManagementService(IConfiguration configuration, IWebHostEnvironment environment)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(environment);

            if (string.IsNullOrWhiteSpace(environment.WebRootPath))
                throw new InvalidOperationException("Web root path cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(configuration["UserDirectoryPath"]))
                throw new InvalidOperationException("User directory path cannot be null or empty.");

            _jobAppDocsPath = Path.Combine(environment.WebRootPath, "JobAppDocs");
            _userDirectoryPath = Path.Combine(environment.WebRootPath, configuration["UserDirectoryPath"].Trim());
        }




        public void InitializeJobAppDocsDirectory()
        {
            if (string.IsNullOrWhiteSpace(_jobAppDocsPath))
                throw new InvalidOperationException("JobAppDocs path is invalid.");

            if (Directory.Exists(_jobAppDocsPath))
            {
                Directory.Delete(_jobAppDocsPath, true);
            }

            Directory.CreateDirectory(_jobAppDocsPath);
        }

        public async Task GenerateJobApplicationSetsAsync(List<JobApplication> jobApplications)
        {
            // Load user data asynchronously
            var user = await FileManagementServiceStatic.LoadUserDataAsync(_userDirectoryPath);
            var uniqueFolderNames = FileManagementServiceStatic.CreateFolderNamesList(jobApplications);

            foreach (var folderName in uniqueFolderNames)
            {
                string targetDirectoryPath = Path.Combine(_jobAppDocsPath, folderName);
                string cvLapSubFolderPath = Path.Combine(targetDirectoryPath, "CV_LAP_separated");

                // Create necessary directories
                FileManagementServiceStatic.CreateDirectories(targetDirectoryPath, cvLapSubFolderPath);

                // Copy job application files with user-specific names
                CopyJobApplicationFiles(targetDirectoryPath, cvLapSubFolderPath, user);
            }
        }
    }
}
