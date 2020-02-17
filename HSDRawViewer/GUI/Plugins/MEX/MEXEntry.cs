using HSDRaw.MEX;
using System.ComponentModel;

namespace HSDRawViewer.GUI.Plugins.MEX
{
    public class MEXIdConverter
    {
        public static int InternalExtraCharCount { get; } = 6;

        public static int ExternalExtraCharCount { get; } = 7;

        private static int[] ExternalToInternal = { };

        private static int[] InternalToExternal = {
            0x08, 0x02, 0x00, 0x01, 0x04, 0x05, 0x06,
            0x13, 0x0B, 0x0C, 0x0E, 0x20, 0x0D, 0x10,
            0x11, 0x0F, 0x0A, 0x07, 0x09, 0x12, 0x15,
            0x16, 0x14, 0x18, 0x03, 0x19, 0x17, 0x1A,
            0x1E, 0x1B, 0x1C, 0x1D, 0x1F};

        public static int ToExternalID(int internalID)
        {
            if (internalID < InternalToExternal.Length)
                return InternalToExternal[internalID];
            return internalID;
        }

        // TODO:
        public static int ToInternalID(int externalID)
        {
            return externalID;
        }
    }

    public class MEXEntry
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

        [DisplayName("Result Screen Scale"), Category("3 - Misc"), Description("")]
        public float ResultScreenScale { get; set; }

        [DisplayName("Insignia ID"), Category("3 - Misc"), Description("")]
        public byte InsigniaID { get; set; }

        [DisplayName("Victory Theme ID"), Category("3 - Misc"), Description("")]
        public int VictoryThemeID { get; set; }

        [DisplayName("Effect File ID"), Category("3 - Misc"), Description("Index of Effect(Ef) file for this fighter")]
        public byte EffectIndex { get; set; }

        [DisplayName("SSM ID"), Category("3 - Misc"), Description("Index of SSM file for this fighter")]
        public byte SSMIndex { get; set; }

        [DisplayName("SSM Bitfield 1"), Category("3 - Misc"), Description("")]
        public int SSMBitfield1 { get; set; }

        [DisplayName("SSM Bitfield 2"), Category("3 - Misc"), Description("")]
        public int SSMBitfield2 { get; set; }

        [DisplayName("Narrator Sound Clip"), Category("3 - Misc"), Description("Index of narrator sound clip")]
        public int NameCallSound { get; set; }

        [DisplayName("SubCharacter Internal ID"), Category("3 - Misc"), Description("")]
        public sbyte SubCharacterInternalID { get; set; }

        [DisplayName("SubCharacter Behavior"), Category("3 - Misc"), Description("")]
        public SubCharacterBehavior SubCharacterBehavior { get; set; }

        public MEXFunctionPointers Functions = new MEXFunctionPointers();

        public bool IsExtendedCharacterInternal(MEX_Data mexData, int internalID)
        {
            return internalID >= mexData.MetaData.NumOfInternalIDs - MEXIdConverter.InternalExtraCharCount;
        }

        public bool IsExtendedCharacterExternal(MEX_Data mexData, int externalID)
        {
            return externalID >= mexData.MetaData.NumOfExternalIDs - MEXIdConverter.ExternalExtraCharCount;
        }

        public MEXEntry LoadData(MEX_Data mexData, int internalId, int externalID)
        {
            Functions.LoadData(mexData, internalId, externalID);

            NameText = mexData.MnSlChr_NameText[externalID].Value;
            FighterDataPath = mexData.Char_CharFiles[internalId].FileName;
            FighterDataSymbol = mexData.Char_CharFiles[internalId].Symbol;
            AnimFile = mexData.Char_AnimFiles[internalId].Value;
            AnimCount = mexData.Char_AnimCount[internalId].AnimCount;

            InsigniaID = mexData.Char_InsigniaIDs[externalID].Value;
            
            Costumes = mexData.Char_CostumeFileSymbols[internalId].CostumeSymbols.Array;

            RstAnimFile = mexData.GmRst_AnimFiles[externalID].Value;

            EffectIndex = mexData.Char_EffectIDs[externalID].Value;
            NameCallSound = mexData.SFX_NameDef[externalID].Value;

            SSMIndex = mexData.SSM_CharSSMFileIDs[externalID].SSMID;
            SSMBitfield1 = mexData.SSM_CharSSMFileIDs[externalID].BitField1;
            SSMBitfield2 = mexData.SSM_CharSSMFileIDs[externalID].BitField2;

            SubCharacterInternalID = (sbyte)mexData.Char_DefineIDs[externalID].SubCharacterInternalID;
            SubCharacterBehavior = mexData.Char_DefineIDs[externalID].SubCharacterBehavior;

            if (!IsExtendedCharacterInternal(mexData, internalId))
            {
                DemoResult = mexData.FtDemo_SymbolNames.Array[internalId].Result;
                DemoIntro = mexData.FtDemo_SymbolNames.Array[internalId].Intro;
                DemoEnding = mexData.FtDemo_SymbolNames.Array[internalId].Ending;
                DemoWait = mexData.FtDemo_SymbolNames.Array[internalId].ViWait;
            }

            if (!IsExtendedCharacterExternal(mexData, externalID))
            {
                RedCostumeID = mexData.Char_CostumeIDs[externalID].RedCostumeIndex;
                GreenCostumeID = mexData.Char_CostumeIDs[externalID].GreenCostumeIndex;
                BlueCostumeID = mexData.Char_CostumeIDs[externalID].BlueCostumeIndex;

                ResultScreenScale = mexData.GmRst_Scale[externalID].Value;
                VictoryThemeID = mexData.GmRst_VictoryTheme[externalID].Value;
            }

            return this;
        }

        public void SaveData(MEX_Data mexData, int internalId, int externalID)
        {
            Functions.SaveData(mexData, internalId, externalID);

            mexData.MnSlChr_NameText.Set(externalID, new HSD_String() { Value = NameText });
            mexData.Char_CharFiles.Set(internalId, new MEX_CharFileStrings() { FileName = FighterDataPath, Symbol = FighterDataSymbol});
            mexData.Char_AnimFiles.Set(internalId, new HSD_String() { Value = AnimFile });
            mexData.Char_AnimCount.Set(internalId, new MEX_AnimCount() { AnimCount = AnimCount });
            mexData.Char_InsigniaIDs.Set(externalID, new HSD_Byte() { Value = InsigniaID });

            mexData.Char_CostumeFileSymbols.Set(internalId, new MEX_CostumeFileSymbolTable() { CostumeSymbols = new HSDRaw.HSDArrayAccessor<MEX_CostumeFileSymbol>() { Array = Costumes } });

            mexData.Char_EffectIDs.Set(externalID, new HSD_Byte() { Value = EffectIndex });
            mexData.SFX_NameDef.Set(externalID, new HSD_Int() { Value = NameCallSound });

            mexData.SSM_CharSSMFileIDs.Set(externalID, new MEX_CharSSMFileID()
            {
                SSMID = SSMIndex,
                BitField1 = SSMBitfield1,
                BitField2 = SSMBitfield2
            });

            mexData.Char_DefineIDs.Set(externalID, new MEX_CharDefineIDs()
            {
                InternalID = (byte)internalId,
                SubCharacterInternalID = (byte)SubCharacterInternalID,
                SubCharacterBehavior = SubCharacterBehavior
            });

            if (!IsExtendedCharacterInternal(mexData, internalId))
            {
                mexData.FtDemo_SymbolNames.Set(internalId, new MEX_FtDemoSymbolNames()
                {
                    Result = DemoResult,
                    Intro = DemoIntro,
                    Ending = DemoEnding,
                    ViWait = DemoWait
                });
            }

            mexData.GmRst_AnimFiles.Set(externalID, new HSD_String() { Value = RstAnimFile });
            
            if (!IsExtendedCharacterExternal(mexData, externalID))
            {
                mexData.Char_CostumeIDs.Set(externalID, new MEX_CostumeIDs()
                {
                    CostumeCount = CostumeCount,
                    RedCostumeIndex = RedCostumeID,
                    GreenCostumeIndex = GreenCostumeID,
                    BlueCostumeIndex = BlueCostumeID
                });

                mexData.GmRst_Scale.Set(externalID, new HSD_Float() { Value = ResultScreenScale });
                mexData.GmRst_VictoryTheme.Set(externalID, new HSD_Int() { Value = VictoryThemeID });
            }

        }

        public override string ToString()
        {
            return NameText;
        }
    }


    public class MEXFunctionPointers
    {
        [TypeConverter(typeof(HexType))]
        public int OnLoad { get; set; }

        [TypeConverter(typeof(HexType))]
        public int OnDeath { get; set; }

        [TypeConverter(typeof(HexType))]
        public int OnUnk { get; set; }

        public MEX_MoveLogic[] MoveLogic { get; set; }

        [TypeConverter(typeof(HexType))]
        public int SpecialN { get; set; }

        [TypeConverter(typeof(HexType))]
        public int SpecialNAir { get; set; }

        [TypeConverter(typeof(HexType))]
        public int SpecialHi { get; set; }

        [TypeConverter(typeof(HexType))]
        public int SpecialHiAir { get; set; }

        [TypeConverter(typeof(HexType))]
        public int SpecialLw { get; set; }

        [TypeConverter(typeof(HexType))]
        public int SpecialLwAir { get; set; }

        [TypeConverter(typeof(HexType))]
        public int SpecialS { get; set; }

        [TypeConverter(typeof(HexType))]
        public int SpecialSAir { get; set; }


        public MEXFunctionPointers LoadData(MEX_Data mexData, int internalId, int externalID)
        {
            OnLoad = mexData.OnLoad[internalId].Value;
            OnDeath = mexData.OnDeath[internalId].Value;
            OnUnk = mexData.OnUnknown[internalId].Value;
            MoveLogic = mexData.MoveLogic.Array[internalId].Array;
            SpecialN = mexData.SpecialN[internalId].Value;
            SpecialNAir = mexData.SpecialNAir[internalId].Value;
            SpecialHi = mexData.SpecialHi[internalId].Value;
            SpecialHiAir = mexData.SpecialHiAir[internalId].Value;
            SpecialLw = mexData.SpecialLw[internalId].Value;
            SpecialLwAir = mexData.SpecialLwAir[internalId].Value;
            SpecialS = mexData.SpecialS[internalId].Value;
            SpecialSAir = mexData.SpecialSAir[internalId].Value;

            return this;
        }

        public void SaveData(MEX_Data mexData, int internalId, int externalID)
        {
            mexData.OnLoad.Set(internalId, new HSD_Int() { Value = OnLoad });
            mexData.OnDeath.Set(internalId, new HSD_Int() { Value = OnDeath });
            mexData.OnUnknown.Set(internalId, new HSD_Int() { Value = OnUnk });
            mexData.MoveLogic.Set(internalId, new HSDRaw.HSDArrayAccessor<MEX_MoveLogic>() { Array = MoveLogic });
            mexData.SpecialN.Set(internalId, new HSD_Int() { Value = SpecialN });
            mexData.SpecialNAir.Set(internalId, new HSD_Int() { Value = SpecialNAir });
            mexData.SpecialHi.Set(internalId, new HSD_Int() { Value = SpecialHi });
            mexData.SpecialHiAir.Set(internalId, new HSD_Int() { Value = SpecialHiAir });
            mexData.SpecialLw.Set(internalId, new HSD_Int() { Value = SpecialLw });
            mexData.SpecialLwAir.Set(internalId, new HSD_Int() { Value = SpecialLwAir });
            mexData.SpecialS.Set(internalId, new HSD_Int() { Value = SpecialS });
            mexData.SpecialSAir.Set(internalId, new HSD_Int() { Value = SpecialSAir });
        }
    }
}
