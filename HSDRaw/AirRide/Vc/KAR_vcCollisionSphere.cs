namespace HSDRaw.AirRide.Vc
{
    public class KAR_vcCollisionSphere : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public int Unk1 { get => _s.GetInt32(0x0); set => _s.SetInt32(0x0, value); }
        public int Padding { get => _s.GetInt32(0x4); set => _s.SetInt32(0x4, value); }
        public float Size { get => _s.GetFloat(0x8); set => _s.SetFloat(0x8, value); }
        public float X { get => _s.GetFloat(0xc); set => _s.SetFloat(0xc, value); }
        public float Y { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }
        public float Z { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }
    }
}
