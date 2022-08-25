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

    public class ScriptRenderer : IDrawableInterface
    {
        public DrawOrder DrawOrder => DrawOrder.First;

        public float ShieldSize = 0;

        public SBM_EnvironmentCollision ECB;

        public SBM_PlayerModelLookupTables LookupTable;

        public bool RenderHurtboxes = false;

        public bool RenderShield = false;

        public bool RenderECB = false;

        public bool ECBGrounded = true;

        public bool RenderLedgeBox = false;

        public bool RenderItem = false;

        public bool RenderHitboxInfo = true;

        public bool RenderInterpolation = true;

        public bool RenderBones { get => FighterModel._settings.RenderBones; set => FighterModel._settings.RenderBones = value; }

        public List<SBM_Hurtbox> Hurtboxes = new List<SBM_Hurtbox>();

        public List<ModelPartAnimations> ModelPartsIndices = new List<ModelPartAnimations>();

        public SubactionProcessor Processor = new SubactionProcessor();


        public RenderJObj FighterModel { get; internal set; }
        private RenderJObj ItemModel;
        private RenderJObj ThrowDummyModel;
        private HurtboxRenderer HurtboxRenderer = new HurtboxRenderer();

        private Vector3[] PreviousHitboxPositions = new Vector3[4];
        private Capsule capsule = new Capsule(Vector3.Zero, Vector3.Zero, 0);


        private static Vector3 ShieldColor = new Vector3(1, 0.4f, 0.4f);
        private static Vector3 ThrowDummyColor = new Vector3(0, 1, 1);
        private static Vector3 HitboxColor = new Vector3(1, 0, 0);
        private static Vector3 GrabboxColor = new Vector3(1, 0, 1);
        private static Vector3 HitboxSelectedColor = new Vector3(1, 1, 1);

        /// <summary>
        /// 
        /// </summary>
        public ScriptRenderer()
        {
            FighterModel = new RenderJObj();
            FighterModel._settings.RenderBones = false;

            ItemModel = new RenderJObj();
            ItemModel._settings.RenderBones = false;

            ThrowDummyModel = new RenderJObj();
            ThrowDummyModel._settings.RenderBones = false;


            Processor.UpdateVISMethod += SetModelVis;
            Processor.AnimateMaterialMethod += AnimateMaterial;
            Processor.AnimateModelMethod += AnimateModel;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetScript(HSDStruct str)
        {
            Processor.SetStruct(str, SubactionGroup.Fighter);
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
        public void LoadThrowDummy(string modelPath)
        {
            LoadModel(ThrowDummyModel, modelPath);
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
        public void LoadAnimation(string symbol, JointAnimManager anim)
        {
            // clear animation
            FighterModel.ClearAnimation(FrameFlags.Joint);

            // load figatree to manager and anim editor
            FighterModel.LoadAnimation(anim, null, null);

            // load throw dummy for thrown animations
            //ThrowDummyManager.SetFigaTree(null);
            //if (name.Contains("Throw") && !name.Contains("Taro"))
            //{
            //    // find thrown anim
            //    Action throwAction = null;
            //    foreach (Action a in actionList.Items)
            //    {
            //        if (a.Symbol != null &&
            //            a.Symbol.Contains("Taro") &&
            //            a.Symbol.Contains(name) &&
            //            !a.Symbol.Equals(treeSymbol))
            //        {
            //            throwAction = a;
            //            break;
            //        }
            //    }
            //    // if throw animation is found
            //    if (throwAction != null &&
            //        throwAction.Symbol != null)
            //    {
            //        var throwData = AJManager.GetAnimationData(throwAction.Symbol);
            //        if (throwData != null)
            //        {
            //            // load throw animation
            //            var tanim = new HSDRawFile(throwData);
            //            if (tanim.Roots[0].Data is HSD_FigaTree tree2)
            //            {
            //                ThrowDummyManager.SetFigaTree(tree2);
            //                if (ThrowDummyLookupTable.Count > 0)
            //                {
            //                    ThrowDummyManager.Animation.EnableBoneLookup = true;
            //                    ThrowDummyManager.Animation.BoneLookup = ThrowDummyLookupTable;
            //                }
            //            }
            //        }
            //    }
            //}
        }

        /// <summary>
        /// Calcuates the previous state hitboxes positions and returns them as a dictionary
        /// </summary>
        /// <returns></returns>
        private void CalculatePreviousState(float frame)
        {
            if (frame == 0 || !RenderInterpolation)
                return;

            FighterModel.RequestAnimationUpdate(FrameFlags.Joint, frame - 1);
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
        public void SetFrame(float frame)
        {
            // calculate previous hitbox state
            CalculatePreviousState(frame);

            // reset model state
            ResetModelState();

            // apply joint animation
            FighterModel.RequestAnimationUpdate(FrameFlags.Joint, frame);

            // apply model part anims
            foreach (var mp in ModelPartsIndices)
                mp.Apply(FighterModel.RootJObj);

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
        }

        /// <summary>
        /// 
        /// </summary>
        public void GLFree()
        {
            FighterModel.FreeResources();
            ThrowDummyModel.FreeResources();
            ItemModel.FreeResources();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            if (FighterModel.RootJObj == null)
                return;

            // render fighter model
            if (!Processor.IsInvisible)
                FighterModel.Render(cam);

            // render item model
            if (RenderItem && ItemModel.RootJObj != null && LookupTable != null)
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

            // TODO: render throw dummy

            // render hurtboxes
            if (RenderHurtboxes)
                HurtboxRenderer.Render(FighterModel.RootJObj, Hurtboxes, null);

            // render hitboxes
            int hitboxId = 0;
            bool isHitboxSelected = false;
            foreach (var hb in Processor.Hitboxes)
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
                //if (subActionList.SelectedIndices.Count == 1 && hb.CommandIndex == subActionList.SelectedIndex)
                //{
                //    hbColor = HitboxSelectedColor;
                //    isHitboxSelected = true;
                //    _transWidget.Transform = hb.GetWorldTransform(JointManager.GetJOBJ(0));
                //}

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

                    // TODO: GLTextRenderer.RenderText(cam, hitboxId.ToString(), worldTransform, StringAlignment.Center, true);
                }
                hitboxId++;
            }

            // render shield
            if (RenderShield && LookupTable != null)
                DrawShape.DrawSphere(FighterModel.RootJObj.GetJObjAtIndex(LookupTable.ShieldBone).WorldTransform, ShieldSize / 2, 16, 16, ShieldColor, 0.5f);

            // render gfx spawn
            foreach (var gfx in Processor.GFXOnFrame.ToArray())
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
