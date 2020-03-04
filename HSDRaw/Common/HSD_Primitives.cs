using System.Text;

namespace HSDRaw.Common
{
    public class HSD_String : HSDAccessor
    {
        public string Value
        {
            get
            {
                var nullpoint = 0;
                foreach (var d in _s.GetData())
                    if (d == 0)
                        break;
                    else
                        nullpoint++;
                return Encoding.UTF8.GetString(_s.GetData(), 0, nullpoint);
            }
            set
            {
                _s.SetData(Encoding.UTF8.GetBytes(value));
                _s.Resize(_s.Length + 1);
                if (_s.Length % 4 != 0)
                    _s.Resize(_s.Length + (4 - (_s.Length % 4)));
            }
        }

        public HSD_String() : base()
        {

        }

        public override void New()
        {
            base.New();
            _s.Resize(0x04);
        }

        public override string ToString()
        {
            return Value;
        }
    }

    public class HSD_Vector3 : HSDAccessor
    {
        public override int TrimmedSize => 0x0C;

        public float X { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }

        public float Y { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public float Z { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }
    }

    public class HSD_UShort : HSDAccessor
    {
        public override int TrimmedSize => 0x02;

        public ushort Value { get => (ushort)_s.GetInt16(0x00); set => _s.SetInt16(0x00, (short)value); }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class HSD_Byte : HSDAccessor
    {
        public override int TrimmedSize => 0x01;

        public byte Value { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }
    }

    public class HSD_UInt : HSDAccessor
    {
        public override int TrimmedSize => 0x04;

        public uint Value { get => (uint)_s.GetInt32(0x00); set => _s.SetInt32(0x00, (int)value); }
    }

    public class HSD_Int : HSDAccessor
    {
        public override int TrimmedSize => 0x04;

        public int Value { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }
    }

    public class HSD_Float : HSDAccessor
    {
        public override int TrimmedSize => 0x04;

        public float Value { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }
    }
}
