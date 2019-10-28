using System;
using System.Windows.Forms;
using HSDRaw;
using System.Collections.Generic;
using HSDRaw.Melee.Pl;
using WeifenLuo.WinFormsUI.Docking;
using HSDRawViewer.GUI.Plugins;
using System.Drawing;
using System.Text;
using HSDRawViewer.Tools;
using HSDRaw.Tools.Melee;
using System.Linq;

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
                return System.Text.RegularExpressions.Regex.Replace(Text.Replace("_figatree", ""), @"Ply.*_Share_ACTION_", "");
            }
        }

        public class SubActionScript
        {
            public byte[] data;

            public HSDStruct Reference;

            public string Name
            {
                get
                {
                    Bitreader r = new Bitreader(data);

                    var sa = SubactionManager.GetSubaction((byte)r.Read(6));

                    return sa.Name;
                }
            }
            
            public string Parameters
            {
                get
                {
                    Bitreader r = new Bitreader(data);

                    var sa = SubactionManager.GetSubaction((byte)r.Read(6));

                    StringBuilder sb = new StringBuilder();

                    for (int i = 0; i < sa.Parameters.Length; i++)
                    {
                        var param = sa.Parameters[i];

                        if (param.Name.Contains("None"))
                            continue;

                        var value = r.Read(param.BitCount);

                        if (param.IsPointer)
                            sb.Append("\tPOINTER->(Edit To View)");
                        else
                            sb.Append("\t" + 
                                param.Name + 
                                " : " + 
                                (param.Hex ? value.ToString("X") : value.ToString()));

                        if (i != sa.Parameters.Length - 1)
                            sb.AppendLine("");
                    }

                    return sb.ToString();
                }
            }

            public SubActionScript Clone()
            {
                return new SubActionScript()
                {
                    data = (byte[])data.Clone(),
                    Reference = Reference
                };
            }

            public override string ToString()
            {
                return Parameters;
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
                var sa = SubactionManager.GetSubaction((byte)(data[i] >> 2));

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
            //buttonUp.Enabled = subActionList.SelectedIndex != -1;
            //buttonDown.Enabled = subActionList.SelectedIndex != -1;
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
            subActionList.BeginUpdate();
            if (subActionList.SelectedItems.Count != 0)
            {
                var list = new List<object>();
                foreach (var v in subActionList.SelectedItems)
                    list.Add(v);
                foreach(var v in list)
                    subActionList.Items.Remove(v);
            }
            subActionList.EndUpdate();

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

                        if (sa.data.Length > 0 && SubactionManager.GetSubaction((byte)(sa.data[0] >> 2)).HasPointer)
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

        private void subActionList_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if(e.Index != -1 && subActionList.Items[e.Index] is SubActionScript script)
            {
                var length = script.Parameters.Split('\n').Length;
                e.ItemHeight = subActionList.Font.Height * (script.Parameters.Equals("") ? 1 : length + 1);
            }
        }

        private void subActionList_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if(e.Index != -1)
            {
                if(subActionList.Items[e.Index] is SubActionScript script)
                {
                    e.Graphics.DrawString(e.Index + ". " + script.Name, e.Font, new SolidBrush(Color.DarkBlue), e.Bounds);
                    var bottomRect = new Rectangle(new Point(e.Bounds.X, e.Bounds.Y + e.Font.Height), new Size(e.Bounds.Width, e.Bounds.Height));
                    e.Graphics.DrawString(script.Parameters, e.Font, new SolidBrush(e.ForeColor), bottomRect);
                }
                else
                    e.Graphics.DrawString(subActionList.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds);

            }
        }

        // move up
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            subActionList.BeginUpdate();
            int[] indexes = subActionList.SelectedIndices.Cast<int>().ToArray();
            if (indexes.Length > 0 && indexes[0] > 0)
            {
                for (int i = 0; i < subActionList.Items.Count; ++i)
                {
                    if (indexes.Contains(i))
                    {
                        object moveItem = subActionList.Items[i];
                        subActionList.Items.Remove(moveItem);
                        subActionList.Items.Insert(i - 1, moveItem);
                        subActionList.SetSelected(i - 1, true);
                    }
                }
            }
            subActionList.EndUpdate();
            SaveSubactionChanges();
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            subActionList.BeginUpdate();
            int[] indexes = subActionList.SelectedIndices.Cast<int>().ToArray();
            if (indexes.Length > 0 && indexes[indexes.Length - 1] < subActionList.Items.Count - 1)
            {
                for (int i = subActionList.Items.Count - 1; i > -1; --i)
                {
                    if (indexes.Contains(i))
                    {
                        object moveItem = subActionList.Items[i];
                        subActionList.Items.Remove(moveItem);
                        subActionList.Items.Insert(i + 1, moveItem);
                        subActionList.SetSelected(i + 1, true);
                    }
                }
            }
            subActionList.EndUpdate();
            SaveSubactionChanges();
        }

        private List<SubActionScript> CopiedScripts = new List<SubActionScript>();

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            CopiedScripts.Clear();
            foreach (SubActionScript scr in subActionList.SelectedItems)
                CopiedScripts.Add(scr);
            CopiedScripts.Reverse();
        }

        private void buttonPaste_Click(object sender, EventArgs e)
        {
            var index = subActionList.SelectedIndex + 1;
            if (index == -1)
                index = 0;
            foreach (var v in CopiedScripts)
            {
                subActionList.Items.Insert(index, v.Clone());
            }
            SaveSubactionChanges();
        }
    }
}
