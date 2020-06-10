using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Forms
{
    public class CustomPaintTrackBar : TrackBar
    {
        public event PaintEventHandler PaintOver;

        private static Brush HitboxBrush = new SolidBrush(Color.Red);

        public CustomPaintTrackBar()
            : base()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // WM_PAINT
            if (m.Msg == 0x0F)
            {
                using (Graphics lgGraphics = Graphics.FromHwndInternal(m.HWnd))
                    OnPaintOver(new PaintEventArgs(lgGraphics, this.ClientRectangle));
            }
        }

        protected virtual void OnPaintOver(PaintEventArgs e)
        {
            if (PaintOver != null)
                PaintOver(this, e);

            // Paint over code here
            //e.Graphics.FillEllipse(HitboxBrush, new RectangleF());
        }
    }
}
