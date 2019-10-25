using System;
using System.Collections.Generic;

namespace HSDRaw.Tools.Melee
{
    public class Bitreader
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

    public class MeleeCMDAction
    {
        public class Bitmapper
        {
            public string Name;
            public int Count;
            public bool Hex = false;
        }

        public string Name { get; internal set; }
        public byte Command { get; internal set; }
        public List<Bitmapper> BMap = new List<Bitmapper>();
        private int ParamCount = 0;

        public bool HasPointer { get; internal set; } = false;

        public int BitSize { get; internal set; }
        public int ByteSize { get => BitSize / 8; }

        public MeleeCMDAction(byte Command, string name, string Map)
        {
            Name = name;
            this.Command = Command;

            string[] args = Map.Split('|');
            
            BitSize = 6;
            if (args.Length > 1)
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
                    BitSize += map.Count;
                    BMap.Add(map);

                    if (!map.Name.Equals("None"))
                        ParamCount++;

                    if (map.Name.Equals("Pointer"))
                        HasPointer = true;
                }

            if (BitSize == 6)
                BitSize = 32;
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

            var data = o.Bytes.ToArray();

            if (data.Length % 8 != 0)
                throw new Exception("Subaction " + Command.ToString() + " was unsuccessfully compiled");

            return data;
        }
    }

    public class ActionCommon
    {
        public static string FunctionPrefix = "function_";

        public static List<MeleeCMDAction> SubActions { get; } = new List<MeleeCMDAction>()
        {
            new MeleeCMDAction(0x00, "EndOfScript", ""),
            new MeleeCMDAction(0x01, "SynchronousTimer", "Frame|26"),
            new MeleeCMDAction(0x02, "AsynchronousTimer", "Frame|26"),
            new MeleeCMDAction(0x03, "SetLoop", "Count|26" ),
            new MeleeCMDAction(0x04, "ExecuteLoop", ""),
            new MeleeCMDAction(0x05, "Subroutine", "Target|26|Pointer|x32"),
            new MeleeCMDAction(0x06, "Return", ""),
            new MeleeCMDAction(0x07, "GoTo", "Target|26|Pointer|x32"),
            new MeleeCMDAction(0x08, "SetTimerAnimation", ""),
            new MeleeCMDAction(0x09, "Unknown0x09", "Unknown|26"),
            new MeleeCMDAction(0x0A, "GraphicEffect", "Unk1|8|Unk2|18|GFXID|10|Boneid|6|Unk2|16|Z|16|Y|16|X|16|RangeZ|16|RangeY|16|RangeX|16"),
            new MeleeCMDAction(0x0B, "CreateHitbox", "ID|3|Unk1|5|Bone|7|Unk2|2|Damage|9|Size|16|Z|16|Y|16|X|16|Angle|9|KBG|9|WeightSetKB|9|Unk3|3|HitboxInteraction|2|BKB|9|Element|5|Unk4|1|ShieldDamage|7|SoundEffect|8|HurtboxInteraction|2"),
            new MeleeCMDAction(0x0C, "AdjustHitboxDamage", "HitboxID|3|Damage|23"),
            new MeleeCMDAction(0x0D, "AdjustHitboxSize", "HitboxID|3|Size|23"),
            new MeleeCMDAction(0x0E, "SetHitboxFlags", "HitboxID|24|Flags|2"),
            new MeleeCMDAction(0x0F, "RemoveHitbox", ""),
            new MeleeCMDAction(0x10, "ClearHitboxes", "None|26"), // Sound effect?
            new MeleeCMDAction(0x11, "SoundEffect", "Unk1|32|Unk2|6|SFX|x20|Offset|x32"),
            new MeleeCMDAction(0x12, "RandomSmashSFX", "Value|26" ),
            new MeleeCMDAction(0x13, "Autocancel", "Flag|2|Value|24" ),
            new MeleeCMDAction(0x14, "ReverseDirection", "" ),
            new MeleeCMDAction(0x15, "Unknown0x15", "Value|x26" ), // set flag
            new MeleeCMDAction(0x16, "Unknown0x16", "Value|x26" ), // set flag
            new MeleeCMDAction(0x17, "AllowInterrupt", "Value|x26" ),
            new MeleeCMDAction(0x18, "ProjectileFlag", "Value|x26" ),
            new MeleeCMDAction(0x19, "Unknown0x19", "Value|x26" ), // related to ground air state
            new MeleeCMDAction(0x1A, "SetBodyCollisionState", "Unknown|24|BodyState|2"),
            new MeleeCMDAction(0x1B, "BodyCollisionStatus", "Unknown|x26"),
            new MeleeCMDAction(0x1C, "SetBoneCollisionState", "BoneId|8|CollisionState|18"),
            new MeleeCMDAction(0x1D, "EnableJapFollowup", ""),
            new MeleeCMDAction(0x1E, "ToggleJabFollowUp", ""),
            new MeleeCMDAction(0x1F, "ChangleModelState", "StructID|6|Unknown|12|ObjectID|8"),
            new MeleeCMDAction(0x20, "RevertModels", ""),
            new MeleeCMDAction(0x21, "RemoveModels", ""),
            new MeleeCMDAction(0x22, "Throw", "Unknown|90"),
            new MeleeCMDAction(0x23, "HeldItemInvisibility", "Unknown|x25|Flag|1"),
            new MeleeCMDAction(0x24, "BodyArticleInvisibility", "Unknown|x25|Flag|1"),
            new MeleeCMDAction(0x25, "CharacterInvisibility", "Unknown|x25|Flag|1"),
            new MeleeCMDAction(0x26, "PseudoRandomSoundEffect", "Unknown|x218"),
            new MeleeCMDAction(0x27, "Unknwon0x27", "Unknown|x122"),
            new MeleeCMDAction(0x28, "AnimateTexture", "MaterialFlag|1|MaterialIndex|7|FrameFlags|7|Frame|11"),
            new MeleeCMDAction(0x29, "AnimateModel", "BodyPart|10|State|4|Unk|12"),
            new MeleeCMDAction(0x2A, "Unknown0x2A", "Unknown|26"),
            new MeleeCMDAction(0x2B, "Rumble", "Unknown|26"),
            new MeleeCMDAction(0x2C, "Unknown0x2A", "Unknown|25|Flag|1"), // set flag
            new MeleeCMDAction(0x2D, "Body Aura", "Unknown|8|Duration|18"),
            new MeleeCMDAction(0x2E, "RemoveColorOverlay", ""),
            new MeleeCMDAction(0x2F, "Unknown0x2A", "Unknwon|26"),
            new MeleeCMDAction(0x30, "SwordTrail", "BeamSword|1|Unknown|17|RenderStatus|8"),
            new MeleeCMDAction(0x31, "EnableRagdoll", "BoneID|26"),
            new MeleeCMDAction(0x32, "SelfDamage", "Unknown|10|Damage|16"),
            new MeleeCMDAction(0x33, "ContinuationControl", "Unknown|26"),
            new MeleeCMDAction(0x34, "Unknown0x34", "Unknown|26"), // set flag
            new MeleeCMDAction(0x35, "FootstepEffect", "Unknown|x58"),
            new MeleeCMDAction(0x36, "LandingEffect", "Unknown|x90"),
            new MeleeCMDAction(0x37, "StartSmashCharge", "Unknown|2|ChargeFrames|8|ChargeRate|16|VisualEffect|8|Unknown|24"),
            new MeleeCMDAction(0x38, "Unknown", "Unknown|26"),
            new MeleeCMDAction(0x39, "AestheticWindEffect", "Unknown|x122"),
            new MeleeCMDAction(0x3A, "Unknown0x3A", "Unknown|x26"),
        };

        /// <summary>
        /// 
        /// </summary>
        public static void PrintCommands()
        {
            foreach(var v in SubActions)
            {
                Console.WriteLine(v.Name + " " + v.BitSize + " " + v.ByteSize);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static MeleeCMDAction GetMeleeCMDAction(String name)
        {
            return SubActions.Find(e => e.Name.ToLower() == name.ToLower());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static MeleeCMDAction GetMeleeCMDAction(byte b)
        {
            return SubActions.Find(e => e.Command == b);
        }
    }
}
