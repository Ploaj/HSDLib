using HSDRaw.MEX.Menus;
using HSDRaw.MEX.Stages;

namespace HSDRaw.MEX
{
    public class MEX_Data : HSDAccessor
    {
        public override int TrimmedSize => 0x30;

        public MEX_Meta MetaData { get => _s.GetReference<MEX_Meta>(0x00); set => _s.SetReference(0x00, value); }

        public MEX_MenuTable MenuTable { get => _s.GetReference<MEX_MenuTable>(0x04); set => _s.SetReference(0x04, value); }

        public MEX_FighterData FighterData { get => _s.GetReference<MEX_FighterData>(0x08); set => _s.SetReference(0x08, value); }
        
        public MEX_FighterFunctionTable FighterFunctions{ get => _s.GetReference<MEX_FighterFunctionTable>(0x0C); set => _s.SetReference(0x0C, value); }

        public MEX_SSMTable SSMTable { get => _s.GetReference<MEX_SSMTable>(0x10); set => _s.SetReference(0x10, value); }

        public MEX_BGMStruct MusicTable { get => _s.GetReference<MEX_BGMStruct>(0x14); set => _s.SetReference(0x14, value); }

        public HSDArrayAccessor<MEX_EffectFiles> EffectFiles { get => _s.GetReference<HSDArrayAccessor<MEX_EffectFiles>>(0x18); set => _s.SetReference(0x18, value); }
        
        public MEX_ItemTables ItemTable { get => _s.GetReference<MEX_ItemTables>(0x1C); set => _s.SetReference(0x1C, value); }
        
        public MEX_KirbyTable KirbyData { get => _s.GetReference<MEX_KirbyTable>(0x20); set => _s.SetReference(0x20, value); }

        public MEX_KirbyFunctionTable KirbyFunctions { get => _s.GetReference<MEX_KirbyFunctionTable>(0x24); set => _s.SetReference(0x24, value); }

        public MEX_StageData StageData { get => _s.GetReference<MEX_StageData>(0x28); set => _s.SetReference(0x28, value); }

        public HSDFixedLengthPointerArrayAccessor<MEX_Stage> StageFunctions { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<MEX_Stage>>(0x2C); set => _s.SetReference(0x2C, value); }

    }
}
