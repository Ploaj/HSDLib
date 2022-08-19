using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Controls
{
    public partial class DockablePropertyGrid : DockContent
    {

        public delegate void PropertyUpdated(object sender, PropertyValueChangedEventArgs args);
        public PropertyUpdated PropertyValueUpdated;

        public object SelectedObject { get => propertyGrid1.SelectedObject; }

        public object[] SelectedObjects { get => propertyGrid1.SelectedObjects; }

        public DockablePropertyGrid()
        {
            InitializeComponent();

            Text = "Properties";

            propertyGrid1.PropertyValueChanged += (sender, args) =>
            {
                PropertyValueUpdated?.Invoke(sender, args);
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

        public void SetObject(object o)
        {
            propertyGrid1.SelectedObject = o;
        }

        public void SetObjects(object[] o)
        {
            propertyGrid1.SelectedObjects = o;
        }
    }
}
