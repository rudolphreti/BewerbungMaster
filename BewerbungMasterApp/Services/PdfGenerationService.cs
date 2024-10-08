﻿using iText.Kernel.Pdf;
using iText.Layout;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using BewerbungMasterApp.Models;
using BewerbungMasterApp.Interfaces;

namespace BewerbungMasterApp.Services
{
    public partial class PdfGenerationService(ILogger<PdfGenerationService> logger) : IPdfGenerationService
    {
        private readonly ILogger<PdfGenerationService> _logger = logger;

        public void GenerateCoverLetter(string outputPath, User user, JobApplication application, JobAppContent jobAppContent)
        {
            try
            {
                using var writer = new PdfWriter(outputPath);
                using var pdf = new PdfDocument(writer);
                using var document = new Document(pdf, PageSize.A4);

                PdfFont regularFont = GetRegularFont();
                PdfFont boldFont = GetBoldFont();

                SetDocumentDefaults(document, regularFont);

                AddHeader(document, user);
                AddRecipient(document, application.Company);
                AddDate(document, user.City);
                AddTitle(document, boldFont, application, jobAppContent);
                AddGreeting(document);
                AddMainContent(document, regularFont,  application, jobAppContent);
                AddClosing(document, user);

                VerifyFileCreation(outputPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating cover letter: {ErrorMessage}", ex.Message);
            }
        }
    }
}