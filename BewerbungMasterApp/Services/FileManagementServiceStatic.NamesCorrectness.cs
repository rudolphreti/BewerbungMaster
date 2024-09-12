namespace BewerbungMasterApp.Services
{
    public partial class FileManagementServiceStatic // to refactor, simplyfy
    {
        public static string CleanName(string fileName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                fileName = fileName.Replace(c, '_');
            }

            if (fileName.Length > 90)
            {
                fileName = fileName[..90];
            }

            // Removes only spaces at the end of the file name - IMPORTANT! without TrimEnd() the file or folder name is not created correctly
            fileName = fileName.TrimEnd();

            return fileName;
        }

        public static string EnsureUniqueName(string name, List<string> existingNames)
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