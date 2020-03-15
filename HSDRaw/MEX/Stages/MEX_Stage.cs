using System.ComponentModel;

namespace HSDRaw.MEX.Stages
{
    public class MEX_Stage : HSDAccessor
    {
        public override int TrimmedSize => 0x34;

        public int StageInternalID { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }
        
        public HSDArrayAccessor<MEX_MapGOBJFunctions> GOBJFunctions { get => _s.GetReference<HSDArrayAccessor<MEX_MapGOBJFunctions>>(0x04); set => _s.SetReference(0x04, value); }
        
        public string StageFileName { get => _s.GetString(0x08); set => _s.SetString(0x08, value); }

        [TypeConverter(typeof(HexType))]
        public uint OnStageInit { get => (uint)_s.GetInt32(0x0C); set => _s.SetInt32(0x0C, (int)value); }

        [TypeConverter(typeof(HexType))]
        public uint OnUnknown1 { get => (uint)_s.GetInt32(0x10); set => _s.SetInt32(0x10, (int)value); }

        [TypeConverter(typeof(HexType))]
        public uint OnStageLoad { get => (uint)_s.GetInt32(0x14); set => _s.SetInt32(0x14, (int)value); }

        [TypeConverter(typeof(HexType))]
        public uint OnStageGo { get => (uint)_s.GetInt32(0x18); set => _s.SetInt32(0x18, (int)value); }

        [TypeConverter(typeof(HexType))]
        public uint OnUnknown2 { get => (uint)_s.GetInt32(0x1C); set => _s.SetInt32(0x1C, (int)value); }

        [TypeConverter(typeof(HexType))]
        public uint OnUnknown3 { get => (uint)_s.GetInt32(0x20); set => _s.SetInt32(0x20, (int)value); }

        [TypeConverter(typeof(HexType))]
        public uint OnUnknown4 { get => (uint)_s.GetInt32(0x24); set => _s.SetInt32(0x24, (int)value); }

        public int UnknownValue { get => _s.GetInt32(0x28); set => _s.SetInt32(0x28, value); }

        public HSDArrayAccessor<MEX_MovingCollisionPoint> MovingCollisionPoint { get => _s.GetReference<HSDArrayAccessor<MEX_MovingCollisionPoint>>(0x2C); set => _s.SetReference(0x2C, value); }

        public int MovingCollisionPointCount { get => _s.GetInt32(0x30); set => _s.SetInt32(0x30, value); }

    }

    public class MEX_MapGOBJFunctions : HSDAccessor
    {
        public override int TrimmedSize => 0x14;

        [TypeConverter(typeof(HexType))]
        public uint OnCreation { get => (uint)_s.GetInt32(0x00); set => _s.SetInt32(0x00, (int)value); }

        [TypeConverter(typeof(HexType))]
        public uint OnDeletion { get => (uint)_s.GetInt32(0x04); set => _s.SetInt32(0x04, (int)value); }

        [TypeConverter(typeof(HexType))]
        public uint OnFrame { get => (uint)_s.GetInt32(0x08); set => _s.SetInt32(0x08, (int)value); }

        [TypeConverter(typeof(HexType))]
        public uint OnUnk { get => (uint)_s.GetInt32(0x0C); set => _s.SetInt32(0x0C, (int)value); }

        public int Bitflags { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }
        
    }

    public class MEX_MovingCollisionPoint : HSDAccessor
    {
        public override int TrimmedSize => 0x06;

        public short LineID { get => _s.GetInt16(0x00); set => _s.SetInt16(0x00, value); }

        public short GOBJID { get => _s.GetInt16(0x02); set => _s.SetInt16(0x02, value); }

        public short Unknown { get => _s.GetInt16(0x04); set => _s.SetInt16(0x04, value); }
    }
}
