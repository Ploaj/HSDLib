using System;

namespace HSDRaw.AirRide.Kx
{
    public class KEX_RiderTable : HSDAccessor
    {
        public override int TrimmedSize => 0x30;

        public int RiderCostumeCount { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public HSDByteArray RuntimeMenuSelection { get => _s.GetReference<HSDByteArray>(0x04); set => _s.SetReference(0x04, value); }

        public HSDArrayAccessor<KEX_RiderFiles> FileStrings { get => _s.GetReference<HSDArrayAccessor<KEX_RiderFiles>>(0x08); set => _s.SetReference(0x08, value); }

        public HSDUIntArray ArchiveSymbolRuntime { get => _s.GetReference<HSDUIntArray>(0x0C); set => _s.SetReference(0x0C, value); }

        public HSDFixedLengthPointerArrayAccessor<KEX_RiderParams> CustomRiderParams { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<KEX_RiderParams>>(0x10); set => _s.SetReference(0x10, value); }

        // 0x10 reserved

        public HSDUIntArray FunctionRuntime { get => _s.GetReference<HSDUIntArray>(0x14); set => _s.SetReference(0x14, value); }

    }

    [Flags]
    public enum KexRiderFlags
    {
        CanInhale       = 1 << 0,
        CanWearHats     = 1 << 1,
    }

    public class KEX_RiderParams : HSDAccessor
    {
        public override int TrimmedSize => 0x30;

        public byte DefaultColor
        {
            get => _s.GetByte(0x00);
            set
            {
                if (value < 0)
                    value = 0;
                else
                    if (value > 7)
                    value = 7;
                _s.SetByte(0x00, value);
            }
        }

        public KexRiderFlags Flags { get => (KexRiderFlags)(_s.GetInt32(0x00) & 0xFFFFFF); set => _s.SetInt32(0x00, ((int)(_s.GetInt32(0x00) & 0xFF000000) | (int)value)); }
    }

    public class KEX_RiderFiles : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public string DataFilePath { get => _s.GetString(0x00); set => _s.SetString(0x00, value); }

        public string DataSymbol { get => _s.GetString(0x04); set => _s.SetString(0x04, value); }
    }
}
