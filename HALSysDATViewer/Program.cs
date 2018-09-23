using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HSDLib;

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
            //HSDFile f = new HSDFile("C:\\Users\\Owen\\Desktop\\Modlee\\Melee\\root\\PlYsRe.dat");
            //f.Save("C:\\Users\\Owen\\Desktop\\Modlee\\Melee\\TyMario.dat");
            //var f2 = new HSDFile("C:\\Users\\Owen\\Desktop\\Modlee\\Melee\\TyMario.dat");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
