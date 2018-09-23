using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSDLib.GX;
using HSDLib.Helpers;

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
        [FieldData(typeof(uint))]
        public uint NameOffset { get; set; }

        [FieldData(typeof(HSD_TOBJ))]
        public override HSD_TOBJ Next { get; set; }

        [FieldData(typeof(GXTexMapID))]
        public GXTexMapID TexMapID { get; set; }//= GXTexMapID.GX_TEXMAP0;
        
        [FieldData(typeof(uint))]
        public uint GXTexGenSrc { get; set; }//= 4;

        [FieldData(typeof(HSD_Transforms), true, 4 * 3 * 4)]
        public HSD_Transforms Transform { get; set; }//= new HSD_Transforms();

        [FieldData(typeof(GXWrapMode))]
        public GXWrapMode WrapS { get; set; }//= GXWrapMode.REPEAT;

        [FieldData(typeof(GXWrapMode))]
        public GXWrapMode WrapT { get; set; }//= GXWrapMode.REPEAT;

        [FieldData(typeof(byte))]
        public byte WScale { get; set; }//= 1;

        [FieldData(typeof(byte))]
        public byte HScale { get; set; }//= 1;

        [FieldData(typeof(ushort))]
        public ushort Padding { get; set; }//= 1;

        [FieldData(typeof(TOBJ_FLAGS))]
        public TOBJ_FLAGS Flags { get; set; }

        [FieldData(typeof(float))]
        public float Blending { get; set; }//= 1;

        [FieldData(typeof(GXTexFilter))]
        public GXTexFilter MagFilter { get; set; }//= GXTexFilter.GX_LINEAR;

        [FieldData(typeof(HSD_Image))]
        public HSD_Image ImageData { get; set; }//

        [FieldData(typeof(HSD_Tlut))]
        public HSD_Tlut Tlut { get; set; }//

        [FieldData(typeof(HSD_TOBJ_LOD))]
        public HSD_TOBJ_LOD LOD { get; set; }//

        [FieldData(typeof(HSD_TOBJ_TEV))]
        public HSD_TOBJ_TEV TEV { get; set; }
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
        public ushort Width, Height;
        public GXTexFmt Format;
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
        [FieldData(typeof(GXTexFilter))]
        public GXTexFilter MinFilter;
        [FieldData(typeof(float))]
        public float Bias;
        [FieldData(typeof(bool))]
        public bool BiasClamp;
        [FieldData(typeof(bool))]
        public bool EnableEdgeLOD;
        [FieldData(typeof(GXAnisotropy))]
        public GXAnisotropy Anisotropy;
    }

    public class HSD_TOBJ_TEV : IHSDNode
    {
        [FieldData(typeof(byte))]
        public byte color_op { get; set; }
        [FieldData(typeof(byte))]
        public byte alpha_op { get; set; }
        [FieldData(typeof(byte))]
        public byte color_bias { get; set; }
        [FieldData(typeof(byte))]
        public byte alpha_bias { get; set; }
        [FieldData(typeof(byte))]
        public byte color_scale { get; set; }
        [FieldData(typeof(byte))]
        public byte alpha_scale { get; set; }
        [FieldData(typeof(byte))]
        public byte color_clamp { get; set; }
        [FieldData(typeof(byte))]
        public byte color_a { get; set; }
        [FieldData(typeof(byte))]
        public byte color_b { get; set; }
        [FieldData(typeof(byte))]
        public byte color_c { get; set; }
        [FieldData(typeof(byte))]
        public byte color_d { get; set; }
        [FieldData(typeof(byte))]
        public byte alpha_a { get; set; }
        [FieldData(typeof(byte))]
        public byte alpha_b { get; set; }
        [FieldData(typeof(byte))]
        public byte alpha_c { get; set; }
        [FieldData(typeof(byte))]
        public byte alpha_d { get; set; }
        [FieldData(typeof(uint))]
        public uint konst { get; set; }
        [FieldData(typeof(uint))]
        public uint tev0 { get; set; }
        [FieldData(typeof(uint))]
        public uint tev1 { get; set; }
        [FieldData(typeof(uint))]
        public uint active { get; set; }
    }
}
