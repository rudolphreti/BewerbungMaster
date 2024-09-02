using BewerbungMasterApp.Components;
using BewerbungMasterApp.Interfaces;
using BewerbungMasterApp.Services;

namespace BewerbungMasterApp
{
    public class Program
    {
        public static void Main(string[] args)
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
            builder.Services.AddSingleton<IPdfGenerationService, PdfGenerationService>();
            builder.Services.AddSingleton<IFileManagementService, FileManagementService>();

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


            using (var scope = app.Services.CreateScope())
            {
                var fileManagementService = scope.ServiceProvider.GetRequiredService<IFileManagementService>();
                var jobApplicationsService = scope.ServiceProvider.GetRequiredService<IGetJobApplicationsService>();

                try
                {
                    // Initialize directories and fetch job applications
                    fileManagementService.InitializeJobAppDocsDirectory();
                    var jobApplications = jobApplicationsService.GetJobApplicationsAsync().Result;

                    // Generate job application folders
                    fileManagementService.GenerateJobApplicationSetsAsync(jobApplications); // with .Wait()? 
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred during initialization: {ex.Message}");
                    // Handle initialization exceptions if necessary
                }
            }

            app.Run();
        }
    }
}
