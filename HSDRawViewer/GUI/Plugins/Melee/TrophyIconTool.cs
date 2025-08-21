using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Melee.Ty;
using HSDRaw.Tools;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Rendering.Widgets;
using OpenTK.Mathematics;
using System;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.Melee
{
    [SupportedTypes(new Type[] { typeof(SBM_TrophyIcon) })]
    public partial class TrophyIconTool : PluginBase, IDrawableInterface
    {
        public override DataNode Node
        {
            get => _node;
            set
            {
                _node = value;
                Init();
            }
        }

        private TranslationWidget _widget = new TranslationWidget();
        private RenderJObj TrophyJoint;
        private RenderJObj StandJoint;

        public DrawOrder DrawOrder => DrawOrder.Last;

        private readonly ViewportControl viewport;

        private DataNode _node;
        public TrophyIconTool()
        {
            InitializeComponent();

            viewport = new ViewportControl();
            viewport.Dock = DockStyle.Fill;
            viewport.AnimationTrackEnabled = false;
            viewport.AddRenderer(this);
            viewport.DisplayGrid = true;
            viewport.RefreshSize();
            viewport.BringToFront();

            previewBox.Controls.Add(viewport);

            // load trophy base from archive
            var joint = MainForm.Instance.GetSymbol("ToyDspExt_Top_joint") as HSD_JOBJ;
            if (joint != null)
            {
                TrophyJoint = new RenderJObj(HSDAccessor.DeepClone<HSD_JOBJ>(joint));
            }

            // load stand model from filesystem
            var p = Path.Combine(Path.GetDirectoryName(MainForm.Instance.FilePath), "TyStandD.dat");
            if (File.Exists(p))
            {
                var f = new HSDRawFile(p);
                if (f.Roots.Count > 0 &&
                    f.Roots[0].Data is HSD_JOBJ standj)
                {
                    StandJoint = new RenderJObj(standj);
                }
            }

            FormClosing += (sender, args) =>
            {
                viewport.Dispose();
            };

            _widget.TransformUpdated += (tr) =>
            {
                if (arrayEditor.SelectedObject is TrophyDisplay t)
                {
                    var trans = tr.ExtractTranslation();
                    t.XOffset = trans.X;
                    t.YOffset = trans.Y;
                    t.ZOffset = trans.Z;
                    UpdateTrophyModel();
                }
            };

            arrayEditor.ArrayUpdated += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine("Array edited");
                UpdateTrophyModel();
            };

            arrayEditor.SelectedObjectChanged += (s, e) =>
            {
                if (arrayEditor.SelectedObject is TrophyDisplay t)
                {
                    _widget.Transform = Matrix4.CreateTranslation(t.XOffset, t.YOffset, t.ZOffset);
                    UpdateTrophyModel();
                }
            };
        }

        private void UpdateTrophyModel()
        {
            if (TrophyJoint == null)
                return;

            if (arrayEditor.SelectedObject is TrophyDisplay t)
            {
                TrophyJoint.RootJObj.Translation = new Vector3(t.XOffset, t.YOffset, t.ZOffset);
                TrophyJoint.RootJObj.Scale.X = t.Scale * (t.Texture.ImageData.Width / 128f);
                TrophyJoint.RootJObj.Scale.Y = t.Scale * (t.Texture.ImageData.Height / 128f);
                TrophyJoint.RootJObj.Desc.Dobj.Mobj.Textures.ImageData = t.Texture.ImageData;
                TrophyJoint.RootJObj.Desc.Dobj.Mobj.Textures.TlutData = t.Texture.TlutData;
            }
        }

        public class TrophyDisplay : ImageArrayItem
        {
            public HSD_TOBJ Texture;
            public float Scale { get; set; }
            public float XOffset { get; set; }
            public float YOffset { get; set; }
            public float ZOffset { get; set; }

            public void Dispose()
            {
            }

            public Image ToImage()
            {
                return Texture.ToImage().ToBitmap();
            }
        }
        public TrophyDisplay[] Trophies { get; set; }

        private void Init()
        {
            // extract material animations
            var matanim = MainForm.Instance.GetSymbol("ToyDspExt_Top_matanim_joint") as HSD_MatAnimJoint;
            System.Diagnostics.Debug.WriteLine(matanim);
            if (matanim == null ||
                matanim.MaterialAnimation == null ||
                matanim.MaterialAnimation.TextureAnimation == null)
                return;

            // extract trophy display information
            var anim = MainForm.Instance.GetSymbol("ToyDspExt_Top_animjoint") as HSD_AnimJoint;

            // load track data
            FOBJ_Player trax = null, tray = null, traz = null, scax = null, scay = null;
            if (anim != null &&
                anim.AOBJ != null)
            {
                var fobj = anim.AOBJ.FObjDesc;
                while (fobj != null)
                {
                    switch (fobj.JointTrackType)
                    {
                        case JointTrackType.HSD_A_J_TRAX: trax = new FOBJ_Player(fobj); break;
                        case JointTrackType.HSD_A_J_TRAY: tray = new FOBJ_Player(fobj); break;
                        case JointTrackType.HSD_A_J_TRAZ: traz = new FOBJ_Player(fobj); break;
                        case JointTrackType.HSD_A_J_SCAX: scax = new FOBJ_Player(fobj); break;
                        case JointTrackType.HSD_A_J_SCAY: scay = new FOBJ_Player(fobj); break;
                    }

                    fobj = fobj.Next;
                }
            }

            // create trophy classes
            var textures = matanim.MaterialAnimation.TextureAnimation.ToTOBJs();
            Trophies = new TrophyDisplay[textures.Length];
            for (int i = 0; i < textures.Length; i++)
            {
                // extract translation and scale
                float tx = 0, ty = 0, tz = 0, sx = 1, sy = 1;
                if (trax != null) tx = trax.GetValue(i);
                if (tray != null) ty = tray.GetValue(i);
                if (traz != null) tz = traz.GetValue(i);
                if (scax != null) sx = scax.GetValue(i);
                if (scay != null) sy = scay.GetValue(i);

                // calculate scale
                Trophies[i] = new TrophyDisplay()
                {
                    Texture = textures[i],
                    XOffset = tx,
                    YOffset = ty,
                    ZOffset = tz,
                    Scale = (float)(sx / (textures[i].ImageData.Width / 128.0)),
                };
            }

            arrayEditor.SetArrayFromProperty(this, "Trophies");
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            var trax = Trophies.Select((e, i) => new FOBJKey() { Frame = i, Value = e.XOffset, InterpolationType = GXInterpolationType.HSD_A_OP_CON });
            var tray = Trophies.Select((e, i) => new FOBJKey() { Frame = i, Value = e.YOffset, InterpolationType = GXInterpolationType.HSD_A_OP_CON });
            var traz = Trophies.Select((e, i) => new FOBJKey() { Frame = i, Value = e.ZOffset, InterpolationType = GXInterpolationType.HSD_A_OP_CON });
            var scax = Trophies.Select((e, i) => new FOBJKey() { Frame = i, Value = e.Scale * (e.Texture.ImageData.Width / 128f), InterpolationType = GXInterpolationType.HSD_A_OP_CON });
            var scay = Trophies.Select((e, i) => new FOBJKey() { Frame = i, Value = e.Scale * (e.Texture.ImageData.Height / 128f), InterpolationType = GXInterpolationType.HSD_A_OP_CON });

            var tracks = new HSD_AOBJ();
            tracks.AddTrack((byte)JointTrackType.HSD_A_J_TRAX, trax.ToList());
            tracks.AddTrack((byte)JointTrackType.HSD_A_J_TRAY, tray.ToList());
            tracks.AddTrack((byte)JointTrackType.HSD_A_J_TRAZ, traz.ToList());
            tracks.AddTrack((byte)JointTrackType.HSD_A_J_SCAX, scax.ToList());
            tracks.AddTrack((byte)JointTrackType.HSD_A_J_SCAY, scay.ToList());

            var texanim = new HSD_TexAnim();
            texanim.FromTOBJs(Trophies.Select(e => e.Texture), true);

            HSD_AnimJoint anim = new HSD_AnimJoint()
            {
                AOBJ = tracks,
            };
            HSD_MatAnimJoint matanim = new HSD_MatAnimJoint()
            {
                MaterialAnimation = new HSD_MatAnim()
                {
                    TextureAnimation = texanim,
                }
            };

            var matanimr = MainForm.Instance.GetSymbol("ToyDspExt_Top_matanim_joint") as HSD_MatAnimJoint;
            var animr = MainForm.Instance.GetSymbol("ToyDspExt_Top_animjoint") as HSD_AnimJoint;

            if (matanimr == null)
                MainForm.AddRoot("ToyDspExt_Top_matanim_joint", matanim);
            else
                matanimr._s = matanim._s;

            if (animr == null)
                MainForm.AddRoot("ToyDspExt_Top_animjoint", anim);
            else
                animr._s = anim._s;
        }

        public void ViewportKeyPress(KeyEventArgs kbState)
        {
        }

        public void ScreenClick(MouseButtons button, PickInformation pick)
        {
        }

        public void ScreenDoubleClick(PickInformation pick)
        {
        }

        public void ScreenDrag(MouseEventArgs args, PickInformation pick, float deltaX, float deltaY)
        {
            if (args.Button == MouseButtons.Left)
                _widget.MouseDown(pick);
            else
                _widget.MouseUp();

            _widget.Drag(pick);

            if (_widget.PendingUpdate)
            {
                _widget.PendingUpdate = false;
            }
        }

        public void ScreenSelectArea(PickInformation start, PickInformation end)
        {
        }

        public bool FreezeCamera()
        {
            return _widget.Interacting;
        }

        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            StandJoint?.Render(cam);
            TrophyJoint?.Render(cam);
            _widget.Render(cam);
        }

        public void GLInit()
        {
            viewport.Camera.DefaultTranslation = new OpenTK.Mathematics.Vector3(0, 5, -160);
            viewport.Camera.DefaultRotationX = (float)(18 * Math.PI / 180);
            viewport.Camera.RestoreDefault();
        }

        public void GLFree()
        {
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            var t = TOBJExtentions.ImportTObjFromFile(HSDRaw.GX.GXTexFmt.CI8, HSDRaw.GX.GXTlutFmt.RGB5A3);
            if (t != null)
            {
                arrayEditor.AddItem(new TrophyDisplay()
                {
                    Texture = t,
                    Scale = 1,
                });
            }
        }
    }
}
