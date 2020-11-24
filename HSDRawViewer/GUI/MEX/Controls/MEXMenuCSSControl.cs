using System;
using System.Windows.Forms;
using HSDRawViewer.Rendering;
using OpenTK;
using HSDRaw;
using System.IO;
using HSDRaw.Melee.Mn;
using HSDRaw.Common;
using HSDRawViewer.Converters;
using HSDRaw.MEX.Menus;
using System.Drawing;
using HSDRaw.Common.Animation;
using System.Linq;
using HSDRawViewer.Tools;
using HSDRawViewer.GUI.Extra;

namespace HSDRawViewer.GUI.MEX.Controls
{
    public partial class MEXMenuCSSControl : UserControl
    {
        public MEX_CSSIconEntry[] Icons { get; set; }

        private JOBJManager singleMenuManager = new JOBJManager();
        private JOBJManager iconJOBJManager = new JOBJManager();
        private JOBJManager cspJOBJManager = new JOBJManager();

        // make a clone of this model for rendering
        private HSD_JOBJ RenderingClone;
        private HSD_JOBJ CSPRender;

        public HSDRawFile MenuFile;

        private SBM_SelectChrDataTable SelectTable { get => (SBM_SelectChrDataTable)MenuFile?["MnSelectChrDataTable"].Data; }

        private MEX_CSSIconEntry SelectedIcon { get => (MEX_CSSIconEntry)cssIconEditor.SelectedObject; }

        /// <summary>
        /// 
        /// </summary>
        public MEXMenuCSSControl()
        {
            InitializeComponent();

            singleMenuManager.settings.RenderBones = false;
            cspJOBJManager.settings.RenderBones = false;
            iconJOBJManager.settings.RenderBones = false;

            HandleDestroyed += (sender, args) =>
            {
                CloseFile();

                Image oldImage = iconPreviewBox.Image;
                iconPreviewBox.Image = null;
                if (oldImage != null)
                    oldImage.Dispose();
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadFile(HSDRawFile file)
        {
            if (MenuFile == null && file != null)
                LoadMnSlChr(file);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SaveFile()
        {
            if (MenuFile != null)// && MessageBox.Show("Save Change to " + Path.GetFileName(MenuFilePath), "Save Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // regenerate and hook node
                MexCssGenerator.SetMexNode((MEX_mexSelectChr)MenuFile["mexSelectChr"].Data, Icons);

                // save file
                //MenuFile.TrimData();
                //MenuFile.Save(MenuFilePath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void CloseFile()
        {
            singleMenuManager.CleanupRendering();
            iconJOBJManager.CleanupRendering();
            cspJOBJManager.CleanupRendering();
            RenderingClone = null;
            CSPRender = null;

            MenuFile = null;

            Enabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        private void LoadMnSlChr(HSDRawFile datFile)
        {
            // don't load if file is already loaded
            if (MenuFile != null)
                return;

            // find/create mexSelectChr node
            var node = datFile["mexSelectChr"];

            if (node == null && datFile.Roots[0].Data is SBM_SelectChrDataTable menu)
            {
                MessageBox.Show("mexSelectChr symbol not found. One will now be generated", "Symbol Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                datFile.Roots.Add(new HSDRootNode()
                {
                    Name = "mexSelectChr",
                    Data = MexCssGenerator.GenerateMEXMapFromVanilla(menu, Icons)
                });
                node = datFile["mexSelectChr"];
            }
            
            // load data for rendering
            if (node != null && datFile["MnSelectChrDataTable"] != null)
            {
                MenuFile = datFile;

                // mex data node
                var mex = node.Data as MEX_mexSelectChr;

                MexCssGenerator.LoadFromMEXNode(mex, Icons);

                // 
                RenderingClone = HSDAccessor.DeepClone<HSD_JOBJ>(SelectTable.SingleMenuModel);

                CSPRender = HSDAccessor.DeepClone<HSD_JOBJ>(RenderingClone.Children[6]);
                RenderingClone.Children[6].Child = null; 

                // clear old icons
                foreach (var oldIcon in RenderingClone.Children[1].Child.BreathFirstList)
                    oldIcon.Dobj = null;
                foreach (var oldIcon in RenderingClone.Children[2].Children)
                    oldIcon.Dobj = null;

                // clear misc single player menu stuff
                RenderingClone.Children[5].Child = null;
                RenderingClone.Children[7].Child = null;
                RenderingClone.Children[9].Child = null;
                RenderingClone.Children[10].Child = null;
                RenderingClone.Children[11].Child = null;
                RenderingClone.Children[12].Child = null;

                // setup rendering
                singleMenuManager.CleanupRendering();
                singleMenuManager.SetJOBJ(RenderingClone);
                singleMenuManager.SetAnimJoint(SelectTable.SingleMenuAnimation);
                singleMenuManager.Frame = 600;

                cspJOBJManager.CleanupRendering();
                cspJOBJManager.SetJOBJ(CSPRender);

                RefreshMenuRender();
                
                LoadPreviewModel();

                Enabled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RefreshMenuRender()
        {
            CSPRender.Child.Dobj.Mobj.Textures = null;

            if (SelectedIcon != null && cspArrayEditor.SelectedIndex >= 0 && cspArrayEditor.SelectedIndex < SelectedIcon.CSPs.Length)
            {
                CSPRender.Child.Dobj.Mobj.Textures = SelectedIcon.CSPs[cspArrayEditor.SelectedIndex].TOBJ;
                CSPRender.Child.Dobj.Mobj.Textures.Flags = TOBJ_FLAGS.LIGHTMAP_DIFFUSE | TOBJ_FLAGS.COLORMAP_MODULATE | TOBJ_FLAGS.ALPHAMAP_MODULATE;
            }

            cspJOBJManager.CleanupRendering();
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadPreviewModel()
        {
            // refresh preview model
            var mod = MexCssGenerator.GenerateIconModel(Icons);

            var model = mod.Item1;

            // setup rendering
            iconJOBJManager.RefreshRendering = true;
            iconJOBJManager.SetJOBJ(mod.Item1);
            iconJOBJManager.SetAnimJoint(mod.Item2);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        public void Render(Camera cam, float frame)
        {
            if(frame == 0)
            {
                HSD_AnimJoint animJoint = new HSD_AnimJoint();
                foreach (var i in Icons)
                {
                    i.AnimJoint.Next = null;
                    i.AnimJoint.Child = null;
                    animJoint.AddChild(i.AnimJoint);
                }
                iconJOBJManager.SetAnimJoint(animJoint);
            }
            if (frame == -1)
                iconJOBJManager.SetAnimJoint(null);
            else
                iconJOBJManager.Frame = frame;

            singleMenuManager.Render(cam);
            iconJOBJManager.Render(cam);
            cspJOBJManager.Render(cam);

            for (int i = 0; i < Icons.Length; i++)
            {
                var selected = cssIconEditor.SelectedIndices.Contains(i);

                Icons[i].Render(selected);

                if (Dragging && selected)
                {
                    var rect = Icons[i].ToRect();
                    DrawShape.Line(new Vector3(-100, rect.Y, 0), new Vector3(100, rect.Y, 0), SnapColor, 1);
                    DrawShape.Line(new Vector3(rect.X, -100, 0), new Vector3(rect.X, 100, 0), SnapColor, 1);
                    DrawShape.Line(new Vector3(-100, rect.Y + rect.Height, 0), new Vector3(100, rect.Y + rect.Height, 0), SnapColor, 1);
                    DrawShape.Line(new Vector3(rect.X + rect.Width, -100, 0), new Vector3(rect.X + rect.Width, 100, 0), SnapColor, 1);
                }
            }
        }

        private static Vector4 SnapColor = new Vector4(1, 1, 0, 1);
        private bool Dragging = false;
        private bool MousePrevDown = false;
        private Vector3 prevPlanePoint = Vector3.Zero;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <param name="pick"></param>
        public void ScreenClick(MouseButtons button, PickInformation pick)
        {
            if (cssIconEditor.SelectedIndex != -1 && button == MouseButtons.Right && MousePrevDown)
            {
                foreach(MEX_CSSIconEntry ico in cssIconEditor.SelectedObjects)
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
                if (i.ToRect().Contains(planePoint.X, planePoint.Y))
                {
                    cssIconEditor.SelectObject(i, key.IsKeyDown(OpenTK.Input.Key.ControlRight) || key.IsKeyDown(OpenTK.Input.Key.ControlLeft));
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

                foreach (MEX_CSSIconEntry icon in cssIconEditor.SelectedObjects)
                {
                    if (!MousePrevDown)
                    {
                        icon.PushPosition();
                        prevPlanePoint = planePoint;
                    }
                    if (icon.ToRect().Contains(prevPlanePoint.X, prevPlanePoint.Y))
                        DragMove = new Vector3(prevPlanePoint.X - planePoint.X, prevPlanePoint.Y - planePoint.Y, 0);
                }
                prevPlanePoint = planePoint;

                foreach (MEX_CSSIconEntry icon in cssIconEditor.SelectedObjects)
                {
                    icon.PositionX -= DragMove.X;
                    icon.PositionY -= DragMove.Y;

                    Dragging = true;

                    if (enableSnapAlignmentToolStripMenuItem.Checked && cssIconEditor.SelectedObjects.Length <= 1)
                        SnapAlignIcon(icon);
                }

            }

            MousePrevDown = mouseDown;
        }
        
        private float SnapDelta = 0.15f;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="icon"></param>
        private void SnapAlignIcon(MEX_CSSIconEntry icon)
        {
            var rect = icon.ToRect();

            foreach (var i in Icons)
            {
                if (i == icon)
                    continue;

                var coll = i.ToRect();

                // if distance between part of rect is less than threshold, snap to it
                if (Math.Abs(rect.X - (coll.X + coll.Width)) < SnapDelta) icon.PositionX = coll.X + coll.Width - icon.OffsetX;
                if (Math.Abs(rect.X - coll.X) < SnapDelta) icon.PositionX = coll.X - icon.OffsetX;

                if (Math.Abs((rect.X + rect.Width) - (coll.X + coll.Width)) < SnapDelta) icon.PositionX = coll.X + coll.Width - rect.Width - icon.OffsetX;
                if (Math.Abs((rect.X + rect.Width) - coll.X) < SnapDelta) icon.PositionX = coll.X - rect.Width - icon.OffsetX;

                if (Math.Abs(rect.Y - (coll.Y - coll.Height)) < SnapDelta) icon.PositionY = coll.Y - coll.Height - icon.OffsetY + icon.Height;
                if (Math.Abs(rect.Y - coll.Y) < SnapDelta) icon.PositionY = coll.Y - icon.OffsetY + icon.Height;

                if (Math.Abs((rect.Y - rect.Height) - (coll.Y - coll.Height)) < SnapDelta) icon.PositionY = coll.Y - coll.Height + rect.Height - icon.OffsetY + icon.Height;
                if (Math.Abs((rect.Y - rect.Height) - coll.Y) < SnapDelta) icon.PositionY = coll.Y + rect.Height - icon.OffsetY + icon.Height;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cssIconEditor_SelectedObjectChanged(object sender, EventArgs e)
        {
            RefreshCSPRender();
        }

        /// <summary>
        /// 
        /// </summary>
        public void RefreshCSPRender()
        {
            if (SelectedIcon == null || SelectedIcon.CSPs.Length == 0)
                return;

            Image oldImage = iconPreviewBox.Image;
            iconPreviewBox.Image = null;
            if (oldImage != null)
                oldImage.Dispose();

            iconPreviewBox.Image = TOBJConverter.ToBitmap(SelectedIcon.IconTexture);

            cspArrayEditor.SetArrayFromProperty(SelectedIcon, "CSPs");

            RefreshMenuRender();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cssIconEditor_ArrayUpdated(object sender, EventArgs e)
        {
            LoadPreviewModel();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cspArrayEditor.SelectedIndex != -1 && cssIconEditor.SelectedObjects.Length == 1 && cssIconEditor.SelectedObject is MEX_CSSIconEntry icon)
            {
                var f = FileIO.OpenFile(ApplicationSettings.ImageFileFilter);

                if (f != null)
                {
                    HSD_TOBJ tobj = null;
                    using (Bitmap bmp = new Bitmap(f))
                        tobj = Converters.TOBJConverter.BitmapToTOBJ(bmp, HSDRaw.GX.GXTexFmt.CI8, HSDRaw.GX.GXTlutFmt.RGB5A3);
                    
                    if(cspArrayEditor.SelectedObject is TOBJProxy proxy)
                    {
                        proxy.TOBJ = tobj;
                        cspArrayEditor.Invalidate();
                    }
                }
                RefreshCSPRender();
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonExportCSP_Click(object sender, EventArgs e)
        {
            if (cspArrayEditor.SelectedObject is TOBJProxy proxy)
            {
                var f = FileIO.SaveFile(ApplicationSettings.ImageFileFilter, "CSP.png");

                if(f != null)
                {
                    using (var bmp = TOBJConverter.ToBitmap(proxy.TOBJ))
                        bmp.Save(f);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void replaceIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cssIconEditor.SelectedObjects.Length == 1 && cssIconEditor.SelectedObject is MEX_CSSIconEntry icon)
            {
                var f = FileIO.OpenFile(ApplicationSettings.ImageFileFilter);

                if (f != null)
                {
                    HSD_TOBJ tobj = null;
                    using (Bitmap bmp = new Bitmap(f))
                        tobj = TOBJConverter.BitmapToTOBJ(bmp, HSDRaw.GX.GXTexFmt.CI8, HSDRaw.GX.GXTlutFmt.RGB5A3);
                    tobj.Flags = TOBJ_FLAGS.LIGHTMAP_DIFFUSE | TOBJ_FLAGS.COLORMAP_BLEND | TOBJ_FLAGS.ALPHAMAP_BLEND;

                    icon.IconTexture = tobj;

                    Image oldImage = iconPreviewBox.Image;
                    iconPreviewBox.Image = null;
                    if (oldImage != null)
                        oldImage.Dispose();

                    iconPreviewBox.Image = TOBJConverter.ToBitmap(icon.IconTexture);
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
            if (cssIconEditor.SelectedObjects.Length == 1 && cssIconEditor.SelectedObject is MEX_CSSIconEntry icon && icon.IconTexture != null)
            {
                var f = FileIO.SaveFile(ApplicationSettings.ImageFileFilter, "Icon.png");

                if (f != null)
                    using (var bmp = TOBJConverter.ToBitmap(icon.IconTexture))
                        bmp.Save(f);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editAnimationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(cssIconEditor.SelectedObjects.Length > 0)
            {
                using (PropertyDialog d = new PropertyDialog("Menu Animation Generator", cssIconEditor.SelectedObjects.Select(a => ((MEX_CSSIconEntry)a).Animation).ToArray()))
                {
                    d.ShowDialog();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEditUI_CheckedChanged(object sender, EventArgs e)
        {
            groupBox1.Visible = buttonEditUI.Checked;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cspArrayEditor_SelectedObjectChanged(object sender, EventArgs e)
        {
            RefreshMenuRender();
        }
    }
}
