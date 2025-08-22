using HSDRaw.Common;
using HSDRaw.GX;
using HSDRawViewer.GUI.Dialog;
using HSDRawViewer.Tools;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HSDRawViewer
{
    public static class TOBJExtentions
    {

        /// <summary>
        /// Import TOBJ from PNG file 
        /// </summary>
        /// <returns></returns>
        public static HSD_TOBJ ImportTObjFromFile()
        {
            string f = FileIO.OpenFile(ApplicationSettings.ImageFileFilter);
            if (f != null)
            {
                using TextureImportDialog settings = new();
                if (FormatFromString(f, out GXTexFmt fmt, out GXTlutFmt pal))
                {
                    settings.PaletteFormat = pal;
                    settings.TextureFormat = fmt;
                }

                if (settings.ShowDialog() == DialogResult.OK)
                {
                    using Image<Bgra32> image = Image.Load<Bgra32>(f);
                    settings.ApplySettings(image);

                    HSD_TOBJ tobj = new()
                    {
                        MagFilter = GXTexFilter.GX_LINEAR,
                        Flags = TOBJ_FLAGS.COORD_UV | TOBJ_FLAGS.LIGHTMAP_DIFFUSE | TOBJ_FLAGS.COLORMAP_MODULATE | TOBJ_FLAGS.ALPHAMAP_MODULATE,
                        RepeatT = 1,
                        RepeatS = 1,
                        WrapS = GXWrapMode.CLAMP,
                        WrapT = GXWrapMode.CLAMP,
                        SX = 1,
                        SY = 1,
                        SZ = 1,
                        GXTexGenSrc = GXTexGenSrc.GX_TG_TEX0,
                        Blending = 1
                    };
                    tobj.InjectBitmap(image, settings.TextureFormat, settings.PaletteFormat);
                    return tobj;
                }
            }

            return null;
        }
        /// <summary>
        /// Import TOBJ from PNG file 
        /// </summary>
        /// <returns></returns>
        public static HSD_TOBJ ImportTObjFromFile(GXTexFmt fmt, GXTlutFmt pal)
        {
            string f = FileIO.OpenFile(ApplicationSettings.ImageFileFilter);
            return ImportTObjFromFile(f, fmt, pal);
        }
        /// <summary>
        /// Import TOBJ from PNG file 
        /// </summary>
        /// <returns></returns>
        public static HSD_TOBJ ImportTObjFromFile(string filePath, GXTexFmt imgFmt, GXTlutFmt tlutFmt)
        {
            HSD_TOBJ TOBJ = new()
            {
                MagFilter = GXTexFilter.GX_LINEAR,
                Flags = TOBJ_FLAGS.COORD_UV | TOBJ_FLAGS.LIGHTMAP_DIFFUSE | TOBJ_FLAGS.COLORMAP_MODULATE | TOBJ_FLAGS.ALPHAMAP_MODULATE,
                RepeatT = 1,
                RepeatS = 1,
                WrapS = GXWrapMode.CLAMP,
                WrapT = GXWrapMode.CLAMP,
                SX = 1,
                SY = 1,
                SZ = 1,
                GXTexGenSrc = GXTexGenSrc.GX_TG_TEX0,
                Blending = 1
            };

            TOBJ.InjectBitmap(filePath, imgFmt, tlutFmt);

            return TOBJ;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tobj"></param>
        /// <returns></returns>
        public static string FormatName(this HSD_TOBJ tobj, string name)
        {
            if (tobj.ImageData != null)
                name += "_" + tobj.ImageData.Format.ToString();

            if (tobj.TlutData != null)
                name += "_" + tobj.TlutData.Format.ToString();

            return name;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="texFmt"></param>
        /// <param name="palFmt"></param>
        public static bool FormatFromString(string name, out GXTexFmt texFmt, out GXTlutFmt palFmt)
        {
            texFmt = GXTexFmt.RGBA8;
            palFmt = GXTlutFmt.RGB5A3;

            string[] parts = Path.GetFileNameWithoutExtension(name).Split('_');

            bool foundFormat = false;

            foreach (string p in parts)
            {
                // skip numeric values
                if (int.TryParse(p, out int i))
                    continue;

                if (Enum.TryParse(p.ToUpper(), out GXTexFmt format))
                {
                    texFmt = format;
                    foundFormat = true;
                }

                if (Enum.TryParse(p.ToUpper(), out GXTlutFmt palFormat))
                    palFmt = palFormat;
            }

            return foundFormat;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tobj"></param>
        /// <returns></returns>
        public static bool IsTransparent(this HSD_TOBJ tobj)
        {
            if (tobj.ImageData.Format == GXTexFmt.RGB565)
                return false;

            if ((tobj.ImageData.Format == GXTexFmt.CI8 && tobj.TlutData.Format == GXTlutFmt.RGB565) ||
                (tobj.ImageData.Format == GXTexFmt.CI4 && tobj.TlutData.Format == GXTlutFmt.RGB565))
                return false;

            byte[] d = tobj.GetDecodedImageData();

            for (int i = 0; i < d.Length; i += 4)
            {
                if (d[i + 3] != 255)
                    return true;
            }

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tobj"></param>
        /// <returns></returns>
        public static Image<Bgra32> ToImage(this HSD_TOBJ tobj)
        {
            // Assuming you have a BGRA byte array
            byte[] bgraBytes = tobj.GetDecodedImageData();

            // Create an Image<Rgba32> object from the BGRA byte array
            return Image.LoadPixelData<Bgra32>(bgraBytes, tobj.ImageData.Width, tobj.ImageData.Height);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tobj"></param>
        /// <param name="path"></param>
        public static void SaveImagePNG(this HSD_TOBJ tobj)
        {
            string path = FileIO.SaveFile(ApplicationSettings.ImageFileFilter);
            if (!string.IsNullOrEmpty(path))
                tobj.SaveImagePNG(path);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tobj"></param>
        /// <param name="path"></param>
        public static void SaveImagePNG(this HSD_TOBJ tobj, string path)
        {
            // Save the image as a PNG file
            using Image<Bgra32> image = tobj.ToImage();
            using FileStream output = File.OpenWrite(path);
            image.SaveAsPng(output);
        }
        /// <summary>
        /// 
        /// </summary>
        public static void ImportImage(this HSD_TOBJ tobj, string filePath, GXTexFmt imgFmt, GXTlutFmt tlutFmt)
        {
            tobj.MagFilter = GXTexFilter.GX_LINEAR;
            tobj.Flags = TOBJ_FLAGS.COORD_UV | TOBJ_FLAGS.LIGHTMAP_DIFFUSE | TOBJ_FLAGS.COLORMAP_MODULATE | TOBJ_FLAGS.ALPHAMAP_MODULATE;
            tobj.RepeatT = 1;
            tobj.RepeatS = 1;
            tobj.WrapS = GXWrapMode.CLAMP;
            tobj.WrapT = GXWrapMode.CLAMP;
            tobj.SX = 1;
            tobj.SY = 1;
            tobj.SZ = 1;
            tobj.GXTexGenSrc = GXTexGenSrc.GX_TG_TEX0;
            tobj.Blending = 1;

            tobj.InjectBitmap(filePath, imgFmt, tlutFmt);
        }
        /// <summary>
        /// Injects <see cref="Bitmap"/> into <see cref="HSD_TOBJ"/>
        /// </summary>
        /// <param name="tobj"></param>
        /// <param name="img"></param>
        /// <param name="imgFormat"></param>
        /// <param name="palFormat"></param>
        public static void InjectBitmap(this HSD_TOBJ tobj, string filepath, GXTexFmt imgFormat, GXTlutFmt palFormat)
        {
            // format override
            if (FormatFromString(filepath, out GXTexFmt fmt, out GXTlutFmt pal))
            {
                palFormat = pal;
                imgFormat = fmt;
            }

            //
            using Image<Bgra32> image = Image.Load<Bgra32>(filepath);
            tobj.InjectBitmap(image, imgFormat, palFormat);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private static byte[] GetBGRA(this Image<Bgra32> image)
        {
            // Load your image from file or create it in some other way
            // Get the width and height of the image
            int width = image.Width;
            int height = image.Height;

            // Create a byte array to hold the BGRA data
            byte[] bgraBytes = new byte[width * height * 4];

            // Get the span of pixels from the image
            if (image.DangerousTryGetSinglePixelMemory(out Memory<Bgra32> pixelSpan))
            {
                // Copy the BGRA data from the image to the byte array
                Span<byte> byteSpan = MemoryMarshal.AsBytes(pixelSpan.Span);
                byteSpan.CopyTo(bgraBytes);
            }
            else
            {
                // Handle the case where getting the pixel span fails
                Console.WriteLine("Failed to get pixel span.");
                return null;
            }

            return bgraBytes;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tobj"></param>
        /// <param name="bmp"></param>
        /// <param name="imgFormat"></param>
        /// <param name="palFormat"></param>
        public static void InjectBitmap(this HSD_TOBJ tobj, Image<Bgra32> image, GXTexFmt imgFormat, GXTlutFmt palFormat)
        {
            tobj.EncodeImageData(image.GetBGRA(), image.Width, image.Height, imgFormat, palFormat);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="fmt"></param>
        /// <param name="pal"></param>
        /// <returns></returns>
        public static HSD_TOBJ ToTObj(this Image<Bgra32> image, GXTexFmt fmt, GXTlutFmt pal)
        {
            HSD_TOBJ tobj = new();
            tobj.InjectBitmap(image, fmt, pal);
            return tobj;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Bgra32? GetSolidColor(this Image<Bgra32> image)
        {
            Bgra32 firstPixel = image[0, 0];

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    if (!image[x, y].Equals(firstPixel))
                    {
                        return null; // Not a solid color
                    }
                }
            }

            return firstPixel; // Solid color detected
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static System.Drawing.Bitmap ToBitmap(this Image<Bgra32> image)
        {
            using MemoryStream ms = new();
            // Save ImageSharp image to a stream in a format that Bitmap can read
            image.Save(ms, new PngEncoder()); // PNG preserves transparency
            ms.Seek(0, SeekOrigin.Begin);

            // Load the stream as a Bitmap
            return new System.Drawing.Bitmap(ms);
        }
    }
}
