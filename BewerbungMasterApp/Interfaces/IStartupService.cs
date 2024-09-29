namespace BewerbungMasterApp.Interfaces
{
    public interface IStartupService
    {
        void ConfigureServices(WebApplicationBuilder builder);
        void Configure(WebApplication app);
    }
}