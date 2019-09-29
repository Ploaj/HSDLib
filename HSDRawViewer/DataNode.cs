using System.Windows.Forms;
using HSDRaw;
using System.Collections.Generic;
using System;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using System.Linq;
using System.Collections;
using HSDRaw.Melee.Gr;

namespace HSDRawViewer
{
    public class DataNode : TreeNode
    {
        public bool ArrayMember { get; internal set; } = false;
        private string ArrayName { get; set; }
        private int ArrayIndex { get; set; }
        public HSDAccessor Accessor { get; set; }

        private static Dictionary<Type, string> typeToImageKey = new Dictionary<Type, string>()
        {
            { typeof(HSD_JOBJ), "jobj" },
            { typeof(HSD_DOBJ), "dobj" },
            { typeof(HSD_MOBJ), "mobj" },
            { typeof(HSD_POBJ), "pobj" },
            { typeof(HSD_TOBJ), "tobj" },
            { typeof(HSD_AOBJ), "aobj" },
            { typeof(HSD_IOBJ), "iobj" },
            { typeof(HSD_SOBJ), "sobj" },
            { typeof(HSD_Camera), "cobj" },
            { typeof(HSD_LOBJ), "lobj" }
        };

        public DataNode(string Text, HSDAccessor accessor)
        {
            this.Text = Text;
            Accessor = accessor;
            if (typeToImageKey.ContainsKey(accessor.GetType()))
            {
                ImageKey = typeToImageKey[accessor.GetType()];
                SelectedImageKey = typeToImageKey[accessor.GetType()];
            } else
            if(accessor.GetType() != typeof(HSDAccessor) )
            {
                ImageKey = "known";
                SelectedImageKey = "known";
            }

            // add dummy only if this node has references
            //if(accessor._s.References.Count != 0)
                Nodes.Add(new TreeNode()); // dummy
        }

        private void AddNext(HSDAccessor access)
        {
            foreach (var prop in access.GetType().GetProperties())
            {
                if (prop.Name == "Next")
                {
                    var acc = (HSDAccessor)prop.GetValue(access);
                    if (acc != null)
                    {
                        Nodes.Add(new DataNode(prop.PropertyType.Name, acc));
                        AddNext(acc);
                    }
                }

            }
        }

        public void Refresh()
        {
            if (Parent != null && Parent is DataNode parent)
            {
                if (ArrayMember)
                {
                    var prop = parent.Accessor.GetType().GetProperty(ArrayName);
                    var arr = prop.GetValue(parent.Accessor) as HSDAccessor[];
                    arr[ArrayIndex] = Accessor;
                    prop.SetValue(parent.Accessor, arr);

                }
                //parent.Refresh();
            }

            Collapse();
            Expand();
        }

        public void ExpandData()
        {
            HashSet<HSDStruct> strucs = new HashSet<HSDStruct>();

            foreach(var prop in Accessor.GetType().GetProperties())
            {
                if (prop.Name.Equals("Item") || prop.Name.Equals("Children"))
                    continue;
                if (prop.PropertyType.IsArray)
                {
                    var acc = prop.GetValue(Accessor) as HSDAccessor[];

                    if (acc != null)
                    {
                        int index = 0;
                        foreach (var a in acc)
                        {
                            if (a == null) continue;
                            strucs.Add(a._s);
                            Nodes.Add(
                                new DataNode
                                (prop.Name + (typeToImageKey.ContainsKey(acc.GetType()) ? "" : $"_{index}:\t" + prop.PropertyType.Name), a)
                            {
                                ArrayMember = true,
                                ArrayName = prop.Name,
                                ArrayIndex = index
                            });
                            // add substructs too so they don't get appended at the end
                            foreach (var ss in a._s.References)
                                strucs.Add(ss.Value);
                            AddNext(a);
                            index++;
                        }
                    }

                }
                if (prop.PropertyType.IsSubclassOf(typeof(HSDAccessor)))
                {
                    var acc = prop.GetValue(Accessor) as HSDAccessor;
                    if(acc != null)
                    {
                        strucs.Add(acc._s);
                        if (prop.Name != "Next")
                        {
                            Nodes.Add(new DataNode(prop.Name + (typeToImageKey.ContainsKey(acc.GetType()) ? "" : ":\t" + prop.PropertyType.Name), acc));
                            AddNext(acc);
                        }
                    }
                }
            }

            // appends structs without labels
            foreach (var v in Accessor._s.References)
            {
                if(!strucs.Contains(v.Value))
                    Nodes.Add(new DataNode("0x" + v.Key.ToString("X6"), Accessor._s.GetReference<HSDAccessor>(v.Key)));
            }
        }

        public void Export()
        {
            using (SaveFileDialog f = new SaveFileDialog())
            {
                f.Filter = "HSD (*.dat)|*.dat";
                f.FileName = Text;

                if(f.ShowDialog() == DialogResult.OK)
                {
                    HSDRawFile r = new HSDRawFile();
                    HSDRootNode root = new HSDRootNode();
                    root.Data = Accessor;
                    root.Name = System.IO.Path.GetFileNameWithoutExtension(f.FileName);
                    r.Roots.Add(root);
                    r.Save(f.FileName);
                }
            }
        }

        private bool OpenDAT(out HSDRawFile file)
        {
            file = null;
            using (OpenFileDialog f = new OpenFileDialog())
            {
                f.Filter = "HSD (*.dat)|*.dat";
                f.FileName = Text;

                if (f.ShowDialog() == DialogResult.OK)
                {
                    file = new HSDRawFile(f.FileName);
                    return true;
                }
            }
            return false;
        }

        private void ReplaceMe(HSDAccessor newStruct)
        {
            Accessor._s.SetData(newStruct._s.GetData());
            Accessor._s.References.Clear();
            foreach (var v in newStruct._s.References)
            {
                Accessor._s.References.Add(v.Key, v.Value);
            }
        }

        public void Import()
        {
            HSDRawFile file;
            if (OpenDAT(out file))
            {
                ReplaceMe(file.Roots[0].Data);
            }
        }

        public void Delete()
        {
            if(Parent != null && Parent is DataNode parent)
            {
                if (ArrayMember)
                {
                    // this is a mess
                    var prop = parent.Accessor.GetType().GetProperty(ArrayName);

                    var arr = prop.GetValue(parent.Accessor) as object[];
                    
                    var list = arr.ToList();
                    list.RemoveAt(ArrayIndex);

                    var outputArray = Array.CreateInstance(Accessor.GetType(), list.Count);
                    Array.Copy(list.ToArray(), outputArray, list.Count);

                    prop.SetValue(parent.Accessor, outputArray);
                }
                else
                if(parent.Accessor._s.RemoveReferenceToStruct(Accessor._s))
                    parent.Nodes.Remove(this);


                parent.Refresh();
            }
        }


#region Special

        /// <summary>
        /// Opens a <see cref="SBM_Model_Group"/> from a dat file and appends it to the <see cref="SBM_Map_Head"/>
        /// </summary>
        public void ImportModelGroup()
        {
            HSDRawFile file;
            if (Accessor is SBM_Map_Head head && OpenDAT(out file))
            {
                var group = head.ModelGroups.ToList();

                group.Add(new SBM_Model_Group() { _s = file.Roots[0].Data._s });

                head.ModelGroups = group.ToArray();

                Refresh();
            }
        }

#endregion

    }
}
