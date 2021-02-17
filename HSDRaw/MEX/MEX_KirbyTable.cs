using HSDRaw.Common;

namespace HSDRaw.MEX
{
    public class MEX_KirbyTable : HSDAccessor
    {
        public override int TrimmedSize => 0x28;

        public HSDArrayAccessor<MEX_KirbyCapFiles> CapFiles { get => _s.GetReference<HSDArrayAccessor<MEX_KirbyCapFiles>>(0x00); set => _s.SetReference(0x00, value); }
        
        public HSDAccessor CapFileRuntime { get => _s.GetReference<HSDAccessor>(0x04); set => _s.SetReference(0x04, value); }

        public HSDFixedLengthPointerArrayAccessor<MEX_KirbyCostume> KirbyCostumes { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<MEX_KirbyCostume>>(0x08); set => _s.SetReference(0x08, value); }

        public HSDAccessor CostumeRuntime { get => _s.GetReference<HSDAccessor>(0x0C); set => _s.SetReference(0x0C, value); }

        public HSDByteArray KirbyEffectIDs { get => _s.GetReference<HSDByteArray>(0x10); set => _s.SetReference(0x10, value); }

        public HSDAccessor CapFtCmdRuntime { get => _s.GetReference<HSDAccessor>(0x14); set => _s.SetReference(0x14, value); }

        public override void New()
        {
            base.New();
            CapFiles = new HSDArrayAccessor<MEX_KirbyCapFiles>();
            CapFileRuntime = new HSDAccessor();
            KirbyCostumes = new HSDFixedLengthPointerArrayAccessor<MEX_KirbyCostume>();
            CostumeRuntime = new HSDAccessor();
            KirbyEffectIDs = new HSDByteArray();
            CapFtCmdRuntime = new HSDAccessor();
        }
    }

    public class MEX_KirbyFunctionTable : HSDAccessor
    {
        public override int TrimmedSize => 0x20;

        public HSDUIntArray OnAbilityGain { get => _s.GetReference<HSDUIntArray>(0x00); set => _s.SetReference(0x00, value); }

        public HSDUIntArray OnAbilityLose { get => _s.GetReference<HSDUIntArray>(0x04); set => _s.SetReference(0x04, value); }

        public HSDUIntArray KirbySpecialN { get => _s.GetReference<HSDUIntArray>(0x08); set => _s.SetReference(0x08, value); }

        public HSDUIntArray KirbySpecialNAir { get => _s.GetReference<HSDUIntArray>(0x0C); set => _s.SetReference(0x0C, value); }

        public HSDUIntArray KirbyOnHit { get => _s.GetReference<HSDUIntArray>(0x10); set => _s.SetReference(0x10, value); }

        public HSDUIntArray KirbyOnItemInit { get => _s.GetReference<HSDUIntArray>(0x14); set => _s.SetReference(0x14, value); }

        public HSDAccessor MoveLogicRuntime { get => _s.GetReference<HSDAccessor>(0x18); set => _s.SetReference(0x18, value); }

        public HSDUIntArray KirbyOnFrame { get => _s.GetReference<HSDUIntArray>(0x1C); set => _s.SetReference(0x1C, value); }

        public override void New()
        {
            base.New();

            OnAbilityGain = new HSDUIntArray();
            OnAbilityLose = new HSDUIntArray();
            KirbySpecialN = new HSDUIntArray();
            KirbySpecialNAir = new HSDUIntArray();
            KirbyOnHit = new HSDUIntArray();
            KirbyOnItemInit = new HSDUIntArray();
            MoveLogicRuntime = new HSDAccessor();
            KirbyOnFrame = new HSDUIntArray();
        }
    }

    public class MEX_KirbyCapFiles : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

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

    public class MEX_KirbyCostume : HSDArrayAccessor<MEX_CostumeFileSymbol>
    {

    }
}
