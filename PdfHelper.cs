using ImageMagick;
using System;

namespace BusuMatchProject.Services
{
    public static class PdfHelper
    {
        public static MagickImage ConvertPdfToMagickImage(string pdfPath, int dpi = 300)
        {
            var settings = new MagickReadSettings
            {
                Density = new Density(dpi)
            };

            using var images = new MagickImageCollection();
            images.Read(pdfPath, settings);

            if (images.Count == 0)
                throw new Exception("ה־PDF ריק או לא נטען כראוי.");

            return (MagickImage)images[0].Clone();
        }
    }
}
