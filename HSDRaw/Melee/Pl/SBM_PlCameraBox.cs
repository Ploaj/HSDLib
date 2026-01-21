namespace HSDRaw.Melee.Pl
{
    public class SBM_PlCameraBox : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public float YOffset { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }

        public float ProjRight { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public float ProjLeft { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public float ProjTop { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        public float ProjBottom { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }

        public float x50_of_camera_box { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }
    }
}
