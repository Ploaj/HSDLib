using HSDRaw.Common;
using System.ComponentModel;
using System.Text;

namespace HSDRaw.MEX
{
    public class MEX_Meta : HSDAccessor
    {
        public override int TrimmedSize => 0x20;
        
        [DisplayName("Internal ID Count")]
        public int NumOfInternalIDs { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        [DisplayName("External ID Count")]
        public int NumOfExternalIDs { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        [DisplayName("CSS Icon Count")]
        public int NumOfCSSIcons { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        [DisplayName("SSM Count")]
        public int NumOfSSMs { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }
        
        [DisplayName("BGM Count")]
        public int NumOfMusic { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        [DisplayName("Effect Count")]
        public int NumOfEffects { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }
    }

    public class MEX_Data : HSDAccessor
    {
        public override int TrimmedSize => 0x100;

        public MEX_Meta MetaData { get => _s.GetReference<MEX_Meta>(0x00); set => _s.SetReference(0x00, value); }

        public MEX_IconData MnSlChr_IconData { get => _s.GetReference<MEX_IconData>(0x04); set => _s.SetReference(0x04, value); }

        public HSDNullPointerArrayAccessor<HSD_String> MnSlChr_NameText { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_String>>(0x08); set => _s.SetReference(0x08, value); }

        public HSDArrayAccessor<MEX_CharFileStrings> Char_CharFiles { get => _s.GetReference<HSDArrayAccessor<MEX_CharFileStrings>>(0x0C); set => _s.SetReference(0x0C, value); }

        public HSDArrayAccessor<MEX_CostumeIDs> Char_CostumeIDs { get => _s.GetReference<HSDArrayAccessor<MEX_CostumeIDs>>(0x10); set => _s.SetReference(0x10, value); }

        public HSDArrayAccessor<MEX_CostumeFileSymbolTable> Char_CostumeFileSymbols { get => _s.GetReference<HSDArrayAccessor<MEX_CostumeFileSymbolTable>>(0x14); set => _s.SetReference(0x14, value); }

        public HSDNullPointerArrayAccessor<HSD_String> Char_AnimFiles { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_String>>(0x18); set => _s.SetReference(0x18, value); }

        public HSDArrayAccessor<MEX_AnimCount> Char_AnimCount { get => _s.GetReference<HSDArrayAccessor<MEX_AnimCount>>(0x1C); set => _s.SetReference(0x1C, value); }

        public HSDArrayAccessor<MEX_EffectFiles> Char_EffectFiles { get => _s.GetReference<HSDArrayAccessor<MEX_EffectFiles>>(0x20); set => _s.SetReference(0x20, value); }

        public HSDArrayAccessor<HSD_Byte> Char_EffectIDs { get => _s.GetReference<HSDArrayAccessor<HSD_Byte>>(0x24); set => _s.SetReference(0x24, value); }
        
        public HSDNullPointerArrayAccessor<HSD_String> GmRst_AnimFiles { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_String>>(0x28); set => _s.SetReference(0x28, value); }

        public HSDArrayAccessor<HSD_Byte> Char_InsigniaIDs { get => _s.GetReference<HSDArrayAccessor<HSD_Byte>>(0x2C); set => _s.SetReference(0x2C, value); }
        
        public HSDArrayAccessor<HSD_Float> GmRst_Scale { get => _s.GetReference<HSDArrayAccessor<HSD_Float>>(0x30); set => _s.SetReference(0x30, value); }

        public HSDArrayAccessor<HSD_Int> GmRst_VictoryTheme { get => _s.GetReference<HSDArrayAccessor<HSD_Int>>(0x34); set => _s.SetReference(0x34, value); }
        
        public HSDNullPointerArrayAccessor<MEX_FtDemoSymbolNames> FtDemo_SymbolNames { get => _s.GetReference<HSDNullPointerArrayAccessor<MEX_FtDemoSymbolNames>>(0x38); set => _s.SetReference(0x38, value); }

        public HSDArrayAccessor<MEX_CharDefineIDs> Char_DefineIDs { get => _s.GetReference<HSDArrayAccessor<MEX_CharDefineIDs>>(0x3C); set => _s.SetReference(0x3C, value); }

        public HSDArrayAccessor<HSD_Int> SFX_NameDef { get => _s.GetReference<HSDArrayAccessor<HSD_Int>>(0x40); set => _s.SetReference(0x40, value); }

        public HSDArrayAccessor<MEX_CharSSMFileID> SSM_CharSSMFileIDs { get => _s.GetReference<HSDArrayAccessor<MEX_CharSSMFileID>>(0x44); set => _s.SetReference(0x44, value); }

        public HSDNullPointerArrayAccessor<HSD_String> SSM_SSMFiles { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_String>>(0x48); set => _s.SetReference(0x48, value); }

        // SSM runtime struct

        public HSDArrayAccessor<HSD_UInt> OnLoad { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x50); set => _s.SetReference(0x50, value); }

        public HSDArrayAccessor<HSD_UInt> OnDeath { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x54); set => _s.SetReference(0x54, value); }

        public HSDArrayAccessor<HSD_UInt> OnUnknown { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x58); set => _s.SetReference(0x58, value); }

        public HSDFixedLengthPointerArrayAccessor<HSDArrayAccessor<MEX_MoveLogic>> MoveLogic { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSDArrayAccessor<MEX_MoveLogic>>>(0x5C); set => _s.SetReference(0x5C, value); }
        
        public HSDArrayAccessor<HSD_UInt> SpecialN { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x60); set => _s.SetReference(0x60, value); }

        public HSDArrayAccessor<HSD_UInt> SpecialNAir { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x64); set => _s.SetReference(0x64, value); }

        public HSDArrayAccessor<HSD_UInt> SpecialHi { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x68); set => _s.SetReference(0x68, value); }

        public HSDArrayAccessor<HSD_UInt> SpecialHiAir { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x6C); set => _s.SetReference(0x6C, value); }

        public HSDArrayAccessor<HSD_UInt> SpecialLw { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x70); set => _s.SetReference(0x70, value); }

        public HSDArrayAccessor<HSD_UInt> SpecialLwAir { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x74); set => _s.SetReference(0x74, value); }
        
        public HSDArrayAccessor<HSD_UInt> SpecialS { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x78); set => _s.SetReference(0x78, value); }

        public HSDArrayAccessor<HSD_UInt> SpecialSAir { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x7C); set => _s.SetReference(0x7C, value); }

        public HSDArrayAccessor<SSMFlag> SSM_Flags { get => _s.GetReference<HSDArrayAccessor<SSMFlag>>(0x80); set => _s.SetReference(0x80, value); }

        public HSDArrayAccessor<MEX_CostumeRuntimePointers> Char_CostumePointers { get => _s.GetReference<HSDArrayAccessor<MEX_CostumeRuntimePointers>>(0x84); set => _s.SetReference(0x84, value); }

        public HSDArrayAccessor<MEX_SSMLookup> SSM_LookupTable { get => _s.GetReference<HSDArrayAccessor<MEX_SSMLookup>>(0x88); set => _s.SetReference(0x88, value); }

        public HSDFixedLengthPointerArrayAccessor<HSD_String> BackgroundMusicStrings { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSD_String>>(0x8C); set => _s.SetReference(0x8C, value); }
    }

    public class MEX_CostumeRuntimePointers : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public HSDAccessor Pointer { get => _s.GetReference<HSDAccessor>(0x00); set => _s.SetReference(0x00, value); }

        public byte CostumeCount { get => _s.GetByte(0x04); set => _s.SetByte(0x04, value); }
    }

    public class MEX_SSMLookup : HSDAccessor
    {
        public override int TrimmedSize => 0x04;

        public byte GroupIndex { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }
        public byte Unknown1 { get => _s.GetByte(0x01); set => _s.SetByte(0x01, value); }
        public byte Unknown2 { get => _s.GetByte(0x02); set => _s.SetByte(0x02, value); }
        public byte Unknown3 { get => _s.GetByte(0x03); set => _s.SetByte(0x03, value); }
    }

    public class SSMFlag : HSDAccessor
    {
        public override int TrimmedSize => 0x8;

        public int SSMFileSize { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public int Flag { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }
    }

    public class MEX_EffectFiles : HSDAccessor
    {
        public override int TrimmedSize => 0x0C;
        
        public string FileName
        {
            get => FileNameS?.Value;
            set
            {
                if (value == null)
                    FileNameS = null;
                else
                {
                    if (FileNameS == null)
                        FileNameS = new HSD_String();
                    FileNameS.Value = value;
                }
            }
        }

        public string Symbol
        {
            get => SymbolS?.Value;
            set
            {
                if (value == null)
                    SymbolS = null;
                else
                {
                    if (SymbolS == null)
                        SymbolS = new HSD_String();
                    SymbolS.Value = value;
                }
            }
        }

        private HSD_String FileNameS { get => _s.GetReference<HSD_String>(0x00); set => _s.SetReference(0x00, value); }

        private HSD_String SymbolS { get => _s.GetReference<HSD_String>(0x04); set => _s.SetReference(0x04, value); }
        
    }

    public class MEX_AnimCount : HSDAccessor
    {
        public override int TrimmedSize => 8;

        // runtime pointer at 0x00

        public int AnimCount { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }
    }

    public class HSD_String : HSDAccessor
    {
        public string Value
        {
            get
            {
                var nullpoint = 0;
                foreach (var d in _s.GetData())
                    if (d == 0)
                        break;
                    else
                        nullpoint++;
                return Encoding.UTF8.GetString(_s.GetData(), 0, nullpoint);
            }
            set
            {
                _s.SetData(Encoding.UTF8.GetBytes(value));
                _s.Resize(_s.Length + 1);
                if (_s.Length % 4 != 0)
                    _s.Resize(_s.Length + (4 - (_s.Length % 4)));
            }
        }

        public override void New()
        {
            base.New();
            _s.Resize(0x04);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
