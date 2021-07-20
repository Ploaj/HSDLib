using HSDRaw.Common;
using HSDRaw.MEX.Characters;

namespace HSDRaw.MEX
{
    public class MEX_FighterData : HSDAccessor
    {
        public override int TrimmedSize => 0x80;

        public HSDFixedLengthPointerArrayAccessor<HSD_String> NameText { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSD_String>>(0x00); set => _s.SetReference(0x00, value); }

        public HSDArrayAccessor<MEX_CharFileStrings> CharFiles { get => _s.GetReference<HSDArrayAccessor<MEX_CharFileStrings>>(0x04); set => _s.SetReference(0x04, value); }

        public HSDByteArray InsigniaIDs { get => _s.GetReference<HSDByteArray>(0x08); set => _s.SetReference(0x08, value); }

        public HSDArrayAccessor<MEX_CharDefineIDs> DefineIDs { get => _s.GetReference<HSDArrayAccessor<MEX_CharDefineIDs>>(0x0C); set => _s.SetReference(0x0C, value); }

        public HSDArrayAccessor<MEX_CostumeIDs> CostumeIDs { get => _s.GetReference<HSDArrayAccessor<MEX_CostumeIDs>>(0x10); set => _s.SetReference(0x10, value); }

        public HSDArrayAccessor<MEX_CostumeFileSymbolTable> CostumeFileSymbols { get => _s.GetReference<HSDArrayAccessor<MEX_CostumeFileSymbolTable>>(0x14); set => _s.SetReference(0x14, value); }

        public HSDFixedLengthPointerArrayAccessor<MEX_FtDemoSymbolNames> FtDemo_SymbolNames { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<MEX_FtDemoSymbolNames>>(0x18); set => _s.SetReference(0x18, value); }

        public HSDFixedLengthPointerArrayAccessor<HSD_String> AnimFiles { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSD_String>>(0x1C); set => _s.SetReference(0x1C, value); }

        public HSDArrayAccessor<MEX_AnimCount> AnimCount { get => _s.GetReference<HSDArrayAccessor<MEX_AnimCount>>(0x20); set => _s.SetReference(0x20, value); }

        public HSDByteArray EffectIDs { get => _s.GetReference<HSDByteArray>(0x24); set => _s.SetReference(0x24, value); }

        public HSDFixedLengthPointerArrayAccessor<HSD_String> ResultAnimFiles { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSD_String>>(0x28); set => _s.SetReference(0x28, value); }

        public HSDFloatArray ResultScale { get => _s.GetReference<HSDFloatArray>(0x2C); set => _s.SetReference(0x2C, value); }

        public HSDIntArray VictoryThemeIDs { get => _s.GetReference<HSDIntArray>(0x30); set => _s.SetReference(0x30, value); }

        public HSDIntArray AnnouncerCalls { get => _s.GetReference<HSDIntArray>(0x34); set => _s.SetReference(0x34, value); }

        public HSDArrayAccessor<MEX_CharSSMFileID> SSMFileIDs { get => _s.GetReference<HSDArrayAccessor<MEX_CharSSMFileID>>(0x38); set => _s.SetReference(0x38, value); }

        public HSDArrayAccessor<MEX_CostumeRuntimePointers> CostumePointers { get => _s.GetReference<HSDArrayAccessor<MEX_CostumeRuntimePointers>>(0x3C); set => _s.SetReference(0x3C, value); }

        // 0x40 ft Data Runtime Pointer Struct. Size is internalIDCount * 8

        public HSDByteArray WallJump { get => _s.GetReference<HSDByteArray>(0x44); set => _s.SetReference(0x44, value); }
        
        public HSDArrayAccessor<MEX_RstRuntime> RstRuntime { get => _s.GetReference<HSDArrayAccessor<MEX_RstRuntime>>(0x48); set => _s.SetReference(0x48, value); }
        
        public HSDArrayAccessor<MEX_ItemLookup> FighterItemLookup { get => _s.GetReference<HSDArrayAccessor<MEX_ItemLookup>>(0x4C); set => _s.SetReference(0x4C, value); }

        public HSDUShortArray TargetTestStageLookups { get => _s.GetReference<HSDUShortArray>(0x50); set => _s.SetReference(0x50, value); }

        public HSDArrayAccessor<MEX_FighterSongID> FighterSongIDs { get => _s.GetReference<HSDArrayAccessor<MEX_FighterSongID>>(0x54); set => _s.SetReference(0x54, value); }

        public HSDFixedLengthPointerArrayAccessor<HSD_String> VIFiles { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSD_String>>(0x58); set => _s.SetReference(0x58, value); }
        
        public HSDFixedLengthPointerArrayAccessor<HSD_String> EndClassicFiles { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSD_String>>(0x5C); set => _s.SetReference(0x5C, value); }

        public HSDFixedLengthPointerArrayAccessor<HSD_String> EndAdventureFiles { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSD_String>>(0x60); set => _s.SetReference(0x60, value); }

        public HSDFixedLengthPointerArrayAccessor<HSD_String> EndAllStarFiles { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSD_String>>(0x64); set => _s.SetReference(0x64, value); }

        public HSDFixedLengthPointerArrayAccessor<HSD_String> EndMovieFiles { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSD_String>>(0x68); set => _s.SetReference(0x68, value); }

        public HSDIntArray RaceToFinishTimeLimits { get => _s.GetReference<HSDIntArray>(0x6C); set => _s.SetReference(0x6C, value); }

        public HSDIntArray RuntimeIntroParamLookup { get => _s.GetReference<HSDIntArray>(0x70); set => _s.SetReference(0x70, value); }

        //public HSDArrayAccessor<HSD_UShort> AdventureTrophyLookup { get => _s.GetReference<HSDArrayAccessor<HSD_UShort>>(0x5C); set => _s.SetReference(0x5C, value); }

        public override void New()
        {
            base.New();

            NameText = new HSDFixedLengthPointerArrayAccessor<HSD_String>();
            CharFiles = new HSDArrayAccessor<MEX_CharFileStrings>();
            InsigniaIDs = new HSDByteArray();
            DefineIDs = new HSDArrayAccessor<MEX_CharDefineIDs>();
            CostumeIDs = new HSDArrayAccessor<MEX_CostumeIDs>();
            CostumeFileSymbols = new HSDArrayAccessor<MEX_CostumeFileSymbolTable>();
            FtDemo_SymbolNames = new HSDFixedLengthPointerArrayAccessor<MEX_FtDemoSymbolNames>();
            AnimFiles = new HSDFixedLengthPointerArrayAccessor<HSD_String>();
            AnimCount = new HSDArrayAccessor<MEX_AnimCount>();
            EffectIDs = new HSDByteArray();
            ResultAnimFiles = new HSDFixedLengthPointerArrayAccessor<HSD_String>();
            ResultScale = new HSDFloatArray();
            VictoryThemeIDs = new HSDIntArray();
            AnnouncerCalls = new HSDIntArray();
            SSMFileIDs = new HSDArrayAccessor<MEX_CharSSMFileID>();
            CostumePointers = new HSDArrayAccessor<MEX_CostumeRuntimePointers>();
            WallJump = new HSDByteArray();
            RstRuntime = new HSDArrayAccessor<MEX_RstRuntime>();
            FighterItemLookup = new HSDArrayAccessor<MEX_ItemLookup>();
            TargetTestStageLookups = new HSDUShortArray();
            FighterSongIDs = new HSDArrayAccessor<MEX_FighterSongID>();
            VIFiles = new HSDFixedLengthPointerArrayAccessor<HSD_String>();
            EndAdventureFiles = new HSDFixedLengthPointerArrayAccessor<HSD_String>();
            EndAllStarFiles = new HSDFixedLengthPointerArrayAccessor<HSD_String>();
            EndClassicFiles = new HSDFixedLengthPointerArrayAccessor<HSD_String>();
            EndMovieFiles = new HSDFixedLengthPointerArrayAccessor<HSD_String>();
            RaceToFinishTimeLimits = new HSDIntArray();
            RuntimeIntroParamLookup = new HSDIntArray();

        }
    }
}
