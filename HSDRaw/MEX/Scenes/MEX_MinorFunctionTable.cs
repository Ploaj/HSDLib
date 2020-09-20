namespace HSDRaw.MEX.Scenes
{
    public class MEX_MinorFunctionTable : HSDAccessor
    {
        public override int TrimmedSize => 0x14;
        
        public byte MinorSceneID { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }
        
        public int SceneThink { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int SceneLoad { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }
        
        public int SceneLeave { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }
    }
}
