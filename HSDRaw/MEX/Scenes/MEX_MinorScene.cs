namespace HSDRaw.MEX.Scenes
{
    public class MEX_MinorScene : HSDAccessor
    {
        public override int TrimmedSize => 0x18;
        
        public byte MinorSceneID { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }

        public byte PersistantHeapCount { get => _s.GetByte(0x01); set => _s.SetByte(0x01, value); }

        public int ScenePrepFunction { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int SceneDecideFunction { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public byte CommonMinorID { get => _s.GetByte(0x0C); set => _s.SetByte(0x0C, value); }

        public int StaticStruct1 { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        public int StaticStruct2 { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }
    }
}
