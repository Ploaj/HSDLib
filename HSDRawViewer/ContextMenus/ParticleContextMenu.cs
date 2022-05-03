using HSDRaw.Common;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus
{
    public class ParticleContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(HSD_ParticleGroup) };

        public ParticleContextMenu() : base()
        {
            MenuItem addFromFile = new MenuItem("Add Particle From File");
            addFromFile.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_ParticleGroup root)
                {
                    var f = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);
                    if (f != null)
                    {
                        var file = new HSDRaw.HSDRawFile(f);
                        var mod = root.Generators;
                        Array.Resize(ref mod, mod.Length + 1);
                        mod[mod.Length - 1] = new HSD_ParticleGenerator()
                        {
                            _s = file.Roots[0].Data._s
                        };
                        root.Generators = mod;
                    }
                }
            };
            MenuItems.Add(addFromFile);

            MenuItem export = new MenuItem("Export PTL");
            export.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_ParticleGroup root)
                {
                    var f = Tools.FileIO.SaveFile("Particle (*.ptl)|*.ptl");

                    if (f != null)
                        System.IO.File.WriteAllBytes(f, root._s.GetData());
                }
            };
            MenuItems.Add(export);
        }
    }

    public class TEXGContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(HSD_TEXGraphicBank) };

        public TEXGContextMenu() : base()
        {
            MenuItem addFromFile = new MenuItem("Add TEXG From File");
            addFromFile.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_TEXGraphicBank root)
                {
                    var f = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);
                    if (f != null)
                    {
                        var file = new HSDRaw.HSDRawFile(f);
                        var mod = root.ParticleImages;
                        Array.Resize(ref mod, mod.Length + 1);
                        mod[mod.Length - 1] = new HSD_TexGraphic()
                        {
                            _s = file.Roots[0].Data._s
                        };
                        root.ParticleImages = mod;
                    }
                }
            };
            MenuItems.Add(addFromFile);

            MenuItem scratch = new MenuItem("Add TEXG From Scratch");
            scratch.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_TEXGraphicBank root)
                {
                    var mod = root.ParticleImages;
                    Array.Resize(ref mod, mod.Length + 1);
                    mod[mod.Length - 1] = new HSD_TexGraphic();
                    root.ParticleImages = mod;
                }
            };
            MenuItems.Add(scratch);


            MenuItem export = new MenuItem("Export TXG");
            export.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_TEXGraphicBank root)
                {
                    var f = Tools.FileIO.SaveFile("TEXG (*.txg)|*.txg");

                    if (f != null)
                        System.IO.File.WriteAllBytes(f, root._s.GetData());
                }
            };
            MenuItems.Add(export);
        }
    }
}
