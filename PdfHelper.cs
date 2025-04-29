using PdfiumViewer;
using System.Drawing;

namespace BusuMatchProject.Services
{
    public static class PdfHelper
    {
        public static Image ConvertPdfToBitmap(string path)
        {
            using var pdfDocument = PdfiumViewer.PdfDocument.Load(path);
            return pdfDocument.Render(0, 300, 300, true); // רק עמוד ראשון
        }
    }
}
