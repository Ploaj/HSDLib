namespace HSDRaw.Common
{
    public class HSD_Vector3 : HSDAccessor
    {
        public override int TrimmedSize => 0x0C;

        public float X { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }

        public float Y { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public float Z { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }
    }


    public class HSD_Byte : HSDAccessor
    {
        public override int TrimmedSize => 0x01;

        public byte Value { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }
    }

    public class HSD_UInt : HSDAccessor
    {
        public override int TrimmedSize => 0x04;

        public uint Value { get => (uint)_s.GetInt32(0x00); set => _s.SetInt32(0x00, (int)value); }
    }

    public class HSD_Int : HSDAccessor
    {
        public override int TrimmedSize => 0x04;

        public int Value { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }
    }

    public class HSD_Float : HSDAccessor
    {
        public override int TrimmedSize => 0x04;

        public float Value { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }
    }
}
