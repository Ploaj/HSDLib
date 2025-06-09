namespace HSDRaw.Melee.Pl
{
    public class SBM_PlItemPickupParam : HSDAccessor
    {
        public override int TrimmedSize => 0x30;

        public float LightGroundedXOffset { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }
        public float LightGroundedYOffset { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }
        public float LightGroundedXRange { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }
        public float LightGroundedYRange { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        public float HeavyXOffset { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }
        public float HeavyYOffset { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }
        public float HeavyXRange { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }
        public float HeavyYRange { get => _s.GetFloat(0x1C); set => _s.SetFloat(0x1C, value); }

        public float LightAerialXOffset { get => _s.GetFloat(0x20); set => _s.SetFloat(0x20, value); }
        public float LightAerialYOffset { get => _s.GetFloat(0x24); set => _s.SetFloat(0x24, value); }
        public float LightAerialXRange { get => _s.GetFloat(0x28); set => _s.SetFloat(0x28, value); }
        public float LightAerialYRange { get => _s.GetFloat(0x2C); set => _s.SetFloat(0x2C, value); }
    }
}
