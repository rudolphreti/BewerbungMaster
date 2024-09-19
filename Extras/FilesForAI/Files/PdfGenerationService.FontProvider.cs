using iText.Kernel.Font;
using iText.IO.Font.Constants;

namespace BewerbungMasterApp.Services
{
    public partial class PdfGenerationService
    {
        private static PdfFont GetRegularFont() => PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

        private static PdfFont GetBoldFont() => PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
    }
}