using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ColorProfiles
{
    public unsafe class FastBitmap : IDisposable
    {
        public Bitmap Bitmap { get; private set; }
        private BitmapData bitmapData;
        private byte* scan0;

        public int Width { get; set; }
        public int Height { get; set; }
        public int BytesPerPixel { get; private set; }
        public int Depth { get; private set; }
        public bool Locked { get; private set; }

        public FastBitmap(Bitmap bitmap)
        {
            this.Bitmap = bitmap;

            Width = bitmap.Width;
            Height = bitmap.Height;
        }

        public Color GetPixel(int x, int y)
        {
            byte* currentLine = scan0 + (y * bitmapData.Stride);
            x *= BytesPerPixel;

            switch (Depth)
            {
                case 32:
                    return Color.FromArgb(currentLine[x + 3], currentLine[x + 2], currentLine[x + 1], currentLine[x]);
                case 24:
                    return Color.FromArgb(currentLine[x + 2], currentLine[x + 1], currentLine[x]);
                default:
                    return Color.FromArgb(currentLine[x], currentLine[x], currentLine[x]);
            }
        }

        public void SetPixel(int x, int y, Color color)
        {
            byte* currentLine = scan0 + (y * bitmapData.Stride);
            x *= BytesPerPixel;

            switch (Depth)
            {
                case 32:
                    currentLine[x + 3] = color.A;
                    currentLine[x + 2] = color.R;
                    currentLine[x + 1] = color.G;
                    currentLine[x] = color.B;
                    break;
                case 24:
                    currentLine[x + 2] = color.R;
                    currentLine[x + 1] = color.G;
                    currentLine[x] = color.B;
                    break;
                default:
                    currentLine[x] = color.R;
                    break;
            }
        }

        public void Lock()
        {
            if (Locked)
            {
                throw new InvalidOperationException("Bitmap is already locked");
            }

            var rect = new Rectangle(0, 0, Bitmap.Width, Bitmap.Height);
            bitmapData = Bitmap.LockBits(rect, ImageLockMode.ReadOnly, Bitmap.PixelFormat);
            scan0 = (byte*)bitmapData.Scan0;

            Depth = Image.GetPixelFormatSize(Bitmap.PixelFormat);
            BytesPerPixel = Depth / 8;
            Locked = true;
        }

        public void Unlock()
        {
            if (!Locked)
            {
                throw new InvalidOperationException("Bitmap is not locked");
            }

            Bitmap.UnlockBits(bitmapData);
            Locked = false;
        }

        public void Dispose()
        {
            if (Locked)
            {
                Unlock();
            }
        }
    }


    public static class FastBitmapExtensions
    {
        public static FastBitmap GetLockedFastBitmap(this Bitmap bitmap)
        {
            FastBitmap fastBitmap = new FastBitmap(bitmap);
            fastBitmap.Lock();

            return fastBitmap;
        }
    }
}
