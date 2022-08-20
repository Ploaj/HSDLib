using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRawViewer.Converters;
using HSDRawViewer.GUI.Dialog;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.GX;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Controls.JObjEditor
{
    public partial class JObjEditorNew : UserControl, IDrawable
    {
        public DrawOrder DrawOrder => DrawOrder.First;

        private DockablePropertyGrid _propertyGrid;

        private DockableJointTree _jointTree;

        private DockableMeshList _meshList;

        private DockableTextureEditor _textureEditor;

        private DockableTrackEditor _trackEditor;

        private DockableViewport _viewport;

        private RenderJObj RenderJObj;

        private HSD_JOBJ _root;

        private JointAnimManager JointAnimation;

        /// <summary>
        /// 
        /// </summary>
        public JObjEditorNew()
        {
            InitializeComponent();

            // initial theme
            dockPanel1.Theme = new VS2015LightTheme();

            // initialize viewport
            _viewport = new DockableViewport();
            _viewport.Show(dockPanel1, DockState.Document);

            // refresh render when viewport reloads
            _viewport.glViewport.Load += (r, a) =>
            {
                RenderJObj.Invalidate();
            };

            // initialize texture editor
            _trackEditor = new DockableTrackEditor();
            _trackEditor.MinimumSize = new System.Drawing.Size(400, 800);
            _trackEditor.Show(dockPanel1);
            _trackEditor.DockState = DockState.DockBottom;
            _trackEditor.Hide();

            // initialize joint tree
            _jointTree = new DockableJointTree();
            _jointTree.Show(dockPanel1, DockState.DockLeft);

            _jointTree.SelectJObj += (i, jobj) =>
            {
                if (jobj.Flags.HasFlag(JOBJ_FLAG.PTCL))
                    _propertyGrid.SetObject(new JObjParticleProxy(jobj));
                else
                if (jobj.Flags.HasFlag(JOBJ_FLAG.SPLINE))
                    _propertyGrid.SetObject(new JObjSplineProxy(jobj));
                else
                    _propertyGrid.SetObject(new JObjProxy(jobj));

                RenderJObj.SelectedJObj = jobj;

                // set animation track
                if (JointAnimation != null)
                {
                    if (i < JointAnimation.NodeCount && i >= 0)
                        _trackEditor.SetKeys(GraphEditor.AnimType.Joint, JointAnimation.Nodes[i].Tracks);
                    else
                        _trackEditor.SetKeys(GraphEditor.AnimType.Joint, null);
                }
            };

            // initialize meshlist
            _meshList = new DockableMeshList();
            _meshList.Show(dockPanel1, DockState.DockLeft);

            _meshList.SelectDObj += (dobj, indices) =>
            {
                _propertyGrid.SetObjects(dobj);

                RenderJObj.ClearDObjSelection();
                foreach (var i in indices)
                    RenderJObj.SetDObjSelected(i, true);

                // set textures in editor
                if (dobj.Length == 1)
                    _textureEditor.SetTextures(dobj[0]);
                else
                    _textureEditor.SetTextures(null);
            };

            _meshList.VisibilityUpdated += () =>
            {
                UpdateVisibility();
            };

            _meshList.ListUpdated += () =>
            {
                RenderJObj.Invalidate();
                UpdateVisibility();

                // set materials in editor
                _textureEditor.SetTextures(null);
            };

            // initialize properties
            _propertyGrid = new DockablePropertyGrid();
            _propertyGrid.Show(dockPanel1);
            _propertyGrid.DockState = DockState.DockRight;

            _propertyGrid.PropertyValueUpdated += (sender, args) =>
            {
                // update all flags
                UpdateFlagsAll();

                // redraw mesh list
                _meshList.Invalidate();

                // update jobj if changed
                if (_propertyGrid.SelectedObject is JObjProxy j)
                {
                    RenderJObj.RootJObj?.GetJObjFromDesc(j.jobj).ResetTransforms();
                    // RenderJObj.GetLiveJOBJ(j.jobj).ResetTransforms();
                }
            };

            // initialize texture editor
            _textureEditor = new DockableTextureEditor();
            _textureEditor.Show(dockPanel1);
            _textureEditor.DockState = DockState.DockLeft;

            _textureEditor.SelectTObj += (tobj) =>
            {
                _propertyGrid.SetObject(tobj);
            };

            _textureEditor.InvalidateTexture += () =>
            {
                RenderJObj.Invalidate();
            };

            // add to renderer
            _viewport.glViewport.AddRenderer(this);

            _viewport.glViewport.FrameChange += (f) =>
            {
                if (JointAnimation != null)
                    JointAnimation.ApplyAnimation(RenderJObj.RootJObj, f);
            };

            // initialize joint manager
            RenderJObj = new RenderJObj();

            // setup params
            renderModeBox.ComboBox.DataSource = Enum.GetValues(typeof(RenderMode));
            viewModeBox.ComboBox.DataSource = Enum.GetValues(typeof(ObjectRenderMode));
            viewModeBox.ComboBox.SelectedIndex = 1;

            renderModeBox.SelectedIndex = 0;
            showBonesToolStripMenuItem.Checked = RenderJObj._settings.RenderBones;
            showBoneOrientationToolStripMenuItem.Checked = RenderJObj._settings.RenderOrientation;
            showSelectionOutlineToolStripMenuItem.Checked = RenderJObj._settings.OutlineSelected;
            showSplinesToolStripMenuItem.Checked = RenderJObj._settings.RenderSplines;
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateFlagsAll()
        {
            if (_root != null)
            {
                _root.UpdateFlags();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jointAnim"></param>
        public void LoadAnimation(JointAnimManager animation)
        {
            JointAnimation = animation;
            var vp = _viewport.glViewport;
            vp.AnimationTrackEnabled = true;
            vp.Frame = 0;
            vp.MaxFrame = animation.FrameCount;
            _trackEditor.Show();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            //RenderJObj.Frame = _viewport.glViewport.Frame;
            RenderJObj._settings.RenderSplines = showSplinesToolStripMenuItem.Checked;
            RenderJObj._settings.RenderBones = showBonesToolStripMenuItem.Checked;
            RenderJObj._settings.RenderOrientation = showBoneOrientationToolStripMenuItem.Checked;
            RenderJObj._settings.OutlineSelected = showSelectionOutlineToolStripMenuItem.Checked;
            RenderJObj.Render(cam);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        public void SetJOBJ(HSD_JOBJ jobj)
        {
            _root = jobj;
            _jointTree.SetJObj(jobj);
            _meshList.SetJObj(jobj);
            RenderJObj.LoadJObj(jobj);
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateVisibility()
        {
            int index = 0;
            Dictionary<int, int> dobjindex = new Dictionary<int, int>();
            foreach (var c in _meshList.EnumerateDObjs)
            {
                // show or hide
                RenderJObj.SetDObjVisible(index, c.Visible);

                // also update dobj index
                if (!dobjindex.ContainsKey(c.JOBJIndex))
                    dobjindex.Add(c.JOBJIndex, 0);

                // update dobj index?
                c.DOBJIndex = dobjindex[c.JOBJIndex];
                dobjindex[c.JOBJIndex] += 1;

                index += 1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fog"></param>
        public void SetFog(HSD_FogDesc fog)
        {
            RenderJObj._fogParam.LoadFromHSD(fog);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        public void SetCamera(HSD_Camera camera)
        {
            _viewport.glViewport.Camera.LoadFromHSD(camera);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void viewModeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RenderJObj != null)
            {
                RenderJObj._settings.RenderObjects = (ObjectRenderMode)(viewModeBox.SelectedIndex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void renderModeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RenderJObj != null)
            {
                RenderJObj.RenderMode = (RenderMode)renderModeBox.SelectedIndex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importModelFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModelImporter.ReplaceModelFromFile(_root);
            SetJOBJ(_root);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportModelToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModelExporter.ExportFile(_root, _jointTree._jointMap);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importSceneSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = FileIO.OpenFile("Scene (*.yaml)|*.yml");

            if (f != null)
                LoadSceneYAML(f);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportSceneSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = FileIO.SaveFile("Scene (*.yaml)|*.yml");

            if (f != null)
            {
                SceneSettings settings = new SceneSettings()
                {
                    Frame = _viewport.glViewport.Frame,
                    CSPMode = _viewport.glViewport.CSPMode,
                    ShowGrid = _viewport.glViewport.DisplayGrid,
                    ShowBackdrop = _viewport.glViewport.EnableBack,
                    Camera = _viewport.glViewport.Camera,
                    Lighting = RenderJObj._lightParam,
                    Settings = RenderJObj._settings,
                    // TODO: Animation = RenderJObj.Animation,
                    HiddenNodes = _meshList.EnumerateDObjs.Where(e => !e.Visible).Select((i, e) => e).ToArray(),
                };
                settings.Serialize(f);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public void LoadSceneYAML(string filePath)
        {
            var settings = SceneSettings.Deserialize(filePath);

            _viewport.glViewport.CSPMode = settings.CSPMode;

            if (settings.CSPMode && showSelectionOutlineToolStripMenuItem.Checked)
                showSelectionOutlineToolStripMenuItem.PerformClick();

            _viewport.glViewport.EnableBack = settings.ShowBackdrop;
            _viewport.glViewport.DisplayGrid = settings.ShowGrid;

            if (settings.Camera != null)
                _viewport.glViewport.Camera = settings.Camera;

            if (settings.Settings != null)
                RenderJObj._settings = settings.Settings;

            if (settings.Lighting != null)
                RenderJObj._lightParam = settings.Lighting;

            if (settings.Animation != null)
            {
                // TODO: animation
                //// load animations
                //LoadAnimation(settings.Animation);

                //// load material animation if exists
                //var symbol = MainForm.SelectedDataNode.Text.Replace("_joint", "_matanim_joint");
                //var matAnim = MainForm.Instance.GetSymbol(symbol);
                //if (matAnim != null && matAnim is HSD_MatAnimJoint maj)
                //    LoadAnimation(maj);

                //// set frames
                //_viewport.glViewport.Frame = settings.Frame;
            }

            if (settings.HiddenNodes != null)
            {
                int i = 0;
                foreach (var d in _meshList.EnumerateDObjs)
                {
                    d.Visible = !settings.HiddenNodes.Contains(i);
                    i++;
                }
                _meshList.Invalidate();
                UpdateVisibility();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void displaySettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (PropertyDialog d = new PropertyDialog("Light Settings", RenderJObj._lightParam))
                d.ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fogSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (PropertyDialog d = new PropertyDialog("Fog Settings", RenderJObj._fogParam))
                d.ShowDialog();
        }
    }
}
