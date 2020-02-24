using HSDRaw.Common;

namespace HSDRaw.MEX
{
    public class MEX_Data : HSDAccessor
    {
        public override int TrimmedSize => 0x28;

        public MEX_Meta MetaData { get => _s.GetReference<MEX_Meta>(0x00); set => _s.SetReference(0x00, value); }

        public MEX_IconData CSSIconData { get => _s.GetReference<MEX_IconData>(0x04); set => _s.SetReference(0x04, value); }

        public MEX_FighterData FighterData { get => _s.GetReference<MEX_FighterData>(0x08); set => _s.SetReference(0x08, value); }
        
        public MEX_FighterFunctionTable FighterFunctions{ get => _s.GetReference<MEX_FighterFunctionTable>(0x0C); set => _s.SetReference(0x0C, value); }

        public MEX_SSMTable SSMTable { get => _s.GetReference<MEX_SSMTable>(0x10); set => _s.SetReference(0x10, value); }

        public MEX_BGMStruct MusicTable { get => _s.GetReference<MEX_BGMStruct>(0x14); set => _s.SetReference(0x14, value); }

        public HSDArrayAccessor<MEX_EffectFiles> EffectFiles { get => _s.GetReference<HSDArrayAccessor<MEX_EffectFiles>>(0x18); set => _s.SetReference(0x18, value); }
        
        // 0x1C

        // 0x20

        // 0x24
    }
}
