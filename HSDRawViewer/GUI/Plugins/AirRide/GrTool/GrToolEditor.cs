using HSDRaw;
using HSDRaw.AirRide.Gr;
using HSDRaw.AirRide.Gr.Data;
using HSDRawViewer.GUI.Controls;
using HSDRawViewer.GUI.Plugins.AirRide.GrTool.Nodes;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Rendering.Renderers;
using HSDRawViewer.Tools;
using System;
using System.IO;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool
{
    [SupportedTypes(new Type[] { typeof(KAR_grData) })]
    public partial class GrToolEditor : PluginBase, IDrawableInterface
    {
        public DrawOrder DrawOrder => DrawOrder.Last;

        private readonly GrDataResource _resource = new GrDataResource();

        private readonly GrRenderResource _render = new GrRenderResource();

        private readonly DockablePropertyGrid _propertyGrid;

        private readonly DockableViewport _viewport;

        private GrDockTree _dockTree;

        private DataNode _node;

        private readonly GLTextRenderer TextRenderer = new();
        public override DataNode Node
        {
            get => _node;
            set
            {
                _node = value;
                if (value != null && value.Accessor is KAR_grData d)
                    _resource.Load(d);
            }
        }

        private void SetupSelectUI()
        {
            buttonSelectCollision.Tag = GrSelectModeKind.Node;
            buttonSelectCollision.Click += SelectMode_Click;

            buttonSelectTriangle.Tag = GrSelectModeKind.Data;
            buttonSelectTriangle.Click += SelectMode_Click;
        }

        public GrToolEditor()
        {
            InitializeComponent();

            SetupSelectUI();

            dockPanel.Theme = new VS2015LightTheme();

            _viewport = new DockableViewport();
            _viewport.Show(dockPanel, DockState.Document);
            _viewport.glViewport.AddRenderer(this);

            _dockTree = new GrDockTree(_resource);
            _dockTree.Show(dockPanel, DockState.DockLeft);

            dockPanel.KeyDown += _dockTree.treeView1_KeyDown;

            _dockTree.OnSelectedNodeChanged += (GrNode n) =>
            {
                if (n != null)
                {
                    _propertyGrid.SetObject(n.Text, n.Tag);

                    //toolStrip2.SuspendLayout();
                    //toolStrip2.Items.Clear();
                    //n.BuildToolStrip(toolStrip2);
                    //toolStrip2.Visible = (toolStrip2.Items.Count > 0);
                    //toolStrip2.ResumeLayout();
                }
            };

            _propertyGrid = new DockablePropertyGrid();
            _propertyGrid.Show(dockPanel, DockState.DockRight);


            // dipose of resources
            Disposed += (s, a) =>
            {
                TextRenderer.Dispose();
                _propertyGrid.Dispose();
                _viewport.Dispose();
            };


            // initialize joint manager
            RenderJObj = new RenderJObj();
            RenderJObj.Initialize += () =>
            {

            };
            TryLoadModelFile(false);
        }

        private HSDRawFile ModelFile;
        private RenderJObj RenderJObj;

        private void TryLoadModelFile(bool use_dialog)
        {
            var path = MainForm.Instance.FilePath;
            var dir = Path.GetDirectoryName(path);
            var fname = Path.GetFileNameWithoutExtension(path);
            var model_path = Path.Combine(dir, $"{fname}Model.dat");
            if (File.Exists(model_path))
            {
                if (MessageBox.Show(
                    $"Load model data from:\n\"{model_path}\"?",
                    "Load Model Data",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    model_path = null;
                }
            }

            if (model_path == null && use_dialog)
            {
                model_path = FileIO.OpenFile(ApplicationSettings.HSDFileFilter, fname);
            }

            if (model_path != null)
            {
                ModelFile = new HSDRawFile(model_path);
                bool found = false;
                foreach (var r in ModelFile.Roots)
                {
                    if (r.Data is KAR_grModel model)
                    {
                        RenderJObj.LoadJObj(model.MainModel.RootNode);
                        found = true;
                        return;
                    }
                }
                if (!found)
                {
                    MessageBox.Show(
                        $"Could not find model information in:\n{model_path}",
                        "Load Model Data",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }

        }

        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            if (RenderJObj != null && modelToolStripMenuItem.Checked)
            {
                RenderJObj._settings.RenderBones = bonesToolStripMenuItem.Checked;

                RenderJObj.Render(cam, false);
                _render.Joints = RenderJObj.RootJObj;
            }

            _render.Camera = cam;
            _render.WindowWidth = windowWidth;
            _render.WindowHeight = windowHeight;

            _render.BeginDraw();
            _dockTree.Draw(_render, _propertyGrid.SelectedObject);

            _render.BeginOverlay();
            _dockTree.DrawOverlay(_render, _propertyGrid.SelectedObject);

            if (RenderJObj != null && boneNamesToolStripMenuItem.Checked)
            {
                int i = 0;
                foreach (var j in RenderJObj.RootJObj.Enumerate)
                {
                    TextRenderer.RenderText(cam, $"{i}", j.WorldTransform, dropShadow: true);
                    i++;
                }
            }
        }

        public void GLInit()
        {
            RenderJObj.Invalidate();
            TextRenderer.InitializeRender(@"Consolas.bff");
        }

        public void GLFree()
        {
            RenderJObj.FreeResources();
            TextRenderer.Dispose();
        }

        public void ViewportKeyPress(KeyEventArgs kbState)
        {
        }

        public void ScreenClick(MouseButtons button, PickInformation pick)
        {
        }

        private GrSelectModeKind SelectMode = GrSelectModeKind.Node;

        private void SelectMode_Click(object sender, EventArgs e)
        {
            var button = (ToolStripButton)sender;

            foreach (ToolStripItem item in toolStrip1.Items)
            {
                if (item is ToolStripButton other)
                    other.Checked = other == button;
            }

            SelectMode = (GrSelectModeKind)button.Tag;
        }

        public void ScreenDoubleClick(PickInformation pick)
        {
            var n = _dockTree.TryPickNode(SelectMode, pick, RenderJObj.RootJObj);
            if (n != null)
                _propertyGrid.SetObject(n.ToString(), n);
        }

        public void ScreenDrag(MouseEventArgs args, PickInformation pick, float deltaX, float deltaY)
        {
        }

        public void ScreenSelectArea(PickInformation start, PickInformation end)
        {
        }

        public bool FreezeCamera()
        {
            return false;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_node.Accessor is not KAR_grData d) return;
            if (RenderJObj == null) return;
            if (RenderJObj.RootJObj == null) return;
            _resource.Save(RenderJObj.RootJObj, d);
            MessageBox.Show("Saved Changes");
        }

        private void loadModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryLoadModelFile(true);
        }

        private void modelToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void bonesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void boneNamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
    }
}
