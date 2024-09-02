using BewerbungMasterApp.Models;

namespace BewerbungMasterApp.Interfaces
{
    public interface IPdfGenerationService
    {
        Task GenerateCoverLetterPdfAsync(string targetDirectoryPath, string fileName, User user, JobApplication application);
    }
}
