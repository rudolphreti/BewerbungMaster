namespace BewerbungMasterApp.Interfaces
{
    public interface IApplicationInitializationService
    {
        Task InitializeAsync(IServiceProvider serviceProvider);
    }
}