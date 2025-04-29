using Tesseract;
using System.IO;

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
                // Convert PDF to images using pdftoppm
                string tempOutputPrefix = Path.Combine(Path.GetTempPath(), "output");

                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "pdftoppm",
                    Arguments = $"-jpeg \"{path}\" \"{tempOutputPrefix}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = System.Diagnostics.Process.Start(startInfo);
                process.WaitForExit();

                var images = Directory.GetFiles(Path.GetTempPath(), "output-*.jpg");
                if (images.Length == 0)
                    throw new Exception("Failed to convert PDF to images");

                var extractedText = new List<string>();
                foreach (var imgPath in images)
                {
                    extractedText.Add(RunOcrOnFile(imgPath));
                    File.Delete(imgPath); // Clean up
                }

                return string.Join("\n", extractedText);
            }
            else
            {
                // For images (jpg, png, etc.)
                return RunOcrOnFile(path);
            }
        }

        // Performs OCR directly on a file using Tesseract
        private string RunOcrOnFile(string filePath)
        {
            using var engine = new TesseractEngine(@"./tessdata", "heb+eng", EngineMode.Default);
            engine.SetVariable("user_defined_dpi", "300");
            using var img = Pix.LoadFromFile(filePath);
            using var page = engine.Process(img);
            return page.GetText();
        }
    }
}
