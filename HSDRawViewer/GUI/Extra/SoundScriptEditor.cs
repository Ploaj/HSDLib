using System;
using System.Windows.Forms;
using HSDRawViewer.Sound;
using HSDRawViewer.Tools;

namespace HSDRawViewer.GUI.Extra
{
    public partial class SoundScriptEditor : UserControl
    {
        private SoundBankEditor _soundBankEditor;
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
                byte[] d;
                if (SEM.CompileSEMScript(scriptBox.Text, out d) == -1)
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
                var pitch = sound.GetOPCodeValue(0x0C);
                if (pitch == -1)
                    pitch = 0;
                var reverb = sound.GetOPCodeValue(0x10);
                if (reverb == -1)
                    reverb = 0;
                _soundBankEditor.PlaySound(pitch, reverb);
            }
        }
    }
}
