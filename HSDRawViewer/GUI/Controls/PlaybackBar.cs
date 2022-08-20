using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Controls
{
    public class PlaybackBarFrameTip
    {
        public enum PlaybackBarFrameTipStyle
        {
            Color,
            Text
        }
        public enum PlaybackBarFrameTipLocation
        {
            Lower,
            Middle,
            Upper
        }

        public int Frame { get; set; }

        public PlaybackBarFrameTipStyle Style { get; set; } = PlaybackBarFrameTipStyle.Color;

        public PlaybackBarFrameTipLocation Location { get; set; } = PlaybackBarFrameTipLocation.Lower;

        public Color Color { get; set; } = Color.Red;

        public string Text { get; set; } = "0";
    }

    public partial class PlaybackBar : UserControl
    {
        public List<PlaybackBarFrameTip> FrameTips { get; internal set; } = new List<PlaybackBarFrameTip>();

        /// <summary>
        /// The start frame for this track
        /// </summary>
        public float StartFrame
        {
            get => _startFrame;
            set
            {
                _startFrame = value;
            }
        }
        private float _startFrame = 0;

        /// <summary>
        /// The end frame for this track
        /// </summary>
        public float EndFrame
        {
            get => _endFrame;
            set
            {
                _endFrame = value;
            }
        }
        private float _endFrame = 10;

        /// <summary>
        /// The current frame 
        /// </summary>
        public float Frame
        {
            get => _frame;
            set
            {
                _frame = value;

                if (ValueChanged != null)
                ValueChanged.Invoke(this, EventArgs.Empty);
                Invalidate();
            }
        }
        private float _frame = 0;


        public event EventHandler ValueChanged;

        /// <summary>
        /// 
        /// </summary>
        public PlaybackBar()
        {
            InitializeComponent();

            DoubleBuffered = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!Focused)
                Focus();
            base.OnMouseDown(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaybackBar_Paint(object sender, PaintEventArgs e)
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


            float numberOfFrames = EndFrame - StartFrame + 1;
            var tickWidth = (rect.Width / numberOfFrames);
            var increment = 1;
            float tickStartY = rect.Height * 0.4f;
            float tickTipHeight = (rect.Height - tickStartY) / 2;

            float tipUpperYStart = tickStartY;
            float tipMiddleYStart = tickStartY + tickTipHeight;
            float tipLowerYStart = tickStartY + tickTipHeight + tickTipHeight;

            RectangleF tipRect = new RectangleF(0, 0, tickWidth, tickTipHeight);

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
            using (var sublinePen = new Pen(Color.Gray))
            {
                for (float i = 0; i <= numberOfFrames; i++)
                {
                    // draw frame tip data
                    foreach (var tip in FrameTips)
                    {
                        if (tip.Frame == i)
                        {
                            tipRect.X = i * tickWidth;

                            switch (tip.Location)
                            {
                                case PlaybackBarFrameTip.PlaybackBarFrameTipLocation.Lower:
                                    tipRect.Y = tipLowerYStart;
                                    break;
                                case PlaybackBarFrameTip.PlaybackBarFrameTipLocation.Middle:
                                    tipRect.Y = tipMiddleYStart;
                                    break;
                                case PlaybackBarFrameTip.PlaybackBarFrameTipLocation.Upper:
                                    tipRect.Y = tipUpperYStart;
                                    break;
                            }

                            using (var brush = new SolidBrush(tip.Color))
                                switch (tip.Style)
                                {
                                    case PlaybackBarFrameTip.PlaybackBarFrameTipStyle.Color:
                                        e.Graphics.FillRectangle(brush, tipRect);
                                        break;
                                    case PlaybackBarFrameTip.PlaybackBarFrameTipStyle.Text:
                                        e.Graphics.DrawString(tip.Text, drawFont, brush, tipRect.X, tipRect.Y - 3);
                                        break;
                                }
                        }
                    }

                    var x = i * tickWidth;

                    if (i % increment == 0)
                    {
                        e.Graphics.DrawLine(linePen, x, tickStartY, x, rect.Height);
                        e.Graphics.DrawString((i + StartFrame).ToString(), drawFont, fontBrush, x, 0);
                    }
                    else
                    {
                        e.Graphics.DrawLine(sublinePen, x, tickStartY + tickTipHeight, x, rect.Height);
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaybackBar_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaybackBar_MouseDown(object sender, MouseEventArgs e)
        {
            PickSelectedFrame(e.X);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaybackBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button.HasFlag(MouseButtons.Left))
                PickSelectedFrame(e.X);

            Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        private void PickSelectedFrame(float x)
        {
            var rect = ClientRectangle;

            float numberOfFrames = EndFrame - StartFrame + 1;
            var tickWidth = (rect.Width / numberOfFrames);

            // fix infinity
            if (numberOfFrames == 0)
                tickWidth = 1;

            Frame = (float)Math.Floor(x / tickWidth);

            if (_frame < StartFrame)
                _frame = StartFrame;

            if (_frame > EndFrame)
                _frame = EndFrame;

            Invalidate();
        }
    }
}
