namespace HSDRaw.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class HSD_Camera : HSDAccessor
    {
        public override int TrimmedSize => 0x38;

        public int Unknown1 { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }
        public int Flag { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }
        public int Width { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }
        public int Height { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }
        public int ProjWidth { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }
        public int ProjHeight { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }
        public HSD_CameraInfo CamInfo1 { get => _s.GetReference<HSD_CameraInfo>(0x18); set => _s.SetReference(0x18, value); }
        public HSD_CameraInfo CamInfo2 { get => _s.GetReference<HSD_CameraInfo>(0x1C); set => _s.SetReference(0x1C, value); }
        public int Unknown2 { get => _s.GetInt32(0x20); set => _s.SetInt32(0x20, value); }
        public int Unknown3 { get => _s.GetInt32(0x24); set => _s.SetInt32(0x24, value); }
        public float Unknown4 { get => _s.GetFloat(0x28); set => _s.SetFloat(0x28, value); }
        public float FarClip { get => _s.GetFloat(0x2C); set => _s.SetFloat(0x2C, value); }
        public HSD_Camera SelfReference { get => _s.GetReference<HSD_Camera>(0x30); set => _s.SetReference(0x30, value); }
        public float FieldOfView { get => _s.GetFloat(0x34); set => _s.SetFloat(0x34, value); }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HSD_CameraInfo : HSDAccessor
    {
        public override int TrimmedSize => 0x14;

        public float Unknown1 { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }
        public float Unknown2 { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }
        public float Unknown3 { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }
        public float Unknown4 { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }
        public float Unknown5 { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }
    }
}
