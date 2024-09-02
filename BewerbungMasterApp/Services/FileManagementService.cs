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
        private readonly IPdfGenerationService _pdfGenerationService;


        public FileManagementService(IConfiguration configuration, IWebHostEnvironment environment, IPdfGenerationService pdfGenerationService)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(environment);
            ArgumentNullException.ThrowIfNull(pdfGenerationService);

            _pdfGenerationService = pdfGenerationService;



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

            // Create a mapping of unique folder names to job applications
            var folderApplicationMap = CreateFolderApplicationMap(jobApplications);

            foreach (var entry in folderApplicationMap)
            {
                string uniqueFolderName = entry.Key;
                JobApplication application = entry.Value;

                string targetDirectoryPath = Path.Combine(_jobAppDocsPath, uniqueFolderName);
                string cvLapSubFolderPath = Path.Combine(targetDirectoryPath, "CV_LAP_separated");

                // Create necessary directories
                FileManagementServiceStatic.CreateDirectories(targetDirectoryPath, cvLapSubFolderPath);

                // Copy job application files with user-specific names
                CopyJobApplicationFiles(targetDirectoryPath, cvLapSubFolderPath, user);

                // Generate the cover letter PDF
                var fileName = $"{user.FirstName}_{user.LastName}_Bewerbungsschreiben.pdf";

                await _pdfGenerationService.GenerateCoverLetterPdfAsync(targetDirectoryPath, fileName, user, application);
            }
        }

        // Helper method to create a mapping from unique folder names to job applications
        private static Dictionary<string, JobApplication> CreateFolderApplicationMap(List<JobApplication> jobApplications)
        {
            var folderApplicationMap = new Dictionary<string, JobApplication>();

            foreach (var application in jobApplications)
            {
                // Create a unique folder name based on the job application details
                string uniqueFolderName = FileManagementServiceStatic.CleanName($"{application.Company}_{application.Position}");

                // Ensure the folder name is unique
                uniqueFolderName = FileManagementServiceStatic.EnsureUniqueName(uniqueFolderName, folderApplicationMap.Keys.ToList());

                folderApplicationMap[uniqueFolderName] = application;
            }

            return folderApplicationMap;
        }

    }
}
