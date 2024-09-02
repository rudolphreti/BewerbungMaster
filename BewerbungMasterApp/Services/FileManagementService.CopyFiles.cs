using BewerbungMasterApp.Interfaces;
using BewerbungMasterApp.Models;

namespace BewerbungMasterApp.Services
{
    public partial class FileManagementService : IFileManagementService
    {
        public void CopyJobApplicationFiles(string targetDirectoryPath, string cvLapSubFolderPath, User user)
        {
            string[] filesToCopy = ["CV_Zeugnis.pdf", "profilfoto.jpg"];
            string[] filesToCopyToSubfolder = ["LAP-Zeugnis.pdf", "CV.pdf"];

            foreach (var fileName in filesToCopy)
            {
                string sourceFilePath = Path.Combine(_userDirectoryPath, fileName);
                string targetFilePath = Path.Combine(targetDirectoryPath, $"{user.FirstName}_{user.LastName}_{fileName}");
                if (File.Exists(sourceFilePath))
                {
                    File.Copy(sourceFilePath, targetFilePath, overwrite: true);
                }
            }

            foreach (var fileName in filesToCopyToSubfolder)
            {
                string sourceFilePath = Path.Combine(_userDirectoryPath, fileName);
                string targetFilePath = Path.Combine(cvLapSubFolderPath, $"{user.FirstName}_{user.LastName}_{fileName}");
                if (File.Exists(sourceFilePath))
                {
                    File.Copy(sourceFilePath, targetFilePath, overwrite: true);
                }
            }
        }
    }
}
