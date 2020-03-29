using HSDRaw.Common.Animation;
using System;
using System.Drawing;

namespace HSDRaw.Common
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

    public class HSD_Light : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public HSD_LOBJ LightObject { get => _s.GetReference<HSD_LOBJ>(0x00); set => _s.SetReference(0, value); }

        public HSD_LightAnimPointer AnimPointer { get => _s.GetReference<HSD_LightAnimPointer>(0x04); set => _s.SetReference(0x04, value); }
    }

    public class HSD_LightAnimPointer : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public HSD_AOBJ LightAnim { get => _s.GetReference<HSD_AOBJ>(0x00); set => _s.SetReference(0, value); }

        public int Unknown { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }
    }

    public class HSD_LOBJ : HSDListAccessor<HSD_LOBJ>
    {
        public override int TrimmedSize => 0x1C;

        public int ClassName { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public override HSD_LOBJ Next { get => _s.GetReference<HSD_LOBJ>(0x04); set => _s.SetReference(0x04, value); }

        public LOBJ_Flags Flags { get => (LOBJ_Flags)_s.GetInt16(0x08); set => _s.SetInt16(0x08, (short)value); }

        public LOBJ_AttenuationFlags AttenuationFlags { get => (LOBJ_AttenuationFlags)_s.GetInt16(0x0A); set => _s.SetInt16(0x0A, (short)value); }

        public byte ColorR { get => _s.GetByte(0x0C); set => _s.SetByte(0x0C, value); }
        public byte ColorG { get => _s.GetByte(0x0D); set => _s.SetByte(0x0D, value); }
        public byte ColorB { get => _s.GetByte(0x0E); set => _s.SetByte(0x0E, value); }
        public byte ColorAlpha { get => _s.GetByte(0x0F); set => _s.SetByte(0x0F, value); }

        public Color LightColor {
            get => _s.GetColorRGBA(0x0C);
            set => _s.SetColorRGBA(0x0C, value);
        }

        public HSD_LOBJPoint Position { get => _s.GetReference<HSD_LOBJPoint>(0x10); set => _s.SetReference(0x10, value); }

        //public int Unknown { get; set; } // TODO: this is pointer

        public HSD_Float InfiniteData
        {
            get => Flags.HasFlag(LOBJ_Flags.LOBJ_INFINITE) ? _s.GetReference<HSD_Float>(0x18) : null;
            set
            {
                _s.SetReference(0x18, value);
                Flags &= ~LOBJ_Flags.LOBJ_POINT;
                Flags |= LOBJ_Flags.LOBJ_INFINITE;
            }
        }
        public HSD_PointSpotData PointSpotData
        {
            get => Flags.HasFlag(LOBJ_Flags.LOBJ_POINT) ? _s.GetReference<HSD_PointSpotData>(0x18) : null;
            set
            {
                _s.SetReference(0x18, value);
                Flags &= ~LOBJ_Flags.LOBJ_INFINITE;
                Flags |= LOBJ_Flags.LOBJ_POINT;
            }
        }
    }

    public class HSD_PointSpotData : HSDAccessor
    {
        public override int TrimmedSize => 0x0C;
        
        public float RangeMin { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }
        public float RangeMax { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }
        public int Flag { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }
    }

    public class HSD_LOBJPoint : HSDAccessor
    {
        public override int TrimmedSize => 0x14;

        public int ClassName { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }
        public float X { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }
        public float Y { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }
        public float Z { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }
        public int Unknown { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }
    }
}
