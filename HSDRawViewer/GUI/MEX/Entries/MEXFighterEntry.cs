using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Melee;
using HSDRaw.MEX;
using HSDRaw.MEX.Characters;
using System;
using System.ComponentModel;

namespace HSDRawViewer.GUI.MEX
{
    public class MEXIdConverter
    {
        private static int BaseCharacterCount { get; } = 0x21;

        public static int InternalSpecialCharCount { get; } = 6;

        public static int ExternalSpecialCharCount { get; } = 7;

        private static int[] ExternalToInternal = {
            0x02, 0x03, 0x01, 0x18, 0x04, 0x05, 0x06,
            0x11, 0x00, 0x12, 0x10, 0x08, 0x09, 0x0C,
            0x0A, 0x0F, 0x0D, 0x0E, 0x13, 0x07, 0x16,
            0x14, 0x15, 0x1A, 0x17, 0x19, 0x1B, 0x1D,
            0x1E, 0x1F, 0x1C, 0x20, 0x0A
        };

        private static int[] InternalToExternal = {
            0x08, 0x02, 0x00, 0x01, 0x04, 0x05, 0x06,
            0x13, 0x0B, 0x0C, 0x0E, 0x20, 0x0D, 0x10,
            0x11, 0x0F, 0x0A, 0x07, 0x09, 0x12, 0x15,
            0x16, 0x14, 0x18, 0x03, 0x19, 0x17, 0x1A,
            0x1E, 0x1B, 0x1C, 0x1D, 0x1F};

        public static int ToExternalID(int internalID, int characterCount)
        {
            var addedChars = characterCount - BaseCharacterCount;
            bool isSpecialCharacter = internalID >= characterCount - InternalSpecialCharCount;

            if (internalID >= characterCount - InternalSpecialCharCount - addedChars &&
                !isSpecialCharacter)
                return (BaseCharacterCount - ExternalSpecialCharCount) + (internalID - (BaseCharacterCount - InternalSpecialCharCount));
            
            int externalId = internalID + (isSpecialCharacter ? -addedChars : 0);

            if (externalId < InternalToExternal.Length)
                externalId = InternalToExternal[externalId];

            if (isSpecialCharacter)
                externalId += addedChars;

            if (internalID == 11) // POPO special case
                externalId = characterCount - 1;

            return externalId;
        }
    }

    [Serializable]
    public class MEXFighterEntry
    {
        [DisplayName("Name"), Category("0 - General"), Description("Name used for CSS screen")]
        public string NameText { get; set; }

        [DisplayName("Fighter Data File"), Category("0 - General"), Description("File containing fighter's data")]
        public string FighterDataPath { get; set; }

        [DisplayName("Fighter Data Symbol"), Category("0 - General"), Description("Symbol used inside of Fighter Data File")]
        public string FighterDataSymbol { get; set; }

        [DisplayName("Animation File"), Category("0 - General"), Description("File Containing the Fighter Animations")]
        public string AnimFile { get; set; }

        [DisplayName("Animation Count"), Category("0 - General"), Description("Number of Animations Fighter has")]
        public int AnimCount { get; set; }

        [DisplayName("Result Animation File"), Category("0 - General"), Description("File Containing the Result Fighter Animations")]
        public string RstAnimFile { get; set; }

        [DisplayName("Result Animation Count"), Category("0 - General"), Description("Number of Result Animations")]
        public int RstAnimCount { get; set; }

        [DisplayName("MEX Items"), Category("0 - General"), Description("MEX Item lookup for Fighter")]
        public HSD_UShort[] MEXItems { get; set; }

        //[DisplayName("MEX Effects"), Category("0 - General"), Description("MEX Effect lookup for Fighter")]
        //public MEXEffectType[] MEXEffects { get; set; }

        [Browsable(false)]
        public byte CostumeCount { get => (byte)Costumes.Length; }

        [DisplayName("Costumes"), Category("1 - CSS"), Description("")]
        public MEX_CostumeFileSymbol[] Costumes { get; set; }

        [DisplayName("Red Costume Index"), Category("1 - CSS"), Description("Index of RED costume")]
        public byte RedCostumeID { get; set; }

        [DisplayName("Blue Costume Index"), Category("1 - CSS"), Description("Index of BLUE costume")]
        public byte BlueCostumeID { get; set; }

        [DisplayName("Green Costume Index"), Category("1 - CSS"), Description("Index of GREEN costume")]
        public byte GreenCostumeID { get; set; }

        [DisplayName("VI Wait File"), Category("2 - Demo"), Description("")]
        public string DemoFile { get; set; }

        [DisplayName("Vi Wait"), Category("2 - Demo"), Description("")]
        public string DemoWait { get; set; }

        [DisplayName("Result"), Category("2 - Demo"), Description("")]
        public string DemoResult { get; set; }

        [DisplayName("Intro"), Category("2 - Demo"), Description("")]
        public string DemoIntro { get; set; }

        [DisplayName("Ending"), Category("2 - Demo"), Description("")]
        public string DemoEnding { get; set; }

        [DisplayName("Classic Ending Image File"), Category("2 - Demo"), Description("")]
        public string EndClassicFile { get; set; }

        [DisplayName("Adventure Ending Image File"), Category("2 - Demo"), Description("")]
        public string EndAdventureFile { get; set; }

        [DisplayName("All Star Ending Image File"), Category("2 - Demo"), Description("")]
        public string EndAllStarFile { get; set; }

        [DisplayName("Ending Movie File"), Category("2 - Demo"), Description("")]
        public string EndMovieFile { get; set; }

        [DisplayName("Target Test Stage"), Category("3 - Misc"), Description("")]
        public int TargetTestStage { get; set; }

        [DisplayName("Race to the Finish Time"), Category("3 - Misc"), Description("")]
        public int RacetoTheFinishTime { get; set; }

        //[DisplayName("Adventure Trophy Index"), Category("3 - Misc"), Description("")]
        //public short AdventureTrophyLookup { get; set; }

        [DisplayName("Result Screen Scale"), Category("3 - Misc"), Description("")]
        public float ResultScreenScale { get; set; }

        [DisplayName("Can Wall Jump"), Category("3 - Misc"), Description("")]
        public bool CanWallJump { get; set; }

        [DisplayName("Insignia ID"), Category("3 - Misc"), Description("")]
        public byte InsigniaID { get; set; }

        [DisplayName("Victory Theme"), Category("3 - Misc"), Description(""), TypeConverter(typeof(MusicIDConverter))]
        public int VictoryThemeID { get; set; }

        [DisplayName("Fighter Song 1"), Category("3 - Misc"), Description(""), TypeConverter(typeof(MusicIDConverter))]
        public int FighterSongID1 { get; set; }

        [DisplayName("Fighter Song 2"), Category("3 - Misc"), Description(""), TypeConverter(typeof(MusicIDConverter))]
        public int FighterSongID2 { get; set; }

        [DisplayName("Effect File"), Category("3 - Misc"), Description(""), TypeConverter(typeof(EffectIDConverter))]
        public int EffectIndex { get; set; }

        [DisplayName("SSM"), Category("3 - Misc"), Description("Index of SSM file for this fighter"), TypeConverter(typeof(SSMIDConverter))]
        public int SSMIndex { get; set; }

        // no need to expose these to the user
        [Browsable(false), DisplayName("SSM Bitfield 1"), Category("3 - Misc"), Description(""), TypeConverter(typeof(HexType))]
        public uint SSMBitfield1 { get; set; }

        [Browsable(false), DisplayName("SSM Bitfield 2"), Category("3 - Misc"), Description(""), TypeConverter(typeof(HexType))]
        public uint SSMBitfield2 { get; set; }

        [DisplayName("Narrator Sound Clip"), Category("3 - Misc"), Description("Index of narrator sound clip")]
        public int AnnouncerCall { get; set; }

        [DisplayName("Sub-Character"), 
            Category("3 - Misc"), Description(""), 
            TypeConverter(typeof(FighterInternalIDConverter))]
        public int SubCharacterInternalID { get; set; }

        [DisplayName("SubCharacter Behavior"), Category("3 - Misc"), Description("")]
        public SubCharacterBehavior SubCharacterBehavior { get; set; }


        ///Note: if you change this name update MexTypeInspector as well
        [DisplayName("Unk Table"), Category("3 - Misc"), Description("")]
        public SBM_PlCoUnknownFighterTableEntry[] UnkTableEntries
        {
            get
            {
                if (UnkTable == null)
                    return null;

                return UnkTable.Entries;
            }
            set
            {
                if (value == null || value.Length == 0)
                    UnkTable = null;
                else
                {
                    if (UnkTable == null)
                        UnkTable = new SBM_PlCoUnknownFighterTable();

                    UnkTable.Entries = value;
                }
            }
        }

        public SBM_PlCoUnknownFighterTable UnkTable;


        [DisplayName("Kirby Cap FileName"), Category("4 - Kirby"), Description("")]
        public string KirbyCapFileName { get; set; }

        [DisplayName("Kirby Cap Symbol"), Category("4 - Kirby"), Description("")]
        public string KirbyCapSymbol { get; set; }

        [DisplayName("Kirby Costumes"), Category("4 - Kirby"), Description("")]
        public MEX_CostumeFileSymbol[] KirbySpecialCostumes { get; set; }

        [DisplayName("Kirby Effect ID"), Category("4 - Kirby"), Description(""), TypeConverter(typeof(EffectIDConverter))]
        public int KirbyEffectID { get; set; }

        //[DisplayName("Kirby Special State Index"), Category("4 - Kirby"), Description("")]
        //public ushort KirbyNSpecialStateIndex { get; set; }

        //[DisplayName("Kirby Special (Air) State Index"), Category("4 - Kirby"), Description("")]
        //public ushort KirbyNSpecialAirStateIndex { get; set; }


        public MEXFunctionPointers Functions = new MEXFunctionPointers();

        public SBM_BoneLookupTable BoneTable;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mexData"></param>
        /// <param name="internalID"></param>
        /// <returns></returns>
        public bool IsSpecialCharacterInternal(MEX_Data mexData, int internalID)
        {
            return internalID >= mexData.MetaData.NumOfInternalIDs - MEXIdConverter.InternalSpecialCharCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mexData"></param>
        /// <param name="externalID"></param>
        /// <returns></returns>
        public bool IsSpecialCharacterExternal(MEX_Data mexData, int externalID)
        {
            return externalID >= mexData.MetaData.NumOfExternalIDs - MEXIdConverter.ExternalSpecialCharCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        private byte[] GenerateSpecialBuffer(int length, string s)
        {
            byte[] b = new byte[length];
            for (int i = 0; i < length; i++)
                b[i] = (byte)s[i % s.Length];
            return b;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mexData"></param>
        /// <param name="internalId"></param>
        /// <param name="externalID"></param>
        /// <returns></returns>
        public MEXFighterEntry LoadData(MEX_Data mexData, int internalId, int externalID)
        {
            var fd = mexData.FighterData;

            Functions.LoadData(mexData, internalId, externalID);

            NameText = fd.NameText[externalID].Value;
            FighterDataPath = fd.CharFiles[internalId].FileName;
            FighterDataSymbol = fd.CharFiles[internalId].Symbol;
            AnimFile = fd.AnimFiles[internalId].Value;
            AnimCount = fd.AnimCount[internalId].AnimCount;
            RstAnimCount = fd.RstRuntime[internalId].AnimMax;

            MEXItems = fd.FighterItemLookup[internalId].Entries;
            //MEXEffects = fd.FighterEffectLookup[internalId].Entries;

            InsigniaID = fd.InsigniaIDs[externalID].Value;

            CanWallJump = fd.WallJump[internalId].Value != 0;

            Costumes = fd.CostumeFileSymbols[internalId].CostumeSymbols.Array;

            RstAnimFile = fd.ResultAnimFiles[externalID].Value;

            EffectIndex = fd.EffectIDs[internalId].Value;
            AnnouncerCall = fd.AnnouncerCalls[externalID].Value;

            SSMIndex = fd.SSMFileIDs[externalID].SSMID;
            SSMBitfield1 = (uint)fd.SSMFileIDs[externalID].BitField1;
            SSMBitfield2 = (uint)fd.SSMFileIDs[externalID].BitField2;

            SubCharacterInternalID = (sbyte)fd.DefineIDs[externalID].SubCharacterInternalID;
            SubCharacterBehavior = fd.DefineIDs[externalID].SubCharacterBehavior;

            KirbyCapFileName = mexData.KirbyData.CapFiles[internalId].FileName;
            KirbyCapSymbol = mexData.KirbyData.CapFiles[internalId].Symbol;
            KirbySpecialCostumes = mexData.KirbyData.KirbyCostumes[internalId]?.Array;
            KirbyEffectID = mexData.KirbyData.KirbyEffectIDs[internalId].Value;
            //KirbyNSpecialStateIndex = mexData.KirbyData.KirbyNState[internalId].Value;
            //KirbyNSpecialAirStateIndex = mexData.KirbyData.KirbyNStateAir[internalId].Value;

            if (!IsSpecialCharacterInternal(mexData, internalId))
            {
                DemoResult = fd.FtDemo_SymbolNames.Array[internalId].Result;
                DemoIntro = fd.FtDemo_SymbolNames.Array[internalId].Intro;
                DemoEnding = fd.FtDemo_SymbolNames.Array[internalId].Ending;
                DemoWait = fd.FtDemo_SymbolNames.Array[internalId].ViWait;
            }

            VictoryThemeID = fd.VictoryThemeIDs[externalID].Value;
            FighterSongID1 = fd.FighterSongIDs[externalID].SongID1;
            FighterSongID2 = fd.FighterSongIDs[externalID].SongID2;

            if (!IsSpecialCharacterExternal(mexData, externalID))
            {
                RedCostumeID = fd.CostumeIDs[externalID].RedCostumeIndex;
                GreenCostumeID = fd.CostumeIDs[externalID].GreenCostumeIndex;
                BlueCostumeID = fd.CostumeIDs[externalID].BlueCostumeIndex;

                ResultScreenScale = fd.ResultScale[externalID].Value;
                TargetTestStage = fd.TargetTestStageLookups[externalID].Value;
                RacetoTheFinishTime = fd.RaceToFinishTimeLimits[externalID].Value;
                DemoFile = fd.VIFiles[externalID].Value;

                EndClassicFile = fd.EndClassicFiles[externalID].Value;
                EndAdventureFile = fd.EndAdventureFiles[externalID].Value;
                EndAllStarFile = fd.EndAllStarFiles[externalID].Value;
                EndMovieFile = fd.EndMovieFiles[externalID].Value;
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mexData"></param>
        /// <param name="internalId"></param>
        /// <param name="externalID"></param>
        public void SaveData(MEX_Data mexData, int internalId, int externalID)
        {
            var fd = mexData.FighterData;

            Functions.SaveData(mexData, internalId, externalID);

            fd.NameText.Set(externalID, new HSD_String() { Value = NameText });
            fd.CharFiles.Set(internalId, new MEX_CharFileStrings() { FileName = FighterDataPath, Symbol = FighterDataSymbol });
            fd.AnimFiles.Set(internalId, new HSD_String() { Value = AnimFile });
            fd.AnimCount.Set(internalId, new MEX_AnimCount() { AnimCount = AnimCount });
            fd.RstRuntime.Set(internalId, new MEX_RstRuntime() { AnimMax = RstAnimCount });

            fd.FighterItemLookup.Set(internalId, new MEX_ItemLookup() { Entries = MEXItems });
            //fd.FighterEffectLookup.Set(internalId, new MEX_EffectTypeLookup() { Entries = MEXEffects });

            fd.InsigniaIDs.Set(externalID, new HSD_Byte() { Value = InsigniaID });

            fd.WallJump.Set(internalId, new HSD_Byte() { Value = CanWallJump ? (byte)1 : (byte)0 });

            fd.CostumeFileSymbols.Set(internalId, new MEX_CostumeFileSymbolTable() { CostumeSymbols = new HSDRaw.HSDArrayAccessor<MEX_CostumeFileSymbol>() { Array = Costumes } });

            fd.EffectIDs.Set(internalId, new HSD_Byte() { Value = (byte)EffectIndex });
            fd.AnnouncerCalls.Set(externalID, new HSD_Int() { Value = AnnouncerCall });

            // Saving Kirby Elements
            if (!string.IsNullOrEmpty(KirbyCapFileName))
                mexData.KirbyData.CapFiles.Set(internalId, new MEX_KirbyCapFiles()
                {
                    FileName = KirbyCapFileName,
                    Symbol = KirbyCapSymbol
                });
            else
                mexData.KirbyData.CapFiles.Set(internalId, new MEX_KirbyCapFiles());

            if (KirbySpecialCostumes != null)
            {
                mexData.KirbyData.KirbyCostumes.Set(internalId, new MEX_KirbyCostume() { Array = KirbySpecialCostumes });
                mexData.KirbyData.CostumeRuntime._s.SetReferenceStruct(internalId * 4, new HSDStruct(GenerateSpecialBuffer(0x30, NameText)));
            }
            else
                mexData.KirbyData.KirbyCostumes.Set(internalId, null);

            mexData.KirbyData.KirbyEffectIDs.Set(internalId, new HSD_Byte() { Value = (byte)KirbyEffectID });
            //mexData.KirbyData.KirbyNState.Set(internalId, new HSD_UShort() { Value = KirbyNSpecialStateIndex });
            //mexData.KirbyData.KirbyNStateAir.Set(internalId, new HSD_UShort() { Value = KirbyNSpecialAirStateIndex });

            fd.CostumePointers.Set(internalId, new MEX_CostumeRuntimePointers()
            {
                CostumeCount = CostumeCount,
                Pointer = new HSDRaw.HSDAccessor() { _s = new HSDRaw.HSDStruct(0x18 * CostumeCount) }
            });

            fd.SSMFileIDs.Set(externalID, new MEX_CharSSMFileID()
            {
                SSMID = (byte)SSMIndex,
                BitField1 = (int)SSMBitfield1,
                BitField2 = (int)SSMBitfield2
            });

            fd.DefineIDs.Set(externalID, new MEX_CharDefineIDs()
            {
                InternalID = (byte)(internalId + (internalId == 11 ? -1 : 0)), // popo id reference ice climbers
                SubCharacterInternalID = (byte)SubCharacterInternalID,
                SubCharacterBehavior = SubCharacterBehavior
            });

            if (!IsSpecialCharacterInternal(mexData, internalId))
            {
                if (DemoResult == null)
                {
                    DemoResult = "";
                    DemoIntro = "";
                    DemoEnding = "";
                    DemoWait = "";
                }
                fd.FtDemo_SymbolNames.Set(internalId, new MEX_FtDemoSymbolNames()
                {
                    Result = DemoResult,
                    Intro = DemoIntro,
                    Ending = DemoEnding,
                    ViWait = DemoWait
                });
            }
            else
            {
                fd.FtDemo_SymbolNames.Set(internalId, null);
            }

            fd.ResultAnimFiles.Set(externalID, new HSD_String() { Value = RstAnimFile });

            fd.VictoryThemeIDs.Set(externalID, new HSD_Int() { Value = VictoryThemeID });
            fd.FighterSongIDs.Set(externalID, new MEX_FighterSongID()
            {
                SongID1 = (short)FighterSongID1,
                SongID2 = (short)FighterSongID2
            }
            );

            if (!IsSpecialCharacterExternal(mexData, externalID))
            {
                fd.CostumeIDs.Set(externalID, new MEX_CostumeIDs()
                {
                    CostumeCount = CostumeCount,
                    RedCostumeIndex = RedCostumeID,
                    GreenCostumeIndex = GreenCostumeID,
                    BlueCostumeIndex = BlueCostumeID
                });

                fd.ResultScale.Set(externalID, new HSD_Float() { Value = ResultScreenScale });
                fd.TargetTestStageLookups.Set(externalID, new HSD_UShort() { Value = (ushort)TargetTestStage });
                fd.RaceToFinishTimeLimits.Set(externalID, new HSD_Int() { Value = RacetoTheFinishTime });
                fd.VIFiles.Set(externalID, new HSD_String() { Value = DemoFile });

                fd.EndClassicFiles.Set(externalID, new HSD_String() { Value = EndClassicFile });
                fd.EndAllStarFiles.Set(externalID, new HSD_String() { Value = EndAllStarFile });
                fd.EndAdventureFiles.Set(externalID, new HSD_String() { Value = EndAdventureFile });
                fd.EndMovieFiles.Set(externalID, new HSD_String() { Value = EndMovieFile });
            }

        }

        public override string ToString()
        {
            return NameText;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MEXFunctionPointers
    {
        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnLoad { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnDeath { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnUnk { get; set; }

        [Category("Fighter")]
        public MEX_MoveLogic[] MoveLogic { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint MoveLogicPointer { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint DemoMoveLogicPointer { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint SpecialN { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint SpecialNAir { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint SpecialHi { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint SpecialHiAir { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint SpecialLw { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint SpecialLwAir { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint SpecialS { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint SpecialSAir { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint SmashUp { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint SmashDown { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint SmashSide { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnAbsorb { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnItemPickup { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnMakeItemInvisible { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnMakeItemVisible { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnItemDrop { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnItemCatch { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnUnknownItemRelated { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnUnknownCharacterFlags1 { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnUnknownCharacterFlags2 { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnHit { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnUnknownEyeTextureRelated { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnFrame { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnActionStateChange { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnRespawn { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnModelRender { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnShadowRender { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnUnknownMultijump { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnActionStateChangeWhileEyeTextureIsChanged { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnTwoEntryTable1 { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnTwoEntryTable2 { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnLand { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnExtRstAnim { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnIndexExtRstAnim { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint EnterFloat { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint EnterDoubleJump { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint EnterTether { get; set; }

        [TypeConverter(typeof(HexType)), DisplayName("Kirby N Special"), Category("Kirby"), Description("")]
        public uint KirbySpecialN { get; set; }

        [TypeConverter(typeof(HexType)), DisplayName("Kirby N Air Special"), Category("Kirby"), Description("")]
        public uint KirbySpecialNAir { get; set; }

        [TypeConverter(typeof(HexType)), DisplayName("Kirby OnSwallow"), Category("Kirby"), Description("")]
        public uint KirbyOnSwallow { get; set; }

        [TypeConverter(typeof(HexType)), DisplayName("Kirby OnLoseAbility"), Category("Kirby"), Description("")]
        public uint KirbyOnLoseAbility { get; set; }

        [TypeConverter(typeof(HexType)), DisplayName("Kirby OnHit"), Category("Kirby"), Description("")]
        public uint KirbyOnHit { get; set; }

        [TypeConverter(typeof(HexType)), DisplayName("Kirby OnItemInit"), Category("Kirby"), Description("")]
        public uint KirbyOnItemInit { get; set; }

        public MEXFunctionPointers LoadData(MEX_Data mexData, int internalId, int externalID)
        {
            var ff = mexData.FighterFunctions;
            var kf = mexData.KirbyFunctions;

            OnLoad = ff.OnLoad[internalId].Value;
            OnDeath = ff.OnDeath[internalId].Value;
            OnUnk = ff.OnUnknown[internalId].Value;

            if (mexData.MetaData.Flags.HasFlag(MexFlags.ContainMoveLogic))
                MoveLogic = ff.MoveLogic.Array[internalId]?.Array;

            if (!mexData.MetaData.Flags.HasFlag(MexFlags.ContainMoveLogic))
                MoveLogicPointer = ff.MoveLogicPointers[internalId].Value;

            DemoMoveLogicPointer = ff.DemoMoveLogic[internalId].Value;

            SpecialN = ff.SpecialN[internalId].Value;
            SpecialNAir = ff.SpecialNAir[internalId].Value;
            SpecialHi = ff.SpecialHi[internalId].Value;
            SpecialHiAir = ff.SpecialHiAir[internalId].Value;
            SpecialLw = ff.SpecialLw[internalId].Value;
            SpecialLwAir = ff.SpecialLwAir[internalId].Value;
            SpecialS = ff.SpecialS[internalId].Value;
            SpecialSAir = ff.SpecialSAir[internalId].Value;
            OnAbsorb = ff.OnAbsorb[internalId].Value;
            OnItemPickup = ff.onItemPickup[internalId].Value;

            OnMakeItemInvisible = ff.onMakeItemInvisible[internalId].Value;
            OnMakeItemVisible = ff.onMakeItemVisible[internalId].Value;
            OnItemDrop = ff.onItemDrop[internalId].Value;
            OnItemCatch = ff.onItemCatch[internalId].Value;
            OnUnknownItemRelated = ff.onUnknownItemRelated[internalId].Value;
            OnUnknownCharacterFlags1 = ff.onUnknownCharacterModelFlags1[internalId].Value;
            OnUnknownCharacterFlags2 = ff.onUnknownCharacterModelFlags2[internalId].Value;
            OnHit = ff.onHit[internalId].Value;
            OnUnknownEyeTextureRelated = ff.onUnknownEyeTextureRelated[internalId].Value;
            OnFrame = ff.onFrame[internalId].Value;
            OnActionStateChange = ff.onActionStateChange[internalId].Value;
            OnRespawn = ff.onRespawn[internalId].Value;
            OnModelRender = ff.onModelRender[internalId].Value;
            OnShadowRender = ff.onShadowRender[internalId].Value;
            OnUnknownMultijump = ff.onUnknownMultijump[internalId].Value;
            OnActionStateChangeWhileEyeTextureIsChanged = ff.onActionStateChangeWhileEyeTextureIsChanged[internalId].Value;
            OnTwoEntryTable1 = ff.onTwoEntryTable[internalId * 2].Value;
            OnTwoEntryTable2 = ff.onTwoEntryTable[internalId * 2 + 1].Value;
            OnLand = ff.onLand[internalId].Value;
            OnExtRstAnim = ff.onExtRstAnim[internalId].Value;
            OnIndexExtRstAnim = ff.onIndexExtResultAnim[internalId].Value;

            SmashDown = ff.onSmashDown[internalId].Value;
            SmashUp = ff.onSmashUp[internalId].Value;
            SmashSide = ff.onSmashForward[internalId].Value;

            EnterFloat = ff.enterFloat[internalId].Value;
            EnterDoubleJump = ff.enterSpecialDoubleJump[internalId].Value;
            EnterTether = ff.enterTether[internalId].Value;

            KirbyOnSwallow = kf.OnAbilityGain[internalId].Value;
            KirbyOnLoseAbility = kf.OnAbilityLose[internalId].Value;
            KirbySpecialN = kf.KirbySpecialN[internalId].Value;
            KirbySpecialNAir = kf.KirbySpecialNAir[internalId].Value;
            KirbyOnHit = kf.KirbyOnHit[internalId].Value;
            KirbyOnItemInit = kf.KirbyOnItemInit[internalId].Value;

            return this;
        }

        public void SaveData(MEX_Data mexData, int internalId, int externalID)
        {
            var ff = mexData.FighterFunctions;
            var kf = mexData.KirbyFunctions;

            ff.OnLoad.Set(internalId, new HSD_UInt() { Value = OnLoad });
            ff.OnDeath.Set(internalId, new HSD_UInt() { Value = OnDeath });
            ff.OnUnknown.Set(internalId, new HSD_UInt() { Value = OnUnk });

            if (mexData.MetaData.Flags.HasFlag(MexFlags.ContainMoveLogic) && MoveLogic != null)
                ff.MoveLogic.Set(internalId, new HSDArrayAccessor<MEX_MoveLogic>() { Array = MoveLogic });

            if (!mexData.MetaData.Flags.HasFlag(MexFlags.ContainMoveLogic))
                ff.MoveLogicPointers.Set(internalId, new HSD_UInt() { Value = MoveLogicPointer });

            ff.DemoMoveLogic.Set(internalId, new HSD_UInt() { Value = DemoMoveLogicPointer });

            ff.SpecialN.Set(internalId, new HSD_UInt() { Value = SpecialN });
            ff.SpecialNAir.Set(internalId, new HSD_UInt() { Value = SpecialNAir });
            ff.SpecialHi.Set(internalId, new HSD_UInt() { Value = SpecialHi });
            ff.SpecialHiAir.Set(internalId, new HSD_UInt() { Value = SpecialHiAir });
            ff.SpecialLw.Set(internalId, new HSD_UInt() { Value = SpecialLw });
            ff.SpecialLwAir.Set(internalId, new HSD_UInt() { Value = SpecialLwAir });
            ff.SpecialS.Set(internalId, new HSD_UInt() { Value = SpecialS });
            ff.SpecialSAir.Set(internalId, new HSD_UInt() { Value = SpecialSAir });
            ff.OnAbsorb.Set(internalId, new HSD_UInt() { Value = OnAbsorb });
            ff.onItemPickup.Set(internalId, new HSD_UInt() { Value = OnItemPickup });
            ff.onMakeItemInvisible.Set(internalId, new HSD_UInt() { Value = OnMakeItemInvisible });
            ff.onMakeItemVisible.Set(internalId, new HSD_UInt() { Value = OnMakeItemVisible });
            ff.onItemDrop.Set(internalId, new HSD_UInt() { Value = OnItemDrop });
            ff.onItemCatch.Set(internalId, new HSD_UInt() { Value = OnItemCatch });
            ff.onUnknownItemRelated.Set(internalId, new HSD_UInt() { Value = OnUnknownItemRelated });
            ff.onUnknownCharacterModelFlags1.Set(internalId, new HSD_UInt() { Value = OnUnknownCharacterFlags1 });
            ff.onUnknownCharacterModelFlags2.Set(internalId, new HSD_UInt() { Value = OnUnknownCharacterFlags2 });
            ff.onHit.Set(internalId, new HSD_UInt() { Value = OnHit });
            ff.onUnknownEyeTextureRelated.Set(internalId, new HSD_UInt() { Value = OnUnknownEyeTextureRelated });
            ff.onFrame.Set(internalId, new HSD_UInt() { Value = OnFrame });
            ff.onActionStateChange.Set(internalId, new HSD_UInt() { Value = OnActionStateChange });
            ff.onRespawn.Set(internalId, new HSD_UInt() { Value = OnRespawn });
            ff.onModelRender.Set(internalId, new HSD_UInt() { Value = OnModelRender });
            ff.onShadowRender.Set(internalId, new HSD_UInt() { Value = OnShadowRender });
            ff.onUnknownMultijump.Set(internalId, new HSD_UInt() { Value = OnUnknownMultijump });
            ff.onActionStateChangeWhileEyeTextureIsChanged.Set(internalId, new HSD_UInt() { Value = OnActionStateChangeWhileEyeTextureIsChanged });
            ff.onTwoEntryTable.Set(internalId * 2, new HSD_UInt() { Value = OnTwoEntryTable1 });
            ff.onTwoEntryTable.Set(internalId * 2 + 1, new HSD_UInt() { Value = OnTwoEntryTable2 });
            ff.onLand.Set(internalId, new HSD_UInt() { Value = OnLand });
            ff.onExtRstAnim.Set(internalId, new HSD_UInt() { Value = OnExtRstAnim });
            ff.onIndexExtResultAnim.Set(internalId, new HSD_UInt() { Value = OnIndexExtRstAnim });

            ff.onSmashDown.Set(internalId, new HSD_UInt() { Value = SmashDown });
            ff.onSmashUp.Set(internalId, new HSD_UInt() { Value = SmashUp });
            ff.onSmashForward.Set(internalId, new HSD_UInt() { Value = SmashSide });

            ff.enterFloat.Set(internalId, new HSD_UInt() { Value = EnterFloat });
            ff.enterSpecialDoubleJump.Set(internalId, new HSD_UInt() { Value = EnterDoubleJump });
            ff.enterTether.Set(internalId, new HSD_UInt() { Value = EnterTether });

            kf.OnAbilityGain.Set(internalId, new HSD_UInt() { Value = KirbyOnSwallow });
            kf.OnAbilityLose.Set(internalId, new HSD_UInt() { Value = KirbyOnLoseAbility });
            kf.KirbySpecialN.Set(internalId, new HSD_UInt() { Value = KirbySpecialN });
            kf.KirbySpecialNAir.Set(internalId, new HSD_UInt() { Value = KirbySpecialNAir });
            kf.KirbyOnHit.Set(internalId, new HSD_UInt() { Value = KirbyOnHit });
            kf.KirbyOnItemInit.Set(internalId, new HSD_UInt() { Value = KirbyOnItemInit });
        }
    }
}
