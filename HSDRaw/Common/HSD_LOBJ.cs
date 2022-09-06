using HSDRaw.Common.Animation;
using HSDRaw.GX;
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

    public enum LObjType
    {
        AMBIENT,
        INFINITE,
        POINT,
        SPOT,
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

        public HSDNullPointerArrayAccessor<HSD_LightAnimPointer> AnimPointer { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_LightAnimPointer>>(0x04); set => _s.SetReference(0x04, value); }
    }

    public class HSD_LightAnimPointer : HSDListAccessor<HSD_LightAnimPointer>
    {
        public override int TrimmedSize => 0x10;

        public override HSD_LightAnimPointer Next { get => _s.GetReference<HSD_LightAnimPointer>(0x00); set => _s.SetReference(0x00, value); }

        public HSD_AOBJ LightAnim { get => _s.GetReference<HSD_AOBJ>(0x04); set => _s.SetReference(0x04, value); }

        public HSD_WOBJAnim PositionAnim { get => _s.GetReference<HSD_WOBJAnim>(0x08); set => _s.SetReference(0x08, value); }

        public HSD_WOBJAnim InterestAnim { get => _s.GetReference<HSD_WOBJAnim>(0x0C); set => _s.SetReference(0x0C, value); }
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

        public HSD_WOBJ Position { get => _s.GetReference<HSD_WOBJ>(0x10); set => _s.SetReference(0x10, value); }

        public HSD_WOBJ Interest { get => _s.GetReference<HSD_WOBJ>(0x14); set => _s.SetReference(0x14, value); }

        public float InfiniteData
        {
            get => (LOBJ_Flags)((int)Flags & 0x3) == (LOBJ_Flags.LOBJ_INFINITE) ? _s.GetReference<HSDAccessor>(0x18)._s.GetFloat(0) : 0;
            set
            {
                var str = new HSDAccessor();
                str._s.SetFloat(0, value);
                _s.SetReference(0x18, str);
                Flags &= ~LOBJ_Flags.LOBJ_POINT;
                Flags &= ~LOBJ_Flags.LOBJ_SPOT;
                Flags &= ~LOBJ_Flags.LOBJ_AMBIENT;
                Flags |= LOBJ_Flags.LOBJ_INFINITE;
            }
        }
        public HSD_LightPoint PointData
        {
            get => (LOBJ_Flags)((int)Flags & 0x3) == (LOBJ_Flags.LOBJ_POINT) ? _s.GetReference<HSD_LightPoint>(0x18) : null;
            set
            {
                _s.SetReference(0x18, value);
                Flags &= ~LOBJ_Flags.LOBJ_INFINITE;
                Flags &= ~LOBJ_Flags.LOBJ_SPOT;
                Flags &= ~LOBJ_Flags.LOBJ_AMBIENT;
                Flags |= LOBJ_Flags.LOBJ_POINT;
            }
        }

        public HSD_LightSpot SpotData
        {
            get => (LOBJ_Flags)((int)Flags & 0x3) == (LOBJ_Flags.LOBJ_SPOT) ? _s.GetReference<HSD_LightSpot>(0x18) : null;
            set
            {
                _s.SetReference(0x18, value);
                Flags &= ~(LOBJ_Flags)0x3;
                Flags |= LOBJ_Flags.LOBJ_SPOT;
            }
        }

    }

    public class HSD_LightPoint : HSDAccessor
    {
        public override int TrimmedSize => 0x0C;
        
        public float RefBrightness { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }

        public float RefDistance { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public GXBrightnessDistance Flag { get => (GXBrightnessDistance)_s.GetInt32(0x08); set => _s.SetInt32(0x08, (int)value); }
    }

    public class HSD_LightSpot : HSDAccessor
    {
        public override int TrimmedSize => 0x14;

        public float Cutoff { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }
        
        public GXSpotFunc SpotFunc { get => (GXSpotFunc)_s.GetInt32(0x04); set => _s.SetInt32(0x04, (byte)value); }
        
        public float RefBrightness { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }
        
        public float RefDistance { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        public GXBrightnessDistance DistFunc { get => (GXBrightnessDistance)_s.GetInt32(0x10); set => _s.SetInt32(0x10, (byte)value); }
    }

    public class HSD_LightAttn : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public float A0 { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }

        public float A1 { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public float A2 { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public float K0 { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        public float K1 { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }

        public float K2 { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }
    }

}
