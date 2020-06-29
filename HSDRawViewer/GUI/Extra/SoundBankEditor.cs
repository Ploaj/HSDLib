using System;
using System.Windows.Forms;
using HSDRawViewer.Sound;
using HSDRawViewer.Tools;

namespace HSDRawViewer.GUI.Extra
{
    public partial class SoundBankEditor : UserControl
    {
        private SEMEntry _entry;

        /// <summary>
        /// 
        /// </summary>
        public SoundBankEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        public void SetEntry(SEMEntry entry)
        {
            _entry = entry;
            soundArrayEditor.SetArrayFromProperty(entry.SoundBank, "Sounds");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSoundBankAdd_Click(object sender, EventArgs e)
        {
            if (_entry == null)
                return;

            var files = FileIO.OpenFiles(DSP.SupportedImportFilter);

            if (files != null)
            {
                foreach (var file in files)
                {
                    var dsp = new DSP();

                    dsp.FromFile(file);

                    soundArrayEditor.AddItem(dsp);
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
            if (_entry == null)
                return;

            if (MessageBox.Show("Are you sure?", "Delete Sound", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) == DialogResult.Yes && soundArrayEditor.SelectedIndex != -1)
            {
                // index to delete
                var index = soundArrayEditor.SelectedIndex;

                soundArrayEditor.RemoveAt(index);

                // adjust ids > index to be less and id at index to be 0?
                foreach (SEMScript s in _entry.Scripts)
                {
                    if (s.SoundCommandIndex == index)
                        s.SoundCommandIndex = 0;

                    if (s.SoundCommandIndex > index)
                        s.SoundCommandIndex -= 1; ;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void soundBankList_MouseDoubleClick(object sender, EventArgs e)
        {
            if (soundArrayEditor.SelectedObject is DSP dsp)
                dspViewer1.PlaySound();
        }

        /// <summary>
        /// 
        /// </summary>
        public void PlaySound(int pitch, int reverb)
        {
            //dspViewer1.ApplyReverb(reverb);
            //dspViewer1.ApplyPitch(1 + pitch / 1200f);
            dspViewer1.PlaySound();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void soundBankList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (soundArrayEditor.SelectedObject is DSP dsp)
                dspViewer1.DSP = dsp;
        }
    }
}
