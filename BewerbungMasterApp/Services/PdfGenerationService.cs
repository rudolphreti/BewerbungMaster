using System.IO;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using BewerbungMasterApp.Interfaces;
using BewerbungMasterApp.Models;

namespace BewerbungMasterApp.Services
{
    public class PdfGenerationService : IPdfGenerationService
    {
        public async Task GenerateCoverLetterPdfAsync(string targetDirectoryPath, string fileName, User user, JobApplication application)
        {
            // Define the PDF file path
            var pdfFilePath = Path.Combine(targetDirectoryPath, fileName);

            // Create a new PDF document
            using (var document = new PdfDocument())
            {
                document.Info.Title = "Cover Letter";

                // Add a page to the document
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont headerFont = new XFont("Verdana", 12);
                XFont bodyFont = new XFont("Verdana", 12);

                double marginLeft = XUnit.FromCentimeter(1).Point;
                double marginTop = XUnit.FromCentimeter(1).Point;

                // Create a text formatter for text wrapping
                XTextFormatter tf = new XTextFormatter(gfx);

                // Add user details
                tf.DrawString($"{user.FirstName} {user.LastName}", headerFont, XBrushes.Black, new XRect(marginLeft, marginTop, page.Width.Point - marginLeft * 2, page.Height.Point), XStringFormats.TopLeft);
                tf.DrawString(user.Address, headerFont, XBrushes.Black, new XRect(marginLeft, marginTop + 20, page.Width.Point - marginLeft * 2, page.Height.Point), XStringFormats.TopLeft);
                tf.DrawString($"{user.ZipCode} {user.City}", headerFont, XBrushes.Black, new XRect(marginLeft, marginTop + 40, page.Width.Point - marginLeft * 2, page.Height.Point), XStringFormats.TopLeft);
                tf.DrawString($"Email: {user.Email}", headerFont, XBrushes.Black, new XRect(marginLeft, marginTop + 60, page.Width.Point - marginLeft * 2, page.Height.Point), XStringFormats.TopLeft);
                tf.DrawString($"LinkedIn: {user.LinkedIn}", headerFont, XBrushes.Black, new XRect(marginLeft, marginTop + 80, page.Width.Point - marginLeft * 2, page.Height.Point), XStringFormats.TopLeft);
                tf.DrawString($"Phone: {user.Phone}", headerFont, XBrushes.Black, new XRect(marginLeft, marginTop + 100, page.Width.Point - marginLeft * 2, page.Height.Point), XStringFormats.TopLeft);

                tf.DrawString("An:", headerFont, XBrushes.Black, new XRect(marginLeft, marginTop + 140, page.Width.Point - marginLeft * 2, page.Height.Point), XStringFormats.TopLeft);
                tf.DrawString(application.Company, headerFont, XBrushes.Black, new XRect(marginLeft, marginTop + 160, page.Width.Point - marginLeft * 2, page.Height.Point), XStringFormats.TopLeft);

                tf.DrawString($"{user.City}, {DateTime.Now.ToString("dd.MM.yyyy")}", headerFont, XBrushes.Black, new XRect(marginLeft, marginTop + 200, page.Width.Point - marginLeft * 2, page.Height.Point), XStringFormats.TopLeft);

                // Add application details
                string applicationTitle = application.Position == "Initiativbewerbung"
                    ? application.Position
                    : $"Bewerbung als {application.Position}";

                tf.DrawString(applicationTitle, headerFont, XBrushes.Black, new XRect(marginLeft, marginTop + 240, page.Width.Point - marginLeft * 2, page.Height.Point), XStringFormats.TopLeft);

                // Generate the content of the application letter
                string content = GenerateApplicationContent(application.Company, application.Position, user);

                // Draw the content with text wrapping
                tf.DrawString(content, bodyFont, XBrushes.Black, new XRect(marginLeft, marginTop + 280, page.Width.Point - marginLeft * 2, page.Height.Point - marginTop), XStringFormats.TopLeft);

                // Save the document to file
                document.Save(pdfFilePath);
            }

            await Task.CompletedTask;
        }

        // Helper method to generate application content
        private string GenerateApplicationContent(string company, string position, User user)
        {
            // Dynamically create the content based on the position
            string positionText = position == "Initiativbewerbung"
                ? $"bei {company}. "
                : $"um die Position {position} bei {company}. ";

            // Build the full content from user data
            string content =
                user.ApplicationText.Introduction +
                user.ApplicationText.PositionIntro + positionText +
                user.ApplicationText.Education +
                user.ApplicationText.Experience +
                user.ApplicationText.Skills +
                user.ApplicationText.InternshipOffer +
                user.ApplicationText.Closing;

            return content;
        }
    }
}
