using System.Windows.Forms;
using HSDRaw;
using System.Collections.Generic;
using System;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using System.Linq;
using HSDRaw.Melee.Gr;
using HSDRaw.Melee.Pl;
using HSDRaw.AirRide.Gr.Data;
using HSDRaw.Melee.Ef;
using HSDRaw.Melee;
using HSDRaw.MEX;
using HSDRaw.MEX.Stages;
using HSDRaw.MEX.Menus;

namespace HSDRawViewer
{
    public class DataNode : TreeNode
    {
        private bool ReferenceNode = false;
        private HSDRootNode Root { get; set; }
        public string RootText { set { Root.Name = value; } }
        public bool IsRootNode { get => Root != null; }
        public bool IsReferenceNode { get => ReferenceNode; }

        public bool IsArrayMember { get; internal set; } = false;
        private string ArrayName { get; set; }
        private int ArrayIndex { get; set; }

        public string StructPath
        {
            get
            {
                return (Parent != null ? ((DataNode)Parent).StructPath : "") + MainForm.Instance.GetStructLocation(Accessor._s) + ": " + Text;
            }
        }

        private System.Drawing.Color KnownColor = System.Drawing.SystemColors.ControlText;
        private System.Drawing.Color UnknownColor = System.Drawing.SystemColors.GrayText;
        private System.Drawing.Color ReferenceNodeColor = System.Drawing.Color.DarkRed;
        private System.Drawing.Color PluginEnabledColor = System.Drawing.Color.Purple; 

        public HSDAccessor Accessor { get => _accessor;
            set
            {
                if (PluginManager.HasEditor(value.GetType()))
                    ForeColor = PluginEnabledColor;

                if(value.GetType() == typeof(HSDAccessor))
                    ForeColor = UnknownColor;

                if (ReferenceNode)
                    ForeColor = ReferenceNodeColor;

                _accessor = value;

                if (typeToImageKey.ContainsKey(value.GetType()))
                {
                    ImageKey = typeToImageKey[value.GetType()];
                    SelectedImageKey = typeToImageKey[value.GetType()];
                }
                else
                if (CheckGenericType(value))
                {

                }
                else
                if (value.GetType() != typeof(HSDAccessor))
                {
                    ImageKey = "known";
                    SelectedImageKey = "known";
                }
            }
        }
        private HSDAccessor _accessor;

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
            { typeof(HSD_LOBJ), "lobj" },
            { typeof(SBM_Coll_Data), "coll" },
            { typeof(KAR_grCollisionNode), "coll" },
            { typeof(HSD_AnimJoint), "anim_joint" },
            { typeof(HSD_MatAnimJoint), "anim_material" },
            { typeof(HSD_ParticleGroup), "group" },
            { typeof(HSD_TEXGraphicBank), "group" },
            { typeof(HSD_TexGraphic), "anim_texture" },
            { typeof(HSD_TexAnim), "anim_texture" },
            { typeof(SBM_Map_Head), "group" },
            { typeof(SBM_GeneralPoints), "group" },
            { typeof(SBM_Map_GOBJ), "group" },
            { typeof(SBM_EffectModel), "group" },
            { typeof(SBM_EffectTable), "table" },
            { typeof(SBM_ArticlePointer), "group" },
            { typeof(SBM_HurtboxBank<SBM_Hurtbox>), "group" },
            { typeof(SBM_HurtboxBank<SBM_ItemHurtbox>), "group" },
            { typeof(SBM_FighterCommandTable), "table" },
            { typeof(SBM_DynamicBehaviorIDs), "table" },
            { typeof(SBM_PlayerModelLookupTables), "table" },
            { typeof(SBM_PlayerSFXTable), "table" },
            { typeof(SBM_HurtboxBank<SBM_ShieldModelContainer>), "folder" },
            { typeof(smSoundTestLoadData), "table" },
            { typeof(MEX_FighterData), "table" },
            { typeof(MEX_SSMTable), "table" },
            { typeof(MEX_FighterFunctionTable), "table" },
            { typeof(MEX_BGMStruct), "table" },
            { typeof(MEX_KirbyTable), "table" },
            { typeof(MEX_KirbyFunctionTable), "table" },
            { typeof(MEX_ItemTables), "table" },
            { typeof(MEX_MenuTable), "table" },
            { typeof(MEX_StageData), "table" },
            { typeof(MEX_EffectData), "table" },
            { typeof(MEX_Data), "kabii" },
            { typeof(MEX_Meta), "fuma" },
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private bool CheckGenericType(HSDAccessor o)
        {
            if(o.GetType().IsGenericType)
            {
                if (o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(HSDArrayAccessor<>))
                    || o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(HSDNullPointerArrayAccessor<>))
                    || o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(HSDFixedLengthPointerArrayAccessor<>)))
                {
                    ImageKey = "folder";
                    SelectedImageKey = "folder";
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="accessor"></param>
        public DataNode(string Text, HSDAccessor accessor, bool referenceNode = false, HSDRootNode root = null)
        {
            Accessor = accessor;
            ReferenceNode = referenceNode;
            this.Text = Text;
            Root = root;

            if (Accessor is HSD_JOBJ jobj && jobj.ClassName != null)
                Text = jobj.ClassName + ":" + Text;
        
            // add dummy only if this node has references or if there is an array in the accessor's properties
            if(accessor._s.References.Count != 0 || Accessor.GetType().GetProperties().ToList().Find(e=>e.PropertyType.IsArray) != null)
                Nodes.Add(new TreeNode()); // dummy
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="access"></param>
        /// <param name="index"></param>
        private void AddNext(HSDAccessor access, int index, List<DataNode> nodes)
        {
            foreach (var prop in access.GetType().GetProperties())
            {
                if (prop.Name == "Next")
                {
                    var acc = (HSDAccessor)prop.GetValue(access);
                    if (acc != null)
                    {
                        var node = new DataNode(prop.PropertyType.Name + "_" + index, acc);
                        nodes.Add(node);
                        AddNext(acc, index + 1, nodes);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Refresh()
        {
            if (Parent != null && Parent is DataNode parent)
            {
                if (IsArrayMember)
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

        /// <summary>
        /// Expands node to contain sub structures
        /// </summary>
        public void ExpandData()
        {
            Dictionary<HSDStruct, IEnumerable<DataNode>> labeledNodes = new Dictionary<HSDStruct, IEnumerable<DataNode>>();

            foreach(var prop in Accessor.GetType().GetProperties())
            {
                // skip these properties
                if (prop.Name.Equals("Item") || 
                    prop.Name.Equals("Children"))
                    continue;

                // handle arrays
                if (prop.PropertyType.IsArray)
                {
                    var acc = prop.GetValue(Accessor) as HSDAccessor[];

                    if (acc != null)
                    {
                        int index = 0;
                        foreach (var a in acc)
                        {
                            if (a == null) continue;

                            // add to labeled nodes so we don't get this node labeled as unknown
                            labeledNodes.Add(a._s, null);

                            var node = new DataNode
                                (prop.Name + (typeToImageKey.ContainsKey(acc.GetType()) ? "" : $"_{index}:\t" + prop.PropertyType.Name), a)
                                {
                                    IsArrayMember = true,
                                    ArrayName = prop.Name,
                                    ArrayIndex = index,
                                };

                            // gives folder icon
                            if (string.IsNullOrEmpty(node.SelectedImageKey))
                            {
                                node.ImageKey = "group";
                                node.SelectedImageKey = "group";
                            }

                            // add substructs too so they don't get appended at the end
                            foreach (var ss in a._s.References)
                                if(!labeledNodes.ContainsKey(ss.Value))
                                    labeledNodes.Add(ss.Value, null);

                            List<DataNode> nodes = new List<DataNode>();
                            nodes.Add(node);
                            AddNext(a, 1, nodes);
                            index++;

                            foreach (var n in nodes)
                                SafeExpand(n);

                            //if (!labeledNodes.ContainsKey(a._s))
                            //    labeledNodes.Add(a._s, nodes);
                        }
                    }

                }
                
                // handle accessors
                if (prop.PropertyType.IsSubclassOf(typeof(HSDAccessor)) || prop.PropertyType == typeof(HSDAccessor))
                {
                    var acc = prop.GetValue(Accessor) as HSDAccessor;
                    
                    if (acc != null && acc._s != Accessor._s)
                    {
                        if (prop.Name != "Next")
                        {
                            var node = new DataNode(prop.Name + (typeToImageKey.ContainsKey(acc.GetType()) ? "" : ":\t" + prop.PropertyType.Name), acc);
                            
                            List<DataNode> nodes = new List<DataNode>();
                            nodes.Add(node);
                            AddNext(acc, 1, nodes);
                            if (!labeledNodes.ContainsKey(acc._s))
                                labeledNodes.Add(acc._s, nodes);
                        }
                        else
                        {
                            labeledNodes.Add(acc._s, null);
                        }
                    }
                }
            }

            // appends structs without labels
            foreach (var v in Accessor._s.References)
            {
                if (!labeledNodes.ContainsKey(v.Value))
                {
                    var node = new DataNode("0x" + v.Key.ToString("X6"), Accessor._s.GetReference<HSDAccessor>(v.Key));
                    SafeExpand(node);
                }
                else
                {
                    SafeExpand(labeledNodes[v.Value]);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        private void SafeExpand(IEnumerable<DataNode> nodes)
        {
            if (nodes == null)
                return;

            foreach (var v in nodes)
                SafeExpand(v);
        }

        /// <summary>
        /// Prevents infinite recursion on node expansion
        /// </summary>
        private void SafeExpand(DataNode n)
        {
            if (n == null)
                return;

            TreeNode par = Parent;
            while(par is DataNode parent)
            {
                if (n.Accessor._s.Equals(parent.Accessor._s))
                    return;
                par = par.Parent;
            }

            Nodes.Add(n);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Export()
        {
            var f = Tools.FileIO.SaveFile("HSD (*.dat)|*.dat", Text + ".dat");
            
            if (f != null)
            {
                HSDRawFile r = new HSDRawFile();
                HSDRootNode root = new HSDRootNode();
                root.Data = Accessor;
                root.Name = System.IO.Path.GetFileNameWithoutExtension(f);
                r.Roots.Add(root);
                r.Save(f);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool OpenDAT(out HSDRawFile file)
        {
            file = null;
            var f = Tools.FileIO.OpenFile("HSD (*.dat)|*.dat");
            if (f != null)
            {
                file = new HSDRawFile(f);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newStruct"></param>
        private void ReplaceMe(HSDAccessor newStruct)
        {
            Accessor._s.SetData(newStruct._s.GetData());
            Accessor._s.References.Clear();
            foreach (var v in newStruct._s.References)
            {
                Accessor._s.References.Add(v.Key, v.Value);
            }
            NotifyChange();
        }

        /// <summary>
        /// Returns true if this node can be safely modified
        /// </summary>
        /// <returns>return true is success</returns>
        private bool CanEdit()
        {
            if (MainForm.Instance.IsOpened(this))
            {
                if (MessageBox.Show("Node is open in editor\nClose Editor?", "Error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    MainForm.Instance.CloseEditor(this);
                else
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Import()
        {
            if (!CanEdit())
                return;

            HSDRawFile file;
            if (OpenDAT(out file))
            {
                ReplaceMe(file.Roots[0].Data);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void Delete()
        {
            if (!CanEdit())
                return;

            if (Parent != null && Parent is DataNode parent)
            {
                if (Accessor is HSD_DOBJ dobj)
                {
                    var current = new HSD_DOBJ();
                    current._s = Accessor._s;
                    
                    if (PrevNode is DataNode prev && prev.Accessor is HSD_DOBJ next)
                    {
                        next.Next = current.Next;
                    }
                    else
                    {
                        parent.Accessor._s.ReplaceReferenceToStruct(Accessor._s, current.Next?._s);
                    }
                }
                else
                if (IsArrayMember)
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
                else 
                if(PrevNode is DataNode prev)
                {
                    prev.Accessor._s.RemoveReferenceToStruct(Accessor._s);
                }


                parent.Refresh();
            }
            else
            {
                if(!ReferenceNode)
                    MainForm.DeleteRoot(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void NotifyChange()
        {
            if(Parent != null && Parent is DataNode parent)
            {
                if (IsArrayMember)
                {
                    var prop = parent.Accessor.GetType().GetProperty(ArrayName);

                    var arr = prop.GetValue(parent.Accessor) as object[];

                    arr[ArrayIndex] = Accessor;

                    prop.SetValue(parent.Accessor, arr);
                }
                parent.NotifyChange();
            }
        }

#region Special

        /// <summary>
        /// Opens a <see cref="SBM_Map_GOBJ"/> from a dat file and appends it to the <see cref="SBM_Map_Head"/>
        /// </summary>
        public void ImportModelGroup()
        {
            HSDRawFile file;
            if (Accessor is SBM_Map_Head head && OpenDAT(out file))
            {
                var group = head.ModelGroups.Array.ToList();

                group.Add(new SBM_Map_GOBJ() { _s = file.Roots[0].Data._s });

                head.ModelGroups.Array = group.ToArray();

                Refresh();
            }
        }

#endregion

    }
}
