using BewerbungMasterApp.Interfaces;

namespace BewerbungMasterApp.Services
{
    public partial class FileManagementService : IFileManagementService // change name to FileAndFoldersService
    {
        private readonly string _jobAppDocsPath;
        private readonly string _userDirectoryPath;
        //private readonly string _coverLetterTemplateName;
        private readonly IPdfGenerationService _pdfGenerationService;


        public string JobAppDocsPath => _jobAppDocsPath; // don't understand; unit test need it
        public string UserDirectoryPath => _userDirectoryPath; // don't understand; unit test need it


        public FileManagementService(IConfiguration configuration, IWebHostEnvironment environment, IPdfGenerationService pdfGenerationService)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(environment);
            ArgumentNullException.ThrowIfNull(pdfGenerationService);


            if (string.IsNullOrWhiteSpace(environment.WebRootPath))
                throw new InvalidOperationException("Web root path cannot be null or empty.");

            var userDirectoryPath = configuration["UserDirectoryPath"];
            if (string.IsNullOrWhiteSpace(userDirectoryPath))
                throw new InvalidOperationException("User directory path cannot be null or empty.");

            _jobAppDocsPath = Path.Combine(environment.WebRootPath, "JobAppDocs");
            _userDirectoryPath = Path.Combine(environment.WebRootPath, userDirectoryPath);
            //_coverLetterTemplateName = configuration["CoverLetterTemplateName"] ?? "CoverLetterTemplate.html";
            _pdfGenerationService = pdfGenerationService;
        }

        public void InitializeJobAppDocsDirectory() //why must be public?
        {
            if (string.IsNullOrWhiteSpace(_jobAppDocsPath))
                throw new InvalidOperationException("JobAppDocs path is invalid.");

            if (Directory.Exists(_jobAppDocsPath))
            {
                Directory.Delete(_jobAppDocsPath, true);
            }

            Directory.CreateDirectory(_jobAppDocsPath);
        }

        //TO DELETE
        //private string GetCoverLetterTemplatePath()
        //{
        //    string templatePath = Path.Combine(_userDirectoryPath, _coverLetterTemplateName);
        //    if (File.Exists(templatePath))
        //    {
        //        return templatePath;
        //    }

        //    throw new FileNotFoundException($"Cover letter template not found at {templatePath}");
        //}




    }
}
