using System;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using System.Text;
using BewerbungMasterApp.Interfaces;

namespace BewerbungMasterApp.Services
{
    public static class PdfGenerationService
    {

        public static void GenerateCoverLetter(string outputPath, string company, string position)
        {
            using (var document = new PdfSharp.Pdf.PdfDocument())
            {
                var page = document.AddPage();
                using (var gfx = XGraphics.FromPdfPage(page))
                {
                    var tf = new XTextFormatter(gfx);
                    var normalFont = new XFont("Arial", 11, XFontStyleEx.Regular);
                    var boldFont = new XFont("Arial", 11, XFontStyleEx.Bold);
                    var titleFont = new XFont("Arial", 14, XFontStyleEx.Bold);

                    // Header
                    var yPosition = 40;
                    tf.DrawString("Mikolaj Kosmalski", boldFont, XBrushes.Black, new XRect(40, yPosition, page.Width, 20), XStringFormats.TopLeft);
                    yPosition += 20;
                    tf.DrawString("Pogrelzstraße 55/6/4", normalFont, XBrushes.Black, new XRect(40, yPosition, page.Width, 20), XStringFormats.TopLeft);
                    yPosition += 20;
                    tf.DrawString("1220 Wien", normalFont, XBrushes.Black, new XRect(40, yPosition, page.Width, 20), XStringFormats.TopLeft);
                    yPosition += 20;
                    tf.DrawString("mikolaj.jakub.kosmalski@gmail.com", normalFont, XBrushes.Black, new XRect(40, yPosition, page.Width, 20), XStringFormats.TopLeft);
                    yPosition += 20;
                    tf.DrawString("linkedin.com/in/mikolajkosmalski", normalFont, XBrushes.Black, new XRect(40, yPosition, page.Width, 20), XStringFormats.TopLeft);
                    yPosition += 40;

                    // Company
                    tf.DrawString($"An:", normalFont, XBrushes.Black, new XRect(40, yPosition, page.Width, 20), XStringFormats.TopLeft);
                    yPosition += 20;
                    tf.DrawString(company, normalFont, XBrushes.Black, new XRect(40, yPosition, page.Width, 20), XStringFormats.TopLeft);
                    yPosition += 40;

                    // Date
                    tf.DrawString($"Wien, {DateTime.Now:dd.MM.yyyy}", normalFont, XBrushes.Black, new XRect(40, yPosition, page.Width, 20), XStringFormats.TopLeft);
                    yPosition += 40;

                    // Title
                    tf.DrawString($"Bewerbung als {position}", titleFont, XBrushes.Black, new XRect(40, yPosition, page.Width, 20), XStringFormats.TopLeft);
                    yPosition += 40;

                    // Content
                    string content = $@"Sehr geehrte Damen und Herren,

mit großer Begeisterung bewerbe ich mich um die Position {position} bei {company}. Ich habe gerade eine zweijährige Ausbildung im Bereich der Anwendungsentwicklung mit LAP abgeschlossen, wo ich solide Grundlagen in der objektorientierten Programmierung mit .NET/C# erlernte. Darüber hinaus bin ich in der Lage, Webanwendungen in ASP.NET mit REST API zu entwickeln, wobei ich SQL-Datenbanken effektiv nutze.

Von 2020 bis 2023 war ich Mitbegründer des Start-ups timagio.com, das Apps für Eltern und Lehrer im Zusammenhang mit der Montessori-Pädagogik entwickelt. Dort habe ich mehrere Prototypen in JavaScript, Automatisierungen in Python und Powershell-Skripte entwickelt sowie Erfahrung mit Scrum, Azure DevOps und Git gesammelt.

Ich lerne schnell, habe einen analytischen Verstand, bin kreativ, offen für Kritik und resistent gegen Stress. Ich bin daher überzeugt, dass ich ein wertvolles Mitglied Ihres Teams sein kann. Ich freue mich darauf, in einem persönlichen Gespräch mehr über diese Position zu erfahren.

Um meine Einarbeitung zu erleichtern, bin ich gerne bereit ein Praktikum zu absolvieren. Sie könnten profitieren von einer Eingliederungsbeihilfe für einen Zeitraum von 6 Monaten mit einem Höchstbetrag von insgesamt 18 000 EUR: ams.at/unternehmen/service-zur-personalsuche/foerderungen/eingliederungsbeihilfe

Ich füge diesem Schreiben meinen Lebenslauf und meine LAP-Zeugnis bei.

Mit freundlichen Grüßen
Mikolaj Kosmalski";

                    tf.DrawString(content, normalFont, XBrushes.Black, new XRect(40, yPosition, page.Width - 80, page.Height - yPosition - 40), XStringFormats.TopLeft);
                }

                document.Save(outputPath);
            }
        }
    }
}