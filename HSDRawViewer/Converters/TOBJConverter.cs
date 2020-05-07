using HSDRaw.Common;
using HSDRaw.GX;
using System.Drawing;
using System.Runtime.InteropServices;
using DirectXTexNet;
using System.Windows.Forms;
using HSDRawViewer.GUI;
using System.IO;
using System;
using nQuant;

namespace HSDRawViewer.Converters
{
    public class TOBJConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Bitmap LoadBitmapFromFile(string path)
        {
            // check for alpha channel bitmap and manually import it
            if(Path.GetExtension(path).ToLower().Equals(".bmp"))
            {
                using (FileStream stream = new FileStream(path, FileMode.Open))
                using (BinaryReader r = new BinaryReader(stream))
                {
                    if (new string(r.ReadChars(2)) == "BM")
                    {
                        var fSize = r.ReadUInt32();
                        r.ReadInt32(); // reserved
                        var dataOffset = r.ReadUInt32();

                        var infoSize = r.ReadUInt32();
                        var width = r.ReadInt32();
                        var height = r.ReadInt32();
                        var planes = r.ReadInt16();
                        var bpp = r.ReadInt16();

                        // only do this if this image is 32 bpp
                        if (bpp == 32)
                        {
                            var comp = r.ReadInt32();

                            if (comp != 0)
                                throw new NotSupportedException("Compressed 32 bpp bitmap not supported");

                            var imageSize = r.ReadInt32();

                            r.BaseStream.Position = dataOffset;

                            var data = r.ReadBytes(imageSize);
                            var scan = new byte[data.Length];

                            for (int w = 0; w < width; w++)
                                for (int h = 0; h < height; h++)
                                {
                                    var i = w + h * width;
                                    var d = w + (height - 1 - h) * width;
                                    i *= 4;
                                    d *= 4;
                                    scan[i] = data[d];
                                    scan[i + 1] = data[d + 1];
                                    scan[i + 2] = data[d + 2];
                                    scan[i + 3] = data[d + 3];
                                }

                            return RgbaToImage(scan, width, height);
                        }
                    }
                }
            }

            // otherwise use dotnet's
            return new Bitmap(path);
        }

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
        /// Import TOBJ from PNG file 
        /// </summary>
        /// <returns></returns>
        public static HSD_TOBJ ImportTOBJFromFile(string filePath, GXTexFmt imgFmt, GXTlutFmt tlutFmt)
        {
            var TOBJ = new HSD_TOBJ()
            {
                MagFilter = GXTexFilter.GX_LINEAR,
                Flags = TOBJ_FLAGS.COORD_UV | TOBJ_FLAGS.LIGHTMAP_DIFFUSE | TOBJ_FLAGS.COLORMAP_MODULATE | TOBJ_FLAGS.ALPHAMAP_MODULATE,
                HScale = 1,
                WScale = 1,
                WrapS = GXWrapMode.CLAMP,
                WrapT = GXWrapMode.CLAMP,
                SX = 1,
                SY = 1,
                SZ = 1,
                GXTexGenSrc = 4,
                Blending = 1
            };

            InjectBitmap(filePath, TOBJ, imgFmt, tlutFmt);

            return TOBJ;
        }

        /// <summary>
        /// Import TOBJ from PNG file 
        /// </summary>
        /// <returns></returns>
        public static HSD_TOBJ ImportTOBJFromFile()
        {
            var TOBJ = new HSD_TOBJ()
            {
                MagFilter = GXTexFilter.GX_LINEAR,
                Flags = TOBJ_FLAGS.COORD_UV | TOBJ_FLAGS.LIGHTMAP_DIFFUSE | TOBJ_FLAGS.COLORMAP_MODULATE | TOBJ_FLAGS.ALPHAMAP_MODULATE,
                HScale = 1,
                WScale = 1,
                WrapS = GXWrapMode.CLAMP,
                WrapT = GXWrapMode.CLAMP,
                SX = 1,
                SY = 1,
                SZ = 1,
                GXTexGenSrc = 4,
                Blending = 1
            };

            var f = Tools.FileIO.OpenFile(ApplicationSettings.ImageFileFilter);
            if (f != null)
            {
                using (TextureImportDialog settings = new TextureImportDialog())
                {
                    if (settings.ShowDialog() == DialogResult.OK)
                        using (Bitmap bmp = new Bitmap(f))
                        {
                            settings.ApplySettings(bmp);
                            TOBJConverter.InjectBitmap(bmp, TOBJ, settings.TextureFormat, settings.PaletteFormat);
                            return TOBJ;
                        }
                }
            }

            return null;
        }

        /// <summary>
        /// Injects <see cref="Bitmap"/> into <see cref="HSD_TOBJ"/>
        /// </summary>
        /// <param name="tobj"></param>
        /// <param name="img"></param>
        /// <param name="imgFormat"></param>
        /// <param name="palFormat"></param>
        public static void InjectBitmap(string filepath, HSD_TOBJ tobj)
        {
            using (Bitmap bmp = LoadBitmapFromFile(filepath))
            {
                using (TextureImportDialog settings = new TextureImportDialog())
                {
                    if (settings.ShowDialog() == DialogResult.OK)
                    {
                        settings.ApplySettings(bmp);

                        InjectBitmap(bmp, tobj, settings.TextureFormat, settings.PaletteFormat);
                    }
                }
            }
        }

        /// <summary>
        /// Injects <see cref="Bitmap"/> into <see cref="HSD_TOBJ"/>
        /// </summary>
        /// <param name="tobj"></param>
        /// <param name="img"></param>
        /// <param name="imgFormat"></param>
        /// <param name="palFormat"></param>
        public static void InjectBitmap(string filepath, HSD_TOBJ tobj,GXTexFmt imgFormat, GXTlutFmt palFormat)
        {
            using (Bitmap bmp = LoadBitmapFromFile(filepath))
            {
                InjectBitmap(bmp, tobj, imgFormat, palFormat);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        public static HSD_TOBJ BitmapToTOBJ(Bitmap bmp, GXTexFmt imgFormat, GXTlutFmt palFormat)
        {
            var TOBJ = new HSD_TOBJ()
            {
                MagFilter = GXTexFilter.GX_LINEAR,
                Flags = TOBJ_FLAGS.COORD_UV | TOBJ_FLAGS.LIGHTMAP_DIFFUSE | TOBJ_FLAGS.COLORMAP_MODULATE | TOBJ_FLAGS.ALPHAMAP_MODULATE,
                HScale = 1,
                WScale = 1,
                WrapS = GXWrapMode.CLAMP,
                WrapT = GXWrapMode.CLAMP,
                SX = 1,
                SY = 1,
                SZ = 1,
                GXTexGenSrc = 4,
                Blending = 1
            };

            InjectBitmap(bmp, TOBJ, imgFormat, palFormat);

            return TOBJ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tobj"></param>
        /// <param name="bmp"></param>
        /// <param name="imgFormat"></param>
        /// <param name="palFormat"></param>
        public static void InjectBitmap(Bitmap bmp, HSD_TOBJ tobj, GXTexFmt imgFormat, GXTlutFmt palFormat)
        {
            if (imgFormat != GXTexFmt.CMP)
            {
               // if (imgFormat == GXTexFmt.CI8) // doesn't work well with alpha
               //     bmp = ReduceColors(bmp, 256);
                if (imgFormat == GXTexFmt.CI4 || imgFormat == GXTexFmt.CI14X2)
                    bmp = ReduceColors(bmp, 16);

                var bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                var length = bitmapData.Stride * bitmapData.Height;

                byte[] bytes = new byte[length];

                Marshal.Copy(bitmapData.Scan0, bytes, 0, length);
                bmp.UnlockBits(bitmapData);

                tobj.EncodeImageData(bytes, bmp.Width, bmp.Height, imgFormat, palFormat);

                // dispose if we use our color reduced bitmap
                if (imgFormat == GXTexFmt.CI8 || imgFormat == GXTexFmt.CI4 || imgFormat == GXTexFmt.CI14X2)
                    bmp.Dispose();
            }
            else
            {
                MemoryStream stream = new MemoryStream();

                bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                byte[] bytes = stream.ToArray();
                stream.Close();
                stream.Dispose();

                IntPtr unmanagedPointer = Marshal.AllocHGlobal(bytes.Length);
                Marshal.Copy(bytes, 0, unmanagedPointer, bytes.Length);

                using (var origImage = TexHelper.Instance.LoadFromWICMemory(unmanagedPointer, bytes.Length, WIC_FLAGS.NONE))
                {
                    var scratch = origImage.Compress(0, DXGI_FORMAT.BC1_UNORM, TEX_COMPRESS_FLAGS.DEFAULT, 1);
                    var ptr = scratch.GetPixels();
                    var length = scratch.GetPixelsSize();
                    byte[] data = new byte[length];

                    Marshal.Copy(ptr, data, 0, (int)length);

                    scratch.Dispose();

                    tobj.EncodeImageData(data, bmp.Width, bmp.Height, GXTexFmt.CMP, GXTlutFmt.IA8);
                }

                // Call unmanaged code
                Marshal.FreeHGlobal(unmanagedPointer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        private static Bitmap ReduceColors(Bitmap bitmap, int colorCount)
        {
            if (bitmap.Width <= 16 && bitmap.Height <= 16) // no need
                return bitmap;

            var quantizer = new WuQuantizer();
            using (var quantized = quantizer.QuantizeImage(bitmap, 10, 70, colorCount))
            {
                return new Bitmap(quantized);
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
