using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Melee.Pl;
using HSDRawViewer.Rendering;
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

        private JOBJManager JOBJManager = new JOBJManager();
        private JOBJManager ThrowDummyManager = new JOBJManager();

        private SBM_EnvironmentCollision ECB = null;

        public DrawOrder DrawOrder => DrawOrder.Last;

        private string AJFilePath;

        //private string ResultFilePath;
        //private string EndingFilePath;
        //private string IntroFilePath;

        private Dictionary<string, byte[]> SymbolToAnimation = new Dictionary<string, byte[]>();

        private ModelPartAnimations[] ModelPartsIndices;

        private SubactionProcessor SubactionProcess = new SubactionProcessor();

        private List<SBM_Hurtbox> Hurtboxes = new List<SBM_Hurtbox>();

        private HurtboxRenderer HurtboxRenderer = new HurtboxRenderer();
        
        private static Vector3 ThrowDummyColor = new Vector3(0, 1, 1);
        private static Vector3 HitboxColor = new Vector3(1, 0, 0);
        private static Vector3 GrabboxColor = new Vector3(1, 0, 1);
        private float ModelScale = 1f;

        /// <summary>
        /// 
        /// </summary>
        private void LoadDemoAnimationFiles()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveDemoAnimationFiles()
        {

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

            // load model
            var modelFile = new HSDRawFile(cFile);
            if (modelFile.Roots.Count > 0 && modelFile.Roots[0].Data is HSD_JOBJ jobj)
            {
                JOBJManager.SetJOBJ(jobj);

                // load material animation if it exists
                if (modelFile.Roots.Count > 1 && modelFile.Roots[1].Data is HSD_MatAnimJoint matanim)
                {
                    JOBJManager.SetMatAnimJoint(matanim);
                    JOBJManager.EnableMaterialFrame = true;
                }
            }
            else
                return;

            // set model scale
            JOBJManager.ModelScale = ModelScale;

            // clear hidden dobjs
            JOBJManager.DOBJManager.HiddenDOBJs.Clear();

            // don't render bones by default
            JOBJManager.settings.RenderBones = false;

            // reset model visibility
            ResetModelVis();

            // load the model parts
            LoadModelParts();

            // populate animation dictionary
            AJFilePath = aFile;
            SymbolToAnimation.Clear();
            using (BinaryReaderExt r = new BinaryReaderExt(new FileStream(aFile, FileMode.Open)))
                foreach (var a in AllActions)
                    if (a.Symbol != null && !SymbolToAnimation.ContainsKey(a.Symbol))
                        SymbolToAnimation.Add(a.Symbol, r.GetSection((uint)a.AnimOffset, a.AnimSize));

            // enable preview box
            previewBox.Visible = true;
            savePlayerRenderingFilesToolStripMenuItem.Enabled = true;

            // reselect action
            if (actionList.SelectedItem is Action action)
                SelectAction(action);
        }


        /// <summary>
        /// 
        /// </summary>
        private void SaveFighterAnimationFile()
        {
            // rendering files not loaded
            if (string.IsNullOrEmpty(AJFilePath))
                return;

            // make sure okay to overwrite
            if (MessageBox.Show($"Is it okay to overwrite {AJFilePath}?", "Save Animation File Changes?", MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                return;

            // collect used symbols from all actions
            var usedSymbols = AllActions.Select(e => e.Symbol);

            // generate new aj file
            Dictionary<string, Tuple<int, int>> animOffsets = new Dictionary<string, Tuple<int, int>>();

            using (MemoryStream ajBuffer = new MemoryStream())
            using (BinaryWriterExt w = new BinaryWriterExt(ajBuffer))
            {
                // collect used symbols
                foreach (var sym in usedSymbols)
                {
                    if (sym != null)
                    {
                        if (SymbolToAnimation.ContainsKey(sym) && !animOffsets.ContainsKey(sym))
                        {
                            // write animation
                            var anim = SymbolToAnimation[sym];
                            animOffsets.Add(sym, new Tuple<int, int>((int)ajBuffer.Position, anim.Length));
                            w.Write(anim);
                            w.Align(0x20, 0xFF);
                        }
                        else
                        if (!animOffsets.ContainsKey(sym))
                        {
                            // animation not found
                            MessageBox.Show($"\"{sym}\" animation not found", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            animOffsets.Add(sym, new Tuple<int, int>(0, 0));
                        }
                    }
                }

                // dump to file
                File.WriteAllBytes(AJFilePath, ajBuffer.ToArray());
            }


            int index = 0;
            foreach (var a in AllActions)
            {
                // don't write subroutines
                if (a.Subroutine)
                    continue;

                // get embedded script
                var ftcmd = new SBM_FighterCommand();
                ftcmd._s = _node.Accessor._s.GetEmbeddedStruct(0x18 * index, ftcmd.TrimmedSize);

                // update symbol name
                ftcmd.Name = a.Symbol;

                if (a.Symbol == null)
                    continue;

                // offset
                var ofst = animOffsets[a.Symbol];

                // update action offset and size
                a.AnimOffset = ofst.Item1;
                a.AnimSize = ofst.Item2;

                // update file offset and size
                ftcmd.AnimationOffset = a.AnimOffset;
                ftcmd.AnimationSize = a.AnimSize;

                // resize if needed
                if (_node.Accessor._s.Length <= 0x18 * index + 0x18)
                    _node.Accessor._s.Resize(0x18 * index + 0x18);

                // update script
                _node.Accessor._s.SetEmbededStruct(0x18 * index, ftcmd._s);
                index++;
            }

            MainForm.Instance.SaveDAT();
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadModelParts()
        {
            var plDat = FighterData;

            if (plDat != null && plDat.ModelPartAnimations != null && JOBJManager.JointCount != 0)
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
            var plDat = FighterData;

            JOBJManager.MatAnimation.SetAllFrames(0);

            if (plDat != null && plDat.ModelLookupTables != null && JOBJManager.JointCount != 0)
            {
                JOBJManager.DOBJManager.HiddenDOBJs.Clear();

                // only show struct 0 vis
                for (int i = 0; i < plDat.ModelLookupTables.CostumeVisibilityLookups[0].HighPoly.Length; i++)
                    SetModelVis(i, 0);

                // hide low poly
                foreach (var lut in plDat.ModelLookupTables.CostumeVisibilityLookups[0].LowPoly.Array)
                    SetModelVis(lut, -1);
            }

            // reset model parts
            if (ModelPartsIndices != null)
            {
                for (int i = 0; i < ModelPartsIndices.Length; i++)
                    ModelPartsIndices[i].AnimIndex = -1;

                JOBJManager.Animation.FrameModifier.Clear();
                JOBJManager.Animation.FrameModifier.AddRange(ModelPartsIndices);
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

            if (plDat.ModelLookupTables != null && JOBJManager.JointCount != 0)
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

            if (plDat.ModelLookupTables != null && JOBJManager.JointCount != 0 && lookuptable.LookupEntries != null)
            {
                var structs = lookuptable.LookupEntries.Array;

                for (int i = 0; i < structs.Length; i++)
                {
                    foreach (var v in structs[i].Entries)
                        if (i == objectid)
                            JOBJManager.ShowDOBJ(v);
                        else
                            JOBJManager.HideDOBJ(v);
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

            if (plDat.ModelLookupTables != null && index < plDat.ModelLookupTables.CostumeMaterialLookups[0].Entries.Length)
            {
                if (matflag == 1)
                {
                    foreach (var v in plDat.ModelLookupTables.CostumeMaterialLookups[0].Entries.Array)
                        JOBJManager.MatAnimation.SetFrame(v, frame);
                }
                else
                {
                    var idx = plDat.ModelLookupTables.CostumeMaterialLookups[0].Entries[index];
                    JOBJManager.MatAnimation.SetFrame(idx, frame);
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

            JOBJManager.Frame = viewport.Frame - 1;
            JOBJManager.UpdateNoRender();
            SubactionProcess.SetFrame(viewport.Frame - 1);

            foreach (var hb in SubactionProcess.Hitboxes)
            {
                var boneID = hb.BoneID;
                if (boneID == 0)
                    boneID = 1;
                var transform = Matrix4.CreateTranslation(hb.Point1) * JOBJManager.GetWorldTransform(boneID);
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
            JOBJManager.DOBJManager.OverlayColor = SubactionProcess.OverlayColor;
            JOBJManager.settings.RenderBones = bonesToolStripMenuItem.Checked;

            // apply model animations
            JOBJManager.Frame = viewport.Frame;
            JOBJManager.UpdateNoRender();

            // character invisibility
            if (!SubactionProcess.CharacterInvisibility && modelToolStripMenuItem.Checked)
                JOBJManager.Render(cam, false);

            // hurtbox collision
            if (hurtboxesToolStripMenuItem.Checked)
                HurtboxRenderer.Render(JOBJManager, Hurtboxes, null, SubactionProcess.BoneCollisionStates, SubactionProcess.BodyCollisionState);

            // hitbox collision
            foreach (var hb in SubactionProcess.Hitboxes)
            {
                var boneID = hb.BoneID;
                if (boneID == 0)
                    if (JOBJManager.GetJOBJ(1).Child == null) // special case for character like mewtwo with a leading bone
                        boneID = 2;
                    else
                        boneID = 1;

                var transform = Matrix4.CreateTranslation(hb.Point1) * JOBJManager.GetWorldTransform(boneID);

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

            // environment collision
            if (ECB != null)
            {
                var topN = JOBJManager.GetWorldTransform(1).ExtractTranslation();

                var bone1 = Vector3.TransformPosition(Vector3.Zero, JOBJManager.GetWorldTransform(ECB.ECBBone1));
                var bone2 = Vector3.TransformPosition(Vector3.Zero, JOBJManager.GetWorldTransform(ECB.ECBBone2));
                var bone3 = Vector3.TransformPosition(Vector3.Zero, JOBJManager.GetWorldTransform(ECB.ECBBone3));
                var bone4 = Vector3.TransformPosition(Vector3.Zero, JOBJManager.GetWorldTransform(ECB.ECBBone4));
                var bone5 = Vector3.TransformPosition(Vector3.Zero, JOBJManager.GetWorldTransform(ECB.ECBBone5));
                var bone6 = Vector3.TransformPosition(Vector3.Zero, JOBJManager.GetWorldTransform(ECB.ECBBone6));

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
            if (throwModelToolStripMenuItem.Checked && !SubactionProcess.ThrownFighter && ThrowDummyManager.JointCount > 0)
            {
                if (viewport.Frame < ThrowDummyManager.Animation.FrameCount)
                    ThrowDummyManager.Frame = viewport.Frame;
                ThrowDummyManager.SetWorldTransform(4, JOBJManager.GetWorldTransform(JOBJManager.JointCount - 2));
                ThrowDummyManager.Render(cam, false);

                DrawShape.DrawSphere(ThrowDummyManager.GetWorldTransform(35), 1.5f, 16, 16, ThrowDummyColor, 0.5f);
                DrawShape.DrawSphere(ThrowDummyManager.GetWorldTransform(4), 1.5f, 16, 16, ThrowDummyColor, 0.5f);
                DrawShape.DrawSphere(ThrowDummyManager.GetWorldTransform(10), 1f, 16, 16, ThrowDummyColor, 0.5f);
                DrawShape.DrawSphere(ThrowDummyManager.GetWorldTransform(15), 1f, 16, 16, ThrowDummyColor, 0.5f);
                DrawShape.DrawSphere(ThrowDummyManager.GetWorldTransform(22), 1f, 16, 16, ThrowDummyColor, 0.5f);
                DrawShape.DrawSphere(ThrowDummyManager.GetWorldTransform(40), 1f, 16, 16, ThrowDummyColor, 0.5f);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        private void LoadAnimation(string symbol)
        {
            // clear animation
            JOBJManager.SetFigaTree(null);

            // check if animation exists
            if (symbol == null || !SymbolToAnimation.ContainsKey(symbol))
                return;

            // load animation
            var anim = new HSDRawFile(SymbolToAnimation[symbol]);
            if (anim.Roots[0].Data is HSD_FigaTree tree)
            {
                var name = new Action() { Symbol = anim.Roots[0].Name }.ToString();

                JOBJManager.SetFigaTree(tree);

                _animEditor.SetJoint(JOBJManager.GetJOBJ(0), JOBJManager.Animation);

                viewport.MaxFrame = tree.FrameCount;

                ThrowDummyManager.CleanupRendering();
                ThrowDummyManager = new JOBJManager();

                //AnimationName = name;

                // load throw dummy for thrown animations
                if (name.Contains("Throw") && !name.Contains("Taro"))
                {
                    // find thrown anim
                    Action throwAction = null;
                    foreach (Action a in actionList.Items)
                    {
                        if (a.Symbol != null && a.Symbol.Contains("Taro") && a.Symbol.Contains(name) && !a.Symbol.Equals(anim.Roots[0].Name))
                        {
                            throwAction = a;
                            break;
                        }
                    }

                    if (throwAction != null && throwAction.Symbol != null && SymbolToAnimation.ContainsKey(throwAction.Symbol))
                    {
                        // load throw dummy
                        ThrowDummyManager.SetJOBJ(DummyThrowModel.GenerateThrowDummy());

                        // load throw animation
                        var tanim = new HSDRawFile(SymbolToAnimation[throwAction.Symbol]);
                        if (tanim.Roots[0].Data is HSD_FigaTree tree2)
                            ThrowDummyManager.SetFigaTree(tree2);
                    }

                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void SaveAnimation()
        {
            if (actionList.SelectedItem is Action action)
            {
                HSDRawFile f = new HSDRawFile();
                f.Roots.Add(new HSDRootNode()
                {
                    Name = action.Symbol,
                    Data = JOBJManager.Animation.ToFigaTree()
                });
                var tempFileName = Path.GetTempFileName();
                f.Save(tempFileName);
                SymbolToAnimation[action.Symbol] = File.ReadAllBytes(tempFileName);
                File.Delete(tempFileName);
            }
        }
    }
}
