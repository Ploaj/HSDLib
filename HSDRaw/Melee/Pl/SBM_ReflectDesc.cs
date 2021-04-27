namespace HSDRaw.Melee.Pl
{
    public class SBM_ReflectDesc : HSDAccessor
    {
        public override int TrimmedSize => 0x24;

        public int Bone { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public int MaxDamage { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public float X { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public float Y { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        public float Z { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }

        public float Radius { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }

        public float DamageMultiplier { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }

        public float VelocityMultiplier { get => _s.GetFloat(0x1C); set => _s.SetFloat(0x1C, value); }

        public int Flags { get => _s.GetInt32(0x20); set => _s.SetInt32(0x20, value); }
    }
}
