namespace HSDRaw.Common
{
    public class HSD_Matrix4x3 : HSDAccessor
    {
        public override int TrimmedSize => 0x30;

        public float M11 { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }
        public float M12 { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }
        public float M13 { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }
        public float M14 { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }
        public float M21 { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }
        public float M22 { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }
        public float M23 { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }
        public float M24 { get => _s.GetFloat(0x1C); set => _s.SetFloat(0x1C, value); }
        public float M31 { get => _s.GetFloat(0x20); set => _s.SetFloat(0x20, value); }
        public float M32 { get => _s.GetFloat(0x24); set => _s.SetFloat(0x24, value); }
        public float M33 { get => _s.GetFloat(0x28); set => _s.SetFloat(0x28, value); }
        public float M34 { get => _s.GetFloat(0x2C); set => _s.SetFloat(0x2C, value); }
    }
}
