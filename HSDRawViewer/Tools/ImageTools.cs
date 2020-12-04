using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace HSDRawViewer.Tools
{
    public class BitmapTools
    {
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
                var color = Color.FromArgb(bytes[i], bytes[i+1], bytes[i+2], bytes[i+3]);
                quantizer.AddColor(color);
            }

            var palette = quantizer.GetPalette(colorCount);

            for (int i = 0; i < bytes.Length; i += 4)
            {
                var color = Color.FromArgb(bytes[i], bytes[i + 1], bytes[i + 2], bytes[i + 3]);
                var index = quantizer.GetPaletteIndex(color);
                var newColor = palette[index];
                bytes[i] = newColor.A;
                bytes[i + 1] = newColor.R;
                bytes[i + 2] = newColor.G;
                bytes[i + 3] = newColor.B;
            }

            Marshal.Copy(bytes, 0, bitmapData.Scan0, length);
            bmp.UnlockBits(bitmapData);

            return bmp;

            /*using (var quantized = quantizer.QuantizeImage(bitmap, 10, 70, colorCount))
            {
                return new Bitmap(quantized);
            }*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap RgbaToImage(byte[] data, int width, int height)
        {
            if (width == 0) width = 1;
            if (height == 0) height = 1;

            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(
                                     new Rectangle(0, 0, bmp.Width, bmp.Height),
                                     System.Drawing.Imaging.ImageLockMode.WriteOnly, bmp.PixelFormat);

                System.Runtime.InteropServices.Marshal.Copy(data, 0, bmpData.Scan0, data.Length);
                bmp.UnlockBits(bmpData);
            }
            catch { bmp.Dispose(); throw; }

            return bmp;
        }
    }
}
