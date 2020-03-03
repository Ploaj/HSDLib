namespace HSDRaw.MEX.Characters
{
    public class MEX_RstRuntime : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public int AnimMax { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }
    }
}
