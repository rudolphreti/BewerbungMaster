using System.Text.RegularExpressions;
using iText.Kernel.Font;
using iText.Layout.Element;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Action;
using BewerbungMasterApp.Models;

namespace BewerbungMasterApp.Services
{
    public partial class PdfGenerationService
    {
        private static string FillPlaceholders(string content, JobApplication application)
        {
            return content
                .Replace("{position}", application.Position)
                .Replace("{company}", application.Company)
                .Replace("\n", "\n\n");
        }

        private static Paragraph ProcessContentWithLinks(string text, PdfFont font, float fontSize)
        {
            Paragraph paragraph = new();
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