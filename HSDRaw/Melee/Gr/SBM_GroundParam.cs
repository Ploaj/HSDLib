namespace HSDRaw.Melee.Gr
{
    public class SBM_GroundParam : HSDAccessor
    {
        public override int TrimmedSize => 0x128;

        public float StageScale { get => _s.GetFloat(0x0); set => _s.SetFloat(0x0, value); }

        public float Unknown1 { get => _s.GetFloat(0x4); set => _s.SetFloat(0x4, value); }

        public float Unknown2 { get => _s.GetFloat(0x8); set => _s.SetFloat(0x8, value); }

        public int Unknown3 { get => _s.GetInt32(0xc); set => _s.SetInt32(0xc, value); }

        public int Unknown4 { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        public int TiltScale { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }

        public float HorizontalRotation { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }

        public float VerticalRotation { get => _s.GetFloat(0x1c); set => _s.SetFloat(0x1c, value); }

        public float Fixedness { get => _s.GetFloat(0x20); set => _s.SetFloat(0x20, value); }

        public float BubbleMultiplier { get => _s.GetFloat(0x24); set => _s.SetFloat(0x24, value); }

        public float CameraSpeedSmoothness { get => _s.GetFloat(0x28); set => _s.SetFloat(0x28, value); }

        public int Unknown5 { get => _s.GetInt32(0x2c); set => _s.SetInt32(0x2c, value); }

        public int PauseMinZ { get => _s.GetInt32(0x30); set => _s.SetInt32(0x30, value); }

        public int PauseInitialZ { get => _s.GetInt32(0x34); set => _s.SetInt32(0x34, value); }

        public int PauseMaxZ { get => _s.GetInt32(0x38); set => _s.SetInt32(0x38, value); }

        public int Unknown6 { get => _s.GetInt32(0x3c); set => _s.SetInt32(0x3c, value); }

        public float PauseMaxAngleUp { get => _s.GetFloat(0x40); set => _s.SetFloat(0x40, value); }

        public float PauseMaxAngleLeft { get => _s.GetFloat(0x44); set => _s.SetFloat(0x44, value); }

        public float PauseMaxAngleRight { get => _s.GetFloat(0x48); set => _s.SetFloat(0x48, value); }

        public float PauseMaxAngleDown { get => _s.GetFloat(0x4c); set => _s.SetFloat(0x4c, value); }

        public float Unknown7 { get => _s.GetFloat(0x50); set => _s.SetFloat(0x50, value); }

        public float Unknown8 { get => _s.GetFloat(0x54); set => _s.SetFloat(0x54, value); }

        public float Unknown9 { get => _s.GetFloat(0x58); set => _s.SetFloat(0x58, value); }

        public float Unknown10 { get => _s.GetFloat(0x5c); set => _s.SetFloat(0x5c, value); }

        public float Unknown11 { get => _s.GetFloat(0x60); set => _s.SetFloat(0x60, value); }

        public float Unknown12 { get => _s.GetFloat(0x64); set => _s.SetFloat(0x64, value); }
    }
}
