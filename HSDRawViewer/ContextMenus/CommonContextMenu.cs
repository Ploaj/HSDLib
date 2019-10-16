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

            if(SupportedTypes != null)
            foreach(var v in SupportedTypes)
            {
                if(PluginManager.HasEditor(v))
                {
                    MenuItem editor = new MenuItem("Open Editor");

                    editor.Click += (sender, args) =>
                    {
                        if (MainForm.SelectedDataNode != null)
                        {
                            MainForm.Instance.OpenEditor();
                        }
                    };

                    MenuItems.Add(editor);
                    break;
                }
            }


            MenuItems.Add(export);
            MenuItems.Add(import);
            MenuItems.Add(delete);
            MenuItems.Add("-");
        }
    }
}
