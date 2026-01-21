using HSDRaw.Common.Animation;
using HSDRawViewer.Converters;
using System;
using System.IO;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus
{
    public class FOBJContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(HSD_FOBJ), typeof(HSD_FOBJDesc) };

        public FOBJContextMenu() : base()
        {
            ToolStripMenuItem Export = new("Export Frames TXT");
            Export.Click += (sender, args) =>
            {
                using SaveFileDialog d = new();
                d.Filter = "TXT (*.txt)|*.txt";

                if (d.ShowDialog() == DialogResult.OK)
                {
                    if (MainForm.SelectedDataNode.Accessor is HSD_FOBJ fobj)
                        File.WriteAllText(d.FileName, ConvFOBJ.ToString(fobj));

                    if (MainForm.SelectedDataNode.Accessor is HSD_FOBJDesc fobjdesc)
                        File.WriteAllText(d.FileName, ConvFOBJ.ToString(fobjdesc));
                }
            };
            Items.Add(Export);

            ToolStripMenuItem Import = new("Import Frames TXT");
            Import.Click += (sender, args) =>
            {
                using OpenFileDialog d = new();
                d.Filter = "TXT (*.txt)|*.txt";

                if (d.ShowDialog() == DialogResult.OK)
                {
                    if (MainForm.SelectedDataNode.Accessor is HSD_FOBJ fobj)
                        ConvFOBJ.ImportKeys(fobj, File.ReadAllLines(d.FileName));

                    if (MainForm.SelectedDataNode.Accessor is HSD_FOBJDesc fobjdesc)
                        ConvFOBJ.ImportKeys(fobjdesc, File.ReadAllLines(d.FileName));
                }
            };
            Items.Add(Import);
        }
    }
}
