using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Melee.Pl;
using HSDRaw.Tools.Melee;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Rendering.Renderers;
using HSDRawViewer.Rendering.Shapes;
using HSDRawViewer.Tools;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.SubactionEditor
{
    /// <summary>
    /// 
    /// </summary>
    public class ModelPartAnimations
    {
        private byte[] Entries;

        private JointAnimManager[] Anims;

        private int StartBone;

        public int AnimIndex = -1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="part"></param>
        public ModelPartAnimations(SBM_ModelPart part)
        {
            StartBone = part.StartingBone;
            Entries = part.Entries;
            Anims = part.Anims.Array.Select(e => new JointAnimManager(e)).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        /// <param name="frame"></param>
        /// <returns></returns>
        public void Apply(LiveJObj root)
        {
            if (AnimIndex < 0 || AnimIndex >= Anims.Length)
                return;

            var anim = Anims[AnimIndex];

            foreach (var e in Entries)
            {
                // get this joint at index
                var joint = root.GetJObjAtIndex(e);

                if (joint != null)
                {
                    // get part node index
                    int index = e - StartBone;
                    if (index < anim.Nodes.Count)
                    {
                        // set default transforms
                        joint.SetDefaultSRT();

                        // apply part animation to joint
                        var node = anim.Nodes[index];
                        joint.ApplyAnimation(node.Tracks, 0);
                    }
                }
            }
        }
    }

    public class ScriptRenderer
    {
        public float ShieldSize = 0;

        public SBM_EnvironmentCollision ECB;

        public SBM_PlayerModelLookupTables LookupTable;

        public bool RenderFighter { get; set; } = true;

        public bool RenderItem { get; set; } = true;

        public bool RenderHitboxes { get; set; } = true;

        public bool RenderHurtboxes { get; set; } = false;

        public bool RenderHitboxInfo { get; set; } = false;

        public bool RenderInterpolation { get; set; } = false;

        public bool RenderECB { get; set; } = false;

        public bool ECBGrounded { get; set; } = true;

        public bool RenderLedgeBox { get; set; } = false;

        public bool RenderShield { get; set; } = true;

        public bool IsShieldState = false;
        public bool HasItem = false;

        public bool RenderBones { get => FighterModel._settings.RenderBones; set => FighterModel._settings.RenderBones = value; }

        public List<SBM_Hurtbox> Hurtboxes = new List<SBM_Hurtbox>();

        public List<ModelPartAnimations> ModelPartsIndices = new List<ModelPartAnimations>();


        public RenderJObj FighterModel { get; internal set; }
        private RenderJObj ItemModel;

        private RenderJObj ThrowDummyModel;
        private JointAnimManager ThrowDummyAnim;
        private static byte[] ThrowDummyLookup = new byte[]
        {
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x2F, 0x30, 0x31, 0x32, 0x33, 0x35, 0x36, 0x37, 0x38, 0x39, 0xFF, 0xFF, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x12, 0xFF, 0x13, 0x14, 0x16, 0x17, 0x1D, 0x1E, 0x1F, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x3B, 0x3C, 0x00, 0x00
        };
        private static byte[] ThrowDummyHide = new byte[]
        {
            0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F, 0x30, 0x31, 0x32, 0x33, 0x34
        };

        private HurtboxRenderer HurtboxRenderer = new HurtboxRenderer();

        private Vector3[] PreviousHitboxPositions = new Vector3[SubactionProcessor.MaxHitboxCount];
        private Capsule capsule = new Capsule(Vector3.Zero, Vector3.Zero, 0);

        private GLTextRenderer Text = new GLTextRenderer();


        private static Vector3 ShieldColor = new Vector3(1, 0.4f, 0.4f);
        private static Vector3 ThrowDummyColor = new Vector3(0, 1, 1);
        private static Vector3 HitboxColor = new Vector3(1, 0, 0);
        private static Vector3 GrabboxColor = new Vector3(1, 0, 1);
        private static Vector3 HitboxSelectedColor = new Vector3(1, 1, 1);

        /// <summary>
        /// 
        /// </summary>
        public ScriptRenderer(SubactionProcessor Processor)
        {
            FighterModel = new RenderJObj();
            FighterModel._settings.RenderBones = false;
            FighterModel.Initialize += () =>
            {
                ResetModelState();
            };

            ItemModel = new RenderJObj();
            ItemModel._settings.RenderBones = false;

            ThrowDummyModel = new RenderJObj();
            ThrowDummyModel._settings.RenderBones = false;

            ThrowDummyModel.Initialize += () =>
            {
                foreach (var v in ThrowDummyHide)
                    ThrowDummyModel.SetDObjVisible(v, false);
            };

            Processor.UpdateVISMethod += SetModelVis;
            Processor.AnimateMaterialMethod += AnimateMaterial;
            Processor.AnimateModelMethod += AnimateModel;
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadFighterModel(string modelPath)
        {
            // load fighter model
            LoadModel(FighterModel, modelPath);

            // reset model state
            ResetModelState();
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadItemModel(HSD_JOBJ desc, float itemScale)
        {
            ItemModel.ModelScale = itemScale;

            if (desc != null)
            {
                ItemModel.LoadJObj(desc);
            }
            else
            {
                ItemModel.Invalidate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelPath"></param>
        private void LoadModel(RenderJObj renderJobj, string modelPath)
        {
            // load model
            var modelFile = new HSDRawFile(modelPath);
            if (modelFile.Roots.Count > 0 && modelFile.Roots[0].Data is HSD_JOBJ jobj)
            {
                // load fighter model
                renderJobj.LoadJObj(jobj);

                // load material animation if it exists
                if (modelFile.Roots.Count > 1 && modelFile.Roots[1].Data is HSD_MatAnimJoint matanim)
                {
                    renderJobj.LoadAnimation(null, matanim, null);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadAnimation(JointAnimManager anim)
        {
            // clear animation
            FighterModel.ClearAnimation(FrameFlags.Joint);

            // load figatree to manager and anim editor
            FighterModel.LoadAnimation(anim, null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelPath"></param>
        public void LoadThrowDummy(string modelPath)
        {
            // LoadModel(ThrowDummyModel, modelPath);
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadThrowDummyAnimation(JointAnimManager anim)
        {
            // reset throw dummy
            // ThrowDummyModel.ResetDefaultStateAll();

            // set anim
            // ThrowDummyAnim = anim;
        }

        /// <summary>
        /// Calcuates the previous state hitboxes positions and returns them as a dictionary
        /// </summary>
        /// <returns></returns>
        private void CalculatePreviousState(SubactionProcessor Processor, float frame)
        {
            if (frame == 0 || !RenderInterpolation)
            {
                for (int i = 0; i < PreviousHitboxPositions.Length; i++)
                    PreviousHitboxPositions[0] = Vector3.Zero;
                return;
            }

            FighterModel.RequestAnimationUpdate(FrameFlags.Joint, frame - 1);
            FighterModel.RootJObj.RecalculateTransforms(null, true);
            Processor.SetFrame(frame - 1);

            int hitboxId = 0;
            foreach (var hb in Processor.Hitboxes)
            {
                if (hb.Active)
                    PreviousHitboxPositions[hitboxId] = hb.GetWorldPosition(FighterModel.RootJObj);

                hitboxId++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetFrame(SubactionProcessor Processor, float frame)
        {
            // calculate previous hitbox state
            CalculatePreviousState(Processor, frame);

            // reset model state
            ResetModelState();

            // apply joint animation
            FighterModel.RequestAnimationUpdate(FrameFlags.Joint, frame);

            // apply model part anims
            foreach (var mp in ModelPartsIndices)
                mp.Apply(FighterModel.RootJObj);

            // update throw dummy
            if (ThrowDummyAnim != null && ThrowDummyModel.RootJObj != null)
            {
                for (int i = 0; i < ThrowDummyAnim.NodeCount; i++)
                {
                    var node = ThrowDummyAnim.Nodes[i];
                    var joint = ThrowDummyModel.RootJObj.GetJObjAtIndex(ThrowDummyLookup[i]);

                    if (joint != null && node.Tracks.Count > 0)
                    {
                        joint.ApplyAnimation(node.Tracks, frame);
                    }
                }
            }

            // TODO: versus mode previews

            // update processor frame
            Processor.SetFrame(frame);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetModelState()
        {
            // reset material animation
            FighterModel.RequestAnimationUpdate(FrameFlags.Material, 0);

            // lookup table
            if (LookupTable != null)
            {
                // show all dobjs?

                // only show struct 0 vis
                for (int i = 0; i < LookupTable.CostumeVisibilityLookups[0].HighPoly.Length; i++)
                    SetModelVis(i, 0);

                // hide low poly
                foreach (var lut in LookupTable.CostumeVisibilityLookups[0].LowPoly.Array)
                    SetModelVis(lut, -1);
            }

            // reset model parts
            foreach (var mp in ModelPartsIndices)
                mp.AnimIndex = -1;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="structid"></param>
        /// <param name="objectid"></param>
        private void SetModelVis(int structid, int objectid)
        {
            SetModelVis(LookupTable.CostumeVisibilityLookups[0].HighPoly[structid], objectid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="structid"></param>
        /// <param name="objectid"></param>
        private void SetModelVis(SBM_LookupTable lookuptable, int objectid)
        {
            if (lookuptable.LookupEntries != null)
            {
                var structs = lookuptable.LookupEntries.Array;

                for (int i = 0; i < structs.Length; i++)
                    foreach (var v in structs[i].Entries)
                        FighterModel.SetDObjVisible(v, i == objectid);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="frame"></param>
        private void AnimateMaterial(int index, int frame, int matflag, int frameflag)
        {
            if (LookupTable != null && index < LookupTable.MaterialLookupLength && FighterModel.MatAnim != null)
            {
                if (matflag == 1)
                {
                    foreach (var v in LookupTable.CostumeMaterialLookups[0].Entries.Array)
                    {
                        FighterModel.MatAnim.SetFrame(v, frame);
                        FighterModel.RequestAnimationUpdate(FrameFlags.Material, -1);
                    }
                }
                else
                {
                    var idx = LookupTable.CostumeMaterialLookups[0].Entries[index];
                    FighterModel.MatAnim.SetFrame(idx, frame);
                    FighterModel.RequestAnimationUpdate(FrameFlags.Material, -1);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="frame"></param>
        private void AnimateModel(int part_index, int anim_index)
        {
            if (ModelPartsIndices != null && part_index < ModelPartsIndices.Count && part_index >= 0)
                ModelPartsIndices[part_index].AnimIndex = anim_index;
        }

        /// <summary>
        /// 
        /// </summary>
        public void GLInit()
        {
            FighterModel.Invalidate();
            ThrowDummyModel.Invalidate();
            ItemModel.Invalidate();
            Text.InitializeRender(@"lib/Consolas.bff");
        }

        /// <summary>
        /// 
        /// </summary>
        public void GLFree()
        {
            FighterModel.FreeResources();
            ThrowDummyModel.FreeResources();
            ItemModel.FreeResources();
            Text.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        public void Draw(Camera cam, SubactionProcessor processor, List<SubactionEvent> selectedEvents)
        {
            if (FighterModel.RootJObj == null)
                return;

            // render fighter model
            if (RenderFighter && !processor.IsInvisible)
                FighterModel.Render(cam);

            // render item model
            if (HasItem && RenderItem && ItemModel.RootJObj != null && LookupTable != null)
            {
                var fighterHoldBone = FighterModel.RootJObj.GetJObjAtIndex(LookupTable.ItemHoldBone);
                if (fighterHoldBone != null)
                {
                    if (ItemModel.RootJObj.Child != null)
                    {
                        ItemModel.RootJObj.Child.WorldTransform = Matrix4.CreateScale(ItemModel.ModelScale) * fighterHoldBone.WorldTransform;
                        if (ItemModel.RootJObj.Child.Child != null)
                        {
                            ItemModel.RootJObj.Child.Child.RecalculateTransforms(cam, true);
                        }
                    }
                    ItemModel.Render(cam, false);
                }
            }

            // render throw dummy
            if (ThrowDummyAnim != null && !processor.ThrownFighter)
            {
                var hip = ThrowDummyModel.RootJObj.GetJObjAtIndex(ThrowDummyLookup[4]);
                var fighterHoldBone = FighterModel.RootJObj.GetJObjAtIndex(LookupTable.ShieldBone);
                if (hip != null && fighterHoldBone != null)
                {
                    hip.WorldTransform = fighterHoldBone.WorldTransform;
                    hip.Child?.RecalculateTransforms(cam, true);
                }
                ThrowDummyModel.Render(cam, false);
            }

            // render hurtboxes
            if (RenderHurtboxes)
                HurtboxRenderer.Render(FighterModel.RootJObj, Hurtboxes, null);

            // render hitboxes
            if (RenderHitboxes)
            {
                int hitboxId = 0;
                foreach (var hb in processor.Hitboxes)
                {
                    if (!hb.Active)
                    {
                        hitboxId++;
                        continue;
                    }

                    // initial hitbox data
                    float alpha = 0.4f;
                    Vector3 hbColor = HitboxColor;

                    // get transform data
                    var worldPosition = hb.GetWorldPosition(FighterModel.RootJObj);
                    var worldTransform = Matrix4.CreateTranslation(worldPosition);

                    // check for grabbox
                    if (hb.Element == 8)
                        hbColor = GrabboxColor;

                    // check if hitbox is selected
                    if (selectedEvents.Contains(hb.EventSource))
                    {
                        hbColor = HitboxSelectedColor;
                    }

                    // drawing a capsule takes more processing power, so only draw it if necessary
                    if (hb.Interpolate && RenderInterpolation)
                    {
                        capsule.SetParameters(worldPosition, PreviousHitboxPositions[hitboxId], hb.Size);
                        capsule.Draw(Matrix4.Identity, new Vector4(hbColor, alpha));
                    }
                    else
                    {
                        DrawShape.DrawSphere(worldTransform, hb.Size, 16, 16, hbColor, alpha);
                    }

                    // draw hitbox angle
                    if (RenderHitboxInfo)
                    {
                        if (hb.Angle != 361)
                            DrawShape.DrawAngleLine(cam, worldTransform, hb.Size, MathHelper.DegreesToRadians(hb.Angle));
                        else
                            DrawShape.DrawSakuraiAngle(cam, worldTransform, hb.Size);

                        // draw hitbox index
                        Text.RenderText(cam, hitboxId.ToString(), worldTransform, StringAlignment.Center, true);
                    }
                    hitboxId++;
                }
            }

            // render shield
            if (IsShieldState && RenderShield && LookupTable != null)
                DrawShape.DrawSphere(FighterModel.RootJObj.GetJObjAtIndex(LookupTable.ShieldBone).WorldTransform, ShieldSize / 2, 16, 16, ShieldColor, 0.5f);

            // render gfx spawn
            foreach (var gfx in processor.GFXOnFrame.ToArray())
            {
                // do processing on bone id
                var boneID = gfx.Bone;
                if (boneID == 0)
                { 
                    // special case for character like mewtwo with a leading bone
                    var ro = FighterModel.RootJObj.GetJObjAtIndex(1);
                    if (ro != null && ro.Child == null)
                        boneID = 2;
                    else
                        boneID = 1;
                }

                // get bone effect is attached to
                var bone = FighterModel.RootJObj.GetJObjAtIndex(boneID);
                if (bone != null)
                {
                    // get transform without scale
                    var transform = Matrix4.CreateTranslation(gfx.Position) * bone.WorldTransform;
                    transform = transform.ClearScale();

                    // draw sphere indicator
                    DrawShape.DrawSphere(transform, 1f, 16, 16, ThrowDummyColor, 0.5f);
                }

            }

            // render ecb
            if (RenderECB && ECB != null)
                DrawECB();

            // TODO: intro preview border
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private Vector3 GetWorldBonePosition(int index)
        {
            return Vector3.TransformPosition(Vector3.Zero, FighterModel.RootJObj.GetJObjAtIndex(index).WorldTransform);
        }

        /// <summary>
        /// 
        /// </summary>
        private void DrawECB()
        {
            var topN = FighterModel.RootJObj.GetJObjAtIndex(1).WorldTransform.ExtractTranslation();

            var bone1 = GetWorldBonePosition(ECB.ECBBone1);
            var bone2 = GetWorldBonePosition(ECB.ECBBone2);
            var bone3 = GetWorldBonePosition(ECB.ECBBone3);
            var bone4 = GetWorldBonePosition(ECB.ECBBone4);
            var bone5 = GetWorldBonePosition(ECB.ECBBone5);
            var bone6 = GetWorldBonePosition(ECB.ECBBone6);

            var minx = float.MaxValue;
            var miny = float.MaxValue;
            var maxx = float.MinValue;
            var maxy = float.MinValue;

            foreach (var p in new Vector3[] { bone1, bone2, bone3, bone4, bone5, bone6 })
            {
                minx = Math.Min(minx, p.Z);
                maxx = Math.Max(maxx, p.Z);
                miny = Math.Min(miny, p.Y);
                maxy = Math.Max(maxy, p.Y);
            }

            // ecb diamond
            DrawShape.DrawECB(topN, minx, miny, maxx, maxy, ECBGrounded);

            // ledge grab
            if (RenderLedgeBox)
            {
                var correct = Math.Abs(minx - maxx) / 2;

                //behind
                DrawShape.DrawLedgeBox(
                    topN.Z,
                    topN.Y + ECB.VerticalOffsetFromTop - ECB.VerticalScale / 2,
                    topN.Z - (correct + ECB.HorizontalScale),
                    topN.Y + ECB.VerticalOffsetFromTop + ECB.VerticalScale / 2,
                    Color.Red);

                // in front
                DrawShape.DrawLedgeBox(
                    topN.Z,
                    topN.Y + ECB.VerticalOffsetFromTop - ECB.VerticalScale / 2,
                    topN.Z + correct + ECB.HorizontalScale,
                    topN.Y + ECB.VerticalOffsetFromTop + ECB.VerticalScale / 2,
                    Color.Blue);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <param name="pick"></param>
        public void ScreenClick(MouseButtons button, PickInformation pick)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pick"></param>
        public void ScreenDoubleClick(PickInformation pick)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="pick"></param>
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        public void ScreenDrag(MouseEventArgs args, PickInformation pick, float deltaX, float deltaY)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void ScreenSelectArea(PickInformation start, PickInformation end)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kbState"></param>
        public void ViewportKeyPress(KeyEventArgs kbState)
        {
        }
    }
}
