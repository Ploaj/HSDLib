using System.ComponentModel;

namespace HSDRaw.MEX
{
    public class MEX_MoveLogic : HSDAccessor
    {
        public override int TrimmedSize => 0x20;

        public int MoveID { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public int StateFlags { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public byte AttackID { get => _s.GetByte(0x08); set => _s.SetByte(0x08, value); }

        [TypeConverter(typeof(HexType))]
        public int AnimationCallBack { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

        [TypeConverter(typeof(HexType))]
        public int IASACallBack { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        [TypeConverter(typeof(HexType))]
        public int PhysicsCallback { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }

        [TypeConverter(typeof(HexType))]
        public int CollisionCallback { get => _s.GetInt32(0x18); set => _s.SetInt32(0x18, value); }

        [TypeConverter(typeof(HexType))]
        public int CameraCallback { get => _s.GetInt32(0x1C); set => _s.SetInt32(0x1C, value); }

    }
}
