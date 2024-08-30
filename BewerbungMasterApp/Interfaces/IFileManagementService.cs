using System.Collections.Generic;
using System.Threading.Tasks;
using BewerbungMasterApp.Models;

namespace BewerbungMasterApp.Interfaces
{
    public interface IFileManagementService
    {
        void InitializeJobAppDocsDirectory();
        Task GenerateJobApplicationFoldersAsync(List<JobApplication> jobApplications);
    }
}
