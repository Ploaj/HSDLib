using HSDRaw.Common;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus
{
    public class SplineContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(HSD_Spline) };

        public SplineContextMenu() : base()
        {
            ToolStripMenuItem Import = new ToolStripMenuItem("Import");
            Import.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_Spline spline)
                {
                    var f = Tools.FileIO.OpenFile("Spline CSV(*.csv)|*.csv");

                    using (FileStream stream = new FileStream(f, FileMode.Open))
                    using (StreamReader s = new StreamReader(stream))
                    {
                        List<HSD_Vector3> points = new List<HSD_Vector3>();
                        int line_index = 0;
                        float totalLength = 0;
                        while(!s.EndOfStream)
                        {
                            var line = s.ReadLine();
                            var ar = line.Split(',');

                            if(ar.Length < 3 || ar.Length >= 4)
                            {
                                MessageBox.Show(
                                    $"Unexpected argument count on line {line_index}", 
                                    "Error Reading File", 
                                    MessageBoxButtons.OK, 
                                    MessageBoxIcon.Error);
                                return;
                            }

                            if(float.TryParse(ar[0], out float x) && 
                            float.TryParse(ar[1], out float y) &&
                            float.TryParse(ar[2], out float z)
                            )
                            {
                                points.Add(new HSD_Vector3() { X = x, Y = y, Z = z });
                                totalLength += new Vector3(x, y, z).Length;
                            }
                            else
                            {
                                MessageBox.Show(
                                    $"Unexpected value in {line} on line {line_index}",
                                    "Error Reading File",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                                return;
                            }

                            line_index++;
                        }

                        float[] lengths = new float[points.Count];
                        for (int i = 0; i < points.Count; i++)
                            lengths[i] = i / (float)(points.Count - 1);

                        spline.TotalLength = totalLength;
                        spline.Points = points.ToArray();
                        spline.Lengths = new HSDRaw.HSDFloatArray() { Array = lengths };
                    }
                }
            };
            Items.Add(Import);

            ToolStripMenuItem Export = new ToolStripMenuItem("Export");
            Export.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_Spline spline)
                {
                    var f = Tools.FileIO.SaveFile("Spline CSV(*.csv)|*.csv");

                    if (f != null)
                    {
                        using (FileStream stream = new FileStream(f, FileMode.Create))
                        using (StreamWriter s = new StreamWriter(stream))
                        {
                            var points = spline.Points;

                            foreach (var p in points)
                            {
                                s.WriteLine($"{p.X},{p.Y},{p.Z}");
                            }
                        }
                    }
                }
            };
            Items.Add(Export);
        }

    }
}
