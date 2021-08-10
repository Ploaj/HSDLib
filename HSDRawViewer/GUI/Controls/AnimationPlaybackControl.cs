using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Controls
{
    public enum PlaybackMode
    {
        None,
        Forward,
        Reverse
    }

    public partial class AnimationPlaybackControl : UserControl
    {
        public class FrameSpeedModifier
        {
            public float StartFrame;
            public float OldRange;
            public float NewRange;
        }

        public bool CanEditStartAndEndTime { get => tbEndTime.Enabled; set
            {
                tbEndTime.Enabled = value;
                //tbStartTime.Enabled = value;
            }
        }

        /// <summary>
        /// The start frame for this track
        /// </summary>
        public float StartFrame
        {
            get => _startFrame;
            set
            {
                _startFrame = value;
                tbStartTime.Text = value.ToString();
            }
        }
        private float _startFrame;

        /// <summary>
        /// The end frame for this track
        /// </summary>
        public float EndFrame
        {
            get => _endFrame;
            set
            {
                _endFrame = value;
                tbEndTime.Text = value.ToString();
            }
        }
        private float _endFrame;

        /// <summary>
        /// The current frame 
        /// </summary>
        public float Frame
        {
            get => _frame;
            set
            {
                _frame = value;
                FrameChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        private float _frame;


        /// <summary>
        /// The current playback speed
        /// </summary>
        public float PlaybackSpeed { get; set; } = 1;


        /// <summary>
        /// Checks if track is currently playing
        /// </summary>
        public PlaybackMode PlaybackMode { get; internal set; }

        /// <summary>
        /// Enabled Frame Speed Modifier hooks
        /// </summary>
        public bool FSMEnabled { get; set; } = false;


        /// <summary>
        /// Executes when the frame value is changed
        /// </summary>
        public event EventHandler FrameChanged;

        private List<FrameSpeedModifier> Modifiers = new List<FrameSpeedModifier>();


        /// <summary>
        /// 
        /// </summary>
        public AnimationPlaybackControl()
        {
            InitializeComponent();

            // add idle hook
            Application.Idle += UpdatePlayer;

            // remove idle hook on dispose
            Disposed += (sender, args) =>
            {
                Application.Idle -= UpdatePlayer;
            };
        }

        private DateTime PrevTime = DateTime.Now;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void UpdatePlayer(object sender, object args)
        {
            var now = DateTime.Now;

            var elapsed = (now - PrevTime).Milliseconds;

            Console.WriteLine(elapsed);

            if (elapsed < 16)
                return;

            PrevTime = now;

            if (PlaybackMode == PlaybackMode.None)
                return;

            // process playback
            if (PlaybackMode == PlaybackMode.Forward)
            {
                if (Frame + 1 > EndFrame)
                    Frame = StartFrame;
                else
                    Frame += 1;
            }

            // process reverse playback
            if (PlaybackMode == PlaybackMode.Reverse)
            {
                if (Frame - 1 < StartFrame)
                    Frame = EndFrame;
                else
                    Frame -= 1;
            }

            // re-paint player
            panelPlayback.Invalidate();


        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panelPlayback_Paint(object sender, PaintEventArgs e)
        {
            var rect = e.ClipRectangle;

            using (var backgroundBrush = new LinearGradientBrush(
                new Point(0, 0),
                new Point(0, rect.Height),
                Color.FromArgb(255, 0, 16, 30),
                Color.FromArgb(255, 20, 40, 64)))
                e.Graphics.FillRectangle(backgroundBrush, e.ClipRectangle);

            if (StartFrame == EndFrame)
                return;

            // give at least 30 pixels as the spacing


            float numberOfFrames = EndFrame - StartFrame;
            var tickWidth = (rect.Width / numberOfFrames);
            var increment = 1;
            float tickStartY = rect.Height * 0.3f;

            // fix infinity
            if (numberOfFrames == 0)
                tickWidth = 1;

            // don't crunch numbers too far together
            if (tickWidth < 30)
                increment = (int)Math.Ceiling(30 / tickWidth);

            // round increment to upper 5
            if (increment != 1)
                increment = (int)(Math.Ceiling(increment / 5f) * 5f);

            // vertical
            using (Font drawFont = new Font("Courier New", 8))
            using (var fontBrush = new SolidBrush(Color.White))
            using (var linePen = new Pen(Color.White))
            {
                for (float i = 0; i <= numberOfFrames; i++)
                {
                    if (i % increment == 0)
                    {
                        var x = i * tickWidth;
                        e.Graphics.DrawLine(linePen, x, tickStartY, x, rect.Height);
                        e.Graphics.DrawString((i + StartFrame).ToString(), drawFont, fontBrush, x, 0);
                    }
                }
            }

            // selected frame
            using (var brush = new SolidBrush(Color.FromArgb(180, Color.LightBlue)))
            {
                e.Graphics.FillRectangle(brush, Frame * tickWidth, tickStartY, tickWidth, rect.Height);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdatePlayImages()
        {
            buttonPlayForward.Image = Properties.Resources.pb_play;
            buttonPlayReverse.Image = Properties.Resources.pb_play_reverse;

            if (PlaybackMode == PlaybackMode.Forward)
                buttonPlayForward.Image = Properties.Resources.pb_pause;

            if (PlaybackMode == PlaybackMode.Reverse)
                buttonPlayReverse.Image = Properties.Resources.pb_pause;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPlayForward_Click(object sender, EventArgs e)
        {
            if (PlaybackMode == PlaybackMode.Forward)
                PlaybackMode = PlaybackMode.None;
            else
                PlaybackMode = PlaybackMode.Forward;

            UpdatePlayImages();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPlayReverse_Click(object sender, EventArgs e)
        {
            if (PlaybackMode == PlaybackMode.Reverse)
                PlaybackMode = PlaybackMode.None;
            else
                PlaybackMode = PlaybackMode.Reverse;

            UpdatePlayImages();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panelPlayback_MouseDown(object sender, MouseEventArgs e)
        {
            float numberOfFrames = EndFrame - StartFrame;
            float mouse_x = e.X;

            Frame = (int)((mouse_x / panelPlayback.Width) * numberOfFrames);

            panelPlayback.Invalidate();
        }
    }
}
