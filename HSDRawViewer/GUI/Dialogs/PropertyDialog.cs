using System;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Dialog
{
    public partial class PropertyDialog : Form
    {
        public PropertyDialog(string name, object propertyObject)
        {
            InitializeComponent();

            CenterToScreen();
            // TopMost = true;

            Text = name;
            propertyGrid1.SelectedObject = propertyObject;
            propertyGrid1.ExpandAllGridItems();
        }

        public PropertyDialog(string name, object[] propertyObjects)
        {
            InitializeComponent();

            CenterToScreen();

            Text = name;
            propertyGrid1.SelectedObjects = propertyObjects;
            propertyGrid1.ExpandAllGridItems();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
