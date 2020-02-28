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

        public HSDArrayAccessor<HSD_Byte> EffectIDs { get => _s.GetReference<HSDArrayAccessor<HSD_Byte>>(0x10); set => _s.SetReference(0x10, value); }

        public HSDArrayAccessor<MEX_KirbyHatLoad> KirbyHatFunctions { get => _s.GetReference<HSDArrayAccessor<MEX_KirbyHatLoad>>(0x14); set => _s.SetReference(0x14, value); }

        public HSDArrayAccessor<HSD_UInt> KirbySpecialN { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x18); set => _s.SetReference(0x18, value); }

        public HSDArrayAccessor<HSD_UInt> KirbySpecialNAir { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x1C); set => _s.SetReference(0x1C, value); }

        public HSDArrayAccessor<HSD_UInt> KirbyOnHit { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x20); set => _s.SetReference(0x20, value); }

        public HSDArrayAccessor<HSD_UInt> KirbyOnItemInit { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x24); set => _s.SetReference(0x24, value); }

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

    public class MEX_KirbyHatLoad : HSDAccessor
    {
        public override int TrimmedSize => 8;

        public uint HatAdd { get => (uint)_s.GetInt32(0x00); set => _s.SetInt32(0x00, (int)value); }

        public uint HatRemove { get => (uint)_s.GetInt32(0x04); set => _s.SetInt32(0x04, (int)value); }

    }

    public class MEX_KirbyCostume : HSDFixedLengthPointerArrayAccessor<HSD_String>
    {

    }
}
