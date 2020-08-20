using System;
using HSDRaw.Melee.Pl;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus
{
    public class FighterArticleContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(SBM_ArticlePointer) };

        public FighterArticleContextMenu() : base()
        {
            MenuItem addFromFile = new MenuItem("Add Article From File");
            addFromFile.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is SBM_ArticlePointer root)
                {
                    var f = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);
                    if (f != null)
                    {
                        var file = new HSDRaw.HSDRawFile(f);
                        var mod = root.Articles;
                        Array.Resize(ref mod, mod.Length + 1);
                        mod[mod.Length - 1] = new SBM_Article()
                        {
                            _s = file.Roots[0].Data._s
                        };
                        root.Articles = mod;
                    }
                }
            };
            MenuItems.Add(addFromFile);
        }
    }
}
