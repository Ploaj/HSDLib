using HSDRaw.Common;

namespace HSDRaw.Melee.Ef
{
    public class SBM_EffectTable : HSDAccessor
    {
        public HSD_TEXGraphic TextureGraphics { get => _s.GetReference<HSD_TEXGraphic>(0x04); set => _s.SetReference(0x04, value); }
    }
}
