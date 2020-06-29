using System;
using System.ComponentModel;
using System.Windows.Forms;
using HSDRawViewer.Sound;
using System.Collections;
using HSDRawViewer.Tools;
using System.Linq;

namespace HSDRawViewer.GUI.Extra
{
    public partial class SEMEditor : UserControl
    {
        private SoundScriptEditor _scriptEditor;

        /// <summary>
        /// 
        /// </summary>
        private BindingList<SEMEntry> Entries = new BindingList<SEMEntry>();

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

            entryList.DataSource = Entries;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void OpenSEMFile(string path)
        {
            Entries.Clear();
            var entries = SEM.ReadSEMFile(path);
            foreach (var v in entries)
            {
                Entries.Add(v);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void SaveSEMFile(string path)
        {
            SEM.SaveSEMFile(path, Entries.ToList(), null);
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
                Entries.Add(entry);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportPackage_Click(object sender, EventArgs e)
        {
            if (entryList.SelectedItem is SEMEntry _entry)
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
            if (entryList.SelectedItem is SEMEntry _entry)
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
                    Entries.Add(new SEMEntry()
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
            if (entryList.SelectedItem == null)
                return;

            _scriptEditor.SetEntry(entryList.SelectedIndex, entryList.SelectedItem as SEMEntry);
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
            listBox_Remove(entryList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox_Remove(ListBox listBox1)
        {
            object data = listBox1.SelectedItem;
            var ds = listBox1.DataSource as IList;
            if (ds != null)
                ds.Remove(data);
        }
    }
}
