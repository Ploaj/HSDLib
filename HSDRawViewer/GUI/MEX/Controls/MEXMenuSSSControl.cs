using System;
using System.Drawing;
using System.Windows.Forms;
using HSDRawViewer.Rendering;
using OpenTK;
using HSDRawViewer.Converters;
using HSDRaw.Melee.Mn;
using HSDRaw.MEX.Stages;
using HSDRaw;
using System.IO;
using HSDRaw.Common;
using System.Collections.Generic;
using System.Linq;
using HSDRawViewer.Tools;

namespace HSDRawViewer.GUI.MEX.Controls
{
    public partial class MEXMenuSSSControl : UserControl
    {
        public MEXStageIconEntry[] Icons { get; set; }

        private JOBJManager IconJOBJManager = new JOBJManager();
        private JOBJManager StageNameJOBJManager = new JOBJManager();

        public HSDRawFile StageMenuFile;

        /// <summary>
        /// 
        /// </summary>
        public MEXMenuSSSControl()
        {
            InitializeComponent();

            IconJOBJManager.RefreshRendering = true;
            StageNameJOBJManager.RefreshRendering = true;

            Disposed += (sender, args) =>
            {
                ResourceCleanup();
            };
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResourceCleanup()
        {
            {
                var img = iconPreviewBox.Image;
                if (img != null)
                {
                    iconPreviewBox.Image = null;
                    img.Dispose();
                }
            }
            {
                var img = namePreviewBox.Image;
                if (img != null)
                {
                    namePreviewBox.Image = null;
                    img.Dispose();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadFile(HSDRawFile file)
        {
            if (file != null)
                LoadMnSlMap(file);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SaveFile()
        {
            if (StageMenuFile != null)//&& MessageBox.Show("Save Change to " + Path.GetFileName(StageMenuFilePath), "Save Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // regenerate and dump node
                var node = MexMapGenerator.GenerateMexMap(StageMenuFile["MnSelectStageDataTable"].Data as SBM_MnSelectStageDataTable, Icons);

                if (StageMenuFile.Roots.Exists(e => e.Name.Equals("mexMapData")))
                    StageMenuFile["mexMapData"].Data = node;
                else
                    StageMenuFile.Roots.Add(new HSDRootNode() { Name = "mexMapData", Data = node });

                // save file
                //StageMenuFile.TrimData();
                //StageMenuFile.Save(StageMenuFilePath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void CloseFile()
        {
            IconJOBJManager.CleanupRendering();
            StageNameJOBJManager.CleanupRendering();
            
            StageMenuFile = null;

            /*if (Icons != null)
                foreach (var v in Icons)
                {
                    v.Joint = null;
                    v.Animation = null;
                    v.NameTOBJ = null;
                    v.IconTOBJ = null;
                }*/

            Enabled = false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        private void LoadMnSlMap(HSDRawFile hsd)
        {
            if (StageMenuFile != null)
                return;

            var org = hsd["MnSelectStageDataTable"];
            var mex = hsd["mexMapData"];

            if (org != null && mex == null)
            {
                MessageBox.Show("MexMapData symbol not found. One will now be generated", "Symbol Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                // load and convert data from vanilla
                MexMapGenerator.LoadIconDataFromVanilla(org.Data as SBM_MnSelectStageDataTable, Icons);

                // generate mex data node
                hsd.Roots.Add(new HSDRootNode()
                {
                    Name = "mexMapData",
                    Data = MexMapGenerator.GenerateMexMap(org.Data as SBM_MnSelectStageDataTable, Icons)
                });

                mex = hsd["mexMapData"];
            }

            if (org != null && mex != null)
            {
                var stage = org.Data as SBM_MnSelectStageDataTable;
                var mexMap = mex.Data as MEX_mexMapData;

                //StageMenuFilePath = filePath;
                StageMenuFile = hsd;
                
                // Load Data from Mex Symbol
                MexMapGenerator.LoadIconDataFromSymbol(mexMap, Icons);

                // Load Dummy Icon Model
                IconJOBJManager.RefreshRendering = true;
                var icon = HSDAccessor.DeepClone<HSD_JOBJ>(mexMap.IconModel);
                IconJOBJManager.SetJOBJ(icon);

                // Load Dummy Stage Name Model
                StageNameJOBJManager.RefreshRendering = true;
                var name = HSDAccessor.DeepClone<HSD_JOBJ>(stage.StageNameModel);
                StageNameJOBJManager.SetJOBJ(name);
                StageNameJOBJManager.SetAnimJoint(stage.StageNameAnimJoint);
                StageNameJOBJManager.Frame = 10;

                Enabled = true;
            }
        }

        private Dictionary<MEXStageIconEntry, JointAnimManager> entryAnimation = new Dictionary<MEXStageIconEntry, JointAnimManager>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="frame"></param>
        public void Render(Camera cam, float frame)
        {
            if (frame == -1 && entryAnimation.Count > 0)
                entryAnimation.Clear();

            if (frame == 0)
            {
                entryAnimation.Clear();

                // Load Animation
                foreach (var i in Icons)
                {
                    if (i.AnimJoint == null)
                        continue;

                    var a = new JointAnimManager();
                    a.FromAnimJoint(i.AnimJoint);
                    entryAnimation.Add(i, a);
                }
            }

            for(int i = 0; i < Icons.Length; i++)
            {
                var ico = Icons[i];

                if (ico.Joint == null)
                    continue;

                var transform = Matrix4.Identity;

                if (entryAnimation.ContainsKey(ico))
                    transform = entryAnimation[ico].GetAnimatedMatrix(frame, 0, ico.Joint);
                else
                    transform = Matrix4.CreateScale(ico.Joint.SX, ico.Joint.SY, ico.Joint.SZ) *
                Matrix4.CreateFromQuaternion(Math3D.FromEulerAngles(ico.Joint.RZ, ico.Joint.RY, ico.Joint.RX)) *
                Matrix4.CreateTranslation(ico.Joint.TX, ico.Joint.TY, ico.Joint.TZ);

                if (IconJOBJManager.JointCount > 0)
                {
                    IconJOBJManager.SetWorldTransform(0, transform);
                    ico.IconTOBJ.Flags = TOBJ_FLAGS.LIGHTMAP_DIFFUSE | TOBJ_FLAGS.COLORMAP_MODULATE | TOBJ_FLAGS.ALPHAMAP_MODULATE;
                    IconJOBJManager.GetJOBJ(0).Child.Dobj.Next.Mobj.Textures = ico.IconTOBJ;
                    IconJOBJManager.Render(cam, false);
                }

                if (sssEditor.SelectedIndices.Contains(i))
                {
                    if (sssEditor.SelectedIndices.Count == 1 && StageNameJOBJManager.JointCount > 0)
                    {
                        ico.NameTOBJ.Flags = TOBJ_FLAGS.LIGHTMAP_DIFFUSE | TOBJ_FLAGS.COLORMAP_MODULATE | TOBJ_FLAGS.ALPHAMAP_MODULATE;
                        StageNameJOBJManager.GetJOBJ(0).Child.Child.Dobj.Mobj.Textures = ico.NameTOBJ;
                    }

                    var rect = ico.ToRectangle();

                    DrawShape.DrawRectangle(rect.X, rect.Y, rect.X + rect.Width, rect.Y + rect.Height, ico.Joint.TZ, 2, MEX_CSSIconEntry.SelectedIconColor);

                    if (Dragging)
                    {
                        DrawShape.Line(new Vector3(-100, rect.Y, 0), new Vector3(100, rect.Y, 0), SnapColor, 1);
                        DrawShape.Line(new Vector3(rect.X, -100, 0), new Vector3(rect.X, 100, 0), SnapColor, 1);
                        DrawShape.Line(new Vector3(-100, rect.Y + rect.Height, 0), new Vector3(100, rect.Y + rect.Height, 0), SnapColor, 1);
                        DrawShape.Line(new Vector3(rect.X + rect.Width, -100, 0), new Vector3(rect.X + rect.Width, 100, 0), SnapColor, 1);
                    }
                }
            }
            
            StageNameJOBJManager.Render(cam);
        }
        
        private static Vector4 SnapColor = new Vector4(1, 1, 0, 1);
        private Vector3 prevPlanePoint = Vector3.Zero;
        private bool Dragging = false;
        private bool MousePrevDown = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <param name="pick"></param>
        public void ScreenClick(MouseButtons button, PickInformation pick)
        {
            if (sssEditor.SelectedIndex != -1 && button == MouseButtons.Right && MousePrevDown)
            {
                foreach (MEXStageIconEntry ico in sssEditor.SelectedObjects)
                    ico.PopPosition();
                MousePrevDown = false;
                Dragging = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pick"></param>
        public void ScreenDoubleClick(PickInformation pick)
        {
            var planePoint = pick.GetPlaneIntersection(Vector3.UnitZ, Vector3.Zero);

            var key = OpenTK.Input.Keyboard.GetState();

            foreach (var i in Icons)
            {
                planePoint = pick.GetPlaneIntersection(Vector3.UnitZ, new Vector3(0, 0, i.Z));
                var rect = i.ToRectangle();
                if (rect.Contains(planePoint.X, planePoint.Y))
                {
                    sssEditor.SelectObject(i, key.IsKeyDown(OpenTK.Input.Key.ControlRight) || key.IsKeyDown(OpenTK.Input.Key.ControlLeft));
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="pick"></param>
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        public void ScreenDrag(ViewportControl viewport, PickInformation pick, float deltaX, float deltaY)
        {
            var mouseDown = OpenTK.Input.Mouse.GetState().IsButtonDown(OpenTK.Input.MouseButton.Left);

            Dragging = false;

            if (mouseDown && viewport.IsAltAction)
            {
                Vector3 DragMove = Vector3.Zero;

                var planePoint = pick.GetPlaneIntersection(Vector3.UnitZ, Vector3.Zero);

                foreach (MEXStageIconEntry icon in sssEditor.SelectedObjects)
                {
                    if (!MousePrevDown)
                    {
                        icon.PushPosition();
                        prevPlanePoint = planePoint;
                    }
                    if (icon.ToRectangle().Contains(prevPlanePoint.X, prevPlanePoint.Y))
                        DragMove = new Vector3(prevPlanePoint.X - planePoint.X, prevPlanePoint.Y - planePoint.Y, 0);
                }
                prevPlanePoint = planePoint;

                foreach (MEXStageIconEntry icon in sssEditor.SelectedObjects)
                {
                    icon.X -= DragMove.X;
                    icon.Y -= DragMove.Y;

                    Dragging = true;
                }

            }

            MousePrevDown = mouseDown;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sssEditor_SelectedObjectChanged(object sender, EventArgs e)
        {
            if (StageMenuFile != null && sssEditor.SelectedObject is MEXStageIconEntry ico)
            {
                IconJOBJManager.SelectetedJOBJ = ico.Joint;

                ResourceCleanup();

                if(ico.IconTOBJ != null)
                    iconPreviewBox.Image = TOBJConverter.ToBitmap(ico.IconTOBJ);

                if (ico.NameTOBJ != null)
                    namePreviewBox.Image = TOBJConverter.ToBitmap(ico.NameTOBJ);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void regenerateAnimationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sssEditor.SelectedObjects.Length > 0)
            {
                using (PropertyDialog d = new PropertyDialog("Menu Animation Generator", sssEditor.SelectedObjects.Select(a => ((MEXStageIconEntry)a).Animation).ToArray()))
                {
                    d.ShowDialog();
                }
                /*using (PopoutJointAnimationEditor popout = new PopoutJointAnimationEditor())
                {
                    // create a proxy structure/animation?
                    HSD_JOBJ root = null;
                    JointAnimManager manager = new JointAnimManager();
                    
                    foreach(MEXStageIconEntry sel in sssEditor.SelectedObjects)
                    {
                        if(sel.Joint != null && sel.AnimJoint != null)
                        {
                            if (root == null)
                                root = sel.Joint;
                            else
                                root.Add(sel.Joint);
                            var man = new JointAnimManager();
                            man.FromAnimJoint(sel.AnimJoint);
                            manager.Nodes.AddRange(man.Nodes);
                        }
                    }

                    // use 
                    popout.SetJoint(root, manager);
                    popout.ShowDialog();

                    var list = root.BreathFirstList;
                    foreach (var j in list)
                        j.Next = null;

                    // TODO: save anim changes
                }*/
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class NameTagSettings
        {
            public string Location { get; set; } = "";

            public string StageName { get; set; } = "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void generateNameTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sssEditor.SelectedObject is MEXStageIconEntry entry)
            {
                var settings = new NameTagSettings();
                using (PropertyDialog d = new PropertyDialog("Name Tag Settings", settings))
                {
                    if (d.ShowDialog() == DialogResult.OK)
                    {
                        using (Bitmap bmp = MexMapGenerator.GenerateStageName(settings.StageName, settings.Location))
                        {
                            if (bmp == null)
                            {
                                MessageBox.Show("Could not find fonts \"Palatino Linotype\" and/or \"A-OTF Folk Pro H\"", "Font not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            entry.NameTOBJ = TOBJConverter.BitmapToTOBJ(bmp, HSDRaw.GX.GXTexFmt.I4, HSDRaw.GX.GXTlutFmt.IA8);
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
        private void buttonImportName_Click(object sender, EventArgs e)
        {
            var f = FileIO.OpenFile(ApplicationSettings.ImageFileFilter);
            if (f != null)
            {
                using (Bitmap bmp = new Bitmap(f))
                {
                    var tobj = TOBJConverter.BitmapToTOBJ(bmp, HSDRaw.GX.GXTexFmt.IA4, HSDRaw.GX.GXTlutFmt.IA8);

                    if (sssEditor.SelectedObject is MEXStageIconEntry ico)
                    {
                        ico.NameTOBJ = tobj;
                        StageNameJOBJManager.RefreshRendering = true;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importNewIconImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = FileIO.OpenFile(ApplicationSettings.ImageFileFilter);
            if (f != null)
            {
                using (Bitmap bmp = new Bitmap(f))
                {
                    var tobj = TOBJConverter.BitmapToTOBJ(bmp, HSDRaw.GX.GXTexFmt.CI8, HSDRaw.GX.GXTlutFmt.RGB565);

                    if (sssEditor.SelectedObject is MEXStageIconEntry ico)
                    {
                        ico.IconTOBJ = tobj;
                        IconJOBJManager.RefreshRendering = true;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonExportIcon_Click(object sender, EventArgs e)
        {
            var f = FileIO.SaveFile(ApplicationSettings.ImageFileFilter, "Icon.png");
            if (f != null && sssEditor.SelectedObject is MEXStageIconEntry ico && ico.IconTOBJ != null)
            {
                using (var bmp = TOBJConverter.ToBitmap(ico.IconTOBJ))
                    bmp.Save(f);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadGameCameraButton_Click(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadGameCamera()
        {
            var cam = (StageMenuFile.Roots[0].Data as SBM_MnSelectStageDataTable).Camera;
            var par = Parent;
            while (par != null)
            {
                if (par is MEXMenuControl mn)
                {
                    mn.viewport.LoadHSDCamera(cam);
                    break;
                }
                par = par.Parent;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEditUI_CheckedChanged(object sender, EventArgs e)
        {
            if(buttonEditUI.Checked)
                groupBox1.Show();
            else
                groupBox1.Hide();
        }
    }
}
