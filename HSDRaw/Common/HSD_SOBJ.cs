using HSDRaw.Common.Animation;
namespace HSDRaw.Common
{
    /// <summary>
    /// Scene Object
    /// </summary>
    public class HSD_SOBJ : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public HSDNullPointerArrayAccessor<HSD_JOBJDesc> JOBJDescs { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_JOBJDesc>>(0x00); set => _s.SetReference(0x00, value); }

        public HSDNullPointerArrayAccessor<HSD_Camera> Camera { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_Camera>>(0x04); set => _s.SetReference(0x04, value); }

        public HSDNullPointerArrayAccessor<HSD_Light> Lights { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_Light>>(0x08); set => _s.SetReference(0x08, value); }

        public HSD_FogAdjDesc Fog { get => _s.GetReference<HSD_FogAdjDesc>(0x0C); set => _s.SetReference(0x0C, value); }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HSD_JOBJDesc : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public HSD_JOBJ RootJoint { get => _s.GetReference<HSD_JOBJ>(0x00); set => _s.SetReference(0x00, value); }

        public HSDNullPointerArrayAccessor<HSD_AnimJoint> JointAnimations { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_AnimJoint>>(0x04); set => _s.SetReference(0x04, value); }

        public HSDNullPointerArrayAccessor<HSD_MatAnimJoint> MaterialAnimations { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_MatAnimJoint>>(0x08); set => _s.SetReference(0x08, value); }

        public HSDNullPointerArrayAccessor<HSD_ShapeAnimJoint> ShapeAnimations { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_ShapeAnimJoint>>(0x0C); set => _s.SetReference(0x0C, value); }

    }

    /// <summary>
    /// 
    /// </summary>
    public class HSD_ModelGroup : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public HSD_JOBJ RootJoint { get => _s.GetReference<HSD_JOBJ>(0x00); set => _s.SetReference(0x00, value); }

        public HSD_AnimJoint AnimJoint { get => _s.GetReference<HSD_AnimJoint>(0x04); set => _s.SetReference(0x04, value); }

        public HSD_MatAnimJoint MatAnimJoint { get => _s.GetReference<HSD_MatAnimJoint>(0x08); set => _s.SetReference(0x08, value); }

        public HSD_ShapeAnimJoint ShapeAnimJoint { get => _s.GetReference<HSD_ShapeAnimJoint>(0x0C); set => _s.SetReference(0x0C, value); }

    }
}
