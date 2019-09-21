namespace HSDRaw.AirRide.Vc
{
    public class KAR_vcCollisionAttributes : HSDAccessor
    {
        public override int TrimmedSize => 0x38;

        public float StarGroundDistance { get => _s.GetFloat(0x0); set => _s.SetFloat(0x0, value); }
        public float WEIRDUnknown { get => _s.GetFloat(0x4); set => _s.SetFloat(0x4, value); }
        public float AffectsSpeedBoostPerfectLanding { get => _s.GetFloat(0x8); set => _s.SetFloat(0x8, value); }
        public float Unknown1 { get => _s.GetFloat(0xc); set => _s.SetFloat(0xc, value); }
        public float Unknown2 { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }
        public float Unknown3 { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }
        public float Unknown4 { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }
        public float Unknown5 { get => _s.GetFloat(0x1c); set => _s.SetFloat(0x1c, value); }
        public float Unknown6 { get => _s.GetFloat(0x20); set => _s.SetFloat(0x20, value); }
        public float Unknown7 { get => _s.GetFloat(0x24); set => _s.SetFloat(0x24, value); }
        public float Unknown8 { get => _s.GetFloat(0x28); set => _s.SetFloat(0x28, value); }
        public float Unknown9 { get => _s.GetFloat(0x2c); set => _s.SetFloat(0x2c, value); }
        public float Unknown10 { get => _s.GetFloat(0x30); set => _s.SetFloat(0x30, value); }
        public int Unknown11 { get => _s.GetInt32(0x34); set => _s.SetInt32(0x34, value); }
    }
}
