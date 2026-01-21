using HSDRaw.Common.Animation;
using System;
using System.IO;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus
{
    public class FigatreeContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(HSD_FigaTree) };

        public FigatreeContextMenu() : base()
        {
            ToolStripMenuItem OpenAsAJ = new("Export As .figatree");
            OpenAsAJ.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_FigaTree figa)
                {
                    string f = Tools.FileIO.SaveFile("Figatree (.figatree)|*.figatree");

                    if (f != null)
                    {
                        using StreamWriter w = new(new FileStream(f, FileMode.Create));
                        w.WriteLine($"FrameCount: {figa.FrameCount}");

                        int ni = 0;
                        foreach (FigaTreeNode n in figa.Nodes)
                        {
                            w.WriteLine($"Node {ni++}:");

                            foreach (HSD_Track t in n.Tracks)
                            {
                                w.WriteLine($"{t.JointTrackType}");

                                w.WriteLine("{");

                                foreach (HSDRaw.Tools.FOBJKey k in t.GetKeys())
                                {
                                    w.WriteLine($"\t{k.Frame} {k.Value} {k.Tan} {k.InterpolationType}");
                                }

                                w.WriteLine("}");
                            }
                        }
                    }
                }
            };
            Items.Add(OpenAsAJ);
        }
    }
}
