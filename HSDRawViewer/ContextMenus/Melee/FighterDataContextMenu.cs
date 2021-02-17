using System;
using System.Windows.Forms;
using HSDRaw.Melee.Pl;

namespace HSDRawViewer.ContextMenus.Melee
{
    /// <summary>
    /// 
    /// </summary>
    public class FighterDataContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(SBM_FighterData) };

        public FighterDataContextMenu() : base()
        {
            MenuItem addFromFile = new MenuItem("Add Article Folder");
            addFromFile.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is SBM_FighterData root)
                {
                    if(root.Articles == null)
                        root.Articles = new SBM_ArticlePointer();
                }
            };
            MenuItems.Add(addFromFile);
        }
    }
}
