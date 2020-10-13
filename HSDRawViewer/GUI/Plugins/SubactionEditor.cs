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
using HSDRawViewer.Rendering;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRawViewer.GUI.Plugins.Melee;
using HSDRawViewer.Rendering.Shapes;
using OpenTK;
using HSDRawViewer.Rendering.Renderers;
using System.ComponentModel;
using HSDRaw.Melee.Cmd;

namespace HSDRawViewer.GUI
{
    public partial class SubactionEditor : DockContent, EditorBase, IDrawable
    {
        public class Action
        {
            public HSDStruct _struct;

            [Category("Animation"), DisplayName("Figatree Symbol")]
            public string Text { get; set; }

            [Category("Animation"), DisplayName("Figatree Offset")]
            public int AnimOffset { get; set; }

            [Category("Animation"), DisplayName("Figatree FileSize")]
            public int AnimSize { get; set; }

            public uint Flags;
            public int Index;

            [Category("Display Flags"), DisplayName("Flags")]
            public string BitFlags { get => Flags.ToString("X"); set { uint v = Flags; uint.TryParse(value, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out v); Flags = v; } }

            [Category("Flags"), DisplayName("Character ID")]
            public uint CharIDCheck { get => Flags & 0x3FF; set => Flags = (Flags & 0xFFFFFC00) | (value & 0x3FF); }

            [Category("Flags"), DisplayName("Utilize animation-induced physics")]
            public bool AnimInducedPhysics { get => (Flags & 0x08000000) != 0; set => Flags = ((Flags & 0x08000000) | (uint)((value ? 1 : 0) << 27)); }

            [Category("Flags"), DisplayName("Loop Animation")]
            public bool LoopAnimation { get => (Flags & 0x04000000) != 0; set => Flags = ((Flags & 0x04000000) | (uint)((value ? 1 : 0) << 26)); }

            [Category("Flags"), DisplayName("Unknown")]
            public bool Unknown { get => (Flags & 0x02000000) != 0; set => Flags = ((Flags & 0x02000000) | (uint)((value ? 1 : 0) << 25)); }

            public override string ToString()
            {
                return System.Text.RegularExpressions.Regex.Replace(Text.Replace("_figatree", ""), @"Ply.*_Share_ACTION_", "");
            }
        }

        [Serializable]
        public class SubActionScript
        {
            public byte[] data;

            public HSDStruct Reference;

            private SubactionGroup SubactionGroup = SubactionGroup.Fighter;

            public SubActionScript(SubactionGroup SubactionGroup)
            {
                this.SubactionGroup = SubactionGroup;
            }

            public SubactionGroup GetGroup()
            {
                return SubactionGroup;
            }

            public string Name
            {
                get
                {
                    Bitreader r = new Bitreader(data);

                    var sa = SubactionManager.GetSubaction((byte)r.Read(8), SubactionGroup);

                    return sa.Name;
                }
            }
            
            public IEnumerable<string> Parameters
            {
                get
                {
                    var sa = SubactionManager.GetSubaction(data[0], SubactionGroup);

                    StringBuilder sb = new StringBuilder();

                    var dparams = sa.GetParameters(data);

                    for (int i = 0; i < sa.Parameters.Length; i++)
                    {
                        var param = sa.Parameters[i];

                        if (param.Name.Contains("None"))
                            continue;

                        var value = param.IsPointer ? 0 : dparams[i];

                        if (param.HasEnums && value < param.Enums.Length)
                            yield return (param.Name +
                                " : " +
                                param.Enums[value]);
                        else
                        if (param.IsPointer)
                            yield return ("POINTER->(Edit To View)");
                        else
                        if (param.IsFloat)
                            yield return (param.Name +
                                " : " +
                                BitConverter.ToSingle(BitConverter.GetBytes(value), 0));
                        else
                            yield return (param.Name + 
                                " : " + 
                                (param.Hex ? value.ToString("X") : value.ToString()));
                        
                    }
                }
            }

            public SubActionScript Clone()
            {
                return new SubActionScript(SubactionGroup)
                {
                    data = (byte[])data.Clone(),
                    Reference = Reference
                };
            }
        }

        public DockState DefaultDockState => DockState.Document;

        public Type[] SupportedTypes => new Type[] { typeof(SBM_FighterCommandTable), typeof(SBM_FighterSubactionData), typeof(SBM_ItemSubactionData), typeof(SBM_ColorSubactionData) };

        public DataNode Node
        {
            get => _node;
            set
            {
                _node = value;
                if (value.Accessor is SBM_FighterSubactionData sudata)
                {
                    if (value.Accessor is SBM_ItemSubactionData)
                        SubactionGroup = SubactionGroup.Item;

                    if (value.Accessor is SBM_ColorSubactionData)
                        SubactionGroup = SubactionGroup.Color;

                    SBM_FighterCommand[] su = new SBM_FighterCommand[]
                    {
                        new SBM_FighterCommand()
                        {
                            SubAction = sudata,
                            Name = "Script"
                        }
                    };

                    LoadActions(su);
                    RefreshActionList();
                    propertyGrid1.Visible = false;
                }
                else
                if(value.Accessor is SBM_FighterCommandTable SubactionTable)
                {
                    LoadActions(SubactionTable.Commands);
                    RefreshActionList();

                    if (Node.Parent is DataNode parent)
                    {
                        if (parent.Accessor is SBM_FighterData plDat)
                        {
                            ModelScale = plDat.Attributes.ModelScale;

                            if (plDat.Hurtboxes != null)
                                Hurtboxes.AddRange(plDat.Hurtboxes.Hurtboxes);
                            
                            ECB = plDat.EnvironmentCollision;

                            ResetModelVis();
                        }
                    }
                }
            }
        }

        public SBM_FighterData FighterData
        {
            get
            {
                if (_node.Accessor is SBM_FighterCommandTable SubactionTable)
                    if (Node.Parent is DataNode parent)
                        if (parent.Accessor is SBM_FighterData plDat)
                            return plDat;

                return null;
            }
        }

        private DataNode _node;

        private SBM_EnvironmentCollision ECB = null;

        private readonly List<Action> AllScripts = new List<Action>();

        public SubactionGroup SubactionGroup = SubactionGroup.Fighter;

        /// <summary>
        /// 
        /// </summary>
        public SubactionEditor()
        {
            InitializeComponent();

            panel1.Visible = false;

            DoubleBuffered = true;

            viewport = new ViewportControl();
            viewport.Dock = DockStyle.Fill;
            viewport.AnimationTrackEnabled = true;
            viewport.AddRenderer(this);
            viewport.EnableFloor = true;
            previewBox.Controls.Add(viewport);
            viewport.RefreshSize();
            viewport.BringToFront();

            toolStripComboBox1.SelectedIndex = 0;

            clickTimer = new Timer();
            clickTimer.Interval = 500;
            clickTimer.Tick += timer_Tick;

            FormClosing += (sender, args) =>
            {
                JOBJManager.CleanupRendering();
                viewport.Dispose();
            };
            
            SubactionProcess.UpdateVISMethod = SetModelVis;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Subactions"></param>
        private void LoadActions(SBM_FighterCommand[] Subactions)
        {
            HashSet<HSDStruct> aHash = new HashSet<HSDStruct>();
            Queue<HSDStruct> extra = new Queue<HSDStruct>();

            int Index = -1;
            foreach (var v in Subactions)
            {
                Index++;

                if (v.SubAction == null)
                    continue;

                if (!aHash.Contains(v.SubAction._s))
                {
                    aHash.Add(v.SubAction._s);
                    AllScripts.Add(new Action()
                    {
                        _struct = v.SubAction._s,
                        AnimOffset = v.AnimationOffset,
                        AnimSize = v.AnimationSize,
                        Flags = v.Flags,
                        Index = Index,
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
            propertyGrid1.SelectedObject = script;

            ClearUndoStack();

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
            
            RefreshSubactionList(script);

            LoadAnimation(script.AnimOffset, script.AnimSize);

            ResetModelVis();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="script"></param>
        private void RefreshSubactionList(Action script)
        {
            // get subaction data
            var data = script._struct.GetData();

            // set the script for the subaction processer for rendering
            SubactionProcess.SetStruct(script._struct, SubactionGroup);

            // begin filling the subaction list
            subActionList.BeginUpdate();
            subActionList.Items.Clear();
            for (int i = 0; i < data.Length;)
            {
                // get subaction
                var sa = SubactionManager.GetSubaction((byte)(data[i]), SubactionGroup);

                // create new script node
                var sas = new SubActionScript(SubactionGroup);
                
                // store any pointers within this subaction
                foreach (var r in script._struct.References)
                {
                    if (r.Key >= i && r.Key < i + sa.ByteSize)
                        if (sas.Reference != null)
                            throw new NotSupportedException("Multiple References not supported");
                        else
                            sas.Reference = r.Value;
                }

                // copy subaction data to script node
                var sub = new byte[sa.ByteSize];

                if (i + sub.Length > data.Length)
                    break;

                for (int j = 0; j < sub.Length; j++)
                    sub[j] = data[i + j];
                
                i += sa.ByteSize;

                sas.data = sub;

                // add new script node
                subActionList.Items.Add(sas);

                // if end of script then stop reading
                if (sa.Code == 0)
                    break;
            }
            subActionList.EndUpdate();
        }


        #region Undo
        private Stack<byte[]> UndoDataStack = new Stack<byte[]>();
        private Stack<Dictionary<int, HSDStruct>> UndoReferenceStack = new Stack<Dictionary<int, HSDStruct>>();

        private void ClearUndoStack()
        {
            UndoDataStack.Clear();
            UndoReferenceStack.Clear();
        }

        private void AddActionToUndo()
        {
            if (actionList.SelectedItem is Action a)
            {
                UndoDataStack.Push(a._struct.GetData());
                UndoReferenceStack.Push(new Dictionary<int, HSDStruct>(a._struct.References));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Undo()
        {
            if (UndoDataStack.Count > 0 && actionList.SelectedItem is Action a)
            {
                a._struct.SetData(UndoDataStack.Pop());
                
                a._struct.References.Clear();
                var prevRef = UndoReferenceStack.Pop();
                foreach (var v in prevRef)
                    a._struct.References.Add(v.Key, v.Value);

                RefreshSubactionList(a);
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        private void SaveSubactionChanges()
        {
            if (actionList.SelectedItem is Action a)
            {
                AddActionToUndo();

                a._struct.References.Clear();
                _node.Accessor._s.SetInt32(0x18 * a.Index + 0x10, (int)a.Flags);

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
                SubactionProcess.SetStruct(a._struct, SubactionGroup);
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
            AllScripts.Insert(actionList.SelectedIndex, action);
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
            //buttonRemove.Visible = subActionList.SelectedIndex != -1;
            //buttonUp.Visible = subActionList.SelectedIndex != -1;
            //buttonDown.Visible = subActionList.SelectedIndex != -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            var ac = new SubActionScript(SubactionGroup)
            {
                data = new byte[] { 0, 0, 0, 0 }
            };
            subActionList.Items.Insert(subActionList.SelectedIndex + 1, ac);
            subActionList.SelectedItem = null;
            subActionList.SelectedItem = ac;

            SaveSubactionChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRemove_Click(object sender, EventArgs e)
        {
            RemoveSelected();
        }

        private void RemoveSelected()
        {
            subActionList.BeginUpdate();
            if (subActionList.SelectedItems.Count != 0)
            {
                var list = new List<object>();
                foreach (var v in subActionList.SelectedItems)
                    list.Add(v);
                foreach (var v in list)
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
            var selectedIndex = subActionList.SelectedIndex;

            if (subActionList.SelectedItem is SubActionScript sa)
            {
                using (SubActionPanel p = new SubActionPanel(AllScripts))
                {
                    p.LoadData(sa.data, sa.Reference, SubactionGroup);
                    if (p.ShowDialog() == DialogResult.OK)
                    {
                        sa.data = p.Data;

                        if (sa.data.Length > 0 && SubactionManager.GetSubaction((byte)(sa.data[0]), SubactionGroup).HasPointer)
                            sa.Reference = p.Reference;
                        else
                            sa.Reference = null;

                        subActionList.Items[selectedIndex] = subActionList.SelectedItem;

                        SaveSubactionChanges();

                        subActionList.Invalidate();
                    }
                }

                // redraw item
                var item = subActionList.SelectedItem;
                var itempos = subActionList.SelectedIndex;
                subActionList.Items.Remove(item);
                subActionList.Items.Insert(itempos, item);

                subActionList.SelectedIndex = selectedIndex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RedrawActionItems()
        {
            var items = new List<object>();
            foreach (var i in subActionList.Items)
                items.Add(i);

            subActionList.Items.Clear();
            foreach(var i in items)
                subActionList.Items.Add(i);
        }

        #region ToolStrip

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGoto_Click(object sender, EventArgs e)
        {
            if(cbReference.SelectedItem is Action a)
            {
                actionList.SelectedIndex = actionList.Items.IndexOf(a);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subActionList_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if(e.Index != -1 && subActionList.Items[e.Index] is SubActionScript script)
            {
                var length = script.Parameters.Count();
                e.ItemHeight = subActionList.Font.Height * (toolStripComboBox1.SelectedIndex != 0 ? 1 : length + 1);
                e.ItemHeight = Math.Min(e.ItemHeight, 255); // limit
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subActionList_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if(e.Index != -1)
            {
                if(subActionList.Items[e.Index] is SubActionScript script)
                {
                    var sa = SubactionManager.GetSubaction(script.data[0], SubactionGroup);
                    e.Graphics.DrawString(e.Index + ". " + script.Name + (toolStripComboBox1.SelectedIndex == 2 ? "(" + string.Join(", ", script.Parameters) + ")" : ""), e.Font, new SolidBrush(sa.IsCustom ? Color.DarkOrange : Color.DarkBlue), e.Bounds);
                    int i = 1;
                    if (toolStripComboBox1.SelectedIndex == 0)
                        foreach (var v in script.Parameters)
                        {
                            if (e.Bounds.Y + e.Font.Height * i >= e.Bounds.Y + e.Bounds.Height)
                                break;
                            var bottomRect = new Rectangle(new Point(e.Bounds.X, e.Bounds.Y + e.Font.Height * i), new Size(e.Bounds.Width, e.Bounds.Height));
                            e.Graphics.DrawString("\t" + v, e.Font, new SolidBrush(e.ForeColor), bottomRect);
                            i++;
                        }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCopy_Click(object sender, EventArgs e)
        {
            CopySelected();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPaste_Click(object sender, EventArgs e)
        {
            Paste();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ReplacePaste()
        {
            var selectedIndex = subActionList.SelectedIndex;
            if (selectedIndex != -1)
            {
                RemoveSelected();
                subActionList.SelectedIndex = selectedIndex - 1;
                Paste();
                subActionList.SelectedIndex = selectedIndex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CopySelected()
        {
            // Lets say its my data format
            Clipboard.Clear();

            // get collections of selected scripts
            var scripts = new List<SubActionScript>();
            foreach (SubActionScript scr in subActionList.SelectedItems)
                scripts.Add(scr);

            // Put data into clipboard
            Clipboard.SetData(typeof(List<SubActionScript>).FullName, scripts);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Paste()
        {
            // get insert index
            var index = subActionList.SelectedIndex + 1;
            if (index == -1)
                index = 0;

            // Get data object from the clipboard
            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject != null)
            {
                // Check if a collection of Slides is in the clipboard
                string dataFormat = typeof(List<SubActionScript>).FullName;
                if (dataObject.GetDataPresent(dataFormat))
                {
                    // Retrieve slides from the clipboard
                    List<SubActionScript> scripts = dataObject.GetData(dataFormat) as List<SubActionScript>;
                    if (scripts != null)
                    {
                        // insert scripts
                        scripts.Reverse();
                        foreach (var v in scripts)
                            // only paste subactions the belong to this group
                            if(v.GetGroup() == SubactionGroup)
                                subActionList.Items.Insert(index, v.Clone());

                        SaveSubactionChanges();
                    }
                }
            }
            
        }

        /// <summary>
        /// Hot keys
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subActionList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Oemplus)
                buttonAdd_Click(null, null);

            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.OemMinus)
                RemoveSelected();

            if (e.Control)
            {
                if (e.KeyCode == Keys.X)
                {
                    CopySelected();
                    RemoveSelected();
                }

                if (e.KeyCode == Keys.C)
                    CopySelected();

                if (e.KeyCode == Keys.V)
                {
                    if (e.Shift)
                        ReplacePaste();
                    else
                        Paste();
                }

                if (e.KeyCode == Keys.Z)
                    Undo();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCut_Click(object sender, EventArgs e)
        {
            CopySelected();
            RemoveSelected();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RedrawActionItems();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleScriptViewToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            RedrawActionItems();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonReplace_Click(object sender, EventArgs e)
        {
            ReplacePaste();
        }

        #endregion

        #region Special Functions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearAllActionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure?\nThis operation cannot be undone", "Clear All Scripts", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                foreach(var script in AllScripts)
                {
                    script._struct.References.Clear();
                    script._struct.SetData(new byte[4]);
                }

                actionList.SelectedIndex = 0;
            }
        }

        #endregion
        
        #region Rendering
        
        private ViewportControl viewport;

        private JOBJManager JOBJManager = new JOBJManager();
        private JOBJManager ThrowDummyManager = new JOBJManager();

        public DrawOrder DrawOrder => DrawOrder.Last;

        private byte[] AJBuffer;

        private SubactionProcessor SubactionProcess = new SubactionProcessor();

        private List<SBM_Hurtbox> Hurtboxes = new List<SBM_Hurtbox>();

        private HurtboxRenderer HurtboxRenderer = new HurtboxRenderer();

        private string AnimationName = "";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadPlayerFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var aFile = MainForm.Instance.FilePath.Replace(".dat", "AJ.dat");
            var cFile = MainForm.Instance.FilePath.Replace(".dat", "Nr.dat");

            bool openFiles = true;
            if(System.IO.File.Exists(aFile) && System.IO.File.Exists(cFile))
            {
                var r = MessageBox.Show($"Load {System.IO.Path.GetFileName(aFile)} and {System.IO.Path.GetFileName(cFile)}", "Open Files", MessageBoxButtons.YesNoCancel);

                if (r == DialogResult.Cancel)
                    return;

                if (r == DialogResult.Yes)
                    openFiles = false;
            }
            if (openFiles)
            {
                cFile = FileIO.OpenFile("Fighter Costume (Pl**Nr.dat)|*.dat");
                if (cFile == null)
                    return;
                aFile = FileIO.OpenFile("Fighter Animation (Pl**AJ.dat)|*.dat");
                if (aFile == null)
                    return;
            }

            var modelFile = new HSDRawFile(cFile);
            if (modelFile.Roots[0].Data is HSD_JOBJ jobj)
                JOBJManager.SetJOBJ(jobj);
            else
                return;

            JOBJManager.ModelScale = ModelScale;
            JOBJManager.DOBJManager.HiddenDOBJs.Clear();
            JOBJManager.settings.RenderBones = false;

            ResetModelVis();

            AJBuffer = System.IO.File.ReadAllBytes(aFile);

            previewBox.Visible = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResetModelVis()
        {
            var plDat = FighterData;

            if (plDat != null && plDat.ModelLookupTables != null && JOBJManager.JointCount != 0)
            {
                JOBJManager.DOBJManager.HiddenDOBJs.Clear();

                // only show struct 0 vis
                for (int i = 0; i < plDat.ModelLookupTables.CostumeVisibilityLookups[0].HighPoly.Length; i++)
                    SetModelVis(i, 0);

                // hide low poly
                foreach (var lut in plDat.ModelLookupTables.CostumeVisibilityLookups[0].LowPoly.Array)
                    SetModelVis(lut, -1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="structid"></param>
        /// <param name="objectid"></param>
        private void SetModelVis(int structid, int objectid)
        {
            var plDat = FighterData;

            if (plDat.ModelLookupTables != null && JOBJManager.JointCount != 0)
                SetModelVis(plDat.ModelLookupTables.CostumeVisibilityLookups[0].HighPoly[structid], objectid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="structid"></param>
        /// <param name="objectid"></param>
        private void SetModelVis(SBM_LookupTable lookuptable, int objectid)
        {
            var plDat = FighterData;

            if (plDat.ModelLookupTables != null && JOBJManager.JointCount != 0 && lookuptable.LookupEntries != null)
            {
                var structs = lookuptable.LookupEntries.Array;

                for (int i = 0; i < structs.Length; i++)
                {
                    foreach (var v in structs[i].Entries)
                        if (i == objectid)
                            JOBJManager.ShowDOBJ(v);
                        else
                            JOBJManager.HideDOBJ(v);
                }
            }
        }

        /// <summary>
        /// Calcuates the previous state hitboxes positions and returns them as a dictionary
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, Vector3> CalculatePreviousState()
        {
            if (viewport.Frame == 0 || !interpolationToolStripMenuItem.Checked)
                return null;

            Dictionary<int, Vector3> previousPosition = new Dictionary<int, Vector3>();

            JOBJManager.Frame = viewport.Frame - 1;
            JOBJManager.UpdateNoRender();
            SubactionProcess.SetFrame(viewport.Frame - 1);

            foreach (var hb in SubactionProcess.Hitboxes)
            {
                var boneID = hb.BoneID;
                if (boneID == 0)
                    boneID = 1;
                var transform = Matrix4.CreateTranslation(hb.Point1) * JOBJManager.GetWorldTransform(boneID);
                transform = transform.ClearScale();
                var pos = Vector3.TransformPosition(Vector3.Zero, transform);
                previousPosition.Add(hb.ID, pos);
            }

            return previousPosition;
        }

        private static Vector3 ThrowDummyColor = new Vector3(0, 1, 1);
        private static Vector3 HitboxColor = new Vector3(1, 0, 0);
        private static Vector3 GrabboxColor = new Vector3(1, 0, 1);
        private float ModelScale = 1f;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            Dictionary<int, Vector3> previousPosition = CalculatePreviousState();

            SubactionProcess.SetFrame(viewport.Frame);

            JOBJManager.DOBJManager.OverlayColor = SubactionProcess.OverlayColor;

            JOBJManager.Frame = viewport.Frame;

            JOBJManager.settings.RenderBones = bonesToolStripMenuItem.Checked;

            // character invisibility
            if (SubactionProcess.CharacterInvisibility || !modelToolStripMenuItem.Checked)
                JOBJManager.UpdateNoRender();
            else
                JOBJManager.Render(cam);

            // hurtbox collision
            if (hurtboxesToolStripMenuItem.Checked)
                HurtboxRenderer.Render(JOBJManager, Hurtboxes, null, SubactionProcess.BoneCollisionStates, SubactionProcess.BodyCollisionState);
            
            // hitbox collision
            foreach (var hb in SubactionProcess.Hitboxes)
            {
                var boneID = hb.BoneID;
                if (boneID == 0)
                    if (JOBJManager.GetJOBJ(1).Child == null) // special case for character like mewtwo with a leading bone
                        boneID = 2;
                    else
                        boneID = 1;

                var transform = Matrix4.CreateTranslation(hb.Point1) * JOBJManager.GetWorldTransform(boneID);

                transform = transform.ClearScale();

                float alpha = 0.4f;
                Vector3 hbColor = HitboxColor;

                if (hb.Element == 8)
                    hbColor = GrabboxColor;

                // drawing a capsule takes more processing power, so only draw it if necessary
                if (interpolationToolStripMenuItem.Checked && previousPosition != null && previousPosition.ContainsKey(hb.ID))
                {
                    var pos = Vector3.TransformPosition(Vector3.Zero, transform);
                    var cap = new Capsule(pos, previousPosition[hb.ID], hb.Size);
                    cap.Draw(Matrix4.Identity, new Vector4(hbColor, alpha));
                }
                else
                {
                    DrawShape.DrawSphere(transform, hb.Size, 16, 16, hbColor, alpha);
                }
                if (hitboxInfoToolStripMenuItem.Checked)
                {
                    if (hb.Angle != 361)
                        DrawShape.DrawAngleLine(cam, transform, hb.Size, MathHelper.DegreesToRadians(hb.Angle));
                    else
                        DrawShape.DrawSakuraiAngle(cam, transform, hb.Size);
                    GLTextRenderer.RenderText(cam, hb.ID.ToString(), transform, StringAlignment.Center, true);
                }
            }

            // environment collision
            if (ECB != null)
            {
                var topN = JOBJManager.GetWorldTransform(1).ExtractTranslation();

                var bone1 = Vector3.TransformPosition(Vector3.Zero, JOBJManager.GetWorldTransform(ECB.ECBBone1));
                var bone2 = Vector3.TransformPosition(Vector3.Zero, JOBJManager.GetWorldTransform(ECB.ECBBone2));
                var bone3 = Vector3.TransformPosition(Vector3.Zero, JOBJManager.GetWorldTransform(ECB.ECBBone3));
                var bone4 = Vector3.TransformPosition(Vector3.Zero, JOBJManager.GetWorldTransform(ECB.ECBBone4));
                var bone5 = Vector3.TransformPosition(Vector3.Zero, JOBJManager.GetWorldTransform(ECB.ECBBone5));
                var bone6 = Vector3.TransformPosition(Vector3.Zero, JOBJManager.GetWorldTransform(ECB.ECBBone6));

                var minx = float.MaxValue;
                var miny = float.MaxValue;
                var maxx = float.MinValue;
                var maxy = float.MinValue;

                foreach (var p in new Vector3[] { bone1, bone2, bone3, bone4, bone5, bone6 })
                {
                    minx = Math.Min(minx, p.Z);
                    maxx = Math.Max(maxx, p.Z);
                    miny = Math.Min(miny, p.Y);
                    maxy = Math.Max(maxy, p.Y);
                }

                // ecb diamond
                if (eCBToolStripMenuItem.Checked)
                {
                    DrawShape.DrawECB(topN, minx, miny, maxx, maxy, groundECH.Checked);
                }

                // ledge grav
                if (ledgeGrabBoxToolStripMenuItem.Checked)
                {
                    var correct = Math.Abs(minx - maxx) / 2;

                    //behind
                    DrawShape.DrawLedgeBox(
                        topN.Z,
                        topN.Y + ECB.VerticalOffsetFromTop - ECB.VerticalScale / 2,
                        topN.Z - (correct + ECB.HorizontalScale),
                        topN.Y + ECB.VerticalOffsetFromTop + ECB.VerticalScale / 2,
                        Color.Red);

                    // in front
                    DrawShape.DrawLedgeBox(
                        topN.Z,
                        topN.Y + ECB.VerticalOffsetFromTop - ECB.VerticalScale / 2,
                        topN.Z + correct + ECB.HorizontalScale,
                        topN.Y + ECB.VerticalOffsetFromTop + ECB.VerticalScale / 2,
                        Color.Blue);
                }
            }

            // throw dummy
            if(throwModelToolStripMenuItem.Checked && !SubactionProcess.ThrownFighter && ThrowDummyManager.JointCount > 0)
            {
                if(viewport.Frame < ThrowDummyManager.Animation.FrameCount)
                    ThrowDummyManager.Frame = viewport.Frame;
                ThrowDummyManager.SetWorldTransform(4, JOBJManager.GetWorldTransform(JOBJManager.JointCount - 2));
                ThrowDummyManager.Render(cam, false);

                DrawShape.DrawSphere(ThrowDummyManager.GetWorldTransform(35), 1.5f, 16, 16, ThrowDummyColor, 0.5f);
                DrawShape.DrawSphere(ThrowDummyManager.GetWorldTransform(4), 1.5f, 16, 16, ThrowDummyColor, 0.5f);
                DrawShape.DrawSphere(ThrowDummyManager.GetWorldTransform(10), 1f, 16, 16, ThrowDummyColor, 0.5f);
                DrawShape.DrawSphere(ThrowDummyManager.GetWorldTransform(15), 1f, 16, 16, ThrowDummyColor, 0.5f);
                DrawShape.DrawSphere(ThrowDummyManager.GetWorldTransform(22), 1f, 16, 16, ThrowDummyColor, 0.5f);
                DrawShape.DrawSphere(ThrowDummyManager.GetWorldTransform(40), 1f, 16, 16, ThrowDummyColor, 0.5f);
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        private void LoadAnimation(int offset, int size)
        {
            JOBJManager.SetFigaTree(null);

            if (size == 0 || AJBuffer == null || offset + size > AJBuffer.Length)
                return;

            var f = new byte[size];
            Array.Copy(AJBuffer, offset, f, 0, size);
            var anim = new HSDRawFile(f);
            if(anim.Roots[0].Data is HSD_FigaTree tree)
            {
                var name = new Action() { Text = anim.Roots[0].Name }.ToString();

                JOBJManager.SetFigaTree(tree);
                viewport.MaxFrame = tree.FrameCount;

                ThrowDummyManager.CleanupRendering();
                ThrowDummyManager = new JOBJManager();

                AnimationName = name;

                if (name.Contains("Throw") && !name.Contains("Taro"))
                {
                    // find thrown anim
                    Action throwAction = null;
                    foreach (Action a in actionList.Items)
                    {
                        if(a.Text.Contains("Taro") && a.Text.Contains(name) && !a.Text.Equals(anim.Roots[0].Name))
                        {
                            throwAction = a;
                            break;
                        }
                    } 

                    if (throwAction != null)
                    {
                        // load throw dummy
                        ThrowDummyManager.SetJOBJ(DummyThrowModel.GenerateThrowDummy());

                        // load throw animation
                        var tf = new byte[throwAction.AnimSize];
                        Array.Copy(AJBuffer, throwAction.AnimOffset, tf, 0, tf.Length);
                        var tanim = new HSDRawFile(tf);
                        if (tanim.Roots[0].Data is HSD_FigaTree tree2)
                            ThrowDummyManager.SetFigaTree(tree2);
                    }

                }
            }
        }

        #endregion

        private Timer clickTimer;
        private Point MousePoint;

        private void timer_Tick(object sender, EventArgs e)
        {
            if (MousePoint == Cursor.Position && subActionList.SelectedItems.Count == 1)
            {
                subActionList.DoDragDrop(subActionList.SelectedItems, DragDropEffects.Move);
                clickTimer.Stop();
            }
            MousePoint = Cursor.Position;
        }

        private void subActionList_MouseDown(object sender, MouseEventArgs e)
        {
            if (subActionList.SelectedItem == null) return;

            MousePoint = Cursor.Position;
            clickTimer.Start();
        }

        private void subActionList_DragDrop(object sender, DragEventArgs e)
        {
            Point point = subActionList.PointToClient(new Point(e.X, e.Y));
            int index = subActionList.IndexFromPoint(point);
            if (index < 0) index = subActionList.Items.Count - 1;

            List<object> data = new List<object>();

            foreach (var i in subActionList.SelectedItems)
                data.Add(i);

            foreach(var i in data)
                subActionList.Items.Remove(i);

            foreach(var i in data)
            {
                subActionList.Items.Insert(index, i);
            }

            SaveSubactionChanges();

            subActionList.SelectedIndex = index;
        }

        private void subActionList_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void subActionList_MouseUp(object sender, MouseEventArgs e)
        {
            clickTimer.Stop();
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            SaveSubactionChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void actionList_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                e.DrawBackground();

                var brush = ApplicationSettings.SystemWindowTextColorBrush;

                var itemText = ((ListBox)sender).Items[e.Index].ToString();

                if (!itemText.StartsWith("Subroutine"))
                {
                    var indText = e.Index.ToString() + ".";

                    var indSize = TextRenderer.MeasureText(indText, e.Font);
                    var indexBound = new Rectangle(e.Bounds.X, e.Bounds.Y, indSize.Width, indSize.Height);
                    var textBound = new Rectangle(e.Bounds.X + indSize.Width, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);

                    e.Graphics.DrawString(indText, e.Font, ApplicationSettings.SystemGrayTextColorBrush, indexBound, StringFormat.GenericDefault);
                    e.Graphics.DrawString(itemText, e.Font, brush, textBound, StringFormat.GenericDefault);
                }
                else
                    e.Graphics.DrawString(itemText, e.Font, brush, e.Bounds, StringFormat.GenericDefault);

                e.DrawFocusRectangle();
            }
            catch
            {

            }
        }
    }
}
