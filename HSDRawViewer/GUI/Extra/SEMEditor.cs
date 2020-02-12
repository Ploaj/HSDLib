using HSDRawViewer.Sound;
using HSDRawViewer.Tools;
using System;
using System.Collections;
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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void OpenSEMFile(string path)
        {
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

        private class SEMName
        {
            public string Name { get; set; }
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
            dspViewer1.PlaySound();
        }
    }
}
