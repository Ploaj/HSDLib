using System;
using System.ComponentModel;
using System.Windows.Forms;
using HSDRawViewer.Sound;
using HSDRawViewer.Tools;
using System.Linq;
using HSDRaw.MEX;

namespace HSDRawViewer.GUI.Extra
{
    public partial class SEMEditor : UserControl
    {
        private SoundScriptEditor _scriptEditor;

        /// <summary>
        /// 
        /// </summary>
        public SEMEntry[] Entries { get; set; } = new SEMEntry[0];

        /// <summary>
        /// 
        /// </summary>
        public SEMEditor()
        {
            InitializeComponent();

            _scriptEditor = new SoundScriptEditor();
            _scriptEditor.Dock = DockStyle.Fill;
            Controls.Add(_scriptEditor);
            _scriptEditor.BringToFront();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void OpenSEMFile(string path, MEX_Data mex = null)
        {
            Entries = SEM.ReadSEMFile(path, true, mex).ToArray();

            entryList.SetArrayFromProperty(this, "Entries");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void SaveSEMFile(string path, MEX_Data mex = null)
        {
            SEM.SaveSEMFile(path, Entries.ToList(), mex);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importSPKG_Click(object sender, EventArgs e)
        {
            var f = FileIO.OpenFile("SEM Package (*.spk)|*.spk");
            if (f != null)
            {
                SEMEntry entry = new SEMEntry();
                entry.LoadFromPackage(f);
                entryList.AddItem(entry);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportPackage_Click(object sender, EventArgs e)
        {
            if (entryList.SelectedObject is SEMEntry _entry)
            {
                var f = FileIO.SaveFile("SEM Package (*.spk)|*.spk");
                if (f != null)
                    _entry.SaveAsPackage(f);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void replacePackageButton_Click(object sender, EventArgs e)
        {
            if (entryList.SelectedObject is SEMEntry _entry)
            {
                var f = FileIO.OpenFile("SEM Package (*.spk)|*.spk");
                if (f != null)
                {
                    _entry.LoadFromPackage(f);
                    _scriptEditor.SetEntry(entryList.SelectedIndex, _entry);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private class SEMName
        {
            [Description("Name of new SSM file")]
            public string Name { get; set; } = "newssm";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddEntry_Click(object sender, EventArgs e)
        {
            var name = new SEMName();
            using (PropertyDialog d = new PropertyDialog("SSM Name", name))
            {
                if (d.ShowDialog() == DialogResult.OK)
                {
                    entryList.AddItem(new SEMEntry()
                    {
                        SoundBank = new SSM()
                        {
                            Name = name.Name.ToLower().EndsWith(".ssm") ? name.Name : name.Name + ".ssm"
                        },
                        Scripts = new SEMScript[0]
                    });
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void entryList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (entryList.SelectedObject == null)
                return;

            _scriptEditor.SetEntry(entryList.SelectedIndex, entryList.SelectedObject as SEMEntry);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void entryList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Oemplus)
            {
                buttonAddEntry_Click(null, null);
            }
            if (e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Delete)
            {
                toolStripButton2_Click(null, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            entryList.RemoveAt(entryList.SelectedIndex);
        }
    }
}
