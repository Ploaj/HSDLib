using HSDRaw.Common;
using HSDRaw.Common.Animation;

namespace HSDRaw.Melee.Gr
{
    public class SBM_Quake_Model_Set : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public HSD_JOBJ RootJOBJ { get => _s.GetReference<HSD_JOBJ>(0x00); set => _s.SetReference(0x00, value); }

        public HSDNullPointerArrayAccessor<SBM_Quake_Model_Entry> Entries { get => _s.GetReference<HSDNullPointerArrayAccessor<SBM_Quake_Model_Entry>>(0x04); set => _s.SetReference(0x04, value); }
        
        public SBM_Quake_Model_Set Self { get => _s.GetReference<SBM_Quake_Model_Set>(0x10); set => _s.SetReference(0x10, value); }
    }

    public class SBM_Quake_Model_Entry : HSDAccessor
    {
        public override int TrimmedSize => 0x14;

        public HSD_AOBJ Animation { get => _s.GetReference<HSD_AOBJ>(0x08); set => _s.SetReference(0x08, value); }

    }
}
