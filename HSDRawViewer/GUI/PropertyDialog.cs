using System;
using System.Windows.Forms;

namespace HSDRawViewer.GUI
{
    public partial class PropertyDialog : Form
    {
        public PropertyDialog(string name, object propertyObject)
        {
            InitializeComponent();

            CenterToScreen();

            Text = name;
            propertyGrid1.SelectedObject = propertyObject;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
