using System;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus
{
    public class CommonContextMenu : ContextMenu
    {
        public virtual Type[] SupportedTypes { get; }

        public CommonContextMenu()
        {
            MenuItem delete = new MenuItem("Delete");
            delete.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode != null)
                {
                    MainForm.SelectedDataNode.Delete();
                }
            };
            MenuItem export = new MenuItem("Export");
            export.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode != null)
                {
                    MainForm.SelectedDataNode.Export();
                }
            };
            MenuItem import = new MenuItem("Replace");
            import.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode != null)
                {
                    MainForm.SelectedDataNode.Import();
                }
            };

            MenuItems.Add(export);
            MenuItems.Add(import);
            MenuItems.Add(delete);
        }
    }
}
