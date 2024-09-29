namespace BewerbungMasterApp.Interfaces
{
    public interface IStartupService
    {
        void ConfigureServices(WebApplicationBuilder builder);
        Task InitializeApplicationAsync(WebApplication app);
    }
}