using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Support
{
    public class Imaging
    {
        public static Bitmap RotateImage(Image image, float angle)
        {
            double num9;
            double num10;
            double num11;
            double num12;
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }
            double width = image.Width;
            double height = image.Height;
            double num3 = (angle * Math.PI) / 180;
            double d = num3;
            while (d < 0)
            {
                d += 2 * Math.PI;
            }
            if (((d >= 0) && (d < Math.PI / 2)) || ((d >= Math.PI) && (d < Math.PI*1.5)))
            {
                num9 = Math.Abs(Math.Cos(d)) * width;
                num10 = Math.Abs(Math.Sin(d)) * width;
                num11 = Math.Abs(Math.Cos(d)) * height;
                num12 = Math.Abs(Math.Sin(d)) * height;
            }
            else
            {
                num9 = Math.Abs(Math.Sin(d)) * height;
                num10 = Math.Abs(Math.Cos(d)) * height;
                num11 = Math.Abs(Math.Sin(d)) * width;
                num12 = Math.Abs(Math.Cos(d)) * width;
            }
            double a = num9 + num12;
            double num6 = num11 + num10;
            int num7 = (int)Math.Ceiling(a);
            int num8 = (int)Math.Ceiling(num6);
            Bitmap bitmap = new Bitmap(num7, num8);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                Point[] pointArray;
                if ((d >= 0) && (d < Math.PI/2))
                {
                    pointArray = new Point[] { new Point((int)num12, 0), new Point(num7, (int)num10), new Point(0, (int)num11) };
                }
                else if ((d >= Math.PI / 2) && (d < Math.PI))
                {
                    pointArray = new Point[] { new Point(num7, (int)num10), new Point((int)num9, num8), new Point((int)num12, 0) };
                }
                else if ((d >= Math.PI) && (d < Math.PI*1.5))
                {
                    pointArray = new Point[] { new Point((int)num9, num8), new Point(0, (int)num11), new Point(num7, (int)num10) };
                }
                else
                {
                    pointArray = new Point[] { new Point(0, (int)num11), new Point((int)num12, 0), new Point((int)num9, num8) };
                }
                graphics.DrawImage(image, pointArray);
            }
            return bitmap;
        }

        public static void BmpAddFast(Bitmap from, Bitmap addTo, Rectangle area)
        {

            BitmapData bitmapdata = addTo.LockBits(area, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            byte[] destination = new byte[bitmapdata.Stride * bitmapdata.Height];
            Marshal.Copy(bitmapdata.Scan0, destination, 0, destination.Length);

            BitmapData bitmapdata2 = from.LockBits(new Rectangle(0,0,from.Width, from.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            byte[] source = new byte[bitmapdata2.Stride * bitmapdata2.Height];
            Marshal.Copy(bitmapdata2.Scan0, source, 0, source.Length);
            from.UnlockBits(bitmapdata2);

            int num = 0;
            for (int i = from.Height - 1; i >= 0; i--)
            {
                for (int j = 0; j < from.Width; j++)
                {
                    destination[num] = source[num++];
                    destination[num] = source[num++];
                    destination[num] = source[num++];
                }
            }
            Marshal.Copy(destination, 0, bitmapdata.Scan0, destination.Length);
            addTo.UnlockBits(bitmapdata);
        }

        public static Bitmap BmpGetRegion(Bitmap from, Rectangle area)
        {

            Color bgra = new Color();
            Bitmap addTo = new Bitmap(area.Width, area.Height, PixelFormat.Format24bppRgb);
            BitmapData bitmapdata = addTo.LockBits(new Rectangle(0,0,area.Width, area.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            byte[] destination = new byte[bitmapdata.Stride * bitmapdata.Height];
            Marshal.Copy(bitmapdata.Scan0, destination, 0, destination.Length);
            int num = 0;
            for (int i = area.Y; i < from.Height && i<area.Bottom; i--)
            {
                for (int j = area.X; j < from.Width && j<area.Right; j++)
                {
                    bgra = from.GetPixel(j, i);
                    destination[num++] = bgra.B;
                    destination[num++] = bgra.G;
                    destination[num++] = bgra.R;
                }
            }
            Marshal.Copy(destination, 0, bitmapdata.Scan0, destination.Length);
            addTo.UnlockBits(bitmapdata);

            return addTo;
        }
    }

    struct ColorBgra
    {
        public byte A;
        public byte R;
        public byte B;
        public byte G;
        public Color ToColor()
        {
            return Color.FromArgb(A, R, G, B);
        }
    }
}
