using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// This class is a double-buffered ListBox for owner drawing.
    /// The double-buffering is accomplished by creating a custom,
    /// off-screen buffer during painting.
    /// </summary>
    public sealed class DoubleBufferedListBox : ListBox
    {
        #region Method Overrides
        /// <summary>
        /// Override OnTemplateListDrawItem to supply an off-screen buffer to event
        /// handlers.
        /// </summary>
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;

            Rectangle newBounds = new Rectangle(0, 0, e.Bounds.Width, e.Bounds.Height);
            using (BufferedGraphics bufferedGraphics = currentContext.Allocate(e.Graphics, newBounds))
            {
                DrawItemEventArgs newArgs = new DrawItemEventArgs(
                    bufferedGraphics.Graphics, e.Font, newBounds, e.Index, e.State, e.ForeColor, e.BackColor);

                // Supply the real OnTemplateListDrawItem with the off-screen graphics context
                base.OnDrawItem(newArgs);

                // Wrapper around BitBlt
                GDI.CopyGraphics(e.Graphics, e.Bounds, bufferedGraphics.Graphics, new Point(0, 0));
            }
        }
        #endregion
    }

    public static class GDI
    {
        private const UInt32 SRCCOPY = 0x00CC0020;

        [DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, UInt32 dwRop);

        public static void CopyGraphics(Graphics g, Rectangle bounds, Graphics bufferedGraphics, Point p)
        {
            IntPtr hdc1 = g.GetHdc();
            IntPtr hdc2 = bufferedGraphics.GetHdc();

            BitBlt(hdc1, bounds.X, bounds.Y,
                bounds.Width, bounds.Height, hdc2, (int)p.X, (int)p.Y, SRCCOPY);

            g.ReleaseHdc(hdc1);
            bufferedGraphics.ReleaseHdc(hdc2);
        }
    }


}
