using HSDRaw;
using HSDRaw.Melee.Cmd;
using HSDRaw.Melee.Pl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Plugins.SubactionEditor
{
    public partial class ScriptEditorActionList : DockContent
    {
        public ScriptAction[] _actions { get; set; }

        public ScriptSubrountine[] _subroutines { get; set; }

        public delegate void SelectedActionCallback(string symbol, HSDStruct action, int index);
        public SelectedActionCallback SelectAction;

        public delegate void ActionListUpdated();
        public ActionListUpdated ActionsUpdated;

        public ScriptAction SelectedAction
        {
            get
            {
                return actionArrayEditor.SelectedObject as ScriptAction;
            }
        }

        public bool CanAddNewActions
        {
            get => actionArrayEditor.CanAdd;
            set
            {
                actionArrayEditor.CanAdd = value;
                actionArrayEditor.CanRemove = value;
                actionArrayEditor.CanClone = value;
                actionArrayEditor.CanMove = value;
                actionArrayEditor.EnablePropertyView = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public ScriptEditorActionList()
        {
            InitializeComponent();

            actionArrayEditor.ArrayUpdated += (a, r) =>
            {
                ActionsUpdated?.Invoke();
            };

            subroutineArrayEditor.ArrayUpdated += (a, r) =>
            {
                ActionsUpdated?.Invoke();
            };

            Text = "Action List";

            cbReference.Enabled = false;
            buttonGoto.Enabled = false;

            // prevent user closing
            CloseButtonVisible = false;
            FormClosing += (sender, args) =>
            {
                if (args.CloseReason == CloseReason.UserClosing)
                {
                    args.Cancel = true;
                }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actions"></param>
        public void LoadActions(SBM_FighterAction[] actions)
        {
            HashSet<HSDStruct> aHash = new HashSet<HSDStruct>();
            Queue<HSDStruct> extra = new Queue<HSDStruct>();
            List<ScriptAction> AllActions = new List<ScriptAction>();
            List<ScriptSubrountine> Subroutines = new List<ScriptSubrountine>();

            // process actions
            int index = 0;
            foreach (var v in actions)
            {
                if (v.SubAction == null)
                    v.SubAction = new SBM_FighterSubactionData();

                if (!aHash.Contains(v.SubAction._s))
                    aHash.Add(v.SubAction._s);

                // add to actions
                AllActions.Add(new ScriptAction(v));

                // cache references
                foreach (var c in v.SubAction._s.References)
                {
                    if (!aHash.Contains(c.Value))
                    {
                        extra.Enqueue(c.Value);
                    }
                }

                index++;
            }

            // process subroutines
            index = 0;
            while (extra.Count > 0)
            {
                var v = extra.Dequeue();
                if (!aHash.Contains(v))
                {
                    aHash.Add(v);
                    Subroutines.Add(new ScriptSubrountine()
                    {
                        _struct = v
                    });
                }

                foreach (var r in v.References)
                    if (!aHash.Contains(r.Value))
                        extra.Enqueue(r.Value);

                index++;
            }

            // initialize array
            _actions = AllActions.ToArray();
            _subroutines = Subroutines.ToArray();
            actionArrayEditor.SetArrayFromProperty(this, "_actions");
            subroutineArrayEditor.SetArrayFromProperty(this, "_subroutines");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SBM_FighterAction> ToActions()
        {
            foreach (var v in _actions)
            {
                yield return v._action;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateReferences(HSDStruct str)
        {
            // clear old references
            cbReference.Items.Clear();

            // gather all references to this script
            cbReference.Items.AddRange(_actions.Where(e => e._struct.References.ContainsValue(str)).ToArray());
            cbReference.Items.AddRange(_subroutines.Where(e => e._struct.References.ContainsValue(str)).ToArray());

            // select first item
            if (cbReference.Items.Count > 0)
            {
                cbReference.SelectedIndex = 0;
                cbReference.Enabled = true;
                buttonGoto.Enabled = true;
            }
            else
            {
                cbReference.Enabled = false;
                buttonGoto.Enabled = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RemoveAllReferences(HSDStruct str)
        {
            foreach (var v in _actions.Where(e => e._struct.References.ContainsValue(str)).ToArray())
                v._struct.RemoveReferenceToStruct(str);

            foreach (var v in _subroutines.Where(e => e._struct.References.ContainsValue(str)).ToArray())
                v._struct.RemoveReferenceToStruct(str);

            // TODO: set these references to dummies or remove the subaction
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Tuple<string, HSDStruct>> GetPointers()
        {
            List<Tuple<string, HSDStruct>> p = new List<Tuple<string, HSDStruct>>();

            p.AddRange(_actions.Select((e, i) => new Tuple<string, HSDStruct>($"{i} {e.ToString()}", e._struct)));
            p.AddRange(_subroutines.Select((e, i) => new Tuple<string, HSDStruct>($"{i} {e.ToString()}", e._struct)));

            return p;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void actionArrayEditor_SelectedObjectChanged(object sender, EventArgs e)
        {
            if (actionArrayEditor.SelectedObject is ScriptAction action)
            {
                UpdateReferences(action._struct);

                if (subroutineArrayEditor != null)
                    subroutineArrayEditor.SelectObject(null);

                SelectAction?.Invoke(action.Symbol, action._struct, Array.FindIndex(_actions, a => a == action));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subroutineArrayEditor_SelectedObjectChanged(object sender, EventArgs e)
        {
            if (subroutineArrayEditor.SelectedObject is ScriptSubrountine action)
            {
                UpdateReferences(action._struct);

                if (actionArrayEditor != null)
                    actionArrayEditor.SelectObject(null);

                SelectAction?.Invoke(null, action._struct, -1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGoto_Click(object sender, EventArgs e)
        {
            if (cbReference.SelectedItem is ScriptAction a)
            {
                tabControl1.SelectedIndex = 0;
                actionArrayEditor.SelectObject(a);
            }

            if (cbReference.SelectedItem is ScriptSubrountine r)
            {
                tabControl1.SelectedIndex = 1;
                subroutineArrayEditor.SelectObject(r);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void actionArrayEditor_OnItemRemove(RemovedItemEventArgs e)
        {
            // remove all references to this item
            if (actionArrayEditor.SelectedObject is ScriptAction action)
            {
                RemoveAllReferences(action._struct);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void subroutineArrayEditor_OnItemRemove(RemovedItemEventArgs e)
        {
            // remove all references to this item
            if (subroutineArrayEditor.SelectedObject is ScriptSubrountine action)
            {
                RemoveAllReferences(action._struct);
            }
        }
    }
}
