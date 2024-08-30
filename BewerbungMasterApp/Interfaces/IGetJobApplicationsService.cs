using System.Threading.Tasks;
using System.Collections.Generic;
using BewerbungMasterApp.Models;

namespace BewerbungMasterApp.Interfaces
{
    public interface IGetJobApplicationsService
    {
        Task<List<JobApplication>> GetJobApplicationsAsync();
    }
}