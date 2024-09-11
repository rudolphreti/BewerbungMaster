using BewerbungMasterApp.Interfaces;

namespace BewerbungMasterApp.Services
{
    public partial class FileManagementService : IFileManagementService // change name to FileAndFoldersService
    {
        private readonly string _jobAppDocsPath;
        private readonly string _userDirectoryPath;
        private readonly IPdfGenerationService _pdfGenerationService;
        public FileManagementService(IConfiguration configuration, IWebHostEnvironment environment, IPdfGenerationService pdfGenerationService)
        {
            ArgumentNullException.ThrowIfNull(configuration); //don't understand this
            ArgumentNullException.ThrowIfNull(environment); //don't understand this
            ArgumentNullException.ThrowIfNull(pdfGenerationService); //don't understand this


            if (string.IsNullOrWhiteSpace(environment.WebRootPath))
                throw new InvalidOperationException("Web root path cannot be null or empty.");

            var userDirectoryPath = configuration["UserDirectoryPath"];
            if (string.IsNullOrWhiteSpace(userDirectoryPath))
                throw new InvalidOperationException("User directory path cannot be null or empty.");

            _jobAppDocsPath = Path.Combine(environment.WebRootPath, "JobAppDocs");
            _userDirectoryPath = Path.Combine(environment.WebRootPath, userDirectoryPath);
            _pdfGenerationService = pdfGenerationService;
        }

        public void InitializeJobAppDocsDirectory() //why must it be public, not internal?
        {
            if (string.IsNullOrWhiteSpace(_jobAppDocsPath))
                throw new InvalidOperationException("JobAppDocs path is invalid.");

            if (Directory.Exists(_jobAppDocsPath))
            {
                Directory.Delete(_jobAppDocsPath, true);
            }

            Directory.CreateDirectory(_jobAppDocsPath);
        }
    }
}
