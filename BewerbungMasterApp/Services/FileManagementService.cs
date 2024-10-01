using BewerbungMasterApp.Interfaces;

namespace BewerbungMasterApp.Services
{
    public partial class FileManagementService : IFileManagementService // change name to FileAndFoldersService
    {
        private readonly string _jobAppDocsPath;
        private readonly string _userDirectoryPath;
        private readonly IPdfGenerationService _pdfGenerationService;
        private readonly IJsonService _jsonService;
        private readonly ILogger<FileManagementService> _logger;
        public FileManagementService(IConfiguration configuration, IWebHostEnvironment environment, IPdfGenerationService pdfGenerationService, IJsonService jsonService, ILogger<FileManagementService> logger)
        {
            _jobAppDocsPath = Path.Combine(environment.WebRootPath, "JobAppDocs");
            _userDirectoryPath = Path.Combine(environment.WebRootPath, configuration["UserDirectoryPath"]);
            _pdfGenerationService = pdfGenerationService;
            _jsonService = jsonService;
            _logger = logger;
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
