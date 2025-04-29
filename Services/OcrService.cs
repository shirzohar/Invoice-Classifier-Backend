using Tesseract;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System;
using System.Text.RegularExpressions;

namespace BusuMatchProject.Services
{
    public class OcrService
    {
        // Main method to extract text from a file (PDF or image)
        public string ExtractTextFromFile(string path)
        {
            var ext = Path.GetExtension(path).ToLower();

            if (ext == ".pdf")
            {
                // Convert PDF to image (first page only)
                var image = PdfHelper.ConvertPdfToBitmap(path);

                // Convert to Bitmap if needed
                if (image is not Bitmap bitmap)
                    throw new InvalidCastException("Image could not be cast to Bitmap.");

                var preprocessed = PreprocessImage(bitmap);
                return RunOcrOnBitmap(preprocessed);
            }
            else
            {
                var image = new Bitmap(path);
                var preprocessed = PreprocessImage(image);
                return RunOcrOnBitmap(preprocessed);
            }
        }

        // Performs OCR using Tesseract on a given bitmap image
        private string RunOcrOnBitmap(Bitmap bitmap)
        {
            using var engine = new TesseractEngine(@"./tessdata", "heb+eng", EngineMode.Default);
            engine.SetVariable("user_defined_dpi", "300");
            using var img = PixConverter.ToPix(bitmap);
            using var page = engine.Process(img);
            return page.GetText();
        }

        // Converts an image to grayscale and applies binary thresholding
        private Bitmap PreprocessImage(Bitmap original)
        {
            Bitmap gray = new Bitmap(original.Width, original.Height);
            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color oc = original.GetPixel(x, y);
                    int grayValue = (int)(0.3 * oc.R + 0.59 * oc.G + 0.11 * oc.B);
                    gray.SetPixel(x, y, Color.FromArgb(grayValue, grayValue, grayValue));
                }
            }

            for (int y = 0; y < gray.Height; y++)
            {
                for (int x = 0; x < gray.Width; x++)
                {
                    Color gc = gray.GetPixel(x, y);
                    gray.SetPixel(x, y, gc.R > 180 ? Color.White : Color.Black);
                }
            }

            return gray;
        }
    }
}
