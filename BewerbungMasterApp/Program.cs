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

                var startupService = new StartupService();
                startupService.ConfigureServices(builder);

                var app = builder.Build();
                await startupService.InitializeApplicationAsync(app);

                await app.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled exception during startup: {ex}");
                // Hier könnte man auch ein Logging-Framework verwenden
            }
        }
    }
}