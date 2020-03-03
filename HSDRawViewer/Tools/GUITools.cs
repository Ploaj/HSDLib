using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HSDRawViewer
{
    public class GUITools
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static void SetIconFromBitmap(Form f, Bitmap bmp)
        {
            IntPtr Hicon = bmp.GetHicon();
            Icon newIcon = Icon.FromHandle(Hicon);
            f.Icon = newIcon;
            //DestroyIcon(newIcon.Handle);
        }
    }
}
