using HSDRaw.GX;
using HSDRaw.Tools;
using System;
using System.ComponentModel;

namespace HSDRaw.Common
{
    [Flags]
    public enum TOBJ_FLAGS
    {
        COORD_UV = (0 << 0),
        COORD_REFLECTION = (1 << 0),
        COORD_HILIGHT = (2 << 0),
        COORD_SHADOW = (3 << 0),
        COORD_TOON = (4 << 0),
        COORD_GRADATION = (5 << 0),
        LIGHTMAP_DIFFUSE = (1 << 4),
        LIGHTMAP_SPECULAR = (1 << 5),
        LIGHTMAP_AMBIENT = (1 << 6),
        LIGHTMAP_EXT = (1 << 7),
        LIGHTMAP_SHADOW = (1 << 8),
        //COLORMAP_NONE = (0 << 16),
        COLORMAP_ALPHA_MASK = (1 << 16),
        COLORMAP_RGB_MASK = (2 << 16),
        COLORMAP_BLEND = (3 << 16),
        COLORMAP_MODULATE = (4 << 16),
        COLORMAP_REPLACE = (5 << 16),
        COLORMAP_PASS = (6 << 16),
        COLORMAP_ADD = (7 << 16),
        COLORMAP_SUB = (8 << 16),
        //ALPHAMAP_NONE = (0 << 20),
        ALPHAMAP_ALPHA_MASK = (1 << 20),
        ALPHAMAP_BLEND = (2 << 20),
        ALPHAMAP_MODULATE = (3 << 20),
        ALPHAMAP_REPLACE = (4 << 20),
        ALPHAMAP_PASS = (5 << 20),
        ALPHAMAP_ADD = (6 << 20),
        ALPHAMAP_SUB = (7 << 20),
        BUMP = (1 << 24),
        MTX_DIRTY = (1 << 31)
    }

    /// <summary>
    /// 
    /// </summary>
    public enum COORD_TYPE
    {
        UV,
        REFLECTION,
        HILIGHT,
        SHADOW,
        TOON,
        GRADATION
    }

    /// <summary>
    /// 
    /// </summary>
    public enum COLORMAP
    {
        NONE,
        ALPHA_MASK,
        RGB_MASK,
        BLEND,
        MODULATE,
        REPLACE,
        PASS,
        ADD,
        SUB,
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ALPHAMAP
    {
        NONE,
        ALPHA_MASK,
        BLEND,
        MODULATE,
        REPLACE,
        PASS,
        ADD,
        SUB,
    }

    /// <summary>
    /// Texture Object
    /// Contains surface information
    /// </summary>
    public class HSD_TOBJ : HSDListAccessor<HSD_TOBJ>
    {
        public override int TrimmedSize { get; } = 0x5C;

        [Category("0 - General")]
        public string ClassName
        {
            get => _s.GetString(0x00);
            set => _s.SetString(0x00, value);
        }

        [Browsable(false)]
        public override HSD_TOBJ Next { get => _s.GetReference<HSD_TOBJ>(0x04); set => _s.SetReference(0x04, value); }


        [Category("0 - General"), Description("Texture ID used for binding texture.")]
        public GXTexMapID TexMapID { get => (GXTexMapID)_s.GetInt32(0x08); set => _s.SetInt32(0x08, (int)value); }

        [Category("0 - General"), Description("Used for generating mipmaps. (Use 4)")]
        public int GXTexGenSrc { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }


        [Category("0 - General"), Description("Amount to scale UVs (U) by when applied to a model")]
        public byte WScale { get => _s.GetByte(0x3C); set => _s.SetByte(0x3C, value); }

        [Category("0 - General"), Description("Amount to scale UVs (V) by when applied to a model")]
        public byte HScale { get => _s.GetByte(0x3D); set => _s.SetByte(0x3D, value); }


        [Category("2- Transform")]
        public float RX { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }
        [Category("2- Transform")]
        public float RY { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }
        [Category("2- Transform")]
        public float RZ { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }
        [Category("2- Transform")]
        public float SX { get => _s.GetFloat(0x1C); set => _s.SetFloat(0x1C, value); }
        [Category("2- Transform")]
        public float SY { get => _s.GetFloat(0x20); set => _s.SetFloat(0x20, value); }
        [Category("2- Transform")]
        public float SZ { get => _s.GetFloat(0x24); set => _s.SetFloat(0x24, value); }
        [Category("2- Transform")]
        public float TX { get => _s.GetFloat(0x28); set => _s.SetFloat(0x28, value); }
        [Category("2- Transform")]
        public float TY { get => _s.GetFloat(0x2C); set => _s.SetFloat(0x2C, value); }
        [Category("2- Transform")]
        public float TZ { get => _s.GetFloat(0x30); set => _s.SetFloat(0x30, value); }


        [Browsable(false)]
        public TOBJ_FLAGS Flags { get => (TOBJ_FLAGS)_s.GetInt32(0x40); set => _s.SetInt32(0x40, (int)value); }


        [Category("1 - Rendering"), Description("Indicates if texture map is used for Diffuse lighting.")]
        public bool DiffuseLightmap { get => Flags.HasFlag(TOBJ_FLAGS.LIGHTMAP_DIFFUSE); set { if (value) Flags |= TOBJ_FLAGS.LIGHTMAP_DIFFUSE; else Flags &= ~TOBJ_FLAGS.LIGHTMAP_DIFFUSE; } }

        [Category("1 - Rendering"), Description("Indicates if texture map is used for Specular lighting.")]
        public bool SpecularLightmap { get => Flags.HasFlag(TOBJ_FLAGS.LIGHTMAP_SPECULAR); set { if (value) Flags |= TOBJ_FLAGS.LIGHTMAP_SPECULAR; else Flags &= ~TOBJ_FLAGS.LIGHTMAP_SPECULAR; } }

        [Category("1 - Rendering"), Description("Indicates if texture map is used for Ambient lighting.")]
        public bool AmbientLightmap { get => Flags.HasFlag(TOBJ_FLAGS.LIGHTMAP_AMBIENT); set { if (value) Flags |= TOBJ_FLAGS.LIGHTMAP_AMBIENT; else Flags &= ~TOBJ_FLAGS.LIGHTMAP_AMBIENT; } }

        [Category("1 - Rendering"), Description("Indicates if texture map is used for Shadow mapping.")]
        public bool ShadowLightmap { get => Flags.HasFlag(TOBJ_FLAGS.LIGHTMAP_SHADOW); set { if (value) Flags |= TOBJ_FLAGS.LIGHTMAP_SHADOW; else Flags &= ~TOBJ_FLAGS.LIGHTMAP_SHADOW; } }

        [Category("1 - Rendering"), Description("Indicates if texture map is used for ext lighting.")]
        public bool ExtLightmap { get => Flags.HasFlag(TOBJ_FLAGS.LIGHTMAP_EXT); set { if (value) Flags |= TOBJ_FLAGS.LIGHTMAP_EXT; else Flags &= ~TOBJ_FLAGS.LIGHTMAP_EXT; } }

        [Category("1 - Rendering"), Description("The color operation this to perform with this texture.")]
        public COLORMAP ColorOperation
        {
            get => (COLORMAP)((((int)Flags) >> 16) & 0xF);
            set
            {
                Flags = (TOBJ_FLAGS)(((int)Flags) & ~(0xF << 16) | (((int)value & 0xF) << 16));
            }
        }
        [Category("1 - Rendering"), Description("The alpha operation this to perform with this texture.")]
        public ALPHAMAP AlphaOperation
        {
            get => (ALPHAMAP)((((int)Flags) >> 20) & 0xF);
            set
            {
                Flags = (TOBJ_FLAGS)(((int)Flags) & ~(0xF << 20) | (((int)value & 0xF) << 20));
            }
        }
        [Category("1 - Rendering"), Description("Indicates whether or not this texture is a bump map")]
        public bool BumpMap { get => Flags.HasFlag(TOBJ_FLAGS.BUMP); set { if (value) Flags |= TOBJ_FLAGS.BUMP; else Flags &= ~TOBJ_FLAGS.BUMP; } }


        [Category("1 - Rendering"), Description("Horizontal wrap mode for the texture.")]
        public GXWrapMode WrapS { get => (GXWrapMode)_s.GetInt32(0x34); set => _s.SetInt32(0x34, (int)value); }

        [Category("1 - Rendering"), Description("Vertical wrap mode for the texture.")]
        public GXWrapMode WrapT { get => (GXWrapMode)_s.GetInt32(0x38); set => _s.SetInt32(0x38, (int)value); }



        [Category("1 - Rendering"), Description("Blending value to use when ColorOperation or AlphaOperation is set to BLEND.")]
        public float Blending { get => _s.GetFloat(0x44); set => _s.SetFloat(0x44, value); }

        [Category("1 - Rendering"), Description("Texture magnification filtering.")]
        public GXTexFilter MagFilter { get => (GXTexFilter)_s.GetInt32(0x48); set => _s.SetInt32(0x48, (int)value); }


        [Browsable(false)]
        public HSD_Image ImageData { get => _s.GetReference<HSD_Image>(0x4C); set => _s.SetReference(0x4C, value); }

        [Browsable(false)]
        public HSD_Tlut TlutData { get => _s.GetReference<HSD_Tlut>(0x50); set => _s.SetReference(0x50, value); }


        [Category("3 - Ext"), Description("Defines level-of-detail texture information.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public HSD_TOBJ_LOD LOD { get => _s.GetReference<HSD_TOBJ_LOD>(0x54); set => _s.SetReference(0x54, value); }

        [Category("3 - Ext"), Description("Texture Environment Unit (TEV) operations for this texture.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public HSD_TOBJ_TEV TEV { get => _s.GetReference<HSD_TOBJ_TEV>(0x58); set => _s.SetReference(0x58, value); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>byte array containing rgba pixel values for the texture</returns>
        public byte[] GetDecodedImageData()
        {
            if(ImageData != null && TlutData != null)
                return GXImageConverter.DecodeTPL(ImageData.Format, ImageData.Width, ImageData.Height, ImageData.ImageData, TlutData.Format, TlutData.ColorCount, TlutData.TlutData);

            if (ImageData != null && TlutData == null)
                return GXImageConverter.DecodeTPL(ImageData.Format, ImageData.Width, ImageData.Height, ImageData.ImageData);

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rgba"></param>
        /// <param name="format"></param>
        /// <param name="palFormat"></param>
        public void EncodeImageData(byte[] data, int width, int height, GXTexFmt format, GXTlutFmt palFormat)
        {
            byte[] palData;
            var encodedData = GXImageConverter.EncodeImage(data, width, height, format, palFormat, out palData);

            if (GXImageConverter.IsPalettedFormat(format))
            {
                if (TlutData == null)
                    TlutData = new HSD_Tlut();
                TlutData.Format = palFormat;
                TlutData.TlutData = palData;
                TlutData.ColorCount = (short)(palData.Length / 2);
            }
            else
                TlutData = null;

            if (ImageData == null)
                ImageData = new HSD_Image();
            ImageData.ImageData = encodedData;
            ImageData.Width = (short)width;
            ImageData.Height = (short)height;
            ImageData.Format = format;
        }

        public override void New()
        {
            base.New();
            SX = 1;
            SY = 1;
            SZ = 1;
            WScale = 1;
            HScale = 1;
        }

        public override bool Equals(object obj)
        {
            if (obj is HSDAccessor acc)
                return _s.Equals(acc._s);

            return false;
        }

        public override int GetHashCode()
        {
            return _s.GetHashCode();
        }
    }

    public class HSD_Image : HSDAccessor
    {
        public override int TrimmedSize { get; } = 0x18;

        public byte[] ImageData { get => _s.GetBuffer(0x00); set => _s.SetBuffer(0x00, value); }

        public short Width { get => _s.GetInt16(0x04); set => _s.SetInt16(0x04, value); }
        public short Height { get => _s.GetInt16(0x06); set => _s.SetInt16(0x06, value); }

        public GXTexFmt Format { get => (GXTexFmt)_s.GetInt32(0x08); set => _s.SetInt32(0x08, (int)value); }

        public int MipMap { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

        public float MinLOD { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }
        public float MaxLOD { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }

        protected override int Trim()
        {
            if (_s.References.ContainsKey(0x00))
                _s.References[0x00].IsBufferAligned = true;

            Console.WriteLine("Trimmed Image " + Width + " " + Height);

            return base.Trim();
        }
    }

    public class HSD_Tlut : HSDAccessor
    {
        public override int TrimmedSize { get; } = 0x20;// actually 0xE, but it's padded to 4 anyway...

        public byte[] TlutData { get => _s.GetBuffer(0x00); set => _s.SetBuffer(0x00, value); }
        
        public GXTlutFmt Format { get => (GXTlutFmt)_s.GetInt32(0x04); set => _s.SetInt32(0x04, (int)value); }

        public int GXTlut { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public short ColorCount { get => _s.GetInt16(0x0C); set => _s.SetInt16(0x0C, value); }

        protected override int Trim()
        {
            if (_s.References.ContainsKey(0x00))
                _s.References[0x00].IsBufferAligned = true;

            return base.Trim();
        }
    }

    public class HSD_TOBJ_LOD : HSDAccessor
    {
        public override int TrimmedSize { get; } = 0x10;

        public GXTexFilter MinFilter { get => (GXTexFilter)_s.GetInt32(0x00); set => _s.SetInt32(0x00, (int)value); }
        public float Bias { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }
        public bool BiasClamp { get => _s.GetByte(0x08) == 1; set => _s.SetByte(0x08, (byte)(value ? 1 : 0)); }
        public bool EnableEdgeLOD { get => _s.GetByte(0x09) == 1; set => _s.SetByte(0x09, (byte)(value ? 1 : 0)); }
        public GXAnisotropy Anisotropy { get => (GXAnisotropy)_s.GetInt32(0x0A); set => _s.SetInt32(0x0A, (int)value); }
    }
}
