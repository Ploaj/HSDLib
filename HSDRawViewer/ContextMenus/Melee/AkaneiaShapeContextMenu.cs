using HSDRaw.Common;
using HSDRaw.MEX.Akaneia;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Windows.Forms;
using SixLabors.ImageSharp;
using System.Linq;

namespace HSDRawViewer.ContextMenus.Melee
{
    public class AkaneiaShapeContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(AK_Shape) };

        public AkaneiaShapeContextMenu() : base()
        {
            ToolStripMenuItem import = new("Import Shape From Image");
            import.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is AK_Shape page)
                {
                    string f = Tools.FileIO.OpenFile(ApplicationSettings.ImageFileFilter);

                    if (f != null)
                    {
                        HSD_TOBJ tobj = new();
                        tobj.ImportImage(f, HSDRaw.GX.GXTexFmt.RGBA8, HSDRaw.GX.GXTlutFmt.IA8);
                        page.FromTOBJ(tobj);
                    }
                }
            };
            Items.Add(import);


            ToolStripMenuItem export = new("Export to Image");
            export.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is AK_Shape page)
                {
                    string f = Tools.FileIO.SaveFile("PNG (*.png)|*.png;");

                    if (f != null)
                    {
                        var pal = page.PaletteData.Array.Select(e=>
                        {
                            return new byte[] {
                                (byte)((e >> 24) & 0xFF),
                                (byte)((e >> 16) & 0xFF),
                                (byte)((e >> 8) & 0xFF),
                                (byte)(e & 0xFF),
                            };
                        }).ToArray();
                        
                        byte[] bgraBytes = page.IndexData.Array.SelectMany(e => pal[e]).ToArray();

                        using var img = Image.LoadPixelData<Rgba32>(bgraBytes, page.Width, page.Height);
                        img.SaveAsPng(f);
                    }
                }
            };
            Items.Add(export);
        }
    }
}
