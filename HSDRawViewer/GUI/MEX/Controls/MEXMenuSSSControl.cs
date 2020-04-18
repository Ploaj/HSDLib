using System;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using HSDRawViewer.Rendering;
using OpenTK;
using HSDRawViewer.Converters;
using HSDRaw.Melee.Mn;
using HSDRaw.MEX.Stages;
using HSDRaw;
using System.IO;
using HSDRaw.Common;

namespace HSDRawViewer.GUI.MEX.Controls
{
    public partial class MEXMenuSSSControl : UserControl
    {
        public MEXStageIconEntry[] StageIcons { get; set; }

        private JOBJManager MnSlMapJOBJManager = new JOBJManager();
        private JOBJManager MnSlNameJOBJManager = new JOBJManager();

        public HSDRawFile StageMenuFile;
        private string StageMenuFilePath;

        /// <summary>
        /// 
        /// </summary>
        public MEXMenuSSSControl()
        {
            InitializeComponent();

            MnSlMapJOBJManager.ClearRenderingCache();
            MnSlNameJOBJManager.ClearRenderingCache();
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadFile()
        {
            var path = Path.Combine(Path.GetDirectoryName(MainForm.Instance.FilePath), "MnSlMap.usd");
            if (!File.Exists(path))
                path = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);

            if (path != null)
            {
                LoadMnSlMap(path);
                Enabled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SaveFile()
        {
            RegenerateMnSlMapAnimation();
            if (StageMenuFile != null && MessageBox.Show("Save Change to " + Path.GetFileName(StageMenuFilePath), "Save Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                StageMenuFile.Save(StageMenuFilePath);
        }

        /// <summary>
        /// 
        /// </summary>
        public void CloseFile()
        {
            MnSlMapJOBJManager.ClearRenderingCache();
            MnSlNameJOBJManager.ClearRenderingCache();
            
            StageMenuFile = null;
            StageMenuFilePath = "";

            foreach (var v in StageIcons)
                v.MapSpace = null;

            Enabled = false;
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
            MnSlMapJOBJManager.Frame = 1200;
        }

        public void Render(Camera cam, float frame)
        {
            //MnSlMapJOBJManager.Frame = frame;
            MnSlMapJOBJManager.Render(cam);

            MnSlNameJOBJManager.Render(cam);

            if (sssEditor.SelectedIndex != -1)
            {
                var ico = StageIcons[sssEditor.SelectedIndex];
                var transform = MnSlMapJOBJManager.GetWorldTransform(sssEditor.SelectedIndex + 1);
                Vector3 point = Vector3.TransformPosition(Vector3.Zero, transform);
                DrawShape.DrawRectangle(point.X - ico.Width, point.Y + ico.Height, point.X + ico.Width, point.Y - ico.Height, point.Z, 2, MEX_CSSIconEntry.SelectedIconColor);
            }
        }

        private bool MousePrevDown = false;
        private Vector3 prevPlanePoint = Vector3.Zero;
        private Vector2 oldPosition = Vector2.Zero;

        public void ScreenClick(MouseButtons button, PickInformation pick)
        {

        }

        public void ScreenDoubleClick(PickInformation pick)
        {
            var planePoint = pick.GetPlaneIntersection(Vector3.UnitZ, Vector3.Zero);

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


        public void ScreenDrag(ViewportControl viewport, PickInformation pick, float deltaX, float deltaY)
        {

        }

        private void sssEditor_SelectedObjectChanged(object sender, EventArgs e)
        {
            if (StageMenuFile != null && sssEditor.SelectedObject is MEXStageIconEntry ico && ico.MapSpace != null)
            {
                MnSlMapJOBJManager.SelectetedJOBJ = ico.MapSpace.JOBJ;
            }

            RefreshStageNameRendering();
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

            //var mexMap = MexMapGenerator.GenerateMexMap(StageMenuFile.Roots[0].Data as SBM_MnSelectStageDataTable, StageIcons.Select(e => e.MapSpace));
            //StageMenuFile.Roots[1].Data = mexMap;
            //RefreshMnSlMapRendering(mexMap);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void regenerateAnimationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegenerateMnSlMapAnimation();
        }

        public class NameTagSettings
        {
            public string StageName { get; set; } = "";
            public string Location { get; set; } = "";
        }

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
                            entry.MapSpace.NameTOBJ = TOBJConverter.BitmapToTOBJ(bmp, HSDRaw.GX.GXTexFmt.I4, HSDRaw.GX.GXTlutFmt.IA8);
                            RefreshStageNameRendering();
                        }
                    }
                }
            }
        }

        private void importNewIconImageToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void loadGameCameraButton_Click(object sender, EventArgs e)
        {
            //var cam = (StageMenuFile.Roots[0].Data as SBM_MnSelectStageDataTable).Camera;

            //viewport.LoadHSDCamera(cam);
        }
    }
}
