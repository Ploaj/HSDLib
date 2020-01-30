namespace HSDRaw.Melee.Gr
{
    public class SBM_GroundParam : HSDAccessor
    {
        public override int TrimmedSize => 0xDC;

        public float StageScale { get => _s.GetFloat(0x0); set => _s.SetFloat(0x0, value); }

        public float DeltaCam1 { get => _s.GetFloat(0x4); set => _s.SetFloat(0x4, value); }

        public float DeltaCam2 { get => _s.GetFloat(0x8); set => _s.SetFloat(0x8, value); }

        public int CameraDistanceMin { get => _s.GetInt32(0xc); set => _s.SetInt32(0xc, value); }

        public int CameraDistanceMax { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        public int TiltScale { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }

        public float HorizontalRotation { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }

        public float VerticalRotation { get => _s.GetFloat(0x1c); set => _s.SetFloat(0x1c, value); }

        public float Fixedness { get => _s.GetFloat(0x20); set => _s.SetFloat(0x20, value); }

        public float BubbleMultiplier { get => _s.GetFloat(0x24); set => _s.SetFloat(0x24, value); }

        public float CameraSpeedSmoothness { get => _s.GetFloat(0x28); set => _s.SetFloat(0x28, value); }

        public int Unknown1 { get => _s.GetInt32(0x2c); set => _s.SetInt32(0x2c, value); }

        public int PauseMinZ { get => _s.GetInt32(0x30); set => _s.SetInt32(0x30, value); }

        public int PauseInitialZ { get => _s.GetInt32(0x34); set => _s.SetInt32(0x34, value); }

        public int PauseMaxZ { get => _s.GetInt32(0x38); set => _s.SetInt32(0x38, value); }

        public float Unknown2 { get => _s.GetFloat(0x3c); set => _s.SetFloat(0x3c, value); }

        public float PauseMaxAngleUp { get => _s.GetFloat(0x40); set => _s.SetFloat(0x40, value); }

        public float PauseMaxAngleLeft { get => _s.GetFloat(0x44); set => _s.SetFloat(0x44, value); }

        public float PauseMaxAngleRight { get => _s.GetFloat(0x48); set => _s.SetFloat(0x48, value); }

        public float PauseMaxAngleDown { get => _s.GetFloat(0x4c); set => _s.SetFloat(0x4c, value); }

        public float Unknown3 { get => _s.GetFloat(0x50); set => _s.SetFloat(0x50, value); }

        public float Unknown4 { get => _s.GetFloat(0x54); set => _s.SetFloat(0x54, value); }

        public float Unknown5 { get => _s.GetFloat(0x58); set => _s.SetFloat(0x58, value); }

        public float Unknown6 { get => _s.GetFloat(0x5c); set => _s.SetFloat(0x5c, value); }

        public float Unknown7 { get => _s.GetFloat(0x60); set => _s.SetFloat(0x60, value); }

        public float Unknown8 { get => _s.GetFloat(0x64); set => _s.SetFloat(0x64, value); }

        public short UnknownShort0 { get => _s.GetInt16(0x68); set => _s.SetInt16(0x68, value); }

        public short UnknownShort1 { get => _s.GetInt16(0x6c); set => _s.SetInt16(0x6c, value); }

        public short UnknownShort2 { get => _s.GetInt16(0x70); set => _s.SetInt16(0x70, value); }

        public short UnknownShort3 { get => _s.GetInt16(0x74); set => _s.SetInt16(0x74, value); }

        public short UnknownShort4 { get => _s.GetInt16(0x78); set => _s.SetInt16(0x78, value); }

        public short UnknownShort5 { get => _s.GetInt16(0x7c); set => _s.SetInt16(0x7c, value); }

        public short UnknownShort6 { get => _s.GetInt16(0x80); set => _s.SetInt16(0x80, value); }

        public short UnknownShort7 { get => _s.GetInt16(0x84); set => _s.SetInt16(0x84, value); }

        public short UnknownShort8 { get => _s.GetInt16(0x88); set => _s.SetInt16(0x88, value); }

        public short UnknownShort9 { get => _s.GetInt16(0x8c); set => _s.SetInt16(0x8c, value); }

        public short UnknownShort10 { get => _s.GetInt16(0x90); set => _s.SetInt16(0x90, value); }

        public short UnknownShort11 { get => _s.GetInt16(0x94); set => _s.SetInt16(0x94, value); }

        public short UnknownShort12 { get => _s.GetInt16(0x98); set => _s.SetInt16(0x98, value); }

        public short UnknownShort13 { get => _s.GetInt16(0x9c); set => _s.SetInt16(0x9c, value); }

        public short UnknownShort14 { get => _s.GetInt16(0xa0); set => _s.SetInt16(0xa0, value); }

        public short UnknownShort15 { get => _s.GetInt16(0xa4); set => _s.SetInt16(0xa4, value); }

        public short UnknownShort16 { get => _s.GetInt16(0xa8); set => _s.SetInt16(0xa8, value); }

        public short UnknownShort17 { get => _s.GetInt16(0xac); set => _s.SetInt16(0xac, value); }

        // TODO: there are actually multiple of these
        public SBM_GroundBGM BGM { get => _s.GetReference<SBM_GroundBGM>(0xB0); set => _s.SetReference(0xB0, value); }

        public int VariationCount { get => _s.GetInt32(0xB4); set => _s.SetInt32(0xB4, value); }

        public float BubbleColorTopLeft { get => _s.GetFloat(0xB8); set => _s.SetFloat(0xB8, value); }

        public float BubbleColorTopMiddle { get => _s.GetFloat(0xBC); set => _s.SetFloat(0xBC, value); }

        public float BubbleColorTopRight { get => _s.GetFloat(0xC0); set => _s.SetFloat(0xC0, value); }

        public float BubbleColorSidesTop { get => _s.GetFloat(0xC4); set => _s.SetFloat(0xC4, value); }

        public float BubbleColorSidesMiddle { get => _s.GetFloat(0xC8); set => _s.SetFloat(0xC8, value); }

        public float BubbleColorSidesBottom { get => _s.GetFloat(0xCC); set => _s.SetFloat(0xCC, value); }

        public float BubbleColorBottomLeft { get => _s.GetFloat(0xD0); set => _s.SetFloat(0xD0, value); }

        public float BubbleColorBottomMiddle { get => _s.GetFloat(0xD4); set => _s.SetSetFloatInt32(0xD4, value); }

        public float BubbleColorBottomRight { get => _s.GetFloat(0xD8); set => _s.SetFloat(0xD8, value); }
    }

    public class SBM_GroundBGM : HSDAccessor
    {
        public override int TrimmedSize => 0x64;

        public int GrKind { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public int MainMusic { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int AltMusic { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public int SuddenDeathMainMusic { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

        public int SuddenDeathAltMusic { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        public short SongBehaviorFlag { get => _s.GetInt16(0x14); set => _s.SetInt16(0x14, value); }

        public short ChanceToPlayAltSong { get => _s.GetInt16(0x16); set => _s.SetInt16(0x16, value); }
    }
}
