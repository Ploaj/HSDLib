using System;
using System.Windows.Forms;
using HSDLib.Common;
using HSDLib.Helpers.TriangleConverter;
using HSDLib.Helpers;

namespace HALSysDATViewer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
