using System;
using System.Drawing;

namespace HSDRawViewer.Converters.SBM
{
    public class CSPMaker
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        public static void MakeCSP(Bitmap bmp)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);
            
            IntPtr ptr = bmpData.Scan0;
            
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];
            
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            rgbValues = MakeCSP(rgbValues, bmp.Width, bmp.Height);
            
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            
            bmp.UnlockBits(bmpData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        private static byte Blend(byte c1, byte c2, float alpha)
        {
            var col = c1 * alpha + c2 * (1 - alpha);

            if (col > byte.MaxValue)
                col = byte.MaxValue;

            return (byte)col;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static byte[] MakeCSP(byte[] image, int width, int height)
        {
            byte[] shadow = new byte[image.Length];

            var sx = 22;
            var sy = 22;
            
            // Apply Outline
            for(int w = 0; w < width; w++)
            {
                for(int h = 0; h < height; h++)
                {
                    var index = (w + h * width) * 4;
                    var alpha = image[index + 3] / 255f;

                    // shadow opacity 109 11x11 pixels away
                    if (w - sx >= 0 && w - sx < width &&
                        h + sy >= 0 && h + sy < height)
                    {
                        var shadow_index = ((w - sx) + (h + sy) * width) * 4;
                        shadow[shadow_index + 3] = (byte)(109 * alpha);
                    }

                    image[index + 0] = Blend(image[index + 0], 0, alpha);
                    image[index + 1] = Blend(image[index + 1], 0, alpha);
                    image[index + 2] = Blend(image[index + 2], 0, alpha);
                    //image[index + 3] = (byte)(image[index + 3] * 2 > 255 ? 255 : image[index + 3] * 2);
                }
            }

            // Apply Shadow
            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    var index = (w + h * width) * 4;
                    var shadowalpha = shadow[index + 3] / 255f;
                    var alpha = image[index + 3] / 255f;

                    image[index + 3] = (byte)Math.Min(255, shadow[index + 3] + image[index + 3]);
                    image[index + 0] = Blend(image[index + 0], shadow[index + 0], alpha); 
                    image[index + 1] = Blend(image[index + 1], shadow[index + 1], alpha);
                    image[index + 2] = Blend(image[index + 2], shadow[index + 2], alpha);
                }
            }

            return image;
        }
    }
}
