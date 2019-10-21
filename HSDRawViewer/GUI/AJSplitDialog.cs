using System;
using System.IO;
using System.Windows.Forms;

namespace HSDRawViewer.GUI
{
    public partial class AJSplitDialog : Form
    {
        public AJSplitDialog()
        {
            InitializeComponent();

            labelAJ.Text = "";
            labelFolder.Text = "";
        }

        private void buttonAJ_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog d = new OpenFileDialog())
            {
                d.Filter = "Melee Animation (*AJ.dat)|*AJ.dat";

                if(d.ShowDialog() == DialogResult.OK)
                {
                    labelAJ.Text = d.FileName;
                }
            }
        }

        private void buttonFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog d = new FolderBrowserDialog())
            {
                d.SelectedPath = Directory.GetCurrentDirectory();

                if (d.ShowDialog() == DialogResult.OK)
                {
                    labelFolder.Text = d.SelectedPath;
                }
            }
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            if(File.Exists(labelAJ.Text) && Directory.Exists(labelFolder.Text))
            {

            }
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            if (File.Exists(labelAJ.Text) && Directory.Exists(labelFolder.Text))
            {

            }
        }
    }
}
