using Tesseract;
using System.Drawing;
using System.IO;
using System;

namespace BusuMatchProject.Services
{
    public class OcrService
    {
        public string ExtractTextFromFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                throw new FileNotFoundException("הקובץ לא נמצא או הנתיב לא תקין", path);

            var ext = Path.GetExtension(path).ToLower();

            Bitmap bitmap;

            try
            {
                if (ext == ".pdf")
                {
                    // ממיר PDF לתמונה – רק העמוד הראשון
                    bitmap = PdfHelper.ConvertPdfToBitmap(path);
                }
                else if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp" || ext == ".tif" || ext == ".tiff")
                {
                    // טעינה ישירה של תמונה
                    using var original = new Bitmap(path);
                    bitmap = new Bitmap(original); // יוצר עותק למניעת נעילה
                }
                else
                {
                    throw new NotSupportedException($"סוג קובץ לא נתמך: {ext}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"שגיאה בטעינת תמונה או המרת PDF: {ex.Message}");
            }

            try
            {
                var preprocessed = PreprocessImage(bitmap);
                return RunOcrOnBitmap(preprocessed);
            }
            finally
            {
                bitmap.Dispose();
            }
        }

        private string RunOcrOnBitmap(Bitmap bitmap)
        {
            using var engine = new TesseractEngine(@"./tessdata", "heb+eng", EngineMode.Default);
            engine.SetVariable("user_defined_dpi", "300");

            using var img = PixConverter.ToPix(bitmap);
            using var page = engine.Process(img);

            return page.GetText().Trim();
        }

        private Bitmap PreprocessImage(Bitmap original)
        {
            // ממיר לגווני אפור
            Bitmap gray = new Bitmap(original.Width, original.Height);
            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    var pixel = original.GetPixel(x, y);
                    int grayValue = (int)(0.3 * pixel.R + 0.59 * pixel.G + 0.11 * pixel.B);
                    gray.SetPixel(x, y, Color.FromArgb(grayValue, grayValue, grayValue));
                }
            }

            // סף בינארי (threshold)
            for (int y = 0; y < gray.Height; y++)
            {
                for (int x = 0; x < gray.Width; x++)
                {
                    var pixel = gray.GetPixel(x, y);
                    gray.SetPixel(x, y, pixel.R > 180 ? Color.White : Color.Black);
                }
            }

            return gray;
        }
    }
}
