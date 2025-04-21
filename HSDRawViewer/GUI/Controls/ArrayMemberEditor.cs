﻿using HSDRaw.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using static System.Windows.Forms.ListBox;

namespace HSDRawViewer.GUI
{
    public partial class ArrayMemberEditor : UserControl
    {
        private PropertyInfo Property { get; set; }

        public List<string> TextOverrides { get; } = new List<string>();

        private object Object
        {
            set
            {
                _object = value;
                Reset();
            }
        }
        private object _object;

        public object[] SelectedObjects => propertyGrid.SelectedObjects;
        public SelectedIndexCollection SelectedIndices => elementList.SelectedIndices;
        public object SelectedObject => elementList.SelectedItem;
        public int SelectedIndex => elementList.SelectedIndex;

        public event EventHandler SelectedObjectChanged;
        public event EventHandler DoubleClickedNode;
        public event EventHandler ArrayUpdated;

        public delegate void OnItemRemoveHandler(RemovedItemEventArgs e);
        public event OnItemRemoveHandler OnItemRemove;

        public delegate void OnItemAddedHandler(AddedItemEventArgs e);
        public event OnItemAddedHandler OnItemAdded;

        [DefaultValue(false)]
        public bool AskBeforeDelete
        {
            get => _askbeforedelete;
            set { _askbeforedelete = value; }
        }
        private bool _askbeforedelete = false;

        [DefaultValue(true)]
        public bool EnableToolStrip
        {
            get => _enableToolStrip;
            set { toolStrip1.Visible = value; _enableToolStrip = value; }
        }
        private bool _enableToolStrip = true;

        [DefaultValue(true)]
        public bool CanAdd
        {
            get => _canAdd;
            set { buttonAdd.Visible = value; _canAdd = value; }
        }
        private bool _canAdd = true;

        [DefaultValue(true)]
        public bool CanRemove
        {
            get => _canRemove;
            set { buttonRemove.Visible = value; _canRemove = value; }
        }
        private bool _canRemove = true;

        [DefaultValue(true)]
        public bool CanMove
        {
            get => _canMove;
            set { buttonDown.Visible = value; buttonUp.Visible = value; _canMove = value; }
        }
        private bool _canMove = true;

        [DefaultValue(true)]
        public bool CanClone
        {
            get => _canClone;
            set { buttonClone.Visible = value; _canClone = value; }
        }
        private bool _canClone = true;

        [DefaultValue(false)]
        public SelectionMode SelectionMode
        {
            get => elementList.SelectionMode;
            set => elementList.SelectionMode = value;
        }

        public int ItemHeight { get => elementList.ItemHeight; set => elementList.ItemHeight = value; }

        public ushort ImageWidth { get; set; } = 24;

        public ushort ImageHeight { get; set; } = 24;

        public bool DisplayItemImages { get; set; } = false;

        public bool DisplayItemIndices { get; set; } = false;

        [DefaultValue(true)]
        public bool EnablePropertyView
        {
            get => _enablePropertyView;
            set
            {
                propertyGrid.Visible = value;
                _enablePropertyView = value;
                if (value)
                {
                    elementList.Dock = DockStyle.Top;
                    splitter1.Visible = true;
                }
                else
                {
                    elementList.Dock = DockStyle.Fill;
                    splitter1.Visible = false;
                }
            }
        }
        private bool _enablePropertyView = true;

        public bool EnablePropertyViewDescription { get => propertyGrid.HelpVisible; set => propertyGrid.HelpVisible = value; }

        public bool IsResetting { get; internal set; } = false;

        public bool InsertCloneAfterSelected { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public void DisableAllControls()
        {
            CanAdd = false;
            CanRemove = false;
            CanMove = false;
            CanClone = false;
        }

        /// <summary>
        /// Starting offset for item index display
        /// </summary>
        public int ItemIndexOffset { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ListBox.ObjectCollection GetItems()
        {
            return elementList.Items;
        }

        private readonly BindingList<object> Items = new();

        /// <summary>
        /// 
        /// </summary>
        private void MakeChanges()
        {
            if (Property != null)
            {
                MethodInfo method = GetType().GetMethod("ToArray").MakeGenericMethod(new Type[] { Property.PropertyType.GetElementType() });
                Property.SetValue(_object, method.Invoke(this, new object[] { }));
                OnArrayUpdated(EventArgs.Empty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T[] ToArray<T>()
        {
            T[] o = new T[Items.Count];

            for (int i = 0; i < Items.Count; i++)
                o[i] = (T)Items[i];

            return o;
        }

        /// <summary>
        /// 
        /// </summary>
        public ArrayMemberEditor()
        {
            InitializeComponent();

            elementList.DataSource = Items;
        }

        /// <summary>
        /// Sets the array property to be edited
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        public void SetArrayFromProperty(object obj, string propertyName)
        {
            if (obj == null)
            {
                Object = null;
                Property = null;
                return;
            }
            Property = obj.GetType().GetProperty(propertyName);
            if (Property != null && Property.PropertyType.IsArray)
            {
                Object = obj;
                OnArrayUpdated(EventArgs.Empty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            elementList.BeginUpdate();
            IsResetting = true;
            Items.Clear();
            if (_object != null)
                foreach (object obj in (object[])Property.GetValue(_object))
                    Items.Add(obj);
            IsResetting = false;
            elementList.EndUpdate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void elementList_SelectedValueChanged(object sender, EventArgs e)
        {
            if (elementList.SelectedItems != null && elementList.SelectedItems.Count > 0)
            {
                object[] selected = new object[elementList.SelectedItems.Count];
                for (int i = 0; i < selected.Length; i++)
                    selected[i] = elementList.SelectedItems[i];
                propertyGrid.SelectedObjects = selected;
            }
            else
                propertyGrid.SelectedObject = elementList.SelectedItem;

            if (!IsResetting)
                OnSelectedObjectChanged(EventArgs.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            // refresh display
            Items[elementList.SelectedIndex] = Items[elementList.SelectedIndex];

            // save changes
            MakeChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (Property == null)
                return;

            RemoveAt(elementList.SelectedIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (Property == null)
                return;

            object ob = Activator.CreateInstance(Property.PropertyType.GetElementType());
            if (ob != null)
            {
                OnItemAdded?.Invoke(new AddedItemEventArgs() { Item = ob });
                if (InsertCloneAfterSelected && SelectedIndex != -1)
                    Items.Insert(SelectedIndex + 1, ob);
                else
                    Items.Add(ob);
                MakeChanges();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonUp_Click(object sender, EventArgs e)
        {
            if (Property == null)
                return;

            if (elementList.SelectedIndex > 0)
            {
                elementList.BeginUpdate();

                int i = elementList.SelectedIndex;
                object moveItem = elementList.Items[i];
                Items.Remove(moveItem);
                Items.Insert(i - 1, moveItem);
                elementList.SetSelected(i - 1, true);

                elementList.EndUpdate();
                MakeChanges();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDown_Click(object sender, EventArgs e)
        {
            if (Property == null)
                return;

            if (elementList.SelectedIndex != -1 && elementList.SelectedIndex + 1 < elementList.Items.Count)
            {
                elementList.BeginUpdate();

                int i = elementList.SelectedIndex;

                object moveItem = elementList.Items[i];
                Items.Remove(moveItem);
                Items.Insert(i + 1, moveItem);
                elementList.SetSelected(i + 1, true);

                elementList.EndUpdate();
                MakeChanges();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void elementList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OnDoubleClickNode(EventArgs.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSelectedObjectChanged(EventArgs e)
        {
            EventHandler handler = SelectedObjectChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDoubleClickNode(EventArgs e)
        {
            EventHandler handler = DoubleClickedNode;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnArrayUpdated(EventArgs e)
        {
            EventHandler handler = ArrayUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        public int AddItem(object o)
        {
            if (o == null)
                return -1;
            OnItemAdded?.Invoke(new AddedItemEventArgs() { Item = o });
            if (InsertCloneAfterSelected && SelectedIndex != -1)
                Items.Insert(SelectedIndex + 1, o);
            else
                Items.Add(o);
            MakeChanges();
            return Items.Count - 1 + ItemIndexOffset;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object GetItemAt(int index)
        {
            if (index >= ItemIndexOffset && index < ItemIndexOffset + GetItems().Count)
                return GetItems()[index - ItemIndexOffset];

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClone_Click(object sender, EventArgs e)
        {
            if (elementList.SelectedItem == null)
                return;

            object clone = ObjectExtensions.Copy(elementList.SelectedItem);

            if (InsertCloneAfterSelected && SelectedIndex != -1)
                Items.Insert(SelectedIndex + 1, clone);
            else
                Items.Add(clone);
            elementList.SelectedItems.Clear();
            elementList.SelectedItem = clone;

            MakeChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void elementList_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            // Get the text of the item
            string itemText = ((ListBox)sender).Items[e.Index].ToString();

            // Measure the size of the text using the Graphics object
            SizeF textSize = e.Graphics.MeasureString(itemText, elementList.Font);

            // Set the item height to be the height of the text
            e.ItemHeight = (int)Math.Max(textSize.Height, ImageHeight);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void elementList_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                e.DrawBackground();

                Brush brush = ApplicationSettings.SystemWindowTextColorBrush;

                string itemText = ((ListBox)sender).Items[e.Index].ToString();

                if (e.Index < TextOverrides.Count && !string.IsNullOrEmpty(TextOverrides[e.Index]))
                    itemText = TextOverrides[e.Index];

                if (string.IsNullOrEmpty(itemText))
                    itemText = "-";

                int offset = 0;

                if (DisplayItemIndices)
                {
                    string indText = (e.Index + ItemIndexOffset).ToString() + ".";
                    Size indSize = TextRenderer.MeasureText(indText, e.Font);

                    Rectangle indexBound = new(e.Bounds.X + offset, e.Bounds.Y, indSize.Width, indSize.Height);

                    if (e.State == DrawItemState.Selected)
                        using (SolidBrush selectedColor = new(Color.White))
                            e.Graphics.DrawString(indText, e.Font, selectedColor, indexBound, StringFormat.GenericDefault);
                    else
                        e.Graphics.DrawString(indText, e.Font, ApplicationSettings.SystemGrayTextColorBrush, indexBound, StringFormat.GenericDefault);

                    offset += indSize.Width;
                }

                if (DisplayItemImages && ((ListBox)sender).Items[e.Index] is ImageArrayItem imageitem)
                {
                    using Image img = imageitem.ToImage();
                    if (img != null)
                    {
                        Rectangle indexBound = new(e.Bounds.X + offset, e.Bounds.Y, ImageWidth, ImageHeight);

                        e.Graphics.DrawImage(img, indexBound);

                        offset += ImageWidth;
                    }
                }

                Rectangle textBound = new(e.Bounds.X + offset, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
                e.Graphics.DrawString(itemText, e.Font, brush, textBound, StringFormat.GenericDefault);

                e.DrawFocusRectangle();
            }
            catch
            {

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        public void SelectObject(object o, bool multi = false)
        {
            if (o == null)
            {
                elementList.SelectedItems.Clear();
            }
            else
            if (elementList.Items.Contains(o))
            {
                if (!multi)
                    elementList.SelectedItems.Clear();

                if (!elementList.SelectedItems.Contains(o))
                    elementList.SelectedItem = o;
                else
                    elementList.SelectedItems.Remove(o);
            }
        }

        /// <summary>
        /// Removes item at index
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            if (index != -1)
            {
                if (AskBeforeDelete && MessageBox.Show($"Are you sure you want to delete {Items[index]}?", "Are you sure?", MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                    return;

                OnItemRemove?.Invoke(new RemovedItemEventArgs() { Index = index });
                Items.RemoveAt(index);
                MakeChanges();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public int IndexOf(object o)
        {
            return Items.IndexOf(o);
        }

        /// <summary>
        /// 
        /// </summary>
        public new void ResetBindings()
        {
            Items.ResetBindings();
        }
    }

    public class RemovedItemEventArgs : EventArgs
    {
        public int Index { get; set; }
    }

    public class AddedItemEventArgs : EventArgs
    {
        public object Item { get; set; }
    }

    public interface ImageArrayItem : IDisposable
    {
        Image ToImage();
    }

    public class TOBJProxy : ImageArrayItem
    {
        public HSD_TOBJ TOBJ = new() { SX = 1, SY = 1, SZ = 1 };

        /// <summary>
        /// 
        /// </summary>
        public TOBJProxy()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tobj"></param>
        public TOBJProxy(HSD_TOBJ tobj)
        {
            TOBJ = tobj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Image ToImage()
        {
            if (TOBJ != null)
                return TOBJ.ToImage().ToBitmap();
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "";
        }
    }
}
