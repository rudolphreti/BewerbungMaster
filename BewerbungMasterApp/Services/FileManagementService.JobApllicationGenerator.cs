﻿using BewerbungMasterApp.Models;
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
                        _pdfGenerationService.GenerateCoverLetter(Path.Combine(targetDirectoryPath, fileName), user, application);
                        _logger.LogInformation($"PDF generated: {fileName} for folder: {uniqueFolderName}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error generating PDF: {fileName} for folder: {uniqueFolderName}");
                    }
                }
                else
                {
                    _logger.LogWarning($"Folder already exists: {uniqueFolderName}");
                }
            }
        }
    }
}