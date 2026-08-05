using HSDRaw;
using HSDRaw.AirRide.Gr.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
namespace HSDRawViewer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //StringBuilder o = new StringBuilder();
            //foreach (var f in Directory.GetFiles(@"E:\KirbyFileSystems\CityTrial\files\"))
            //{
            //    if (!Path.GetFileName(f).Contains("Common") && 
            //        Path.GetFileName(f).StartsWith("Gr") && 
            //        !Path.GetFileNameWithoutExtension(f).EndsWith("Model") && 
            //        !Path.GetFileNameWithoutExtension(f).EndsWith("Event"))
            //    {
            //        var d = new HSDRawFile(f).Roots[0].Data as KAR_grData;

            //        if (d == null || d.CollisionNode == null || d.CollisionNode.ZoneJoints == null)
            //            continue;

            //        var tri = d.CollisionNode.ZoneTriangles;

            //        foreach (var z in d.CollisionNode.ZoneJoints)
            //        {
            //            if (z.ZoneFaceSize != 12)
            //                o.Append(z.ZoneFaceSize + " " + f);

            //            var r = tri[z.ZoneFaceStart];

            //            for (int i = z.ZoneFaceStart; i < z.ZoneFaceStart + z.ZoneFaceSize; i++)
            //            {
            //                var face = tri[i];

            //                if (r.Flags != face.Flags || face.Flags != 0)
            //                    o.Append($"Coll Flag {face.Flags:X8} {f}\n");

            //                //if (r.UnknownIndex != face.UnknownIndex)
            //                //    o.Append($"Index {f}\n");

            //                if (r.Type != face.Type)
            //                    o.Append($"Type {f}\n");
            //            }
            //        }
            //    }
            //}

            //Debug.WriteLine(o.ToString());

            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            PluginManager.Init();
            Rendering.OpenTKResources.Init();
            MainForm.Init();
            ApplicationSettings.Init();
            if (args.Length > 0)
                MainForm.Instance.OpenFile(args[0]);
            Application.Run(MainForm.Instance);
        }
    }
}
