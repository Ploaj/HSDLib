using HSDRaw.Common;
using HSDRaw.Melee.Pl;

namespace HSDRaw.MEX
{
    public class MEX_CostumeSymbol : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public MEX_CostumeSymbolVisLookup CostumeVisLookup { get => _s.GetReference<MEX_CostumeSymbolVisLookup>(0x00); set => _s.SetReference(0x00 ,value); }

        public MEX_CostumeSymbolMatLookup CostumeMatLookup { get => _s.GetReference<MEX_CostumeSymbolMatLookup>(0x04); set => _s.SetReference(0x04, value); }

        public int AccessoryCount { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public HSDFixedLengthPointerArrayAccessor<MEX_CostumeAccessory> Accessories { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<MEX_CostumeAccessory>>(0x0C); set => _s.SetReference(0x0C, value); }
    }

    public class MEX_CostumeAccessory : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public HSD_JOBJ RootJoint { get => _s.GetReference<HSD_JOBJ>(0x00); set => _s.SetReference(0x00, value); }

        public int AttachBone { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int DynamicCount { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public HSDFixedLengthPointerArrayAccessor<SBM_DynamicDesc> DynamicDef { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<SBM_DynamicDesc>>(0x0C); set => _s.SetReference(0x0C, value); }

        public int LookupCount { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        public SBM_CostumeLookupTable LookupTable { get => _s.GetReference<SBM_CostumeLookupTable>(0x14); set => _s.SetReference(0x14, value); }
    }

    public class MEX_CostumeSymbolVisLookup : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public int Count { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public SBM_CostumeLookupTable LookupTable { get => _s.GetReference<SBM_CostumeLookupTable>(0x04); set => _s.SetReference(0x04 ,value); }
    }

    public class MEX_CostumeSymbolMatLookup : HSDAccessor
    {
        public override int TrimmedSize => 0x04;
        public HSDShortArray LookupTable { get => _s.GetReference<HSDShortArray>(0x00); set => _s.SetReference(0x00, value); }
    }
}
