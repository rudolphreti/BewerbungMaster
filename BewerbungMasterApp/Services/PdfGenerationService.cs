using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Font;
using iText.Layout.Properties;
using iText.Layout.Hyphenation;
using iText.Kernel.Geom;
using BewerbungMasterApp.Models;
using BewerbungMasterApp.Interfaces;
using Microsoft.Extensions.Logging;

namespace BewerbungMasterApp.Services
{
    public partial class PdfGenerationService : IPdfGenerationService
    {
        private readonly ILogger<PdfGenerationService> _logger;

        public PdfGenerationService(ILogger<PdfGenerationService> logger)
        {
            _logger = logger;
        }

        public void GenerateCoverLetter(string outputPath, User user, JobApplication application)
        {
            _logger.LogInformation($"Generating cover letter for {user.FirstName} {user.LastName}: {outputPath}");
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
                AddTitle(document, boldFont, application.Position);
                AddGreeting(document);
                AddMainContent(document, regularFont, user, application);
                AddClosing(document, user);

                VerifyFileCreation(outputPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating cover letter: {ex.Message}");
            }
        }

        private void SetDocumentDefaults(Document document, PdfFont regularFont)
        {
            document.SetFont(regularFont).SetFontSize(10);
        }

        private void AddHeader(Document document, User user)
        {
            AddParagraph(document, $"{user.FirstName} {user.LastName}", 0);
            AddParagraph(document, user.Address, 0);
            AddParagraph(document, $"{user.ZipCode} {user.City}", 0);
            AddParagraphWithLinks(document, user.Email, 0);
            if (!string.IsNullOrEmpty(user.LinkedIn))
            {
                AddParagraphWithLinks(document, user.LinkedIn, 0);
            }
            AddParagraph(document, $"Telefonnummer: {user.Phone}", 10);
        }

        private void AddRecipient(Document document, string company)
        {
            AddParagraph(document, "An:", 0);
            AddParagraph(document, company, 0);
        }

        private void AddDate(Document document, string city)
        {
            AddParagraph(document, $"{city}, {DateTime.Now:dd.MM.yyyy}", 10, TextAlignment.RIGHT);
        }

        private void AddTitle(Document document, PdfFont boldFont, string position)
        {
            AddParagraph(document, $"Bewerbung als {position}", 20, TextAlignment.CENTER, 14, boldFont);
        }

        private void AddGreeting(Document document)
        {
            AddParagraph(document, "Sehr geehrte Damen und Herren,", 10);
        }

        private void AddMainContent(Document document, PdfFont regularFont, User user, JobApplication application)
        {
            string filledContent = FillPlaceholders(user.JobAppContent, application);
            Paragraph content = ProcessContentWithLinks(filledContent, regularFont, 11);
            content.SetTextAlignment(TextAlignment.JUSTIFIED);
            content.SetHyphenation(new HyphenationConfig("de", "DE", 2, 2));
            document.Add(content);
        }

        private void AddClosing(Document document, User user)
        {
            AddParagraph(document, "Mit freundlichen Grüßen", 20);
            AddParagraph(document, $"{user.FirstName} {user.LastName}", 0);
        }

        private void AddParagraph(Document document, string text, float marginBottom, TextAlignment alignment = TextAlignment.LEFT, float fontSize = 10, PdfFont font = null)
        {
            var paragraph = new Paragraph(text)
                .SetMarginBottom(marginBottom)
                .SetTextAlignment(alignment)
                .SetFontSize(fontSize);

            if (font != null)
            {
                paragraph.SetFont(font);
            }

            document.Add(paragraph);
        }

        private void AddParagraphWithLinks(Document document, string text, float marginBottom)
        {
            document.Add(ProcessContentWithLinks(text, GetRegularFont(), 10).SetMarginBottom(marginBottom));
        }

        private void VerifyFileCreation(string outputPath)
        {
            FileInfo fileInfo = new(outputPath);
            if (fileInfo.Exists && fileInfo.Length > 0)
            {
                _logger.LogInformation($"Cover letter successfully created: {outputPath}");
                _logger.LogInformation($"File size: {fileInfo.Length} bytes");
            }
            else
            {
                _logger.LogWarning($"Cover letter file was created but appears to be empty: {outputPath}");
            }
        }
    }
}