namespace HSDRaw.Melee.Pl
{
    public class SBM_PlActionChance : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public int ActionID { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public int Chance { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }
    }
}
