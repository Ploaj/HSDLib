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
    /// Animation Object
    /// </summary>
    public class HSD_AOBJ : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public AOBJ_Flags Flags { get => (AOBJ_Flags)_s.GetInt32(0); set => _s.SetInt32(0, (int)value); }

        public float EndFrame { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public HSD_FOBJDesc FObjDesc { get => _s.GetReference<HSD_FOBJDesc>(0x08); set => _s.SetReference(0x08, value); }

        //TODO:
        //public uint PathAnimJoint { get; set; }
    }
}
