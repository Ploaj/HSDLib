namespace HSDRaw.MEX.Scenes
{
    public class MEX_SceneData : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public HSDArrayAccessor<MEX_MajorScene> MajorScenes { get => _s.GetReference<HSDArrayAccessor<MEX_MajorScene>>(0x00); set => _s.SetReference(0x00, value); }

        public HSDArrayAccessor<MEX_MinorFunctionTable> MinorSceneFunctions { get => _s.GetReference<HSDArrayAccessor<MEX_MinorFunctionTable>>(0x04); set => _s.SetReference(0x04, value); }
        
    }
}
