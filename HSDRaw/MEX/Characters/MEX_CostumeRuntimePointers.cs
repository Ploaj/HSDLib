namespace HSDRaw.MEX
{
    public class MEX_CostumeRuntimePointers : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public HSDAccessor Pointer { get => _s.GetReference<HSDAccessor>(0x00); set => _s.SetReference(0x00, value); }

        public byte CostumeCount { get => _s.GetByte(0x04); set => _s.SetByte(0x04, value); }
    }
}
