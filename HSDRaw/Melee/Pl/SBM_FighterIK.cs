namespace HSDRaw.Melee.Pl
{
    public class SBM_FighterIK : HSDAccessor
    {
        public override int TrimmedSize => 0x34;

        public byte RLegJ { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }

        public byte LLegJ { get => _s.GetByte(0x01); set => _s.SetByte(0x01, value); }

        public float LegParam { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public byte RKneeJ { get => _s.GetByte(0x08); set => _s.SetByte(0x08, value); }

        public byte LKneeJ { get => _s.GetByte(0x09); set => _s.SetByte(0x09, value); }

        public float KneeParam { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        public byte RFootJ { get => _s.GetByte(0x10); set => _s.SetByte(0x10, value); }

        public byte LFootJ { get => _s.GetByte(0x11); set => _s.SetByte(0x11, value); }

        public float FootParam1 { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }

        public float FootParam2 { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }

        public byte RShoulderJ { get => _s.GetByte(0x1C); set => _s.SetByte(0x1C, value); }

        public byte LShoulderJ { get => _s.GetByte(0x1D); set => _s.SetByte(0x1D, value); }

        public float ShoulderParam { get => _s.GetFloat(0x20); set => _s.SetFloat(0x20, value); }

        public byte RArmJ { get => _s.GetByte(0x24); set => _s.SetByte(0x24, value); }

        public byte LArmJ { get => _s.GetByte(0x25); set => _s.SetByte(0x25, value); }

        public float ArmParam { get => _s.GetFloat(0x28); set => _s.SetFloat(0x28, value); }

        public float UnkParam1 { get => _s.GetFloat(0x2C); set => _s.SetFloat(0x2C, value); }

        public float UnkParam2 { get => _s.GetFloat(0x30); set => _s.SetFloat(0x30, value); }
    }
}
