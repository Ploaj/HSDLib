﻿using HSDRaw.Common;
using HSDRaw.GX;
using System.Drawing;
using System.Runtime.InteropServices;
using DirectXTexNet;
using System.Windows.Forms;
using HSDRawViewer.GUI;
using System.IO;
using System;

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
        /// Import TOBJ from PNG file 
        /// </summary>
        /// <returns></returns>
        public static HSD_TOBJ ImportTOBJFromFile(string filePath, GXTexFmt imgFmt, GXTlutFmt tlutFmt)
        {
            var TOBJ = new HSD_TOBJ()
            {
                MagFilter = GXTexFilter.GX_LINEAR,
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

            InjectBitmap(TOBJ, filePath, imgFmt, tlutFmt);

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

            var f = Tools.FileIO.OpenFile("PNG (.png)|*.png");
            if (f != null)
            {
                using (TextureImportDialog settings = new TextureImportDialog())
                {
                    if (settings.ShowDialog() == DialogResult.OK)
                    {
                        InjectBitmap(TOBJ, f, settings.TextureFormat, settings.PaletteFormat);
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