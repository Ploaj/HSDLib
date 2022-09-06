using System;

namespace HSDRaw.Common.Animation
{
    [Flags]
    public enum AOBJ_Flags
    {
        ANIM_REWINDED = (1 << 26),
        FIRST_PLAY = (1 << 27),
        NO_UPDATE = (1 << 28),
        ANIM_LOOP = (1 << 29),
        NO_ANIM = (1 << 30)
    }

    /// <summary>
    /// 
    /// </summary>
    public class HSD_AOBJDesc : HSDListAccessor<HSD_AOBJDesc>
    {
        public override HSD_AOBJDesc Next { get => _s.GetReference<HSD_AOBJDesc>(0x00); set => _s.SetReference(0x00, value); }

        public HSD_AOBJ AnimationObject { get => _s.GetReference<HSD_AOBJ>(0x04); set => _s.SetReference(0x04, value); }
    }

    /// <summary>
    /// Animation Object
    /// </summary>
    public class HSD_AOBJ : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public AOBJ_Flags Flags { get => (AOBJ_Flags)_s.GetInt32(0); set => _s.SetInt32(0, (int)value); }

        public float EndFrame { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public HSD_FOBJDesc FObjDesc { get => _s.GetReference<HSD_FOBJDesc>(0x08); set => _s.SetReference(0x08, value); }

        public HSD_JOBJ ObjectReference { get => _s.GetReference<HSD_JOBJ>(0x0C); set => _s.SetReference(0x0C, value); }
    }
}
