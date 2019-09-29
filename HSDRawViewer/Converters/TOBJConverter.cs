using HSDRaw.Common;
using HSDRaw.GX;
using System.Drawing;
using System.Runtime.InteropServices;
using DirectXTexNet;

namespace HSDRawViewer.Converters
{
    public class TOBJConverter
    {
        /// <summary>
        /// Converts <see cref="HSD_TOBJ"/> into <see cref="Bitmap"/>
        /// </summary>
        /// <param name="tobj"></param>
        /// <returns></returns>
        public static Bitmap ToBitmap(HSD_TOBJ tobj)
        {
            var rgba = tobj.GetDecodedImageData();

            return RgbaToImage(rgba, tobj.ImageData.Width, tobj.ImageData.Height);
        }

        /// <summary>
        /// Injects <see cref="Bitmap"/> into <see cref="HSD_TOBJ"/>
        /// </summary>
        /// <param name="tobj"></param>
        /// <param name="img"></param>
        /// <param name="imgFormat"></param>
        /// <param name="palFormat"></param>
        public static void InjectBitmap(HSD_TOBJ tobj, string filepath, GXTexFmt imgFormat, GXTlutFmt palFormat)
        {
            Bitmap bmp = new Bitmap(filepath);
            InjectBitmap(tobj, bmp, imgFormat, palFormat);
            bmp.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tobj"></param>
        /// <param name="bmp"></param>
        /// <param name="imgFormat"></param>
        /// <param name="palFormat"></param>
        public static void InjectBitmap(HSD_TOBJ tobj, Bitmap bmp, GXTexFmt imgFormat, GXTlutFmt palFormat)
        {
            if (imgFormat != GXTexFmt.CMP)
            {
                var bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                var length = bitmapData.Stride * bitmapData.Height;

                byte[] bytes = new byte[length];

                Marshal.Copy(bitmapData.Scan0, bytes, 0, length);
                bmp.UnlockBits(bitmapData);

                tobj.EncodeImageData(bytes, bmp.Width, bmp.Height, imgFormat, palFormat);
            }
            else
            {
                var temp = System.IO.Path.GetTempFileName() + ".png";
                bmp.Save(temp);
                using (var origImage = TexHelper.Instance.LoadFromWICFile(temp, WIC_FLAGS.NONE))
                {
                    var scratch = origImage.Compress(0, DXGI_FORMAT.BC1_UNORM_SRGB, TEX_COMPRESS_FLAGS.DEFAULT, 1);
                    var ptr = scratch.GetPixels();
                    var length = scratch.GetPixelsSize();
                    byte[] data = new byte[length];

                    Marshal.Copy(ptr, data, 0, (int)length);

                    scratch.Dispose();

                    tobj.EncodeImageData(data, bmp.Width, bmp.Height, GXTexFmt.CMP, GXTlutFmt.IA8);
                }
                System.IO.File.Delete(temp);
            }
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
