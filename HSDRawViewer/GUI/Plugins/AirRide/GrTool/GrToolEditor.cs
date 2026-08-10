using HSDRaw;
using HSDRaw.AirRide.Gr;
using HSDRaw.AirRide.Gr.Data;
using HSDRawViewer.GUI.Controls;
using HSDRawViewer.GUI.Dialog;
using HSDRawViewer.GUI.Plugins.AirRide.GrTool.Nodes;
using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Widgets;
using HSDRawViewer.Tools;
using OpenTK.Mathematics;
using System;
using System.IO;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool
{
    public enum GrToolMode
    {
        Translation,
        Rotation,
    }

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

        public GrToolMode TransformMode = GrToolMode.Translation;

        public bool TranslationEnabled = false;
        private TranslationWidget _translationWidget;

        public bool RotationEnabled = false;
        private RotationWidget _rotationWidget;

        public override DataNode Node
        {
            get => _node;
            set
            {
                _node = value;
                if (value != null && value.Accessor is KAR_grData d)
                {
                    _resource.Load(d);
                    _dockTree.LoadMiscData(d);
                }
            }
        }

        private void SetupSelectUI()
        {
            buttonSelectCollision.Tag = GrSelectModeKind.Node;
            buttonSelectCollision.Click += SelectMode_Click;

            buttonSelectTriangle.Tag = GrSelectModeKind.Data;
            buttonSelectTriangle.Click += SelectMode_Click;
        }

        private void SetupWidgets()
        {
            _translationWidget = new TranslationWidget();
            _translationWidget.TransformUpdated += (t) =>
            {
                if (_dockTree == null) return;

                if (_dockTree.SelectedNode is not IGrTranslate node)
                    return;

                node.SetTranslate(_propertyGrid.SelectedObject, _render.Joints, t.ExtractTranslation());
            };

            _rotationWidget = new RotationWidget();
            _rotationWidget.TransformUpdated += (t) =>
            {
                if (_dockTree == null) return;

                if (_dockTree.SelectedNode is not IGrRotate node)
                    return;

                node.SetRotation(_propertyGrid.SelectedObject, _render.Joints, t.ExtractRotation());
            };
        }

        public bool ProcessKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                if (_dockTree.SelectedNode is IUndo undo)
                {
                    undo.Undo(_propertyGrid.SelectedObject);
                    e.Handled = true;
                    SelectObject(_propertyGrid.ObjectName, _propertyGrid.SelectedObject);
                    return true;
                }
            }
            if (e.Control && e.KeyCode == Keys.Y)
            {
                if (_dockTree.SelectedNode is IUndo undo)
                {
                    undo.Redo(_propertyGrid.SelectedObject);
                    e.Handled = true;
                    SelectObject(_propertyGrid.ObjectName, _propertyGrid.SelectedObject);
                    return true;
                }
            }
            if (e.KeyCode == Keys.E)
            {
                if (SelectMode == GrSelectModeKind.Data)
                {
                    buttonSelectCollision.PerformClick();
                }
                else
                {
                    buttonSelectTriangle.PerformClick();
                }
                e.Handled = true;
                return true;
            }
            else
            if (e.KeyCode == Keys.G)
            {
                collViewComboBox.SelectedIndex = (collViewComboBox.SelectedIndex + 1) % collViewComboBox.Items.Count;
                e.Handled = true;
                return true;
            }

            return false;
        }

        public GrToolEditor()
        {
            InitializeComponent();

            SetupSelectUI();
            SetupWidgets();

            dockPanel.Theme = new VS2015LightTheme();

            _viewport = new DockableViewport();
            _viewport.Show(dockPanel, DockState.Document);
            _viewport.glViewport.AddRenderer(this);

            _dockTree = new GrDockTree(_resource);
            _dockTree.Show(dockPanel, DockState.DockLeft);

            //_dockTree.KeyDown += (s, e) =>
            //{
            //    ProcessKeyDown(s, e);
            //};

            _viewport.ViewportKeyDown += (s, e) =>
            {
                if (ProcessKeyDown(s, e))
                {
                }
                else
                {
                    _dockTree.treeView1_KeyDown(s, e);
                }
            };

            _dockTree.OnSelectedNodeChanged += (GrNode n) =>
            {
                if (n != null)
                {
                    SelectObject(n.Text, n.Tag);

                    //toolStrip2.SuspendLayout();
                    //toolStrip2.Items.Clear();
                    //n.BuildToolStrip(toolStrip2);
                    //toolStrip2.Visible = (toolStrip2.Items.Count > 0);
                    //toolStrip2.ResumeLayout();
                }
            };

            _propertyGrid = new DockablePropertyGrid();
            _propertyGrid.Show(dockPanel, DockState.DockRight);
            _propertyGrid.PropertyValueUpdated += (object s, PropertyValueChangedEventArgs e) =>
            {
                if (e.ChangedItem.PropertyDescriptor.Name == nameof(KdZone.Type))
                {
                    if (_propertyGrid.SelectedObject is IUndo undo)
                    {
                        undo.ClearHistory();
                    }
                    _propertyGrid.Refresh();
                }
            };

            foreach (GrCollisionNodeRenderKind e in Enum.GetValues(typeof(GrCollisionNodeRenderKind)))
            {
                collViewComboBox.Items.Add(e.ToString());
            }
            collViewComboBox.SelectedIndex = 0;
            collViewComboBox.SelectedIndexChanged += (s, e) =>
            {
                _render.CollisionRenderKind = (GrCollisionNodeRenderKind)collViewComboBox.SelectedIndex;
            };


            // dipose of resources
            Disposed += (s, a) =>
            {
                _render.Dispose();
                _propertyGrid.Dispose();
                _viewport.Dispose();
            };

            // initialize joint manager
            TryLoadModelFile(false);
        }

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
                var ModelFile = new HSDRawFile(model_path);
                bool found = false;
                foreach (var r in ModelFile.Roots)
                {
                    if (r.Data is KAR_grModel model)
                    {
                        _render.LoadModel(model);
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
            _render.DrawModel(cam);

            _render.Camera = cam;
            _render.WindowWidth = windowWidth;
            _render.WindowHeight = windowHeight;
            _render.IsXRay = buttonXRay.Checked;
            _render.DrawWireframe = wireframeButton.Checked;

            _render.BeginDraw();
            _dockTree.Draw(_render, _propertyGrid.SelectedObject);

            _render.BeginOverlay();
            _dockTree.DrawOverlay(_render, _propertyGrid.SelectedObject);

            //_render.DrawTexture(cam, Vector3.Zero, 50, 50, true);
            //_render.DrawTexture(cam, Vector3.Zero, 50, 50, false);

            _render.EndDraw();

            switch (TransformMode)
            {
                case GrToolMode.Translation:
                    if (TranslationEnabled)
                        _translationWidget.Render(cam);
                    break;
                case GrToolMode.Rotation:
                    if (RotationEnabled)
                        _rotationWidget.Render(cam);
                    break;
            }

            _render.DrawBoneLabels(cam);
        }

        public void GLInit()
        {
            _render.GLInit();
        }

        public void GLFree()
        {
            _render.GLFree();
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
                if (item is ToolStripButton other && other.Tag is GrSelectModeKind)
                    other.Checked = other == button;
            }

            SelectMode = (GrSelectModeKind)button.Tag;
        }

        public void SelectObject(string name, object o)
        {
            _propertyGrid.SetObject(name, o);

            _translationWidget.MouseUp();
            _rotationWidget.MouseUp();

            TranslationEnabled = false;
            RotationEnabled = false;

            switch (TransformMode)
            {
                case GrToolMode.Translation:
                    if (_dockTree.SelectedNode is IGrTranslate t && t.CanTranslate(o))
                    {
                        TranslationEnabled = true;
                        _translationWidget.Transform = Matrix4.CreateTranslation(t.GetTranslate(o, _render.Joints));
                    }
                    break;
                case GrToolMode.Rotation:
                    if (_dockTree.SelectedNode is IGrRotate r && r.CanRotate(o))
                    {
                        RotationEnabled = true;
                        _rotationWidget.Transform = r.GetRotation(o, _render.Joints);
                    }
                    break;
            }
        }

        public void ScreenDoubleClick(PickInformation pick)
        {
            var n = _dockTree.TryPickNode(SelectMode, pick, _render.Joints);
            if (n != null)
                SelectObject(n.ToString(), n);
        }

        public void ScreenDrag(MouseEventArgs args, PickInformation pick, float deltaX, float deltaY)
        {
            switch (TransformMode)
            {
                case GrToolMode.Translation:
                    if (TranslationEnabled)
                    {
                        if (args.Button == MouseButtons.Left)
                        {
                            if (!_translationWidget.Interacting && 
                                _dockTree.SelectedNode is IUndo undo)
                                undo.Commit(_propertyGrid.SelectedObject);

                            _translationWidget.MouseDown(pick);
                        }
                        else
                            _translationWidget.MouseUp();

                        _translationWidget.Drag(pick);

                        if (_translationWidget.PendingUpdate)
                        {
                            _translationWidget.PendingUpdate = false;
                        }
                    }
                    break;
                case GrToolMode.Rotation:
                    if (RotationEnabled)
                    {
                        if (args.Button == MouseButtons.Left)
                        {
                            if (!_rotationWidget.Interacting && 
                                _dockTree.SelectedNode is IUndo undo)
                                undo.Commit(_propertyGrid.SelectedObject);

                            _rotationWidget.MouseDown(pick);
                        }
                        else
                            _rotationWidget.MouseUp();

                        _rotationWidget.Drag(pick);
                    }
                    break;
            }
        }

        public void ScreenSelectArea(PickInformation start, PickInformation end)
        {
        }

        public bool FreezeCamera()
        {
            return (TranslationEnabled && _translationWidget.Interacting) || (_rotationWidget.Interacting && RotationEnabled);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_node.Accessor is not KAR_grData d) return;
            if (_render.Joints == null)
            {
                MessageBox.Show("The model must be loaded before saving.", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            _resource.Save(_render.Joints, d);
            _dockTree.SaveMiscData(d);
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

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            using (var p = new PropertyDialog("Display Settings", GrRenderResource.Settings))
            {
                p.ShowDialog();
            }
        }
    }
}
