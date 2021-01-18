using HSDRaw.Common;
using HSDRaw.GX;
using System.Drawing;
using System.Windows.Forms;
using HSDRawViewer.GUI;
using System.IO;
using System;
using HSDRawViewer.Tools;

namespace HSDRawViewer.Converters
{
    public class TOBJConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tobj"></param>
        /// <returns></returns>
        public static string FormatName(string name, HSD_TOBJ tobj)
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

            var parts = Path.GetFileNameWithoutExtension(name).Split('_');

            bool foundFormat = false;

            foreach (var p in parts)
            {
                // skip numeric values
                if (int.TryParse(p, out int i))
                    continue;

                if(Enum.TryParse(p.ToUpper(), out GXTexFmt format))
                {
                    texFmt = format;
                    foundFormat = true;
                }

                if (Enum.TryParse(p.ToUpper(), out GXTlutFmt palFormat ))
                    palFmt = palFormat;
            }

            return foundFormat;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tobj"></param>
        /// <returns></returns>
        public static bool IsTransparent(HSD_TOBJ tobj)
        {
            if (tobj.ImageData.Format == GXTexFmt.RGB565)
                return false;

            if ((tobj.ImageData.Format == GXTexFmt.CI8 && tobj.TlutData.Format == GXTlutFmt.RGB565) ||
                (tobj.ImageData.Format == GXTexFmt.CI4 && tobj.TlutData.Format == GXTlutFmt.RGB565))
                return false;

            var d = tobj.GetDecodedImageData();
            
            for(int i = 0; i < d.Length; i += 4)
            {
                if (d[i + 3] != 255)
                    return true;
            }

            return false;
        }

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

                            return BitmapTools.BGRAToBitmap(scan, width, height);
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

            return BitmapTools.BGRAToBitmap(rgba, tobj.ImageData.Width, tobj.ImageData.Height);
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
                    if (FormatFromString(f, out GXTexFmt fmt, out GXTlutFmt pal))
                    {
                        settings.PaletteFormat = pal;
                        settings.TextureFormat = fmt;
                    }

                    if (settings.ShowDialog() == DialogResult.OK)
                        using (Bitmap bmp = new Bitmap(f))
                        {
                            settings.ApplySettings(bmp);
                            InjectBitmap(bmp, TOBJ, settings.TextureFormat, settings.PaletteFormat);
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
                    if (FormatFromString(filepath, out GXTexFmt fmt, out GXTlutFmt pal))
                    {
                        settings.PaletteFormat = pal;
                        settings.TextureFormat = fmt;
                    }

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
            // format override
            if (FormatFromString(filepath, out GXTexFmt fmt, out GXTlutFmt pal))
            {
                palFormat = pal;
                imgFormat = fmt;
            }

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
            tobj.EncodeImageData(bmp.GetBGRAData(), bmp.Width, bmp.Height, imgFormat, palFormat);
        }
    }
}
