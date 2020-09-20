namespace HSDRaw.MEX.Scenes
{
    public class MEX_MajorScene : HSDAccessor
    {
        public override int TrimmedSize => 0x14;

        public bool Preload { get => _s.GetByte(0x00) == 1; set => _s.SetByte(0x00, value ? (byte)1 : (byte)0); }

        public byte MajorSceneID { get => _s.GetByte(0x01); set => _s.SetByte(0x01, value); }

        public int LoadFunction { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int UnloadFunction { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public int OnBootFunction { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

        //public int MinorScene { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        public HSDArrayAccessor<MEX_MinorScene> MinorScene { get => _s.GetReference<HSDArrayAccessor<MEX_MinorScene>>(0x10); set => _s.SetReference(0x10, value); }
    }
}
