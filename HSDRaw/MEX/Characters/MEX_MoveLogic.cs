using System.ComponentModel;

namespace HSDRaw.MEX
{
    public class MEX_MoveLogic : HSDAccessor
    {
        public override int TrimmedSize => 0x20;

        public int AnimationID { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        [TypeConverter(typeof(HexType))]
        public uint StateFlags { get => (uint)_s.GetInt32(0x04); set => _s.SetInt32(0x04, (int)value); }

        public byte AttackID { get => _s.GetByte(0x08); set => _s.SetByte(0x08, value); }

        [TypeConverter(typeof(HexType))]
        public uint AnimationCallBack { get => (uint)_s.GetInt32(0x0C); set => _s.SetInt32(0x0C, (int)value); }

        [TypeConverter(typeof(HexType))]
        public uint IASACallBack { get => (uint)_s.GetInt32(0x10); set => _s.SetInt32(0x10, (int)value); }

        [TypeConverter(typeof(HexType))]
        public uint PhysicsCallback { get => (uint)_s.GetInt32(0x14); set => _s.SetInt32(0x14, (int)value); }

        [TypeConverter(typeof(HexType))]
        public uint CollisionCallback { get => (uint)_s.GetInt32(0x18); set => _s.SetInt32(0x18, (int)value); }

        [TypeConverter(typeof(HexType))]
        public uint CameraCallback { get => (uint)_s.GetInt32(0x1C); set => _s.SetInt32(0x1C, (int)value); }

        public override string ToString()
        {
            return $"{AnimationID} {AttackID} {StateFlags.ToString("X8")}";
        }
    }
}
