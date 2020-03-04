using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HSDRaw;
using HSDRawViewer.Rendering;
using HSDRawViewer.GUI;
using WeifenLuo.WinFormsUI.Docking;
using HSDRawViewer.GUI.Plugins;
using HSDRaw.Common.Animation;
using HSDRaw.Melee.Pl;
using HSDRawViewer.GUI.Extra;
using System.Linq;
using static System.ComponentModel.TypeConverter;
using System.ComponentModel;

namespace HSDRawViewer
{
    public partial class MainForm : DockContent
    {
        /// <summary>
        /// 
        /// </summary>
        public static MainForm Instance { get; internal set; }

        private PropertyView _nodePropertyViewer;
        public CommonViewport Viewport { get; internal set; }
        private SubactionEditor _ScriptEditor;

        public string FilePath { get; internal set; }

        private HSDRawFile RawHSDFile = new HSDRawFile();

        private Dictionary<string, StructData> stringToStruct = new Dictionary<string, StructData>();

        public static DataNode SelectedDataNode { get; internal set; } = null;

        public static bool RefreshNode = false;

        private List<EditorBase> Editors = new List<EditorBase>();

        private IDockContent LastActiveContent = null;

        public static void Init()
        {
            if (Instance == null)
                Instance = new MainForm();
        }

        public MainForm()
        {
            InitializeComponent();

            IsMdiContainer = true;

            PluginManager.Init();
            OpenTKResources.Init();

            _nodePropertyViewer = new PropertyView();
            _nodePropertyViewer.Dock = DockStyle.Fill;
            _nodePropertyViewer.Show(dockPanel);
            
            //dockPanel.ShowDocumentIcon = true;
            dockPanel.ActiveContentChanged += (sender, args) =>
            {
                if (dockPanel.ActiveContent != null)
                    LastActiveContent = dockPanel.ActiveContent;
            };

            Viewport = new CommonViewport();
            Viewport.Dock = DockStyle.Fill;
            Viewport.Show(dockPanel);

            _ScriptEditor = new SubactionEditor();
            _ScriptEditor.Dock = DockStyle.Fill;

            ImageList myImageList = new ImageList();
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
                    /*var kb = OpenTK.Input.Keyboard.GetState();
                    if (kb.IsKeyDown(OpenTK.Input.Key.ShiftLeft) || kb.IsKeyDown(OpenTK.Input.Key.ShiftRight))
                    {
                        treeView1.BeginUpdate();
                        treeView1.SelectedNode.ExpandAll();
                        treeView1.EndUpdate();
                    }*/
                }
                catch (Exception)
                {

                }
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
                    _nodePropertyViewer.SetAccessor(n.Accessor);
                }
                else
                {
                    cast._s = n.Accessor._s;
                    n.Accessor = cast;
                    _nodePropertyViewer.SetAccessor(cast);
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
            var toDel = Instance.RawHSDFile.Roots.Find(r => r.Data == root.Accessor);
            if (toDel != null)
            {
                Instance.RawHSDFile.Roots.Remove(toDel);
                Instance.treeView1.Nodes.Remove(root);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        private void OpenFile(string filePath)
        {
            FilePath = filePath;

            RawHSDFile = new HSDRawFile();
            RawHSDFile.Open(filePath);
            RefreshTree();

#if !DEBUG
            if(RawHSDFile.Roots.Count > 0 && RawHSDFile.Roots[0].Data is HSDRaw.MEX.MEX_Data)
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
            var f = Tools.FileIO.OpenFile("HSD (*.dat,*.usd,*.ssm,*.sem)|*.dat;*.usd;*.ssm;*.sem");
            if (f != null)
            {
                if (f.ToLower().EndsWith(".sem"))
                {
                    SEMEditor d = new SEMEditor();
                    {
                        d.Show();
                    }
                    d.OpenSEMFile(f);
                    d.BringToFront();
                }
                else
                if (f.ToLower().EndsWith(".ssm"))
                {
                    SSMTool d = new SSMTool();
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
            var f = Tools.FileIO.SaveFile("HSD (*.dat,*.usd)|*.dat;*.usd", System.IO.Path.GetFileName(FilePath), "Save File As");
            if (f != null)
            {
                RawHSDFile.Save(f);
                OpenFile(f);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsUnoptimizedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.SaveFile("HSD (*.dat,*.usd)|*.dat;*.usd", System.IO.Path.GetFileName(FilePath));
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
            foreach (var r in RawHSDFile.Roots)
            {
                treeView1.Nodes.Add(new DataNode(r.Name, r.Data, root: r));
            }
            foreach (var r in RawHSDFile.References)
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
            var f = Tools.FileIO.OpenFile("HSD (*.dat;*.txg)|*.dat;*.txg");
            if (f != null)
            {
                if (f.ToLower().EndsWith(".dat"))
                {
                    var file = new HSDRawFile(f);

                    RawHSDFile.Roots.Add(file.Roots[0]);
                }
                if (f.ToLower().EndsWith(".txg"))
                {
                    var str = new HSDStruct();
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
            foreach (var c in toClose)
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
            List<Form> editors = new List<Form>();
            foreach (var c in dockPanel.Contents)
            {
                if (c is EditorBase b && b.Node.Accessor._s == n.Accessor._s && c is Form form)
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
            foreach (var c in dockPanel.Contents)
            {
                if (c is EditorBase b && b.Node.Accessor._s == n.Accessor._s)
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
            var structs = s.GetSubStructs();
            foreach (var c in dockPanel.Contents)
            {
                if (c is EditorBase b && structs.Contains(b.Node.Accessor._s))
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
            List<DockContent> ToRemove = new List<DockContent>();
            foreach (var c in dockPanel.Contents)
            {
                if (c is DockContent dc && c != Viewport && c != _nodePropertyViewer)
                {
                    ToRemove.Add(dc);
                }
            }
            foreach (var v in ToRemove)
                v.Close();
        }

        /// <summary>
        /// Opens editor for currently selected node if editor exists
        /// </summary>
        public void OpenEditor()
        {
            if (SelectedDataNode == null)
                return;

            var edit = PluginManager.GetEditorFromType(SelectedDataNode.Accessor.GetType());

            // Special animation override
            if (SelectedDataNode.Accessor is HSD_AnimJoint
                || SelectedDataNode.Accessor is HSD_FigaTree)
            {
                //foreach (var v in dockPanel.Contents)
                {
                    if (LastActiveContent is JOBJEditor jedit && jedit.Visible)
                    {
                        if (SelectedDataNode.Accessor is HSD_AnimJoint joint)
                            jedit.LoadAnimation(joint);

                        if (SelectedDataNode.Accessor is HSD_FigaTree tree)
                            jedit.LoadAnimation(tree);
                    }
                }
            }

            if (IsOpened(SelectedDataNode))
            {
                var editor = GetEditors(SelectedDataNode);
                editor[0].BringToFront();
            } else
            if (!IsChildOpened(SelectedDataNode.Accessor._s) &&
                edit != null &&
                edit is DockContent dc)
            {
                Editors.Add(edit);
                SelectedDataNode.Collapse();
                edit.Node = SelectedDataNode;

                dc.Show(dockPanel);

                dc.Text = SelectedDataNode.Text;
                dc.TabText = SelectedDataNode.Text;
                //if (dc is EditorBase b)
                //    dc.DockState = b.DefaultDockState;
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
            if (c == Viewport)
            {
                viewportToolStripMenuItem.Checked = false;
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
            if (viewportToolStripMenuItem.Checked)
            {
                if (Viewport.IsHidden)
                    Viewport.Show();
            }
            else
            {
                Viewport.Hide();
            }
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
            using (AJSplitDialog d = new AJSplitDialog())
            {
                d.ShowDialog();
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sSMEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SSMTool d = new SSMTool();
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
                RawHSDFile.Save(FilePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sEMEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SEMEditor d = new SEMEditor();
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
            var settings = new NewRootSettings();
            using (HSDTypeDialog t = new HSDTypeDialog())
            {
                if(t.ShowDialog() == DialogResult.OK)
                {
                    using (PropertyDialog d = new PropertyDialog("New Root", settings))
                    {
                        if (d.ShowDialog() == DialogResult.OK)
                        {
                            var root = new HSDRootNode();

                            root.Name = settings.Symbol;
                            root.Data = (HSDAccessor)Activator.CreateInstance(t.HSDAccessorType);

                            RawHSDFile.Roots.Add(root);

                            RefreshTree();
                        }
                    }
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
            var f = Tools.FileIO.OpenFile("All Files |*.*");

            if(f != null)
            {
                var root = new HSDRootNode();

                root.Name = System.IO.Path.GetFileNameWithoutExtension(f);
                root.Data = new HSDAccessor() ;
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
            if(!(e.Node is DataNode d && d.IsRootNode && !d.IsReferenceNode))
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
    }
    
}
