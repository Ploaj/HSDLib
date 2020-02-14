using System;
using System.Windows.Forms;
using HSDRawViewer.Tools;
using System.IO;
using HSDRawViewer.Sound;

namespace HSDRawViewer.GUI.Extra
{
    public partial class DSPViewer : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public DSP DSP { get => _dsp; set
            {
                _dsp = value;

                propertyGrid.SelectedObject = _dsp;

                if (_dsp != null)
                {
                    toolStrip1.Enabled = true;

                    channelBox.DataSource = _dsp.Channels;
                }
                else
                {
                    toolStrip1.Enabled = false;
                    channelBox.DataSource = null;
                    propertyGrid.SelectedObject = null;
                }
            }
        }
        private DSP _dsp;

        private static string Supported = "Supported Types (*.wav*.dsp)|*.wav;*.dsp";

        /// <summary>
        /// 
        /// </summary>
        public DSPViewer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void playButton_Click(object sender, EventArgs e)
        {
            PlaySound();
        }

        /// <summary>
        /// 
        /// </summary>
        public void PlaySound(float reverb = 0, float pitch = 0)
        {
            DSPPlayer.PlayDSP(DSP, reverb, pitch);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopButton_Click(object sender, EventArgs e)
        {
            DSPPlayer.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void replaceButton_CLick(object sender, EventArgs e)
        {
            var file = FileIO.OpenFile("DSP (*.dsp*.wav)|*.dsp;*.wav");

            if (file != null)
            {
                if (file.ToLower().EndsWith(".dsp"))
                    DSP.FromDSP(file);

                if (file.ToLower().EndsWith(".wav"))
                    DSP.FromWAVE(File.ReadAllBytes(file));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportButton_Click(object sender, EventArgs e)
        {
            var file = FileIO.SaveFile(Supported);

            if (file != null)
            {
                if (file.EndsWith(".dsp"))
                    DSP.ExportDSP(file);

                if (file.EndsWith(".wav"))
                    File.WriteAllBytes(file, DSP.ToWAVE());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void channelBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (channelBox.SelectedItem is DSPChannel channel)
            {
                propertyGrid.SelectedObjects = new object[] { channel };
            }
        }
    }
}
