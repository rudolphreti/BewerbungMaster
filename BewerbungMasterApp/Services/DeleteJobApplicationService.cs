using System.Text.Json;
using BewerbungMasterApp.Interfaces;
using BewerbungMasterApp.Models;

namespace BewerbungMasterApp.Services
{
    public class DeleteJobApplicationService : IDeleteJobApplicationService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<DeleteJobApplicationService> _logger;

        public DeleteJobApplicationService(IWebHostEnvironment environment, ILogger<DeleteJobApplicationService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<bool> DeleteJobApplicationAsync(Guid id)
        {
            var jsonFilePath = Path.Combine(_environment.WebRootPath, "data.json");

            try
            {
                if (!File.Exists(jsonFilePath))
                {
                    _logger.LogError("Data file not found: {FilePath}", jsonFilePath);
                    return false;
                }

                var jsonData = await File.ReadAllTextAsync(jsonFilePath);
                var jobApplications = JsonSerializer.Deserialize<List<JobApplication>>(jsonData);

                var jobToRemove = jobApplications.FirstOrDefault(job => job.Id == id);
                if (jobToRemove == null)
                {
                    _logger.LogWarning("Job application with ID {JobId} not found.", id);
                    return false;
                }

                jobApplications.Remove(jobToRemove);

                var updatedJsonData = JsonSerializer.Serialize(jobApplications, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(jsonFilePath, updatedJsonData);

                _logger.LogInformation("Job application with ID {JobId} successfully deleted.", id);
                return true;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing or serializing JSON data.");
                return false;
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Error reading from or writing to the data file.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting job application.");
                return false;
            }
        }
    }
}