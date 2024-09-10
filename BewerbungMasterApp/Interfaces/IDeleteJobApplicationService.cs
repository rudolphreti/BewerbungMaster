namespace BewerbungMasterApp.Interfaces
{
    public interface IDeleteJobApplicationService
    {
        Task<bool> DeleteJobApplicationAsync(Guid id);
    }
}