using System;
using System.Windows.Forms;
using HSDRaw.MEX;
using HSDRawViewer.GUI.Extra;
using HSDRawViewer.Tools;
using System.IO;

namespace HSDRawViewer.GUI.MEX.Controls
{
    public partial class MEXSemControl : UserControl, IMEXControl
    {
        private SEMEditor _editor;

        private MEX_Data _data;

        private string SemPath;

        /// <summary>
        /// 
        /// </summary>
        public MEXSemControl()
        {
            InitializeComponent();

            _editor = new SEMEditor();
            _editor.Dock = DockStyle.Fill;
            Controls.Add(_editor);
            _editor.BringToFront();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetControlName()
        {
            return "Sounds";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void LoadData(MEX_Data data)
        {
            _data = data;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetDataBindings()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void SaveData(MEX_Data data)
        {
            if(!string.IsNullOrEmpty(SemPath) && MessageBox.Show("Save SEM", "Save SEM and SSM files?", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                _editor.SaveSEMFile(SemPath, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openButton_Click(object sender, EventArgs e)
        {
            SemPath = null;

            var path = Path.Combine(Path.GetDirectoryName(MainForm.Instance.FilePath), "audio/us/smash2.sem");

            if (File.Exists(path))
                SemPath = path;
            else
                SemPath = FileIO.OpenFile("SEM (*.sem)|*.sem", "smash2.sem");

            if (SemPath != null && File.Exists(SemPath))
                _editor.OpenSEMFile(SemPath, _data);
        }
    }
}
