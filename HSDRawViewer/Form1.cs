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

        private HSDRawFile RawHSDFile = new HSDRawFile();

        private Dictionary<string, StructData> stringToStruct = new Dictionary<string, StructData>();

        public static DataNode SelectedDataNode { get; internal set; } = null;
        
        public static bool RefreshNode = false;

        private List<EditorBase> Editors = new List<EditorBase>();

        public static void Init()
        {
            if(Instance == null)
            Instance = new MainForm();
        }

        public MainForm()
        {
            InitializeComponent();

            IsMdiContainer = true;

            PluginManager.Init();

            _nodePropertyViewer = new PropertyView();
            _nodePropertyViewer.Dock = DockStyle.Fill;
            _nodePropertyViewer.Show(dockPanel);

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

            treeView1.ImageList = myImageList;

            treeView1.AfterExpand += (sender, args) =>
            {
                args.Node.Nodes.Clear();
                if(args.Node is DataNode node)
                {
                    node.ExpandData();
                }

            };

            treeView1.AfterCollapse += (sender, args) =>
            {
                args.Node.Nodes.Clear();
                args.Node.Nodes.Add(new TreeNode());
            };

            treeView1.AfterSelect += (sender, args) =>
            {
                SelectNode(null);
            };

            treeView1.NodeMouseClick += (sender, args) =>
            {
                treeView1.SelectedNode = treeView1.GetNodeAt(args.Location);
                if(args.Button == MouseButtons.Right && args.Node != null && args.Node is DataNode node)
                {
                    PluginManager.GetContextMenuFromType(node.Accessor.GetType()).Show(this, args.Location);
                }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cast"></param>
        public void SelectNode(HSDAccessor cast)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode is DataNode n)
            {
                if (cast == null)
                {
                    _nodePropertyViewer.SetAccessor(n.Accessor);
                    //Viewport.SelectedAccessor = n.Accessor;
                }
                else
                {
                    cast._s = n.Accessor._s;
                    _nodePropertyViewer.SetAccessor(cast);
                    //Viewport.SelectedAccessor = cast;
                }
                SelectedDataNode = n;
                
                LocationLabel.Text = "Location: 0x" + RawHSDFile.GetOffsetFromStruct(n.Accessor._s).ToString("X8") + " -> " + n.FullPath;
            }
        }
        
        public static void DeleteRoot(DataNode root)
        {
            var toDel = Instance.RawHSDFile.Roots.Find(r=>r.Data == root.Accessor);
            if (toDel != null)
            {
                Instance.RawHSDFile.Roots.Remove(toDel);
                Instance.treeView1.Nodes.Remove(root);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FilePath"></param>
        private void OpenFile(string FilePath)
        {
            RawHSDFile = new HSDRawFile();
            RawHSDFile.Open(FilePath);
            RefreshTree();

            aJToolToolStripMenuItem.Enabled = (RawHSDFile.Roots.Find(e => e.Data is HSDRaw.Melee.Pl.SBM_PlayerData) != null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog d = new OpenFileDialog())
            {
                d.Filter = "HSD (*.dat,*.usd)|*.dat;*.usd";

                if(d.ShowDialog() == DialogResult.OK)
                    OpenFile(d.FileName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog d = new SaveFileDialog())
            {
                d.Filter = "HSD (*.dat,*.usd)|*.dat;*.usd";

                if (d.ShowDialog() == DialogResult.OK)
                {
                    RawHSDFile.Save(d.FileName);
                    OpenFile(d.FileName);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsUnoptimizedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog d = new SaveFileDialog())
            {
                d.Filter = "HSD (*.dat,*.usd)|*.dat;*.usd";

                if (d.ShowDialog() == DialogResult.OK)
                {
                    RawHSDFile.Save(d.FileName, false, false);
                    OpenFile(d.FileName);
                }
            }
        }

        /// <summary>
        /// Reloads all the data nodes in the tree
        /// </summary>
        private void RefreshTree()
        {
            treeView1.Nodes.Clear();
            foreach (var r in RawHSDFile.Roots)
            {
                treeView1.Nodes.Add(new DataNode(r.Name, r.Data));
            }
            if (treeView1.Nodes.Count > 0)
                treeView1.SelectedNode = treeView1.Nodes[0];

            ClearWorkspace();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addRootFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog f = new OpenFileDialog())
            {
                f.Filter = "HSD (*.dat)|*.dat";
                f.FileName = Text;

                if (f.ShowDialog() == DialogResult.OK)
                {
                    var file = new HSDRawFile(f.FileName);

                    RawHSDFile.Roots.Add(file.Roots[0]);

                    RefreshTree();
                }
            }
        }
        
        /// <summary>
        /// Returns true if node is currently open in editor
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public bool IsOpened(DataNode n)
        {
            foreach(var c in dockPanel.Contents)
            {
                if(c is EditorBase b && b.Node.Accessor._s == n.Accessor._s)
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
            if(SelectedDataNode.Accessor is HSD_AnimJoint
                || SelectedDataNode.Accessor is HSD_FigaTree)
            {
                foreach(var v in dockPanel.Contents)
                {
                    if(v is JOBJEditor jedit)
                    {
                        if (SelectedDataNode.Accessor is HSD_AnimJoint joint)
                            jedit.LoadAnimation(joint);

                        if (SelectedDataNode.Accessor is HSD_FigaTree tree)
                            jedit.LoadAnimation(tree);
                    }
                }
            }

            if (!IsChildOpened(SelectedDataNode.Accessor._s) && !IsOpened(SelectedDataNode) && edit != null && edit is DockContent dc)
            {
                Editors.Add(edit);
                SelectedDataNode.Collapse();
                edit.Node = SelectedDataNode;
                dc.Show(dockPanel);
                dc.Text = SelectedDataNode.Text;
                dc.TabText = SelectedDataNode.Text;

                try
                {
                    //if (dc is EditorBase b)
                    //    dc.DockState = b.DefaultDockState;
                }catch(Exception)
                {

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        public void TryClose(Control c)
        {
            if(c == _nodePropertyViewer)
            {
                propertyViewToolStripMenuItem.Checked = false;
            }
            if(c == Viewport)
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
            if(treeView1.SelectedNode != null && treeView1.SelectedNode is DataNode dn)
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
            var root = RawHSDFile.Roots.Find(r => r.Data is SBM_PlayerData);

            if(root != null)
            {
                treeView1.CollapseAll();

                using (AJSplitDialog d = new AJSplitDialog((SBM_PlayerData)root.Data))
                {
                    d.ShowDialog();
                }
            }
        }

        private SSMTool ssmTool = new SSMTool();

        private void sSMEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ssmTool.Show();
        }
    }
}
