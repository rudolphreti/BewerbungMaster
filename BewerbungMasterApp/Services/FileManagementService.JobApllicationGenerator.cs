using BewerbungMasterApp.Models;
using BewerbungMasterApp.Interfaces;

namespace BewerbungMasterApp.Services
{
    public partial class FileManagementService : IFileManagementService
    {
        public async Task GenerateJobApplicationSetsAsync(List<JobApplication> jobApplications)
        {
            var user = await GetUserData.GetUserDataAsync(_userDirectoryPath);
            var folderApplicationMap = FileManagementServiceStatic.CreateFolderApplicationMap(jobApplications);

            foreach (var (uniqueFolderName, application) in folderApplicationMap)
            {
                string targetDirectoryPath = Path.Combine(_jobAppDocsPath, uniqueFolderName);
                string cvLapSubFolderPath = Path.Combine(targetDirectoryPath, "CV_LAP_separated");

                FileManagementServiceStatic.CreateDirectories(targetDirectoryPath, cvLapSubFolderPath);

                CopyJobApplicationFiles(targetDirectoryPath, cvLapSubFolderPath, user);

                var fileName = $"{user.FirstName}_{user.LastName}_Bewerbungsschreiben.pdf";

                //string templatePath = GetCoverLetterTemplatePath();

                try
                {
                    _pdfGenerationService.GenerateCoverLetter(Path.Combine(targetDirectoryPath, fileName), user, application);
                    Console.WriteLine($"PDF generated: {fileName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error generating PDF: {ex.Message}");
                }
            }

            // Überprüfen Sie die Anzahl der generierten Ordner
            int generatedFolderCount = Directory.GetDirectories(_jobAppDocsPath).Length;
            if (generatedFolderCount != jobApplications.Count)
            {
                throw new InvalidOperationException($"Mismatch in generated folders. Expected: {jobApplications.Count}, Actual: {generatedFolderCount}");
            }
        }
    }
}
