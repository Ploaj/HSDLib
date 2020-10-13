namespace HSDRaw.Common.Animation
{
    public class HSD_ROBJAnimJoint : HSDListAccessor<HSD_ROBJAnimJoint>
    {
        public override int TrimmedSize => 0x8;
        
        public override HSD_ROBJAnimJoint Next { get => _s.GetReference<HSD_ROBJAnimJoint>(0x00); set => _s.SetReference(0x00, value); }
        
        public HSD_AOBJ AOBJ { get => _s.GetReference<HSD_AOBJ>(0x04); set => _s.SetReference(0x04, value); }
    }
}
