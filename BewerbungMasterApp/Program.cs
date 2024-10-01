using BewerbungMasterApp.Interfaces;
using BewerbungMasterApp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BewerbungMasterApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                builder.Configuration.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);

                var startupService = new StartupService();
                startupService.ConfigureServices(builder);

                builder.Services.AddSingleton<IStartupService>(startupService);

                var app = builder.Build();

                startupService.Configure(app);

                var initializationService = app.Services.GetRequiredService<IApplicationInitializationService>();
                await initializationService.InitializeAsync(app.Services);

                await app.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled exception: {ex}");
            }
        }
    }
}