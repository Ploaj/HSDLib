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

namespace HSDRawViewer.GUI
{
    public partial class SubactionEditor : DockContent, EditorBase, IDrawable
    {
        public class Action
        {
            public HSDStruct _struct;

            public string Text;

            public int AnimOffset;
            public int AnimSize;

            public uint Flags;
            public int Index;

            [Category("Display Flags"), DisplayName("Flags")]
            public string BitFlags { get => Flags.ToString("X"); set { uint v = Flags; uint.TryParse(value, out v); Flags = v; } }

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

        public class SubActionScript
        {
            public byte[] data;

            public HSDStruct Reference;

            private SubactionGroup SubactionGroup = SubactionGroup.Fighter;

            public SubActionScript(SubactionGroup SubactionGroup)
            {
                this.SubactionGroup = SubactionGroup;
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

        public Type[] SupportedTypes => new Type[] { typeof(SBM_SubActionTable), typeof(SBM_FighterSubactionData), typeof(SBM_ItemSubactionData) };

        public DataNode Node
        {
            get => _node;
            set
            {
                _node = value;
                if (value.Accessor is SBM_ItemSubactionData suidata)
                {
                    SubactionGroup = SubactionGroup.Item;
                    SBM_FighterSubAction[] su = new SBM_FighterSubAction[]
                    {
                        new SBM_FighterSubAction()
                        {
                            SubAction = suidata,
                            Name = "Script"
                        }
                    };

                    LoadActions(su);
                    RefreshActionList();
                }else
                if (value.Accessor is SBM_FighterSubactionData sudata)
                {
                    SBM_FighterSubAction[] su = new SBM_FighterSubAction[]
                    {
                        new SBM_FighterSubAction()
                        {
                            SubAction = sudata,
                            Name = "Script"
                        }
                    };

                    LoadActions(su);
                    RefreshActionList();
                }else
                if(value.Accessor is SBM_SubActionTable SubactionTable)
                {
                    LoadActions(SubactionTable.Subactions);
                    RefreshActionList();

                    if (Node.Parent is DataNode parent)
                    {
                        if (parent.Accessor is SBM_FighterData plDat)
                        {
                            if (plDat.Hurtboxes != null)
                                Hurtboxes.AddRange(plDat.Hurtboxes.Hurtboxes);

                            if (plDat.ModelLookupTables != null)
                            {
                                var lowpolyStruct = plDat.ModelLookupTables
                                    ?._s.GetReference<HSDAccessor>(0x04)
                                    ?._s.GetReference<HSDAccessor>(0x04)
                                    ?._s.GetReference<HSDAccessor>(0x04)?._s;

                                var tab = lowpolyStruct?.GetReference<HSDAccessor>(0x04)?._s;

                                if (lowpolyStruct != null && tab != null)
                                {
                                    for (int i = 0; i < lowpolyStruct.GetInt32(0); i++)
                                    {
                                        HiddenDOBJIndices.Add(tab.GetByte(i));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private DataNode _node;

        private readonly List<Action> AllScripts = new List<Action>();

        public SubactionGroup SubactionGroup = SubactionGroup.Fighter;

        /// <summary>
        /// 
        /// </summary>
        public SubactionEditor()
        {
            InitializeComponent();

            panel1.Visible = false;

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
                JOBJManager.ClearRenderingCache();
                viewport.Dispose();
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Subactions"></param>
        private void LoadActions(SBM_FighterSubAction[] Subactions)
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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="script"></param>
        private void RefreshSubactionList(Action script)
        {
            var data = script._struct.GetData();

            SubactionProcess.SetStruct(script._struct, SubactionGroup);

            subActionList.Items.Clear();
            subActionList.BeginUpdate();
            for (int i = 0; i < data.Length;)
            {
                var sa = SubactionManager.GetSubaction((byte)(data[i]), SubactionGroup);

                var sas = new SubActionScript(SubactionGroup);

                foreach (var r in script._struct.References)
                {
                    if (r.Key >= i && r.Key < i + sa.ByteSize)
                        if (sas.Reference != null)
                            throw new NotSupportedException("Multiple References not supported");
                        else
                            sas.Reference = r.Value;
                }

                var sub = new byte[sa.ByteSize];

                if (i + sub.Length > data.Length)
                {
                    break;
                }

                for (int j = 0; j < sub.Length; j++)
                    sub[j] = data[i + j];

                sas.data = sub;

                subActionList.Items.Add(sas);

                i += sa.ByteSize;
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
                e.ItemHeight = subActionList.Font.Height *
                    (toolStripComboBox1.SelectedIndex != 0
                    ? 1 :
                    (script.Parameters.Equals("") ? 1 : length + 1));
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

        private List<SubActionScript> CopiedScripts = new List<SubActionScript>();

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
            CopiedScripts.Clear();
            foreach (SubActionScript scr in subActionList.SelectedItems)
                CopiedScripts.Add(scr);
            CopiedScripts.Reverse();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Paste()
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

        public DrawOrder DrawOrder => DrawOrder.Last;

        private byte[] AJBuffer;

        private SubactionProcessor SubactionProcess = new SubactionProcessor();

        private List<SBM_Hurtbox> Hurtboxes = new List<SBM_Hurtbox>();

        private HurtboxRenderer HurtboxRenderer = new HurtboxRenderer();

        private List<int> HiddenDOBJIndices = new List<int>();

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

            JOBJManager.DOBJManager.HiddenDOBJs.Clear();
            JOBJManager.HideDOBJs(HiddenDOBJIndices);

            AJBuffer = System.IO.File.ReadAllBytes(aFile);

            JOBJManager.RenderBones = false;

            previewBox.Visible = true;
        }

        /// <summary>
        /// Calcuates the previous state hitboxes positions and returns them as a dictionary
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, Vector3> CalculatePreviousState()
        {
            if (viewport.Frame == 0 || !renderHitboxInterpolationToolStripMenuItem.Checked)
                return null;

            Dictionary<int, Vector3> previousPosition = new Dictionary<int, Vector3>();

            JOBJManager.Frame = viewport.Frame - 1;
            JOBJManager.UpdateNoRender();
            SubactionProcess.SetFrame(viewport.Frame - 1);

            foreach (var hb in SubactionProcess.Hitboxes)
            {
                var transform = Matrix4.CreateTranslation(hb.Point1) * JOBJManager.GetWorldTransform(hb.BoneID).ClearScale();
                var pos = Vector3.TransformPosition(Vector3.Zero, transform);
                previousPosition.Add(hb.ID, pos);
            }

            return previousPosition;
        }

        private static Vector3 HitboxColor = new Vector3(1, 0, 0);
        private static Vector3 GrabboxColor = new Vector3(1, 0, 1);

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

            if (SubactionProcess.CharacterInvisibility)
                JOBJManager.UpdateNoRender();
            else
                JOBJManager.Render(cam);

            if (renderHurtboxsToolStripMenuItem.Checked)
                HurtboxRenderer.Render(JOBJManager, Hurtboxes, null, SubactionProcess.BoneCollisionStates, SubactionProcess.BodyCollisionState);
            
            foreach (var hb in SubactionProcess.Hitboxes)
            {
                var transform = Matrix4.CreateTranslation(hb.Point1 / 2) * JOBJManager.GetWorldTransform(hb.BoneID).ClearScale();

                float alpha = 0.4f;
                Vector3 hbColor = HitboxColor;

                if (hb.Element == 8)
                    hbColor = GrabboxColor;

                // drawing a capsule takes more processing power, so only draw it if necessary
                if (renderHitboxInterpolationToolStripMenuItem.Checked && previousPosition.ContainsKey(hb.ID))
                {
                    var pos = Vector3.TransformPosition(Vector3.Zero, transform);
                    var cap = new Capsule(pos, previousPosition[hb.ID], hb.Size);
                    cap.Draw(Matrix4.Identity, new Vector4(hbColor, alpha));
                }
                else
                {
                    DrawShape.DrawSphere(transform, hb.Size, 16, 16, hbColor, alpha);
                }
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
                JOBJManager.SetFigaTree(tree);
                viewport.MaxFrame = tree.FrameCount;
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
    }
}
