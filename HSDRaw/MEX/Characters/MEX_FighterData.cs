using HSDRaw.Common;

namespace HSDRaw.MEX
{
    public class MEX_FighterData : HSDAccessor
    {
        public override int TrimmedSize => 0x48;

        public HSDNullPointerArrayAccessor<HSD_String> NameText { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_String>>(0x00); set => _s.SetReference(0x00, value); }

        public HSDArrayAccessor<MEX_CharFileStrings> CharFiles { get => _s.GetReference<HSDArrayAccessor<MEX_CharFileStrings>>(0x04); set => _s.SetReference(0x04, value); }

        public HSDArrayAccessor<HSD_Byte> InsigniaIDs { get => _s.GetReference<HSDArrayAccessor<HSD_Byte>>(0x08); set => _s.SetReference(0x08, value); }

        public HSDArrayAccessor<MEX_CharDefineIDs> DefineIDs { get => _s.GetReference<HSDArrayAccessor<MEX_CharDefineIDs>>(0x0C); set => _s.SetReference(0x0C, value); }

        public HSDArrayAccessor<MEX_CostumeIDs> CostumeIDs { get => _s.GetReference<HSDArrayAccessor<MEX_CostumeIDs>>(0x10); set => _s.SetReference(0x10, value); }

        public HSDArrayAccessor<MEX_CostumeFileSymbolTable> CostumeFileSymbols { get => _s.GetReference<HSDArrayAccessor<MEX_CostumeFileSymbolTable>>(0x14); set => _s.SetReference(0x14, value); }

        public HSDNullPointerArrayAccessor<MEX_FtDemoSymbolNames> FtDemo_SymbolNames { get => _s.GetReference<HSDNullPointerArrayAccessor<MEX_FtDemoSymbolNames>>(0x18); set => _s.SetReference(0x18, value); }

        public HSDNullPointerArrayAccessor<HSD_String> AnimFiles { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_String>>(0x1C); set => _s.SetReference(0x1C, value); }

        public HSDArrayAccessor<MEX_AnimCount> AnimCount { get => _s.GetReference<HSDArrayAccessor<MEX_AnimCount>>(0x20); set => _s.SetReference(0x20, value); }

        public HSDArrayAccessor<HSD_Byte> EffectIDs { get => _s.GetReference<HSDArrayAccessor<HSD_Byte>>(0x24); set => _s.SetReference(0x24, value); }

        public HSDNullPointerArrayAccessor<HSD_String> ResultAnimFiles { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_String>>(0x28); set => _s.SetReference(0x28, value); }

        public HSDArrayAccessor<HSD_Float> ResultScale { get => _s.GetReference<HSDArrayAccessor<HSD_Float>>(0x2C); set => _s.SetReference(0x2C, value); }

        public HSDArrayAccessor<HSD_Int> VictoryThemeIDs { get => _s.GetReference<HSDArrayAccessor<HSD_Int>>(0x30); set => _s.SetReference(0x30, value); }

        public HSDArrayAccessor<HSD_Int> AnnouncerCalls { get => _s.GetReference<HSDArrayAccessor<HSD_Int>>(0x34); set => _s.SetReference(0x34, value); }

        public HSDArrayAccessor<MEX_CharSSMFileID> SSMFileIDs { get => _s.GetReference<HSDArrayAccessor<MEX_CharSSMFileID>>(0x38); set => _s.SetReference(0x38, value); }

        public HSDArrayAccessor<MEX_CostumeRuntimePointers> CostumePointers { get => _s.GetReference<HSDArrayAccessor<MEX_CostumeRuntimePointers>>(0x3C); set => _s.SetReference(0x3C, value); }
        
        // 0x40 ft Data Runtime Pointer Struct. Size is internalIDCount * 8
    }
}
