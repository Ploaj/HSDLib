using System;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Extra
{
    /// <summary>
    /// Single Popout Editor for SEM files
    /// </summary>
    public partial class SEMEditorTool : Form
    {
        private readonly SEMEditor _semeditor;

        public SEMEditorTool()
        {
            InitializeComponent();
            _semeditor = new SEMEditor();
            _semeditor.Dock = DockStyle.Fill;
            Controls.Add(_semeditor);
            _semeditor.BringToFront();

            CenterToScreen();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void OpenSEMFile(string path)
        {
            _semeditor.OpenSEMFile(path);
            //smStdatToolStripMenuItem1.Enabled = true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        private void SaveSEMFile(string path)
        {
            _semeditor.SaveSEMFile(path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openSEMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string f = Tools.FileIO.OpenFile("SEM (*.sem)|*.sem");
            if (f != null)
            {
                OpenSEMFile(f);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportSEMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string f = Tools.FileIO.SaveFile("SEM (*.sem)|*.sem");
            if (f != null)
            {
                SaveSEMFile(f);
            }
        }

        //private HSDRawFile SoundTestDat;
        //private smSoundTestLoadData SoundTestData;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*if (SoundTestDat != null)
            {
                if (MessageBox.Show("Sound Test Already Loaded\nReload?", "Load Sound Test Dat", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
            }
            var f = FileIO.OpenFile(ApplicationSettings.HSDFileFilter, "SmSt.dat");
            if (f != null)
            {
                HSDRawFile db = new HSDRawFile(f);

                if (db.Roots.Count > 0 && db.Roots[0].Data is smSoundTestLoadData std)
                {
                    SoundTestDat = db;
                    SoundTestData = std;

                    var names = std.SoundNames;
                    var indices = std.SoundIDs;

                    for (int i = 0; i < indices.Length; i++)
                    {
                        var ei = indices[i] / 10000;
                        var si = indices[i] % 10000;

                        if (entryList.Items[ei] is SEMEntry entry && si < entry.Scripts.Length)
                            entry.Scripts[si].Name = names[i];
                    }

                    exportToolStripMenuItem.Enabled = true;
                    // TODO: enable rename button
                }
            }*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void exportSmStdatToolStripMenuItem_Click(object sender, EventArgs args)
        {
            /*if (SoundTestDat != null)
            {
                var f = FileIO.SaveFile(ApplicationSettings.HSDFileFilter, "SmSt.dat");
                if (f == null)
                    return;

                string[] groupNames = new string[entryList.Items.Count];
                int[] groupCounts = new int[entryList.Items.Count];
                List<string> soundNames = new List<string>();
                List<int> soundIndices = new List<int>();

                int eIndex = 0;
                foreach (SEMEntry e in entryList.Items)
                {
                    int sIndex = 0;
                    foreach (var s in e.Scripts)
                    {
                        soundNames.Add(s.Name);
                        soundIndices.Add(eIndex * 10000 + sIndex++);
                    }
                    groupNames[eIndex] = "GRPSFX_" + e.SoundBank.Name.Replace(".ssm", "");
                    groupCounts[eIndex] = e.Scripts.Length;
                    eIndex++;
                }

                SoundTestData.SoundBankNames = groupNames;
                SoundTestData.SoundBankCount = groupCounts;
                SoundTestData.SoundNames = soundNames.ToArray();
                SoundTestData.SoundIDs = soundIndices.ToArray();

                SoundTestDat.Save(f);
            }*/
        }

        /// <summary>
        /// 
        /// </summary>
        private class RenameBox
        {
            public string Name { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void renameButton_Click(object sender, EventArgs e)
        {
            /*if (scriptArrayEditor.SelectedObject is SEMScript sound)
            {
                var rn = new RenameBox();
                rn.Name = sound.Name;
                using (PropertyDialog d = new PropertyDialog("Rename", rn))
                {
                    if (d.ShowDialog() == DialogResult.OK)
                    {
                        sound.Name = rn.Name;
                    }
                }
            }*/
        }
    }
}
