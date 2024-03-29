﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HSDRaw.Tools.Melee
{
    public class ActionCompiler
    {
        private static Dictionary<HSDStruct, Dictionary<int, string>> structToOffsetToFunction = new Dictionary<HSDStruct, Dictionary<int, string>>();

        private static Dictionary<string, HSDStruct> nameToFunction = new Dictionary<string, HSDStruct>();

        private static Dictionary<string, string> scriptToReference = new Dictionary<string, string>();

        private static Dictionary<string, HSDStruct> scriptToBinary = new Dictionary<string, HSDStruct>();

        public static int ScriptCount => scriptToBinary.Count;

        public static HSDStruct GetBinary(string script)
        {
            return scriptToBinary[script];
        }

        public static void LinkStructs()
        {
            // Set offsets
            foreach (var so in structToOffsetToFunction)
            {
                var s = so.Key;
                var offs = so.Value;

                foreach (var v in offs)
                {
                    s.SetReferenceStruct(v.Key, nameToFunction[v.Value]);
                }
            }

            foreach(var re in scriptToReference)
            {
                scriptToBinary.Add(re.Key, nameToFunction[re.Value]);
            }

            scriptToReference.Clear();
            structToOffsetToFunction.Clear();
            nameToFunction.Clear();
        }

        /// <summary>
        /// Compiles script into subaction byte code
        /// </summary>
        public static void Compile(string script)
        {
            if (scriptToBinary.ContainsKey(script) || scriptToReference.ContainsKey(script))
                return;

            HSDStruct main = null;

            var stream = Regex.Replace(script, @"\s+", string.Empty);

            if (script.StartsWith("ref:"))
            {
                var name = stream.Split(':')[1];
                scriptToReference.Add(script, name);
                return;
            }

            var functions = Regex.Matches(stream, @"([^\{])*\{([^\}]*)\}");

            foreach (Match g in functions)
            {
                var name = Regex.Match(g.Value, @".+?(?={)").Value;
                var code = Regex.Match(g.Value, @"(?<=\{).+?(?=\})").Value.Split(';');

                if (nameToFunction.ContainsKey(name))
                    continue;

                HSDStruct s = new HSDStruct();
                nameToFunction.Add(name, s);
                structToOffsetToFunction.Add(s, new Dictionary<int, string>());

                if (main == null)
                    main = s;

                List<byte> output = new List<byte>();

                foreach (var c in code)
                {
                    var cname = Regex.Match(c, @".+?(?=\()").Value;
                    var cparameters = Regex.Match(c, @"(?<=\().+?(?=\))").Value.Split(',');

                    byte flag = ActionCommon.GetMeleeCMDAction(cname).Command;

                    if (flag == 0x5 || flag == 0x7) //goto and subroutine
                    {
                        structToOffsetToFunction[s].Add(output.Count + 4, cparameters[0]);
                        output.AddRange(new byte[] { (byte)(flag << 2), 0, 0, 0, 0, 0, 0, 0});
                    }
                    else
                        output.AddRange(CompileCommand(cname, cparameters));

                    // padd
                    if (output.Count % 4 != 0)
                        output.AddRange(new byte[4 - (output.Count % 4)]);
                }
                if(true)
                {
                    output.Add(0);
                    if (output.Count % 4 != 0)
                        output.AddRange(new byte[4 - (output.Count % 4)]);
                }
                s.SetData(output.ToArray());
            }

            if (main == null)
                main = new HSDStruct(4);

            scriptToBinary.Add(script, main);
        }

        private static byte[] CompileCommand(string name, string[] parameters)
        {
            if (name.Equals(""))
                return new byte[0];
            
            MeleeCMDAction action = ActionCommon.GetMeleeCMDAction(name);

            byte[] data = new byte[] { (byte)(action.Command << 2), 0, 0, 0 };
            if (action != null)
            {
                data = action.Compile(parameters);
                if (data == null)
                {
                    //ErrorCode = CompileError.ParameterCount;
                    return null;
                }
            }

            return data;
        }
        
    }
}
