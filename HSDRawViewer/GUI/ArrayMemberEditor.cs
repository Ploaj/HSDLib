using System;
using System.Windows.Forms;
using System.Reflection;

namespace HSDRawViewer.GUI
{
    public partial class ArrayMemberEditor : UserControl
    {
        private PropertyInfo Property { get; set; }

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

        public void EnablePropertyViewer(bool enable)
        {
            propertyGrid.Visible = enable;
        }

        public void EnablePropertyViewerDescription(bool enable)
        {
            propertyGrid.HelpVisible = enable;
        }

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
    }
}
