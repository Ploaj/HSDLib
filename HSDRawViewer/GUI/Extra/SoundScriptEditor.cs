using HSDRawViewer.Sound;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Extra
{
    public partial class SoundScriptEditor : UserControl
    {
        private readonly SoundBankEditor _soundBankEditor;
        private SEMEntry _entry;

        /// <summary>
        /// 
        /// </summary>
        public SoundScriptEditor()
        {
            InitializeComponent();
            _soundBankEditor = new SoundBankEditor();
            _soundBankEditor.Dock = DockStyle.Fill;
            _soundBankEditor.Visible = false;
            Controls.Add(_soundBankEditor);
            _soundBankEditor.BringToFront();

#if DEBUG

#else
            mushroomButton.Visible = false;
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        public void SetEntry(int entryIndex, SEMEntry entry)
        {
            scriptArrayEditor.ItemIndexOffset = 10000 * entryIndex;

            _entry = entry;

            RefreshEntry();
        }

        /// <summary>
        /// 
        /// </summary>
        private void RefreshEntry()
        {
            scriptArrayEditor.SetArrayFromProperty(_entry, "Scripts");

            if (_entry.SoundBank != null)
            {
                _soundBankEditor.Visible = true;
                _soundBankEditor.SetEntry(_entry);
                soundBox.Dock = DockStyle.Top;
            }
            else
            {
                _soundBankEditor.Visible = false;
                soundBox.Dock = DockStyle.Fill;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSaveScript_Click(object sender, EventArgs e)
        {
            if (scriptArrayEditor.SelectedObject is SEMScript sound)
            {
                if (SEM.CompileSEMScript(scriptBox.Text, out byte[] d) == -1)
                    sound.CommandData = d;
                scriptBox.Text = SEM.DecompileSEMScript(sound.CommandData);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void copyScriptButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(scriptBox.Text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pasteScriptButton_Click(object sender, EventArgs e)
        {
            IDataObject iData = Clipboard.GetDataObject();

            if (iData.GetDataPresent(DataFormats.Text))
            {
                scriptBox.Text = (String)iData.GetData(DataFormats.Text);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void scriptArrayEditor_SelectedObjectChanged(object sender, EventArgs e)
        {
            if (scriptArrayEditor.SelectedObject is SEMScript sound)
                scriptBox.Text = SEM.DecompileSEMScript(sound.CommandData);
        }

        /*private void soundList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Oemplus)
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
                soundList_DoubleClick(null, null);
            }
        }*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void soundList_DoubleClick(object sender, EventArgs e)
        {
            if (scriptArrayEditor.SelectedObject is SEMScript sound)
            {
                // TODO: figure out how these values are stored
                int pitch = sound.GetOPCodeValue(0x0C);
                if (pitch == -1)
                    pitch = 0;
                int reverb = sound.GetOPCodeValue(0x10);
                if (reverb == -1)
                    reverb = 0;
                _soundBankEditor.PlaySound(pitch, reverb);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mushroomButton_Click(object sender, EventArgs e)
        {
            SEMScript[] entries = _entry.Scripts;

            SEMScript[] newEntries = new SEMScript[((entries.Length - 1) * 3) + 1];
            newEntries[0] = entries[0];

            for (int i = 0; i < entries.Length - 1; i++)
            {
                SEMScript entry = entries[1 + i];

                // normal entry
                newEntries[1 + i * 3] = entry;

                Console.WriteLine($"{1 + i} -> {1 + i * 3}");

                // mushroom small -350
                short small_pitch = -350;
                byte[] small_pitch_command = new byte[] { 0x0C, 0x00, (byte)((small_pitch >> 8) & 0xFF), (byte)((small_pitch) & 0xFF) };
                SEMScript small = new();
                small.Name = entry.Name + "_small";
                small.CommandData = new byte[entry.CommandData.Length + 4];
                Array.Copy(entry.CommandData, 0, small.CommandData, 0, 8);
                Array.Copy(small_pitch_command, 0, small.CommandData, 8, 4);
                Array.Copy(entry.CommandData, 8, small.CommandData, 12, entry.CommandData.Length - 8);
                newEntries[1 + i * 3 + 1] = small;

                // mushroom big 450
                short big_pitch = 450;
                byte[] big_pitch_command = new byte[] { 0x0C, 0x00, (byte)((big_pitch >> 8) & 0xFF), (byte)((big_pitch) & 0xFF) };
                SEMScript big = new();
                big.Name = entry.Name + "_big";
                big.CommandData = new byte[entry.CommandData.Length + 4];
                Array.Copy(entry.CommandData, 0, big.CommandData, 0, 8);
                Array.Copy(big_pitch_command, 0, big.CommandData, 8, 4);
                Array.Copy(entry.CommandData, 8, big.CommandData, 12, entry.CommandData.Length - 8);
                newEntries[1 + i * 3 + 2] = big;
            }

            _entry.Scripts = newEntries;
            RefreshEntry();
        }
    }
}
