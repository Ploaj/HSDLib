using HSDRaw;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI
{
    public partial class PropertyView : DockContent
    {
        public PropertyView()
        {
            InitializeComponent();

            Text = "Property View";

            FormClosing += (sender, args) =>
            {
                if (args.CloseReason == CloseReason.UserClosing)
                {
                    args.Cancel = true;
                    MainForm.Instance.TryClose(this);
                }
            };
        }

        public void SetAccessor(HSDAccessor accessor)
        {
            if (accessor == null)
                return;

            propertyGrid1.SelectedObject = accessor;
            panel1.SetBytes(accessor._s.GetData());
        }

        private void propertyGrid1_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
        {
            if(propertyGrid1.SelectedObject is HSDAccessor accessor)
                panel1.SetBytes(accessor._s.GetData());
        }
    }
}
