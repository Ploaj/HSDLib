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
            MenuItem OpenAsAJ = new MenuItem("Export As .figatree");
            OpenAsAJ.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_FigaTree figa)
                {
                    var f = Tools.FileIO.SaveFile("Figatree (.figatree)|*.figatree");

                    if(f != null)
                    {
                        using (StreamWriter w = new StreamWriter(new FileStream(f, FileMode.Create)))
                        {
                            w.WriteLine($"FrameCount: {figa.FrameCount}");

                            var ni = 0;
                            foreach(var n in figa.Nodes)
                            {
                                w.WriteLine($"Node {ni++}:");

                                foreach(var t in n.Tracks)
                                {
                                    w.WriteLine($"{t.JointTrackType}");

                                    w.WriteLine("{");

                                    foreach(var k in t.GetKeys())
                                    {
                                        w.WriteLine($"\t{k.Frame} {k.Value} {k.Tan} {k.InterpolationType}");
                                    }

                                    w.WriteLine("}");
                                }
                            }
                        }
                    }
                }
            };
            MenuItems.Add(OpenAsAJ);
        }
    }
}
