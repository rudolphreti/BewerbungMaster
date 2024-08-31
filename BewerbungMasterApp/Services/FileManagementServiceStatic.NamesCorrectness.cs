using BewerbungMasterApp.Interfaces;

namespace BewerbungMasterApp.Services
{
    public partial class FileManagementServiceStatic
    {
        private static string CleanName(string fileName)
        {
            fileName = fileName.Trim();

            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                fileName = fileName.Replace(c, '_');
            }

            if (fileName.Length > 100)
            {
                fileName = fileName[..100];
            }

            return fileName;
        }

        private static string EnsureUniqueName(string name, List<string> existingNames)
        {
            int count = 1;
            string uniqueName = name;

            while (existingNames.Contains(uniqueName))
            {
                uniqueName = $"{name}_{count}";
                count++;
            }

            return uniqueName;
        }
    }
}
