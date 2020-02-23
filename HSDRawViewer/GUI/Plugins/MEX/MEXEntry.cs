using HSDRaw;
using HSDRaw.Common;
using HSDRaw.MEX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.TypeInspectors;

namespace HSDRawViewer.GUI.Plugins.MEX
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

        [DisplayName("SSM Bitfield 1"), Category("3 - Misc"), Description(""), TypeConverter(typeof(HexType))]
        public uint SSMBitfield1 { get; set; }

        [DisplayName("SSM Bitfield 2"), Category("3 - Misc"), Description(""), TypeConverter(typeof(HexType))]
        public uint SSMBitfield2 { get; set; }

        [DisplayName("Narrator Sound Clip"), Category("3 - Misc"), Description("Index of narrator sound clip")]
        public int NameCallSound { get; set; }

        [DisplayName("Sub-Character"), 
            Category("3 - Misc"), Description(""), 
            TypeConverter(typeof(FighterInternalIDConverter))]
        public int SubCharacterInternalID { get; set; }

        [DisplayName("SubCharacter Behavior"), Category("3 - Misc"), Description("")]
        public SubCharacterBehavior SubCharacterBehavior { get; set; }

        public MEXFunctionPointers Functions = new MEXFunctionPointers();

        public bool IsSpecialCharacterInternal(MEX_Data mexData, int internalID)
        {
            return internalID >= mexData.MetaData.NumOfInternalIDs - MEXIdConverter.InternalSpecialCharCount;
        }

        public bool IsSpecialCharacterExternal(MEX_Data mexData, int externalID)
        {
            return externalID >= mexData.MetaData.NumOfExternalIDs - MEXIdConverter.ExternalSpecialCharCount;
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

            EffectIndex = mexData.Char_EffectIDs[internalId].Value;
            NameCallSound = mexData.SFX_NameDef[externalID].Value;

            SSMIndex = mexData.SSM_CharSSMFileIDs[externalID].SSMID;
            SSMBitfield1 = (uint)mexData.SSM_CharSSMFileIDs[externalID].BitField1;
            SSMBitfield2 = (uint)mexData.SSM_CharSSMFileIDs[externalID].BitField2;

            SubCharacterInternalID = (sbyte)mexData.Char_DefineIDs[externalID].SubCharacterInternalID;
            SubCharacterBehavior = mexData.Char_DefineIDs[externalID].SubCharacterBehavior;
            
            if (!IsSpecialCharacterInternal(mexData, internalId))
            {
                DemoResult = mexData.FtDemo_SymbolNames.Array[internalId].Result;
                DemoIntro = mexData.FtDemo_SymbolNames.Array[internalId].Intro;
                DemoEnding = mexData.FtDemo_SymbolNames.Array[internalId].Ending;
                DemoWait = mexData.FtDemo_SymbolNames.Array[internalId].ViWait;
            }

            if (!IsSpecialCharacterExternal(mexData, externalID))
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

            mexData.Char_EffectIDs.Set(internalId, new HSD_Byte() { Value = EffectIndex });
            mexData.SFX_NameDef.Set(externalID, new HSD_Int() { Value = NameCallSound });

            mexData.Char_CostumePointers.Set(internalId, new MEX_CostumeRuntimePointers()
            {
                CostumeCount = CostumeCount,
                Pointer = new HSDRaw.HSDAccessor() { _s = new HSDRaw.HSDStruct(4 * 6) }
            });

            mexData.SSM_CharSSMFileIDs.Set(externalID, new MEX_CharSSMFileID()
            {
                SSMID = SSMIndex,
                BitField1 = (int)SSMBitfield1,
                BitField2 = (int)SSMBitfield2
            });

            mexData.Char_DefineIDs.Set(externalID, new MEX_CharDefineIDs()
            {
                InternalID = (byte)(internalId + (internalId == 11 ? -1 : 0)), // popo id reference ice climbers
                SubCharacterInternalID = (byte)SubCharacterInternalID,
                SubCharacterBehavior = SubCharacterBehavior
            });

            if (!IsSpecialCharacterInternal(mexData, internalId))
            {
                if(DemoResult == null)
                {
                    DemoResult = "";
                    DemoIntro = "";
                    DemoEnding = "";
                    DemoWait = "";
                }
                mexData.FtDemo_SymbolNames.Set(internalId, new MEX_FtDemoSymbolNames()
                {
                    Result = DemoResult,
                    Intro = DemoIntro,
                    Ending = DemoEnding,
                    ViWait = DemoWait
                });
            }

            mexData.GmRst_AnimFiles.Set(externalID, new HSD_String() { Value = RstAnimFile });
            
            if (!IsSpecialCharacterExternal(mexData, externalID))
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
        public uint OnLoad { get; set; }

        [TypeConverter(typeof(HexType))]
        public uint OnDeath { get; set; }

        [TypeConverter(typeof(HexType))]
        public uint OnUnk { get; set; }

        public MEX_MoveLogic[] MoveLogic { get; set; }

        [TypeConverter(typeof(HexType))]
        public uint SpecialN { get; set; }

        [TypeConverter(typeof(HexType))]
        public uint SpecialNAir { get; set; }

        [TypeConverter(typeof(HexType))]
        public uint SpecialHi { get; set; }

        [TypeConverter(typeof(HexType))]
        public uint SpecialHiAir { get; set; }

        [TypeConverter(typeof(HexType))]
        public uint SpecialLw { get; set; }

        [TypeConverter(typeof(HexType))]
        public uint SpecialLwAir { get; set; }

        [TypeConverter(typeof(HexType))]
        public uint SpecialS { get; set; }

        [TypeConverter(typeof(HexType))]
        public uint SpecialSAir { get; set; }


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
            mexData.OnLoad.Set(internalId, new HSD_UInt() { Value = OnLoad });
            mexData.OnDeath.Set(internalId, new HSD_UInt() { Value = OnDeath });
            mexData.OnUnknown.Set(internalId, new HSD_UInt() { Value = OnUnk });
            mexData.MoveLogic.Set(internalId, new HSDRaw.HSDArrayAccessor<MEX_MoveLogic>() { Array = MoveLogic });
            mexData.SpecialN.Set(internalId, new HSD_UInt() { Value = SpecialN });
            mexData.SpecialNAir.Set(internalId, new HSD_UInt() { Value = SpecialNAir });
            mexData.SpecialHi.Set(internalId, new HSD_UInt() { Value = SpecialHi });
            mexData.SpecialHiAir.Set(internalId, new HSD_UInt() { Value = SpecialHiAir });
            mexData.SpecialLw.Set(internalId, new HSD_UInt() { Value = SpecialLw });
            mexData.SpecialLwAir.Set(internalId, new HSD_UInt() { Value = SpecialLwAir });
            mexData.SpecialS.Set(internalId, new HSD_UInt() { Value = SpecialS });
            mexData.SpecialSAir.Set(internalId, new HSD_UInt() { Value = SpecialSAir });
        }
    }
    
    /// <summary>
    /// use proxy class for make selecting character id easier
    /// </summary>
    public class MEX_CSSIconEntry
    {
        [Description("Joint ID on the CSS to use for Icon Flash Animation")]
        public int JointID { get; set; }

        [Description("Indicates a clone fighter, can leave false for added fighters")]
        public bool Clone { get; set; }

        [DisplayName("Fighter"), TypeConverter(typeof(FighterExternalIDConverter))]
        public int FighterExternalID { get; set; }

        [Description("Starting X Coord")]
        public float X1 { get; set; }

        [Description("Ending X Coord")]
        public float X2 { get; set; }

        [Description("Starting Y Coord")]
        public float Y1 { get; set; }

        [Description("Ending Y Coord")]
        public float Y2 { get; set; }

        public static MEX_CSSIconEntry FromIcon(MEX_CSSIcon icon)
        {
            return new MEX_CSSIconEntry()
            {
                JointID = icon.JointID,
                FighterExternalID = icon.ExternalCharID,
                Clone = icon.ExternalCharID != icon.CharUNKID,
                X1 = icon.X1,
                Y1 = icon.Y1,
                X2 = icon.X2,
                Y2 = icon.Y2
            };
        }

        public MEX_CSSIcon ToIcon()
        {
            return new MEX_CSSIcon()
            {
                JointID = (byte)JointID,
                UnkID = (byte)JointID,
                ExternalCharID = (byte)FighterExternalID,
                CharUNKID = (byte)(FighterExternalID + (Clone ? -1 : 0)),
                X1 = X1,
                Y1 = Y1,
                X2 = X2,
                Y2 = Y2
            };
        }

        public override string ToString()
        {
            return $"{FighterConverter.externalIDValues[FighterExternalID + 1]} ({X1}, {Y1}, {X2}, {Y2})";
        }
    }

    public class MEXTypeInspector : TypeInspectorSkeleton
    {
        private readonly ITypeInspector _innerTypeDescriptor;

        public MEXTypeInspector(ITypeInspector innerTypeDescriptor)
        {
            _innerTypeDescriptor = innerTypeDescriptor;
        }

        public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
        {
            var props = _innerTypeDescriptor.GetProperties(type, container);
            props = props.Where(p => p.Type != typeof(HSDStruct) && p.Name != "trimmedSize" && p.Name != "costumeCount");
            return props;
        }
    }
}
