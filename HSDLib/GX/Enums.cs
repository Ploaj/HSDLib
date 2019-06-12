namespace HSDLib.GX
{
    public enum GXAnisotropy
    {
        GX_ANISO_1, GX_ANISO_2, GX_ANISO_4, GX_MAX_ANISOTROPY
    };

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

        GX_BL_DSTCLR = GX_BL_SRCCLR,
        GX_BL_INVDSTCLR = GX_BL_INVSRCCLR
    }
}
