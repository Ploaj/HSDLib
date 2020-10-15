using HSDRaw.Common.Animation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HSDRaw.MEX
{
    [Flags]
    public enum HSD_BUTTON
    {
        DPAD_LEFT = 0x0001,
        DPAD_RIGHT = 0x0002,
        DPAD_DOWN = 0x0004,
        DPAD_UP = 0x0008,
        TRIGGER_Z = 0x0010,
        TRIGGER_R = 0x0020,
        TRIGGER_L = 0x0040,
        BUTTON_A = 0x0100,
        BUTTON_B = 0x0200,
        BUTTON_X = 0x0400,
        BUTTON_Y = 0x0800,
        BUTTON_START = 0x1000,
        BUTTON_UP = 0x10000,
        BUTTON_DOWN = 0x20000,
        BUTTON_LEFT = 0x40000,
        BUTTON_RIGHT = 0x80000,
    }

    public class MEX_Stock : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public short Reserved { get => _s.GetInt16(0x00); set => _s.SetInt16(0x00, value); }

        public short Stride { get => _s.GetInt16(0x02); set => _s.SetInt16(0x02, value); }

        public HSD_MatAnimJoint MatAnimJoint { get => _s.GetReference<HSD_MatAnimJoint>(0x04); set => _s.SetReference(0x04, value); }

        public int CustomStockLength { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public HSDArrayAccessor<MEX_StockEgg> CustomStockEntries { get => _s.GetReference<HSDArrayAccessor<MEX_StockEgg>>(0x0C); set => _s.SetReference(0x0C, value); }
    }

    public class MEX_StockEgg : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        private static readonly Encoding SHIFT_JIS = Encoding.GetEncoding("Shift_JIS");

        public byte ReserveredStockIndex { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }

        public HSD_BUTTON Input { get => (HSD_BUTTON)(_s.GetInt32(0x00) & 0xFFFFFF); set => _s.SetInt32(0x00, (ReserveredStockIndex << 24) | ((int)value & 0xFFFFFF)); }
        
        public string Name
        {
            get
            {
                var data = _s.GetReference<HSDAccessor>(0x04);

                if(data != null)
                {
                    return new string(SHIFT_JIS.GetChars(data._s.GetData()));
                }

                return "";
            }
            set
            {
                if(string.IsNullOrEmpty(value))
                {
                    _s.SetReference(0x04, null);
                }
                else
                {
                    // max of 8 chars
                    if (value.Length > 8)
                        value = value.Substring(0, 8);

                    // shift-jis stylize string and validate
                    var chars = value.ToCharArray();
                    for(int i = 0; i < chars.Length; i++)
                    {
                        // convert to shift jis
                        if (CharToShiftChar.ContainsKey(chars[i]))
                            chars[i] = CharToShiftChar[chars[i]];

                        // TODO: validate char
                    }

                    // set data
                    var s = _s.GetCreateReference<HSDAccessor>(0x04)._s;
                    s.SetData(SHIFT_JIS.GetBytes(new string(chars)));

                    // null terminated
                    s.Resize(s.Length + 1);
                }
            }
        }

        private static Dictionary<char, char> CharToShiftChar = new Dictionary<char, char>()
            {
                { ' ' ,' ' },
                { '0' ,'０' },
                { '1' ,'１' },
                { '2' ,'２' },
                { '3' ,'３' },
                { '4' ,'４' },
                { '5' ,'５' },
                { '6' ,'６' },
                { '7' ,'７' },
                { '8' ,'８' },
                { '9' ,'９' },
                { 'A' ,'Ａ' },
                { 'B' ,'Ｂ' },
                { 'C' ,'Ｃ' },
                { 'D' ,'Ｄ' },
                { 'E' ,'Ｅ' },
                { 'F' ,'Ｆ' },
                { 'G' ,'Ｇ' },
                { 'H' ,'Ｈ' },
                { 'I' ,'Ｉ' },
                { 'J' ,'Ｊ' },
                { 'K' ,'Ｋ' },
                { 'L' ,'Ｌ' },
                { 'M' ,'Ｍ' },
                { 'N' ,'Ｎ' },
                { 'O' ,'Ｏ' },
                { 'P' ,'Ｐ' },
                { 'Q' ,'Ｑ' },
                { 'R' ,'Ｒ' },
                { 'S' ,'Ｓ' },
                { 'T' ,'Ｔ' },
                { 'U' ,'Ｕ' },
                { 'V' ,'Ｖ' },
                { 'W' ,'Ｗ' },
                { 'X' ,'Ｘ' },
                { 'Y' ,'Ｙ' },
                { 'Z' ,'Ｚ' },
                { 'a' ,'Ａ' },
                { 'b' ,'Ｂ' },
                { 'c' ,'Ｃ' },
                { 'd' ,'Ｄ' },
                { 'e' ,'Ｅ' },
                { 'f' ,'Ｆ' },
                { 'g' ,'Ｇ' },
                { 'h' ,'Ｈ' },
                { 'i' ,'Ｉ' },
                { 'j' ,'Ｊ' },
                { 'k' ,'Ｋ' },
                { 'l' ,'Ｌ' },
                { 'm' ,'Ｍ' },
                { 'n' ,'Ｎ' },
                { 'o' ,'Ｏ' },
                { 'p' ,'Ｐ' },
                { 'q' ,'Ｑ' },
                { 'r' ,'Ｒ' },
                { 's' ,'Ｓ' },
                { 't' ,'Ｔ' },
                { 'u' ,'Ｕ' },
                { 'v' ,'Ｖ' },
                { 'w' ,'Ｗ' },
                { 'x' ,'Ｘ' },
                { 'y' ,'Ｙ' },
                { 'z' ,'Ｚ' },
                { '+' ,'＋' },
                { '-' ,'－' },
                { '=' ,'＝' },
                { '?' ,'？' },
                { '!' ,'！' },
                { '@' ,'＠' },
                { '%' ,'％' },
                { '&' ,'＆' },
                { '$' ,'＄' },
            };

        public override string ToString()
        {
            return Name;
        }

    }
}
