using System.Drawing;
using System.Drawing.Imaging;
using Tesseract;
using System.IO;

namespace BusuMatchProject.Services
{
    public static class PixConverter
    {
        public static Pix ToPix(Bitmap bitmap)
        {
            using var stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
            stream.Position = 0;
            return Pix.LoadFromMemory(stream.ToArray());
        }
    }
}
