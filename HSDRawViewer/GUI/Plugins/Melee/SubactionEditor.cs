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
using System.IO;
using System.Text.RegularExpressions;
using HSDRawViewer.GUI.Extra;

namespace HSDRawViewer.GUI
{
    public partial class SubactionEditor : DockContent, EditorBase, IDrawable
    {
        public class Action
        {
            public HSDStruct _struct;

            private string DisplayText;

            [Category("Animation"), DisplayName("Figatree Symbol")]
            public string Symbol
            {
                get => _symbol;
                set
                {
                    _symbol = value;

                    if(!string.IsNullOrEmpty(_symbol))
                        DisplayText = Regex.Replace(_symbol.Replace("_figatree", ""), @"Ply.*_Share_ACTION_", "");
                }
            }

            private string _symbol;

            public bool Subroutine = false;

            public int Index;
            
            public int AnimOffset;
            
            public int AnimSize;

            public uint Flags;

            [Category("Display Flags"), DisplayName("Flags")]
            public string BitFlags { get => Flags.ToString("X"); set { uint v = Flags; uint.TryParse(value, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out v); Flags = v; } }
            
            [Category("Flags"), DisplayName("Utilize animation-induced physics")]
            public bool AnimInducedPhysics { get => (Flags & 0x80000000) != 0; set => Flags = (uint)((Flags & ~0x80000000) | 0x80000000); }

            [Category("Flags"), DisplayName("Loop Animation")]
            public bool LoopAnimation { get => (Flags & 0x40000000) != 0; set => Flags = (uint)((Flags & ~0x40000000) | 0x40000000); }

            [Category("Flags"), DisplayName("Unknown")]
            public bool Unknown { get => (Flags & 0x20000000) != 0; set => Flags = (uint)((Flags & ~0x20000000) | 0x20000000); }

            [Category("Flags"), DisplayName("Unknown Flag")]
            public bool UnknownFlag { get => (Flags & 0x10000000) != 0; set => Flags = (uint)((Flags & ~0x10000000) | 0x10000000); }

            [Category("Flags"), DisplayName("Disable Dynamics")]
            public bool DisableDynamics { get => (Flags & 0x08000000) != 0; set => Flags = (uint)((Flags & ~0x08000000) | 0x08000000); }

            [Category("Flags"), DisplayName("Unknown TransN Update")]
            public bool TransNUpdate { get => (Flags & 0x04000000) != 0; set => Flags = (uint)((Flags & ~0x04000000) | 0x04000000); }

            [Category("Flags"), DisplayName("TransN Affected by Model Scale")]
            public bool AffectModelScale { get => (Flags & 0x02000000) != 0; set => Flags = (uint)((Flags & ~0x02000000) | 0x02000000); }

            [Category("Flags"), DisplayName("Additional Bone Value")]
            public uint AdditionalBone { get => (Flags & 0x003FFE00) >> 9; set => Flags = (uint)((Flags & ~0x003FFE00) | ((value << 9) & 0x003FFE00)); }
            
            [Category("Flags"), DisplayName("Disable Blend on Bone Index")]
            public uint BoneIndex { get => (Flags & 0x1C0) >> 7; set => Flags = (uint)(Flags & ~0x1C0) | ((value << 7) & 0x1C0); }

            [Category("Flags"), DisplayName("Character ID")]
            public uint CharIDCheck { get => Flags & 0x3F; set => Flags = (Flags & 0xFFFFFFC0) | (value & 0x3F); }

            public override string ToString()
            {
                return DisplayText == null ? (Subroutine ? "Subroutine_" : "Function_") + Index : DisplayText;
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

            public IEnumerable<string> GetParamsAsString(SubactionEditor editor)
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
                        if (editor != null && editor.AllActions.Find(e => e._struct == Reference) != null)
                            yield return ("&" + editor.AllActions.Find(e => e._struct == Reference).ToString());
                        else
                            yield return ("POINTER->(Edit To View)");
                    else
                    if (param.IsFloat)
                        yield return (param.Name +
                            " : " +
                            BitConverter.ToSingle(BitConverter.GetBytes(value), 0));
                    else
                        yield return (param.Name +
                            " : " +
                            (param.Hex ? "0x" + value.ToString("X") : value.ToString()));
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

            public override string ToString()
            {
                return Name +  "(" + string.Join(", ", GetParamsAsString(null)) + ")";
            }

            public string Serialize(SubactionEditor editor)
            {
                return Name + "(" + string.Join(", ", GetParamsAsString(editor)) + ")";
            }

            public static void Deserialize(string script)
            {
                // TODO:
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

                    // disable fighter only stuff
                    loadPlayerFilesToolStripMenuItem.Enabled = false;
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
                            ModelScale = plDat.ParametersCommon.ModelScale;

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

        private readonly List<Action> AllActions = new List<Action>();

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
                SaveFile();
                JOBJManager.CleanupRendering();
                viewport.Dispose();
                _animEditor.Dispose();
            };

            _animEditor.FormClosing += (sender, args) =>
            {
                if(MessageBox.Show("Save Changes Made to Animation?", "Save Animation?", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                    SaveAnimation();
            };
            
            SubactionProcess.UpdateVISMethod = SetModelVis;
            SubactionProcess.AnimateMaterialMethod = AnimateMaterial;
            SubactionProcess.AnimateModelMethod = AnimateModel;
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
                    aHash.Add(v.SubAction._s);

                AllActions.Add(new Action()
                {
                    _struct = v.SubAction._s,
                    AnimOffset = v.AnimationOffset,
                    AnimSize = v.AnimationSize,
                    Flags = v.Flags,
                    Symbol = v.Name,
                });

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
                    AllActions.Add(new Action()
                    {
                        _struct = v,
                        Subroutine = true
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
            var actionIndex = 0;
            var routineIndex = 0;
            foreach (var sa in AllActions)
            {
                if(sa.Subroutine)
                    sa.Index = routineIndex++;
                else
                    sa.Index = actionIndex++;
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
            var references = AllActions.FindAll(e=>e._struct.References.ContainsValue(script._struct));

            cbReference.Items.Clear();
            foreach(var r in references)
            {
                cbReference.Items.Add(r);
            }

            panel1.Visible = (cbReference.Items.Count > 0);

            if (cbReference.Items.Count > 0)
                cbReference.SelectedIndex = 0;
            
            RefreshSubactionList(script);

            LoadAnimation(script.Symbol);

            ResetModelVis();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="script"></param>
        private void RefreshSubactionList(Action script)
        {
            // set the script for the subaction processer for rendering
            SubactionProcess.SetStruct(script._struct, SubactionGroup);

            // begin filling the subaction list
            subActionList.BeginUpdate();
            subActionList.Items.Clear();
            var scripts = GetScripts(script);
            foreach (var s in scripts)
                subActionList.Items.Add(s);
            subActionList.EndUpdate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public List<SubActionScript> GetScripts(Action script)
        {
            // get subaction data
            var data = script._struct.GetData();
            List<SubActionScript> scripts = new List<SubActionScript>();

            // process data
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
                scripts.Add(sas);

                // if end of script then stop reading
                if (sa.Code == 0)
                    break;
            }

            return scripts;
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
        private void SaveAllActionChanges()
        {
            for (int i = 0; i < AllActions.Count; i++)
                SaveActionChanges(i);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveSelectedActionChanges()
        {
            int index = actionList.SelectedIndex;
            if (index != -1)
                SaveActionChanges(index);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveActionChanges(int index)
        {
            var a = AllActions[index];
            AddActionToUndo();

            if (!a.Subroutine)
            {
                var ftcmd = new SBM_FighterCommand();
                ftcmd._s = _node.Accessor._s.GetEmbeddedStruct(0x18 * index, ftcmd.TrimmedSize);
                ftcmd.Name = a.Symbol;
                ftcmd.AnimationOffset = a.AnimOffset;
                ftcmd.AnimationSize = a.AnimSize;
                ftcmd.Flags = a.Flags;

                if (_node.Accessor._s.Length <= 0x18 * index + 0x18)
                    _node.Accessor._s.Resize(0x18 * index + 0x18);

                _node.Accessor._s.SetEmbededStruct(0x18 * index, ftcmd._s);
            }

            // compile subaction
            a._struct.References.Clear();
            List<byte> scriptData = new List<byte>();
            foreach (SubActionScript scr in subActionList.Items)
            {
                // TODO: are all references in this position?
                if (scr.Reference != null)
                {
                    a._struct.References.Add(scriptData.Count + 4, scr.Reference);
                }
                scriptData.AddRange(scr.data);
            }

            // update struct
            a._struct.SetData(scriptData.ToArray());
            SubactionProcess.SetStruct(a._struct, SubactionGroup);

        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveFile()
        {
            GenerateAndSaveNewAJFile();
        }

        /// <summary>
        /// 
        /// </summary>
        private void GenerateAndSaveNewAJFile()
        {
            // rendering files not loaded
            if (string.IsNullOrEmpty(AJFilePath))
                return;

            // make sure okay to overwrite
            if (MessageBox.Show($"Is it okay to overwrite {AJFilePath}?", "Save Animation File Changes?", MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                return;

            // collect used symbols from all actions
            var usedSymbols = AllActions.Select(e => e.Symbol);

            // generate new aj file
            Dictionary<string, Tuple<int, int>> animOffsets = new Dictionary<string, Tuple<int, int>>();

            using (MemoryStream ajBuffer = new MemoryStream())
            using (BinaryWriterExt w = new BinaryWriterExt(ajBuffer))
            {
                // collect used symbols
                foreach (var sym in usedSymbols)
                {
                    if(sym != null)
                    {
                        if (SymbolToAnimation.ContainsKey(sym) && !animOffsets.ContainsKey(sym))
                        {
                            // write animation
                            var anim = SymbolToAnimation[sym];
                            animOffsets.Add(sym, new Tuple<int, int>((int)ajBuffer.Position, anim.Length));
                            w.Write(anim);
                            w.Align(0x20, 0xFF);
                        }
                        else
                        if (!animOffsets.ContainsKey(sym))
                        {
                            // animation not found
                            MessageBox.Show($"\"{sym}\" animation not found", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            animOffsets.Add(sym, new Tuple<int, int>(0, 0));
                        }
                    }
                }

                // dump to file
                File.WriteAllBytes(AJFilePath, ajBuffer.ToArray());
            }


            int index = 0;
            foreach(var a in AllActions)
            {
                // don't write subroutines
                if (a.Subroutine)
                    continue;

                // get embedded script
                var ftcmd = new SBM_FighterCommand();
                ftcmd._s = _node.Accessor._s.GetEmbeddedStruct(0x18 * index, ftcmd.TrimmedSize);
                
                // update symbol name
                ftcmd.Name = a.Symbol;

                // offset
                var ofst = animOffsets[a.Symbol];

                // update action offset and size
                a.AnimOffset = ofst.Item1;
                a.AnimSize = ofst.Item2;

                // update file offset and size
                ftcmd.AnimationOffset = a.AnimOffset;
                ftcmd.AnimationSize = a.AnimSize;

                // resize if needed
                if (_node.Accessor._s.Length <= 0x18 * index + 0x18)
                    _node.Accessor._s.Resize(0x18 * index + 0x18);

                // update script
                _node.Accessor._s.SetEmbededStruct(0x18 * index, ftcmd._s);
                index++;
            }

            MainForm.Instance.SaveDAT();
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
        public int ActionCount
        {
            get
            {
                int index = 0;
                foreach (var v in AllActions)
                {
                    if (v.Subroutine)
                        break;
                    index++;
                }
                return index;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var data = new byte[] { 0, 0, 0, 0 };
            var action = new Action()
            {
                _struct = new HSDStruct(data)
            };

            var index = actionList.SelectedIndex;

            if (index == -1 || index > ActionCount)
                index = ActionCount;

            AllActions.Insert(index, action);
            RefreshActionList();
            actionList.SelectedItem = action;

            SaveAllActionChanges();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createNewSubroutineToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var data = new byte[] { 0x18, 0, 0, 0 };
            var action = new Action()
            {
                _struct = new HSDStruct(data),
                Subroutine = true
            };
            AllActions.Insert(actionList.Items.Count, action);
            RefreshActionList();
            actionList.SelectedItem = action;

            SaveAllActionChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteSelectedActionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(actionList.SelectedIndex != -1)
            {
                AllActions.RemoveAt(actionList.SelectedIndex);
                RefreshActionList();

                SaveAllActionChanges();
            }
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

            SaveSelectedActionChanges();
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

            SaveSelectedActionChanges();
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
                using (SubActionPanel p = new SubActionPanel(AllActions))
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

                        SaveSelectedActionChanges();

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
                var length = script.GetParamsAsString(null).Count();
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
                    e.Graphics.DrawString(e.Index + ". " + script.Name + (toolStripComboBox1.SelectedIndex == 2 ? "(" + string.Join(", ", script.GetParamsAsString(null)) + ")" : ""), e.Font, new SolidBrush(sa.IsCustom ? Color.DarkOrange : Color.DarkBlue), e.Bounds);
                    int i = 1;
                    if (toolStripComboBox1.SelectedIndex == 0)
                        foreach (var v in script.GetParamsAsString(null))
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
            SaveSelectedActionChanges();
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
            SaveSelectedActionChanges();
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

                        SaveSelectedActionChanges();
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
                foreach(var script in AllActions)
                {
                    script._struct.References.Clear();
                    script._struct.SetData(new byte[4]);
                }

                actionList.SelectedIndex = 0;
            }
        }

        #endregion
        
        #region Rendering
        
        public class ModelPartAnimations : IJointFrameModifier
        {
            private byte[] Entries;

            private JointAnimManager[] Anims;

            private int StartBone;

            public int AnimIndex = -1;

            public ModelPartAnimations(SBM_ModelPart part)
            {
                StartBone = part.StartingBone;

                Entries = new byte[part.Count];
                for(int i = 0; i < part.Count; i++)
                    Entries[i] = part.Entries[i];

                Anims = part.Anims.Array.Select(e => new JointAnimManager(e)).ToArray();
            }

            public bool OverrideAnim(float frame, int boneIndex, HSD_JOBJ jobj, ref float TX, ref float TY, ref float TZ, ref float RX, ref float RY, ref float RZ, ref float SX, ref float SY, ref float SZ)
            {
                // check if bone index is in entries
                if (AnimIndex == -1 || boneIndex < StartBone || boneIndex >= StartBone + Anims[0].NodeCount)
                    return false;

                // get anim for entry
                foreach(var e in Entries)
                    if(e == boneIndex && AnimIndex < Anims.Length)
                    {
                        var anim = Anims[AnimIndex];
                        anim.GetAnimatedState(0, boneIndex - StartBone, jobj, out TX, out TY, out TZ, out RX, out RY, out RZ, out SX, out SY, out SZ);
                        return true;
                    }

                return false;
            }
        }

        private ViewportControl viewport;

        private JOBJManager JOBJManager = new JOBJManager();
        private JOBJManager ThrowDummyManager = new JOBJManager();

        public DrawOrder DrawOrder => DrawOrder.Last;

        private string AJFilePath;
        private Dictionary<string, byte[]> SymbolToAnimation = new Dictionary<string, byte[]>();
        
        private ModelPartAnimations[] ModelPartsIndices;

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

            // try to automatically locate files
            bool openFiles = true;
            if (File.Exists(aFile) && File.Exists(cFile))
            {
                var r = MessageBox.Show($"Load {System.IO.Path.GetFileName(aFile)} and {System.IO.Path.GetFileName(cFile)}", "Open Files", MessageBoxButtons.YesNoCancel);

                if (r == DialogResult.Cancel)
                    return;

                if (r == DialogResult.Yes)
                    openFiles = false;
            }

            // find files to open
            if (openFiles)
            {
                cFile = FileIO.OpenFile("Fighter Costume (Pl**Nr.dat)|*.dat");
                if (cFile == null)
                    return;
                aFile = FileIO.OpenFile("Fighter Animation (Pl**AJ.dat)|*.dat");
                if (aFile == null)
                    return;
            }

            // load model
            var modelFile = new HSDRawFile(cFile);
            if (modelFile.Roots.Count > 0 && modelFile.Roots[0].Data is HSD_JOBJ jobj)
            {
                JOBJManager.SetJOBJ(jobj);

                // load material animation if it exists
                if (modelFile.Roots.Count > 1 && modelFile.Roots[1].Data is HSD_MatAnimJoint matanim)
                {
                    JOBJManager.SetMatAnimJoint(matanim);
                    JOBJManager.EnableMaterialFrame = true;
                }
            }
            else
                return;
            
            // set model scale
            JOBJManager.ModelScale = ModelScale;

            // clear hidden dobjs
            JOBJManager.DOBJManager.HiddenDOBJs.Clear();

            // don't render bones by default
            JOBJManager.settings.RenderBones = false;

            // reset model visibility
            ResetModelVis();

            // load the model parts
            LoadModelParts();

            // populate animation dictionary
            AJFilePath = aFile;
            SymbolToAnimation.Clear();
            using (BinaryReaderExt r = new BinaryReaderExt(new FileStream(aFile, FileMode.Open)))
                foreach (var a in AllActions)
                    if (a.Symbol != null && !SymbolToAnimation.ContainsKey(a.Symbol))
                        SymbolToAnimation.Add(a.Symbol, r.GetSection((uint)a.AnimOffset, a.AnimSize));
            
            // enable preview box
            previewBox.Visible = true;
            savePlayerRenderingFilesToolStripMenuItem.Enabled = true;

            // reselect action
            if(actionList.SelectedItem is Action action)
                SelectAction(action);
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadModelParts()
        {
            var plDat = FighterData;

            if (plDat != null && plDat.ModelPartAnimations != null && JOBJManager.JointCount != 0)
            {
                ModelPartsIndices = plDat.ModelPartAnimations.Array.Select(
                    e => new ModelPartAnimations(e)
                ).ToArray() ;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResetModelVis()
        {
            var plDat = FighterData;

            JOBJManager.MatAnimation.SetAllFrames(0);

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

            // reset model parts
            if(ModelPartsIndices != null)
            {
                for (int i = 0; i < ModelPartsIndices.Length; i++)
                    ModelPartsIndices[i].AnimIndex = -1;

                JOBJManager.Animation.FrameModifier.Clear();
                JOBJManager.Animation.FrameModifier.AddRange(ModelPartsIndices);
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
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="frame"></param>
        private void AnimateMaterial(int index, int frame, int matflag, int frameflag)
        {
            var plDat = FighterData;

            if (plDat.ModelLookupTables != null && index < plDat.ModelLookupTables.CostumeMaterialLookups[0].Entries.Length)
            {
                if(matflag == 1)
                {
                    foreach(var v in plDat.ModelLookupTables.CostumeMaterialLookups[0].Entries.Array)
                        JOBJManager.MatAnimation.SetFrame(v.Value, frame);
                }
                else
                {
                    var idx = plDat.ModelLookupTables.CostumeMaterialLookups[0].Entries[index];
                    JOBJManager.MatAnimation.SetFrame(idx.Value, frame);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="frame"></param>
        private void AnimateModel(int part_index, int anim_index)
        {
            if(ModelPartsIndices != null && part_index < ModelPartsIndices.Length && part_index >= 0)
                ModelPartsIndices[part_index].AnimIndex = anim_index;
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
            // store previous hitbox state info
            Dictionary<int, Vector3> previousPosition = CalculatePreviousState();

            // reset model parts
            if (ModelPartsIndices != null)
                for (int i = 0; i < ModelPartsIndices.Length; i++)
                    ModelPartsIndices[i].AnimIndex = -1;

            // process ftcmd
            SubactionProcess.SetFrame(viewport.Frame);

            // update display info
            JOBJManager.DOBJManager.OverlayColor = SubactionProcess.OverlayColor;
            JOBJManager.settings.RenderBones = bonesToolStripMenuItem.Checked;

            // apply model animations
            JOBJManager.Frame = viewport.Frame;
            JOBJManager.UpdateNoRender();

            // character invisibility
            if (!SubactionProcess.CharacterInvisibility && modelToolStripMenuItem.Checked)
                JOBJManager.Render(cam, false);

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

                // draw hitbox angle
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
        private void LoadAnimation(string symbol)
        {
            // clear animation
            JOBJManager.SetFigaTree(null);

            // check if animation exists
            if (symbol == null || !SymbolToAnimation.ContainsKey(symbol))
                return;

            // load animation
            var anim = new HSDRawFile(SymbolToAnimation[symbol]);
            if(anim.Roots[0].Data is HSD_FigaTree tree)
            {
                var name = new Action() { Symbol = anim.Roots[0].Name }.ToString();

                JOBJManager.SetFigaTree(tree);
                
                _animEditor.SetJoint(JOBJManager.GetJOBJ(0), JOBJManager.Animation);

                viewport.MaxFrame = tree.FrameCount;

                ThrowDummyManager.CleanupRendering();
                ThrowDummyManager = new JOBJManager();

                AnimationName = name;

                // load throw dummy for thrown animations
                if (name.Contains("Throw") && !name.Contains("Taro"))
                {
                    // find thrown anim
                    Action throwAction = null;
                    foreach (Action a in actionList.Items)
                    {
                        if(a.Symbol.Contains("Taro") && a.Symbol.Contains(name) && !a.Symbol.Equals(anim.Roots[0].Name))
                        {
                            throwAction = a;
                            break;
                        }
                    } 

                    if (throwAction != null && throwAction.Symbol != null && SymbolToAnimation.ContainsKey(throwAction.Symbol))
                    {
                        // load throw dummy
                        ThrowDummyManager.SetJOBJ(DummyThrowModel.GenerateThrowDummy());

                        // load throw animation
                        var tanim = new HSDRawFile(SymbolToAnimation[throwAction.Symbol]);
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

            SaveSelectedActionChanges();

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
            SaveSelectedActionChanges();
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

                if (!itemText.StartsWith("Subroutine") && !itemText.StartsWith("Custom"))
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var f = FileIO.SaveFile("Text File (*.txt)|*.txt", "cmd_script.txt");

            if(f != null)
            {
                using (FileStream stream = new FileStream(f, FileMode.Create))
                using (StreamWriter w = new StreamWriter(stream))
                    foreach (var v in AllActions)
                    {
                        w.WriteLine($"[Symbol = \"{v.Symbol}\"]");
                        w.WriteLine($"[AnimOffset = 0x{v.AnimOffset}]");
                        w.WriteLine($"[AnimSize = 0x{v.AnimSize}]");
                        w.WriteLine($"[Flags = 0x{v.Flags.ToString("X")}]");

                        var scripts = GetScripts(v);
                        w.WriteLine(v.ToString() + "()");
                        w.WriteLine("{");
                        foreach (var s in scripts)
                            w.WriteLine($"\t{s.Serialize(this)};");
                        w.WriteLine("}");
                    }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importFromTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = FileIO.OpenFile("Text File (*.txt)|*.txt");

            if (f != null)
            {
                using (FileStream stream = new FileStream(f, FileMode.Create))
                using (StreamReader w = new StreamReader(stream))
                {
                    var script = new Action();
                    while(!w.EndOfStream)
                    {
                        var line = w.ReadLine();
                        
                        // process attribute




                        // process script

                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void savePlayerRenderingFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importFigatreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (actionList.SelectedItem is Action a)
            {
                var f = FileIO.OpenFile(ApplicationSettings.HSDFileFilter);

                if (f != null)
                {
                    // check valid dat file
                    var file = new HSDRawFile(f);

                    // grab symbol
                    var symbol = file.Roots[0].Name;

                    // check if symbol exists and ok to overwrite
                    if(SymbolToAnimation.ContainsKey(symbol))
                    {
                        if(MessageBox.Show($"Symbol \"{symbol}\" already exists.\nIs it okay to overwrite?", "Overwrite Symbol", MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                            return;

                        SymbolToAnimation[symbol] = File.ReadAllBytes(f);
                    }
                    else
                        SymbolToAnimation.Add(symbol, File.ReadAllBytes(f));
                        
                    // set action symbol
                    a.Symbol = symbol;

                    // reselect action
                    LoadAnimation(symbol);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportFigatreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(actionList.SelectedItem is Action a)
            {
                if(a.Symbol != null && SymbolToAnimation.ContainsKey(a.Symbol))
                {
                    var f = FileIO.SaveFile(ApplicationSettings.HSDFileFilter, a.Symbol + ".dat");

                    if (f != null)
                        File.WriteAllBytes(f, SymbolToAnimation[a.Symbol]);
                }
            }
        }

        private PopoutJointAnimationEditor _animEditor = new PopoutJointAnimationEditor();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void popoutEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _animEditor.Show();
            _animEditor.Visible = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAnimationChangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAnimation();
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveAnimation()
        {
            if(actionList.SelectedItem is Action action)
            {
                HSDRawFile f = new HSDRawFile();
                f.Roots.Add(new HSDRootNode()
                {
                    Name = action.Symbol,
                    Data = JOBJManager.Animation.ToFigaTree()
                });
                var tempFileName = Path.GetTempFileName();
                f.Save(tempFileName);
                SymbolToAnimation[action.Symbol] = File.ReadAllBytes(tempFileName);
                File.Delete(tempFileName);
            }
        }
    }
}
