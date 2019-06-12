using System;
using HSDLib.GX;
using HSDLib.Helpers;
using System.Drawing;

namespace HSDLib.Common
{
    [Flags]
    public enum TOBJ_FLAGS
    {
        COORD_UV = (0<<0),
        COORD_REFLECTION = (1<<0),
        COORD_HILIGHT = (2<<0),
        COORD_SHADOW = (3<<0),
        COORD_TOON = (4<<0),
        COORD_GRADATION = (5<<0),
        LIGHTMAP_DIFFUSE = (1<<4),
        LIGHTMAP_SPECULAR = (1<<5),
        LIGHTMAP_AMBIENT = (1<<6),
        LIGHTMAP_EXT = (1<<7),
        LIGHTMAP_SHADOW = (1<<8),
        COLORMAP_NONE = (0<<16),
        COLORMAP_ALPHA_MASK = (1<<16),
        COLORMAP_RGB_MASK = (2<<16),
        COLORMAP_BLEND = (3<<16),
        COLORMAP_MODULATE = (4<<16),
        COLORMAP_REPLACE = (5<<16),
        COLORMAP_PASS = (6<<16),
        COLORMAP_ADD = (7<<16),
        COLORMAP_SUB = (8<<16),
        ALPHAMAP_ALPHA_MASK = (1<<20),
        ALPHAMAP_BLEND = (2<<20),
        ALPHAMAP_MODULATE = (3<<20),
        ALPHAMAP_REPLACE = (4<<20),
        ALPHAMAP_PASS = (5<<20),
        ALPHAMAP_ADD = (6<<20),
        ALPHAMAP_SUB = (7<<20),
        BUMP = (1<<24),
        MTX_DIRTY = (1<<31)
    }

    public class HSD_TOBJ : IHSDList<HSD_TOBJ>
    {
        public uint NameOffset { get; set; }
        
        public override HSD_TOBJ Next { get; set; }
        
        public GXTexMapID TexMapID { get; set; }//= GXTexMapID.GX_TEXMAP0;
        
        public uint GXTexGenSrc { get; set; }//= 4;

        [HSDInLine()]
        public HSD_Transforms Transform { get; set; }//= new HSD_Transforms();
        
        public GXWrapMode WrapS { get; set; }//= GXWrapMode.REPEAT;
        
        public GXWrapMode WrapT { get; set; }//= GXWrapMode.REPEAT;
        
        public byte WScale { get; set; }//= 1;
        
        public byte HScale { get; set; }//= 1;
        
        public ushort Padding { get; set; }//= 1;
        
        public TOBJ_FLAGS Flags { get; set; }
        
        public float Blending { get; set; }//= 1;
        
        public GXTexFilter MagFilter { get; set; }//= GXTexFilter.GX_LINEAR;
        
        public HSD_Image ImageData { get; set; }//
        
        public HSD_Tlut Tlut { get; set; }//
        
        public HSD_TOBJ_LOD LOD { get; set; }//
        
        public HSD_TOBJ_TEV TEV { get; set; }

        /// <summary>
        /// Creates a <see cref="Bitmap"/> of this texture
        /// </summary>
        /// <returns></returns>
        public Bitmap ToBitmap()
        {
            if(ImageData != null && Tlut != null)
                return TPL.ConvertFromTextureMelee(ImageData.Data, ImageData.Width, ImageData.Height, (int)ImageData.Format, Tlut.Data, Tlut.ColorCount, (int)Tlut.Format);

            if (ImageData != null)
                return TPL.ConvertFromTextureMelee(ImageData.Data, ImageData.Width, ImageData.Height, (int)ImageData.Format, null, 0, 0);

            return null;
        }

        /// <summary>
        /// Sets the image data from a <see cref="Bitmap"/>
        /// </summary>
        /// <param name="b"></param>
        public void SetFromBitmap(Bitmap b, GXTexFmt imageFormat, GXTlutFmt paletteFormat)
        {
            byte[] palData;
            ImageData = new HSD_Image();
            ImageData.Width = (ushort)b.Width;
            ImageData.Height = (ushort)b.Height;
            ImageData.Format = imageFormat;
            ImageData.Data = TPL.ConvertToTextureMelee(b, (int)imageFormat, (int)paletteFormat, out palData);
            if(palData != null && palData.Length > 0)
            {
                Tlut = new HSD_Tlut();
                Tlut.Format = paletteFormat;
                Tlut.ColorCount = (ushort)(palData.Length / 2);
                Tlut.Data = palData;
            }
        }
    }

    public class HSD_Tlut : IHSDNode
    {
        public byte[] Data;
        public GXTlutFmt Format;
        public uint GXTlut;
        public ushort ColorCount;

        public override void Open(HSDReader Reader)
        {
            uint DataOffset = Reader.ReadUInt32();
            Format = (GXTlutFmt)Reader.ReadUInt32();
            GXTlut = Reader.ReadUInt32();
            ColorCount = Reader.ReadUInt16();
            Reader.ReadUInt16();

            Data = Reader.ReadBuffer(DataOffset, TPL.PaletteByteSize((TPL_PaletteFormat)Format, ColorCount));
        }

        public override void Save(HSDWriter Writer)
        {
            if (Data != null)
                Writer.WriteTexture(Data);

            Writer.AddObject(this);
            Writer.WritePointer(Data);
            Writer.Write((uint)Format);
            Writer.Write(GXTlut);
            Writer.Write(ColorCount);
            Writer.Write((ushort)0);
        }
    }

    public class HSD_Image : IHSDNode
    {
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public GXTexFmt Format { get; set; }
        public uint Mipmap;
        public float MinLOD;
        public float MaxLOD;
        public byte[] Data;

        public override void Open(HSDReader Reader)
        {
            uint DataOffset = Reader.ReadUInt32();
            Width = Reader.ReadUInt16();
            Height = Reader.ReadUInt16();
            Format = (GXTexFmt)Reader.ReadUInt32();
            Mipmap = Reader.ReadUInt32();
            MinLOD = Reader.ReadSingle();
            MaxLOD = Reader.ReadSingle();

            Data = Reader.ReadBuffer(DataOffset, TPL.TextureByteSize((TPL_TextureFormat)Format, Width, Height));
        }

        public override void Save(HSDWriter Writer)
        {
            if (Data != null)
                Writer.WriteTexture(Data);

            Writer.AddObject(this);
            Writer.WritePointer(Data);
            Writer.Write(Width);
            Writer.Write(Height);
            Writer.Write((uint)Format);
            Writer.Write(Mipmap);
            Writer.Write(MinLOD);
            Writer.Write(MaxLOD);
        }
    }

    public class HSD_TOBJ_LOD : IHSDNode
    {
        public GXTexFilter MinFilter { get; set; }
        public float Bias { get; set; }
        public bool BiasClamp { get; set; }
        public bool EnableEdgeLOD { get; set; }
        public GXAnisotropy Anisotropy { get; set; }
    }

    public class HSD_TOBJ_TEV : IHSDNode
    {
        public byte color_op { get; set; }
        public byte alpha_op { get; set; }
        public byte color_bias { get; set; }
        public byte alpha_bias { get; set; }
        
        public byte color_scale { get; set; }
        
        public byte alpha_scale { get; set; }
        
        public byte color_clamp { get; set; }
        
        public byte color_a { get; set; }
        
        public byte color_b { get; set; }
        
        public byte color_c { get; set; }
        
        public byte color_d { get; set; }
        
        public byte alpha_a { get; set; }
        
        public byte alpha_b { get; set; }
        
        public byte alpha_c { get; set; }
        
        public byte alpha_d { get; set; }
        
        public uint konst { get; set; }
        
        public uint tev0 { get; set; }
        
        public uint tev1 { get; set; }
        
        public uint active { get; set; }
    }
}
