using System;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
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

        public bool DisplayItemIndices { get; set; } = false;

        [DefaultValue(true)]
        public bool EnablePropertyView { get => _enablePropertyView; set { propertyGrid.Visible = value; _enablePropertyView = value; } }
        private bool _enablePropertyView = true;

        public bool EnablePropertyViewDescription { get => propertyGrid.HelpVisible; set => propertyGrid.HelpVisible = value; }


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

        private BindingList<object> Items = new BindingList<object>();

        /// <summary>
        /// 
        /// </summary>
        private void MakeChanges()
        {
            if(Property != null)
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
            if(obj == null)
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
            Items.Clear();
            foreach(var obj in (object[])Property.GetValue(_object))
                Items.Add(obj);
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
            RemoveAt(elementList.SelectedIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            var ob = Activator.CreateInstance(Property.PropertyType.GetElementType());
            if(ob != null)
            {
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
            if(elementList.SelectedIndex != -1)
            {
                elementList.BeginUpdate();

                var i = elementList.SelectedIndex;
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
            if (elementList.SelectedIndex != -1)
            {
                elementList.BeginUpdate();

                var i = elementList.SelectedIndex;

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
        public void AddItem(object o)
        {
            if (o == null)
                return;
            Items.Add(o);
            MakeChanges();
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

            Items.Add(ObjectExtensions.Copy(elementList.SelectedItem));

            elementList.SelectedIndex = Items.Count - 1;

            MakeChanges();
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

                var brush = ApplicationSettings.SystemWindowTextColorBrush;

                var itemText = ((ListBox)sender).Items[e.Index].ToString();

                if (e.Index < TextOverrides.Count && !string.IsNullOrEmpty(TextOverrides[e.Index]))
                    itemText = TextOverrides[e.Index];

                if (string.IsNullOrEmpty(itemText))
                    itemText = "-";

                if (DisplayItemIndices)
                {
                    var indText = (e.Index + ItemIndexOffset).ToString() + ".";

                    var indSize =  TextRenderer.MeasureText(indText, e.Font);
                    var indexBound = new Rectangle(e.Bounds.X, e.Bounds.Y, indSize.Width, indSize.Height);
                    var textBound = new Rectangle(e.Bounds.X + indSize.Width, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);

                    e.Graphics.DrawString(indText, e.Font, ApplicationSettings.SystemGrayTextColorBrush, indexBound, StringFormat.GenericDefault);
                    e.Graphics.DrawString(itemText, e.Font, brush, textBound, StringFormat.GenericDefault);
                }
                else
                    e.Graphics.DrawString(itemText, e.Font, brush, e.Bounds, StringFormat.GenericDefault);

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
            if (elementList.Items.Contains(o))
            {
                if(!multi)
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

}
