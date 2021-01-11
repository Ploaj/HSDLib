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
    }
}
