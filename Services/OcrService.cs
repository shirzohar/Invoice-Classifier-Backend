using Tesseract;
using ImageMagick;
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
            MagickImage image;

            try
            {
                if (ext == ".pdf")
                {
                    image = PdfHelper.ConvertPdfToMagickImage(path);
                }
                else if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp" || ext == ".tif" || ext == ".tiff")
                {
                    image = new MagickImage(path);
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
                var preprocessed = PreprocessImage(image);
                return RunOcrOnMagickImage(preprocessed);
            }
            finally
            {
                image?.Dispose();
            }
        }

        private string RunOcrOnMagickImage(MagickImage image)
        {
            using var engine = new TesseractEngine(@"./tessdata", "heb+eng", EngineMode.Default);
            engine.SetVariable("user_defined_dpi", "300");

            using var stream = new MemoryStream();
            image.Format = MagickFormat.Bmp;
            image.Write(stream);
            stream.Position = 0;

            using var pix = Pix.LoadFromMemory(stream.ToArray());
            using var page = engine.Process(pix);

            return page.GetText().Trim();
        }

        private MagickImage PreprocessImage(MagickImage image)
        {
            var processed = (MagickImage)image.Clone(); // ✅ המרה מפורשת

            processed.ColorType = ColorType.Grayscale;
            processed.Threshold(new Percentage(70)); // בינאריזציה פשוטה

            return processed;
        }

    }
}
