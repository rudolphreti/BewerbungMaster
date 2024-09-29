using BewerbungMasterApp.Components;
using BewerbungMasterApp.Configuration;
using BewerbungMasterApp.Interfaces;

namespace BewerbungMasterApp.Services
{
    public class StartupService : IStartupService
    {
        public void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            ConfigureSignalR(builder);
            ConfigureBlazorServer(builder);
            ConfigureConfiguration(builder);
            RegisterApplicationServices(builder);
            ConfigureLogging(builder);
        }

        private void ConfigureSignalR(WebApplicationBuilder builder)
        {
            builder.Services.AddSignalR(options =>
            {
                options.ClientTimeoutInterval = TimeSpan.FromDays(1);
                options.KeepAliveInterval = TimeSpan.FromSeconds(10);
                options.HandshakeTimeout = TimeSpan.FromSeconds(30);
                options.MaximumReceiveMessageSize = 1024 * 1024; // 1 MB
            });
        }

        private void ConfigureBlazorServer(WebApplicationBuilder builder)
        {
            builder.Services.AddServerSideBlazor(options =>
            {
                options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromDays(1);
                options.DisconnectedCircuitMaxRetained = 100;
                options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(5);
                options.MaxBufferedUnacknowledgedRenderBatches = 100;
                options.DetailedErrors = true;
            });
        }

        private void ConfigureConfiguration(WebApplicationBuilder builder)
        {
            var appSettings = new AppSettings();
            builder.Configuration.Bind(appSettings);
            builder.Services.AddSingleton(appSettings);

            ValidateConfiguration(appSettings);
        }

        private void ValidateConfiguration(AppSettings appSettings)
        {
            var requiredConfigs = new[] { "UserDirectoryPath", "JobDataFile", "JobAppContentFile", "UserDataFile" };
            foreach (var configItem in requiredConfigs)
            {
                if (string.IsNullOrEmpty(appSettings.GetType().GetProperty(configItem)?.GetValue(appSettings) as string))
                {
                    throw new InvalidOperationException($"Missing required configuration: {configItem}");
                }
            }
        }

        private void RegisterApplicationServices(WebApplicationBuilder builder)
        {
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<IFileManagementService, FileManagementService>();
            builder.Services.AddSingleton<IPdfGenerationService, PdfGenerationService>();
            builder.Services.AddSingleton<IJsonService, JsonService>();
            builder.Services.AddSingleton<IApplicationInitializationService, ApplicationInitializationService>();
            builder.Services.AddSingleton<JobEditService>();
        }

        private void ConfigureLogging(WebApplicationBuilder builder)
        {
            builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            builder.Logging.SetMinimumLevel(LogLevel.Trace);
        }

        public async Task InitializeApplicationAsync(WebApplication app)
        {
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

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            var initializationService = app.Services.GetRequiredService<IApplicationInitializationService>();
            await initializationService.InitializeAsync(app.Services);
        }
    }
}