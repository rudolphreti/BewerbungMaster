using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Layout.Properties;
using iText.Layout.Hyphenation;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Action;
using System.Text.RegularExpressions;
using BewerbungMasterApp.Models;

namespace BewerbungMasterApp.Services
{
    public static class PdfGenerationService
    {
        public static void GenerateCoverLetter(string outputPath, User user, JobApplication application)
        {
            Console.WriteLine($"Generating cover letter for {user.FirstName} {user.LastName}: {outputPath}");
            try
            {
                using (var writer = new PdfWriter(outputPath))
                using (var pdf = new PdfDocument(writer))
                using (var document = new Document(pdf, PageSize.A4))
                {
                    PdfFont regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                    PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                    // Set default font and size for the entire document
                    document.SetFont(regularFont).SetFontSize(10);

                    // Header
                    AddHeader(document, regularFont, user);

                    // Recipient
                    document.Add(new Paragraph("An:").SetMarginBottom(0));
                    document.Add(new Paragraph(application.Company).SetMarginBottom(0));

                    // Date
                    document.Add(new Paragraph($"{user.City}, {DateTime.Now:dd.MM.yyyy}")
                        .SetTextAlignment(TextAlignment.RIGHT)
                        .SetMarginTop(10));

                    // Title
                    document.Add(new Paragraph($"Bewerbung als {application.Position}")
                        .SetFont(boldFont)
                        .SetFontSize(14)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginTop(20)
                        .SetMarginBottom(20));

                    document.Add(new Paragraph("Sehr geehrte Damen und Herren,").SetMarginBottom(10));

                    // Main content
                    AddMainContent(document, regularFont, user, application);

                    // Closing
                    document.Add(new Paragraph("Mit freundlichen Grüßen").SetMarginTop(20));
                    document.Add(new Paragraph($"{user.FirstName} {user.LastName}"));
                }

                // Verify file creation
                FileInfo fileInfo = new FileInfo(outputPath);
                if (fileInfo.Exists && fileInfo.Length > 0)
                {
                    Console.WriteLine($"Cover letter successfully created: {outputPath}");
                    Console.WriteLine($"File size: {fileInfo.Length} bytes");
                }
                else
                {
                    Console.WriteLine($"Warning: Cover letter file was created but appears to be empty: {outputPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating cover letter: {ex.GetType().Name} - {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }

        private static void AddHeader(Document document, PdfFont regularFont, User user)
        {
            document.Add(new Paragraph($"{user.FirstName} {user.LastName}").SetMarginBottom(0));
            document.Add(new Paragraph(user.Address).SetMarginBottom(0));
            document.Add(new Paragraph($"{user.ZipCode} {user.City}").SetMarginBottom(0));
            document.Add(ProcessContentWithLinks(user.Email, regularFont, 10).SetMarginBottom(0));
            if (!string.IsNullOrEmpty(user.LinkedIn))
            {
                document.Add(ProcessContentWithLinks(user.LinkedIn, regularFont, 10).SetMarginBottom(0));
            }
            document.Add(new Paragraph($"Telefonnummer: {user.Phone}").SetMarginBottom(10));
        }

        private static void AddMainContent(Document document, PdfFont regularFont, User user, JobApplication application)
        {
            // Replace placeholders in the jobAppContent
            string filledContent = user.JobAppContent
                .Replace("{position}", application.Position)
                .Replace("{company}", application.Company)
                .Replace("\n", "\n\n");

            Paragraph content = ProcessContentWithLinks(filledContent, regularFont, 11);
            content.SetTextAlignment(TextAlignment.JUSTIFIED);
            content.SetHyphenation(new HyphenationConfig("de", "DE", 2, 2));
            document.Add(content);
        }

        private static Paragraph ProcessContentWithLinks(string text, PdfFont font, float fontSize)
        {
            Paragraph paragraph = new Paragraph();
            string pattern = @"(https?://\S+[^.,;!?)\s]|[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})([.,;!?)])?";
            MatchCollection matches = Regex.Matches(text, pattern);
            int lastIndex = 0;

            foreach (Match match in matches)
            {
                // Add text before the link or email
                string beforeMatch = text[lastIndex..match.Index];
                if (!string.IsNullOrEmpty(beforeMatch))
                {
                    paragraph.Add(new Text(beforeMatch).SetFont(font).SetFontSize(fontSize));
                }

                string fullMatch = match.Groups[1].Value;
                string punctuation = match.Groups[2].Value;

                // Determine if it's a URL or an email
                bool isUrl = fullMatch.StartsWith("http://") || fullMatch.StartsWith("https://");

                string displayText = fullMatch;
                if (isUrl)
                {
                    // Remove http:// or https:// from display URL
                    displayText = displayText.StartsWith("http://") ? displayText[7..] : displayText[8..];
                }

                // Add the link or email
                Text linkText = new Text(displayText)
                    .SetFont(font)
                    .SetFontSize(fontSize)
                    .SetFontColor(ColorConstants.BLUE);

                linkText.SetAction(isUrl ? PdfAction.CreateURI(fullMatch) : PdfAction.CreateURI("mailto:" + fullMatch));

                paragraph.Add(linkText);

                // Add punctuation after the link or email
                if (!string.IsNullOrEmpty(punctuation))
                {
                    paragraph.Add(new Text(punctuation).SetFont(font).SetFontSize(fontSize));
                }

                lastIndex = match.Index + match.Length;
            }

            // Add any remaining text after the last match
            if (lastIndex < text.Length)
            {
                string remainingText = text[lastIndex..];
                paragraph.Add(new Text(remainingText).SetFont(font).SetFontSize(fontSize));
            }

            return paragraph;
        }
    }
}