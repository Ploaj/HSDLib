using System;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;

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
        
        public object SelectedObject => elementList.SelectedItem;

        public event EventHandler SelectedObjectChanged;
        public event EventHandler DoubleClickedNode;
        public event EventHandler ArrayUpdated;

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
            T[] o = new T[elementList.Items.Count];

            for (int i = 0; i < elementList.Items.Count; i++)
                o[i] = (T)elementList.Items[i];

            return o;
        }

        /// <summary>
        /// 
        /// </summary>
        public ArrayMemberEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the array property to be edited
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        public void SetArrayFromProperty(object obj, string propertyName)
        {
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
        private void Reset()
        {
            elementList.Items.Clear();
            elementList.Items.AddRange((object[])Property.GetValue(_object));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void elementList_SelectedValueChanged(object sender, EventArgs e)
        {
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
            elementList.Items[elementList.SelectedIndex] = elementList.Items[elementList.SelectedIndex];
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
            if (elementList.SelectedIndex != -1)
            {
                elementList.Items.RemoveAt(elementList.SelectedIndex);
                MakeChanges();
            }
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
                elementList.Items.Add(ob);
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
                elementList.Items.Remove(moveItem);
                elementList.Items.Insert(i - 1, moveItem);
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
                elementList.Items.Remove(moveItem);
                elementList.Items.Insert(i + 1, moveItem);
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
            elementList.Items.Add(o);
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

            elementList.Items.Add(ObjectExtensions.Copy(elementList.SelectedItem));

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
    }
}
