using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MeleeLib.DAT.Script
{
    public enum CompileError
    {
        None,
        Syntax,
        ParameterCount,
        UnknownCommand
    }

    /*
     * 00 .Normal
04 .Fire
08 .Electric
0C .Slash
10 .Coin
14 .Ice
18 .Sleep
1C .Sleep
20 .Grounded
24 .Grounded
28 .Cape
2C .Empty (Gray hitbox that doesn't hit)
30 .Disabled (M2's disable, shieldbreak)
34 .Darkness
38 .Screw Attack (same effect like when you throw it at them)
3C .Poison/Flower
40 .Nothing (no graphic on hit)
     * */


    public class MeleeCMD
    {
        private class MeleeCMDAction
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
                for(int i =0; i < args.Length; i += 2)
                {
                    Bitmapper map = new Bitmapper();
                    if (args[i + 1].Contains("x"))
                    {
                        args[i + 1] = args[i + 1].Split('x')[1];
                        map.Hex = true;
                    }
                    map.Count = int.Parse(args[i+1]);
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
                    for(int j = 0; j < size; j++)
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
                    for(int j = 0; j < size; j++)
                    {
                        if (i / 8 >= Bytes.Count) Bytes.Add(0);
                        Bytes[i / 8] |= (byte)(((b>>(size-1-j)) & 0x1)<<(7-i%8));
                        i++;
                    }
                }
            }

            public string Decompile(byte[] b)
            {
                string o = "";
                Bitreader r = new Bitreader(b);
                r.Skip(6);
                for(int i = 0; i < BMap.Count; i++)
                {
                    if (BMap[i].Name.Contains("None")) continue;
                    o += BMap[i].Name + "=" + (BMap[i].Hex ? "0x"+r.Read(BMap[i].Count).ToString("x") : r.Read(BMap[i].Count).ToString());
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
                for(int i = 0; i < param.Length; i++)
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

        private static List<MeleeCMDAction> SubActions = new List<MeleeCMDAction>()
        {
            new MeleeCMDAction(0x01, "Frame|26"),
            new MeleeCMDAction(0x02, "Frame|26"),
            new MeleeCMDAction(0x0A, "Unk1|26|GFXID|10|Boneid|6|Unk2|16|Z|16|Y|16|X|16|RangeZ|16|RangeY|16|RangeX|16"),
            new MeleeCMDAction(0x0B, "ID|3|Unk1|5|Bone|7|Unk2|2|Damage|9|Size|16|Z|16|Y|16|X|16|Angle|9|KBG|9|WeightSetKB|9|Unk3|3|HitboxInteraction|2|BKB|9|Element|5|Unk4|1|ShieldDamage|7|SoundEffect|8|HurtboxInteraction|2"),
            new MeleeCMDAction(0x10, "None|26"),
            new MeleeCMDAction(0x11, "Unk1|32|Unk2|4|SFX|x20|SomeOffset|x32"),
            new MeleeCMDAction(0x17, "None|26"),
            new MeleeCMDAction(0x29, "BodyPart|10|State|4|Unk|12"),
            new MeleeCMDAction(0x33, "Percent|33"),
        };

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
            foreach(byte b in CMD_NAMES.Keys)
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


        private static MeleeCMDAction GetMeleeCMDAction(byte b)
        {
            foreach (MeleeCMDAction a in SubActions)
            {
                if (a.Command == b) return a;
            }
            return null;
        }

        public static string DecompileSubAction(SubAction action)
        {
            string o = GetActionName((byte)(action.Data[0] >> 2)) + "(";

            MeleeCMDAction act = GetMeleeCMDAction((byte)(action.Data[0] >> 2));

            if (act != null)
            {
                o += act.Decompile(action.Data);
            }
            else
            {
                for (int i = 0; i < action.Data.Length; i++)
                {
                    if (i == 0)
                    {
                        o += "0x" + (action.Data[i] & 0x3).ToString("x");
                    }
                    else
                    {
                        o += ",0x" + action.Data[i].ToString("x");
                    }
                }
            }

            o += ");";
            return o;
        }

        public static SubAction CompileCommand(string cmd, out CompileError ErrorCode)
        {
            SubAction a = new SubAction();
            ErrorCode = CompileError.None;

            if (Regex.Match(cmd, "[A-z,0-9]+\\((([A-F,a-f,0-9]+,)|((0x)?[A-F,a-f,0-9]*)|([A-z,0-9]+=((0x)?[A-F,a-f,0-9]+)))*?\\);").Success)
            {
                string CommandName = cmd.Split('(')[0];
                string[] parameters = cmd.Split('(')[1].Split(')')[0].Split(',');
                byte flag = GetFlag(CommandName);
                
                MeleeCMDAction action = GetMeleeCMDAction(flag);
                if(action != null)
                {
                    byte[] data = action.Compile(parameters);
                    if(data == null)
                    {
                        ErrorCode = CompileError.ParameterCount;
                        return null;
                    }
                    a.Data = data;
                }
                else
                {
                    a.Data = new byte[parameters.Length];
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        byte b = Convert.ToByte(parameters[i].Split('x')[1], 16);
                        if (i == 0)
                        {
                            b |= (byte)(flag<<2);
                        }
                        a.Data[i] = b;
                    }
                }
            }
            else
            {
                ErrorCode = CompileError.Syntax;
            }

            return a;
        }
    }
}
