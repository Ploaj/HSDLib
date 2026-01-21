using HSDRaw;
using HSDRaw.Common;
using HSDRawViewer.Tools;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus
{
    public class DOBJContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(HSD_DOBJ) };

        public DOBJContextMenu() : base()
        {
            ToolStripMenuItem ImportAfter = new("Import DOBJ After");
            ImportAfter.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_DOBJ dobj)
                {
                    string f = FileIO.OpenFile("DAT (.dat)|*.dat");
                    if (f != null)
                    {
                        HSDRawFile dat = new(f);
                        HSD_DOBJ newDOBJ = new();
                        newDOBJ._s = dat.Roots[0].Data._s;
                        if (newDOBJ._s.Length == newDOBJ.TrimmedSize)
                        {
                            newDOBJ.Next = dobj.Next;
                            dobj.Next = newDOBJ;
                        }
                    }
                }
            };
            Items.Add(ImportAfter);
        }
    }
}
