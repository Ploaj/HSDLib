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
        private string MenuFilePath;

        private SBM_SelectChrDataTable SelectTable { get => (SBM_SelectChrDataTable)MenuFile?["MnSelectChrDataTable"].Data; }

        private MEX_CSSIconEntry SelectedIcon { get => (MEX_CSSIconEntry)cssIconEditor.SelectedObject; }

        /// <summary>
        /// 
        /// </summary>
        public MEXMenuCSSControl()
        {
            InitializeComponent();

            singleMenuManager.RenderBones = false;
            cspJOBJManager.RenderBones = false;
            iconJOBJManager.RenderBones = false;

            HandleDestroyed += (sender, args) =>
            {
                CloseFile();
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadFile()
        {
            var path = Path.Combine(Path.GetDirectoryName(MainForm.Instance.FilePath), "MnSlChr.usd");
            if(!File.Exists(path))
                path = FileIO.OpenFile(ApplicationSettings.HSDFileFilter);

            if (path != null)
            {
                LoadMnSlChr(path);
                Enabled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SaveFile()
        {
            if (MenuFile != null && MessageBox.Show("Save Change to " + Path.GetFileName(MenuFilePath), "Save Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // regenerate and hook node
                MexCssGenerator.SetMexNode((MEX_mexSelectChr)MenuFile["mexSelectChr"].Data, Icons);
                MexCssGenerator.HookMexNode((SBM_SelectChrDataTable)MenuFile["MnSelectChrDataTable"].Data, (MEX_mexSelectChr)MenuFile["mexSelectChr"].Data);

                // save file
                MenuFile.Save(MenuFilePath);
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
            MenuFilePath = "";

            Enabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        private void LoadMnSlChr(string filePath)
        {
            HSDRawFile hsd = new HSDRawFile(filePath);

            var node = hsd["mexSelectChr"];

            if (node == null && hsd.Roots[0].Data is SBM_SelectChrDataTable menu)
            {
                MessageBox.Show("mexSelectChr symbol not found. One will now be generated", "Symbol Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                hsd.Roots.Add(new HSDRootNode()
                {
                    Name = "mexSelectChr",
                    Data = MexCssGenerator.GenerateMEXMapFromVanilla(menu, Icons)
                });
                node = hsd["mexSelectChr"];
            }

            if (node != null && hsd.Roots[0].Data is SBM_SelectChrDataTable tab)
            {
                MenuFilePath = filePath;
                MenuFile = hsd;

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
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RefreshMenuRender()
        {
            CSPRender.Child.Dobj.Mobj.Textures = null;

            if (SelectedIcon != null && cspSelectBox.SelectedIndex >= 0 && cspSelectBox.SelectedIndex < SelectedIcon.CSPs.Count)
            {
                CSPRender.Child.Dobj.Mobj.Textures = SelectedIcon.CSPs[cspSelectBox.SelectedIndex];
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
        private void cspSelectBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshMenuRender();
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
            cspSelectBox.Items.Clear();

            if (SelectedIcon == null || SelectedIcon.CSPs.Count == 0)
                return;

            for (int i = 0; i < SelectedIcon.CSPs.Count; i++)
                cspSelectBox.Items.Add("CSP " + i.ToString());

            cspSelectBox.SelectedIndex = 0;
            RefreshMenuRender();
        }

        /// <summary>
        /// 
        /// </summary>
        public void CalculateEvenSpacing()
        {
            foreach(var ico in Icons)
            {

            }
        }

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
            if (cspSelectBox.SelectedIndex != -1 && cssIconEditor.SelectedObjects.Length == 1 && cssIconEditor.SelectedObject is MEX_CSSIconEntry icon)
            {
                var f = FileIO.OpenFile(ApplicationSettings.ImageFileFilter);

                if (f != null)
                {
                    HSD_TOBJ tobj = null;
                    using (Bitmap bmp = new Bitmap(f))
                        tobj = Converters.TOBJConverter.BitmapToTOBJ(bmp, HSDRaw.GX.GXTexFmt.CI8, HSDRaw.GX.GXTlutFmt.RGB5A3);

                    icon.CSPs[cspSelectBox.SelectedIndex] = tobj;
                }
                RefreshCSPRender();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cspSelectBox.SelectedIndex != -1 && cssIconEditor.SelectedObjects.Length == 1 && cssIconEditor.SelectedObject is MEX_CSSIconEntry icon)
            {
                var f = FileIO.OpenFile(ApplicationSettings.ImageFileFilter);

                if (f != null)
                {
                    HSD_TOBJ tobj = null;
                    using (Bitmap bmp = new Bitmap(f))
                        tobj = Converters.TOBJConverter.BitmapToTOBJ(bmp, HSDRaw.GX.GXTexFmt.CI8, HSDRaw.GX.GXTlutFmt.RGB5A3);

                    icon.CSPs.Add(tobj);
                }
                RefreshCSPRender();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(cspSelectBox.SelectedIndex != -1 && cssIconEditor.SelectedObjects.Length == 1 && cssIconEditor.SelectedObject is MEX_CSSIconEntry icon)
            {
                icon.CSPs.RemoveAt(cspSelectBox.SelectedIndex);
                RefreshCSPRender();
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
                        tobj = Converters.TOBJConverter.BitmapToTOBJ(bmp, HSDRaw.GX.GXTexFmt.CI8, HSDRaw.GX.GXTlutFmt.RGB5A3);

                    icon.Joint.Dobj.Next.Mobj.Textures = tobj;
                    tobj.Flags = TOBJ_FLAGS.LIGHTMAP_DIFFUSE | TOBJ_FLAGS.COLORMAP_BLEND | TOBJ_FLAGS.ALPHAMAP_BLEND;
                }
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

        /*
         *
        private void importIconButton_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFile("PNG (*.png)|*.png");

            if (f != null)
            {
                HSD_TOBJ tobj = null;
                using (Bitmap bmp = new Bitmap(f))
                    tobj = Converters.TOBJConverter.BitmapToTOBJ(bmp, HSDRaw.GX.GXTexFmt.CI8, HSDRaw.GX.GXTlutFmt.RGB5A3);

                var chrsel = MenuFile.Roots[0].Data as SBM_SelectChrDataTable;

                InjectCSSIconTexture(chrsel.MenuModel, chrsel.MenuAnimation, chrsel.MenuMaterialAnimation, tobj, 17, false);
                InjectCSSIconTexture(chrsel.SingleMenuModel, chrsel.SingleMenuAnimation, chrsel.SingleMenuMaterialAnimation, tobj, 13, true);
            }
        }
        
         * 
         */

    }
}
