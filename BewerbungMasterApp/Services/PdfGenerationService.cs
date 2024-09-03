using System;
using System.IO;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using BewerbungMasterApp.Interfaces;
using BewerbungMasterApp.Models;

namespace BewerbungMasterApp.Services;

public class PdfGenerationService : IPdfGenerationService
{
    public async Task GenerateCoverLetterPdfAsync(string targetDirectoryPath, string fileName, User user, JobApplication application)
    {
        var pdfFilePath = Path.Combine(targetDirectoryPath, fileName);

        using var document = new PdfDocument();
        document.Info.Title = "Cover Letter";

        var page = document.AddPage();
        using var gfx = XGraphics.FromPdfPage(page);
        var headerFont = new XFont("Verdana", 12);
        var bodyFont = new XFont("Verdana", 12);

        var marginLeft = XUnit.FromCentimeter(2).Point;
        var marginRight = XUnit.FromCentimeter(2).Point;
        var marginTop = XUnit.FromCentimeter(2).Point;

        var tf = new XTextFormatter(gfx);

        // User details
        DrawUserDetails(tf, headerFont, marginLeft, marginTop, user);

        // Company details
        DrawCompanyDetails(tf, headerFont, marginLeft, marginTop, application);

        // Date
        tf.DrawString($"{user.City}, {DateTime.Now:dd.MM.yyyy}", headerFont, XBrushes.Black,
            new XRect(marginLeft, marginTop + 200, page.Width.Point - marginLeft - marginRight, 20), XStringFormats.TopLeft);

        // Application title
        var applicationTitle = application.Position == "Initiativbewerbung"
            ? application.Position
            : $"Bewerbung als {application.Position}";
        tf.DrawString(applicationTitle, headerFont, XBrushes.Black,
            new XRect(marginLeft, marginTop + 240, page.Width.Point - marginLeft - marginRight, 20), XStringFormats.TopLeft);

        // Application content
        var content = GenerateApplicationContent(application.Company, application.Position, user);
        DrawContentWithParagraphs(tf, bodyFont, marginLeft, marginTop + 280, page.Width.Point - marginLeft - marginRight, page.Height.Point - marginTop, content);

        document.Save(pdfFilePath);

        await Task.CompletedTask;
    }

    private static void DrawUserDetails(XTextFormatter tf, XFont font, double marginLeft, double marginTop, User user)
    {
        var userDetails = new[]
        {
            $"{user.FirstName} {user.LastName}",
            user.Address,
            $"{user.ZipCode} {user.City}",
            $"Email: {user.Email}",
            $"LinkedIn: {user.LinkedIn}",
            $"Phone: {user.Phone}"
        };

        for (int i = 0; i < userDetails.Length; i++)
        {
            tf.DrawString(userDetails[i], font, XBrushes.Black,
                new XRect(marginLeft, marginTop + i * 20, 400, 20), XStringFormats.TopLeft);
        }
    }

    private static void DrawCompanyDetails(XTextFormatter tf, XFont font, double marginLeft, double marginTop, JobApplication application)
    {
        tf.DrawString("An:", font, XBrushes.Black,
            new XRect(marginLeft, marginTop + 140, 400, 20), XStringFormats.TopLeft);
        tf.DrawString(application.Company, font, XBrushes.Black,
            new XRect(marginLeft, marginTop + 160, 400, 20), XStringFormats.TopLeft);
    }

    private static void DrawContentWithParagraphs(XTextFormatter tf, XFont font, double x, double y, double width, double height, string content)
    {
        var paragraphs = content.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
        var yOffset = y;

        foreach (var paragraph in paragraphs)
        {
            tf.DrawString(paragraph, font, XBrushes.Black, new XRect(x, yOffset, width, height), XStringFormats.TopLeft);
            yOffset += 30; // Add space between paragraphs
        }
    }

    private static string GenerateApplicationContent(string company, string position, User user)
    {
        var positionText = position == "Initiativbewerbung"
            ? $"bei {company}. "
            : $"um die Position {position} bei {company}. ";

        return $"""
            {user.ApplicationText.Introduction}

            {user.ApplicationText.PositionIntro}{positionText}

            {user.ApplicationText.Education}

            {user.ApplicationText.Experience}

            {user.ApplicationText.Skills}

            {user.ApplicationText.InternshipOffer}

            {user.ApplicationText.Closing}
            """;
    }
}