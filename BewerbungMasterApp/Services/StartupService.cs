using BewerbungMasterApp.Components;
using BewerbungMasterApp.Interfaces;
using BewerbungMasterApp.Services;

namespace BewerbungMasterApp.Services
{
    public class StartupService : IStartupService
    {
        public void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            ConfigureSignalR(builder.Services);
            ConfigureBlazorServer(builder.Services);
            ConfigureConfiguration(builder);
            RegisterApplicationServices(builder.Services);
            ConfigureLogging(builder.Logging, builder.Configuration);
        }

        private static void ConfigureSignalR(IServiceCollection services)
        {
            services.AddSignalR(options =>
            {
                options.ClientTimeoutInterval = TimeSpan.FromDays(1);
                options.KeepAliveInterval = TimeSpan.FromSeconds(10);
                options.HandshakeTimeout = TimeSpan.FromSeconds(30);
                options.MaximumReceiveMessageSize = 1024 * 1024; // 1 MB
            });
        }

        private static void ConfigureBlazorServer(IServiceCollection services)
        {
            services.AddServerSideBlazor(options =>
            {
                options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromDays(1);
                options.DisconnectedCircuitMaxRetained = 100;
                options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(5);
                options.MaxBufferedUnacknowledgedRenderBatches = 100;
                options.DetailedErrors = true;
            });
        }

        private static void ConfigureConfiguration(WebApplicationBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .Build();

            builder.Services.AddSingleton<IConfiguration>(config);

            ValidateConfiguration(config);
        }

        private static void ValidateConfiguration(IConfiguration config)
        {
            var requiredConfigs = new[] { "UserDirectoryPath", "JobDataFile", "JobAppContentFile", "UserDataFile" };
            foreach (var configItem in requiredConfigs)
            {
                if (string.IsNullOrEmpty(config[configItem]))
                {
                    throw new InvalidOperationException($"Missing required configuration: {configItem}");
                }
            }
        }

        private static void RegisterApplicationServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddSingleton<IFileManagementService, FileManagementService>();
            services.AddSingleton<IPdfGenerationService, PdfGenerationService>();
            services.AddSingleton<IJsonService, JsonService>();
            services.AddSingleton<IApplicationInitializationService, ApplicationInitializationService>();
            services.AddSingleton<JobEditService>();
        }

        private static void ConfigureLogging(ILoggingBuilder logging, IConfiguration configuration)
        {
            logging.AddConfiguration(configuration.GetSection("Logging"));
            logging.AddConsole();
            logging.AddDebug();
            logging.SetMinimumLevel(LogLevel.Trace);
        }

        public void Configure(WebApplication app)
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
        }
    }
}