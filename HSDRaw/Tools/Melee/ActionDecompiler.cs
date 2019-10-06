using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HSDRaw.Tools.Melee
{
    public class ActionDecompiler
    {
        private Dictionary<HSDStruct, string> tempStructToName = new Dictionary<HSDStruct, string>();

        private Dictionary<HSDStruct, string> structToFunctionName = new Dictionary<HSDStruct, string>();

        /// <summary>
        /// Decompiles subaction byte code into script
        /// </summary>
        /// <param name="commanddata"></param>
        /// <returns></returns>
        public string Decompile(string name, HSDAccessor commanddata)
        {
            if (commanddata == null)
                return "";
            StringBuilder output = new StringBuilder();

            tempStructToName.Clear();
            DecompileGroup(output, name, commanddata._s);

            if(output.ToString() == "")
            {
                output.AppendLine("ref: " + structToFunctionName[commanddata._s]);
            }

            return output.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        /// <param name="name"></param>
        /// <param name="datas"></param>
        /// <param name="structToFunctionName"></param>
        private void DecompileGroup(StringBuilder output, string name, HSDStruct datas)
        {
            if (structToFunctionName.ContainsKey(datas))
                return;
            structToFunctionName.Add(datas, name);
            
            output.AppendLine(name);
            output.AppendLine("{");
            using (BinaryReaderExt r = new BinaryReaderExt(new MemoryStream(datas.GetData())))
            {
                byte flag = (byte)(r.ReadByte() >> 2);

                var cmd = ActionCommon.GetMeleeCMDAction(flag);

                while (flag != 0)
                {
                    r.BaseStream.Position -= 1;
                    var size = cmd.ByteSize;
                    var command = r.GetSection(r.Position, size);
                    r.Skip((uint)size);

                    if (flag == 5 || flag == 7) //goto
                    {
                        var re = datas.GetReference<HSDAccessor>((int)r.BaseStream.Position - 4);
                        if (re != null)
                        {
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
                    }
                    else
                    {
                        output.AppendLine("\t" + DecompileCommand(command));
                    }

                    if (r.BaseStream.Position >= r.BaseStream.Length)
                        break;

                    flag = (byte)(r.ReadByte() >> 2);
                    cmd = ActionCommon.GetMeleeCMDAction(flag);
                }
            }
            output.AppendLine("}");

            foreach (var re in datas.References)
            {
                DecompileGroup(output, name + "_" + re.Key.ToString("X4"), re.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string DecompileCommand(byte[] data)
        {
            MeleeCMDAction act = ActionCommon.GetMeleeCMDAction((byte)(data[0] >> 2));

            string o = act.Name + "(";

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
