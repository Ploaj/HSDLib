using HSDRaw.Melee;
using System;
using System.IO;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus.Melee
{
    internal class CpuTableContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(SBM_PlCoCPUTable) };

        public CpuTableContextMenu() : base()
        {
            ToolStripMenuItem Export = new("Export Command List");
            Export.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is SBM_PlCoCPUTable table)
                {
                    using SaveFileDialog sd = new();
                    sd.Filter = "Text (.txt)|*.txt";

                    if (sd.ShowDialog() == DialogResult.OK)
                    {
                        using FileStream fstream = new(sd.FileName, FileMode.Create);
                        using StreamWriter writer = new(fstream);
                        SBM_CPUCommand[] com = table.Scripts.Array;

                        for (int i = 0; i < com.Length; i++)
                        {
                            writer.WriteLine($"Script - {i}");
                            if (com[i] != null)
                            {
                                writer.WriteLine("{");
                                writer.WriteLine(com[i].Script);
                                writer.WriteLine("}");
                            }
                        }
                    }
                }
            };
            Items.Add(Export);


            //ToolStripMenuItem Export2 = new ToolStripMenuItem("Export Command List");
            //Export2.Click += (sender, args) =>
            //{
            //    if (MainForm.SelectedDataNode.Accessor is SBM_PlCoCPUTable table)
            //    {
            //        using (SaveFileDialog sd = new SaveFileDialog())
            //        {
            //            sd.Filter = "Text (.txt)|*.txt";

            //            if (sd.ShowDialog() == DialogResult.OK)
            //            {
            //                using (var fstream = new FileStream(sd.FileName, FileMode.Create))
            //                using (var writer = new StreamWriter(fstream))
            //                {
            //                    var com = table.Scripts.Array;

            //                    for (int i = 0; i < com.Length; i++)
            //                    {
            //                        writer.WriteLine($"Script - {i}");
            //                        if (com[i] != null)
            //                        {
            //                            writer.WriteLine("{");
            //                            writer.WriteLine(com[i].Script);
            //                            writer.WriteLine("}");
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //};
            //Items.Add(Export2);
        }
    }
}
