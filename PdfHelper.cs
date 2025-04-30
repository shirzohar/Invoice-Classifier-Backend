using ImageMagick;
using System.Drawing;
using System.IO;

namespace BusuMatchProject.Services
{
    public static class PdfHelper
    {
        public static Bitmap ConvertPdfToBitmap(string pdfPath, int dpi = 300)
        {
            var settings = new MagickReadSettings
            {
                Density = new Density(dpi),
            };

            using var images = new MagickImageCollection();
            images.Read(pdfPath, settings);

            if (images.Count == 0)
                throw new Exception("ה־PDF ריק או לא נטען כראוי.");

            using var image = (MagickImage)images[0];
            image.Format = MagickFormat.Png;

            // ✅ ממיר את MagickImage ל־Bitmap דרך stream
            var bytes = image.ToByteArray();
            using var ms = new MemoryStream(bytes);
            return new Bitmap(ms);
        }
    }
}
