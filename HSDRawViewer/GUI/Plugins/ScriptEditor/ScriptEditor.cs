using HSDRaw;
using HSDRaw.AirRide;
using HSDRaw.AirRide.Rd;
using HSDRaw.Common.Animation;
using HSDRaw.Melee;
using HSDRaw.Melee.Cmd;
using HSDRaw.Melee.Pl;
using HSDRaw.Tools;
using HSDRaw.Tools.Melee;
using HSDRawViewer.GUI.Controls;
using HSDRawViewer.GUI.Extra;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Renderers;
using HSDRawViewer.Tools;
using System;
using System.Collections.Generic;
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
        typeof(KAR_RdScript),
        typeof(KAR_WpScript) })]
    public partial class ScriptEditor : SaveableEditorBase, IDrawableInterface
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

                    if (value.Accessor is KAR_WpScript)
                        SubactionGroup = SubactionGroup.Weapon;

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
        private GLTextRenderer text = new GLTextRenderer();

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

        private JointAnimManager BackupAnim;
        public List<FrameSpeedMultiplier> FrameSpeedModifiers { get; set; } = new List<FrameSpeedMultiplier>();

        public DrawOrder DrawOrder => DrawOrder.First;


        public SubactionProcessor Processor = new SubactionProcessor();

        public List<SubactionEvent> SelectedEvents { get; internal set; } = new List<SubactionEvent>();

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
                renderer.SetFrame(Processor, f);
            };
            _viewport.glViewport.AddRenderer(this);

            // create and add renderer
            renderer = new ScriptRenderer(Processor);

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

                // get events
                var events = SubactionEvent.GetEvents(SubactionGroup, a).ToList();

                // set script in editor
                _subactionEditor.InitScript(SubactionGroup, events);

                // load animation 
                LoadAnimation();

                // update renderer script
                SelectedEvents.Clear();
                if (SubactionGroup == SubactionGroup.Fighter)
                    Processor.SetStruct(events, SubactionGroup);

                // update frame tips
                UpdateFrameTips();
            };

            _actionList.ActionsUpdated += () =>
            {
                // update pointer array
                CustomPointerValue.Values = _actionList.GetPointers().Select(e => new CustomPointerValue() { Struct = e.Item2, Value = e.Item1 }).ToList();

                // update data
                if (_table != null)
                    _table.Commands = _actionList.ToActions().ToArray();
            };

            // initialize subaction editor
            _subactionEditor = new ScriptEditorSubactionEditor();
            _subactionEditor.Show(dockPanel1, DockState.DockTop);

            _subactionEditor.ScriptEdited += (events) =>
            {
                // update select action script
                if (_selectedAction != null)
                    _selectedAction.SetFromStruct(SubactionEvent.CompileEvent(events));

                // update renderer script
                SelectedEvents.RemoveAll(e => !events.Contains(e));
                if (SubactionGroup == SubactionGroup.Fighter)
                    Processor.SetStruct(events, SubactionGroup);

                // update frame tips
                UpdateFrameTips();
            };

            _subactionEditor.SelectedIndexedChanged += (e) =>
            {
                SelectedEvents = e;
            };

            // initialize animation editor
            _animEditor = new PopoutJointAnimationEditor(false);
            _animEditor.FormClosing += (sender, args) =>
            {
                // quietly save changes to animation
                if (_animEditor.MadeChanges)
                {
                    // MessageBox.Show("Changes Made");
                    //if (MessageBox.Show("Save Changes to Animation?", "Save Animation", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                    SaveAnimationChanges();
                }
            };

            // dipose of resources
            FormClosing += (s, a) =>
            {
                if (AJManager != null && AJManager.Edited)
                {
                    var result = MessageBox.Show("Save Animation Changes?", "Closing Action Editor", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        OnDatFileSave();
                    }

                    if (result == DialogResult.Cancel)
                    {
                        a.Cancel = true;
                    }
                    else
                    {
                        EditorCloseDispose();
                    }
                }
                else
                {
                    EditorCloseDispose();
                }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        private void EditorCloseDispose()
        {
            ItCo = null;
            _animEditor.CloseOnExit = true;
            _animEditor.Dispose();
            _actionList.Dispose();
            _subactionEditor.Dispose();
            _viewport.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private HSD_FigaTree GetFigatreeFromSymbol(string symbol)
        {
            // get animation from file
            var data = AJManager.GetAnimationData(symbol);
            if (data != null)
            {
                var animFile = new HSDRawFile(data);
                if (animFile.Roots[0].Data is HSD_FigaTree tree)
                {
                    return tree;
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="throwstate"></param>
        private void CheckLoadThrowDummy(int state, int throwstate)
        {
            if (_selectStateIndex == state && throwstate < _actionList._actions.Length)
            {
                var throwAction = _actionList._actions[throwstate];
                var treeDummy = GetFigatreeFromSymbol(throwAction.Symbol);

                if (treeDummy != null)
                    renderer.LoadThrowDummyAnimation(new JointAnimManager(treeDummy));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadAnimation()
        {
            // clear fsm
            FrameSpeedModifiers.Clear();

            //
            renderer.LoadThrowDummyAnimation(null);

            //
            if (renderer.FighterModel.RootJObj != null)
            {
                // get animation from file
                var tree = GetFigatreeFromSymbol(_selectedActionSymbol);
                if (tree != null)
                {
                    // load as joint animation
                    var anim = new JointAnimManager(tree);

                    // set backup anim
                    BackupAnim = new JointAnimManager(tree);

                    // load animation in anim editor
                    _animEditor.SetJoint(renderer.FighterModel.RootJObj.Desc, anim);

                    // load animation into render
                    renderer.LoadAnimation(anim);

                    // load throw dummy for thrown animations
                    //247, 248, 249, 250
                    //262, 263, 264, 265
                    CheckLoadThrowDummy(247, 262);
                    CheckLoadThrowDummy(248, 263);
                    CheckLoadThrowDummy(249, 264);
                    CheckLoadThrowDummy(250, 265);

                    // set end frame
                    _viewport.glViewport.Frame = 0;
                    _viewport.glViewport.MaxFrame = tree.FrameCount;
                }
                else
                {
                    // load animation into render
                    renderer.LoadAnimation(null);
                }

                // clear action state render tips
                renderer.IsShieldState = false;
                renderer.HasItem = false;

                // load and display optional data
                if (!string.IsNullOrEmpty(_selectedActionSymbol))
                {
                    // set sheild render
                    renderer.IsShieldState = _selectedActionSymbol.Contains("Guard");

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
                        renderer.HasItem = true;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveAnimationChanges()
        {
            HSDRawFile f = new HSDRawFile();

            f.Roots.Add(new HSDRootNode()
            {
                Name = _selectedActionSymbol,
                Data = renderer.FighterModel.JointAnim.ToFigaTree()
            });

            using (MemoryStream stream = new MemoryStream())
            {
                f.Save(stream);
                AJManager.SetAnimation(_selectedActionSymbol, stream.ToArray());
            }

            BackupAnim.FromFigaTree(renderer.FighterModel.JointAnim.ToFigaTree());
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
            // rendering files not loaded
            if (string.IsNullOrEmpty(AJFilePath))
                return;

            // collect used symbols from all actions
            var usedSymbols = _actionList._actions.Select(e => e.Symbol).ToArray();

            // generate new aj file
            var newAJFile = AJManager.RebuildAJFile(usedSymbols, false);

            // update animation offset and sizes
            foreach (var a in _actionList._actions)
            {
                // update animation size and offset
                if (!string.IsNullOrEmpty(a.Symbol))
                {
                    var offsize = AJManager.GetOffsetSize(a.Symbol);
                    a.AnimOffset = offsize.Item1;
                    a.AnimSize = offsize.Item2;
                }
            }

            // save commands
            if (_table != null)
                _table.Commands = _actionList.ToActions().ToArray();

            // dump to file
            File.WriteAllBytes(AJFilePath, newAJFile);
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
            if (string.IsNullOrEmpty(symbol))
                return false;

            var AllActions = _actionList._actions.ToList();

            // get actions
            var actions = new ScriptAction[actionend - actionstart + 1];
            for (int i = actionstart; i <= actionend; i++)
                actions[i - actionstart] = AllActions[i];

            // rebuild aj file
            var data = AJManager.RebuildAJFile(actions.Select(e => e.Symbol).ToArray(), false);

            // update animation offset and sizes
            foreach (var a in actions)
            {
                // update animation size and offset
                if (!string.IsNullOrEmpty(a.Symbol))
                {
                    var offsize = AJManager.GetOffsetSize(a.Symbol);
                    a.AnimOffset = offsize.Item1;
                    a.AnimSize = offsize.Item2;
                }
            }

            // save commands
            if (_table != null)
                _table.Commands = _actionList.ToActions().ToArray();

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

            // TODO: update or add easy table
            //if (symbol == IntroSymbol)
            //{
            //    if (file["gmIntroEasyTable"] != null)
            //        file["gmIntroEasyTable"].Data = easyTable;
            //    else
            //        file.Roots.Add(new HSDRootNode() { Name = "gmIntroEasyTable", Data = easyTable });
            //}

            // save file
            file.Save(ajpath);

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
                    Processor.SetFrame(i);

                    // hitbox indication
                    if (Processor.HitboxesActive)
                        _viewport.glViewport.FrameTips.Add(new GUI.Controls.PlaybackBarFrameTip()
                        {
                            Frame = i,
                            Color = Color.Red,
                            Style = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipStyle.Color,
                            Location = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipLocation.Lower
                        });

                    // interrupt
                    if (Processor.AllowInterrupt)
                        _viewport.glViewport.FrameTips.Add(new GUI.Controls.PlaybackBarFrameTip()
                        {
                            Frame = i,
                            Color = Color.Green,
                            Style = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipStyle.Color,
                            Location = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipLocation.Middle
                        });

                    // interrupt
                    if (Processor.FighterFlagWasSetThisFrame.Any(e => e != false))
                    {
                        _viewport.glViewport.FrameTips.Add(new GUI.Controls.PlaybackBarFrameTip()
                        {
                            Frame = i,
                            Color = Color.Purple,
                            Style = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipStyle.Color,
                            Location = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipLocation.Upper
                        });

                        string set = "";

                        for (int k = 0; k < Processor.FighterFlagWasSetThisFrame.Length; k++)
                        {
                            if (Processor.FighterFlagWasSetThisFrame[k])
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
            foreach (var fsm in FrameSpeedModifiers)
            {
                _viewport.glViewport.FrameTips.Add(new GUI.Controls.PlaybackBarFrameTip()
                {
                    Frame = fsm.Frame,
                    Color = Color.Purple,
                    Style = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipStyle.Color,
                    Location = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipLocation.Upper
                });

                _viewport.glViewport.FrameTips.Add(new GUI.Controls.PlaybackBarFrameTip()
                {
                    Frame = fsm.Frame,
                    Color = Color.White,
                    Text = fsm.Rate.ToString(),
                    Style = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipStyle.Text,
                    Location = GUI.Controls.PlaybackBarFrameTip.PlaybackBarFrameTipLocation.Upper
                });
            }
            //}

            _viewport.Invalidate();

            // set frame
            renderer.SetFrame(Processor, _viewport.glViewport.Frame);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnDatFileSave()
        {
            if (!string.IsNullOrEmpty(AJFilePath))
                SaveFighterAnimationFile();

            if (!string.IsNullOrEmpty(ResultFilePath))
                SaveDemoAnimationFiles();

            if (exportTXTOnSaveToolStripMenuItem.Checked)
            {
                var newPath = Path.Combine(Path.GetDirectoryName(MainForm.Instance.FilePath), Path.GetFileNameWithoutExtension(MainForm.Instance.FilePath) + "_" + _node.Text + ".txt");
                ExportAsText(newPath);
            }    
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        private void ExportAsText(string f)
        {
            using (FileStream strem = new FileStream(f, FileMode.Create))
            using (StreamWriter w = new StreamWriter(strem))
            {
                int i = 0;
                foreach (var a in _actionList._actions)
                {
                    w.WriteLine($"[Flags(0x{a.Flags.ToString("X8")})]");
                    w.WriteLine($"{i} {a.ToString()}");
                    w.WriteLine("{");
                    var sub = SubactionEvent.GetEvents(SubactionGroup, a._struct);
                    foreach (var ev in sub)
                    {
                        w.WriteLine($"\t{ev.ToStringDescriptive()}");
                    }
                    w.WriteLine("}");
                    i++;
                }
                i = 0;
                foreach (var a in _actionList._subroutines)
                {
                    w.WriteLine($"{i} {a.ToString()}");
                    w.WriteLine("{");
                    var sub = SubactionEvent.GetEvents(SubactionGroup, a._struct);
                    foreach (var ev in sub)
                        w.WriteLine($"\t{ev.ToStringDescriptive()}");
                    w.WriteLine("}");
                    i++;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (renderer.FighterModel.RootJObj == null)
                return;
            
            if (_actionList.SelectedAction != null)
            {
                var f = FileIO.OpenFile("Supported Formats |*.dat;*.anim");

                // check if file is null
                if (f == null)
                    return;

                HSDRawFile file;

                // if it's a maya anim then convert to figatree and set the symbol
                if (f.ToLower().EndsWith(".anim"))
                {
                    var anim = Converters.ConvMayaAnim.ImportFromMayaAnim(f, null);

                    file = new HSDRawFile();
                    file.Roots.Add(new HSDRootNode()
                    {
                        Name = _selectedActionSymbol,
                        Data = anim.ToFigaTree(0.01f)
                    });
                }
                else
                {
                    // just load dat normally
                    try
                    {
                        file = new HSDRawFile(f);
                    }
                    catch
                    {
                        return;
                    }
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
                    _actionList.SelectedAction.Symbol = symbol;
                    _selectedActionSymbol = symbol;

                    // reselect action
                    LoadAnimation();

                    // 
                    _actionList.Invalidate();

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void asMayaAnimToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (renderer.FighterModel.RootJObj == null || renderer.FighterModel.JointAnim == null)
                return;

            renderer.FighterModel.JointAnim.ExportAsMayaAnim(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void asFigaTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (renderer.FighterModel.RootJObj == null || renderer.FighterModel.JointAnim == null)
                return;

            var f = Tools.FileIO.SaveFile(ApplicationSettings.HSDFileFilter);

            if (f != null)
                File.WriteAllBytes(f, AJManager.GetAnimationData(_selectedActionSymbol));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (renderer.FighterModel.RootJObj == null || renderer.FighterModel.JointAnim == null)
                return;

            _animEditor.Show();
            _animEditor.Visible = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applyFSMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (renderer.FighterModel.RootJObj == null || renderer.FighterModel.JointAnim == null)
                return;

            // component box edit fsms
            PopoutCollectionEditor.EditValue(this, this, "FrameSpeedModifiers");

            // remove rates of 0
            FrameSpeedModifiers.RemoveAll(e => e.Rate <= 0);

            // load backup animation
            var tempanim = new JointAnimManager();
            tempanim.FromFigaTree(BackupAnim.ToFigaTree());
            var backup = BackupAnim;

            // apply fsms to backup animation
            tempanim.ApplyFSMs(FrameSpeedModifiers);

            // load edited anim
            renderer.LoadAnimation(tempanim);
            _animEditor.SetJoint(renderer.FighterModel.RootJObj.Desc, tempanim);
            _viewport.glViewport.MaxFrame = tempanim.FrameCount;

            // new backup
            BackupAnim = backup;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applyFSMToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (renderer.FighterModel.RootJObj == null || renderer.FighterModel.JointAnim == null)
                return;

            if (MessageBox.Show("Are you sure you want to apply fsms?\nThis operation cannot be undone.", "Apply FSMs", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                SaveAnimationChanges();
                FrameSpeedModifiers.Clear();
                LoadAnimation();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateRenderCheckboxes(object sender, EventArgs e)
        {
            renderer.RenderFighter = fighterModelToolStripMenuItem.Checked;
            renderer.RenderItem = itemModelToolStripMenuItem.Checked;
            renderer.RenderBones = bonesToolStripMenuItem.Checked;

            renderer.RenderHitboxes = hitboxesToolStripMenuItem.Checked;
            renderer.RenderHurtboxes = hurtboxesToolStripMenuItem.Checked;
            renderer.RenderInterpolation = hitboxInterpolationToolStripMenuItem.Checked;
            renderer.RenderHitboxInfo = hitboxInfoToolStripMenuItem.Checked;

            renderer.RenderECB = environmentCollisionToolStripMenuItem.Checked;
            renderer.ECBGrounded = groundedECBToolStripMenuItem.Checked;
            renderer.RenderLedgeBox = ledgeGrabBoxToolStripMenuItem.Checked;
            renderer.RenderShield = shieldBubbleToolStripMenuItem.Checked;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kbState"></param>
        public void ViewportKeyPress(KeyEventArgs kbState)
        {
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
            List<SubactionEvent> hitboxes = new List<SubactionEvent>();

            foreach (var hb in Processor.Hitboxes)
            {
                if (hb.Active)
                {
                    var position = hb.GetWorldPosition(renderer.FighterModel.RootJObj);

                    if (pick.CheckSphereHitDistance(position, hb.Size, out float dis))
                    {
                        hitboxes.Add(hb.EventSource);
                    }
                }
            }

            _subactionEditor.SelectEvents(hitboxes);
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
            bool update = false;
            foreach (var hb in Processor.Hitboxes)
            {
                if (hb.Active && SelectedEvents.Contains(hb.EventSource))
                {
                    if (args.Button == MouseButtons.Left)
                        hb._widget.MouseDown(pick);
                    else
                        hb._widget.MouseUp();

                    hb._widget.Drag(pick);

                    if (hb._widget.PendingUpdate)
                    {
                        update = true;
                        hb._widget.PendingUpdate = false;
                    }
                }
            }
            if (update)
                _subactionEditor.ApplyScriptChanges();
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
        public void GLInit()
        {
            renderer.GLInit();
            text.InitializeRender(@"Consolas.bff");
        }

        /// <summary>
        /// 
        /// </summary>
        public void GLFree()
        {
            renderer.GLFree();
            text.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            renderer.Draw(cam, Processor, SelectedEvents);

            foreach (var hb in Processor.Hitboxes)
            {
                if (hb.Active && SelectedEvents.Contains(hb.EventSource))
                {
                    hb._widget.Render(cam, text);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool FreezeCamera()
        {
            return Processor.Hitboxes.Any(e => e.Active && e._widget.Interacting);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportAllAsTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.SaveFile("Plain Text (*.txt)|*.txt");

            if (f != null)
                ExportAsText(f);
        }
    }
}
