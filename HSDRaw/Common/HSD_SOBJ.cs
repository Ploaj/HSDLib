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

        public int Fog { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HSD_JOBJDesc : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public HSD_JOBJ RootJoint { get => _s.GetReference<HSD_JOBJ>(0x00); set => _s.SetReference(0x00, value); }

        public HSDNullPointerArrayAccessor<HSD_AnimJoint> SkelAnimations { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_AnimJoint>>(0x04); set => _s.SetReference(0x04, value); }

        public HSDNullPointerArrayAccessor<HSD_MatAnimJoint> MatAnimations { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_MatAnimJoint>>(0x08); set => _s.SetReference(0x08, value); }

        public int Unknown { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }
    }
}
