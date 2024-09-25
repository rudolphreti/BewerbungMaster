using BewerbungMasterApp.Components;
using BewerbungMasterApp.Interfaces;
using BewerbungMasterApp.Services;

namespace BewerbungMasterApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);
                // Add services to the container
                builder.Services.AddRazorComponents()
                    .AddInteractiveServerComponents();

                // Configure SignalR for indefinite connection
                builder.Services.AddSignalR(options =>
                {
                    options.ClientTimeoutInterval = TimeSpan.FromDays(1);
                    options.KeepAliveInterval = TimeSpan.FromSeconds(10);
                    options.HandshakeTimeout = TimeSpan.FromSeconds(30);
                    options.MaximumReceiveMessageSize = 1024 * 1024; // 1 MB
                });

                // Configure Blazor Server options
                builder.Services.AddServerSideBlazor(options =>
                {
                    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromDays(1);
                    options.DisconnectedCircuitMaxRetained = 100;
                    options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(5);
                    options.MaxBufferedUnacknowledgedRenderBatches = 100;
                    options.DetailedErrors = true; // Enable detailed errors for development
                });

                // Set up configuration
                var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .Build();

                // Ensure all necessary configurations are loaded
                var requiredConfigs = new[] { "UserDirectoryPath", "JobDataFile", "JobAppContentFile", "UserDataFile" };
                foreach (var configItem in requiredConfigs)
                {
                    if (string.IsNullOrEmpty(builder.Configuration[configItem]))
                    {
                        throw new InvalidOperationException($"Missing required configuration: {configItem}");
                    }
                }

                // Register configuration
                builder.Services.AddSingleton<IConfiguration>(config);

                // Register HttpClient for use in services
                builder.Services.AddHttpClient();

                // TODO: create a service (with a method with services and logging instructions)
                // Register application services
                builder.Services.AddSingleton<IFileManagementService, FileManagementService>();
                builder.Services.AddSingleton<IPdfGenerationService, PdfGenerationService>();
                builder.Services.AddSingleton<IJsonService, JsonService>();
                builder.Services.AddSingleton<IApplicationInitializationService, ApplicationInitializationService>();
                builder.Services.AddSingleton<JobEditService>();

                // Register the application logger
                builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
                builder.Logging.AddConsole();
                builder.Logging.AddDebug();
                builder.Logging.SetMinimumLevel(LogLevel.Trace);  // Fügen Sie diese Zeile hinzu



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
                var initializationService = app.Services.GetRequiredService<IApplicationInitializationService>();
                await initializationService.InitializeAsync(app.Services);

                await app.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled exception: {ex}");
                // Optionally log the exception
            }
            
        }

        
    }
}