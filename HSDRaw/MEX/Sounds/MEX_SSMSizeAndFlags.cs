namespace HSDRaw.MEX
{
    public class MEX_SSMSizeAndFlags : HSDAccessor
    {
        public override int TrimmedSize => 0x8;

        public int SSMFileSize { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public int Flag { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }
    }
}
