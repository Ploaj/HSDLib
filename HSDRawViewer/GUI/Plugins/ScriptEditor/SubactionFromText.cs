using HSDRawViewer.Tools;
using IONET.Collada.Core.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HSDRawViewer.GUI.Plugins.ScriptEditor
{
    public class SubactionFromText
    {
        public enum HitboxParam
        {
            ID = 0,
            BONE = 3,
            DAMAGE = 5,
            SIZE = 6,
            XOFFSET = 7,
            YOFFSET = 8,
            ZOFFSET = 9,
            ANGLE = 10,
            GROWTH = 11,
            FIXED = 12,
            ITEM_HIT = 13,
            CLANK = 16,
            FLINCHLESS = 17,
            BASE = 18,
            ELEMENT = 19,
            SHIELD_DAMAGE = 20,
            HIT_GROUND = 23,
            HIT_AERIAL = 24,
        }

        private static readonly Dictionary<string, int> hitboxRemapper = new()
        {
            { "hitbox_id:", 0 }, // ID
            //{ "hitbox_id", 1 }, // Hit Group
            //{ "hitbox_id", 2 }, // Only Hit Grabbed Fighter (bugged)
            { "bone_index:", 3 }, // Bone
            //{ "hitbox_id", 4 }, // Use Common Bone IDs
            { "damage:", 5 }, // Damage
            { "size:", 6 }, // Size
            { "x_offset:", 7 }, // Z-Offset
            { "y_offset:", 8 }, // Y-Offset
            { "z_offset:", 9 }, // X-Offset
            { "trajectory:", 10 }, // Angle
            { "kbg:", 11 }, // Knockback Growth
            { "wdsk:", 12 }, // Weight Set Knockback
            //{ "hitbox_id", 13 }, // Item Hit Interaction
            //{ "remain_grabbed:", 14 }, // Ignore Thrown Fighters
            //{ "hitbox_id", 15 }, // Ignore Fighter Scale
            { "clang:", 16 }, // Clank
            { "flinchless:", 17 }, // Rebound
            { "bkb:", 18 }, // Base Knockback
            //{ "hitbox_id", 19 }, // Element
            { "shield_damage:", 20 }, // Shield Damage
            //{ "hitbox_id", 21 }, // Hit SFX Severity
            //{ "hitbox_id", 22 }, // Hit SFX Kind
            { "ground:", 23 }, // Hit Grounded Fighters
            { "aerial:", 24 }, // Hit Aerial Fighters
        };

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<SubactionEvent> FromBrawlText(string input)
        {
            List<SubactionEvent> sa = new();

            string[] lines = input.Split('\n');

            foreach (string v in lines)
            {
                string name = v.Split('(')[0].Trim();
                MatchCollection param = Regex.Matches(v, @"(.+?)(?:,|$)");

                if (name.ToLower().Contains("hitbox") && name.ToLower().Contains("create"))
                {
                    Subaction subaction = SubactionManager.GetSubaction(11 << 2, SubactionGroup.Fighter);
                    Dictionary<string, int> remap = hitboxRemapper;

                    int[] saparam = new int[subaction.Parameters.Length];
                    saparam[13] = 1;

                    Console.WriteLine(name);

                    foreach (object p in param)
                    {
                        string label = Regex.Match(p.ToString(), @"([^:\s]*):").ToString();
                        string value = Regex.Match(p.ToString(), @"([^:\(\)]+)(?=[,\)])").ToString();

                        Console.WriteLine($"\t{label} = {value}");

                        if (remap.ContainsKey(label.ToString()))
                        {
                            if (p.ToString().Contains("Constant"))
                                saparam[remap[label.ToString()]] = (int)float.Parse(value);
                            else
                            if (value.Contains("true"))
                                saparam[remap[label.ToString()]] = 1;
                            else
                            if (value.Contains("false"))
                                saparam[remap[label.ToString()]] = 1;
                            else
                            if (value.Contains("."))
                                saparam[remap[label.ToString()]] = (int)(float.Parse(value) * 256);
                            else
                                saparam[remap[label.ToString()]] = int.Parse(value);
                        }
                    }

                    sa.Add(new SubactionEvent(SubactionGroup.Fighter, subaction.Compile(saparam), null));
                }

            }

            return sa;
        }

        public class UltimateAttack
        {
            public int Agent { get; set; }
            public int Id { get; set; }
            public int Part { get; set; }
            public string Bone { get; set; } = "";
            public double Damage { get; set; }
            public int Angle { get; set; }
            public int KnockbackGrowth { get; set; }
            public int FixedKnockback { get; set; }
            public int BaseKnockback { get; set; }
            public double Size { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
            public double Hitlag { get; set; }
            public double SDI { get; set; }
            public string SetoffKind { get; set; } = "";
            public string LRCheck { get; set; } = "";
            public bool FriendlyFire { get; set; }
            public int Effect { get; set; }
            public double ShieldDamage { get; set; }
            public int Trip { get; set; }
            public bool Rehit { get; set; }
            public bool Reflectable { get; set; }
            public bool Absorbable { get; set; }
            public bool Flinchless { get; set; }
            public bool DisableHitlag { get; set; }
            public string CollisionSituation { get; set; } = "";
            public string CollisionCategory { get; set; } = "";
            public string CollisionPart { get; set; } = "";
            public bool FriendlyFireOverride { get; set; }
            public string Attribute { get; set; } = "";
            public string SoundLevel { get; set; } = "";
            public string SoundAttribute { get; set; } = "";
            public string AttackRegion { get; set; } = "";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static SubactionEvent ParseUltimateAttack(string input)
        {
            int start = input.IndexOf('(') + 1;
            int end = input.LastIndexOf(')');
            string argsString = input.Substring(start, end - start);

            string[] parts = argsString.Split(',')
                                       .Select(p => p.Trim())
                                       .ToArray();

            var attack = new UltimateAttack
            {
                //Agent = int.Parse(parts[0]),
                Id = int.Parse(parts[1]),
                Part = int.Parse(parts[2]),
                //Bone = parts[3].Replace("Hash40::new(\"", "").Replace("\")", ""),
                Damage = double.Parse(parts[4]),
                Angle = int.Parse(parts[5]),
                KnockbackGrowth = int.Parse(parts[6]),
                FixedKnockback = int.Parse(parts[7]),
                BaseKnockback = int.Parse(parts[8]),
                Size = double.Parse(parts[9]),
                X = double.Parse(parts[10]),
                Y = double.Parse(parts[11]),
                Z = double.Parse(parts[12]),
                Hitlag = double.Parse(parts[16]),
                SDI = double.Parse(parts[17]),
                SetoffKind = parts[18].Trim('*'),
                LRCheck = parts[19].Trim('*'),
                FriendlyFire = bool.Parse(parts[20]),
                Effect = int.Parse(parts[21]),
                ShieldDamage = double.Parse(parts[22]),
                //Trip = int.Parse(parts[23]),
                //Rehit = bool.Parse(parts[24]),
                Reflectable = bool.Parse(parts[25]),
                Absorbable = bool.Parse(parts[26]),
                Flinchless = bool.Parse(parts[27]),
                //DisableHitlag = bool.Parse(parts[28]),
                //CollisionSituation = parts[29].Trim('*'),
                //CollisionCategory = parts[30].Trim('*'),
                //CollisionPart = parts[31].Trim('*'),
                //FriendlyFireOverride = bool.Parse(parts[32]),
                //Attribute = parts[33].Replace("Hash40::new(\"", "").Replace("\")", ""),
                //SoundLevel = parts[34].Trim('*'),
                //SoundAttribute = parts[35].Trim('*'),
                //AttackRegion = parts[36].Trim('*'),
            };

            // init params
            Subaction subaction = SubactionManager.GetSubaction(11 << 2, SubactionGroup.Fighter);
            int[] saparam = new int[subaction.Parameters.Length];
            saparam[(int)HitboxParam.ID] = attack.Id;
            saparam[(int)HitboxParam.ITEM_HIT] = 1;
            saparam[(int)HitboxParam.DAMAGE] = (int)attack.Damage;
            saparam[(int)HitboxParam.ANGLE] = attack.Angle;
            saparam[(int)HitboxParam.GROWTH] = attack.KnockbackGrowth;
            saparam[(int)HitboxParam.FIXED] = attack.FixedKnockback;
            saparam[(int)HitboxParam.BASE] = attack.BaseKnockback;
            saparam[(int)HitboxParam.SIZE] = (int)(attack.Size * 255);
            saparam[(int)HitboxParam.XOFFSET] = (int)(attack.X * 255);
            saparam[(int)HitboxParam.YOFFSET] = (int)(attack.Y * 255);
            saparam[(int)HitboxParam.ZOFFSET] = (int)(attack.Z * 255);
            saparam[(int)HitboxParam.SHIELD_DAMAGE] = (int)attack.ShieldDamage;
            saparam[(int)HitboxParam.HIT_GROUND] = 1;
            saparam[(int)HitboxParam.HIT_AERIAL] = 1;
            saparam[(int)HitboxParam.CLANK] = 1;
            saparam[(int)HitboxParam.FLINCHLESS] = attack.Flinchless ? 1 : 0;


            return new SubactionEvent(SubactionGroup.Fighter, subaction.Compile(saparam), null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<SubactionEvent> FromUltimateText(string input)
        {
            List<SubactionEvent> sa = new();

            string[] lines = input.Split('\n');

            foreach (string v in lines.Select(e=>e.Trim()))
            {
                if (v.Contains("::ATTACK"))
                {
                    sa.Add(ParseUltimateAttack(v));
                }
            }

            return sa;
        }
    }
}
