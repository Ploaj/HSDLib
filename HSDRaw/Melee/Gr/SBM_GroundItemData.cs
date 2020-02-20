using HSDRaw.Melee.Pl;

namespace HSDRaw.Melee.Gr
{
    public class SBM_MapItem : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public int Index { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public SBM_Article article { get => _s.GetReference<SBM_Article>(0x04); set => _s.SetReference(0x04, value); }
    }
}
