using BewerbungMasterApp.Models;

namespace BewerbungMasterApp.Services
{
    public partial class FileManagementServiceStatic
    {
        public static List<string> CreateFolderNamesList(List<JobApplication> jobApplications)
        {
            List<string> uniqueFolderNames = [];

            for (int i = 0; i < jobApplications.Count; i++)
            {
                var application = jobApplications[i];
                string subFolderName = CleanName($"{application.Company}_{application.Position}");

                subFolderName = EnsureUniqueName(subFolderName, uniqueFolderNames);
                uniqueFolderNames.Add(subFolderName);
            }

            return uniqueFolderNames;
        }
    }
}
