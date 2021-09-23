namespace HSDRaw.AirRide.Kx
{
    public class KEX_RdFunctionTable : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public HSDUIntArray StateLogic { get => _s.GetReference<HSDUIntArray>(0x00); set => _s.SetReference(0x00, value); }

        public HSDUIntArray OnInit { get => _s.GetReference<HSDUIntArray>(0x04); set => _s.SetReference(0x04, value); }

        public HSDUIntArray OnInput { get => _s.GetReference<HSDUIntArray>(0x08); set => _s.SetReference(0x08, value); }

        public void SetRiderCount(int count)
        {
            OnInit.Array = new uint[count];
            OnInput.Array = new uint[count];
        }
    }
}
