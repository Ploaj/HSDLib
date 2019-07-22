using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HSDRaw.Tools.Melee
{
    public class ActionDecompiler
    {
        private static Dictionary<byte, List<string>> decompiledFunctions = new Dictionary<byte, List<string>>();

        public static void DumpDecompiledCommands()
        {
            foreach (var d in decompiledFunctions)
            {
                using (StreamWriter w = new StreamWriter(new FileStream("ScriptDump\\" + ActionCommon.GetActionName(d.Key) + ".txt", FileMode.Create)))
                {
                    foreach (var v in d.Value)
                        w.WriteLine(v);
                }
            }
        }

        /// <summary>
        /// Decompiles subaction byte code into script
        /// </summary>
        /// <param name="commanddata"></param>
        /// <returns></returns>
        public static string Decompile(string name, HSDAccessor commanddata, ref Dictionary<HSDStruct, string> structToFunctionName)
        {
            StringBuilder output = new StringBuilder();

            tempStructToName.Clear();
            DecompileGroup(output, name, commanddata._s, ref structToFunctionName);

            if(output.ToString() == "")
            {
                output.AppendLine("ref: " + structToFunctionName[commanddata._s]);
            }

            return output.ToString();
        }

        private static Dictionary<HSDStruct, string> tempStructToName = new Dictionary<HSDStruct, string>();
        private static void DecompileGroup(StringBuilder output, string name, HSDStruct datas, ref Dictionary<HSDStruct, string> structToFunctionName)
        {
            if (structToFunctionName.ContainsKey(datas))
                return;
            structToFunctionName.Add(datas, name);


            output.AppendLine(name);
            output.AppendLine("{");
            using (BinaryReaderExt r = new BinaryReaderExt(new MemoryStream(datas.GetData())))
            {
                byte flag = (byte)(r.ReadByte() >> 2);
                while (flag != 0)
                {
                    r.BaseStream.Position -= 1;
                    var size = ActionCommon.GetSize(flag);
                    var command = r.GetSection(r.Position, size);
                    r.Skip((uint)size);

#if DEBUG
                    if (!decompiledFunctions.ContainsKey(flag))
                        decompiledFunctions.Add(flag, new List<string>());
                    decompiledFunctions[flag].Add(DecompileCommand(command));
#endif
                    if (flag == 5 || flag == 7) //goto
                    {
                        var re = datas.GetReference<HSDAccessor>((int)r.BaseStream.Position - 4);
                        if (!tempStructToName.ContainsKey(re._s))
                        {
                            if (structToFunctionName.ContainsKey(re._s))
                                tempStructToName.Add(re._s, structToFunctionName[re._s]);
                            else
                                tempStructToName.Add(re._s, name + "_" + ((int)r.BaseStream.Position - 4).ToString("X4"));
                        }
                        var funcname = tempStructToName[re._s];
                        output.AppendLine("\t" + (flag == 5 ? "Goto" : "Subroutine") + "(" + funcname + ");");
                    }
                    else
                    {
                        output.AppendLine("\t" + DecompileCommand(command));
                    }

                    if (r.BaseStream.Position >= r.BaseStream.Length)
                        break;

                    flag = (byte)(r.ReadByte() >> 2);
                }
            }
            output.AppendLine("}");

            foreach (var re in datas.References)
            {
                DecompileGroup(output, name + "_" + re.Key.ToString("X4"), re.Value, ref structToFunctionName);
            }
        }

        private static string DecompileCommand(byte[] data)
        {
            string o = ActionCommon.GetActionName((byte)(data[0] >> 2)) + "(";

            MeleeCMDAction act = ActionCommon.GetMeleeCMDAction((byte)(data[0] >> 2));

            if (act != null)
            {
                o += act.Decompile(data);
            }
            else
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (i == 0)
                    {
                        o += "0x" + (data[i] & 0x3).ToString("x");
                    }
                    else
                    {
                        o += ",0x" + data[i].ToString("x");
                    }
                }
            }

            o += ");";
            return o;
        }
    }
}
