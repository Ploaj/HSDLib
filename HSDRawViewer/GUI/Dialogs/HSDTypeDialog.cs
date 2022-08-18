using System;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Dialog
{
    public partial class HSDTypeDialog : Form
    {
        public Type HSDAccessorType { get => (Type)comboBoxType.SelectedItem; }

        public HSDTypeDialog()
        {
            InitializeComponent();

            foreach (var v in ApplicationSettings.HSDTypes)
            {
                comboBoxType.Items.Add(v);
            }
            comboBoxType.SelectedIndex = 0;

            CenterToScreen();
        }

        private void buttonOkay_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void comboBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
