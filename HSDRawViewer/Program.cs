using OpenTK;
using System;
using System.Threading;
using System.Windows.Forms;
using System.Linq;

namespace HSDRawViewer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetCompatibleTextRenderingDefault(false);
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            MainForm.Init();
            ApplicationSettings.Init();
            Application.Run(MainForm.Instance);
        }
    }
}
