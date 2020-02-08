using HSDRaw.Common;
using HSDRaw.Common.Animation;

namespace HSDRaw.Melee.Ef
{
    public class SBM_EffectTable : HSDAccessor
    {
        public HSD_ParticleGroup Particles { get => _s.GetReference<HSD_ParticleGroup>(0x00); set => _s.SetReference(0x00, value); }
        
        public HSD_TEXGraphicBank TextureGraphics { get => _s.GetReference<HSD_TEXGraphicBank>(0x04); set => _s.SetReference(0x04, value); }
        
        public SBM_EffectModel[] Models { get => _s.GetEmbeddedAccessorArray<SBM_EffectModel>(0x08, (_s.Length - 0x08) / 0x14); set => _s.SetEmbeddedAccessorArray(0x08, value); }
    }

    public class SBM_EffectModel : HSDAccessor
    {
        public override int TrimmedSize => 0x14;

        public float FrameCount { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }

        public HSD_JOBJ RootJoint { get => _s.GetReference<HSD_JOBJ>(0x04); set => _s.SetReference(0x04, value); }

        public HSD_AnimJoint JointAnim { get => _s.GetReference<HSD_AnimJoint>(0x08); set => _s.SetReference(0x08, value); }

        public HSD_MatAnimJoint MaterialAnim { get => _s.GetReference<HSD_MatAnimJoint>(0x0C); set => _s.SetReference(0x0C, value); }
        
        public HSD_ShapeAnimJoint ShapeAnim { get => _s.GetReference<HSD_ShapeAnimJoint>(0x10); set => _s.SetReference(0x10, value); }
        
    }
}
