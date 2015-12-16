using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
namespace RecognitionOfPassports
{
    class ImageRecizer
    {
        public static unsafe Bitmap Resize(Bitmap sourceBitmap, Size newSize)
        {
            GC.Collect();
            Bitmap destinationBitmap = new Bitmap(newSize.Width, newSize.Height);
            BitmapData destBitmapData = destinationBitmap.LockBits(new Rectangle(0, 0, destinationBitmap.Width, destinationBitmap.Height), ImageLockMode.WriteOnly, destinationBitmap.PixelFormat);
            BitmapData srcBitmapData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, sourceBitmap.PixelFormat);
            int destBytesPerPixel = Bitmap.GetPixelFormatSize(destBitmapData.PixelFormat) / 8;
            int destWidthInBytes = destBitmapData.Width * destBytesPerPixel;
            byte* destPtrFirstPixel = (byte*)destBitmapData.Scan0;
            int srcBytesPerPixel = Bitmap.GetPixelFormatSize(srcBitmapData.PixelFormat) / 8;
            int srcWidthInBytes = srcBitmapData.Width * srcBytesPerPixel;
            byte* srcPtrFirstPixel = (byte*)srcBitmapData.Scan0;
            SizeF scaleFactor = new SizeF((float)newSize.Width / sourceBitmap.Width, (float)newSize.Height / sourceBitmap.Height);
            Parallel.For(0, destBitmapData.Height, i =>
            {
                byte* destCurrentLine = destPtrFirstPixel + (i * destBitmapData.Stride);
                for (int j = 0; j < destWidthInBytes; j += destBytesPerPixel)
                {
                    Point srcPoint = new Point(Convert.ToInt32(Math.Floor((j / destBytesPerPixel) / scaleFactor.Width)), Convert.ToInt32(Math.Floor(i / scaleFactor.Height)));
                    destCurrentLine[j] = (srcPtrFirstPixel + srcPoint.Y * srcBitmapData.Stride)[srcPoint.X * srcBytesPerPixel];
                    destCurrentLine[j + 1] = (srcPtrFirstPixel + srcPoint.Y * srcBitmapData.Stride)[srcPoint.X * srcBytesPerPixel + 1];
                    destCurrentLine[j + 2] = (srcPtrFirstPixel + srcPoint.Y * srcBitmapData.Stride)[srcPoint.X * srcBytesPerPixel + 2];
                }
            });
            destinationBitmap.UnlockBits(destBitmapData);
            sourceBitmap.UnlockBits(srcBitmapData);
            return destinationBitmap;
        }

        public static Bitmap Cut(Bitmap sourceBitmap, Rectangle rectangle)
        {
            GC.Collect();
            Bitmap destinationBitmap = sourceBitmap.Clone(rectangle, sourceBitmap.PixelFormat);
            return destinationBitmap;
        }

        public static List<Bitmap> Cut(Bitmap sourceBitmap, List<Rectangle> rectangles)
        {
            List<Bitmap> destinationBitmaps = new List<Bitmap>();
            foreach (Rectangle rectangle in rectangles)
            {
                destinationBitmaps.Add(Cut(sourceBitmap, rectangle));
            }
            return destinationBitmaps;
        }
    }
}
