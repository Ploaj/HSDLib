namespace HSDRaw.Melee.Pl
{
    public class SBM_EnvironmentCollision : HSDAccessor
    {
        public override int TrimmedSize => 0x1C;

        public short ECBBone1 { get => _s.GetInt16(0x00); set => _s.SetInt16(0x00, value); }

        public short ECBBone2 { get => _s.GetInt16(0x02); set => _s.SetInt16(0x02, value); }

        public short ECBBone3 { get => _s.GetInt16(0x04); set => _s.SetInt16(0x04, value); }
        
        public short ECBBone4 { get => _s.GetInt16(0x06); set => _s.SetInt16(0x06, value); }

        public short ECBBone5 { get => _s.GetInt16(0x08); set => _s.SetInt16(0x08, value); }

        public short ECBBone6 { get => _s.GetInt16(0x0A); set => _s.SetInt16(0x0A, value); }

        public float Multiplier { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }
        public float LedgeGrabWidth { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }
        public float LedgeGrabYOffset { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }
        public float LedgeGrabHeight { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }
    }
}
