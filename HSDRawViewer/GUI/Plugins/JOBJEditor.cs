using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRawViewer.Rendering;
using System;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using HSDRawViewer.GUI.Extra;
using HSDRawViewer.Converters;

namespace HSDRawViewer.GUI.Plugins
{
    /// <summary>
    /// 
    /// </summary>
    public partial class JOBJEditor : DockContent, EditorBase, IDrawable
    {
        public DockState DefaultDockState => DockState.DockLeft;

        public DrawOrder DrawOrder => DrawOrder.First;

        private bool SelectDOBJ { get => (toolStripComboBox1.SelectedIndex == 1); }

        public JOBJEditor()
        {
            InitializeComponent();

            JOBJManager = new JOBJManager();

            listDOBJ.DataSource = dobjList;

            toolStripComboBox1.SelectedIndex = 0;

            treeJOBJ.AfterSelect += (sender, args) =>
            {
                propertyGrid1.SelectedObject = treeJOBJ.SelectedNode.Tag;
                JOBJManager.SelectetedJOBJ = treeJOBJ.SelectedNode.Tag as HSD_JOBJ;
            };

            listDOBJ.SelectedIndexChanged += (sender, args) => {
                propertyGrid1.SelectedObject = listDOBJ.SelectedItem;
                if(SelectDOBJ)
                    JOBJManager.DOBJManager.SelectedDOBJ = (listDOBJ.SelectedItem as DOBJContainer)?.DOBJ;
            };

            propertyGrid1.PropertyValueChanged += (sender, args) =>
            {
                // refresh
                listDOBJ.SelectedItem = listDOBJ.SelectedItem;
            };

            if(PluginManager.GetCommonViewport() != null)
                PluginManager.GetCommonViewport().AddRenderer(this);

            FormClosing += (sender, args) =>
            {
                if (PluginManager.GetCommonViewport() != null)
                {
                    PluginManager.GetCommonViewport().AnimationTrackEnabled = false;
                    PluginManager.GetCommonViewport().RemoveRenderer(this);
                    JOBJManager.ClearRenderingCache();
                }
            };
        }

        public Type[] SupportedTypes => new Type[] { typeof(HSD_JOBJ) };

        public DataNode Node { get => _node;
            set
            {
                _node = value;

                if (_node.Accessor is HSD_JOBJ jobj)
                {
                    root = jobj;
                    JOBJManager.SetJOBJ(root);
                    RefreshGUI();
                }
            }
        }

        private DataNode _node;
        private HSD_JOBJ root;
        private Dictionary<HSD_JOBJ, int> jobjToIndex = new Dictionary<HSD_JOBJ, int>();
        
        private class DOBJContainer
        {
            public int Index;
            public int JOBJIndex;
            public HSD_JOBJ ParentJOBJ;
            public HSD_DOBJ DOBJ;

            public int PolygonCount { get => DOBJ.Pobj != null ? DOBJ.Pobj.List.Count : 0; }

            public int TextureCount { get => DOBJ.Mobj.Textures != null ? DOBJ.Mobj.Textures.List.Count : 0; }

            public bool HasPixelProcessing { get => DOBJ.Mobj?.PEDesc != null; }

            public bool HasMaterialColor { get => DOBJ.Mobj?.Material != null; }

            public RENDER_MODE MOBJFlags { get => DOBJ.Mobj.RenderFlags; set => DOBJ.Mobj.RenderFlags = value; }

            public override string ToString()
            {
                return $"JOBJ {JOBJIndex} : DOBJ {Index} : POBJs {PolygonCount} : TOBJS {TextureCount} : PP {HasPixelProcessing}" +
                    $"";
            }
        }

        private BindingList<DOBJContainer> dobjList = new BindingList<DOBJContainer>();

        private JOBJManager JOBJManager;

        private void RefreshGUI()
        {
            treeJOBJ.Nodes.Clear();
            dobjList.Clear();
            jobjToIndex.Clear();

            LoadJOBJ(root);

            treeJOBJ.ExpandAll();
        }

        private void LoadJOBJ(HSD_JOBJ jobj, TreeNode parent = null)
        {
            TreeNode tree = new TreeNode();
            tree.Text = "JOBJ";
            tree.Tag = jobj;

            jobjToIndex.Add(jobj, jobjToIndex.Count);

            if(jobj.Dobj != null)
            {
                int dobjIndex = 0;
                foreach (var dobj in jobj.Dobj.List)
                {
                    dobjList.Add(new DOBJContainer() { DOBJ = dobj, ParentJOBJ = jobj, Index = dobjIndex++, JOBJIndex = jobjToIndex[jobj] } );
                }
            }

            if (parent == null)
                treeJOBJ.Nodes.Add(tree);
            else
                parent.Nodes.Add(tree);

            foreach(var c in jobj.Children)
            {
                LoadJOBJ(c, tree);
            }
        }

        private bool IsSelected(object ob)
        {
            foreach (var v in propertyGrid1.SelectedObjects)
                if (v == ob)
                    return true;
            return false;
        }

        public void LoadAnimation(HSD_FigaTree tree)
        {
            var vp = PluginManager.GetCommonViewport();
            vp.AnimationTrackEnabled = true;
            vp.Frame = 0;
            vp.MaxFrame = tree.FrameCount;
            JOBJManager.SetFigaTree(tree);
        }

        public void LoadAnimation(HSD_AnimJoint joint)
        {
            var vp = PluginManager.GetCommonViewport();
            vp.AnimationTrackEnabled = true;
            vp.Frame = 0;
            vp.MaxFrame = JOBJManager.SetAnimJoint(joint);
        }

        public void Draw(int windowWidth, int windowHeight)
        {
            JOBJManager.Frame = PluginManager.GetCommonViewport().Frame;
            JOBJManager.Render();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(toolStripComboBox1.SelectedIndex == 0)
            {
                JOBJManager.RenderObjects = true;
                JOBJManager.DOBJManager.SelectedDOBJ = null;
            }
            if (toolStripComboBox1.SelectedIndex == 1)
            {
                JOBJManager.RenderObjects = true;
                if (listDOBJ.SelectedItem != null)
                    JOBJManager.DOBJManager.SelectedDOBJ = (listDOBJ.SelectedItem as DOBJContainer).DOBJ;
            }
            if (toolStripComboBox1.SelectedIndex == 2)
            {
                JOBJManager.RenderObjects = false;
                if (listDOBJ.SelectedItem != null)
                    JOBJManager.DOBJManager.SelectedDOBJ = (listDOBJ.SelectedItem as DOBJContainer).DOBJ;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            JOBJManager.RenderBones = toolStripButton1.Checked;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            using (MOBJEditor m = new MOBJEditor())
            {
                m.SetMOBJ((listDOBJ.SelectedItem as DOBJContainer).DOBJ.Mobj);

                if (m.ShowDialog() != DialogResult.OK)
                {
                    dobjList[listDOBJ.SelectedIndex] = dobjList[listDOBJ.SelectedIndex];
                }
            }
        }

        private void vertexColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            JOBJManager.DOBJManager.RenderVertexColor = vertexColorsToolStripMenuItem.Checked;
        }

        private void importModelFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModelImporter.ReplaceModelFromFile(root);
            RefreshGUI();
        }
    }
}
