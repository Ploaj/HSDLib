﻿using HSDRaw.Common;
using HSDRaw.MEX.Akaneia;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus.Melee
{
    public class AkaneiaShapeContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(AK_Shape) };

        public AkaneiaShapeContextMenu() : base()
        {
            ToolStripMenuItem export = new ToolStripMenuItem("Import Shape From Image");
            export.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is AK_Shape page)
                {
                    var f = Tools.FileIO.OpenFile(ApplicationSettings.ImageFileFilter);

                    if (f != null)
                    {
                        var tobj = new HSD_TOBJ();
                        tobj.ImportImage(f, HSDRaw.GX.GXTexFmt.RGBA8, HSDRaw.GX.GXTlutFmt.IA8);
                        page.FromTOBJ(tobj);
                    }
                }
            };
            Items.Add(export);
        }
    }
}
