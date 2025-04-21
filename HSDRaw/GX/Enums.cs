﻿namespace HSDRaw.GX
{
    public enum GXPrimitiveType
    {
        Points = 0xB8,
        Lines = 0xA8,
        LineStrip = 0xB0,
        Triangles = 0x90,
        TriangleStrip = 0x98,
        TriangleFan = 0xA0,
        Quads = 0x80
    }

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

    public enum GXTexGenSrc
    {
        GX_TG_POS,
        GX_TG_NRM,
        GX_TG_BINRM,
        GX_TG_TANGENT,
        GX_TG_TEX0,
        GX_TG_TEX1,
        GX_TG_TEX2,
        GX_TG_TEX3,
        GX_TG_TEX4,
        GX_TG_TEX5,
        GX_TG_TEX6,
        GX_TG_TEX7,
        GX_TG_TEXCOORD0,
        GX_TG_TEXCOORD1,
        GX_TG_TEXCOORD2,
        GX_TG_TEXCOORD3,
        GX_TG_TEXCOORD4,
        GX_TG_TEXCOORD5,
        GX_TG_TEXCOORD6,
        GX_TG_COLOR0,
        GX_TG_COLOR1,
    }

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
        Unused = 5,
    }

    public enum GXCompTypeClr
    {
        RGB565 = 0,
        RGB8 = 1,
        RGBX8 = 2,
        RGBA4 = 3,
        RGBA6 = 4,
        RGBA8 = 5,
    }

    public enum GXAnisotropy
    {
        GX_ANISO_1, GX_ANISO_2, GX_ANISO_4, GX_MAX_ANISOTROPY
    }

    public enum GXTexFmt
    {
        /// <summary>
        /// Intensity, 4bpp
        /// </summary>
        I4 = 0,
        /// <summary>
        /// Intensity, 8bpp
        /// </summary>
        I8 = 1,
        /// <summary>
        /// Intensity, Alpha, 8bpp
        /// </summary>
        IA4 = 2,
        /// <summary>
        /// Intensity, Alpha, 16bpp
        /// </summary>
        IA8 = 3,
        /// <summary>
        /// RGB, 16bpp
        /// </summary>
        RGB565 = 4,
        /// <summary>
        /// RGB, Alpha, 16bpp
        /// </summary>
        RGB5A3 = 5,
        /// <summary>
        /// RGB, Alpha, 32bpp
        /// </summary>
        RGBA8 = 6,
        /// <summary>
        /// Indexed, 4bpp
        /// </summary>
        CI4 = 8,
        /// <summary>
        /// Indexed, 8bpp
        /// </summary>
        CI8 = 9,
        /// <summary>
        /// Indexed, 14bpp
        /// </summary>
        CI14X2 = 10,
        /// <summary>
        /// S3TC Compressed
        /// </summary>
        CMP = 14,
    }

    public enum GXTexMapID
    {
        GX_TEXMAP0,
        GX_TEXMAP1,
        GX_TEXMAP2,
        GX_TEXMAP3,
        GX_TEXMAP4,
        GX_TEXMAP5,
        GX_TEXMAP6,
        GX_TEXMAP7,
        GX_MAX_TEXMAP,
        GX_TEXMAP_NULL,
        GX_TEXMAP_DISABLE
    };

    public enum GXTlutFmt
    {
        IA8,
        RGB565,
        RGB5A3
    }

    public enum GXWrapMode
    {
        CLAMP,
        REPEAT,
        MIRROR
    }

    public enum GXTexFilter
    {
        GX_NEAR,
        GX_LINEAR,
        GX_NEAR_MIP_NEAR,
        GX_LIN_MIP_NEAR,
        GX_NEAR_MIP_LIN,
        GX_LIN_MIP_LIN
    };

    public enum GXBlendMode
    {
        GX_NONE = 0,
        GX_BLEND = 1,
        GX_LOGIC = 2,
        GX_SUBTRACT = 3
    }

    public enum GXLogicOp
    {
        GX_LO_CLEAR,
        GX_LO_AND,
        GX_LO_REVAND,
        GX_LO_COPY,
        GX_LO_INVAND,
        GX_LO_NOOP,
        GX_LO_XOR,
        GX_LO_OR,
        GX_LO_NOR,
        GX_LO_EQUIV,
        GX_LO_INV,
        GX_LO_REVOR,
        GX_LO_INVCOPY,
        GX_LO_INVOR,
        GX_LO_NAND,
        GX_LO_SET
    }

    public enum GXCompareType
    {
        Never = 0,
        Less = 1,
        Equal = 2,
        LEqual = 3,
        Greater = 4,
        NEqual = 5,
        GEqual = 6,
        Always = 7
    }

    public enum GXAlphaOp
    {
        And = 0,
        Or = 1,
        XOR = 2,
        XNOR = 3
    }

    public enum GXBlendFactor
    {
        GX_BL_ZERO,
        GX_BL_ONE,
        GX_BL_SRCCLR,
        GX_BL_INVSRCCLR,
        GX_BL_SRCALPHA,
        GX_BL_INVSRCALPHA,
        GX_BL_DSTALPHA,
        GX_BL_INVDSTALPHA,

        //GX_BL_DSTCLR = GX_BL_SRCCLR,
        //GX_BL_INVDSTCLR = GX_BL_INVSRCCLR
    }

    public enum TevColorOp
    {
        GX_TEV_ADD = 0,
        GX_TEV_SUB = 1,

        GX_TEV_COMP_R8_GT = 8,
        GX_TEV_COMP_R8_EQ = 9,
        GX_TEV_COMP_GR16_GT = 10,
        GX_TEV_COMP_GR16_EQ = 11,
        GX_TEV_COMP_BGR24_GT = 12,
        GX_TEV_COMP_BGR24_EQ = 13,
        GX_TEV_COMP_RGB8_GT = 14,
        GX_TEV_COMP_RGB8_EQ = 15,
    }

    public enum TevAlphaOp
    {
        GX_TEV_ADD = 0,
        GX_TEV_SUB = 1,

        GX_TEV_COMP_R8_GT = 8,
        GX_TEV_COMP_R8_EQ = 9,
        GX_TEV_COMP_GR16_GT = 10,
        GX_TEV_COMP_GR16_EQ = 11,
        GX_TEV_COMP_BGR24_GT = 12,
        GX_TEV_COMP_BGR24_EQ = 13,
        GX_TEV_COMP_A8_GT = 14,
        GX_TEV_COMP_A8_EQ = 15,
    }

    public enum TevScale
    {
        GX_CS_SCALE_1,
        GX_CS_SCALE_2,
        GX_CS_SCALE_4,
        GX_CS_DIVIDE_2
    }

    public enum TevBias
    {
        GX_TB_ZERO,
        GX_TB_ADDHALF,
        GX_TB_SUBHALF
    }

    public enum TevColorIn
    {
        GX_CC_CPREV,
        GX_CC_APREV,
        GX_CC_C0,
        GC_CC_A0,
        GX_CC_C1,
        GC_CC_A1,
        GX_CC_C2,
        GC_CC_A2,
        GX_CC_TEXC,
        GX_CC_TEXA,
        GX_CC_RASC,
        GX_CC_RASA,
        GX_CC_ONE,
        GX_CC_HALF,
        GX_CC_KONST,
        GX_CC_ZERO
    }

    public enum GXBrightnessDistance
    {
        GX_DA_OFF = 0,
        GX_DA_GENTLE = 1,
        GX_DA_MEDIUM = 2,
        GX_DA_STEEP = 3,
    }

    public enum GXSpotFunc
    {
        GX_SP_OFF = 0,
        GX_SP_COS = 2,
        GX_SP_FLAT = 1,
        GX_SP_COS2 = 3,
        GX_SP_SHARP = 4,
        GX_SP_RING1 = 5,
        GX_SP_RING2 = 6,
    }
}
