using System;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using HSDRaw.MEX;
using HSDRawViewer.Rendering;
using HSDRaw.MEX.Menus;
using OpenTK;
using HSDRaw;
using HSDRaw.Common;
using HSDRaw.MEX.Stages;
using HSDRaw.Common.Animation;
using HSDRaw.Melee.Mn;
using HSDRawViewer.Converters;
using System.IO;

namespace HSDRawViewer.GUI.MEX.Controls
{
    public partial class MEXMenuControl : UserControl, IMEXControl, IDrawableInterface
    {
        /// <summary>
        /// 
        /// </summary>
        public MEX_Data MexData
        {
            get
            {
                var c = Parent;
                while (c != null && !(c is MexDataEditor)) c = c.Parent;
                if (c is MexDataEditor e) return e._data;
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public MexDataEditor MexDataEditor
        {
            get
            {
                var c = Parent;
                while (c != null && !(c is MexDataEditor)) c = c.Parent;
                if (c is MexDataEditor e) return e;
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public MEX_CSSIconEntry[] Icons { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MEXStageIconEntry[] StageIcons { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DrawOrder DrawOrder => DrawOrder.Last;

        /// <summary>
        /// 
        /// </summary>
        private ViewportControl viewport;
        
        public HSDRawFile StageMenuFile;
        private string StageMenuFilePath;

        public HSDRawFile MenuFile;
        private string MenuFilePath;

        public AddedIcon[] AddedIcons;

        private bool CSSSelected => cssIconTabControl.SelectedIndex <= 1;

        private JOBJManager MnSlChrJOBJManager = new JOBJManager();
        private JOBJManager MnSlMapJOBJManager = new JOBJManager();
        private JOBJManager MnSlNameJOBJManager = new JOBJManager();

        public class AddedIcon
        {
            public HSD_JOBJ jobj;

            public float X { get => jobj.TX; set => jobj.TX = value; }
            public float Y { get => jobj.TY; set => jobj.TY = value; }
            public float Z { get => jobj.TZ; set => jobj.TZ = value; }

            public AddedIcon(HSD_JOBJ jobj)
            {
                this.jobj = jobj;
            }

            public override string ToString()
            {
                return $"{X} {Y} {Z}";
            }
        }

        public MEXMenuControl()
        {
            InitializeComponent();


            viewport = new ViewportControl();
            viewport.Dock = DockStyle.Fill;
            viewport.AnimationTrackEnabled = false;
            viewport.AddRenderer(this);
            viewport.EnableFloor = false;
            viewport.MaxFrame = 1600;
            groupBox2.Controls.Add(viewport);
            viewport.RefreshSize();
            viewport.BringToFront();
            //viewport.Visible = false;

            MnSlChrJOBJManager.RenderBones = false;

            HandleDestroyed += (sender, args) =>
            {
                Console.WriteLine("Cleaning up");
                MnSlChrJOBJManager.ClearRenderingCache();
                MnSlMapJOBJManager.ClearRenderingCache();
                viewport.Dispose();
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetControlName()
        {
            return "Menus";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void LoadData(MEX_Data data)
        {
            Icons = new MEX_CSSIconEntry[data.MenuTable.CSSIconData.Icons.Length];
            for (int i = 0; i < Icons.Length; i++)
            {
                Icons[i] = MEX_CSSIconEntry.FromIcon(data.MenuTable.CSSIconData.Icons[i]);
            }
            cssIconEditor.SetArrayFromProperty(this, "Icons");

            StageIcons = data.MenuTable.SSSIconData.Array.Select(e => new MEXStageIconEntry() { IconData = e }).ToArray();
            sssEditor.SetArrayFromProperty(this, "StageIcons");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void SaveData(MEX_Data data)
        {
            data.MetaData.NumOfCSSIcons = Icons.Length;
            MEX_CSSIcon[] ico = new MEX_CSSIcon[Icons.Length];
            for (int i = 0; i < ico.Length; i++)
                ico[i] = Icons[i].ToIcon();
            data.MenuTable.CSSIconData.Icons = ico;

            data.MetaData.NumOfSSSIcons = StageIcons.Length;
            data.MenuTable.SSSIconData.Array = StageIcons.Select(e => e.IconData).ToArray();

            SaveMenuFiles();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetDataBindings()
        {

        }

        #region Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSaveCSS_Click(object sender, EventArgs e)
        {
            SaveData(MexData);
        }

        #endregion

        #region Rendering

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            if (!viewport.Visible)
                return;

            // camera view
            DrawShape.DrawRectangle(32.51167f, -24.38375f, -32.51167f, 24.38375f, Color.Transparent);

            if (CSSSelected)
            {
                MnSlChrJOBJManager.Render(cam);

                foreach (var i in Icons)
                {
                    i.Render(cssIconEditor.SelectedObject == (object)i);
                }
            }
            else if (StageMenuFile != null)
            {
                MnSlMapJOBJManager.Frame = viewport.Frame;
                MnSlMapJOBJManager.Render(cam);

                MnSlNameJOBJManager.Render(cam);

                if (sssEditor.SelectedObject is MEXStageIconEntry ico)
                {
                    var transform = MnSlMapJOBJManager.GetWorldTransform(sssEditor.SelectedIndex + 1);
                    Vector3 point = Vector3.TransformPosition(Vector3.Zero, transform);
                    DrawShape.DrawRectangle(point.X - ico.Width, point.Y + ico.Height, point.X + ico.Width, point.Y - ico.Height, point.Z, 2, MEX_CSSIconEntry.SelectedIconColor);
                }
            }

        }

        #endregion

        #region Viewport Controls

        public void ScreenClick(MouseButtons button, PickInformation pick)
        {
            if (CSSSelected && cssIconEditor.SelectedObject is MEX_CSSIconEntry icon)
            {
                if (button == MouseButtons.Right && MousePrevDown)
                {
                    icon.X = oldPosition.X;
                    icon.Y = oldPosition.Y;
                    MousePrevDown = false;
                }
            }
        }

        public void ScreenDoubleClick(PickInformation pick)
        {
            var planePoint = pick.GetPlaneIntersection(Vector3.UnitZ, Vector3.Zero);

            if (CSSSelected)
                foreach (var i in Icons)
                {
                    if (i.ToRect().Contains(planePoint.X, planePoint.Y))
                    {
                        cssIconEditor.SelectObject(i);
                        break;
                    }
                }
            else
            {
                int index = 0;
                foreach (var i in StageIcons)
                {
                    planePoint = pick.GetPlaneIntersection(Vector3.UnitZ, new Vector3(0, 0, i.Z));
                    var transform = MnSlMapJOBJManager.GetWorldTransform(index++ + 1);
                    Vector3 point = Vector3.TransformPosition(Vector3.Zero, transform);
                    var rect = new RectangleF(point.X - i.Width, point.Y - i.Height, i.Width * 2, i.Height * 2);
                    if (rect.Contains(planePoint.X, planePoint.Y))
                    {
                        sssEditor.SelectObject(i);
                        break;
                    }
                }
            }
        }

        private bool MousePrevDown = false;
        private Vector3 prevPlanePoint = Vector3.Zero;
        private Vector2 oldPosition = Vector2.Zero;
        public void ScreenDrag(PickInformation pick, float deltaX, float deltaY)
        {
            var mouseDown = OpenTK.Input.Mouse.GetState().IsButtonDown(OpenTK.Input.MouseButton.Left);

            if (CSSSelected && cssIconEditor.SelectedObject is MEX_CSSIconEntry icon)
            {
                if (mouseDown &&
                    viewport.IsAltAction)
                {
                    var planePoint = pick.GetPlaneIntersection(Vector3.UnitZ, Vector3.Zero);
                    if (!MousePrevDown)
                    {
                        oldPosition = new Vector2(icon.X, icon.Y);
                        prevPlanePoint = planePoint;
                    }
                    if (icon.ToRect().Contains(prevPlanePoint.X, prevPlanePoint.Y))
                    {
                        icon.X -= prevPlanePoint.X - planePoint.X;
                        icon.Y -= prevPlanePoint.Y - planePoint.Y;
                        SnapAlignIcon(icon);
                    }
                    prevPlanePoint = planePoint;
                }
            }

            MousePrevDown = mouseDown;
        }

        public void ScreenSelectArea(PickInformation start, PickInformation end)
        {

        }

        private float SnapDelta = 0.15f;

        private void SnapAlignIcon(MEX_CSSIconEntry icon)
        {
            foreach (var i in Icons)
            {
                if (i == icon)
                    continue;

                // if distance between part of rect is less than threshold, snap to it
                if (Math.Abs(icon.X - (i.X + i.Width)) < SnapDelta) icon.X = i.X + i.Width;
                if (Math.Abs(icon.X - i.X) < SnapDelta) icon.X = i.X;

                if (Math.Abs((icon.X + icon.Width) - (i.X + i.Width)) < SnapDelta) icon.X = i.X + i.Width - icon.Width;
                if (Math.Abs((icon.X + icon.Width) - i.X) < SnapDelta) icon.X = i.X - icon.Width;

                if (Math.Abs(icon.Y - (i.Y - i.Height)) < SnapDelta) icon.Y = i.Y - i.Height;
                if (Math.Abs(icon.Y - i.Y) < SnapDelta) icon.Y = i.Y;

                if (Math.Abs((icon.Y - icon.Height) - (i.Y - i.Height)) < SnapDelta) icon.Y = i.Y - i.Height + icon.Height;
                if (Math.Abs((icon.Y - icon.Height) - i.Y) < SnapDelta) icon.Y = i.Y + icon.Height;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addedIconEditor_SelectedObjectChanged(object sender, EventArgs e)
        {
            MnSlChrJOBJManager.DOBJManager.SelectedDOBJ = null;
            if (MenuFile != null && addedIconEditor.SelectedObject is AddedIcon ico)
            {
                MnSlChrJOBJManager.DOBJManager.SelectedDOBJ = ico.jobj.Dobj;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sssEditor_SelectedObjectChanged(object sender, EventArgs e)
        {
            if (StageMenuFile != null && sssEditor.SelectedObject is MEXStageIconEntry ico && ico.MapSpace != null)
            {
                MnSlMapJOBJManager.SelectetedJOBJ = ico.MapSpace.JOBJ;
            }

            RefreshStageNameRendering();
        }

        #endregion

        
        /// <summary>
        /// 
        /// </summary>
        private void SaveMenuFiles()
        {
            if (MenuFile != null && MessageBox.Show("Save Change to " + Path.GetFileName(MenuFilePath), "Save Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                MenuFile.Save(MenuFilePath);

            if (StageMenuFile != null && MessageBox.Show("Save Change to " + Path.GetFileName(StageMenuFilePath), "Save Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                RegenerateMnSlMapAnimation();
                StageMenuFile.Save(StageMenuFilePath);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void LoadChrModel(JOBJManager JOBJManager, HSD_JOBJ jobj)
        {
            JOBJManager.ClearRenderingCache();
            JOBJManager.SetJOBJ(jobj);

            addedIconEditor.Visible = true;
            if (!viewport.Visible)
                viewport.Visible = true;

            addedIconEditor.ItemIndexOffset = jobj.BreathFirstList.Count + 1;

            AddedIcons = new AddedIcon[0];
            addedIconEditor.SetArrayFromProperty(this, "AddedIcons");

            if (jobj.Children.Length > 13)
            {
                addedIconEditor.ItemIndexOffset = JOBJManager.IndexOf(jobj.Children[13]) + 1;
                for (int i = 0; i < jobj.Children[13].Children.Length; i++)
                {
                    addedIconEditor.AddItem(new AddedIcon(jobj.Children[13].Children[i]));
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void UnloadMenuFiles()
        {
            {
                MnSlChrJOBJManager.ClearRenderingCache();
                MnSlMapJOBJManager.ClearRenderingCache();
                MnSlNameJOBJManager.ClearRenderingCache();

                mnslchrToolStrip.Visible = false;
                mnslmapToolStrip.Visible = false;

                sssEditor.Visible = false;
                addedIconEditor.Visible = false;
                viewport.Visible = false;

                MenuFile = null;
                StageMenuFile = null;

                MenuFilePath = "";
                StageMenuFilePath = "";

                foreach (var v in StageIcons)
                    v.MapSpace = null;

                addedIconEditor.SetArrayFromProperty(null, null);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void DisplayMenuToolStrip()
        {
            mnslchrToolStrip.Visible = false;
            mnslmapToolStrip.Visible = false;
            viewport.AnimationTrackEnabled = false;
            if (CSSSelected && MenuFile != null)
            {
                mnslchrToolStrip.Visible = true;
            }
            if (!CSSSelected && StageMenuFile != null)
            {
                mnslmapToolStrip.Visible = true;
                viewport.AnimationTrackEnabled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="tobj"></param>
        /// <param name="jobjSection"></param>
        /// <param name="setSelectedIcon"></param>
        private void InjectCSSIconTexture(HSD_JOBJ menu, HSD_AnimJoint anm, HSD_MatAnimJoint matAnm, HSD_TOBJ tobj, int jobjSection, bool setSelectedIcon)
        {
            var iconAnimClone = HSDAccessor.DeepClone<HSD_AnimJoint>(anm.Children[2].Children[0]);
            iconAnimClone.Next = null;
            iconAnimClone.Child = null;

            var iconMatAnimClone = HSDAccessor.DeepClone<HSD_MatAnimJoint>(matAnm.Children[2].Children[0]);
            iconMatAnimClone.Next = null;
            iconMatAnimClone.Child = null;

            var iconClone = HSDAccessor.DeepClone<HSD_JOBJ>(menu.Children[2].Children[0]);
            iconClone.Next = null;
            iconClone.Child = null;
            iconClone.TX = 0;
            iconClone.TY = 0;
            iconClone.TZ = 0;
            iconClone.Dobj.List[1].Mobj.Textures = tobj;

            if (menu.Children.Length < jobjSection)
                menu.AddChild(new HSD_JOBJ() { SX = 1, SY = 1, SZ = 1, Flags = JOBJ_FLAG.CLASSICAL_SCALING | JOBJ_FLAG.ROOT_XLU });
            menu.Children[jobjSection - 1].AddChild(iconClone);

            if (anm.Children.Length < jobjSection)
                anm.AddChild(new HSD_AnimJoint());
            anm.Children[jobjSection - 1].AddChild(iconAnimClone);

            if (matAnm.Children.Length < jobjSection)
                matAnm.AddChild(new HSD_MatAnimJoint());
            matAnm.Children[jobjSection - 1].AddChild(iconMatAnimClone);

            if (setSelectedIcon && cssIconEditor.SelectedObject is MEX_CSSIconEntry ico)
            {
                MnSlChrJOBJManager.ClearRenderingCache();
                MnSlChrJOBJManager.UpdateNoRender();
                addedIconEditor.AddItem(new AddedIcon(iconClone));
                iconClone.TX = ico.X;
                iconClone.TY = ico.Y;
                ico.JointID = MnSlChrJOBJManager.IndexOf(iconClone);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mexMap"></param>
        private void RefreshMnSlMapRendering(MEX_mexMapData mexMap)
        {
            var cloned = HSDAccessor.DeepClone<HSD_JOBJ>(mexMap.PositionModel);
            var tobjs = mexMap.IconMatAnimJoint.Child.MaterialAnimation.Next.TextureAnimation.ToTOBJs();
            var tobjanim = mexMap.IconMatAnimJoint.Child.MaterialAnimation.Next.TextureAnimation.AnimationObject.FObjDesc.GetDecodedKeys();

            for (int i = 0; i < cloned.Children.Length; i++)
            {
                var icon = HSDAccessor.DeepClone<HSD_DOBJ>(mexMap.IconModel.Child.Dobj);
                if ((int)tobjanim[i].Value + 2 < tobjs.Length)
                {
                    icon.Next.Mobj.Textures = tobjs[(int)tobjanim[i].Value + 2];
                    icon.Next.Mobj.Textures.Flags = TOBJ_FLAGS.COORD_UV | TOBJ_FLAGS.LIGHTMAP_DIFFUSE | TOBJ_FLAGS.COLORMAP_MODULATE;
                }
                cloned.Children[i].Dobj = icon;
            }

            MnSlMapJOBJManager.ClearRenderingCache();
            MnSlMapJOBJManager.SetJOBJ(cloned);
            MnSlMapJOBJManager.SetAnimJoint(mexMap.PositionAnimJoint);
            viewport.Frame = 1600;
        }


        /// <summary>
        /// 
        /// </summary>
        private void RefreshStageNameRendering()
        {
            if (StageMenuFile == null)
                return;

            var stage = StageMenuFile.Roots[0].Data as SBM_MnSelectStageDataTable;

            if (stage == null)
                return;

            var cloned = HSDAccessor.DeepClone<HSD_JOBJ>(stage.StageNameModel);

            if (sssEditor.SelectedObject is MEXStageIconEntry entry)
            {
                cloned.Child.Child.Dobj.Mobj.Textures.ImageData = entry.MapSpace.NameTOBJ.ImageData;
                if (entry.MapSpace.NameTOBJ.TlutData != null)
                    cloned.Child.Child.Dobj.Mobj.Textures.TlutData = entry.MapSpace.NameTOBJ.TlutData;
            }

            MnSlNameJOBJManager.ClearRenderingCache();
            MnSlNameJOBJManager.SetJOBJ(cloned);
            MnSlNameJOBJManager.SetAnimJoint(stage.StageNameAnimJoint);
            MnSlNameJOBJManager.Frame = 10;
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void RegenerateMnSlMapAnimation()
        {
            StageIcons = StageIcons.ToList().OrderBy(e => e.ExternalID == 0).ToArray();
            sssEditor.Reset();

            var mexMap = MexMapGenerator.GenerateMexMap(StageMenuFile.Roots[0].Data as SBM_MnSelectStageDataTable, StageIcons.Select(e => e.MapSpace));
            StageMenuFile.Roots[1].Data = mexMap;
            RefreshMnSlMapRendering(mexMap);
        }

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

        private void removeIconButton_Click(object sender, EventArgs e)
        {
            if (cssIconTabControl.SelectedIndex == 1 && addedIconEditor.SelectedObject is AddedIcon ico)
            {
                var index = addedIconEditor.IndexOf(ico);

                addedIconEditor.RemoveAt(index);

                foreach (var v in Icons)
                    if (v.JointID > addedIconEditor.ItemIndexOffset + index)
                        v.JointID--;

                var chrsel = MenuFile.Roots[0].Data as SBM_SelectChrDataTable;

                chrsel.MenuModel.Children[17 - 1].RemoveChildAt(index);
                chrsel.MenuAnimation.Children[17 - 1].RemoveChildAt(index);
                chrsel.MenuMaterialAnimation.Children[17 - 1].RemoveChildAt(index);

                chrsel.SingleMenuModel.Children[13].RemoveChildAt(index);
                chrsel.SingleMenuAnimation.Children[13].RemoveChildAt(index);
                chrsel.SingleMenuMaterialAnimation.Children[13].RemoveChildAt(index);

                MnSlChrJOBJManager.ClearRenderingCache();
            }
        }

        private void regenerateAnimationButton_Click(object sender, EventArgs e)
        {
            RegenerateMnSlMapAnimation();
        }

        private void importStageIconButton_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFile("PNG (*.png)|*.png");
            if (f != null)
            {
                using (Bitmap bmp = new Bitmap(f))
                {
                    var tobj = TOBJConverter.BitmapToTOBJ(bmp, HSDRaw.GX.GXTexFmt.CI8, HSDRaw.GX.GXTlutFmt.RGB565);

                    if (sssEditor.SelectedObject is MEXStageIconEntry ico)
                    {
                        ico.MapSpace.IconTOBJ = tobj;
                        MnSlMapJOBJManager.ClearRenderingCache();
                    }
                }
            }
        }


        public class NameTagSettings
        {
            public string StageName { get; set; } = "";
            public string Location { get; set; } = "";
        }

        private void makeNameTagButton_Click(object sender, EventArgs e)
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
                            entry.MapSpace.NameTOBJ = TOBJConverter.BitmapToTOBJ(bmp, HSDRaw.GX.GXTexFmt.I4, HSDRaw.GX.GXTlutFmt.IA8);
                            RefreshStageNameRendering();
                        }
                    }
                }
            }
        }

        private void loadHSDCamButton_Click(object sender, EventArgs e)
        {
            var cam = (StageMenuFile.Roots[0].Data as SBM_MnSelectStageDataTable).Camera;

            viewport.LoadHSDCamera(cam);
        }

        private void buttonImportMnSlcChr_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);

            if (f != null)
            {
                LoadMnSlChr(f);
            }
        }

        private void buttonImportMnSlMap_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);

            if (f != null)
            {
                LoadMnSlMap(f);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        private void LoadMnSlChr(string filePath)
        {
            HSDRawFile hsd = new HSDRawFile(filePath);

            if (hsd.Roots[0].Data is SBM_SelectChrDataTable tab)
            {
                MenuFilePath = filePath;
                MenuFile = hsd;

                DisplayMenuToolStrip();

                LoadChrModel(MnSlChrJOBJManager, tab.SingleMenuModel);
                MnSlChrJOBJManager.SetAnimJoint(tab.SingleMenuAnimation);
                MnSlChrJOBJManager.Frame = 600;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        private void LoadMnSlMap(string filePath)
        {
            HSDRawFile hsd = new HSDRawFile(filePath);

            var org = hsd["MnSelectStageDataTable"];
            var mex = hsd["mexMapData"];

            if (org != null && mex == null)
            {
                MessageBox.Show("MexMapData symbol not found. One will now be generated", "Symbol Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                hsd.Roots.Add(new HSDRootNode()
                {
                    Name = "mexMapData",
                    Data = MexMapGenerator.GenerateMexMap(org.Data as SBM_MnSelectStageDataTable)
                });
                mex = hsd["mexMapData"];
            }

            if (mex != null)
            {
                //var cam = (hsd.Roots[0].Data as SBM_MnSelectStageDataTable).Camera;

                //viewport.LoadHSDCamera(cam);

                var mexMap = mex.Data as MEX_mexMapData;

                StageMenuFilePath = filePath;
                StageMenuFile = hsd;

                DisplayMenuToolStrip();

                sssEditor.Visible = true;

                RefreshMnSlMapRendering(mexMap);

                var spaces = MexMapGenerator.LoadMexMapDataFromSymbol(hsd.Roots[0].Data as SBM_MnSelectStageDataTable, mexMap);
                for (int i = 0; i < StageIcons.Length; i++)
                {
                    if (i < spaces.Count)
                        StageIcons[i].MapSpace = spaces[i];
                    else
                        StageIcons[i].MapSpace = new MexMapSpace();
                }

                RefreshStageNameRendering();
            }
        }

        private void cssIconTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayMenuToolStrip();
        }
    }
}
