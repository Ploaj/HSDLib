using HSDRaw.Common;
using HSDRaw.Common.Animation;

namespace HSDRaw.MEX.Menus
{
    public class MEX_mexSelectChr : HSDAccessor
    {
        public override int TrimmedSize => 0x10;
        
        public HSD_JOBJ IconModel { get => _s.GetReference<HSD_JOBJ>(0x00); set => _s.SetReference(0x00, value); }

        public HSD_AnimJoint IconAnimJoint { get => _s.GetReference<HSD_AnimJoint>(0x04); set => _s.SetReference(0x04, value); }

        public HSD_MatAnimJoint IconMatAnimJoint { get => _s.GetReference<HSD_MatAnimJoint>(0x08); set => _s.SetReference(0x08, value); }
        
        public HSD_MatAnim CSPMatAnim { get => _s.GetReference<HSD_MatAnim>(0x0C); set => _s.SetReference(0x0C, value); }

        //public HSD_MatAnim StockMatAnim { get => _s.GetReference<HSD_MatAnim>(0x10); set => _s.SetReference(0x10, value); }

        //public HSD_MatAnim EmblemMatAnim { get => _s.GetReference<HSD_MatAnim>(0x14); set => _s.SetReference(0x14, value); }
    }
}
