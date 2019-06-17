using System;

namespace HSDLib.Common
{
    [Flags]
    public enum LOBJ_AttenuationFlags
    {
        LOBJ_LIGHT_ATTN_NONE = 0,
        LOBJ_LIGHT_ATTN = 1
    }

    [Flags]
    public enum LOBJ_Flags
    {
        LOBJ_AMBIENT = (0 << 0),
        LOBJ_INFINITE = (1 << 0),
        LOBJ_POINT = (2 << 0),
        LOBJ_SPOT = (3 << 0),
        LOBJ_DIFFUSE = (1 << 2),
        LOBJ_SPECULAR = (1 << 3),
        LOBJ_ALPHA = (1 << 4),
        LOBJ_HIDDEN = (1 << 5),
        LOBJ_RAW_PARAM = (1 << 6),
        LOBJ_DIFF_DIRTY = (1 << 7),
        LOBJ_SPEC_DIRTY = (1 << 8)
    }
    public class HSD_LOBJ : IHSDList<HSD_LOBJ>
    {
        public int ClassName { get; set; }

        public override HSD_LOBJ Next { get; set; }

        public ushort Flags { get; set; }

        public ushort AttenuationFlags { get; set; }

        public byte ColorR { get; set; }
        public byte ColorG { get; set; }
        public byte ColorB { get; set; }
        public byte ColorAlpha { get; set; }

        public HSD_LOBJPoint Position { get; set; }

        public int Unknown { get; set; } // TODO: this is pointer

        public HSD_Float PointSpotData { get; set; }
    }

    public class HSD_LOBJPoint : IHSDNode
    {
        public int ClassName { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public int Unknown { get; set; }
    }

    public class HSD_Float : IHSDNode
    {
        public float Value { get; set; }
    }
}
