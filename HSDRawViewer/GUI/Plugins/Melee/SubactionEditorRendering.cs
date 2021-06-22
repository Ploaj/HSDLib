using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Melee;
using HSDRaw.Melee.Pl;
using HSDRaw.Tools.Melee;
using HSDRawViewer.Converters.Animation;
using HSDRawViewer.GUI.Extra;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Rendering.Renderers;
using HSDRawViewer.Rendering.Shapes;
using HSDRawViewer.Tools;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.Melee
{
    public partial class SubactionEditor : IDrawable
    {
        /// <summary>
        /// 
        /// </summary>
        public class ModelPartAnimations : IJointFrameModifier
        {
            private byte[] Entries;

            private JointAnimManager[] Anims;

            private int StartBone;

            public int AnimIndex = -1;

            public ModelPartAnimations(SBM_ModelPart part)
            {
                StartBone = part.StartingBone;

                Entries = new byte[part.Count];
                for (int i = 0; i < part.Count; i++)
                    Entries[i] = part.Entries[i];

                Anims = part.Anims.Array.Select(e => new JointAnimManager(e)).ToArray();
            }

            public bool OverrideAnim(float frame, int boneIndex, HSD_JOBJ jobj, ref float TX, ref float TY, ref float TZ, ref float RX, ref float RY, ref float RZ, ref float SX, ref float SY, ref float SZ)
            {
                // check if bone index is in entries
                if (AnimIndex == -1 || boneIndex < StartBone || boneIndex >= StartBone + Anims[0].NodeCount)
                    return false;

                // get anim for entry
                foreach (var e in Entries)
                    if (e == boneIndex && AnimIndex < Anims.Length)
                    {
                        var anim = Anims[AnimIndex];
                        anim.GetAnimatedState(0, boneIndex - StartBone, jobj, out TX, out TY, out TZ, out RX, out RY, out RZ, out SX, out SY, out SZ);
                        return true;
                    }

                return false;
            }
        }

        private ViewportControl viewport;

        private PopoutJointAnimationEditor _animEditor = new PopoutJointAnimationEditor(false);

        private JOBJManager JointManager = new JOBJManager();
        private JOBJManager ThrowDummyManager = new JOBJManager();
        private Dictionary<int, int> ThrowDummyLookupTable = new Dictionary<int, int>();

        private SBM_EnvironmentCollision ECB = null;

        public DrawOrder DrawOrder => DrawOrder.Last;

        private float DisplayShieldSize = 0;

        private ModelPartAnimations[] ModelPartsIndices;

        private SubactionProcessor SubactionProcess = new SubactionProcessor();

        private List<SBM_Hurtbox> Hurtboxes = new List<SBM_Hurtbox>();

        private HurtboxRenderer HurtboxRenderer = new HurtboxRenderer();

        private static Vector3 ShieldColor = new Vector3(1, 0.4f, 0.4f);
        private static Vector3 ThrowDummyColor = new Vector3(0, 1, 1);
        private static Vector3 HitboxColor = new Vector3(1, 0, 0);
        private static Vector3 GrabboxColor = new Vector3(1, 0, 1);
        private float ModelScale = 1f;

        private FighterAJManager AJManager;

        private string AJFilePath;

        private string ResultFilePath;
        private string EndingFilePath;
        private string IntroFilePath;
        private string WaitFilePath;

        private string ResultSymbol;
        private string EndingSymbol;
        private string IntroSymbol;
        private string WaitSymbol;

        /// <summary>
        /// 
        /// </summary>
        public void OnDatFileSave()
        {
            if (!string.IsNullOrEmpty(AJFilePath))
                SaveFighterAnimationFile();

            if (!string.IsNullOrEmpty(ResultFilePath))
                SaveDemoAnimationFiles();
        }

        /// <summary>
        /// Initializes rendering
        /// </summary>
        private void InitRendering()
        {
            // set model scale
            JointManager.ModelScale = ModelScale;

            // clear hidden dobjs
            JointManager.DOBJManager.HiddenDOBJs.Clear();

            // don't render bones by default
            JointManager._settings.RenderBones = false;

            // reset model visibility
            ResetModelVis();

            // load the model parts
            LoadModelParts();

            // enable preview box
            previewBox.Visible = true;

            // reselect action
            if (actionList.SelectedItem is Action action)
                SelectAction(action);

            // load bone table if plco is found

            //string DummyModelFile = "PLMrNr.dat";
            //int DummyModelExternalId = 0;

            /*var plcoFile = Path.Combine(Path.GetDirectoryName(MainForm.Instance.FilePath), "PlCo.dat");
            var plmrFile = Path.Combine(Path.GetDirectoryName(MainForm.Instance.FilePath), DummyModelFile);
            if (File.Exists(plcoFile) && File.Exists(plmrFile))
            {
                ThrowDummyManager.SetJOBJ(new HSDRawFile(plmrFile).Roots[0].Data as HSD_JOBJ);

                ThrowDummyManager._settings.RenderBones = false;

                ThrowDummyManager.DOBJManager.OverlayColor = new Vector3(1.5f, 1.5f, 1.5f);

                // load bone lookup table
                var plco = new HSDRawFile(plcoFile).Roots[0].Data as SBM_ftLoadCommonData;
                var lookuptable = plco.BoneTables[DummyModelExternalId]._s.GetReference<HSDByteArray>(0x04);
                ThrowDummyLookupTable.Clear();
                for (int i = 0; i < 54; i++)
                {
                    var bone = lookuptable[i];
                    if (bone != 255 && !ThrowDummyLookupTable.ContainsKey(bone))
                        ThrowDummyLookupTable.Add(bone, i);
                }
            }
            else*/
            {
                ThrowDummyManager.SetJOBJ(DummyThrowModel.GenerateThrowDummy());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelPath"></param>
        private void LoadModel(string modelPath)
        {
            // load model
            var modelFile = new HSDRawFile(modelPath);
            if (modelFile.Roots.Count > 0 && modelFile.Roots[0].Data is HSD_JOBJ jobj)
            {
                JointManager.SetJOBJ(jobj);

                // load material animation if it exists
                if (modelFile.Roots.Count > 1 && modelFile.Roots[1].Data is HSD_MatAnimJoint matanim)
                {
                    JointManager.SetMatAnimJoint(matanim);
                    JointManager.EnableMaterialFrame = true;
                }
            }
            else
                return;
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadDemoAnimationFiles()
        {
            // attempt to automatically locate files
            var modelFile = MainForm.Instance.FilePath.Replace(".dat", "Nr.dat");

            var path = Path.GetDirectoryName(MainForm.Instance.FilePath);
            var fighterKey = Path.GetFileNameWithoutExtension(MainForm.Instance.FilePath).Replace("Pl", "");
            var fighterName = _node.Parent.Text.Replace("ftData", "");

            ResultFilePath = Path.Combine(path, $"GmRstM{fighterKey}.dat");
            WaitFilePath = Path.Combine(path, $"Pl{fighterKey}DViWaitAJ.dat");
            IntroFilePath = Path.Combine(path, $"ftDemoIntroMotionFile{fighterName}.dat");
            EndingFilePath = Path.Combine(path, $"ftDemoEndingMotionFile{fighterName}.dat");

            if (!File.Exists(modelFile))
                modelFile = FileIO.OpenFile("Fighter Model (Pl**Nr.dat)|*.dat", $"Pl{fighterKey}Nr.dat");

            if (string.IsNullOrEmpty(modelFile))
                return;

            if (!File.Exists(ResultFilePath))
                ResultFilePath = FileIO.OpenFile("Fighter Result Anim (GmRstM**.dat)|*.dat", $"GmRstM{fighterKey}.dat");

            if (!File.Exists(WaitFilePath))
                WaitFilePath = FileIO.OpenFile("Fighter Wait Anim (Pl**DViWaitAJ.dat)|*.dat", $"Pl{fighterKey}DViWaitAJ.dat");

            if (!File.Exists(IntroFilePath))
                IntroFilePath = FileIO.OpenFile("Fighter Intro Anim Bank (ftDemoIntroMotionFile**.dat)|*.dat", $"ftDemoIntroMotionFile{fighterName}.dat");

            if (!File.Exists(EndingFilePath))
                EndingFilePath = FileIO.OpenFile("Fighter Ending Anim Bank (ftDemoEndingMotionFile**.dat)|*.dat", $"ftDemoEndingMotionFile{fighterName}.dat");

            // load animation data
            AJManager = new FighterAJManager();

            ResultSymbol = AJManager.ScanAJFile(ResultFilePath);
            WaitSymbol = AJManager.ScanAJFile(WaitFilePath);
            IntroSymbol = AJManager.ScanAJFile(IntroFilePath);
            EndingSymbol = AJManager.ScanAJFile(EndingFilePath);

            MessageBox.Show($"Loaded:\nResultBank: {ResultFilePath}\nWaitBank: {WaitFilePath}\nIntroBank: {IntroFilePath}\nEndingBank: {EndingFilePath}");

            // load model
            LoadModel(modelFile);

            // shared rendering init
            InitRendering();
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveDemoAnimationFiles()
        {
            //private byte[] ResultFile; 0-9
            //private byte[] IntroFile; 10-11
            //private byte[] EndingFile; 12
            //private byte[] WaitFile; 13
            // 14 and 15 are mario and luigi exclusive

            BuildDemoAJFile(ResultSymbol, ResultFilePath, 0, 9);
            BuildDemoAJFile(IntroSymbol, IntroFilePath, 10, 11);
            BuildDemoAJFile(EndingSymbol, EndingFilePath, 12, 12);
            BuildDemoAJFile(WaitSymbol, WaitFilePath, 13, 13);
        }

        /// <summary>
        /// 
        /// </summary>
        private bool BuildDemoAJFile(string symbol, string ajpath, int actionstart, int actionend)
        {
            if (string.IsNullOrEmpty(symbol))
                return false;

            // get actions
            var actions = new Action[actionend - actionstart + 1];
            for (int i = actionstart; i <= actionend; i++)
                actions[i - actionstart] = AllActions[i];

            // rebuild aj file
            var data = AJManager.RebuildAJFile(actions.Select(e => e.Symbol).ToArray(), false);

            // update animation offset and sizes
            foreach (var a in actions)
            {
                // don't write subroutines
                if (a.Subroutine)
                    continue;

                // update animation size and offset
                if (!string.IsNullOrEmpty(a.Symbol))
                {
                    var offsize = AJManager.GetSizeOffset(a.Symbol);
                    a.AnimOffset = offsize.Item1;
                    a.AnimSize = offsize.Item2;
                }
            }

            // save action changes to dat file
            SaveAllActionChanges();

            // save aj file
            HSDRawFile file = new HSDRawFile();
            file.Roots.Add(new HSDRootNode() { Name = symbol, Data = new HSDAccessor() { _s = new HSDStruct(data) } });
            file.Save(ajpath);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadFighterAnimationFiles()
        {
            var aFile = MainForm.Instance.FilePath.Replace(".dat", "AJ.dat");
            var cFile = MainForm.Instance.FilePath.Replace(".dat", "Nr.dat");

            // try to automatically locate files
            bool openFiles = true;
            if (File.Exists(aFile) && File.Exists(cFile))
            {
                var r = MessageBox.Show($"Load {System.IO.Path.GetFileName(aFile)} and {System.IO.Path.GetFileName(cFile)}", "Open Files", MessageBoxButtons.YesNoCancel);

                if (r == DialogResult.Cancel)
                    return;

                if (r == DialogResult.Yes)
                    openFiles = false;
            }

            // find files to open
            if (openFiles)
            {
                cFile = FileIO.OpenFile("Fighter Costume (Pl**Nr.dat)|*.dat");
                if (cFile == null)
                    return;
                aFile = FileIO.OpenFile("Fighter Animation (Pl**AJ.dat)|*.dat");
                if (aFile == null)
                    return;
            }

            // load animation data
            AJFilePath = aFile;
            AJManager = new FighterAJManager(File.ReadAllBytes(aFile));

            // load model
            LoadModel(cFile);

            // shared rendering init
            InitRendering();
        }


        /// <summary>
        /// 
        /// </summary>
        private void SaveFighterAnimationFile()
        {
            // rendering files not loaded
            if (string.IsNullOrEmpty(AJFilePath))
                return;

            // collect used symbols from all actions
            var usedSymbols = AllActions.Select(e => e.Symbol).ToArray();

            // generate new aj file
            var newAJFile = AJManager.RebuildAJFile(usedSymbols, false);

            // update animation offset and sizes
            foreach (var a in AllActions)
            {
                // don't write subroutines
                if (a.Subroutine)
                    continue;

                // update animation size and offset
                if (!string.IsNullOrEmpty(a.Symbol))
                {
                    var offsize = AJManager.GetSizeOffset(a.Symbol);
                    a.AnimOffset = offsize.Item1;
                    a.AnimSize = offsize.Item2;
                }
            }

            // save action changes to dat file
            SaveAllActionChanges();

            // dump to file
            File.WriteAllBytes(AJFilePath, newAJFile);
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadModelParts()
        {
            var plDat = FighterData;

            if (plDat != null && plDat.ModelPartAnimations != null && JointManager.JointCount != 0)
            {
                ModelPartsIndices = plDat.ModelPartAnimations.Array.Select(
                    e => new ModelPartAnimations(e)
                ).ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResetModelVis()
        {
            JointManager.MatAnimation.SetAllFrames(0);

            var ftData = FighterData;

            // lookup table
            if (ftData != null && ftData.ModelLookupTables != null && JointManager.JointCount != 0)
            {
                JointManager.DOBJManager.HiddenDOBJs.Clear();

                // only show struct 0 vis
                for (int i = 0; i < ftData.ModelLookupTables.CostumeVisibilityLookups[0].HighPoly.Length; i++)
                    SetModelVis(i, 0);

                // hide low poly
                foreach (var lut in ftData.ModelLookupTables.CostumeVisibilityLookups[0].LowPoly.Array)
                    SetModelVis(lut, -1);
            }

            // reset model parts
            if (ModelPartsIndices != null)
            {
                for (int i = 0; i < ModelPartsIndices.Length; i++)
                    ModelPartsIndices[i].AnimIndex = -1;

                JointManager.Animation.FrameModifier.Clear();
                JointManager.Animation.FrameModifier.AddRange(ModelPartsIndices);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="structid"></param>
        /// <param name="objectid"></param>
        private void SetModelVis(int structid, int objectid)
        {
            var plDat = FighterData;

            if (plDat.ModelLookupTables != null && JointManager.JointCount != 0)
                SetModelVis(plDat.ModelLookupTables.CostumeVisibilityLookups[0].HighPoly[structid], objectid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="structid"></param>
        /// <param name="objectid"></param>
        private void SetModelVis(SBM_LookupTable lookuptable, int objectid)
        {
            var plDat = FighterData;

            if (plDat.ModelLookupTables != null && JointManager.JointCount != 0 && lookuptable.LookupEntries != null)
            {
                var structs = lookuptable.LookupEntries.Array;

                for (int i = 0; i < structs.Length; i++)
                {
                    foreach (var v in structs[i].Entries)
                        if (i == objectid)
                            JointManager.ShowDOBJ(v);
                        else
                            JointManager.HideDOBJ(v);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="frame"></param>
        private void AnimateMaterial(int index, int frame, int matflag, int frameflag)
        {
            var plDat = FighterData;

            if (plDat != null && plDat.ModelLookupTables != null && index < plDat.ModelLookupTables.CostumeMaterialLookups[0].Entries.Length)
            {
                if (matflag == 1)
                {
                    foreach (var v in plDat.ModelLookupTables.CostumeMaterialLookups[0].Entries.Array)
                        JointManager.MatAnimation.SetFrame(v, frame);
                }
                else
                {
                    var idx = plDat.ModelLookupTables.CostumeMaterialLookups[0].Entries[index];
                    JointManager.MatAnimation.SetFrame(idx, frame);
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
            if (ModelPartsIndices != null && part_index < ModelPartsIndices.Length && part_index >= 0)
                ModelPartsIndices[part_index].AnimIndex = anim_index;
        }

        /// <summary>
        /// Calcuates the previous state hitboxes positions and returns them as a dictionary
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, Vector3> CalculatePreviousState()
        {
            if (viewport.Frame == 0 || !interpolationToolStripMenuItem.Checked)
                return null;

            Dictionary<int, Vector3> previousPosition = new Dictionary<int, Vector3>();

            JointManager.Frame = viewport.Frame - 1;
            JointManager.UpdateNoRender();
            SubactionProcess.SetFrame(viewport.Frame - 1);

            foreach (var hb in SubactionProcess.Hitboxes)
            {
                var boneID = hb.BoneID;
                if (boneID == 0)
                    boneID = 1;
                var transform = Matrix4.CreateTranslation(hb.Point1) * JointManager.GetWorldTransform(boneID);
                transform = transform.ClearScale();
                var pos = Vector3.TransformPosition(Vector3.Zero, transform);
                previousPosition.Add(hb.ID, pos);
            }

            return previousPosition;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            // store previous hitbox state info
            Dictionary<int, Vector3> previousPosition = CalculatePreviousState();

            // reset model parts
            if (ModelPartsIndices != null)
                for (int i = 0; i < ModelPartsIndices.Length; i++)
                    ModelPartsIndices[i].AnimIndex = -1;

            // process ftcmd
            SubactionProcess.SetFrame(viewport.Frame);

            // update display info
            JointManager.DOBJManager.OverlayColor = SubactionProcess.OverlayColor;
            JointManager._settings.RenderBones = bonesToolStripMenuItem.Checked;

            // apply model animations
            JointManager.Frame = viewport.Frame;
            JointManager.UpdateNoRender();

            // character invisibility
            if (!SubactionProcess.CharacterInvisibility && modelToolStripMenuItem.Checked)
                JointManager.Render(cam, false);

            // hurtbox collision
            if (hurtboxesToolStripMenuItem.Checked)
                HurtboxRenderer.Render(JointManager, Hurtboxes, null, SubactionProcess.BoneCollisionStates, SubactionProcess.BodyCollisionState);

            // hitbox collision
            foreach (var hb in SubactionProcess.Hitboxes)
            {
                var boneID = hb.BoneID;
                if (boneID == 0)
                    if (JointManager.GetJOBJ(1).Child == null) // special case for character like mewtwo with a leading bone
                        boneID = 2;
                    else
                        boneID = 1;

                var transform = Matrix4.CreateTranslation(hb.Point1) * JointManager.GetWorldTransform(boneID);

                transform = transform.ClearScale();

                float alpha = 0.4f;
                Vector3 hbColor = HitboxColor;

                if (hb.Element == 8)
                    hbColor = GrabboxColor;

                // drawing a capsule takes more processing power, so only draw it if necessary
                if (interpolationToolStripMenuItem.Checked && previousPosition != null && previousPosition.ContainsKey(hb.ID))
                {
                    var pos = Vector3.TransformPosition(Vector3.Zero, transform);
                    var cap = new Capsule(pos, previousPosition[hb.ID], hb.Size);
                    cap.Draw(Matrix4.Identity, new Vector4(hbColor, alpha));
                }
                else
                {
                    DrawShape.DrawSphere(transform, hb.Size, 16, 16, hbColor, alpha);
                }

                // draw hitbox angle
                if (hitboxInfoToolStripMenuItem.Checked)
                {
                    if (hb.Angle != 361)
                        DrawShape.DrawAngleLine(cam, transform, hb.Size, MathHelper.DegreesToRadians(hb.Angle));
                    else
                        DrawShape.DrawSakuraiAngle(cam, transform, hb.Size);
                    GLTextRenderer.RenderText(cam, hb.ID.ToString(), transform, StringAlignment.Center, true);
                }
            }

            // draw shield during guard animation
            if (DisplayShieldSize > 0)
                DrawShape.DrawSphere(JointManager.GetWorldTransform(JointManager.JointCount - 2), DisplayShieldSize, 16, 16, ShieldColor, 0.5f);

            // gfx spawn indicator
            foreach (var gfx in SubactionProcess.GFXOnFrame)
            {
                var boneID = gfx.Bone;
                if (boneID == 0)
                    if (JointManager.GetJOBJ(1).Child == null) // special case for character like mewtwo with a leading bone
                        boneID = 2;
                    else
                        boneID = 1;
                var transform = Matrix4.CreateTranslation(gfx.Position) * JointManager.GetWorldTransform(boneID);
                transform = transform.ClearScale();

                DrawShape.DrawSphere(transform, 1f, 16, 16, ThrowDummyColor, 0.5f);
            }

            // environment collision
            if (ECB != null)
            {
                var topN = JointManager.GetWorldTransform(1).ExtractTranslation();

                var bone1 = Vector3.TransformPosition(Vector3.Zero, JointManager.GetWorldTransform(ECB.ECBBone1));
                var bone2 = Vector3.TransformPosition(Vector3.Zero, JointManager.GetWorldTransform(ECB.ECBBone2));
                var bone3 = Vector3.TransformPosition(Vector3.Zero, JointManager.GetWorldTransform(ECB.ECBBone3));
                var bone4 = Vector3.TransformPosition(Vector3.Zero, JointManager.GetWorldTransform(ECB.ECBBone4));
                var bone5 = Vector3.TransformPosition(Vector3.Zero, JointManager.GetWorldTransform(ECB.ECBBone5));
                var bone6 = Vector3.TransformPosition(Vector3.Zero, JointManager.GetWorldTransform(ECB.ECBBone6));

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
                if (eCBToolStripMenuItem.Checked)
                {
                    DrawShape.DrawECB(topN, minx, miny, maxx, maxy, groundECH.Checked);
                }

                // ledge grav
                if (ledgeGrabBoxToolStripMenuItem.Checked)
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

            // throw dummy
            if (ThrowDummyManager.Animation.NodeCount != 0 && 
                throwModelToolStripMenuItem.Checked && 
                !SubactionProcess.ThrownFighter && 
                ThrowDummyManager.JointCount > 0)
            {
                if (viewport.Frame < ThrowDummyManager.Animation.FrameCount)
                    ThrowDummyManager.Frame = viewport.Frame;

                ThrowDummyManager.SetWorldTransform(4, JointManager.GetWorldTransform(JointManager.JointCount - 2));
                ThrowDummyManager.Render(cam, false);

                if (ThrowDummyLookupTable.Count == 0)
                {
                    DrawShape.DrawSphere(ThrowDummyManager.GetWorldTransform(35), 1.5f, 16, 16, ThrowDummyColor, 0.5f);
                    DrawShape.DrawSphere(ThrowDummyManager.GetWorldTransform(4), 1.5f, 16, 16, ThrowDummyColor, 0.5f);
                    DrawShape.DrawSphere(ThrowDummyManager.GetWorldTransform(10), 1f, 16, 16, ThrowDummyColor, 0.5f);
                    DrawShape.DrawSphere(ThrowDummyManager.GetWorldTransform(15), 1f, 16, 16, ThrowDummyColor, 0.5f);
                    DrawShape.DrawSphere(ThrowDummyManager.GetWorldTransform(22), 1f, 16, 16, ThrowDummyColor, 0.5f);
                    DrawShape.DrawSphere(ThrowDummyManager.GetWorldTransform(40), 1f, 16, 16, ThrowDummyColor, 0.5f);
                }
            }

            // sword trail
            //AfterImageRenderer.RenderAfterImage(JointManager, viewport.Frame, after_desc);
        }
        
        private static AfterImageDesc after_desc = new AfterImageDesc()
        {
            Bone = 75,
            Bottom = 1.75f,
            Top = 9.63f,
            Color1 = new Vector3(1),
            Color2 = new Vector3()
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        private void LoadAnimation(string symbol)
        {
            // clear animation
            JointManager.SetFigaTree(null);
            ThrowDummyManager.SetFigaTree(null);
            DisplayShieldSize = 0;

            // check if animations are loaded
            if (AJManager == null)
                return;

            // check if animation exists
            var animData = AJManager.GetAnimationData(symbol);
            if (animData == null)
                return;

            // load animation
            var anim = new HSDRawFile(animData);
            if (anim.Roots[0].Data is HSD_FigaTree tree)
            {
                var name = new Action() { Symbol = anim.Roots[0].Name }.ToString();

                // load figatree to manager and anim editor
                JointManager.SetFigaTree(tree);
                _animEditor.SetJoint(JointManager.GetJOBJ(0), JointManager.Animation);

                // set frame
                viewport.MaxFrame = tree.FrameCount;


                // enable shield display
                if (FighterData != null && name.Equals("Guard"))
                    DisplayShieldSize = FighterData.Attributes.ShieldSize / 2;


                // load throw dummy for thrown animations
                if (name.Contains("Throw") && !name.Contains("Taro"))
                {
                    // find thrown anim
                    Action throwAction = null;
                    foreach (Action a in actionList.Items)
                    {
                        if (a.Symbol != null &&
                            a.Symbol.Contains("Taro") &&
                            a.Symbol.Contains(name) &&
                            !a.Symbol.Equals(anim.Roots[0].Name))
                        {
                            throwAction = a;
                            break;
                        }
                    }

                    // if throw animation is found
                    if (throwAction != null &&
                        throwAction.Symbol != null)
                    {
                        var throwData = AJManager.GetAnimationData(throwAction.Symbol);

                        if (throwData != null)
                        {
                            // load throw animation
                            var tanim = new HSDRawFile(throwData);
                            if (tanim.Roots[0].Data is HSD_FigaTree tree2)
                            {
                                ThrowDummyManager.SetFigaTree(tree2);
                                if (ThrowDummyLookupTable.Count > 0)
                                {
                                    ThrowDummyManager.Animation.EnableBoneLookup = true;
                                    ThrowDummyManager.Animation.BoneLookup = ThrowDummyLookupTable;
                                }
                            }
                        }
                    }

                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void SaveAnimationChanges()
        {
            if (actionList.SelectedItem is Action action)
            {
                HSDRawFile f = new HSDRawFile();

                f.Roots.Add(new HSDRootNode()
                {
                    Name = action.Symbol,
                    Data = JointManager.Animation.ToFigaTree()
                });

                using (MemoryStream stream = new MemoryStream())
                {
                    f.Save(stream);
                    AJManager.SetAnimation(action.Symbol, stream.ToArray());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importFigatreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (actionList.SelectedItem is Action a)
            {
                var f = FileIO.OpenFile("Supported Formats |*.dat;*.anim");

                HSDRawFile file;

                // if it's a maya anim then convert to figatree and set the symbol
                if (f.ToLower().EndsWith(".anim"))
                {
                    var anim = Converters.ConvMayaAnim.ImportFromMayaAnim(f, null);

                    file = new HSDRawFile(f);
                    file.Roots.Add(new HSDRootNode()
                    {
                        Name = a.Symbol,
                        Data = anim.ToFigaTree(0.01f)
                    });
                }
                else
                    // just load dat normally
                    try
                    {
                        file = new HSDRawFile(f);
                    }
                    catch
                    {
                        return;
                    }

                // check if figatree data is found
                if (file == null || file.Roots.Count > 0 && file.Roots[0].Data is HSD_FigaTree tree)
                {
                    //grab symbol
                    var symbol = file.Roots[0].Name;

                    //check if symbol exists and ok to overwrite
                    if (AJManager.GetAnimationData(symbol) != null)
                    {
                        if (MessageBox.Show($"Symbol \"{symbol}\" already exists.\nIs it okay to overwrite?", "Overwrite Symbol", MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                            return;
                    }

                    // set animation data
                    using (MemoryStream stream = new MemoryStream())
                    {
                        file.Save(stream);
                        AJManager.SetAnimation(symbol, stream.ToArray());
                    }

                    // set action symbol
                    a.Symbol = symbol;

                    // reselect action
                    LoadAnimation(symbol);

                    // 
                    actionList.Invalidate();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void figatreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (actionList.SelectedItem is Action a)
            {
                var figaData = AJManager.GetAnimationData(a.Symbol);

                if (a.Symbol != null && figaData != null)
                {
                    var f = FileIO.SaveFile(ApplicationSettings.HSDFileFilter, a.Symbol + ".dat");

                    if (f != null)
                        File.WriteAllBytes(f, figaData);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void popoutEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _animEditor.Show();
            _animEditor.Visible = true;
        }
    }
}
