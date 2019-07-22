using HSDRaw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HSDRaw.Melee.Pl;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using HSDRaw.Tools.Melee;
using System.Xml;

namespace HSDRawViewer.Converters
{
    public class PlayStruct
    {
        public class ScriptFile
        {
            [XmlElement()]
            public ActionGroup[] Actions { get; set; }
        }

        public class ActionGroup
        {
            [XmlAttribute]
            public string animation_name { get; set; }

            [XmlAttribute]
            public int flags { get; set; }

            [XmlAttribute]
            public int off { get; set; }

            [XmlAttribute]
            public int size { get; set; }

            [XmlText]
            public string script { get; set; }
        }

        public static void Deconstruct(string plFilePath, string ajFilePath, string outputFolder)
        {
            var plfile = new HSDRawFile(plFilePath);

            var path = Path.GetDirectoryName(plFilePath) + "\\" + outputFolder + "\\";
            Directory.CreateDirectory(path);

            var data = plfile.Roots[0].Data as SBM_PlayerData;

            if (data == null)
                return;

            var ajfile = new HSDRawFile(ajFilePath);

            foreach (var prop in data.GetType().GetProperties())
            {
                var val = prop.GetValue(data) as HSDAccessor;

                if (val == null)
                    continue;
                if(prop.PropertyType == typeof(SBM_CommonFighterAttributes))
                {
                    var attr = prop.GetValue(data) as SBM_CommonFighterAttributes;
                    using (StreamWriter w = new StreamWriter(new FileStream(path + prop.Name + ".ini", FileMode.Create)))
                    {
                        foreach(var v in attr.GetType().GetProperties())
                        {
                            if (v.Name.Equals("TrimmedSize"))
                                continue;
                            w.WriteLine($"{v.Name}={v.GetValue(attr)}");
                        }
                    }
                }else
                if (prop.Name.Equals("SubActionTable"))
                {
                    ScriptFile f = new ScriptFile();
                    
                    f.Actions = new ActionGroup[val._s.Length / 0x18];

                    Dictionary<HSDStruct, string> structToFunction = new Dictionary<HSDStruct, string>();
                    HashSet<string> ExportedAnimations = new HashSet<string>();

                    for (int i = 0; i < f.Actions.Length; i++)
                    {
                        SBM_FighterSubAction subaction = new SBM_FighterSubAction();
                        subaction._s = val._s.GetEmbededStruct(0x18 * i, 0x18);

                        if (!ExportedAnimations.Contains(subaction.Name) && subaction.Name != null && subaction.Name != "")
                        {
                            ExportedAnimations.Add(subaction.Name);
                            if (ajFilePath != null && File.Exists(ajFilePath))
                            {
                                using (BinaryReaderExt r = new BinaryReaderExt(new FileStream(ajFilePath, FileMode.Open)))
                                {
                                    var animdata = r.GetSection((uint)subaction.AnimationOffset, subaction.AnimationSize);
                                    File.WriteAllBytes(path + "Animations\\" + subaction.Name + ".dat", animdata);
                                }
                            }
                        }

                        ActionGroup g = new ActionGroup();
                        g.animation_name = subaction.Name;
                        g.flags = subaction.Flags;
                        g.script = ActionDecompiler.Decompile("Func_" + i.ToString("X3"), subaction.SubAction, ref structToFunction);
                        //g.script = 
                        g.off = subaction.AnimationOffset;
                        g.size = subaction.AnimationSize;
                        f.Actions[i] = g;

                        Console.WriteLine(i + " " + subaction.Name + " " + subaction._s.GetReference<HSDAccessor>(0x0C)._s.References.Count);
                    }

                    XmlSerializer writer = new XmlSerializer(typeof(ScriptFile));
                    using (var w = new XmlTextWriter(new FileStream(path + prop.Name + ".txt", FileMode.Create), Encoding.UTF8))
                        writer.Serialize(w, f);
                }
                else
                {
                    HSDRootNode root = new HSDRootNode();

                    root.Name = prop.Name;

                    root.Data = val;

                    HSDRawFile file = new HSDRawFile();
                    file.Roots.Add(root);

                    file.Save(path + prop.Name + ".dat");

                    Console.WriteLine(prop.Name + " " + val._s.GetSubStructs().Count);
                }
            }
        }

        public static void Reconstruct(string filepath, string animPath, string directory, string rootName)
        {
            var path = Path.GetDirectoryName(directory);

            HSDRawFile file = new HSDRawFile();

            HSDRootNode root = new HSDRootNode();
            root.Name = rootName;
            var ftData = new SBM_PlayerData();
            root.Data = ftData;
            file.Roots.Add(root);
            var prop = root.Data.GetType().GetProperties().ToList();

            foreach (var f in Directory.GetFiles(directory))
            {
                if (f.EndsWith(".dat"))
                {
                    HSDRawFile chunk = new HSDRawFile(f);
                    Console.WriteLine(f + " " + chunk.Roots.Count);
                    var property = prop.Find(e => e.Name == chunk.Roots[0].Name);

                    if (property != null)
                    {
                        var newt = Activator.CreateInstance(property.PropertyType);
                        {
                            var dset = newt as HSDAccessor;
                            if (dset != null)
                                dset._s = chunk.Roots[0].Data._s;
                        }
                        property.SetValue(root.Data, newt);
                    }
                } else if (f.EndsWith(".ini"))
                {
                    SBM_CommonFighterAttributes attr = new SBM_CommonFighterAttributes();
                    using (StreamReader r = new StreamReader(new FileStream(f, FileMode.Open)))
                    {
                        foreach (var v in attr.GetType().GetProperties())
                        {
                            if (v.Name.Equals("TrimmedSize"))
                                continue;
                            var line = r.ReadLine().Split('=');
                            if (line.Length < 2 || line[0] != v.Name)
                                throw new InvalidDataException("Invalid Attribute " + string.Join("=", line));
                            if (v.PropertyType == typeof(int))
                            {
                                v.SetValue(attr, int.Parse(line[1].Trim()));
                            }
                            if (v.PropertyType == typeof(float))
                            {
                                v.SetValue(attr, float.Parse(line[1].Trim()));
                            }
                        }
                    }
                    ftData.Attributes = attr;
                }
                else if (f.EndsWith(".txt"))
                {
                    XmlSerializer writer = new XmlSerializer(typeof(ScriptFile));
                    var script = (ScriptFile)writer.Deserialize(new FileStream(f, FileMode.Open));

                    Dictionary<string, Tuple<int, int>> animationToOffset = new Dictionary<string, Tuple<int, int>>();
                    
                    List<SBM_FighterSubAction> SubActions = new List<SBM_FighterSubAction>();
                    Dictionary<SBM_FighterSubAction, string> subActionToScript = new Dictionary<SBM_FighterSubAction, string>();

                    Dictionary<string, HSDStruct> stringToStruct = new Dictionary<string, HSDStruct>();

                    using (BinaryWriter w = new BinaryWriter(new FileStream(animPath, FileMode.Create)))
                    {
                        foreach (var s in script.Actions)
                        {
                            SBM_FighterSubAction subaction = new SBM_FighterSubAction();
                            subaction.Flags = s.flags;
                            if (s.animation_name != null)
                            {
                                if (!stringToStruct.ContainsKey(s.animation_name))
                                {
                                    var namestruct = new HSDStruct();
                                    byte[] data = new byte[s.animation_name.Length + 1];
                                    var bytes = UTF8Encoding.UTF8.GetBytes(s.animation_name);
                                    for (int i = 0; i < s.animation_name.Length; i++)
                                        data[i] = bytes[i];
                                    namestruct.SetData(data);
                                    stringToStruct.Add(s.animation_name, namestruct);
                                }
                                subaction._s.SetReferenceStruct(0, stringToStruct[s.animation_name]);
                                //subaction.Name = s.animation_name;
                                if (!animationToOffset.ContainsKey(s.animation_name))
                                {
                                    if (File.Exists(path + "\\Animations\\" + s.animation_name + ".dat"))
                                    {
                                        var data = File.ReadAllBytes(path + "\\Animations\\" + s.animation_name + ".dat");
                                        animationToOffset.Add(s.animation_name, new Tuple<int, int>((int)w.BaseStream.Position, data.Length));
                                        w.Write(data);
                                        if (w.BaseStream.Length % 0x20 != 0)
                                        {
                                            var padd = new byte[0x20 - (w.BaseStream.Position % 0x20)];
                                            for (int i = 0; i < padd.Length; i++)
                                                padd[i] = 0xFF;
                                            w.Write(padd);
                                        }
                                    }
                                    else
                                    {
                                        throw new FileNotFoundException("Could not find animation " + path + "\\Animations\\" + s.animation_name + ".dat");
                                    }
                                }
                                subaction.AnimationOffset = animationToOffset[s.animation_name].Item1;
                                subaction.AnimationSize = animationToOffset[s.animation_name].Item2;
                            }

                            if (s.script != null)
                            {
                                ActionCompiler.Compile(s.script);
                                subActionToScript.Add(subaction, s.script);
                            }

                            SubActions.Add(subaction);
                        }

                    }
                    
                    ftData.SubActionTable = new HSDAccessor();
                    ftData.SubActionTable._s.Resize(script.Actions.Length * 0x18);

                    ActionCompiler.LinkStructs();

                    for(int i = 0; i < SubActions.Count; i++)
                    {
                        SubActions[i].SubAction = new HSDAccessor() { _s = ActionCompiler.GetBinary(subActionToScript[SubActions[i]]) };
                        ftData.SubActionTable._s.SetEmbededStruct(i * 0x18, SubActions[i]._s);
                    }
                    Console.WriteLine("recompiled count " + ftData.SubActionTable._s.GetSubStructs().Count);
                }
            }

            file.Save(filepath);
        }
    }
}
