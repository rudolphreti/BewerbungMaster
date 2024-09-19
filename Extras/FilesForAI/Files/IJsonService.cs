using BewerbungMasterApp.Models;

namespace BewerbungMasterApp.Services
{
    public interface IJsonService
    {
        Task<List<T>> GetAllAsync<T>();
        Task<T> GetByIdAsync<T>(Guid id) where T : class, new();
        Task<T> AddAsync<T>(T item) where T : class, new();
        Task<T> UpdateAsync<T>(T item) where T : class, new();
        Task<bool> DeleteAsync<T>(Guid id) where T : class, new();
        Task UpdateAllAsync<T>(List<T> items) where T : class, new();
        Task<User> GetUserDataAsync();
    }
}