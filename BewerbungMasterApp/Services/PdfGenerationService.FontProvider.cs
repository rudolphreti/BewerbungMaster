using iText.Kernel.Font;
using iText.IO.Font.Constants;

namespace BewerbungMasterApp.Services
{
    public partial class PdfGenerationService
    {
        private PdfFont GetRegularFont()
        {
            return PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
        }

        private PdfFont GetBoldFont()
        {
            return PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
        }
    }
}