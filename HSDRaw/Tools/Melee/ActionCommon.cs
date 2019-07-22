using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HSDRaw.Tools.Melee
{
    public class MeleeCMDAction
    {
        public class Bitmapper
        {
            public int Count;
            public string Name;
            public bool Hex = false;
        }
        public List<Bitmapper> BMap = new List<Bitmapper>();
        public byte Command;
        private int ParamCount = 0;

        public MeleeCMDAction(byte Command, string Map)
        {
            this.Command = Command;
            string[] args = Map.Split('|');
            for (int i = 0; i < args.Length; i += 2)
            {
                Bitmapper map = new Bitmapper();
                if (args[i + 1].Contains("x"))
                {
                    args[i + 1] = args[i + 1].Split('x')[1];
                    map.Hex = true;
                }
                map.Count = int.Parse(args[i + 1]);
                map.Name = args[i];
                BMap.Add(map);
                if (!map.Name.Equals("None")) ParamCount++;
            }
        }

        private class Bitreader
        {
            private byte[] bytes;
            private int i = 0;
            public Bitreader(byte[] b)
            {
                this.bytes = b;
            }
            public int Read(int size)
            {
                int o = 0;
                for (int j = 0; j < size; j++)
                {
                    o |= ((bytes[i / 8] >> (7 - (i % 8))) & 0x1) << (size - j - 1);
                    i++;
                }
                return o;
            }
            public void Skip(int size)
            {
                i += size;
            }
        }

        public class BitWriter
        {
            public List<byte> Bytes = new List<byte>();
            int i = 0;
            public void Write(int b, int size)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i / 8 >= Bytes.Count) Bytes.Add(0);
                    Bytes[i / 8] |= (byte)(((b >> (size - 1 - j)) & 0x1) << (7 - i % 8));
                    i++;
                }
            }
        }

        public string Decompile(byte[] b)
        {
            string o = "";
            Bitreader r = new Bitreader(b);
            r.Skip(6);
            for (int i = 0; i < BMap.Count; i++)
            {
                if (BMap[i].Name.Contains("None")) continue;
                o += BMap[i].Name + "=" + (BMap[i].Hex ? "0x" + r.Read(BMap[i].Count).ToString("x") : r.Read(BMap[i].Count).ToString());
                if (i != BMap.Count - 1)
                    o += ",";
            }
            return o;
        }

        public byte[] Compile(string[] param)
        {
            BitWriter o = new BitWriter();
            o.Write(Command, 6);

            if (param.Length != ParamCount && param.Length != 1) return null;
            for (int i = 0; i < param.Length; i++)
            {
                int v = 0;
                if (param[i].Length < 1) continue;
                string val = param[i].Split('=')[1];
                if (val.Contains("x"))
                    v = Convert.ToInt32(val.Split('x')[1], 16);
                else
                    v = int.Parse(val);
                o.Write(v, BMap[i].Count);
            }

            return o.Bytes.ToArray();
        }
    }

    public class ActionCommon
    {
        public static string FunctionPrefix = "function_";

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
            { 0x26, 0x1C }, //?? used with hurt spinny animation
            { 0x27, 0x10 }, //?? used with ness up b hit
            { 0x29, 0x04 },
            { 0x2B, 0x04 },
            { 0x33, 0x04 },
            { 0x36, 0x0C },
            { 0x37, 0x0C },
            { 0x38, 0x08 },
            { 0x3A, 0x10 },
        };

        private static Dictionary<byte, string> CMD_NAMES = new Dictionary<byte, string>()
        {
            { 0x01, "SynchronousTimer" },
            { 0x02, "AsynchronousTimer" },
            { 0x03, "SetLoop" },
            { 0x04, "ExecuteLoop" },
            { 0x05, "Goto" },
            { 0x06, "Return" },
            { 0x07, "Subroutine" },
            { 0x0A, "GraphicEffect" },
            { 0x0B, "Hitbox" },
            { 0x0F, "RemoveHitbox" },
            { 0x10, "ClearHitboxes" },
            { 0x11, "SoundEffect" },
            { 0x12, "RandomSoundEffect" },
            { 0x13, "Autocancel" },
            { 0x14, "ReverseDirection" },
            { 0x17, "IASA" },
            { 0x1A, "SetBodyState" },
            { 0x1B, "SetAllBones" },
            { 0x1C, "SetBoneAt" },
            { 0x1F, "ModelMod" },
            { 0x22, "Throw" },
            { 0x29, "AnimateBodypart" },
            { 0x2B, "GenerateArticle" },
            { 0x33, "SelfDamage" },
            { 0x38, "StartSmashCharge" },
            { 0x3A, "Unknown0x3A" },
        };

        public static int GetSize(byte flag)
        {
            if (CMD_SIZES.ContainsKey(flag))
                return CMD_SIZES[flag];
            return 4;
        }

        public static string GetActionName(byte flag)
        {
            if (CMD_NAMES.ContainsKey(flag))
                return CMD_NAMES[flag];
            return "Unknown0x" + flag.ToString("x");
        }

        public static byte GetFlag(string s)
        {
            foreach (byte b in CMD_NAMES.Keys)
            {
                if (CMD_NAMES[b].Equals(s))
                {
                    return b;
                }
            }
            if (Regex.Match(s, "Unknown0x[0-9]*").Success)
            {
                return (byte)Convert.ToUInt32(s.Split('x')[1], 16);
            }
            return 0;
        }

        private static List<MeleeCMDAction> SubActions = new List<MeleeCMDAction>()
        {
            new MeleeCMDAction(0x01, "Frame|26"),
            new MeleeCMDAction(0x02, "Frame|26"),
            new MeleeCMDAction(0x03, "Count|26" ),
            new MeleeCMDAction(0x04, "None|26"), // return
            new MeleeCMDAction(0x06, "None|26"),// Execute loop
            new MeleeCMDAction(0x0A, "Unk1|8|Unk2|18|GFXID|10|Boneid|6|Unk2|16|Z|16|Y|16|X|16|RangeZ|16|RangeY|16|RangeX|16"),
            new MeleeCMDAction(0x0B, "ID|3|Unk1|5|Bone|7|Unk2|2|Damage|9|Size|16|Z|16|Y|16|X|16|Angle|9|KBG|9|WeightSetKB|9|Unk3|3|HitboxInteraction|2|BKB|9|Element|5|Unk4|1|ShieldDamage|7|SoundEffect|8|HurtboxInteraction|2"),
            new MeleeCMDAction(0x10, "None|26"),
            new MeleeCMDAction(0x11, "Unk1|32|Unk2|4|SFX|x20|SomeOffset|x32"),
            new MeleeCMDAction(0x13, "Flag|2|Value|24" ),
            new MeleeCMDAction(0x17, "None|26"),
            new MeleeCMDAction(0x1A, "State|26"),
            new MeleeCMDAction(0x29, "BodyPart|10|State|4|Unk|12"),
            new MeleeCMDAction(0x33, "Percent|33"),
        };

        public static MeleeCMDAction GetMeleeCMDAction(byte b)
        {
            foreach (MeleeCMDAction a in SubActions)
            {
                if (a.Command == b) return a;
            }
            return null;
        }
    }
}
