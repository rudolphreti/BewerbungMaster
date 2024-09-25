using BewerbungMasterApp.Models;

namespace BewerbungMasterApp.Interfaces
{
    public interface IPdfGenerationService
    {
        void GenerateCoverLetter(string outputPath, User user, JobApplication application, JobAppContent jobAppContent);
    }
}