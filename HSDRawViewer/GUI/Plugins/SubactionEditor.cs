using System;
using System.Windows.Forms;
using HSDRaw;
using HSDRaw.Tools.Melee;
using System.Collections.Generic;
using HSDRaw.Melee.Pl;
using WeifenLuo.WinFormsUI.Docking;
using HSDRawViewer.GUI.Plugins;
using System.Drawing;

namespace HSDRawViewer.GUI
{
    public partial class SubactionEditor : DockContent, EditorBase
    {
        public class Action
        {
            public HSDStruct _struct;

            public string Text;

            public override string ToString()
            {
                return Text;
            }
        }

        public class SubActionScript
        {
            public byte[] data;

            public HSDStruct Reference;

            public string Text
            {
                get
                {
                    Bitreader r = new Bitreader(data);

                    var sa = ActionCommon.GetMeleeCMDAction((byte)r.Read(6));

                    string name = sa.Name + "(";

                    for (int i = 0; i < sa.BMap.Count; i++)
                    {
                        if (sa.BMap[i].Name.Contains("None"))
                            continue;
                        var value = r.Read(sa.BMap[i].Count);

                        if (sa.BMap[i].Name.Contains("Pointer"))
                        {
                            name += "POINTER";
                        }
                        else
                            name += sa.BMap[i].Name + "=" + (sa.BMap[i].Hex ? value.ToString("X") : value.ToString());

                        if (i != sa.BMap.Count - 1)
                            name += ", ";
                    }

                    return name + ")";
                }
            }

            public override string ToString()
            {
                return Text;
            }
        }

        public DockState DefaultDockState => DockState.DockTop;

        public Type[] SupportedTypes => new Type[] { typeof(SBM_SubActionTable) };

        public DataNode Node
        {
            get => _node;
            set
            {
                _node = value;
                SubactionTable = _node.Accessor as SBM_SubActionTable;
                LoadActions(SubactionTable.Subactions);
                RefreshActionList();
            }
        }

        private DataNode _node;
        private SBM_SubActionTable SubactionTable;

        private readonly List<Action> AllScripts = new List<Action>();

        /// <summary>
        /// 
        /// </summary>
        public SubactionEditor()
        {
            InitializeComponent();

            panel1.Visible = false;
        }

        private void LoadActions(SBM_FighterSubAction[] Subactions)
        {
            HashSet<HSDStruct> aHash = new HashSet<HSDStruct>();
            Queue<HSDStruct> extra = new Queue<HSDStruct>();

            int Index = 0;
            foreach (var v in Subactions)
            {
                if (!aHash.Contains(v.SubAction._s))
                {
                    aHash.Add(v.SubAction._s);
                    AllScripts.Add(new Action()
                    {
                        _struct = v.SubAction._s,
                        Text = v.Name == null ? "Func_" + Index.ToString("X") : v.Name
                    });
                }

                foreach (var c in v.SubAction._s.References)
                {
                    if (!aHash.Contains(c.Value))
                    {
                        extra.Enqueue(c.Value);
                    }
                }

                Index++;
            }

            Index = 0;
            while (extra.Count > 0)
            {
                var v = extra.Dequeue();
                if (!aHash.Contains(v))
                {
                    aHash.Add(v);
                    AllScripts.Add(new Action()
                    {
                        _struct = v,
                        Text = "Subroutine_" + Index.ToString("X")
                    });
                }
                foreach (var r in v.References)
                    if (!aHash.Contains(r.Value))
                        extra.Enqueue(r.Value);
                Index++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RefreshActionList()
        {
            actionList.Items.Clear();
            subActionList.Items.Clear();
            foreach (var sa in AllScripts)
            {
                actionList.Items.Add(sa);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="script"></param>
        private void SelectAction(Action script)
        {
            // gather all references to this script
            var references = AllScripts.FindAll(e=>e._struct.References.ContainsValue(script._struct));

            cbReference.Items.Clear();
            foreach(var r in references)
            {
                cbReference.Items.Add(r);
            }

            panel1.Visible = (cbReference.Items.Count > 0);

            if (cbReference.Items.Count > 0)
                cbReference.SelectedIndex = 0;

            // decode data
            var data = script._struct.GetData();

            subActionList.Items.Clear();
            for (int i = 0; i < data.Length;)
            {
                var sa = ActionCommon.GetMeleeCMDAction((byte)(data[i] >> 2));

                var sas = new SubActionScript();
                
                foreach (var r in script._struct.References)
                {
                    if (r.Key >= i && r.Key < i + sa.ByteSize)
                        if (sas.Reference != null)
                            throw new NotSupportedException("Multiple References not supported");
                        else
                            sas.Reference = r.Value;
                }
                
                var sub = new byte[sa.ByteSize];

                for(int j = 0; j < sub.Length; j++)
                {
                    sub[j] = data[i + j];
                }

                sas.data = sub;

                subActionList.Items.Add(sas);

                i += sa.ByteSize;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveSubactionChanges()
        {
            if (actionList.SelectedItem is Action a)
            {
                a._struct.References.Clear();

                List<byte> scriptData = new List<byte>();
                foreach (SubActionScript scr in subActionList.Items)
                { 
                    // TODO: are all references in this position?
                    if(scr.Reference != null)
                    {
                        a._struct.References.Add(scriptData.Count + 4, scr.Reference);
                    }
                    scriptData.AddRange(scr.data);
                }
                
                a._struct.SetData(scriptData.ToArray());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void actionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(actionList.SelectedItem is Action a)
            {
                SelectAction(a);
            }
            else
            {
                subActionList.Items.Clear();
                cbReference.Items.Clear();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subActionList_DoubleClick(object sender, EventArgs e)
        {
            EditSubAction();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subActionList_MouseDown(object sender, MouseEventArgs e)
        {
            if (subActionList.SelectedItem == null || e.Clicks == 2) return;
            subActionList.DoDragDrop(subActionList.SelectedItem, DragDropEffects.Move);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subActionList_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subActionList_DragDrop(object sender, DragEventArgs e)
        {
            Point point = subActionList.PointToClient(new Point(e.X, e.Y));
            int index = subActionList.IndexFromPoint(point);
            if (index < 0) index = subActionList.Items.Count - 1;
            object data = subActionList.SelectedItem;
            if(subActionList.Items.IndexOf(data) != index)
            {
                subActionList.Items.Remove(data);
                subActionList.Items.Insert(index, data);

                SaveSubactionChanges();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var data = new byte[] { 0x18, 0, 0, 0 };
            var action = new Action()
            {
                Text = "Custom_" + AllScripts.Count,
                _struct = new HSDStruct(data)
            };
            AllScripts.Add(action);
            RefreshActionList();
            actionList.SelectedItem = action;
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subActionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //buttonRemove.Enabled = subActionList.SelectedIndex != -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            subActionList.Items.Add(new SubActionScript()
            {
                data = new byte[] { 0, 0, 0, 0} 
            });

            SaveSubactionChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if(subActionList.SelectedItem != null)
                subActionList.Items.Remove(subActionList.SelectedItem);

            SaveSubactionChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEdit_Click(object sender, EventArgs e)
        {
            EditSubAction();
        }

        /// <summary>
        /// 
        /// </summary>
        private void EditSubAction()
        {
            if (subActionList.SelectedItem is SubActionScript sa)
            {
                using (SubActionPanel p = new SubActionPanel(AllScripts))
                {
                    p.LoadData(sa.data, sa.Reference);
                    if (p.ShowDialog() == DialogResult.OK)
                    {
                        sa.data = p.Data;

                        if (sa.data.Length > 0 && ActionCommon.GetMeleeCMDAction((byte)(sa.data[0] >> 2)).HasPointer)
                            sa.Reference = p.Reference;
                        else
                            sa.Reference = null;

                        subActionList.Items[subActionList.SelectedIndex] = subActionList.SelectedItem;

                        SaveSubactionChanges();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGoto_Click(object sender, EventArgs e)
        {
            if(cbReference.SelectedItem is Action a)
            {
                Console.WriteLine(a.Text + " " + actionList.Items.IndexOf(a));
                actionList.SelectedIndex = actionList.Items.IndexOf(a);
            }
        }
    }
}
