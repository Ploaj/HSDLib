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

        [DisplayName("MEX Effects"), Category("0 - General"), Description("MEX Effect lookup for Fighter")]
        public MEXEffectType[] MEXEffects { get; set; }

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

        [DisplayName("Result"), Category("2 - Demo"), Description("")]
        public string DemoResult { get; set; }

        [DisplayName("Intro"), Category("2 - Demo"), Description("")]
        public string DemoIntro { get; set; }

        [DisplayName("Ending"), Category("2 - Demo"), Description("")]
        public string DemoEnding { get; set; }

        [DisplayName("Vi Wait"), Category("2 - Demo"), Description("")]
        public string DemoWait { get; set; }

        [DisplayName("Target Test Stage"), Category("3 - Misc"), Description("")]
        public int TargetTestStage { get; set; }

        [DisplayName("Result Screen Scale"), Category("3 - Misc"), Description("")]
        public float ResultScreenScale { get; set; }

        [DisplayName("Can Wall Jump"), Category("3 - Misc"), Description("")]
        public bool CanWallJump { get; set; }

        [DisplayName("Insignia ID"), Category("3 - Misc"), Description("")]
        public byte InsigniaID { get; set; }

        [DisplayName("Victory Theme"), Category("3 - Misc"), Description(""), TypeConverter(typeof(MusicIDConverter))]
        public int VictoryThemeID { get; set; }

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


        [DisplayName("Kirby Cap FileName"), Category("4 - Kirby"), Description("")]
        public string KirbyCapFileName { get; set; }

        [DisplayName("Kirby Cap Symbol"), Category("4 - Kirby"), Description("")]
        public string KirbyCapSymbol { get; set; }

        [DisplayName("Kirby Costumes"), Category("4 - Kirby"), Description("")]
        public MEX_CostumeFileSymbol[] KirbySpecialCostumes { get; set; }

        [DisplayName("Kirby Effect ID"), Category("4 - Kirby"), Description(""), TypeConverter(typeof(EffectIDConverter))]
        public int KirbyEffectID { get; set; }


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
            Functions.LoadData(mexData, internalId, externalID);

            NameText = mexData.FighterData.NameText[externalID].Value;
            FighterDataPath = mexData.FighterData.CharFiles[internalId].FileName;
            FighterDataSymbol = mexData.FighterData.CharFiles[internalId].Symbol;
            AnimFile = mexData.FighterData.AnimFiles[internalId].Value;
            AnimCount = mexData.FighterData.AnimCount[internalId].AnimCount;
            RstAnimCount = mexData.FighterData.RstRuntime[internalId].AnimMax;

            MEXItems = mexData.FighterData.FighterItemLookup[internalId].Entries;
            MEXEffects = mexData.FighterData.FighterEffectLookup[internalId].Entries;

            InsigniaID = mexData.FighterData.InsigniaIDs[externalID].Value;

            CanWallJump = mexData.FighterData.WallJump[internalId].Value != 0;
            
            Costumes = mexData.FighterData.CostumeFileSymbols[internalId].CostumeSymbols.Array;

            RstAnimFile = mexData.FighterData.ResultAnimFiles[externalID].Value;

            EffectIndex = mexData.FighterData.EffectIDs[internalId].Value;
            AnnouncerCall = mexData.FighterData.AnnouncerCalls[externalID].Value;

            SSMIndex = mexData.FighterData.SSMFileIDs[externalID].SSMID;
            SSMBitfield1 = (uint)mexData.FighterData.SSMFileIDs[externalID].BitField1;
            SSMBitfield2 = (uint)mexData.FighterData.SSMFileIDs[externalID].BitField2;

            SubCharacterInternalID = (sbyte)mexData.FighterData.DefineIDs[externalID].SubCharacterInternalID;
            SubCharacterBehavior = mexData.FighterData.DefineIDs[externalID].SubCharacterBehavior;

            KirbyCapFileName = mexData.KirbyData.CapFiles[internalId].FileName;
            KirbyCapSymbol = mexData.KirbyData.CapFiles[internalId].Symbol;
            KirbySpecialCostumes = mexData.KirbyData.KirbyCostumes[internalId]?.Array;
            KirbyEffectID = mexData.KirbyData.EffectIDs[internalId].Value;

            if (!IsSpecialCharacterInternal(mexData, internalId))
            {
                DemoResult = mexData.FighterData.FtDemo_SymbolNames.Array[internalId].Result;
                DemoIntro = mexData.FighterData.FtDemo_SymbolNames.Array[internalId].Intro;
                DemoEnding = mexData.FighterData.FtDemo_SymbolNames.Array[internalId].Ending;
                DemoWait = mexData.FighterData.FtDemo_SymbolNames.Array[internalId].ViWait;
            }

            if (!IsSpecialCharacterExternal(mexData, externalID))
            {
                RedCostumeID = mexData.FighterData.CostumeIDs[externalID].RedCostumeIndex;
                GreenCostumeID = mexData.FighterData.CostumeIDs[externalID].GreenCostumeIndex;
                BlueCostumeID = mexData.FighterData.CostumeIDs[externalID].BlueCostumeIndex;

                ResultScreenScale = mexData.FighterData.ResultScale[externalID].Value;
                VictoryThemeID = mexData.FighterData.VictoryThemeIDs[externalID].Value;
                TargetTestStage = mexData.FighterData.TargetTestStageLookups[externalID].Value;
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
            Functions.SaveData(mexData, internalId, externalID);

            mexData.FighterData.NameText.Set(externalID, new HSD_String() { Value = NameText });
            mexData.FighterData.CharFiles.Set(internalId, new MEX_CharFileStrings() { FileName = FighterDataPath, Symbol = FighterDataSymbol});
            mexData.FighterData.AnimFiles.Set(internalId, new HSD_String() { Value = AnimFile });
            mexData.FighterData.AnimCount.Set(internalId, new MEX_AnimCount() { AnimCount = AnimCount });
            mexData.FighterData.RstRuntime.Set(internalId, new MEX_RstRuntime() { AnimMax = RstAnimCount });

            mexData.FighterData.FighterItemLookup.Set(internalId, new MEX_ItemLookup() { Entries = MEXItems });
            mexData.FighterData.FighterEffectLookup.Set(internalId, new MEX_EffectTypeLookup() { Entries = MEXEffects });

            mexData.FighterData.InsigniaIDs.Set(externalID, new HSD_Byte() { Value = InsigniaID });

            mexData.FighterData.WallJump.Set(internalId, new HSD_Byte() { Value = CanWallJump ? (byte)1 : (byte)0 });

            mexData.FighterData.CostumeFileSymbols.Set(internalId, new MEX_CostumeFileSymbolTable() { CostumeSymbols = new HSDRaw.HSDArrayAccessor<MEX_CostumeFileSymbol>() { Array = Costumes } });

            mexData.FighterData.EffectIDs.Set(internalId, new HSD_Byte() { Value = (byte)EffectIndex });
            mexData.FighterData.AnnouncerCalls.Set(externalID, new HSD_Int() { Value = AnnouncerCall });
            
            // Saving Kirby Elements
            if (!string.IsNullOrEmpty(KirbyCapFileName))
                mexData.KirbyData.CapFiles.Set(internalId, new MEX_KirbyCapFiles()
                {
                    FileName = KirbyCapFileName,
                    Symbol = KirbyCapSymbol
                });
            else
                mexData.KirbyData.CapFiles.Set(internalId, new MEX_KirbyCapFiles());

            if(KirbySpecialCostumes != null)
            {
                mexData.KirbyData.KirbyCostumes.Set(internalId, new MEX_KirbyCostume() { Array = KirbySpecialCostumes });
                mexData.KirbyData.CostumeRuntime._s.SetReferenceStruct(internalId * 4, new HSDStruct(GenerateSpecialBuffer(0x30, NameText)));
            }
            else
                mexData.KirbyData.KirbyCostumes.Set(internalId, null);
            
            mexData.KirbyData.EffectIDs.Set(internalId, new HSD_Byte() { Value = (byte)KirbyEffectID });

            mexData.FighterData.CostumePointers.Set(internalId, new MEX_CostumeRuntimePointers()
            {
                CostumeCount = CostumeCount,
                Pointer = new HSDRaw.HSDAccessor() { _s = new HSDRaw.HSDStruct(4 * 6) }
            });

            mexData.FighterData.SSMFileIDs.Set(externalID, new MEX_CharSSMFileID()
            {
                SSMID = (byte)SSMIndex,
                BitField1 = (int)SSMBitfield1,
                BitField2 = (int)SSMBitfield2
            });

            mexData.FighterData.DefineIDs.Set(externalID, new MEX_CharDefineIDs()
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
                mexData.FighterData.FtDemo_SymbolNames.Set(internalId, new MEX_FtDemoSymbolNames()
                {
                    Result = DemoResult,
                    Intro = DemoIntro,
                    Ending = DemoEnding,
                    ViWait = DemoWait
                });
            }
            else
            {
                mexData.FighterData.FtDemo_SymbolNames.Set(internalId, null);
            }
            
            mexData.FighterData.ResultAnimFiles.Set(externalID, new HSD_String() { Value = RstAnimFile });
            
            if (!IsSpecialCharacterExternal(mexData, externalID))
            {
                mexData.FighterData.CostumeIDs.Set(externalID, new MEX_CostumeIDs()
                {
                    CostumeCount = CostumeCount,
                    RedCostumeIndex = RedCostumeID,
                    GreenCostumeIndex = GreenCostumeID,
                    BlueCostumeIndex = BlueCostumeID
                });

                mexData.FighterData.ResultScale.Set(externalID, new HSD_Float() { Value = ResultScreenScale });
                mexData.FighterData.VictoryThemeIDs.Set(externalID, new HSD_Int() { Value = VictoryThemeID });
                mexData.FighterData.TargetTestStageLookups.Set(externalID, new HSD_UShort() { Value = (ushort)TargetTestStage });
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
        public uint OnTwoEntryTable { get; set; }

        [TypeConverter(typeof(HexType)), Category("Fighter")]
        public uint OnLand { get; set; }

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
            OnLoad = mexData.FighterFunctions.OnLoad[internalId].Value;
            OnDeath = mexData.FighterFunctions.OnDeath[internalId].Value;
            OnUnk = mexData.FighterFunctions.OnUnknown[internalId].Value;
            MoveLogic = mexData.FighterFunctions.MoveLogic.Array[internalId].Array;
            SpecialN = mexData.FighterFunctions.SpecialN[internalId].Value;
            SpecialNAir = mexData.FighterFunctions.SpecialNAir[internalId].Value;
            SpecialHi = mexData.FighterFunctions.SpecialHi[internalId].Value;
            SpecialHiAir = mexData.FighterFunctions.SpecialHiAir[internalId].Value;
            SpecialLw = mexData.FighterFunctions.SpecialLw[internalId].Value;
            SpecialLwAir = mexData.FighterFunctions.SpecialLwAir[internalId].Value;
            SpecialS = mexData.FighterFunctions.SpecialS[internalId].Value;
            SpecialSAir = mexData.FighterFunctions.SpecialSAir[internalId].Value;
            OnAbsorb = mexData.FighterFunctions.OnAbsorb[internalId].Value;
            OnItemPickup = mexData.FighterFunctions.onItemPickup[internalId].Value;

            OnMakeItemInvisible = mexData.FighterFunctions.onMakeItemInvisible[internalId].Value;
            OnMakeItemVisible = mexData.FighterFunctions.onMakeItemVisible[internalId].Value;
            OnItemDrop = mexData.FighterFunctions.onItemDrop[internalId].Value;
            OnItemCatch = mexData.FighterFunctions.onItemCatch[internalId].Value;
            OnUnknownItemRelated = mexData.FighterFunctions.onUnknownItemRelated[internalId].Value;
            OnUnknownCharacterFlags1 = mexData.FighterFunctions.onUnknownCharacterModelFlags1[internalId].Value;
            OnUnknownCharacterFlags2 = mexData.FighterFunctions.onUnknownCharacterModelFlags2[internalId].Value;
            OnHit = mexData.FighterFunctions.onHit[internalId].Value;
            OnUnknownEyeTextureRelated = mexData.FighterFunctions.onUnknownEyeTextureRelated[internalId].Value;
            OnFrame = mexData.FighterFunctions.onFrame[internalId].Value;
            OnActionStateChange = mexData.FighterFunctions.onActionStateChange[internalId].Value;
            OnRespawn = mexData.FighterFunctions.onRespawn[internalId].Value;
            OnModelRender = mexData.FighterFunctions.onModelRender[internalId].Value;
            OnShadowRender = mexData.FighterFunctions.onShadowRender[internalId].Value;
            OnUnknownMultijump = mexData.FighterFunctions.onUnknownMultijump[internalId].Value;
            OnActionStateChangeWhileEyeTextureIsChanged = mexData.FighterFunctions.onActionStateChangeWhileEyeTextureIsChanged[internalId].Value;
            OnTwoEntryTable = mexData.FighterFunctions.onTwoEntryTable[internalId].Value;
            OnLand = mexData.FighterFunctions.onLand[internalId].Value;

            SmashDown = mexData.FighterFunctions.onSmashDown[internalId].Value;
            SmashUp = mexData.FighterFunctions.onSmashUp[internalId].Value;
            SmashSide = mexData.FighterFunctions.onSmashForward[internalId].Value;

            EnterFloat = mexData.FighterFunctions.enterFloat[internalId].Value;
            EnterDoubleJump = mexData.FighterFunctions.enterSpecialDoubleJump[internalId].Value;
            EnterTether = mexData.FighterFunctions.enterTether[internalId].Value;

            KirbyOnSwallow = mexData.KirbyFunctions.OnAbilityGain[internalId].Value;
            KirbyOnLoseAbility = mexData.KirbyFunctions.OnAbilityLose[internalId].Value;
            KirbySpecialN = mexData.KirbyFunctions.KirbySpecialN[internalId].Value;
            KirbySpecialNAir = mexData.KirbyFunctions.KirbySpecialNAir[internalId].Value;
            KirbyOnHit = mexData.KirbyFunctions.KirbyOnHit[internalId].Value;
            KirbyOnItemInit = mexData.KirbyFunctions.KirbyOnItemInit[internalId].Value;

            return this;
        }

        public void SaveData(MEX_Data mexData, int internalId, int externalID)
        {
            mexData.FighterFunctions.OnLoad.Set(internalId, new HSD_UInt() { Value = OnLoad });
            mexData.FighterFunctions.OnDeath.Set(internalId, new HSD_UInt() { Value = OnDeath });
            mexData.FighterFunctions.OnUnknown.Set(internalId, new HSD_UInt() { Value = OnUnk });
            mexData.FighterFunctions.MoveLogic.Set(internalId, new HSDRaw.HSDArrayAccessor<MEX_MoveLogic>() { Array = MoveLogic });
            mexData.FighterFunctions.SpecialN.Set(internalId, new HSD_UInt() { Value = SpecialN });
            mexData.FighterFunctions.SpecialNAir.Set(internalId, new HSD_UInt() { Value = SpecialNAir });
            mexData.FighterFunctions.SpecialHi.Set(internalId, new HSD_UInt() { Value = SpecialHi });
            mexData.FighterFunctions.SpecialHiAir.Set(internalId, new HSD_UInt() { Value = SpecialHiAir });
            mexData.FighterFunctions.SpecialLw.Set(internalId, new HSD_UInt() { Value = SpecialLw });
            mexData.FighterFunctions.SpecialLwAir.Set(internalId, new HSD_UInt() { Value = SpecialLwAir });
            mexData.FighterFunctions.SpecialS.Set(internalId, new HSD_UInt() { Value = SpecialS });
            mexData.FighterFunctions.SpecialSAir.Set(internalId, new HSD_UInt() { Value = SpecialSAir });
            mexData.FighterFunctions.OnAbsorb.Set(internalId, new HSD_UInt() { Value = OnAbsorb });
            mexData.FighterFunctions.onItemPickup.Set(internalId, new HSD_UInt() { Value = OnItemPickup });
            mexData.FighterFunctions.onMakeItemInvisible.Set(internalId, new HSD_UInt() { Value = OnMakeItemInvisible });
            mexData.FighterFunctions.onMakeItemVisible.Set(internalId, new HSD_UInt() { Value = OnMakeItemVisible });
            mexData.FighterFunctions.onItemDrop.Set(internalId, new HSD_UInt() { Value = OnItemDrop });
            mexData.FighterFunctions.onItemCatch.Set(internalId, new HSD_UInt() { Value = OnItemCatch });
            mexData.FighterFunctions.onUnknownItemRelated.Set(internalId, new HSD_UInt() { Value = OnUnknownItemRelated });
            mexData.FighterFunctions.onUnknownCharacterModelFlags1.Set(internalId, new HSD_UInt() { Value = OnUnknownCharacterFlags1 });
            mexData.FighterFunctions.onUnknownCharacterModelFlags2.Set(internalId, new HSD_UInt() { Value = OnUnknownCharacterFlags2 });
            mexData.FighterFunctions.onHit.Set(internalId, new HSD_UInt() { Value = OnHit });
            mexData.FighterFunctions.onUnknownEyeTextureRelated.Set(internalId, new HSD_UInt() { Value = OnUnknownEyeTextureRelated });
            mexData.FighterFunctions.onFrame.Set(internalId, new HSD_UInt() { Value = OnFrame });
            mexData.FighterFunctions.onActionStateChange.Set(internalId, new HSD_UInt() { Value = OnActionStateChange });
            mexData.FighterFunctions.onRespawn.Set(internalId, new HSD_UInt() { Value = OnRespawn });
            mexData.FighterFunctions.onModelRender.Set(internalId, new HSD_UInt() { Value = OnModelRender });
            mexData.FighterFunctions.onShadowRender.Set(internalId, new HSD_UInt() { Value = OnShadowRender });
            mexData.FighterFunctions.onUnknownMultijump.Set(internalId, new HSD_UInt() { Value = OnUnknownMultijump });
            mexData.FighterFunctions.onActionStateChangeWhileEyeTextureIsChanged.Set(internalId, new HSD_UInt() { Value = OnActionStateChangeWhileEyeTextureIsChanged });
            mexData.FighterFunctions.onTwoEntryTable.Set(internalId, new HSD_UInt() { Value = OnTwoEntryTable });
            mexData.FighterFunctions.onLand.Set(internalId, new HSD_UInt() { Value = OnLand });

            mexData.FighterFunctions.onSmashDown.Set(internalId, new HSD_UInt() { Value = SmashDown });
            mexData.FighterFunctions.onSmashUp.Set(internalId, new HSD_UInt() { Value = SmashUp });
            mexData.FighterFunctions.onSmashForward.Set(internalId, new HSD_UInt() { Value = SmashSide });

            mexData.FighterFunctions.enterFloat.Set(internalId, new HSD_UInt() { Value = EnterFloat });
            mexData.FighterFunctions.enterSpecialDoubleJump.Set(internalId, new HSD_UInt() { Value = EnterDoubleJump });
            mexData.FighterFunctions.enterTether.Set(internalId, new HSD_UInt() { Value = EnterTether });

            mexData.KirbyFunctions.OnAbilityGain.Set(internalId, new HSD_UInt() { Value = KirbyOnSwallow });
            mexData.KirbyFunctions.OnAbilityLose.Set(internalId, new HSD_UInt() { Value = KirbyOnLoseAbility });
            mexData.KirbyFunctions.KirbySpecialN.Set(internalId, new HSD_UInt() { Value = KirbySpecialN });
            mexData.KirbyFunctions.KirbySpecialNAir.Set(internalId, new HSD_UInt() { Value = KirbySpecialNAir });
            mexData.KirbyFunctions.KirbyOnHit.Set(internalId, new HSD_UInt() { Value = KirbyOnHit });
            mexData.KirbyFunctions.KirbyOnItemInit.Set(internalId, new HSD_UInt() { Value = KirbyOnItemInit });
        }
    }
}
