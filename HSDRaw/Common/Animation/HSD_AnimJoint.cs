using System;
namespace HSDRaw.Common.Animation
{
    public class HSD_AnimJoint : HSDTreeAccessor<HSD_AnimJoint>
    {
        public override int TrimmedSize => 0x14;

        public override HSD_AnimJoint Child { get => _s.GetReference<HSD_AnimJoint>(0x00); set => _s.SetReference(0x00, value); }

        public override HSD_AnimJoint Next { get => _s.GetReference<HSD_AnimJoint>(0x04); set => _s.SetReference(0x04, value); }
        
        public HSD_AOBJ AOBJ { get => _s.GetReference<HSD_AOBJ>(0x08); set => _s.SetReference(0x08, value); }

        //public uint ROBJPointer { get; set; }

        public uint Flags { get => (uint)_s.GetInt32(0x10); set => _s.SetInt32(0x10, (int)value); }
    }
}
