using HSDRaw.Common;

namespace HSDRaw.AirRide.Kx
{
    public class KEX_VehicleTable : HSDAccessor
    {
        public override int TrimmedSize => 0x30;

        public KEX_VehicleMetadata MetaData { get => _s.GetReference<KEX_VehicleMetadata>(0x00); set => _s.SetReference(0x00, value); }

        // archive strings
        public HSDArrayAccessor<KEX_VehicleArchiveType> ArchiveStrings { get => _s.GetReference<HSDArrayAccessor<KEX_VehicleArchiveType>>(0x04); set => _s.SetReference(0x04, value); }

        // runtime archives
        public HSDUIntArray RuntimeArchivePointers { get => _s.GetReference<HSDUIntArray>(0x08); set => _s.SetReference(0x08, value); }

        // function table 1
        public HSDUIntArray FunctionTable1 { get => _s.GetReference<HSDUIntArray>(0x0C); set => _s.SetReference(0x0C, value); }

        // function table 2
        public HSDUIntArray FunctionTable2 { get => _s.GetReference<HSDUIntArray>(0x10); set => _s.SetReference(0x10, value); }

        public HSDByteArray SelectIconToVehicleLookup { get => _s.GetReference<HSDByteArray>(0x14); set => _s.SetReference(0x14, value); }

        public HSDArrayAccessor<KEX_VehicleSelectLookup> VehicleLookUp { get => _s.GetReference<HSDArrayAccessor<KEX_VehicleSelectLookup>>(0x18); set => _s.SetReference(0x18, value); }

        public HSDUIntArray VehicleSelectNameSISIndex { get => _s.GetReference<HSDUIntArray>(0x1C); set => _s.SetReference(0x1C, value); }

        public HSDUIntArray VehicleSelectDescSISIndex { get => _s.GetReference<HSDUIntArray>(0x20); set => _s.SetReference(0x20, value); }

        public override void New()
        {
            base.New();
            MetaData = new KEX_VehicleMetadata();
            ArchiveStrings = new HSDArrayAccessor<KEX_VehicleArchiveType>();
            RuntimeArchivePointers = new HSDUIntArray();
            FunctionTable1 = new HSDUIntArray();
            FunctionTable2 = new HSDUIntArray();
        }
    }

    public class KEX_VehicleMetadata : HSDAccessor
    {
        public override int TrimmedSize => 0x30;

        public int NumberOfStars { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public int NumberOfWheels { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int NumberOfSelectIcons { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public int SelectIconStride { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

        public int NumberOfDededeColors { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        public int NumberOfMetaknightColors { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }
    }

    public class KEX_VehicleArchiveType : HSDAccessor
    {
        public override int TrimmedSize => 0x04;

        public HSDArrayAccessor<KEX_VehicleArchiveStrings> FileName { get => _s.GetReference<HSDArrayAccessor<KEX_VehicleArchiveStrings>>(0x00); set => _s.SetReference(0x00, value); }
    }

    public class KEX_VehicleArchiveStrings : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public string FileName { get => _s.GetString(0x00); set => _s.SetString(0x00, value); }

        public string Symbol { get => _s.GetString(0x04); set => _s.SetString(0x04, value); }
    }

    public enum KARVehicleTypes
    {
        Star,
        Wheel
    }

    public enum KARRiderTypes
    {
        Kirby,
        Dedede,
        MetaKnight
    }

    public class KEX_VehicleSelectLookup : HSDAccessor
    {
        public override int TrimmedSize => 0x03;

        public KARRiderTypes RiderID { get => (KARRiderTypes)_s.GetByte(0x00); set => _s.SetByte(0x00, (byte)value); }

        public KARVehicleTypes TypeID { get => (KARVehicleTypes)_s.GetByte(0x01); set => _s.SetByte(0x01, (byte)value); }

        public byte VehicleID { get => _s.GetByte(0x02); set => _s.SetByte(0x02, value); }
    }
}
