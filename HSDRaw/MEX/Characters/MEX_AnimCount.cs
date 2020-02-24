namespace HSDRaw.MEX
{
    public class MEX_AnimCount : HSDAccessor
    {
        public override int TrimmedSize => 8;

        // runtime pointer at 0x00

        public int AnimCount { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }
    }
}
