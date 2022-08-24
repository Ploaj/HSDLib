using HSDRaw;
using HSDRaw.AirRide.Rd;
using HSDRaw.Melee.Cmd;
using HSDRaw.Melee.Pl;
using HSDRawViewer.GUI.Controls;
using HSDRawViewer.Tools;
using System;
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
                    //if (Node.Parent is DataNode parent)
                    //{
                    //    if (parent.Accessor is SBM_FighterData plDat)
                    //    {
                    //        ModelScale = plDat.Attributes.ModelScale;

                    //        if (plDat.Hurtboxes != null)
                    //            Hurtboxes.AddRange(plDat.Hurtboxes.Hurtboxes);

                    //        ECB = plDat.EnvironmentCollision;

                    //        ResetModelVis();
                    //    }
                    //}
                }
            }
        }

        private DataNode _node;

        private SBM_FighterActionTable _table;

        private SubactionGroup SubactionGroup = SubactionGroup.Fighter;

        private ScriptEditorActionList _actionList;

        private ScriptEditorSubactionEditor _subactionEditor;

        private DockableViewport _viewport;

        private HSDStruct _selectedAction;

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
            _viewport.Show(dockPanel1, DockState.Document);
            _viewport.GLLoad += () =>
            {
                _viewport.glViewport.Camera.DefaultTranslation = new OpenTK.Mathematics.Vector3(0, 10, -300);
                _viewport.glViewport.Camera.DefaultRotationX = 0;
                _viewport.glViewport.Camera.DefaultRotationY = 90 * (float)Math.PI / 180;
                _viewport.glViewport.Camera.RestoreDefault();
            };

            // updating z order
            dockPanel1.UpdateDockWindowZOrder(DockStyle.Left, true);

            // initialize action list
            _actionList = new ScriptEditorActionList();
            _actionList.Show(dockPanel1, DockState.DockLeft);

            _actionList.SelectAction += (a) =>
            {
                _selectedAction = a;
                _subactionEditor.InitScript(SubactionGroup, a);
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
                if (_selectedAction != null)
                    _selectedAction.SetFromStruct(a);
            };

            // dipose of resources
            FormClosing += (s, a) =>
            {
                _actionList.Dispose();
                _subactionEditor.Dispose();
                _viewport.Dispose();
            };
        }
    }
}
