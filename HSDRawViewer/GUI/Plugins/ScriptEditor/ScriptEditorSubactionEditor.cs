using HSDRaw;
using HSDRawViewer.GUI.Plugins.ScriptEditor;
using HSDRawViewer.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Plugins.SubactionEditor
{
    public partial class ScriptEditorSubactionEditor : DockContent
    {
        public enum ScriptDisplayType
        {
            Simple,
            Descriptive,
        };

        private SubactionGroup Type = SubactionGroup.None;

        public delegate void ScriptEditedCallback(List<SubactionEvent> events);
        public ScriptEditedCallback ScriptEdited;

        public delegate void SelectedIndexChange(List<SubactionEvent> selectedEvents);
        public SelectedIndexChange SelectedIndexedChanged;

        private readonly Stack<HSDStruct> UndoDataStack = new();
        private readonly Stack<HSDStruct> RedoDataStack = new();

        private readonly Timer dragDropTimer;
        private Point dragDropMousePosition;

        /// <summary>
        /// 
        /// </summary>
        public ScriptEditorSubactionEditor()
        {
            InitializeComponent();

            Text = "Action Script";

            //
            cbDisplayType.ComboBox.DataSource = Enum.GetValues(typeof(ScriptDisplayType));
            cbDisplayType.SelectedIndex = 1;

            // 
            propertyGrid1.PropertyValueChanged += (s, a) =>
            {
                ApplyScriptChanges();
                subActionList.Invalidate();
            };

            // initialize drag and drop timer
            dragDropTimer = new Timer();
            dragDropTimer.Interval = 500;
            dragDropTimer.Tick += (s, a) =>
            {
                if (dragDropMousePosition == Cursor.Position && subActionList.SelectedItems.Count == 1)
                {
                    subActionList.DoDragDrop(subActionList.SelectedItems, DragDropEffects.Move);
                    dragDropTimer.Stop();
                }
                dragDropMousePosition = Cursor.Position;
            };

            // dispose timer
            Disposed += (s, a) =>
            {
                dragDropTimer.Stop();
                dragDropTimer.Dispose();
            };

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
        /// <param name="events"></param>
        public void SelectEvents(IEnumerable<SubactionEvent> events)
        {
            subActionList.ClearSelected();

            foreach (SubactionEvent e in events)
            {
                int index = subActionList.Items.IndexOf(e);

                if (index >= 0)
                    subActionList.SetSelected(index, true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ApplyScriptChanges()
        {
            List<SubactionEvent> events = new();

            foreach (object i in subActionList.Items)
                if (i is SubactionEvent ev)
                    events.Add(ev);

            ScriptEdited?.Invoke(events);

            RedoDataStack.Clear();
            UndoDataStack.Push(SubactionEvent.CompileEvent(events));
        }

        /// <summary>
        /// 
        /// </summary>
        public void InitScript(SubactionGroup type, List<SubactionEvent> events)
        {
            //
            LoadData(type, events);

            // initialize undo stack
            UndoDataStack.Clear();
            UndoDataStack.Push(SubactionEvent.CompileEvent(events));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        private void LoadData(SubactionGroup type, List<SubactionEvent> events)
        {
            // load action types
            if (type != Type)
            {
                // set type
                Type = type;

                // set new actions
                cbActionType.Items.Clear();
                cbActionType.Items.AddRange(SubactionManager.GetGroup(Type).ToArray());
            }

            //
            // var events = SubactionEvent.GetEvents(type, data);

            //
            subActionList.BeginUpdate();
            subActionList.Items.Clear();
            foreach (SubactionEvent v in events)
                subActionList.Items.Add(v);
            subActionList.EndUpdate();

            //
            propertyGrid1.SelectedObject = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Undo()
        {
            if (UndoDataStack.Count > 0)
            {
                HSDStruct undo = UndoDataStack.Pop();

                if (undo != null)
                {
                    List<SubactionEvent> events = SubactionEvent.GetEvents(Type, undo).ToList();
                    LoadData(Type, events);
                    RedoDataStack.Push(undo);
                    ScriptEdited?.Invoke(events);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Redo()
        {
            if (RedoDataStack.Count > 0)
            {
                HSDStruct redo = RedoDataStack.Pop();

                if (redo != null)
                {
                    List<SubactionEvent> events = SubactionEvent.GetEvents(Type, redo).ToList();
                    LoadData(Type, events);
                    UndoDataStack.Push(redo);
                    ScriptEdited?.Invoke(events);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RemoveSelected()
        {
            subActionList.BeginUpdate();
            if (subActionList.SelectedItems.Count != 0)
            {
                List<object> toRemove = new();

                foreach (object v in subActionList.SelectedItems)
                    toRemove.Add(v);

                foreach (object v in toRemove)
                    subActionList.Items.Remove(v);
            }
            subActionList.EndUpdate();

            ApplyScriptChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        private void CopySelected()
        {
            // Lets say its my data format
            Clipboard.Clear();

            // get collections of selected scripts
            List<SubactionEvent> scripts = new();
            foreach (SubactionEvent scr in subActionList.SelectedItems)
                scripts.Add(scr);

            // Put data into clipboard
            Clipboard.SetData(typeof(Tuple<SubactionGroup, HSDStruct>).FullName, new Tuple<SubactionGroup, HSDStruct>(Type, SubactionEvent.CompileEvent(scripts)));
        }

        /// <summary>
        /// 
        /// </summary>
        private void Paste()
        {
            // get insert index
            int index = subActionList.SelectedIndex + 1;
            if (index == -1)
                index = 0;

            string text = Clipboard.GetText();
            if (text != null)
            {
                // try to parse script code
                List<SubactionEvent> scripts = new List<SubactionEvent>();
                scripts.AddRange(SubactionFromText.FromBrawlText(text));
                scripts.AddRange(SubactionFromText.FromUltimateText(text));

                // insert scripts
                scripts.Reverse();
                foreach (SubactionEvent v in scripts)
                    // only paste subactions the belong to this group
                    if (v.Type == SubactionGroup.Fighter)
                        subActionList.Items.Insert(index, v);

                // apply changes
                ApplyScriptChanges();
            }

            // Get data object from the clipboard
            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject != null)
            {
                // Check if a collection of Slides is in the clipboard
                string dataFormat = typeof(Tuple<SubactionGroup, HSDStruct>).FullName;
                if (dataObject.GetDataPresent(dataFormat))
                {
                    // Retrieve slides from the clipboard
                    Tuple<SubactionGroup, HSDStruct> scripts = dataObject.GetData(dataFormat) as Tuple<SubactionGroup, HSDStruct>;

                    if (scripts != null && scripts.Item1 == Type)
                    {
                        // insert scripts
                        IEnumerable<SubactionEvent> events = SubactionEvent.GetEvents(Type, scripts.Item2).Reverse();

                        // only paste subactions the belong to this group
                        subActionList.BeginUpdate();
                        foreach (SubactionEvent v in events)
                            if (v.Type == Type)
                                subActionList.Items.Insert(index, v);
                        subActionList.EndUpdate();

                        // apply changes
                        ApplyScriptChanges();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ReplacePaste()
        {
            int selectedIndex = subActionList.SelectedIndex;
            if (selectedIndex != -1)
            {
                RemoveSelected();
                subActionList.SelectedIndex = selectedIndex - 1;
                Paste();
                subActionList.SelectedIndex = selectedIndex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subActionList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Oemplus)
                buttonAdd_Click(buttonAdd, null);

            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.OemMinus)
                RemoveSelected();

            if (e.Control)
            {
                if (e.KeyCode == Keys.X)
                {
                    CopySelected();
                    RemoveSelected();
                }

                if (e.KeyCode == Keys.C)
                    CopySelected();

                if (e.KeyCode == Keys.V)
                {
                    if (e.Shift)
                        ReplacePaste();
                    else
                        Paste();
                }

                if (e.KeyCode == Keys.Z)
                    Undo();

                if (e.KeyCode == Keys.Y)
                    Redo();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            SubactionEvent ac = new(Type, 0);

            subActionList.Items.Insert(subActionList.SelectedIndex + 1, ac);
            subActionList.SelectedItem = null;
            subActionList.SelectedItem = ac;

            ApplyScriptChanges();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRemove_Click(object sender, EventArgs e)
        {
            RemoveSelected();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCut_Click(object sender, EventArgs e)
        {
            CopySelected();
            RemoveSelected();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCopy_Click(object sender, EventArgs e)
        {
            CopySelected();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPaste_Click(object sender, EventArgs e)
        {
            Paste();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonReplace_Click(object sender, EventArgs e)
        {
            ReplacePaste();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonUp_Click(object sender, EventArgs e)
        {
            subActionList.BeginUpdate();
            int[] indexes = subActionList.SelectedIndices.Cast<int>().ToArray();
            if (indexes.Length > 0 && indexes[0] > 0)
            {
                for (int i = 0; i < subActionList.Items.Count; ++i)
                {
                    if (indexes.Contains(i))
                    {
                        object moveItem = subActionList.Items[i];
                        subActionList.Items.Remove(moveItem);
                        subActionList.Items.Insert(i - 1, moveItem);
                        subActionList.SetSelected(i - 1, true);
                    }
                }
            }
            subActionList.EndUpdate();
            ApplyScriptChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDown_Click(object sender, EventArgs e)
        {
            subActionList.BeginUpdate();
            int[] indexes = subActionList.SelectedIndices.Cast<int>().ToArray();
            if (indexes.Length > 0 && indexes[indexes.Length - 1] < subActionList.Items.Count - 1)
            {
                for (int i = subActionList.Items.Count - 1; i > -1; --i)
                {
                    if (indexes.Contains(i))
                    {
                        object moveItem = subActionList.Items[i];
                        subActionList.Items.Remove(moveItem);
                        subActionList.Items.Insert(i + 1, moveItem);
                        subActionList.SetSelected(i + 1, true);
                    }
                }
            }
            subActionList.EndUpdate();
            ApplyScriptChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subActionList_DragDrop(object sender, DragEventArgs e)
        {
            Point point = subActionList.PointToClient(new Point(e.X, e.Y));
            int index = subActionList.IndexFromPoint(point);
            if (index < 0) index = subActionList.Items.Count - 1;

            List<object> data = new();

            foreach (object i in subActionList.SelectedItems)
                data.Add(i);

            foreach (object i in data)
                subActionList.Items.Remove(i);

            foreach (object i in data)
            {
                subActionList.Items.Insert(index, i);
            }

            subActionList.SelectedIndex = index;

            ApplyScriptChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subActionList_DragOver(object sender, DragEventArgs e)
        {

            e.Effect = DragDropEffects.Move;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subActionList_MouseDown(object sender, MouseEventArgs e)
        {
            if (subActionList.SelectedItem == null) return;

            dragDropMousePosition = Cursor.Position;
            dragDropTimer.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subActionList_MouseUp(object sender, MouseEventArgs e)
        {
            dragDropTimer.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subActionList_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            //if (e.Index != -1 && subActionList.Items[e.Index] is SubactionEvent script)
            //{
            //    var length = script.GetParamsAsString().Count();
            //    e.ItemHeight = subActionList.Font.Height * (cbDisplayType.SelectedIndex != 0 ? 1 : length + 1);
            //    e.ItemHeight = Math.Min(e.ItemHeight, 255); // limit
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subActionList_DrawItem(object sender, DrawItemEventArgs e)
        {
            ScriptDisplayType displayType = (ScriptDisplayType)cbDisplayType.SelectedItem;

            e.DrawBackground();
            if (e.Index != -1)
            {
                if (subActionList.Items[e.Index] is SubactionEvent script)
                {
                    Subaction sa = SubactionManager.GetSubaction(script.Code, script.Type);

                    Color color = Color.DarkBlue;

                    if (sa.IsCustom)
                        color = Color.DarkOrange;

                    using SolidBrush brush = new(color);
                    switch (displayType)
                    {
                        case ScriptDisplayType.Simple:
                            {
                                e.Graphics.DrawString($"{e.Index}. {sa.Name}", e.Font, brush, e.Bounds);

                            }
                            break;
                        case ScriptDisplayType.Descriptive:
                            {
                                e.Graphics.DrawString($"{e.Index}. {script.ToStringDescriptive()}", e.Font, brush, e.Bounds);
                            }
                            break;
                    }
                    //int i = 1;
                    //if (displayType == ScriptDisplayType.ex)
                    //    foreach (var v in script.GetParamsAsString())
                    //    {
                    //        if (e.Bounds.Y + e.Font.Height * i >= e.Bounds.Y + e.Bounds.Height)
                    //            break;
                    //        var bottomRect = new Rectangle(new Point(e.Bounds.X, e.Bounds.Y + e.Font.Height * i), new Size(e.Bounds.Width, e.Bounds.Height));
                    //        e.Graphics.DrawString("\t" + v, e.Font, new SolidBrush(e.ForeColor), bottomRect);
                    //        i++;
                    //    }
                }
                else
                {
                    using SolidBrush brush = new(e.ForeColor);
                    e.Graphics.DrawString(subActionList.Items[e.Index].ToString(), e.Font, brush, e.Bounds);
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subActionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // clear selection
            propertyGrid1.SelectedObject = null;

            // clear type selection
            cbActionType.SelectedIndex = 0;

            // check number of selected items
            if (subActionList.SelectedItems.Count <= 1)
            {
                // set selected item type
                if (subActionList.SelectedItem is SubactionEvent ev)
                    cbActionType.SelectedItem = SubactionManager.GetSubaction(ev.Code, Type);

                // select object
                propertyGrid1.SelectedObject = subActionList.SelectedItem;

                // enable action change
                cbActionType.Enabled = true;
            }
            else
            {
                // multiselect items
                //object[] a = new object[subActionList.SelectedItems.Count];
                //for (int i = 0; i < a.Length; i++)
                //    a[i] = subActionList.SelectedItems[i];
                //propertyGrid1.SelectedObjects = a;

                // disable action change for multiple objects
                cbActionType.Enabled = false;
            }

            List<SubactionEvent> selected = new();
            foreach (SubactionEvent s in subActionList.SelectedItems)
                selected.Add(s);
            SelectedIndexedChanged?.Invoke(selected);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            subActionList.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbActionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (propertyGrid1.SelectedObject is SubactionEvent ev && cbActionType.SelectedItem is Subaction sa)
            {
                ev.SetCode(Type, sa.Code);
                ApplyScriptChanges();

                propertyGrid1.SelectedObject = subActionList.SelectedItem;
                subActionList.Invalidate();
            }
        }
    }
}
