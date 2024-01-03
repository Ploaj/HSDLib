using HSDRawViewer.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HSDRawViewer.GUI.Plugins.ScriptEditor
{
    public class SubactionFromText
    {

        private static Dictionary<string, int> hitboxRemapper = new Dictionary<string, int>()
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
            var sa = new List<SubactionEvent>();

            var lines = input.Split('\n');

            foreach (var v in lines)
            {
                var name = v.Split('(')[0].Trim();
                var param = Regex.Matches(v, @"(.+?)(?:,|$)");

                if (name.ToLower().Contains("hitbox") && name.ToLower().Contains("create"))
                {
                    var subaction = SubactionManager.GetSubaction(11 << 2, SubactionGroup.Fighter);
                    Dictionary<string, int> remap = hitboxRemapper;

                    int[] saparam = new int[subaction.Parameters.Length];
                    saparam[13] = 1;

                    Console.WriteLine(name);

                    foreach (var p in param)
                    {
                        var label = Regex.Match(p.ToString(), @"([^:\s]*):").ToString();
                        var value = Regex.Match(p.ToString(), @"([^:\(\)]+)(?=[,\)])").ToString();

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
    }
}
