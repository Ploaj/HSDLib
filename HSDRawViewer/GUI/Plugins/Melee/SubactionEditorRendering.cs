using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Melee;
using HSDRaw.Melee.Pl;
using HSDRaw.Tools;
using HSDRaw.Tools.Melee;
using HSDRawViewer.Converters.Animation;
using HSDRawViewer.GUI.Extra;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Rendering.Renderers;
using HSDRawViewer.Rendering.Shapes;
using HSDRawViewer.Rendering.Widgets;
using HSDRawViewer.Tools;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;

namespace HSDRawViewer.GUI.Plugins.Melee
{
    public partial class SubactionEditor : IDrawableInterface
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

            public bool OverrideAnim(LiveJObj jobj, float frame)
            {
                int boneIndex = jobj.Index;

                // check if bone index is in entries
                if (AnimIndex == -1 || boneIndex < StartBone || boneIndex >= StartBone + Anims[0].NodeCount)
                    return false;

                // get anim for entry
                foreach (var e in Entries)
                    if (e == boneIndex && AnimIndex < Anims.Length)
                    {
                        var anim = Anims[AnimIndex];
                        if (anim.Nodes[boneIndex - StartBone].Tracks.Count > 0)
                            anim.ApplyAnimation(jobj, 0);
                            // anim.GetAnimatedState(0, boneIndex - StartBone, jobj, out TX, out TY, out TZ, out RX, out RY, out RZ, out SX, out SY, out SZ);
                        return true;
                    }

                return false;
            }
        }

        private ViewportControl viewport;

        private PopoutJointAnimationEditor _animEditor = new PopoutJointAnimationEditor(false);

        private JOBJManager JointManager = new JOBJManager();
        private JOBJManager ThrowDummyManager = new JOBJManager();

        private JOBJManager ItemJointManager = new JOBJManager();
        private HSDRawFile ItCo;

        private Dictionary<int, int> ThrowDummyLookupTable = new Dictionary<int, int>();

        private SBM_EnvironmentCollision ECB = null;


        private Vector3 IrOffset = Vector3.Zero;
        private static readonly Vector3 IrLeftOffset = new Vector3(-9, -10.1f, 0);
        private static readonly Vector3 IrRightOffset = new Vector3(9, -10.1f, 0);
        private SBM_gmIntroEasyTable easyTable = new SBM_gmIntroEasyTable();


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
        private static Vector3 HitboxSelectedColor = new Vector3(1, 1, 1);
        private float ModelScale = 1f;
        private float ItemScale = 1f;

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

        private JointAnimManager BackupAnim;

        public List<FrameSpeedMultiplier> FrameSpeedModifiers { get; set; } = new List<FrameSpeedMultiplier>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fSMApplyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*var mult = new FrameSpeedModifierSettings();

            using (PropertyDialog d = new PropertyDialog("Frame Speed Multipler Settings", mult))
                if (d.ShowDialog() == DialogResult.OK)
                {
                    JointManager.Animation.ApplyFSMs(mult.Modifiers);
                    viewport.MaxFrame = JointManager.Animation.FrameCount;
                }*/
        }


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
            // clear hidden dobjs
            JointManager.ShowAllDOBJs();

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
            IntroFilePath = Path.Combine(path, $"ftDemoIntro{fighterName}.dat");
            EndingFilePath = Path.Combine(path, $"ftDemoEnding{fighterName}.dat");

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

            // load easy table
            if (File.Exists(IntroFilePath))
            {
                var f = new HSDRawFile(IntroFilePath);
                if (f["gmIntroEasyTable"] != null)
                    easyTable = f["gmIntroEasyTable"].Data as SBM_gmIntroEasyTable;
            }

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
                    var offsize = AJManager.GetOffsetSize(a.Symbol);
                    a.AnimOffset = offsize.Item1;
                    a.AnimSize = offsize.Item2;
                }
            }

            // save action changes to dat file
            SaveAllActionChanges();

            // load or create file
            HSDRawFile file = null;
            if (File.Exists(ajpath))
                file = new HSDRawFile(ajpath);
            else
                file = new HSDRawFile();

            // update or add aj data
            var dataAccessor = new HSDAccessor() { _s = new HSDStruct(data) };
            if (file[symbol] != null)
                file[symbol].Data = dataAccessor;
            else
                file.Roots.Add(new HSDRootNode() { Name = symbol, Data = dataAccessor });

            // update or add easy table
            if (symbol == IntroSymbol)
            {
                if (file["gmIntroEasyTable"] != null)
                    file["gmIntroEasyTable"].Data = easyTable;
                else
                    file.Roots.Add(new HSDRootNode() { Name = "gmIntroEasyTable", Data = easyTable });
            }

            // save file
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

            var itcoPath = Path.GetDirectoryName(MainForm.Instance.FilePath) + "\\ItCo.dat";
            if (File.Exists(itcoPath))
                ItCo = new HSDRawFile(itcoPath);

            // load model
            LoadModel(cFile);

            // shared rendering init
            InitRendering();
        }

        private bool IsSaving = false;


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
                    var offsize = AJManager.GetOffsetSize(a.Symbol);
                    a.AnimOffset = offsize.Item1;
                    a.AnimSize = offsize.Item2;
                }
            }

            // save action changes to dat file
            IsSaving = true;
            SaveAllActionChanges();
            IsSaving = false;

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
                JointManager.ShowAllDOBJs();

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

            if (plDat != null && plDat.ModelLookupTables != null && index < plDat.ModelLookupTables.MaterialLookupLength)
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
        private void CalculatePreviousState()
        {
            if (viewport.Frame == 0 || !interpolationToolStripMenuItem.Checked)
                return;

            JointManager.Frame = viewport.Frame - 1;
            JointManager.UpdateNoRender();
            SubactionProcess.SetFrame(viewport.Frame - 1);

            int hitboxId = 0;
            foreach (var hb in SubactionProcess.Hitboxes)
            {
                if (hb.Active)
                    PreviousPositions[hitboxId] = hb.GetWorldPosition(JointManager);

                hitboxId++;
            }
        }

        private Vector3[] PreviousPositions = new Vector3[4];
        private Capsule capsule = new Capsule(Vector3.Zero, Vector3.Zero, 0);

        private TranslationWidget _transWidget = new TranslationWidget();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            // if model not loaded do nothing
            if (JointManager == null)
                return;

            // store previous hitbox state info
            CalculatePreviousState();

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

            // versus mode preview
            if (introDropDownButton.Visible && enablePreviewToolStripMenuItem.Checked)
            {
                // 3.8, 1, 7
                JointManager.SetWorldTransform(0,
                    Matrix4.CreateScale(easyTable.XScale, easyTable.YScale, easyTable.ZScale) *
                    Matrix4.CreateTranslation(IrOffset) *
                    Matrix4.CreateTranslation(easyTable.XOffset, easyTable.YOffset, 0));
                //cam.FovDegrees = 20;
                //cam.Translation = new Vector3(0, 0, -62);
                cam.RotationXDegrees = 0;
                cam.RotationYDegrees = 0;
                //cam.Mode = CameraMode.Orthogonal;
                cam.Translation = new Vector3(0, 0, -1);
                //JointManager._lightParam.UseCameraLight = false;
                //JointManager._lightParam.LightX = 1;
                //JointManager._lightParam.LightY = 1;
                //JointManager._lightParam.LightZ = 6;
            }
            else
            {
                //if (cam.Mode == CameraMode.Orthogonal)
                //    cam.Mode = CameraMode.Persepective;
                //JointManager._lightParam.UseCameraLight = true;
            }

            // scale model
            JointManager.SetWorldTransform(0, Matrix4.CreateScale(ModelScale));

            // render character model
            if (!SubactionProcess.CharacterInvisibility && modelToolStripMenuItem.Checked)
                JointManager.Render(cam, false);

            // render item model
            if (ItemJointManager != null && FighterData != null)
            {
                ItemJointManager.SetWorldTransform(1, Matrix4.CreateScale(ItemScale) * JointManager.GetWorldTransform(FighterData.ModelLookupTables.ItemHoldBone));
                ItemJointManager.Render(cam, false);
            }

            // hurtbox collision
            if (hurtboxesToolStripMenuItem.Checked)
                HurtboxRenderer.Render(JointManager, Hurtboxes, null, SubactionProcess.BoneCollisionStates, SubactionProcess.BodyCollisionState);

            // hitbox collision
            int hitboxId = 0;
            bool isHitboxSelected = false;
            foreach (var hb in SubactionProcess.Hitboxes)
            {
                if (!hb.Active)
                {
                    hitboxId++;
                    continue;
                }

                float alpha = 0.4f;
                Vector3 hbColor = HitboxColor;

                var worldPosition = hb.GetWorldPosition(JointManager);
                var worldTransform = Matrix4.CreateTranslation(worldPosition);

                if (hb.Element == 8)
                    hbColor = GrabboxColor;

                if (subActionList.SelectedIndices.Count == 1 && hb.CommandIndex == subActionList.SelectedIndex)
                {
                    hbColor = HitboxSelectedColor;
                    isHitboxSelected = true;
                    _transWidget.Transform = hb.GetWorldTransform(JointManager);
                }

                // drawing a capsule takes more processing power, so only draw it if necessary
                if (hb.Interpolate &&
                    interpolationToolStripMenuItem.Checked)
                {
                    capsule.SetParameters(worldPosition, PreviousPositions[hitboxId], hb.Size);
                    capsule.Draw(Matrix4.Identity, new Vector4(hbColor, alpha));
                }
                else
                {
                    DrawShape.DrawSphere(worldTransform, hb.Size, 16, 16, hbColor, alpha);
                }

                // draw hitbox angle
                if (hitboxInfoToolStripMenuItem.Checked)
                {
                    if (hb.Angle != 361)
                        DrawShape.DrawAngleLine(cam, worldTransform, hb.Size, MathHelper.DegreesToRadians(hb.Angle));
                    else
                        DrawShape.DrawSakuraiAngle(cam, worldTransform, hb.Size);

                    GLTextRenderer.RenderText(cam, hitboxId.ToString(), worldTransform, StringAlignment.Center, true);
                }
                hitboxId++;
            }

            // draw shield during guard animation
            if (DisplayShieldSize > 0)
                DrawShape.DrawSphere(JointManager.GetWorldTransform(JointManager.JointCount - 2), DisplayShieldSize, 16, 16, ShieldColor, 0.5f);


            // gfx spawn indicator
            foreach (var gfx in SubactionProcess.GFXOnFrame)
            {
                var boneID = gfx.Bone;
                if (boneID == 0)
                    if (JointManager.GetJOBJ(1) != null && JointManager.GetJOBJ(1).Child == null) // special case for character like mewtwo with a leading bone
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

                // ledge grab
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

            if (isHitboxSelected)
                _transWidget.Render(cam);

            // sword trail
            //AfterImageRenderer.RenderAfterImage(JointManager, viewport.Frame, after_desc);

            // draw irvs preview border
            if (introDropDownButton.Visible && enablePreviewToolStripMenuItem.Checked)
            {
                DrawShape.DrawRectangle(5.6f, 4.75f, -5.6f, 1.8f, 40, 0, Color.Black);
                DrawShape.DrawRectangle(5.6f, -1.8f, -5.6f, -4.75f, 40, 0, Color.Black);
            }
        }

        //private static AfterImageDesc after_desc = new AfterImageDesc()
        //{
        //    Bone = 57,//27,
        //    Bottom = 0,
        //    Top = 4,
        //    Color1 = new Vector3(1, 1, 1),
        //    Color2 = new Vector3(0, 1, 1)
        //};
        //private static AfterImageDesc after_desc = new AfterImageDesc()
        //{
        //    Bone = 75,
        //    Bottom = 1.75f,
        //    Top = 9.63f,
        //    Color1 = new Vector3(1, 1, 1),
        //    Color2 = new Vector3(0, 1, 1)
        //};

        private void LoadItemModel(int id)
        {
            if (ItCo != null)
            {
                if (ItCo["itPublicData"]?.Data is itPublicData it)
                {
                    var item = it.Items.Articles[id];

                    if (item.Model.RootModelJoint != null && 
                        ItemJointManager.GetJOBJ(0)?.Desc != item.Model.RootModelJoint)
                    {
                        ItemJointManager.SetJOBJ(item.Model.RootModelJoint);
                        ItemScale = item.Parameters.ModelScale;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        private void LoadAnimation(string symbol)
        {
            // reset display sheild size
            DisplayShieldSize = 0;

            // clear item model
            ItemJointManager.SetJOBJ(null);

            // load attributes
            if (!string.IsNullOrEmpty(symbol))
            {
                if (FighterData != null && symbol.Contains("Guard"))
                    DisplayShieldSize = FighterData.Attributes.ShieldSize / 2;

                if (FighterData != null)
                {
                    // LoadItemModel(12);
                    if (symbol.Contains("ItemParasol"))
                        LoadItemModel(13);
                    if (symbol.Contains("ItemShoot"))
                        LoadItemModel(16);
                    if (symbol.Contains("ItemScope"))
                        LoadItemModel(21);
                    if (symbol.Contains("ItemHammer"))
                        LoadItemModel(28);
                }
            }

            // check to render irv
            introDropDownButton.Visible = false;
            viewport.DisplayGrid = true;

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
                LoadFigaTree(anim.Roots[0].Name, tree);
            }

            // enable intro view
            if (symbol.Contains("IntroL_figatree"))
            {
                introDropDownButton.Visible = true;
                IrOffset = IrLeftOffset;
                viewport.DisplayGrid = false;
            }
            if (symbol.Contains("IntroR_figatree"))
            {
                introDropDownButton.Visible = true;
                IrOffset = IrRightOffset;
                viewport.DisplayGrid = false;
            }
            
            // apply fsms
            if (FrameSpeedModifiers.Count > 0)
                UpdateAnimationWithFSMs();
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadFigaTree(string treeSymbol, HSD_FigaTree tree)
        {
            // clear animation
            JointManager.SetFigaTree(null);
            ThrowDummyManager.SetFigaTree(null);

            // create new action
            var name = new Action() { Symbol = treeSymbol }.ToString();

            // load figatree to manager and anim editor
            JointManager.SetFigaTree(tree);
            _animEditor.SetJoint(JointManager.GetJOBJ(0).Desc, JointManager.Animation);

            // set backup anim
            BackupAnim = new JointAnimManager();
            BackupAnim.FromFigaTree(tree);

            // set frame
            viewport.MaxFrame = tree.FrameCount;

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
                        !a.Symbol.Equals(treeSymbol))
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

                BackupAnim.FromFigaTree(JointManager.Animation.ToFigaTree());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateAnimationWithFSMs()
        {
            // load backup animation
            var tempanim = new JointAnimManager();
            tempanim.FromFigaTree(BackupAnim.ToFigaTree());
            var backup = BackupAnim;

            // apply fsms to backup animation
            tempanim.ApplyFSMs(FrameSpeedModifiers);

            // load edited anim
            LoadFigaTree(SelectedAction.Symbol, tempanim.ToFigaTree());
            BackupAnim = backup;

            // refresh fsm display tips
            UpdateFrameTips();
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

        /// <summary>
        /// 
        /// </summary>
        private void UpdateFrameTips()
        {
            // clear frame tips
            viewport.FrameTips.Clear();

            // update trackbar data
            if (trackInfoToolStripMenuItem.Checked)
            {
                // add frame tips for each frame
                for (int i = 0; i <= viewport.MaxFrame; i++)
                {
                    SubactionProcess.SetFrame(i);

                    // hitbox indication
                    if (SubactionProcess.HitboxesActive)
                        viewport.FrameTips.Add(new GUI.Controls.PlaybackBarFrameTip()
                        {
                            Frame = i,
                            Color = Color.Red,
                            Style = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipStyle.Color,
                            Location = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipLocation.Lower
                        });

                    // interrupt
                    if (SubactionProcess.AllowInterrupt)
                        viewport.FrameTips.Add(new GUI.Controls.PlaybackBarFrameTip()
                        {
                            Frame = i,
                            Color = Color.Green,
                            Style = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipStyle.Color,
                            Location = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipLocation.Middle
                        });

                    // interrupt
                    if (SubactionProcess.FighterFlagWasSetThisFrame.Any(e => e != false))
                    {
                        viewport.FrameTips.Add(new GUI.Controls.PlaybackBarFrameTip()
                        {
                            Frame = i,
                            Color = Color.Purple,
                            Style = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipStyle.Color,
                            Location = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipLocation.Upper
                        });

                        string set = "";

                        for (int k = 0; k < SubactionProcess.FighterFlagWasSetThisFrame.Length; k++)
                        {
                            if (SubactionProcess.FighterFlagWasSetThisFrame[k])
                            {
                                set += k + " ";
                            }
                        }

                        viewport.FrameTips.Add(new GUI.Controls.PlaybackBarFrameTip()
                        {
                            Frame = i,
                            Color = Color.White,
                            Text = set,
                            Style = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipStyle.Text,
                            Location = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipLocation.Upper
                        });
                    }
                }
            }

            if (fsmMode.Checked)
            {
                foreach (var fsm in FrameSpeedModifiers)
                {
                    viewport.FrameTips.Add(new GUI.Controls.PlaybackBarFrameTip()
                    {
                        Frame = fsm.Frame,
                        Color = Color.Purple,
                        Style = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipStyle.Color,
                        Location = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipLocation.Upper
                    });

                    viewport.FrameTips.Add(new GUI.Controls.PlaybackBarFrameTip()
                    {
                        Frame = fsm.Frame,
                        Color = Color.White,
                        Text = fsm.Rate.ToString(),
                        Style = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipStyle.Text,
                        Location = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipLocation.Upper
                    });
                }
            }

            viewport.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kbState"></param>
        public void ViewportKeyPress(KeyboardState kbState)
        {
            if (subActionList.SelectedItem is SubActionScript script &&
                script.CodeID == 11)
            {
                var desc = script.SubactionDesc;
                var parameters = desc.GetParameters(script.data);

                //if (kbState.IsKeyDown(Keys.KeyPadAdd))
                //{
                //    // size
                //    if (kbState.IsKeyDown(Keys.ShiftLeft) || kbState.IsKeyDown(Keys.ShiftRight))
                //        parameters[6] += 100;
                //    else
                //        parameters[6] += 10;

                //    if (parameters[6] > ushort.MaxValue)
                //        parameters[6] = ushort.MaxValue;
                //}
                //if (kbState.IsKeyDown(Keys.Minus))
                //{
                //    // size
                //    if (kbState.IsKeyDown(Keys.ShiftLeft) || kbState.IsKeyDown(Keys.ShiftRight))
                //        parameters[6] -= 100;
                //    else
                //        parameters[6] -= 10;

                //    if (parameters[6] < 0)
                //        parameters[6] = 0;
                //}

                script.data = desc.Compile(parameters);

                subActionList.Invalidate(); 
                SaveSelectedActionChanges();
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
            JointManager.Frame = viewport.Frame;
            JointManager.UpdateNoRender();
            SubactionProcess.SetFrame(viewport.Frame);

            var shortestDistance = float.MaxValue;
            foreach (var hb in SubactionProcess.Hitboxes)
            {
                if (hb.Active)
                {
                    if (pick.CheckSphereHit(hb.GetWorldPosition(JointManager), hb.Size, out float distance))
                    {
                        if (distance < shortestDistance)
                        {
                            shortestDistance = distance;
                            subActionList.ClearSelected();
                            subActionList.SelectedIndex = hb.CommandIndex;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pick"></param>
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        public void ScreenDrag(PickInformation pick, float deltaX, float deltaY)
        {
            //_transWidget.Drag(pick);
            //OpenTK.Windowing.GraphicsLibraryFramework.
            //var keyState = Keyboard.GetState();

            //bool drag = keyState.IsKeyDown(Key.AltLeft) || keyState.IsKeyDown(Key.AltRight);

            //if (drag && 
            //    subActionList.SelectedItem is SubActionScript script && 
            //    script.CodeID == 11)
            //{
            //    var desc = script.SubactionDesc;
            //    var parameters = desc.GetParameters(script.data);

            //    // position
            //    // 7 8 9 

            //    script.data = desc.Compile(parameters);
            //    subActionList.Invalidate();
            //    SaveSelectedActionChanges();
            //}
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (PropertyDialog d = new PropertyDialog("Intro Position Editor", easyTable))
            {
                d.ShowDialog();
            }
        }
    }
}
