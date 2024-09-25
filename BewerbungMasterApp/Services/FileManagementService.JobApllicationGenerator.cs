using BewerbungMasterApp.Models;
using BewerbungMasterApp.Interfaces;
namespace BewerbungMasterApp.Services
{
    public partial class FileManagementService : IFileManagementService
    {
        public async Task GenerateJobApplicationSetsAsync(List<JobApplication> jobApplications)
        {
            var user = await _jsonService.GetUserDataAsync();
            var folderApplicationMap = FileManagementServiceStatic.CreateFolderApplicationMap(jobApplications);

            foreach (var (uniqueFolderName, application) in folderApplicationMap)
            {
                string targetDirectoryPath = Path.Combine(_jobAppDocsPath, uniqueFolderName);
                string cvLapSubFolderPath = Path.Combine(targetDirectoryPath, "CV_LAP_separated");

                if (!Directory.Exists(targetDirectoryPath))
                {
                    FileManagementServiceStatic.CreateDirectories(targetDirectoryPath, cvLapSubFolderPath);
                    CopyJobApplicationFiles(targetDirectoryPath, cvLapSubFolderPath, user);

                    var fileName = $"{user.FirstName}_{user.LastName}_Bewerbungsschreiben.pdf";
                    try
                    {
                        var jobAppContent = await _jsonService.GetJobAppContentByNameAsync(application.Type);
                        if (jobAppContent != null)
                        {
                            _pdfGenerationService.GenerateCoverLetter(Path.Combine(targetDirectoryPath, fileName), user, application, jobAppContent);
                            _logger.LogInformation("PDF generated: {FileName} for folder: {FolderName}", fileName, uniqueFolderName);
                        }
                        else
                        {
                            _logger.LogWarning("JobAppContent not found for type: {Type}", application.Type);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error generating PDF: {FileName} for folder: {FolderName}", fileName, uniqueFolderName);
                    }
                }
                else
                {
                    _logger.LogWarning("Folder already exists: {FolderName}", uniqueFolderName);
                }
            }
        }
    }
}