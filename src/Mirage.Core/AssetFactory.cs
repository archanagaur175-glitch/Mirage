using System;
using System.IO;

namespace Mirage.Core
{
    /// <summary>
    /// Generates license-safe, original Tahoe-inspired wallpaper bitmaps at runtime
    /// (no Apple artwork, no redistributed proprietary fonts). Pure .NET, so it is
    /// fully unit-testable and contains no networking.
    /// </summary>
    public static class AssetFactory
    {
        public static string WriteGradientWallpaper(string path, int width = 1920, int height = 1080)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? ".");
            byte[] bmp = CreateGradientBmp(width, height);
            File.WriteAllBytes(path, bmp);
            return path;
        }

        private static byte[] CreateGradientBmp(int width, int height)
        {
            const int bpp = 24;
            int rowBytes = ((width * bpp + 31) / 32) * 4;
            int pixelBytes = rowBytes * height;
            int fileSize = 54 + pixelBytes;

            using var ms = new MemoryStream(fileSize);
            using var bw = new BinaryWriter(ms);

            // BITMAPFILEHEADER
            bw.Write((byte)'B'); bw.Write((byte)'M');
            bw.Write(fileSize);
            bw.Write((ushort)0); bw.Write((ushort)0);
            bw.Write(54);

            // BITMAPINFOHEADER
            bw.Write(40);
            bw.Write(width);
            bw.Write(height);
            bw.Write((ushort)1);
            bw.Write((ushort)bpp);
            bw.Write(0);            // no compression
            bw.Write(pixelBytes);
            bw.Write(0); bw.Write(0);
            bw.Write(0); bw.Write(0);

            // Pixel data (bottom-up, BGR)
            double cx = width * 0.5;
            double cy = height * 0.42;
            double maxd = Math.Sqrt(cx * cx + cy * cy);
            for (int y = 0; y < height; y++)
            {
                // Vertical gradient: deep blue (top) -> violet (bottom).
                double t = (double)y / (height - 1);
                for (int x = 0; x < width; x++)
                {
                    // Soft radial glow toward the upper-center (Tahoe "light").
                    double d = Math.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy)) / maxd;
                    double glow = Math.Max(0.0, 1.0 - d);
                    byte r = (byte)Math.Min(255, 18 + t * 70 + glow * 90);
                    byte g = (byte)Math.Min(255, 26 + t * 26 + glow * 60);
                    byte b = (byte)Math.Min(255, 110 + t * 90 + glow * 40);

                    bw.Write((byte)b);
                    bw.Write((byte)g);
                    bw.Write((byte)r);
                }

                int pad = rowBytes - width * 3;
                for (int p = 0; p < pad; p++)
                {
                    bw.Write((byte)0);
                }
            }

            return ms.ToArray();
        }
    }
}
