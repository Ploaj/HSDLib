using HSDRaw.Common;

namespace HSDRaw.MEX
{
    public class MEX_CharFileStrings : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public string FileName { get => FileNameS.Value; set => FileNameS.Value = value; }

        public string Symbol { get => SymbolS.Value; set => SymbolS.Value = value; }

        private HSD_String FileNameS { get => _s.GetCreateReference<HSD_String>(0x00); set => _s.SetReference(0x00, value); }

        private HSD_String SymbolS { get => _s.GetCreateReference<HSD_String>(0x04); set => _s.SetReference(0x04, value); }
    }
}
