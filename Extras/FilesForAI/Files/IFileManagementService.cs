using System.Collections.Generic;
using System.Threading.Tasks;
using BewerbungMasterApp.Models;

namespace BewerbungMasterApp.Interfaces
{
    public interface IFileManagementService
    {
        void InitializeJobAppDocsDirectory();
        Task GenerateJobApplicationSetsAsync(List<JobApplication> jobApplications);
        void CopyJobApplicationFiles(string targetDirectoryPath, string cvLapSubFolderPath, User user);
    }
}
