using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDLib.GX
{
    public enum GXAttribName
    {
        GX_VA_PNMTXIDX = 0,    // position/normal matrix index
        GX_VA_TEX0MTXIDX,      // texture 0 matrix index
        GX_VA_TEX1MTXIDX,      // texture 1 matrix index
        GX_VA_TEX2MTXIDX,      // texture 2 matrix index
        GX_VA_TEX3MTXIDX,      // texture 3 matrix index
        GX_VA_TEX4MTXIDX,      // texture 4 matrix index
        GX_VA_TEX5MTXIDX,      // texture 5 matrix index
        GX_VA_TEX6MTXIDX,      // texture 6 matrix index
        GX_VA_TEX7MTXIDX,      // texture 7 matrix index
        GX_VA_POS = 9,    // position
        GX_VA_NRM,             // normal
        GX_VA_CLR0,            // color 0
        GX_VA_CLR1,            // color 1
        GX_VA_TEX0,            // input texture coordinate 0
        GX_VA_TEX1,            // input texture coordinate 1
        GX_VA_TEX2,            // input texture coordinate 2
        GX_VA_TEX3,            // input texture coordinate 3
        GX_VA_TEX4,            // input texture coordinate 4
        GX_VA_TEX5,            // input texture coordinate 5
        GX_VA_TEX6,            // input texture coordinate 6
        GX_VA_TEX7,            // input texture coordinate 7

        GX_POS_MTX_ARRAY,      // position matrix array pointer
        GX_NRM_MTX_ARRAY,      // normal matrix array pointer
        GX_TEX_MTX_ARRAY,      // texture matrix array pointer
        GX_LIGHT_ARRAY,        // light parameter array pointer
        GX_VA_NBT,             // normal, bi-normal, tangent 
        GX_VA_MAX_ATTR,        // maximum number of vertex attributes

        GX_VA_NULL = 0xff  // NULL attribute (to mark end of lists)
    };
    
    public enum GXAttribType
    {
        GX_NONE = 0,
        GX_DIRECT,
        GX_INDEX8,
        GX_INDEX16
    };

    public enum GXCompCnt
    {
        PosXY = 0,
        PosXYZ = 1,
        NrmXYZ = 0,
        NrmNBT = 1, // one index per NBT
        NrmNBT3 = 2, // one index per each of N/B/T
        ClrRGB = 0,
        ClrRGBA = 1,
        TexS = 0,
        TexST = 1
    }

    public enum GXCompType
    {
        UInt8 = 0,
        Int8 = 1,
        UInt16 = 2,
        Int16 = 3,
        Float = 4,

        RGB565 = 0,
        RGB8 = 1,
        RGBX8 = 2,
        RGBA4 = 3,
        RGBA6 = 4,
        RGBA8 = 5
    }

    public class GXVertexBuffer : IHSDNode
    {
        public GXAttribName Name = GXAttribName.GX_VA_NULL;
        public GXAttribType AttributeType;
        public GXCompCnt CompCount;
        public GXCompType CompType;

        public byte Scale;
        public ushort Stride;
        public uint Offset;
        
        public byte[] DataBuffer;

        public override void Open(HSDReader Reader)
        {
            Name = (GXAttribName)Reader.ReadUInt32();
            AttributeType = (GXAttribType)Reader.ReadUInt32();
            CompCount = (GXCompCnt)Reader.ReadInt32();
            CompType = (GXCompType)Reader.ReadUInt32();

            Scale = Reader.ReadByte();
            Reader.ReadByte();//Padding
            Stride = Reader.ReadUInt16();
            Offset = Reader.ReadUInt32();
        }

        public override void Save(HSDWriter Writer)
        {
            if (DataBuffer != null)
                Writer.WriteBuffer(DataBuffer);
            
            Writer.AddObject(this);
            Writer.Write((uint)Name);
            Writer.Write((uint)AttributeType);
            Writer.Write((uint)CompCount);
            Writer.Write((uint)CompType);
            Writer.Write(Scale);
            Writer.Write((byte)0);
            Writer.Write(Stride);
            Writer.WritePointer(DataBuffer);
        }
    }
}
