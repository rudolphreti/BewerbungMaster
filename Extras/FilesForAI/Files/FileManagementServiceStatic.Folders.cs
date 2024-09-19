using BewerbungMasterApp.Models;

namespace BewerbungMasterApp.Services
{
    public partial class FileManagementServiceStatic
    {
        internal static List<string> CreateFolderNamesList(List<JobApplication> jobApplications)
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

        internal static void CreateDirectories(string targetDirectoryPath, string cvLapSubFolderPath) // to rename, "folders" not "directory"
        {
            Directory.CreateDirectory(targetDirectoryPath);
            Directory.CreateDirectory(cvLapSubFolderPath);
        }

        // Helper method to create a mapping from unique folder names to job applications
        internal static Dictionary<string, JobApplication> CreateFolderApplicationMap(List<JobApplication> jobApplications)
        {
            var folderApplicationMap = new Dictionary<string, JobApplication>();

            foreach (var application in jobApplications)
            {
                // Create a unique folder name based on the job application details
                string uniqueFolderName = FileManagementServiceStatic.CleanName($"{application.Company}_{application.Position}");

                // Ensure the folder name is unique
                uniqueFolderName = FileManagementServiceStatic.EnsureUniqueName(uniqueFolderName, [.. folderApplicationMap.Keys]);

                folderApplicationMap[uniqueFolderName] = application;
            }

            return folderApplicationMap;
        }
    }
}