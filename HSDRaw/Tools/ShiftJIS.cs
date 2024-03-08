using System.Collections.Generic;
using System.Text;

namespace HSDRaw.Tools
{
    internal class ShiftJIS
    {
        public static byte[] ToShiftJIS(string input)
        {
            List<byte> output = new List<byte>();

            foreach (char c in input)
            {
                if (c == ' ')
                {
                    output.Add(0x81);
                    output.Add(0x40);
                }
                else if ((c < '0' || '9' < c))
                {
                    if ((c < 'A' || 'Z' < c))
                    {
                        if ((c < 'a' || 'z' < c))
                        {
                            output.Add(0x81);
                            output.Add(0x44);
                        }
                        else
                        {
                            output.Add(0x82);
                            output.Add((byte)(c + ' '));
                        }
                    }
                    else
                    {
                        output.Add(0x82);
                        output.Add((byte)(c + '\x1f'));
                    }
                }
                else
                {
                    output.Add(0x82);
                    output.Add((byte)(c + '\x1f'));
                }
            }

            output.Add(0);

            return output.ToArray();
        }

        public static string ToUnicode(byte[] inputBytes)
        {
            StringBuilder output = new StringBuilder();

            for (int i = 0; i < inputBytes.Length; i += 2)
            {
                byte firstByte = inputBytes[i];
                byte secondByte = i + 1 < inputBytes.Length ? inputBytes[i + 1] : (byte)0;

                if (firstByte == 0x00)
                {
                    break;
                }
                else if (firstByte == 0x81 && secondByte == 0x40)
                {
                    output.Append(' ');
                }
                else if (firstByte == 0x81 && secondByte == 0x44)
                {
                    // Handle special case
                }
                else if (firstByte == 0x82)
                {
                    char secondChar = (char)(secondByte - '\x1f');
                    output.Append(secondChar);
                }
                else
                {
                    char originalChar = (char)(secondByte - '\x1f');
                    output.Append(originalChar);
                }
            }

            return output.ToString();
        }
    }
}
