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
            ToolStripMenuItem addFromFile = new("Add Particle From File");
            addFromFile.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_ParticleGroup root)
                {
                    string f = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);
                    if (f != null)
                    {
                        HSDRaw.HSDRawFile file = new(f);
                        HSD_ParticleGenerator[] mod = root.Generators;
                        Array.Resize(ref mod, mod.Length + 1);
                        mod[mod.Length - 1] = new HSD_ParticleGenerator()
                        {
                            _s = file.Roots[0].Data._s
                        };
                        root.Generators = mod;
                    }
                }
            };
            Items.Add(addFromFile);

            ToolStripMenuItem export = new("Export PTL");
            export.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_ParticleGroup root)
                {
                    string f = Tools.FileIO.SaveFile("Particle (*.ptl)|*.ptl");

                    if (f != null)
                        System.IO.File.WriteAllBytes(f, root._s.GetData());
                }
            };
            Items.Add(export);
        }
    }

    public class TEXGContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(HSD_TEXGraphicBank) };

        public TEXGContextMenu() : base()
        {
            ToolStripMenuItem addFromFile = new("Add TEXG From File");
            addFromFile.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_TEXGraphicBank root)
                {
                    string f = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);
                    if (f != null)
                    {
                        HSDRaw.HSDRawFile file = new(f);
                        HSD_TexGraphic[] mod = root.ParticleImages;
                        Array.Resize(ref mod, mod.Length + 1);
                        mod[mod.Length - 1] = new HSD_TexGraphic()
                        {
                            _s = file.Roots[0].Data._s
                        };
                        root.ParticleImages = mod;
                    }
                }
            };
            Items.Add(addFromFile);

            ToolStripMenuItem scratch = new("Add TEXG From Scratch");
            scratch.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_TEXGraphicBank root)
                {
                    HSD_TexGraphic[] mod = root.ParticleImages;
                    Array.Resize(ref mod, mod.Length + 1);
                    mod[mod.Length - 1] = new HSD_TexGraphic();
                    root.ParticleImages = mod;
                }
            };
            Items.Add(scratch);


            ToolStripMenuItem export = new("Export TXG");
            export.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_TEXGraphicBank root)
                {
                    string f = Tools.FileIO.SaveFile("TEXG (*.txg)|*.txg");

                    if (f != null)
                        System.IO.File.WriteAllBytes(f, root._s.GetData());
                }
            };
            Items.Add(export);
        }
    }
}
