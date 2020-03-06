using HSDRaw.Common;

namespace HSDRaw.Melee.Gr
{
    public class SBM_Quake_Model_Set : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public HSD_JOBJ RootJOBJ { get => _s.GetReference<HSD_JOBJ>(0x00); set => _s.SetReference(0x00, value); }


        public SBM_Quake_Model_Set Self { get => _s.GetReference<SBM_Quake_Model_Set>(0x10); set => _s.SetReference(0x10, value); }
    }
}
