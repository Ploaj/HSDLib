﻿using HSDRaw.Common.Animation;

namespace HSDRaw.Common
{
    public class HSD_WOBJ : HSDAccessor
    {
        public override int TrimmedSize => 0x14;

        public string ClassName { get => _s.GetString(0x00); set => _s.SetString(0x00, value); }

        public float V1 { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public float V2 { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public float V3 { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        public int RObj { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

    }

    public class HSD_WOBJAnim : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public HSD_AOBJ Animation { get => _s.GetReference<HSD_AOBJ>(0x00); set => _s.SetReference(0x00, value); }

        public HSD_ROBJAnimJoint AnimJoint { get => _s.GetReference<HSD_ROBJAnimJoint>(0x04); set => _s.SetReference(0x04, value); }

    }
}
