using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Melee.Pl;
using HSDRawViewer.GUI;
using HSDRawViewer.GUI.Dialog;
using HSDRawViewer.GUI.Extra;
using HSDRawViewer.GUI.Plugins;
using HSDRawViewer.Rendering;
using HSDRawViewer.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer
{
    public partial class MainForm : DockContent
    {
        /// <summary>
        /// 
        /// </summary>
        public static MainForm Instance { get; internal set; }

        private readonly PropertyView _nodePropertyViewer;

        public string FilePath { get; internal set; }

        private HSDRawFile RawHSDFile = new();

        public static DataNode SelectedDataNode { get; internal set; } = null;

        public static bool RefreshNode = false;

        private IDockContent LastActiveContent = null;

        public static void Init()
        {
            if (Instance == null)
                Instance = new MainForm();
        }

        public int GetStructLocation(HSDStruct str)
        {
            return RawHSDFile.GetOffsetFromStruct(str);
        }

        public MainForm()
        {
            InitializeComponent();

            IsMdiContainer = true;

            dockPanel.Theme = new VS2015LightTheme();

#if DEBUG
            var vp = new GUI.Controls.DockableViewport();
            vp.Dock = DockStyle.Fill;
            vp.Show(dockPanel);
            TestRendering test = new TestRendering();
            vp.glViewport.AddRenderer(test);
#endif

            _nodePropertyViewer = new PropertyView();
            _nodePropertyViewer.Dock = DockStyle.Fill;
            _nodePropertyViewer.Show(dockPanel);

            //dockPanel.ShowDocumentIcon = true;
            dockPanel.ActiveContentChanged += (sender, args) =>
            {
                if (dockPanel.ActiveContent != null)
                    LastActiveContent = dockPanel.ActiveContent;
            };

            ImageList myImageList = new();
            myImageList.ImageSize = new System.Drawing.Size(24, 24);
            myImageList.Images.Add("unknown", Properties.Resources.ico_unknown);
            myImageList.Images.Add("known", Properties.Resources.ico_known);
            myImageList.Images.Add("folder", Properties.Resources.ico_folder);
            myImageList.Images.Add("group", Properties.Resources.ico_group);
            myImageList.Images.Add("table", Properties.Resources.ico_table);
            myImageList.Images.Add("jobj", Properties.Resources.ico_jobj);
            myImageList.Images.Add("dobj", Properties.Resources.ico_dobj);
            myImageList.Images.Add("pobj", Properties.Resources.ico_pobj);
            myImageList.Images.Add("mobj", Properties.Resources.ico_mobj);
            myImageList.Images.Add("tobj", Properties.Resources.ico_tobj);
            myImageList.Images.Add("aobj", Properties.Resources.ico_aobj);
            myImageList.Images.Add("cobj", Properties.Resources.ico_cobj);
            myImageList.Images.Add("fobj", Properties.Resources.ico_fobj);
            myImageList.Images.Add("iobj", Properties.Resources.ico_iobj);
            myImageList.Images.Add("lobj", Properties.Resources.ico_lobj);
            myImageList.Images.Add("sobj", Properties.Resources.ico_sobj);
            myImageList.Images.Add("coll", Properties.Resources.ico_coll);
            myImageList.Images.Add("anim_texture", Properties.Resources.ico_anim_texture);
            myImageList.Images.Add("anim_material", Properties.Resources.ico_anim_material);
            myImageList.Images.Add("anim_joint", Properties.Resources.ico_anim_joint);
            myImageList.Images.Add("anim_shape", Properties.Resources.ico_anim_shape);
            myImageList.Images.Add("kabii", Properties.Resources.ico_kabii);
            myImageList.Images.Add("fuma", Properties.Resources.ico_fuma);

            treeView1.ImageList = myImageList;

            bool dc = false;

            treeView1.MouseDown += (sender, args) =>
            {
                dc = args.Clicks > 1;
            };

            treeView1.BeforeExpand += (sender, args) =>
            {
                args.Cancel = dc;

                if (args.Node is DataNode node && Instance.IsOpened(node) && !dc)
                {
                    MessageBox.Show("Error: This node is currently open in an editor\nPlease close it first to expand");
                    args.Cancel = true;
                }

                dc = false;
            };

            treeView1.AfterExpand += (sender, args) =>
            {
                args.Node.Nodes.Clear();
                treeView1.BeginUpdate();
                if (args.Node is DataNode node)
                {
                    node.ExpandData();
                }
                treeView1.EndUpdate();
            };

            treeView1.AfterCollapse += (sender, args) =>
            {
                treeView1.BeginUpdate();
                args.Node.Nodes.Clear();
                args.Node.Nodes.Add(new TreeNode());
                treeView1.EndUpdate();
            };

            treeView1.AfterSelect += (sender, args) =>
            {
                SelectNode<HSDAccessor>();
            };

            treeView1.NodeMouseClick += (sender, args) =>
            {
                treeView1.SelectedNode = treeView1.GetNodeAt(args.Location);
                if (args.Button == MouseButtons.Right && args.Node != null && args.Node is DataNode node)
                {
                    PluginManager.GetContextMenuFromType(node.Accessor.GetType()).Show(this, args.Location);
                }
                try
                {
                    // TODO: expand all
                    //var kb = OpenTK.Input.Keyboard.GetState();
                    //if (kb.IsKeyDown(OpenTK.Input.Key.ShiftLeft) || kb.IsKeyDown(OpenTK.Input.Key.ShiftRight))
                    //{
                    //    treeView1.BeginUpdate();
                    //    treeView1.SelectedNode.ExpandAll();
                    //    treeView1.EndUpdate();
                    //}
                }
                catch (Exception)
                {

                }
            };

            FormClosing += (s, a) =>
            {
                ClearWorkspace();
            };

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cast"></param>
        public void SelectNode<T>(T cast = null) where T : HSDAccessor
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode is DataNode n)
            {
                if (cast == null)
                {
                    _nodePropertyViewer.SetNode(n);
                }
                else
                {
                    cast._s = n.Accessor._s;
                    n.Accessor = cast;
                    _nodePropertyViewer.SetNode(n);
                }
                SelectedDataNode = n;

                LocationLabel.Text = "Location: 0x" + RawHSDFile.GetOffsetFromStruct(n.Accessor._s).ToString("X8") + " -> " + n.FullPath;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        public static void DeleteRoot(DataNode root)
        {
            HSDRootNode toDel = Instance.RawHSDFile.Roots.Find(r => r.Data == root.Accessor);
            if (toDel != null)
            {
                Instance.RawHSDFile.Roots.Remove(toDel);
                Instance.treeView1.Nodes.Remove(root);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        public static void AddRoot(string name, HSDAccessor accesor)
        {
            HSDRootNode root = new()
            {
                Name = name,
                Data = accesor
            };

            Instance.RawHSDFile.Roots.Add(root);
            Instance.treeView1.Nodes.Add(new DataNode(name, accesor, root: root));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public void OpenFile(string filePath)
        {
            FilePath = filePath;

            RawHSDFile = new HSDRawFile();
            RawHSDFile.Open(filePath);
            RefreshTree();

#if !DEBUG
            if (RawHSDFile.Roots.Count > 0 && RawHSDFile.Roots[0].Data is HSDRaw.MEX.MEX_Data)
            {
                if (nodeBox.Visible)
                {
                    // hide nodes
                    showHideButton_Click(null, null);

                    // select the mexData node
                    treeView1.SelectedNode = treeView1.Nodes[0];

                    // open the editor
                    OpenEditor();
                }
            }
#endif

            Text = "HSD DAT Browser - " + filePath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string f = Tools.FileIO.OpenFile("HSD (*.dat,*.usd,*.ssm,*.sem)|*.dat;*.usd;*.ssm;*.sem");
            if (f != null)
            {
                if (f.ToLower().EndsWith(".sem"))
                {
                    SEMEditorTool d = new();
                    {
                        d.Show();
                    }
                    d.OpenSEMFile(f);
                    d.BringToFront();
                }
                else
                if (f.ToLower().EndsWith(".ssm"))
                {
                    SSMTool d = new();
                    {
                        d.Show();
                    }
                    d.OpenFile(f);
                    d.BringToFront();
                }
                else
                    OpenFile(f);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string f = Tools.FileIO.SaveFile("HSD (*.dat,*.usd)|*.dat;*.usd", System.IO.Path.GetFileName(FilePath), "Save File As");
            if (f != null)
            {
                RawHSDFile.Save(f);
                OpenFile(f);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SaveDAT()
        {
            foreach (IDockContent c in dockPanel.Contents)
            {
                if (c is SaveableEditorBase save)
                {
                    System.Diagnostics.Debug.WriteLine($"{c.GetType()} is saving...");
                    save.OnDatFileSave();
                }
            }

            if (RawHSDFile != null)
                RawHSDFile.Save(FilePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsUnoptimizedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string f = Tools.FileIO.SaveFile("HSD (*.dat,*.usd)|*.dat;*.usd", System.IO.Path.GetFileName(FilePath));
            if (f != null)
            {
                RawHSDFile.Save(f, false, false);

                if (MessageBox.Show("Reload File?", "Reload File to Update Location Offsets?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    OpenFile(f);
                }
            }
        }

        /// <summary>
        /// Reloads all the data nodes in the tree
        /// </summary>
        private void RefreshTree()
        {
            treeView1.BeginUpdate();

            treeView1.Nodes.Clear();
            foreach (HSDRootNode r in RawHSDFile.Roots)
            {
                treeView1.Nodes.Add(new DataNode(r.Name, r.Data, root: r));
            }
            foreach (HSDRootNode r in RawHSDFile.References)
            {
                treeView1.Nodes.Add(new DataNode(r.Name, r.Data, root: r, referenceNode: true));
            }
            if (treeView1.Nodes.Count > 0)
                treeView1.SelectedNode = treeView1.Nodes[0];

            treeView1.EndUpdate();

            ClearWorkspace();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addRootFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string f = Tools.FileIO.OpenFile("HSD (*.dat;*.txg)|*.dat;*.txg");
            if (f != null)
            {
                if (f.ToLower().EndsWith(".dat"))
                {
                    HSDRawFile file = new(f);

                    RawHSDFile.Roots.Add(file.Roots[0]);
                }
                if (f.ToLower().EndsWith(".txg"))
                {
                    HSDStruct str = new();
                    str.SetData(System.IO.File.ReadAllBytes(f));

                    RawHSDFile.Roots.Add(new HSDRootNode()
                    {
                        Name = "TextureGraphic",
                        Data = new HSDRaw.Common.HSD_TEXGraphicBank() { _s = str }
                    });
                }

                RefreshTree();
            }
        }

        /// <summary>
        /// Closes any editors that are using the given node
        /// </summary>
        /// <param name="n"></param>
        /// <returns>true if editor was successfully closed</returns>
        public bool CloseEditor(DataNode n)
        {
            List<Form> toClose = GetEditors(n);
            foreach (Form c in toClose)
                c.Close();
            return toClose.Count > 0;
        }

        /// <summary>
        /// Gets editors using given node
        /// </summary>
        /// <param name="n"></param>
        /// <returns>Editors that are using this node</returns>
        public List<Form> GetEditors(DataNode n)
        {
            List<Form> editors = new();
            foreach (IDockContent c in dockPanel.Contents)
            {
                if (c is PluginBase b && b.Node.Accessor._s == n.Accessor._s && c is Form form)
                {
                    editors.Add(form);
                }
            }
            return editors;
        }

        /// <summary>
        /// Returns true if node is currently open in editor
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public bool IsOpened(DataNode n)
        {
            foreach (IDockContent c in dockPanel.Contents)
            {
                if (c is PluginBase b && b.Node.Accessor._s == n.Accessor._s)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public bool IsChildOpened(HSDStruct s)
        {
            List<HSDStruct> structs = s.GetSubStructs();
            foreach (IDockContent c in dockPanel.Contents)
            {
                if (c is PluginBase b && structs.Contains(b.Node.Accessor._s))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public void ClearWorkspace()
        {
            List<DockContent> ToRemove = new();
            foreach (IDockContent c in dockPanel.Contents)
            {
                if (c is SaveableEditorBase save)
                    save.ForceClose = true;

                if (c is DockContent dc && c != _nodePropertyViewer)
                {
                    ToRemove.Add(dc);
                }
            }
            foreach (DockContent v in ToRemove)
                v.Close();
        }

        // For model part previewing
        static SBM_ModelPart prev_selected_part = null;
        static int part_select_index = 0;

        /// <summary>
        /// Opens editor for currently selected node if editor exists
        /// </summary>
        public void OpenEditor()
        {
            if (SelectedDataNode == null)
                return;

            PluginBase edit = PluginManager.GetEditorFromType(SelectedDataNode.Accessor.GetType());

            // Special animation override
            if (SelectedDataNode.Accessor is HSD_AnimJoint
                || SelectedDataNode.Accessor is HSD_FigaTree
                || SelectedDataNode.Accessor is HSD_MatAnimJoint
                || SelectedDataNode.Accessor is HSD_ShapeAnimJoint
                || SelectedDataNode.Accessor is HSD_FogDesc
                || SelectedDataNode.Accessor is HSD_Camera
                || SelectedDataNode.Accessor is SBM_ModelPart
                || SelectedDataNode.Accessor is SBM_PhysicsGroup
                || SelectedDataNode.Accessor is HSDNullPointerArrayAccessor<HSD_Light>)
            {
                //foreach (var v in dockPanel.Contents)
                {
                    if (LastActiveContent is JobjEditorDock jedit && jedit.Visible)
                    {
                        if (SelectedDataNode.Accessor is SBM_PhysicsGroup group)
                            jedit.LoadPhysics(group);

                        if (SelectedDataNode.Accessor is HSD_MatAnimJoint matjoint)
                            jedit.LoadAnimation(matjoint);

                        if (SelectedDataNode.Accessor is HSD_ShapeAnimJoint shapeJoint)
                            jedit.LoadAnimation(shapeJoint);

                        if (SelectedDataNode.Accessor is HSD_AnimJoint joint)
                            jedit.LoadAnimation(new JointAnimManager(joint));

                        if (SelectedDataNode.Accessor is HSD_FigaTree tree)
                            jedit.LoadAnimation(new JointAnimManager(tree));

                        if (SelectedDataNode.Accessor is HSD_FogDesc fog)
                            jedit.Editor.SetFog(fog);

                        if (SelectedDataNode.Accessor is HSDNullPointerArrayAccessor<HSD_Light> lights)
                            jedit.Editor.SetLights(lights.Array);

                        if (SelectedDataNode.Accessor is HSD_Camera camera)
                            jedit.Editor.SetCamera(camera);

                        if (SelectedDataNode.Accessor is SBM_ModelPart modelPart && modelPart.Anims.Length > 0)
                        {
                            if (prev_selected_part == modelPart)
                                part_select_index++;
                            else
                                part_select_index = 0;

                            prev_selected_part = modelPart;

                            if (part_select_index > modelPart.Anims.Length)
                                part_select_index = 0;

                            JointAnimManager manager = new();
                            for (int i = 0; i < modelPart.StartingBone; i++)
                                manager.Nodes.Add(new AnimNode());
                            manager.Nodes.AddRange(new JointAnimManager(modelPart.Anims[part_select_index]).Nodes);
                            jedit.LoadAnimation(manager);
                        }
                    }
                }
            }

            if (IsOpened(SelectedDataNode))
            {
                List<Form> editor = GetEditors(SelectedDataNode);
                editor[0].BringToFront();
            }
            else
            if (!IsChildOpened(SelectedDataNode.Accessor._s) &&
                edit != null &&
                edit is DockContent dc)
            {
                //Editors.Add(edit);
                SelectedDataNode.Collapse();
                edit.Node = SelectedDataNode;

                dc.Show(dockPanel);

                dc.Text = SelectedDataNode.Text;
                dc.TabText = SelectedDataNode.Text;
                if (dc is PluginBase b)
                    dc.DockState = b.DefaultDockState;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        public void TryClose(Control c)
        {
            if (c == _nodePropertyViewer)
            {
                propertyViewToolStripMenuItem.Checked = false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void propertyViewToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (propertyViewToolStripMenuItem.Checked)
            {
                if (_nodePropertyViewer.IsHidden)
                    _nodePropertyViewer.Show();
            }
            else
            {
                _nodePropertyViewer.Hide();
            }
        }

        private void viewportToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void showHideButton_Click(object sender, EventArgs e)
        {
            nodeBox.Visible = !nodeBox.Visible;
            showHideButton.Text = nodeBox.Visible ? "<" : ">";
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode is DataNode dn)
            {
                OpenEditor();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aJToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using AJSplitDialog d = new();
            d.ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sSMEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SSMTool d = new();
            d.Show();
            d.BringToFront();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            if (RawHSDFile.Roots.Count == 0 || MessageBox.Show("Current unsaved changes will be lost", "Open File?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                if (s.Length > 0)
                    OpenFile(s[0]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (FilePath == null)
                FilePath = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);

            if (FilePath != null)
                SaveDAT();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sEMEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SEMEditorTool d = new();
            {
                d.Show();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OpenEditor();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && dockPanel.ActiveContent is DockContent t)
            {
                t.Close();
            }
        }


        public class NewRootSettings
        {
            [DisplayName("Symbol Name"), Description("Name of the symbol being added")]
            public string Symbol { get; set; } = "newRoot";

            /*[Browsable(true),
             TypeConverter(typeof(HSDTypeConverter))]
            public Type Type { get; set; } = typeof(HSDAccessor);*/

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addRootFromTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewRootSettings settings = new();
            using HSDTypeDialog t = new();
            if (t.ShowDialog() == DialogResult.OK)
            {
                using PropertyDialog d = new("New Root", settings);
                if (d.ShowDialog() == DialogResult.OK)
                {
                    HSDRootNode root = new();

                    root.Name = settings.Symbol;
                    root.Data = (HSDAccessor)Activator.CreateInstance(t.HSDAccessorType);

                    RawHSDFile.Roots.Add(root);

                    RefreshTree();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addRootFromFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string f = Tools.FileIO.OpenFile("All Files |*.*");

            if (f != null)
            {
                HSDRootNode root = new();

                root.Name = System.IO.Path.GetFileNameWithoutExtension(f);
                root.Data = new HSDAccessor();
                root.Data._s.SetData(System.IO.File.ReadAllBytes(f));

                RawHSDFile.Roots.Add(root);

                RefreshTree();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            // Can only edit root node labels
            if (!(e.Node is DataNode d && d.IsRootNode && !d.IsReferenceNode))
            {
                e.CancelEdit = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node is DataNode d && d.IsRootNode && !e.CancelEdit && !string.IsNullOrEmpty(e.Label))
            {
                d.RootText = e.Label;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAudioPlaybackDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplicationSettings.SelectAudioPlaybackDevice();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trimExcessDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int trimmed = 0;
            foreach (DataNode d in treeView1.Nodes)
            {
                if (d.Accessor != null)
                    trimmed += d.Accessor.Optimize();
            }
            MessageBox.Show($"Trimmed 0x{trimmed.ToString("X")} bytes", "Trimmed File");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public HSDAccessor GetSymbol(string symbol)
        {
            foreach (DataNode d in treeView1.Nodes)
                if (d.Text == symbol)
                    return d.Accessor;

            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public IEnumerable<string> GetSymbols()
        {
            foreach (DataNode d in treeView1.Nodes)
                yield return d.Text;
        }
    }
}
