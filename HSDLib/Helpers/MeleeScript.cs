using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDLib.Helpers
{
    public class MeleeScript
    {
        private static Dictionary<byte, int> CMD_SIZES = new Dictionary<byte, int>()
        {
            { 0x01, 0x04 },
            { 0x02, 0x04 },
            { 0x03, 0x04 },
            { 0x04, 0x04 },
            { 0x05, 0x08 },
            { 0x06, 0x04 },
            { 0x07, 0x08 },
            { 0x0A, 0x14 },
            { 0x0B, 0x14 },
            { 0x0F, 0x04 },
            { 0x10, 0x04 },
            { 0x11, 0x0C },
            { 0x12, 0x04 },
            { 0x13, 0x04 },
            { 0x14, 0x04 },
            { 0x17, 0x04 },
            { 0x1A, 0x04 },
            { 0x1B, 0x04 },
            { 0x1C, 0x04 },
            { 0x1F, 0x04 },
            { 0x22, 0x0C },
            { 0x29, 0x04 },
            { 0x2B, 0x04 },
            { 0x33, 0x04 },
            { 0x36, 0x0C },
            { 0x37, 0x0C },
            { 0x38, 0x08 },
            { 0x3A, 0x10 },
        };

        public static int GetCommandSize(byte flag)
        {
            if (CMD_SIZES.ContainsKey(flag))
                return CMD_SIZES[flag];
            return 4;
        }
    }
}
