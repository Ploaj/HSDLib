using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace System.Drawing
{
    public static class BitmapExt
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static byte[] GetBGRAData(this Bitmap bmp)
        {
            var bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var length = bitmapData.Stride * bitmapData.Height;

            byte[] bytes = new byte[length];

            Marshal.Copy(bitmapData.Scan0, bytes, 0, length);
            bmp.UnlockBits(bitmapData);

            return bytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static byte[] GetRGBAData(this Bitmap bmp)
        {
            var bytes = bmp.GetBGRAData();

            for(int i = 0; i < bytes.Length; i+=4)
            {
                var temp = bytes[i];
                bytes[i] = bytes[i + 2];
                bytes[i + 2] = temp;
            }

            return bytes;
        }

    }
}

namespace HSDRawViewer.Tools
{
    public class BitmapTools
    {
        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static Bitmap Multiply(Bitmap bitmap, byte r, byte g, byte b, PixelFormat format = PixelFormat.Format32bppArgb)
        {
            var size = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var bitmapData = bitmap.LockBits(size, ImageLockMode.ReadOnly, format);

            var buffer = new byte[bitmapData.Stride * bitmapData.Height];
            Marshal.Copy(bitmapData.Scan0, buffer, 0, buffer.Length);
            bitmap.UnlockBits(bitmapData);

            byte Calc(byte c1, byte c2)
            {
                var cr = c1 / 255d * c2 / 255d * 255d;
                return (byte)(cr > 255 ? 255 : cr);
            }

            for (var i = 0; i < buffer.Length; i += 4)
            {
                buffer[i] = Calc(buffer[i], b);
                buffer[i + 1] = Calc(buffer[i + 1], g);
                buffer[i + 2] = Calc(buffer[i + 2], r);
            }

            var result = new Bitmap(bitmap.Width, bitmap.Height);
            var resultData = result.LockBits(size, ImageLockMode.WriteOnly, format);

            Marshal.Copy(buffer, 0, resultData.Scan0, buffer.Length);
            result.UnlockBits(resultData);

            return result;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        public static Bitmap ReduceColors(Bitmap bmp, int colorCount)
        {
            if (bmp.Width <= 16 && bmp.Height <= 16) // no need
                return bmp;

            var bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var length = bitmapData.Stride * bitmapData.Height;

            byte[] bytes = new byte[length];
            Marshal.Copy(bitmapData.Scan0, bytes, 0, length);


            // edit bytes
            var quantizer = new SimplePaletteQuantizer.Quantizers.XiaolinWu.WuColorQuantizer();
            quantizer.Prepare(bmp.Width, bmp.Height);
            for (int i = 0; i < bytes.Length; i+=4)
            {
                var color = Color.FromArgb(bytes[i + 3], bytes[i], bytes[i + 1], bytes[i + 2]);
                quantizer.AddColor(color);
            }

            var palette = quantizer.GetPalette(colorCount);

            for (int i = 0; i < bytes.Length; i += 4)
            {
                var color = Color.FromArgb(bytes[i + 3], bytes[i], bytes[i + 1], bytes[i + 2]);
                var index = quantizer.GetPaletteIndex(color);
                var newColor = palette[index];
                bytes[i] = newColor.R;
                bytes[i + 1] = newColor.G;
                bytes[i + 2] = newColor.B;
                bytes[i + 3] = newColor.A;
                if (newColor.A != 255)
                    System.Console.WriteLine(newColor.A);
            }

            Marshal.Copy(bytes, 0, bitmapData.Scan0, length);
            bmp.UnlockBits(bitmapData);

            return bmp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap RGBAToBitmap(byte[] data, int width, int height)
        {
            for (int i = 0; i < data.Length; i += 4)
            {
                var temp = data[i];
                data[i] = data[i + 2];
                data[i + 2] = temp;
            }

            return BGRAToBitmap(data, width, height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap BGRAToBitmap(byte[] data, int width, int height)
        {
            if (width == 0) width = 1;
            if (height == 0) height = 1;

            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                BitmapData bmpData = bmp.LockBits(
                                     new Rectangle(0, 0, bmp.Width, bmp.Height),
                                     ImageLockMode.WriteOnly, bmp.PixelFormat);

                Marshal.Copy(data, 0, bmpData.Scan0, data.Length);
                bmp.UnlockBits(bmpData);
            }
            catch { bmp.Dispose(); throw; }

            return bmp;
        }

        /// <summary>
        /// 
        /// </summary>
        /*public static void GenerateCSP(string datPath, string yamlPath)
        {
            using (JobjEditor editor = new JobjEditor())
            {
                HSDRawFile file = new HSDRawFile(datPath);

                foreach(var root in file.Roots)
                {
                    if (root.Data is HSD_JOBJ jobj)
                        editor.SetJOBJ(jobj);

                    if (root.Data is HSD_MatAnimJoint matAnim)
                        editor.LoadAnimation(matAnim);
                }

                editor.LoadSceneYAML(yamlPath);

                editor.TakeScreenShot();
            }
        }*/
    }
}
