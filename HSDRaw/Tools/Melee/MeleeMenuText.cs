using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HSDRaw.Tools.Melee
{
    public enum TEXT_OP_CODE
    {
        END = 0x00,
        RESET = 0x01,
        UNKNOWN_02 = 0x02,
        LINE_BREAK = 0x03,
        UNKNOWN_04 = 0x04,
        UNKNOWN_05 = 0x05,
        UNKNOWN_06 = 0x06,
        OFFSET = 0x07,
        UNKNOWN_08 = 0x08,
        UNKNOWN_09 = 0x09,
        SCALING = 0x0A,
        RESET_SCALING = 0x0B,
        COLOR = 0x0C,
        CLEAR_COLOR = 0x0D,
        SET_TEXTBOX = 0x0E,
        RESET_TEXTBOX = 0x0F,
        CENTERED = 0x10,
        RESET_CENTERED = 0x11,
        LEFT_ALIGNED = 0x12,
        RESET_LEFT_ALIGN = 0x13,
        RIGHT_ALIGNED = 0x14,
        RESET_RIGHT_ALIGN = 0x15,
        KERNING = 0x16,
        NO_KERNING = 0x17,
        FITTING = 0x18,
        NO_FITTING = 0x19,
        SPACE = 0x1A,
        COMMON_CHARACTER = 0x20,
        SPECIAL_CHARACTER = 0x40
    }

    /// <summary>
    /// 
    /// </summary>
    public class MeleeMenuText
    {
        public byte[] Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }
        private byte[] _data = new byte[] { 0 };

        public List<Tuple<TEXT_OP_CODE, ushort[]>> OPCodes { get => DeserializeCodes(_data); }

        public static Dictionary<TEXT_OP_CODE, Tuple<string, string>> CODES = new Dictionary<TEXT_OP_CODE, Tuple<string, string>>()
            {
                { TEXT_OP_CODE.CENTERED, new Tuple<string, string>("CENTER", "") },
                { TEXT_OP_CODE.RESET_CENTERED, new Tuple<string, string>("/CENTER", "") },
                { TEXT_OP_CODE.CLEAR_COLOR, new Tuple<string, string>("/COLOR", "") },
                { TEXT_OP_CODE.COLOR, new Tuple<string, string>("COLOR", "bbb") },
                { TEXT_OP_CODE.END, new Tuple<string, string>("END", "") },
                { TEXT_OP_CODE.FITTING, new Tuple<string, string>("FIT", "") },
                { TEXT_OP_CODE.KERNING, new Tuple<string, string>("KERN", "") },
                { TEXT_OP_CODE.LEFT_ALIGNED, new Tuple<string, string>("LEFT", "") },
                { TEXT_OP_CODE.LINE_BREAK, new Tuple<string, string>("BR", "") },
                { TEXT_OP_CODE.NO_FITTING, new Tuple<string, string>("/FIT", "") },
                { TEXT_OP_CODE.NO_KERNING, new Tuple<string, string>("/KERN", "") },
                { TEXT_OP_CODE.OFFSET, new Tuple<string, string>("OFFSET", "ss") },
                { TEXT_OP_CODE.RESET, new Tuple<string, string>("RESET", "") },
                { TEXT_OP_CODE.RESET_LEFT_ALIGN, new Tuple<string, string>("/LEFT", "") },
                { TEXT_OP_CODE.RESET_RIGHT_ALIGN, new Tuple<string, string>("/RIGHT", "") },
                { TEXT_OP_CODE.RESET_SCALING, new Tuple<string, string>("/SCALE", "") },
                { TEXT_OP_CODE.RESET_TEXTBOX, new Tuple<string, string>("/TEXTBOX", "") },
                { TEXT_OP_CODE.RIGHT_ALIGNED, new Tuple<string, string>("/RIGHT", "") },
                { TEXT_OP_CODE.SCALING, new Tuple<string, string>("SCALE", "bbbb") },
                { TEXT_OP_CODE.SET_TEXTBOX, new Tuple<string, string>("TEXTBOX", "ss") },
                { TEXT_OP_CODE.UNKNOWN_02, new Tuple<string, string>("UNK02", "") },
                { TEXT_OP_CODE.UNKNOWN_04, new Tuple<string, string>("UNK04", "") },
                { TEXT_OP_CODE.UNKNOWN_05, new Tuple<string, string>("UNK05", "s") },
                { TEXT_OP_CODE.UNKNOWN_06, new Tuple<string, string>("UNK06", "ss") },
                { TEXT_OP_CODE.UNKNOWN_08, new Tuple<string, string>("UNK08", "") },
                { TEXT_OP_CODE.UNKNOWN_09, new Tuple<string, string>("UNK09", "") },
                { TEXT_OP_CODE.SPACE, new Tuple<string, string>("S", "") },
            };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<Tuple<TEXT_OP_CODE, ushort[]>> DeserializeCodes(byte[] data)
        {
            List<Tuple<TEXT_OP_CODE, ushort[]>> d = new List<Tuple<TEXT_OP_CODE, ushort[]>>();

            for (int i = 0; i < data.Length;)
            {
                var opcode = (TEXT_OP_CODE)data[i++];
                ushort[] param = new ushort[0];

                int textCode = (byte)opcode;

                if ((textCode >> 4) == 2)
                    param = new ushort[] { (ushort)(((textCode << 8) | (data[i++] & 0xFF)) & 0xFFF) };
                else
                if ((textCode >> 4) == 4)
                    param = new ushort[] { (ushort)(((textCode << 8) | (data[i++] & 0xFF)) & 0xFFF) };
                else
                if (!CODES.ContainsKey(opcode))
                    throw new Exception("Opcode Not Supported " + opcode.ToString("X"));
                else
                {
                    var code = CODES[opcode];
                    var p = code.Item2.ToCharArray();
                    param = new ushort[p.Length];
                    for (int j = 0; j < param.Length; j++)
                    {
                        switch (p[j])
                        {
                            case 'b':
                                param[j] = (ushort)(data[i++] & 0xFF);
                                break;
                            case 's':
                                param[j] = (ushort)(((data[i++] & 0xFF) << 8) | (data[i++] & 0xFF));
                                break;
                        }
                    }
                }

                Tuple<TEXT_OP_CODE, ushort[]> c = new Tuple<TEXT_OP_CODE, ushort[]>(opcode, param);
                d.Add(c);

                if (opcode == TEXT_OP_CODE.END)
                    break;
            }

            return d;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string DeserializeString(byte[] data)
        {
            StringBuilder sb = new StringBuilder();

            var codes = DeserializeCodes(data);

            foreach (var d in codes)
            {
                if (CODES.ContainsKey(d.Item1))
                {
                    var code = CODES[d.Item1];
                    sb.Append("<" + code.Item1 + (d.Item2.Length > 0 ? ", " : "") + string.Join(", ", d.Item2) + ">");
                }
                else
                {
                    if (d.Item1 == TEXT_OP_CODE.SPECIAL_CHARACTER)
                        sb.Append("<" + "CHR" + ", " + d.Item2[0] + ">");

                    if (d.Item1 == TEXT_OP_CODE.COMMON_CHARACTER)
                        sb.Append(CharMAP[d.Item2[0]]);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        public bool FromString(string str)
        {
            var matches = System.Text.RegularExpressions.Regex.Matches(str, @"((?<=<).+?(?=>))|((?<=>*)([^>]+?)(?=<))");

            List<byte> newData = new List<byte>();

            foreach (var m in matches)
            {
                var p = m.ToString().Split(',');

                if (p.Length == 0)
                    continue;

                // check if p is a code
                var key = CODES.FirstOrDefault(x => x.Value.Item1 == p[0]);

                if (key.Value != null)
                {
                    if (p.Length - 1 != key.Value.Item2.Length)
                        return false;

                    newData.Add((byte)key.Key);

                    for (int j = 0; j < key.Value.Item2.Length; j++)
                    {
                        switch (key.Value.Item2[j])
                        {
                            case 'b':
                                if (byte.TryParse(p[j + 1].Trim(), out byte res))
                                    newData.Add(res);
                                else
                                    newData.Add(0);
                                break;
                            case 's':
                                if (ushort.TryParse(p[j + 1].Trim(), out ushort sht))
                                {
                                    newData.Add((byte)(sht >> 8));
                                    newData.Add((byte)(sht & 0xFF));
                                }
                                else
                                {
                                    newData.Add(0);
                                    newData.Add(0);
                                }
                                break;
                        }
                    }
                }
                else
                {
                    // process string otherwise

                    if (p.Length >= 2 && p[0] == "CHR")
                    {
                        if (ushort.TryParse(p[1].Trim(), out ushort ch))
                        {
                            ushort sht = (ushort)(((ushort)TEXT_OP_CODE.SPECIAL_CHARACTER << 8) | ch);
                            newData.Add((byte)(sht >> 8));
                            newData.Add((byte)(sht & 0xFF));
                        }
                    }
                    else
                        foreach (var chr in p[0])
                        {
                            var index = CharMAP.IndexOf(chr);
                            if (index != -1)
                            {
                                ushort sht = (ushort)(((ushort)TEXT_OP_CODE.COMMON_CHARACTER << 8) | index);
                                newData.Add((byte)(sht >> 8));
                                newData.Add((byte)(sht & 0xFF));
                            }
                            else
                                return false;
                        }
                }


            }

            _data = newData.ToArray();

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return DeserializeString(_data);
        }


        /// <summary>
        /// 
        /// </summary>
        public static List<char> CharMAP = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'ぁ', 'あ', 'ぃ', 'い', 'ぅ', 'う', 'ぇ', 'え', 'ぉ', 'お', 'か', 'が', 'き', 'ぎ', 'く', 'ぐ', 'け', 'げ', 'こ', 'ご', 'さ', 'ざ', 'し', 'じ', 'す', 'ず', 'せ', 'ぜ', 'そ', 'ぞ', 'た', 'だ', 'ち', 'ぢ', 'っ', 'つ', 'づ', 'て', 'で', 'と', 'ど', 'な', 'に', 'ぬ', 'ね', 'の', 'は', 'ば', 'ぱ', 'ひ', 'び', 'ぴ', 'ふ', 'ぶ', 'ぷ', 'へ', 'べ', 'ぺ', 'ほ', 'ぼ', 'ぽ', 'ま', 'み', 'む', 'め', 'も', 'ゃ', 'や', 'ゅ', 'ゆ', 'ょ', 'よ', 'ら', 'り', 'る', 'れ', 'ろ', 'ゎ', 'わ', 'を', 'ん', 'ァ', 'ア', 'ィ', 'イ', 'ゥ', 'ウ', 'ェ', 'エ', 'ォ', 'オ', 'カ', 'ガ', 'キ', 'ギ', 'ク', 'グ', 'ケ', 'ゲ', 'コ', 'ゴ', 'サ', 'ザ', 'シ', 'ジ', 'ス', 'ズ', 'セ', 'ゼ', 'ソ', 'ゾ', 'タ', 'ダ', 'チ', 'ヂ', 'ッ', 'ツ', 'ヅ', 'テ', 'デ', 'ト', 'ド', 'ナ', 'ニ', 'ヌ', 'ネ', 'ノ', 'ハ', 'バ', 'パ', 'ヒ', 'ビ', 'ピ', 'フ', 'ブ', 'プ', 'ヘ', 'ベ', 'ペ', 'ホ', 'ボ', 'ポ', 'マ', 'ミ', 'ム', 'メ', 'モ', 'ャ', 'ヤ', 'ュ', 'ユ', 'ョ', 'ヨ', 'ラ', 'リ', 'ル', 'レ', 'ロ', 'ヮ', 'ワ', 'ヲ', 'ン', 'ヴ', 'ヵ', 'ヶ', '　', '、', '。', ',', '.', '•', ',', ';', '?', '!', '^', '_', '—', '/', '~', '|', '\'', '"', '(', ')', '[', ']', '{', '}', '+', '-', '×', '=', '<', '>', '¥', '$', '%', '#', '&', '*', '@', '扱', '押', '軍', '源', '個', '込', '指', '示', '取', '書', '詳', '人', '生', '説', '体', '団', '電', '読', '発', '抜', '閑', '本', '明' };

    }
}
