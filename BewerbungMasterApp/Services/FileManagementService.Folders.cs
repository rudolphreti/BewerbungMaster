using BewerbungMasterApp.Interfaces;
using BewerbungMasterApp.Models;
using BewerbungMasterApp.Services;

namespace BewerbungMasterApp.Services
{
    public partial class FileManagementService : IFileManagementService
    {
        public void GenerateJobApplicationFolders(List<JobApplication> jobApplications)
        {
            //var user = await LoadUserDataAsync(_userDirectoryPath);
            var uniqueFolderNames = FileManagementServiceStatic.CreateFolderNamesList(jobApplications);

            foreach (var folderName in uniqueFolderNames)
            {
                string targetDirectoryPath = Path.Combine(_jobAppDocsPath, folderName);
                Directory.CreateDirectory(targetDirectoryPath);

                string cvLapSubFolderPath = Path.Combine(targetDirectoryPath, "CV_LAP_separated");
                Directory.CreateDirectory(cvLapSubFolderPath);
            }
        }
    }
}
