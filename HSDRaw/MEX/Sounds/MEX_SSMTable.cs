using HSDRaw.Common;

namespace HSDRaw.MEX
{
    public class MEX_SSMTable : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public HSDNullPointerArrayAccessor<HSD_String> SSM_SSMFiles { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_String>>(0x00); set => _s.SetReference(0x00, value); }

        public HSDArrayAccessor<MEX_SSMSizeAndFlags> SSM_Flags { get => _s.GetReference<HSDArrayAccessor<MEX_SSMSizeAndFlags>>(0x04); set => _s.SetReference(0x04, value); }

        public HSDArrayAccessor<MEX_SSMLookup> SSM_LookupTable { get => _s.GetReference<HSDArrayAccessor<MEX_SSMLookup>>(0x08); set => _s.SetReference(0x08, value); }

        // 0x0C - runtime struct
    }
}
