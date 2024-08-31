using System.Collections.Generic;
using System.Threading.Tasks;
using BewerbungMasterApp.Models;

namespace BewerbungMasterApp.Interfaces
{
    public interface IFileManagementService
    {
        void InitializeJobAppDocsDirectory();
        void GenerateJobApplicationFolders(List<JobApplication> jobApplications);
    }
}
