using HSDRaw.Common;
using HSDRawViewer.Tools;
using System;
using System.Windows.Forms;
using HSDRaw;

namespace HSDRawViewer.ContextMenus
{
    public class DOBJContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(HSD_DOBJ) };

        public DOBJContextMenu() : base()
        {
            MenuItem ImportAfter = new MenuItem("Import DOBJ After");
            ImportAfter.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_DOBJ dobj)
                {
                    var f = FileIO.OpenFile("DAT (.dat)|*.dat");
                    if (f != null)
                    {
                        HSDRawFile dat = new HSDRawFile(f);
                        HSD_DOBJ newDOBJ = new HSD_DOBJ();
                        newDOBJ._s = dat.Roots[0].Data._s;
                        if(newDOBJ._s.Length == newDOBJ.TrimmedSize)
                        {
                            newDOBJ.Next = dobj.Next;
                            dobj.Next = newDOBJ;
                        }
                    }
                }
            };
            MenuItems.Add(ImportAfter);
        }
    }
}
