using HSDRaw;
using HSDRaw.Melee;
using HSDRawViewer.Sound;
using HSDRawViewer.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Extra
{
    public partial class SEMEditor : Form
    {
        private BindingList<SEMEntry> Entries = new BindingList<SEMEntry>();

        public SEMEditor()
        {
            InitializeComponent();

            entryList.DataSource = Entries;
            
            CenterToScreen();

            FormClosed += (sender, args) =>
            {
                DSPPlayer.Stop();
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void OpenSEMFile(string path)
        {
            soundList.DataSource = null;
            soundBankList.DataSource = null;
            dspViewer1.DSP = null;
            Entries.Clear();
            var entries = SEM.ReadSEMFile(path);
            foreach (var v in entries)
            {
                Entries.Add(v);
            }
            smStdatToolStripMenuItem.Enabled = true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        private void SaveSEMFile(string path)
        {
            SEM.SaveSEMFile(path, Entries.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openSEMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFile("SEM (*.sem)|*.sem");
            if(f != null)
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
            var f = Tools.FileIO.SaveFile("SEM (*.sem)|*.sem");
            if (f != null)
            {
                SaveSEMFile(f);
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
            soundList.DataSource = (entryList.SelectedItem as SEMEntry).Sounds;
            soundBankList.DataSource = (entryList.SelectedItem as SEMEntry).SoundBank?.Sounds;
            dspViewer1.DSP = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void soundList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectSound();
        }

        private void SelectSound()
        {
            if (soundList.SelectedItem is SEMSound sound &&
                entryList.SelectedItem is SEMEntry entry)
            {
                scriptBox.Text = SEM.DecompileSEMScript(sound.CommandData);

                if (entry.SoundBank != null && sound.SoundCommandIndex > -1 && sound.SoundCommandIndex < entry.SoundBank.Sounds.Count)
                    dspViewer1.DSP = entry.SoundBank.Sounds[sound.SoundCommandIndex];
            }
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
            if(ds != null)
                ds.Remove(data);
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
        private void buttonRemoveSound_Click(object sender, EventArgs e)
        {
            listBox_Remove(soundList);
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
                        Index = Entries.Count,
                        SoundBank = new SSM()
                        {
                            Name = name.Name.ToLower().EndsWith(".ssm") ? name.Name : name.Name + ".ssm"
                        },
                        Sounds = new BindingList<SEMSound>()
                    });
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddSound_Click(object sender, EventArgs e)
        {
            object data = soundList.SelectedItem;
            var ds = soundList.DataSource as IList;
            if (ds != null)
            {
                ds.Add(new SEMSound()
                {
                    Index = ds.Count + entryList.SelectedIndex * 10000,
                    Name = "SFX_", 
                    CommandData = new byte[] { 0x01, 0, 0, 0, 0x0E, 0, 0, 0}
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSaveScript_Click(object sender, EventArgs e)
        {
            if (soundList.SelectedItem is SEMSound sound)
            {
                byte[] d;
                if(SEM.CompileSEMScript(scriptBox.Text, out d) == -1)
                    sound.CommandData = d;
                scriptBox.Text = SEM.DecompileSEMScript(sound.CommandData);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void soundBankList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(soundBankList.SelectedItem is DSP dsp)
                DSPPlayer.PlayDSP(dsp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void soundBankList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (soundBankList.SelectedItem is DSP dsp)
                dspViewer1.DSP = dsp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void soundList_DataSourceChanged(object sender, EventArgs e)
        {
            var index = 0;
            if(soundList.DataSource != null)
            foreach(var v in soundList.DataSource as BindingList<SEMSound>)
            {
                v.Index = entryList.SelectedIndex * 10000 + index++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void soundBankList_DataSourceChanged(object sender, EventArgs e)
        {
            var index = 0;
            if (soundBankList.DataSource != null)
                foreach (var v in soundBankList.DataSource as BindingList<DSP>)
                {
                    v.Index = index++;
                }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonMoveUp_Click(object sender, EventArgs e)
        {
            if(soundList.SelectedItem != null)
            {
                var sounds = soundList.DataSource as BindingList<SEMSound>;

                var index = soundList.SelectedIndex;
                if (index - 1 < 0)
                    return;
                var sou = sounds[index];

                soundList.BeginUpdate();
                sounds.RemoveAt(index);
                sounds.Insert(index - 1, sou);

                soundList.DataSource = null;
                soundList.DataSource = sounds;
                soundList.SelectedIndex = index - 1;
                
                soundList.EndUpdate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonMoveDown_Click(object sender, EventArgs e)
        {
            if (soundList.SelectedItem != null)
            {
                var sounds = soundList.DataSource as BindingList<SEMSound>;

                var index = soundList.SelectedIndex;
                if (index + 1 >= sounds.Count)
                    return;
                var sou = sounds[index];

                soundList.BeginUpdate();
                sounds.RemoveAt(index);
                sounds.Insert(index + 1, sou);
                
                soundList.DataSource = null;
                soundList.DataSource = sounds;
                soundList.SelectedIndex = index + 1;

                soundList.EndUpdate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSoundBankAdd_Click(object sender, EventArgs e)
        {
            var files = FileIO.OpenFiles("Supported Formats (*.dsp, *.wav, *.hps)|*.dsp;*.wav;*.hps");

            if (files != null)
            {
                foreach (var file in files)
                {
                    var dsp = new DSP();

                    if (file.ToLower().EndsWith(".dsp"))
                        dsp.FromDSP(file);

                    if (file.ToLower().EndsWith(".wav"))
                        dsp.FromWAVE(File.ReadAllBytes(file));

                    if (file.ToLower().EndsWith(".hps"))
                        dsp.FromHPS(File.ReadAllBytes(file));

                    (entryList.SelectedItem as SEMEntry)?.SoundBank?.Sounds.Add(dsp);

                    // refresh
                    var temp = soundBankList.DataSource as BindingList<DSP>;
                    soundBankList.DataSource = null;
                    soundBankList.DataSource = temp;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSoundBankDelete_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are you sure?", "Delete Sound", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) == DialogResult.Yes && soundBankList.SelectedIndex != -1)
            {
                // index to delete
                var index = soundBankList.SelectedIndex;

                // remove sound from ssm
                (entryList.SelectedItem as SEMEntry)?.SoundBank?.Sounds.RemoveAt(index);

                // adjust ids > index to be less and id at index to be 0?
                foreach(SEMSound s in soundList.Items)
                {
                    if (s.SoundCommandIndex == index)
                        s.SoundCommandIndex = 0;

                    if (s.SoundCommandIndex > index)
                        s.SoundCommandIndex -= 1; ;
                }

                // refresh indices
                var temp = soundBankList.DataSource as BindingList<DSP>;
                soundBankList.DataSource = null;
                soundBankList.DataSource = temp;

                SelectSound();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void soundList_DoubleClick(object sender, EventArgs e)
        {
            if(soundList.SelectedItem is SEMSound sound)
            {
                // TODO: figure out how these values are stored
                var pitch = sound.GetOPCodeValue(0x0C);
                if (pitch == -1)
                    pitch = 0;
                var reverb = sound.GetOPCodeValue(0x10);
                if (reverb == -1)
                    reverb = 0;
                dspViewer1.PlaySound(reverb / 255f, 1 + pitch / ((float)short.MaxValue));
            }
        }

        private HSDRawFile SoundTestDat;
        private smSoundTestLoadData SoundTestData;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(SoundTestDat != null)
            {
                if (MessageBox.Show("Sound Test Already Loaded\nReload?", "Load Sound Test Dat", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
            }
            var f = FileIO.OpenFile(ApplicationSettings.HSDFileFilter, "SmSt.dat");
            if (f != null)
            {
                HSDRawFile db = new HSDRawFile(f);

                if(db.Roots.Count > 0 && db.Roots[0].Data is smSoundTestLoadData std)
                {
                    SoundTestDat = db;
                    SoundTestData = std;

                    var names = std.SoundNames;

                    int index = 0;
                    foreach(SEMEntry entry in entryList.Items)
                    {
                        foreach(var v in entry.Sounds)
                        {
                            if (index > names.Length)
                                break;
                            v.Name = names[index++];
                        }
                    }

                    (entryList.SelectedItem as SEMEntry).Sounds.ResetBindings();

                    exportSmStdatToolStripMenuItem.Enabled = true;
                    renameButton.Enabled = true;
                }
            }
        }

        private void exportSmStdatToolStripMenuItem_Click(object sender, EventArgs args)
        {
            if(SoundTestDat != null)
            {
                var f = FileIO.SaveFile(ApplicationSettings.HSDFileFilter, "SmSt.dat");
                if (f == null)
                    return;

                string[] groupNames = new string[entryList.Items.Count];
                int[] groupCounts = new int[entryList.Items.Count];
                List<string> soundNames = new List<string>();
                List<int> soundIndices = new List<int>();

                int eIndex = 0;
                foreach(SEMEntry e in entryList.Items)
                {
                    int sIndex = 0;
                    foreach (var s in e.Sounds)
                    {
                        soundNames.Add(s.Name);
                        soundIndices.Add(eIndex * 10000 + sIndex++);
                    }
                    groupNames[eIndex] = "GRPSFX_" + e.SoundBank.Name.Replace(".ssm", "");
                    groupCounts[eIndex] = e.Sounds.Count;
                    eIndex++;
                }

                SoundTestData.SoundBankNames = groupNames;
                SoundTestData.SoundBankCount = groupCounts;
                SoundTestData.SoundNames = soundNames.ToArray();
                SoundTestData.SoundIDs = soundIndices.ToArray();

                SoundTestDat.Save(f);
            }
        }

        private class RenameBox
        {
            public string Name { get; set; }
        }

        private void renameButton_Click(object sender, EventArgs e)
        {
            if(soundList.SelectedItem is SEMSound sound)
            {
                var rn = new RenameBox();
                rn.Name = sound.Name;
                using(PropertyDialog d = new PropertyDialog("Rename", rn))
                {
                    if(d.ShowDialog() == DialogResult.OK)
                    {
                        sound.Name = rn.Name;
                        (entryList.SelectedItem as SEMEntry).Sounds.ResetBindings();
                    }
                }
            }
        }

        private void soundList_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Oemplus)
            {
                buttonAddSound_Click(null, null);
            }
            if (e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Delete)
            {
                buttonRemoveSound_Click(null, null);
            }
            if (e.KeyCode == Keys.Enter)
            {
                renameButton_Click(null, null);
            }
            if (e.KeyCode == Keys.Space)
            {
                soundBankList_MouseDoubleClick(null, null);
            }
        }

        private void copyScriptButton_Click(object sender, EventArgs e)
        {
            scriptBox.Copy();
        }

        private void pasteScriptButton_Click(object sender, EventArgs e)
        {
            scriptBox.Paste();
        }

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
    }
}
