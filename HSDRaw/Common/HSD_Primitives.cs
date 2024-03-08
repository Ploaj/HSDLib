using System.Collections.Generic;
using System.Text;

namespace HSDRaw.Common
{
    public class HSD_ShiftJIS_String : HSDAccessor
    {
        private static readonly Encoding SHIFT_JIS = Encoding.GetEncoding("Shift_JIS");

        private static readonly Dictionary<char, char> CharToShiftChar = new Dictionary<char, char>()
            {
                { '+' ,'＋' },
                { '-' ,'－' },
                { '=' ,'＝' },
                { '?' ,'？' },
                { '!' ,'！' },
                { '@' ,'＠' },
                { '%' ,'％' },
                { '&' ,'＆' },
                { '$' ,'＄' },
                { ',' ,'，' },
                { '•' ,'・' },
                { ';' ,'；' },
                { ':' ,'：' },
                { '^' ,'＾' },
                { '_' ,'＿' },
                { '—' ,'ー' },
                { '~' ,'～' },
                { '/' ,'／' },
                { '|' ,'｜' },
                { '\\' ,'＼' },
                { '"' ,'“' },
                { '(' ,'（' },
                { ')' ,'）' },
                { '[' ,'［' },
                { ']' ,'］' },
                { '{' ,'｛' },
                { '}' ,'｝' },
                { '<' ,'〈' },
                { '>' ,'〉' },
                { '¥' ,'￥' },
                { '#' ,'＃' },
                { '*' ,'＊' },
                { '\'', '’'}
            };

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
                return SHIFT_JIS.GetString(_s.GetData(), 0, nullpoint);
            }
            set
            {
                // shift-jis stylize string and validate
                var chars = value.ToCharArray();
                for (int i = 0; i < chars.Length; i++)
                {
                    // convert to shift jis
                    if (CharToShiftChar.ContainsKey(chars[i]))
                        chars[i] = CharToShiftChar[chars[i]];
                }

                // set data
                _s.SetData(SHIFT_JIS.GetBytes(new string(chars)));

                // null terminated
                _s.Resize(_s.Length + 1);
                
                if (_s.Length % 4 != 0)
                    _s.Resize(_s.Length + (4 - (_s.Length % 4)));
            }
        }

        public HSD_ShiftJIS_String() : base()
        {
        }

        public HSD_ShiftJIS_String(string value) : base()
        {
            Value = value;
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
                if(value != null)
                {
                    _s.SetData(Encoding.UTF8.GetBytes(value));
                    _s.Resize(_s.Length + 1);
                    if (_s.Length % 4 != 0)
                        _s.Resize(_s.Length + (4 - (_s.Length % 4)));
                }
            }
        }

        public HSD_String() : base()
        {

        }

        public HSD_String(string value) : base()
        {
            Value = value;
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

        public float Length
        {
            get
            {
                return (float)System.Math.Sqrt(X * X + Y * Y + Z * Z);
            }
        }

        public HSD_Vector3()
        {

        }

        public HSD_Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Adds two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static HSD_Vector3 operator +(HSD_Vector3 left, HSD_Vector3 right)
        {
            return new HSD_Vector3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static HSD_Vector3 operator -(HSD_Vector3 left, HSD_Vector3 right)
        {
            return new HSD_Vector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="vec">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>The result of the calculation.</returns>
        public static HSD_Vector3 operator *(HSD_Vector3 vec, float scale)
        {
            return new HSD_Vector3(vec.X * scale, vec.Y * scale, vec.Z * scale);
        }

        /// <summary>
        /// Divides an instance by a scalar.
        /// </summary>
        /// <param name="vec">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>The result of the calculation.</returns>
        public static HSD_Vector3 operator /(HSD_Vector3 vec, float scale)
        {
            return vec * (1.0f / scale);
        }

        /// <summary>
        /// Scales the Vector3 to unit length.
        /// </summary>
        public void Normalize()
        {
            float scale = 1.0f / Length;
            X *= scale;
            Y *= scale;
            Z *= scale;
        }
    }
}
