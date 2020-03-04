using System;
using System.Windows.Forms;
using HSDRawViewer.Tools;
using System.IO;
using HSDRawViewer.Sound;

namespace HSDRawViewer.GUI.Extra
{
    public partial class DSPViewer : UserControl
    {
        public bool ReplaceButtonVisible { get => buttonReplace.Visible; set => buttonReplace.Visible = value; }

        private DSPPlayer Player;

        public string SoundName
        {
            set
            {
                groupBox1.Text = "DSP Player: " + value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DSP DSP { get => _dsp; set
            {
                _dsp = value;

                if (Player != null)
                {
                    Player.LoadDSP(_dsp);
                    Player.Position = TimeSpan.Zero;
                    soundTrack.Maximum = Player.Length.Milliseconds;
                }
                
                propertyGrid.SelectedObject = _dsp;

                if (_dsp != null)
                {
                    toolStrip1.Enabled = true;

                    channelBox.Items.Clear();
                    channelBox.Items.Add(_dsp);
                    foreach(var c in _dsp.Channels)
                        channelBox.Items.Add(c);
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

        /// <summary>
        /// 
        /// </summary>
        public DSPViewer()
        {
            InitializeComponent();

            Player = new DSPPlayer();

            Player.PlaybackStopped += (sender, args) =>
            {
                if(_dsp != null && 
                _dsp.Channels.Count > 0 &&
                _dsp.Channels[0].LoopStart != 0 && 
                Player.Position == Player.Length)
                {
                    //var sec = (int)Math.Ceiling(_dsp.Channels[0].LoopStart / 2 / (double)_dsp.Frequency * 1.75f);
                    var mill = (int)(_dsp.Channels[0].LoopStart / 2 / (double)_dsp.Frequency * 1.75f * 1000);
                    Player.Position = TimeSpan.FromMilliseconds(mill);
                    PlaySound();
                }
                else
                    PauseSound();
            };

            Disposed += (sender, args) =>
            {
                Player.Dispose();
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void playButton_Click(object sender, EventArgs e)
        {
            if(sender == playButton)
                PlaySound();
            if (sender == pauseButton)
                PauseSound();
        }

        /// <summary>
        /// 
        /// </summary>
        public void PauseSound()
        {
            Player.Pause();
            playButton.Visible = true;
            pauseButton.Visible = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void PlaySound()
        {
            Player.Play();
            playButton.Visible = false;
            pauseButton.Visible = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopButton_Click(object sender, EventArgs e)
        {
            PauseSound();
            Player.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void replaceButton_CLick(object sender, EventArgs e)
        {
            var file = FileIO.OpenFile("DSP (*.dsp*.hps*.wav)|*.dsp;*.hps;*.wav");

            if (file != null)
            {
                DSP.FromFormat(File.ReadAllBytes(file), Path.GetExtension(file));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportButton_Click(object sender, EventArgs e)
        {
            var file = FileIO.SaveFile(DSP.SupportedExportFilter);

            if (file != null)
            {
                DSP.ExportFormat(file);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void channelBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            propertyGrid.SelectedObjects = new object[] { channelBox.SelectedItem };
        }

# region TrackSlider

        private bool _stopSliderUpdate;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeSpan position = Player.Position;
            TimeSpan length = Player.Length;
            if (position > length)
                length = position;

            label1.Text = "Time: " + String.Format(@"{0:mm\:ss} / {1:mm\:ss}", position, length);

            if (!_stopSliderUpdate &&
                length != TimeSpan.Zero && position != TimeSpan.Zero)
            {
                double perc = position.TotalMilliseconds / length.TotalMilliseconds * soundTrack.Maximum;
                soundTrack.Value = (int)perc;
            }
        }

        private void soundTrack_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _stopSliderUpdate = true;
        }

        private void soundTrack_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _stopSliderUpdate = false;
        }

        private void soundTrack_ValueChanged(object sender, EventArgs e)
        {
            if (_stopSliderUpdate)
            {
                double perc = soundTrack.Value / (double)soundTrack.Maximum;
                TimeSpan position = TimeSpan.FromMilliseconds(Player.Length.TotalMilliseconds * perc);
                Player.Position = position;
            }
        }

        #endregion

        private void buttonSetLoop_Click(object sender, EventArgs e)
        {
            _dsp.SetLoopFromTimeSpan(Player.Position);
            propertyGrid.SelectedObject = _dsp;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetEffects()
        {
            Player.LoadDSP(_dsp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reverb"></param>
        public void ApplyReverb(float reverb)
        {
            Player.ApplyReverb(reverb);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reverb"></param>
        public void ApplyPitch(float pitch)
        {
            Player.ApplyPitch(pitch);
        }
    }
}
