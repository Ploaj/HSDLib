namespace HSDRaw.AirRide.Vc
{
    public class KAR_vcHandlingAttributes : HSDAccessor
    {
        public override int TrimmedSize => 0xF8;

        public float AffectsBounciness { get => _s.GetFloat(0x0); set => _s.SetFloat(0x0, value); }
        public float GeneralSpeeding { get => _s.GetFloat(0x4); set => _s.SetFloat(0x4, value); }
        public float Unknown1 { get => _s.GetFloat(0x8); set => _s.SetFloat(0x8, value); }
        public float BaseHandlingLowSpeed { get => _s.GetFloat(0xc); set => _s.SetFloat(0xc, value); }
        public float BaseHandlingHighSpeed { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }
        public float Unknown2 { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }
        public float Unknown3 { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }
        public float Unknown4 { get => _s.GetFloat(0x1c); set => _s.SetFloat(0x1c, value); }
        public float Unknown5 { get => _s.GetFloat(0x20); set => _s.SetFloat(0x20, value); }
        public float Unknown6 { get => _s.GetFloat(0x24); set => _s.SetFloat(0x24, value); }
        public float Unknown7 { get => _s.GetFloat(0x28); set => _s.SetFloat(0x28, value); }
        public float Unknown8 { get => _s.GetFloat(0x2c); set => _s.SetFloat(0x2c, value); }
        public float Unknown9 { get => _s.GetFloat(0x30); set => _s.SetFloat(0x30, value); }
        public float Unknown10 { get => _s.GetFloat(0x34); set => _s.SetFloat(0x34, value); }
        public float Unknown11 { get => _s.GetFloat(0x38); set => _s.SetFloat(0x38, value); }
        public float Unknown12 { get => _s.GetFloat(0x3c); set => _s.SetFloat(0x3c, value); }
        public float Unknown13 { get => _s.GetFloat(0x40); set => _s.SetFloat(0x40, value); }
        public float Unknown14 { get => _s.GetFloat(0x44); set => _s.SetFloat(0x44, value); }
        public float StoppingSpeed { get => _s.GetFloat(0x48); set => _s.SetFloat(0x48, value); }
        public float Unknown15 { get => _s.GetFloat(0x4c); set => _s.SetFloat(0x4c, value); }
        public float Unknown16 { get => _s.GetFloat(0x50); set => _s.SetFloat(0x50, value); }
        public float Unknown17 { get => _s.GetFloat(0x54); set => _s.SetFloat(0x54, value); }
        public float Unknown18 { get => _s.GetFloat(0x58); set => _s.SetFloat(0x58, value); }
        public float Unknown19 { get => _s.GetFloat(0x5c); set => _s.SetFloat(0x5c, value); }
        public float Unknown20 { get => _s.GetFloat(0x60); set => _s.SetFloat(0x60, value); }
        public float Unknown21 { get => _s.GetFloat(0x64); set => _s.SetFloat(0x64, value); }
        public float Unknown22 { get => _s.GetFloat(0x68); set => _s.SetFloat(0x68, value); }
        public float Unknown23 { get => _s.GetFloat(0x6c); set => _s.SetFloat(0x6c, value); }
        public float Unknown24 { get => _s.GetFloat(0x70); set => _s.SetFloat(0x70, value); }
        public float Unknown25 { get => _s.GetFloat(0x74); set => _s.SetFloat(0x74, value); }
        public float Unknown26 { get => _s.GetFloat(0x78); set => _s.SetFloat(0x78, value); }
        public float Unknown27 { get => _s.GetFloat(0x7c); set => _s.SetFloat(0x7c, value); }
        public float Unknown28 { get => _s.GetFloat(0x80); set => _s.SetFloat(0x80, value); }
        public float Unknown29 { get => _s.GetFloat(0x84); set => _s.SetFloat(0x84, value); }
        public float UnknownAngleCharge { get => _s.GetFloat(0x88); set => _s.SetFloat(0x88, value); }
        public float GravitationalPull { get => _s.GetFloat(0x8c); set => _s.SetFloat(0x8c, value); }
        public float Unknown30 { get => _s.GetFloat(0x90); set => _s.SetFloat(0x90, value); }
        public float Unknown31 { get => _s.GetFloat(0x94); set => _s.SetFloat(0x94, value); }
        public float Unknown32 { get => _s.GetFloat(0x98); set => _s.SetFloat(0x98, value); }
        public float Unknown33 { get => _s.GetFloat(0x9c); set => _s.SetFloat(0x9c, value); }
        public float Unknown34 { get => _s.GetFloat(0xa0); set => _s.SetFloat(0xa0, value); }
        public float Unknown35 { get => _s.GetFloat(0xa4); set => _s.SetFloat(0xa4, value); }
        public float Unknown36 { get => _s.GetFloat(0xa8); set => _s.SetFloat(0xa8, value); }
        public float Unknown37 { get => _s.GetFloat(0xac); set => _s.SetFloat(0xac, value); }
        public float AllowStarFlight { get => _s.GetFloat(0xb0); set => _s.SetFloat(0xb0, value); }
        public int Unknown38 { get => _s.GetInt32(0xb4); set => _s.SetInt32(0xb4, value); }
        public float Gravity { get => _s.GetFloat(0xb8); set => _s.SetFloat(0xb8, value); }
        public float Unknown39 { get => _s.GetFloat(0xbc); set => _s.SetFloat(0xbc, value); }
        public float Unknown40 { get => _s.GetFloat(0xc0); set => _s.SetFloat(0xc0, value); }
        public float VerticalSpeedUpOffPlatform { get => _s.GetFloat(0xc4); set => _s.SetFloat(0xc4, value); }
        public float BaseGlide { get => _s.GetFloat(0xc8); set => _s.SetFloat(0xc8, value); }
        public float FallSpeedWhenHoldingA { get => _s.GetFloat(0xcc); set => _s.SetFloat(0xcc, value); }
        public float Unknown41 { get => _s.GetFloat(0xd0); set => _s.SetFloat(0xd0, value); }
        public float Unknown42 { get => _s.GetFloat(0xd4); set => _s.SetFloat(0xd4, value); }
        public float Unknown43 { get => _s.GetFloat(0xd8); set => _s.SetFloat(0xd8, value); }
        public float Unknown44 { get => _s.GetFloat(0xdc); set => _s.SetFloat(0xdc, value); }
        public float Unknown45 { get => _s.GetFloat(0xe0); set => _s.SetFloat(0xe0, value); }
        public float Unknown46 { get => _s.GetFloat(0xe4); set => _s.SetFloat(0xe4, value); }
        public float Unknown47 { get => _s.GetFloat(0xe8); set => _s.SetFloat(0xe8, value); }
        public float Unknown48 { get => _s.GetFloat(0xec); set => _s.SetFloat(0xec, value); }
        public float Unknown49 { get => _s.GetFloat(0xf0); set => _s.SetFloat(0xf0, value); }
        public float Unknown50 { get => _s.GetFloat(0xf4); set => _s.SetFloat(0xf4, value); }
    }
}
