using HSDRaw.Common;
using HSDRaw.Common.Animation;

namespace HSDRaw.Melee.Ef
{
    public class SBM_EffectTable : HSDAccessor
    {
        // Particle at 0x00

        public HSD_TEXGraphic TextureGraphics { get => _s.GetReference<HSD_TEXGraphic>(0x04); set => _s.SetReference(0x04, value); }

        // Unknown at 0x08

        public SBM_EffectModel[] Models { get => _s.GetEmbeddedAccessorArray<SBM_EffectModel>(0x0C, (_s.Length - 0x0C) / 0x14); set => _s.SetEmbeddedAccessorArray<SBM_EffectModel>(0x0C, value); }
    }

    public class SBM_EffectModel : HSDAccessor
    {
        public override int TrimmedSize => 0x14;

        public HSD_JOBJ RootJoint { get => _s.GetReference<HSD_JOBJ>(0x00); set => _s.SetReference(0x00, value); }
        public HSD_AnimJoint JointAnim { get => _s.GetReference<HSD_AnimJoint>(0x04); set => _s.SetReference(0x04, value); }
        public HSD_MatAnimJoint MaterialAnim { get => _s.GetReference<HSD_MatAnimJoint>(0x08); set => _s.SetReference(0x08, value); }
        public HSD_MatAnimJoint MaterialAnim2 { get => _s.GetReference<HSD_MatAnimJoint>(0x0C); set => _s.SetReference(0x0C, value); }
        public HSDAccessor Unknown { get => _s.GetReference<HSDAccessor>(0x10); set => _s.SetReference(0x10, value); }
    }
}
