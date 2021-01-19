using System;
using System.Drawing;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Controls
{
    public partial class TexturePanel : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public Image Image { get => pictureBox1.Image; set { pictureBox1.Image = value; RepositionImage(); ZoomFactor = 1; } }

        /// <summary>
        /// 
        /// </summary>
        public float ZoomFactor = 1;

        /// <summary>
        /// 
        /// </summary>
        public TexturePanel()
        {
            InitializeComponent();

            DoubleBuffered = true;

            MouseWheel += (sender, args) =>
            {
                if (pictureBox1.Image != null)
                {
                    if (args.Delta > 0)
                    {
                        ZoomFactor = ZoomFactor * 1.05f;
                    }
                    else if (ZoomFactor != 1.0)
                    {
                        ZoomFactor = ZoomFactor / 1.05f;
                    }
                    
                    RepositionImage();
                }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TexturePanel_Resize(object sender, EventArgs e)
        {
            RepositionImage();
        }

        /// <summary>
        /// 
        /// </summary>
        private void RepositionImage()
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Width = (int)(pictureBox1.Image.Width * ZoomFactor);
                pictureBox1.Height = (int)(pictureBox1.Image.Height * ZoomFactor);
                pictureBox1.Location = new Point(Width / 2 - pictureBox1.Width / 2, Height / 2 - pictureBox1.Height / 2);
            }

            Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TexturePanel_Paint(object sender, PaintEventArgs e)
        {
            using (var brush = new SolidBrush(ForeColor))
                e.Graphics.DrawString($"Zoom: {(int)(ZoomFactor * 100)} %", Font, brush, new Point(0, 0));
        }
    }
}
