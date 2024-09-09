using BewerbungMasterApp.Components;
using BewerbungMasterApp.Interfaces;
using BewerbungMasterApp.Services;
using static System.Net.Mime.MediaTypeNames;

namespace BewerbungMasterApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            var environment = builder.Environment.EnvironmentName;
            Console.WriteLine($"Current Environment: {environment}");

            // Set up configuration
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .Build();

            var userDirectoryPath = config["UserDirectoryPath"];
            Console.WriteLine("Configuration Loaded. UserDirectoryPath: " + (userDirectoryPath ?? "null or empty"));

            foreach (var key in config.AsEnumerable())
            {
                Console.WriteLine($"{key.Key}: {key.Value}");
            }

            // Register configuration
            builder.Services.AddSingleton<IConfiguration>(config);

            // Register HttpClient for use in services
            builder.Services.AddHttpClient();

            // Register application services
            builder.Services.AddSingleton<IGetJobApplicationsService, GetJobApplicationsService>();
            builder.Services.AddSingleton<IFileManagementService, FileManagementService>();
            builder.Services.AddSingleton<IPdfGenerationService, PdfGenerationService>();

            // Register the application logger
            builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
            builder.Logging.AddConsole();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAntiforgery();
            app.UseAuthorization();

            // Map Razor Components
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            // Initialize application
            await InitializeApplicationAsync(app);

            await app.RunAsync();

            
            

        }
        

        private static async Task InitializeApplicationAsync(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var fileManagementService = scope.ServiceProvider.GetRequiredService<IFileManagementService>();
                var jobApplicationsService = scope.ServiceProvider.GetRequiredService<IGetJobApplicationsService>();
                try
                {
                    // Initialize directories and fetch job applications
                    fileManagementService.InitializeJobAppDocsDirectory();
                    var jobApplications = await jobApplicationsService.GetJobApplicationsAsync();
                    Console.WriteLine($"Fetched {jobApplications.Count} job applications");

                    // Generate job application folders
                    await fileManagementService.GenerateJobApplicationSetsAsync(jobApplications);
                    Console.WriteLine("Job application sets generation completed");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred during initialization: {ex.Message}");
                    // Handle initialization exceptions if necessary
                    throw; // Re-throw the exception to stop the application if necessary
                }
            }
        }

        

    }
}