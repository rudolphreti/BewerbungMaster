using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Font;
using iText.Layout.Properties;
using iText.Layout.Hyphenation;
using BewerbungMasterApp.Models;

namespace BewerbungMasterApp.Services
{
    public partial class PdfGenerationService
    {
        private static void SetDocumentDefaults(Document document, PdfFont regularFont)
        {
            document.SetFont(regularFont).SetFontSize(10);
        }

        private static void AddHeader(Document document, User user)
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

        private static void AddRecipient(Document document, string company)
        {
            AddParagraph(document, "An:", 0);
            AddParagraph(document, company, 0);
        }

        private static void AddDate(Document document, string city)
        {
            AddParagraph(document, $"{city}, {DateTime.Now:dd.MM.yyyy}", 10, TextAlignment.RIGHT);
        }

        private static void AddTitle(Document document, PdfFont boldFont, string position)
        {
            AddParagraph(document, $"Bewerbung als {position}", 20, TextAlignment.CENTER, 14, boldFont);
        }

        private static void AddGreeting(Document document)
        {
            AddParagraph(document, "Sehr geehrte Damen und Herren,", 10);
        }

        private static void AddMainContent(Document document, PdfFont regularFont, JobApplication application, JobAppContent jobAppContent)
        {
            string filledContent = FillPlaceholders(jobAppContent.Content, application);
            Paragraph content = ProcessContentWithLinks(filledContent, regularFont, 11);
            content.SetTextAlignment(TextAlignment.JUSTIFIED);
            content
                .SetHyphenation(new HyphenationConfig("de", "DE", 2, 2))
                .SetMarginBottom(20);
            document.Add(content);
        }

        private static void AddClosing(Document document, User user)
        {
            AddParagraph(document, "Mit freundlichen Grüßen", 0);
            AddParagraph(document, $"{user.FirstName} {user.LastName}", 0);
        }

        private static void AddParagraph(Document document, string text, float marginBottom, TextAlignment alignment = TextAlignment.LEFT, float fontSize = 10, PdfFont? font = null)
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

        private static void AddParagraphWithLinks(Document document, string text, float marginBottom)
        {
            document.Add(ProcessContentWithLinks(text, GetRegularFont(), 10).SetMarginBottom(marginBottom));
        }

        private void VerifyFileCreation(string outputPath)
        {
            FileInfo fileInfo = new(outputPath);
            if (fileInfo.Exists && fileInfo.Length > 0)
            {
                _logger.LogInformation("Cover letter successfully created. Path: {OutputPath}", outputPath);
                _logger.LogInformation("Cover letter file size: {FileSize} bytes", fileInfo.Length);
            }
            else
            {
                _logger.LogWarning("Cover letter file created but empty or not found. Path: {OutputPath}", outputPath);
            }
        }
    }
}