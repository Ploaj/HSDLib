using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HSDRaw;
using HSDRawViewer.Rendering;
using HSDRawViewer.GUI;
using System.Linq;
using HSDRawViewer.ContextMenus;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer
{
    public partial class MainForm : DockContent
    {
        /// <summary>
        /// 
        /// </summary>
        public static MainForm Instance { get; } = new MainForm();

        private PropertyView _nodePropertyViewer;
        private Viewport _Viewport;
        private SubactionEditor _ScriptEditor;

        private HSDRawFile RawHSDFile = new HSDRawFile();

        private Dictionary<string, StructData> stringToStruct = new Dictionary<string, StructData>();

        public static DataNode SelectedDataNode { get; internal set; } = null;

        private Dictionary<Type, ContextMenu> typeToContextMenu = new Dictionary<Type, ContextMenu>();
        private CommonContextMenu commonContextMenu = new CommonContextMenu();

        public static bool RefreshNode = false;

        public MainForm()
        {
            InitializeComponent();

            IsMdiContainer = true;

            _nodePropertyViewer = new PropertyView();
            _nodePropertyViewer.Dock = DockStyle.Fill;
            _nodePropertyViewer.Show(dockPanel);

            _Viewport = new Viewport();
            _Viewport.Dock = DockStyle.Fill;
            _Viewport.Show(dockPanel);

            _ScriptEditor = new SubactionEditor();
            _ScriptEditor.Dock = DockStyle.Fill;

            ImageList myImageList = new ImageList();
            myImageList.ImageSize = new System.Drawing.Size(24, 24);
            myImageList.Images.Add("unknown", Properties.Resources.ico_unknown);
            myImageList.Images.Add("known", Properties.Resources.ico_known);
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
                    if (typeToContextMenu.ContainsKey(node.Accessor.GetType()))
                    {
                        var cm = typeToContextMenu[node.Accessor.GetType()];
                        cm.Show(this, args.Location);
                    }
                    else
                    {
                        commonContextMenu.Show(this, args.Location);
                    }
                }
            };

            GenerateContextMenus();
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
                    _Viewport.SelectedAccessor = n.Accessor;
                }
                else
                {
                    cast._s = n.Accessor._s;
                    _nodePropertyViewer.SetAccessor(cast);
                    _Viewport.SelectedAccessor = cast;
                }
                SelectedDataNode = n;
                
                LocationLabel.Text = "Location: 0x" + RawHSDFile.GetOffsetFromStruct(n.Accessor._s).ToString("X8") + " -> " + n.FullPath;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FilePath"></param>
        private void OpenFile(string FilePath)
        {
            treeView1.Nodes.Clear();
            RawHSDFile = new HSDRawFile();
            RawHSDFile.Open(FilePath);
            foreach(var r in RawHSDFile.Roots)
            {
                treeView1.Nodes.Add(new DataNode(r.Name, r.Data));
            }
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
                d.Filter = "HSD (*.dat)|*.dat;*.usd";

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
                d.Filter = "HSD (*.dat)|*.dat;*.usd";

                if (d.ShowDialog() == DialogResult.OK)
                {
                    RawHSDFile.Save(d.FileName);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void GenerateContextMenus()
        {
            var types = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                         from assemblyType in domainAssembly.GetTypes()
                         where typeof(CommonContextMenu).IsAssignableFrom(assemblyType)
                         select assemblyType).ToArray();

            foreach (var t in types)
            {
                if (t != typeof(CommonContextMenu))
                {
                    var ren = (CommonContextMenu)Activator.CreateInstance(t);

                    foreach (var v in ren.SupportedTypes)
                    {
                        typeToContextMenu.Add(v, ren);
                    }
                }
            }
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
            if(c == _Viewport)
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
                if (_Viewport.IsHidden)
                    _Viewport.Show();
            }
            else
            {
                _Viewport.Hide();
            }
        }
    }
}
