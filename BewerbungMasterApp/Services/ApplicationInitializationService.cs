using BewerbungMasterApp.Interfaces;
using BewerbungMasterApp.Models;

namespace BewerbungMasterApp.Services
{
    public class ApplicationInitializationService(ILogger<ApplicationInitializationService> logger) : IApplicationInitializationService
    {
        private readonly ILogger<ApplicationInitializationService> _logger = logger;

        public async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var fileManagementService = scope.ServiceProvider.GetRequiredService<IFileManagementService>();
            var jsonService = scope.ServiceProvider.GetRequiredService<IJsonService>();

            try
            {
                // Initialize directories
                fileManagementService.InitializeJobAppDocsDirectory();

                // Fetch job applications
                var jobApplications = await jsonService.GetAllAsync<JobApplication>();
                _logger.LogInformation("Fetched {Count} job applications", jobApplications.Count);

                // Generate job application folders
                await fileManagementService.GenerateJobApplicationSetsAsync(jobApplications);
                _logger.LogInformation("Job application sets generation completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during initialization");
                throw; // Re-throw the exception to stop the application if necessary
            }
        }
    }
}