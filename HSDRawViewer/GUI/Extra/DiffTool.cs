using HSDRawViewer.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Extra
{
    public partial class DiffTool : Form
    {
        public DiffTool()
        {
            InitializeComponent();
        }

        private void DiffTool_Load(object sender, EventArgs e)
        {

        }

        // TODO: I forgot to change the buttons reference names before double clicking them. oops?
        private void button1_Click(object sender, EventArgs e)
        {
             this.saveOriginalFileInput.Text = Tools.FileIO.OpenFile(FileIO.NORMAL_EXTENSIONS);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.saveModifiedFileInput.Text = Tools.FileIO.OpenFile(FileIO.NORMAL_EXTENSIONS);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.saveDiffFileInput.Text = Tools.FileIO.SaveFile(FileIO.DIFF_EXTENSIONS);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.mergeOriginalFileInput.Text = Tools.FileIO.OpenFile(FileIO.NORMAL_EXTENSIONS);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.mergeDiffFileInput.Text = Tools.FileIO.OpenFile(FileIO.DIFF_EXTENSIONS);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.mergeFileInput.Text = Tools.FileIO.SaveFile(FileIO.NORMAL_EXTENSIONS);
        }

        private void saveDiffButton_Click(object sender, EventArgs e)
        {
            if (this.saveOriginalFileInput.Text == null) return;
            if (this.saveModifiedFileInput.Text == null) return;
            if (this.saveDiffFileInput.Text == null) return;

            using (FileStream origStream = new FileStream(this.saveOriginalFileInput.Text, FileMode.Open, FileAccess.Read))
            using (FileStream modifiedStream = new FileStream(this.saveModifiedFileInput.Text, FileMode.Open, FileAccess.Read))
            using (FileStream diffStream = new FileStream(this.saveDiffFileInput.Text, FileMode.Create, FileAccess.Write))
            {
                FileIO.SaveDiffToFile(origStream, modifiedStream, diffStream);
                MessageBox.Show("Diff Saved!");
            }
        }

        private void mergeDiffButton_Click(object sender, EventArgs e)
        {
            if (this.mergeOriginalFileInput.Text == null) return;
            if (this.mergeDiffFileInput.Text == null) return;
            if (this.mergeFileInput.Text == null) return;


            using (FileStream origStream = new FileStream(this.mergeOriginalFileInput.Text, FileMode.Open, FileAccess.Read))
            using (FileStream modifiedStream = new FileStream(this.mergeDiffFileInput.Text, FileMode.Open, FileAccess.Read))
            using (FileStream mergedStream = new FileStream(this.mergeFileInput.Text, FileMode.Create, FileAccess.Write))
            {
                FileIO.MergeDiffToDat(origStream, modifiedStream, mergedStream);
                MessageBox.Show("Merged Diff and saved to: "+ this.mergeFileInput.Text);
            }
        }
    }
}
