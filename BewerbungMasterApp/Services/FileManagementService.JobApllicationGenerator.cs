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

            HashSet<string> expectedFolders = new HashSet<string>();
            HashSet<string> createdFolders = new HashSet<string>();

            _logger.LogInformation($"Total job applications: {jobApplications.Count}");
            _logger.LogInformation($"Folder application map count: {folderApplicationMap.Count}");

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

                    createdFolders.Add(uniqueFolderName);
                }
                else
                {
                    _logger.LogWarning($"Folder already exists: {uniqueFolderName}");
                }

                expectedFolders.Add(uniqueFolderName);
            }

            var allFoldersInDirectory = new HashSet<string>(Directory.GetDirectories(_jobAppDocsPath).Select(Path.GetFileName));

            _logger.LogInformation($"Total folders in directory: {allFoldersInDirectory.Count}");
            _logger.LogInformation($"Total folders created in this run: {createdFolders.Count}");
            _logger.LogInformation($"Total expected folders: {expectedFolders.Count}");

            var extraFolders = allFoldersInDirectory.Except(expectedFolders).ToList();
            if (extraFolders.Any())
            {
                _logger.LogWarning($"Extra folders found: {string.Join(", ", extraFolders)}");
                foreach (var extraFolder in extraFolders)
                {
                    var folderPath = Path.Combine(_jobAppDocsPath, extraFolder);
                    var folderCreationTime = Directory.GetCreationTime(folderPath);
                    var folderContent = Directory.GetFileSystemEntries(folderPath);
                    _logger.LogWarning($"Extra folder details - Name: {extraFolder}, Creation Time: {folderCreationTime}, Content Count: {folderContent.Length}");
                }
            }

            var missingFolders = expectedFolders.Except(allFoldersInDirectory).ToList();
            if (missingFolders.Any())
            {
                _logger.LogWarning($"Missing folders: {string.Join(", ", missingFolders)}");
            }

            if (allFoldersInDirectory.Count != jobApplications.Count)
            {
                _logger.LogWarning($"Mismatch in folder count. Expected: {jobApplications.Count}, Actual: {allFoldersInDirectory.Count}");
            }
        }
    }
}
