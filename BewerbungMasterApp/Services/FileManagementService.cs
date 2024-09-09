using BewerbungMasterApp.Interfaces;
using BewerbungMasterApp.Models;

namespace BewerbungMasterApp.Services
{
    public partial class FileManagementService : IFileManagementService
    {
        private readonly string _jobAppDocsPath;
        private readonly string _userDirectoryPath;
        private readonly string _coverLetterTemplateName;

        public string JobAppDocsPath => _jobAppDocsPath; // don't understand; unit test need it
        public string UserDirectoryPath => _userDirectoryPath; // don't understand; unit test need it


        public FileManagementService(IConfiguration configuration, IWebHostEnvironment environment)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(environment);

            if (string.IsNullOrWhiteSpace(environment.WebRootPath))
                throw new InvalidOperationException("Web root path cannot be null or empty.");

            var userDirectoryPath = configuration["UserDirectoryPath"];
            if (string.IsNullOrWhiteSpace(userDirectoryPath))
                throw new InvalidOperationException("User directory path cannot be null or empty.");

            _jobAppDocsPath = Path.Combine(environment.WebRootPath, "JobAppDocs");
            _userDirectoryPath = Path.Combine(environment.WebRootPath, userDirectoryPath);
            _coverLetterTemplateName = configuration["CoverLetterTemplateName"] ?? "CoverLetterTemplate.html";
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
            var user = await GetUserData.GetUserDataAsync(_userDirectoryPath);
            var folderApplicationMap = CreateFolderApplicationMap(jobApplications);

            foreach (var (uniqueFolderName, application) in folderApplicationMap)
            {
                string targetDirectoryPath = Path.Combine(_jobAppDocsPath, uniqueFolderName);
                string cvLapSubFolderPath = Path.Combine(targetDirectoryPath, "CV_LAP_separated");

                FileManagementServiceStatic.CreateDirectories(targetDirectoryPath, cvLapSubFolderPath);

                CopyJobApplicationFiles(targetDirectoryPath, cvLapSubFolderPath, user);

                var fileName = $"{user.FirstName}_{user.LastName}_Bewerbungsschreiben.pdf";

                string templatePath = GetCoverLetterTemplatePath();

                try
                {
                    PdfGenerationService.GenerateCoverLetter(Path.Combine(targetDirectoryPath, fileName), user, application);
                    Console.WriteLine($"PDF generated: {fileName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error generating PDF: {ex.Message}");
                }
            }

            Console.WriteLine("GenerateJobApplicationSetsAsync completed");

            // Überprüfen Sie die Anzahl der generierten Ordner
            int generatedFolderCount = Directory.GetDirectories(_jobAppDocsPath).Length;
            if (generatedFolderCount != jobApplications.Count)
            {
                throw new InvalidOperationException($"Mismatch in generated folders. Expected: {jobApplications.Count}, Actual: {generatedFolderCount}");
            }
        }
        private string GetCoverLetterTemplatePath()
        {
            string templatePath = Path.Combine(_userDirectoryPath, _coverLetterTemplateName);
            if (File.Exists(templatePath))
            {
                return templatePath;
            }

            throw new FileNotFoundException($"Cover letter template not found at {templatePath}");
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
