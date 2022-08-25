using HSDRaw;
using HSDRaw.AirRide.Rd;
using HSDRaw.Common.Animation;
using HSDRaw.Melee;
using HSDRaw.Melee.Cmd;
using HSDRaw.Melee.Pl;
using HSDRaw.Tools.Melee;
using HSDRawViewer.GUI.Controls;
using HSDRawViewer.GUI.Extra;
using HSDRawViewer.Rendering;
using HSDRawViewer.Tools;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Plugins.SubactionEditor
{
    [SupportedTypes(new Type[] {
        typeof(SBM_FighterActionTable),
        typeof(SBM_FighterSubactionData),
        typeof(SBM_ItemSubactionData),
        typeof(SBM_ColorSubactionData),
        typeof(KAR_RdScript) })]
    public partial class ScriptEditor : SaveableEditorBase
    {
        public override DataNode Node
        {
            get => _node;
            set
            {
                _node = value;
                if (value.Accessor is SBM_FighterSubactionData sudata)
                {
                    if (value.Accessor is SBM_ItemSubactionData)
                        SubactionGroup = SubactionGroup.Item;

                    if (value.Accessor is SBM_ColorSubactionData)
                        SubactionGroup = SubactionGroup.Color;

                    if (value.Accessor is KAR_RdScript)
                        SubactionGroup = SubactionGroup.Rider;

                    SBM_FighterAction[] su = new SBM_FighterAction[]
                    {
                        new SBM_FighterAction()
                        {
                            SubAction = sudata,
                            Name = "Script"
                        }
                    };

                    _actionList.LoadActions(su);
                    _actionList.CanAddNewActions = false;

                    // disable fighter only stuff
                    //loadPlayerFilesToolStripMenuItem.Enabled = false;
                    //propertyGrid1.Visible = false;
                }
                else
                if (value.Accessor is SBM_FighterActionTable SubactionTable)
                {
                    _actionList.LoadActions(SubactionTable.Commands);
                    _table = SubactionTable;
                    if (Node.Parent is DataNode parent)
                    {
                        if (parent.Accessor is SBM_FighterData plDat)
                        {
                            renderer.FighterModel.ModelScale = plDat.Attributes.ModelScale;
                            renderer.ShieldSize = plDat.Attributes.ShieldSize;
                            renderer.LookupTable = plDat.ModelLookupTables;
                            renderer.ECB = plDat.EnvironmentCollision;

                            if (plDat.Hurtboxes != null)
                                renderer.Hurtboxes.AddRange(plDat.Hurtboxes.Hurtboxes);

                            if (plDat.ModelPartAnimations != null)
                                renderer.ModelPartsIndices.AddRange(plDat.ModelPartAnimations.Array.Select(e => new ModelPartAnimations(e)));

                        }
                    }
                }
            }
        }

        private DataNode _node;

        private SBM_FighterActionTable _table;

        private SubactionGroup SubactionGroup = SubactionGroup.Fighter;

        private ScriptEditorActionList _actionList;

        private ScriptEditorSubactionEditor _subactionEditor;

        private DockableViewport _viewport;

        private PopoutJointAnimationEditor _animEditor;

        private ScriptRenderer renderer;

        private HSDStruct _selectedAction;
        private string _selectedActionSymbol;
        private int _selectStateIndex;


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

        private HSDRawFile ItCo;

        private bool IsSaving = false;

        private JointAnimManager BackupAnim;


        /// <summary>
        /// 
        /// </summary>
        public ScriptEditor()
        {
            InitializeComponent();

            // initial theme
            dockPanel1.Theme = new VS2015LightTheme();

            // initialize viewport
            _viewport = new DockableViewport();
            _viewport.glViewport.AnimationTrackEnabled = true;
            _viewport.Show(dockPanel1, DockState.Document);
            _viewport.GLLoad += () =>
            {
                _viewport.glViewport.Camera.DefaultTranslation = new OpenTK.Mathematics.Vector3(0, 10, -300);
                _viewport.glViewport.Camera.DefaultRotationX = 0;
                _viewport.glViewport.Camera.DefaultRotationY = 90 * (float)Math.PI / 180;
                _viewport.glViewport.Camera.RestoreDefault();
            };
            _viewport.glViewport.FrameChange += (f) =>
            {
                renderer.SetFrame(f);
            };

            // create and add renderer
            renderer = new ScriptRenderer();
            _viewport.glViewport.AddRenderer(renderer);

            // updating z order
            dockPanel1.UpdateDockWindowZOrder(DockStyle.Left, true);

            // initialize action list
            _actionList = new ScriptEditorActionList();
            _actionList.Show(dockPanel1, DockState.DockLeft);

            _actionList.SelectAction += (symbol, a, index) =>
            {
                // get selected actin
                _selectedAction = a;
                _selectedActionSymbol = symbol;
                _selectStateIndex = index;

                // set script in editor
                _subactionEditor.InitScript(SubactionGroup, a);

                // load animation 
                LoadAnimation();

                // update renderer script
                renderer.SetScript(a);

                // update frame tips
                UpdateFrameTips();
            };

            _actionList.ActionsUpdated += () =>
            {
                // update pointer array
                CustomPointerProperty.pointers = _actionList.GetPointers();

                // update data
                if (_table != null)
                    _table.Commands = _actionList.ToActions().ToArray();
            };

            // initialize subaction editor
            _subactionEditor = new ScriptEditorSubactionEditor();
            _subactionEditor.Show(dockPanel1, DockState.DockTop);

            _subactionEditor.ScriptEdited += (a) =>
            {
                // update select action script
                if (_selectedAction != null)
                    _selectedAction.SetFromStruct(a);

                // update renderer script
                renderer.SetScript(a);

                // update frame tips
                UpdateFrameTips();
            };

            // initialize animation editor
            _animEditor = new PopoutJointAnimationEditor(false);

            // dipose of resources
            FormClosing += (s, a) =>
            {
                ItCo = null;
                _animEditor.CloseOnExit = true;
                _animEditor.Dispose();
                _actionList.Dispose();
                _subactionEditor.Dispose();
                _viewport.Dispose();
            };
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadAnimation()
        {
            if (renderer.FighterModel.RootJObj != null)
            {
                // get animation from file
                var data = AJManager.GetAnimationData(_selectedActionSymbol);
                if (data != null)
                {
                    var animFile = new HSDRawFile(data);
                    if (animFile.Roots[0].Data is HSD_FigaTree tree)
                    {
                        // load as joint animation
                        var anim = new JointAnimManager(tree);

                        // set backup anim
                        BackupAnim = new JointAnimManager(tree);

                        // load animation in anim editor
                        _animEditor.SetJoint(renderer.FighterModel.RootJObj.Desc, anim);

                        // load animation into render
                        renderer.LoadAnimation(_selectedActionSymbol, anim);

                        // set end frame
                        _viewport.glViewport.Frame = 0;
                        _viewport.glViewport.MaxFrame = tree.FrameCount;
                    }
                }
                else
                {
                    // load animation into render
                    renderer.LoadAnimation(_selectedActionSymbol, null);
                }

                // clear action state render tips
                renderer.RenderShield = false;
                renderer.RenderItem = false;

                // load and display optional data
                if (!string.IsNullOrEmpty(_selectedActionSymbol))
                {
                    // set sheild render
                    renderer.RenderShield = _selectedActionSymbol.Contains("Guard");

                    // load optional item model
                    if (_selectedActionSymbol.Contains("ItemParasol"))
                        LoadItemModel(13);
                    if (_selectedActionSymbol.Contains("ItemShoot"))
                        LoadItemModel(16);
                    if (_selectedActionSymbol.Contains("ItemScope"))
                        LoadItemModel(21);
                    if (_selectedActionSymbol.Contains("ItemHammer"))
                        LoadItemModel(28);
                }

                if (_selectStateIndex != -1)
                {
                    if (_selectStateIndex >= 108 && _selectStateIndex <= 111)
                        LoadItemModel(12);
                    if (_selectStateIndex >= 112 && _selectStateIndex <= 115)
                        LoadItemModel(11);
                    if (_selectStateIndex >= 116 && _selectStateIndex <= 119)
                        LoadItemModel(13);
                    if (_selectStateIndex >= 120 && _selectStateIndex <= 123)
                        LoadItemModel(24);
                    if (_selectStateIndex >= 124 && _selectStateIndex <= 127)
                        LoadItemModel(22);
                    if (_selectStateIndex >= 128 && _selectStateIndex <= 131)
                        LoadItemModel(23);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        private void LoadItemModel(int id)
        {
            if (ItCo != null)
            {
                if (ItCo["itPublicData"]?.Data is itPublicData it)
                {
                    var item = it.Items.Articles[id];

                    if (item.Model.RootModelJoint != null)
                    {
                        renderer.LoadItemModel(item.Model.RootModelJoint, item.Parameters.ModelScale);
                        renderer.RenderItem = true;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadModelAndAnimationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_node.Text.Contains("Fighter"))
            {
                LoadFighterAnimationFiles();
            }
            else
            if (_node.Text.Contains("Demo"))
            {
                LoadDemoAnimationFiles();
            }
            else
            {
                MessageBox.Show("Rendering not available for this node", "Unsupported Rendering", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            LoadAnimation();
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

            // item data
            var itcoPath = Path.GetDirectoryName(MainForm.Instance.FilePath) + "\\ItCo.dat";
            if (File.Exists(itcoPath))
                ItCo = new HSDRawFile(itcoPath);

            // dummy model
            var dummyPath = Path.GetDirectoryName(MainForm.Instance.FilePath) + "\\PlMrNr.dat";
            if (File.Exists(dummyPath))
                renderer.LoadThrowDummy(dummyPath);

            // load model
            renderer.LoadFighterModel(cFile);
        }


        /// <summary>
        /// 
        /// </summary>
        private void SaveFighterAnimationFile()
        {
            //// rendering files not loaded
            //if (string.IsNullOrEmpty(AJFilePath))
            //    return;

            //// collect used symbols from all actions
            //var usedSymbols = AllActions.Select(e => e.Symbol).ToArray();

            //// generate new aj file
            //var newAJFile = AJManager.RebuildAJFile(usedSymbols, false);

            //// update animation offset and sizes
            //foreach (var a in AllActions)
            //{
            //    // don't write subroutines
            //    if (a.Subroutine)
            //        continue;

            //    // update animation size and offset
            //    if (!string.IsNullOrEmpty(a.Symbol))
            //    {
            //        var offsize = AJManager.GetOffsetSize(a.Symbol);
            //        a.AnimOffset = offsize.Item1;
            //        a.AnimSize = offsize.Item2;
            //    }
            //}

            //// save action changes to dat file
            //IsSaving = true;
            //SaveAllActionChanges();
            //IsSaving = false;

            //// dump to file
            //File.WriteAllBytes(AJFilePath, newAJFile);
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
            //if (File.Exists(IntroFilePath))
            //{
            //    var f = new HSDRawFile(IntroFilePath);
            //    if (f["gmIntroEasyTable"] != null)
            //        easyTable = f["gmIntroEasyTable"].Data as SBM_gmIntroEasyTable;
            //}

            MessageBox.Show($"Loaded:\nResultBank: {ResultFilePath}\nWaitBank: {WaitFilePath}\nIntroBank: {IntroFilePath}\nEndingBank: {EndingFilePath}");

            // load model
            renderer.LoadFighterModel(modelFile);
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
            //if (string.IsNullOrEmpty(symbol))
            //    return false;

            //// get actions
            //var actions = new Action[actionend - actionstart + 1];
            //for (int i = actionstart; i <= actionend; i++)
            //    actions[i - actionstart] = AllActions[i];

            //// rebuild aj file
            //var data = AJManager.RebuildAJFile(actions.Select(e => e.Symbol).ToArray(), false);

            //// update animation offset and sizes
            //foreach (var a in actions)
            //{
            //    // don't write subroutines
            //    if (a.Subroutine)
            //        continue;

            //    // update animation size and offset
            //    if (!string.IsNullOrEmpty(a.Symbol))
            //    {
            //        var offsize = AJManager.GetOffsetSize(a.Symbol);
            //        a.AnimOffset = offsize.Item1;
            //        a.AnimSize = offsize.Item2;
            //    }
            //}

            //// save action changes to dat file
            //SaveAllActionChanges();

            //// load or create file
            //HSDRawFile file = null;
            //if (File.Exists(ajpath))
            //    file = new HSDRawFile(ajpath);
            //else
            //    file = new HSDRawFile();

            //// update or add aj data
            //var dataAccessor = new HSDAccessor() { _s = new HSDStruct(data) };
            //if (file[symbol] != null)
            //    file[symbol].Data = dataAccessor;
            //else
            //    file.Roots.Add(new HSDRootNode() { Name = symbol, Data = dataAccessor });

            //// update or add easy table
            //if (symbol == IntroSymbol)
            //{
            //    if (file["gmIntroEasyTable"] != null)
            //        file["gmIntroEasyTable"].Data = easyTable;
            //    else
            //        file.Roots.Add(new HSDRootNode() { Name = "gmIntroEasyTable", Data = easyTable });
            //}

            //// save file
            //file.Save(ajpath);

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateFrameTips()
        {
            // clear frame tips
            _viewport.glViewport.FrameTips.Clear();

            // update trackbar data
            //if (trackInfoToolStripMenuItem.Checked)
            {
                // add frame tips for each frame
                for (int i = 0; i <= _viewport.glViewport.MaxFrame; i++)
                {
                    renderer.Processor.SetFrame(i);

                    // hitbox indication
                    if (renderer.Processor.HitboxesActive)
                        _viewport.glViewport.FrameTips.Add(new GUI.Controls.PlaybackBarFrameTip()
                        {
                            Frame = i,
                            Color = Color.Red,
                            Style = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipStyle.Color,
                            Location = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipLocation.Lower
                        });

                    // interrupt
                    if (renderer.Processor.AllowInterrupt)
                        _viewport.glViewport.FrameTips.Add(new GUI.Controls.PlaybackBarFrameTip()
                        {
                            Frame = i,
                            Color = Color.Green,
                            Style = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipStyle.Color,
                            Location = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipLocation.Middle
                        });

                    // interrupt
                    if (renderer.Processor.FighterFlagWasSetThisFrame.Any(e => e != false))
                    {
                        _viewport.glViewport.FrameTips.Add(new GUI.Controls.PlaybackBarFrameTip()
                        {
                            Frame = i,
                            Color = Color.Purple,
                            Style = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipStyle.Color,
                            Location = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipLocation.Upper
                        });

                        string set = "";

                        for (int k = 0; k < renderer.Processor.FighterFlagWasSetThisFrame.Length; k++)
                        {
                            if (renderer.Processor.FighterFlagWasSetThisFrame[k])
                            {
                                set += k + " ";
                            }
                        }

                        _viewport.glViewport.FrameTips.Add(new GUI.Controls.PlaybackBarFrameTip()
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

            //if (fsmMode.Checked)
            //{
            //    foreach (var fsm in FrameSpeedModifiers)
            //    {
            //        viewport.FrameTips.Add(new GUI.Controls.PlaybackBarFrameTip()
            //        {
            //            Frame = fsm.Frame,
            //            Color = Color.Purple,
            //            Style = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipStyle.Color,
            //            Location = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipLocation.Upper
            //        });

            //        viewport.FrameTips.Add(new GUI.Controls.PlaybackBarFrameTip()
            //        {
            //            Frame = fsm.Frame,
            //            Color = Color.White,
            //            Text = fsm.Rate.ToString(),
            //            Style = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipStyle.Text,
            //            Location = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipLocation.Upper
            //        });
            //    }
            //}

            _viewport.glViewport.Invalidate();

            // set frame
            renderer.SetFrame(_viewport.glViewport.Frame);
        }
    }
}
