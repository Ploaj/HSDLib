using System.Windows.Forms;
using HSDRaw;
using System.Collections.Generic;
using System;
using HSDRaw.Common;
using HSDRaw.Common.Animation;

namespace HSDRawViewer
{
    public class DataNode : TreeNode
    {
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
                        foreach (var a in acc)
                        {
                            strucs.Add(a._s);
                            Nodes.Add(new DataNode(prop.Name + (typeToImageKey.ContainsKey(acc.GetType()) ? "" : ":\t" + prop.PropertyType.Name), a));
                            AddNext(a);
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

            foreach (var v in Accessor._s.References)
            {
                if(!strucs.Contains(v.Value))
                    Nodes.Add(new DataNode("0x" + v.Key.ToString("X6"), Accessor._s.GetReference<HSDAccessor>(v.Key)));
            }
        }

        public void Delete()
        {
            if(Parent != null && Parent is DataNode parent)
            {
                parent.Accessor._s.RemoveReferenceToStruct(Accessor._s);
                parent.Nodes.Remove(this);
            }
        }

    }
}
