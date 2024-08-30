using BewerbungMasterApp.Models;

namespace BewerbungMasterApp.Services
{
    public partial class FileManagementService
    {
        private static List<string> CreateFolderNamesList(List<JobApplication> jobApplications)
        {
            List<string> uniqueFolderNames = [];

            for (int i = 0; i < jobApplications.Count; i++)
            {
                var application = jobApplications[i];
                string subFolderName = CleanName($"{application.Company}_{application.Position}");

                subFolderName = EnsureUniqueName(subFolderName, uniqueFolderNames);
                uniqueFolderNames.Add(subFolderName);
            }

            return uniqueFolderNames;
        }

        public async Task GenerateJobApplicationFoldersAsync(List<JobApplication> jobApplications)
        {
            var user = await LoadUserDataAsync(_userDirectoryPath);
            var uniqueFolderNames = CreateFolderNamesList(jobApplications);

            foreach (var folderName in uniqueFolderNames)
            {
                string targetDirectoryPath = Path.Combine(_jobAppDocsPath, folderName);
                Directory.CreateDirectory(targetDirectoryPath);
                Console.WriteLine($"Created directory: {targetDirectoryPath}");

                string cvLapSubFolderPath = Path.Combine(targetDirectoryPath, "CV_LAP_separated");
                Directory.CreateDirectory(cvLapSubFolderPath);
                Console.WriteLine($"Path name: {Path.Combine(targetDirectoryPath, "CV_LAP_separated")}");
            }

            Console.WriteLine($"Successfully created {uniqueFolderNames.Count} folders for {jobApplications.Count} applications.");
        }
    }
}
