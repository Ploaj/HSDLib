using System.Text;

namespace HSDRaw.MEX
{
    public class MEX_Data : HSDAccessor
    {
        public override int TrimmedSize => 0x80;

        public MEX_IconData MnSlChr_IconData { get => _s.GetReference<MEX_IconData>(0x00); set => _s.SetReference(0x00, value); }

        public HSDNullPointerArrayAccessor<HSD_String> MnSlChr_NameText { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_String>>(0x04); set => _s.SetReference(0x04, value); }

        public HSDArrayAccessor<MEX_CharFileStrings> Char_CharFiles { get => _s.GetReference<HSDArrayAccessor<MEX_CharFileStrings>>(0x08); set => _s.SetReference(0x08, value); }

        public HSDArrayAccessor<MEX_CostumeIDs> Char_CostumeIDs { get => _s.GetReference<HSDArrayAccessor<MEX_CostumeIDs>>(0x0C); set => _s.SetReference(0x0C, value); }

        public HSDArrayAccessor<MEX_CostumeFileSymbolTable> Char_CostumeFileSymbols { get => _s.GetReference<HSDArrayAccessor<MEX_CostumeFileSymbolTable>>(0x10); set => _s.SetReference(0x10, value); }

        public HSDNullPointerArrayAccessor<HSD_String> Char_AnimFiles { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_String>>(0x14); set => _s.SetReference(0x14, value); }

        public HSDArrayAccessor<MEX_AnimCount> Char_AnimCount { get => _s.GetReference<HSDArrayAccessor<MEX_AnimCount>>(0x18); set => _s.SetReference(0x18, value); }

        public HSDArrayAccessor<MEX_EffectFiles> Char_EffectFiles { get => _s.GetReference<HSDArrayAccessor<MEX_EffectFiles>>(0x1C); set => _s.SetReference(0x1C, value); }

        // Effect IDs

        public HSDNullPointerArrayAccessor<HSD_String> GmRst_AnimFiles { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_String>>(0x24); set => _s.SetReference(0x24, value); }

        // insignia ids

        public HSDArrayAccessor<HSD_Float> GmRst_Scale { get => _s.GetReference<HSDArrayAccessor<HSD_Float>>(0x2C); set => _s.SetReference(0x2C, value); }

        // GmRst_VictoryTheme

        public HSDNullPointerArrayAccessor<MEX_FtDemoSymbolNames> FtDemo_SymbolNames { get => _s.GetReference<HSDNullPointerArrayAccessor<MEX_FtDemoSymbolNames>>(0x34); set => _s.SetReference(0x34, value); }

        public HSDArrayAccessor<MEX_CharDefineIDs> Char_DefineIDs { get => _s.GetReference<HSDArrayAccessor<MEX_CharDefineIDs>>(0x38); set => _s.SetReference(0x38, value); }

        // SFX_NameDef

        public HSDArrayAccessor<MEX_CharSSMFileID> SSM_CharSSMFileIDs { get => _s.GetReference<HSDArrayAccessor<MEX_CharSSMFileID>>(0x40); set => _s.SetReference(0x40, value); }

        public HSDNullPointerArrayAccessor<HSD_String> SSM_SSMFiles { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_String>>(0x44); set => _s.SetReference(0x44, value); }

        // SSM runtime struct

    }

    public class HSD_Float : HSDAccessor
    {
        public override int TrimmedSize => 0x04;

        public float Value { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }
    }

    public class MEX_EffectFiles : HSDAccessor
    {
        public override int TrimmedSize => 0x0C;
        
        public string FileName
        {
            get => FileNameS?.Value;
            set
            {
                if (FileNameS == null)
                    FileNameS = new HSD_String();
                FileNameS.Value = value;
            }
        }

        public string Symbol
        {
            get => SymbolS?.Value;
            set
            {
                if (SymbolS == null)
                    SymbolS = new HSD_String();
                SymbolS.Value = value;
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
    }
}
