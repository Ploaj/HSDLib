using HSDRaw.Common;

namespace HSDRaw.MEX
{
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return FileNameS?.Value;
        }

    }
}
