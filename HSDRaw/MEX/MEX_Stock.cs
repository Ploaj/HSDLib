using HSDRaw.Common.Animation;

namespace HSDRaw.MEX
{
    public class MEX_Stock : HSDAccessor
    {
        public override int TrimmedSize => 8;

        public short Reserved { get => _s.GetInt16(0x00); set => _s.SetInt16(0x00, value); }

        public short Stride { get => _s.GetInt16(0x02); set => _s.SetInt16(0x02, value); }

        public HSD_MatAnimJoint MatAnimJoint { get => _s.GetReference<HSD_MatAnimJoint>(0x04); set => _s.SetReference(0x04, value); }
    }
}
