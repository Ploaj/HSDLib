using HSDRawViewer.GUI;
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

            if (SupportedTypes != null)
                foreach (var v in SupportedTypes)
                {
                    if (PluginManager.HasEditor(v))
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

            MenuItem addRootReference = new MenuItem("Add Reference To Root");
            addRootReference.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode != null)
                {
                    var setting = new RootNameCreator();
                    using (var prop = new PropertyDialog("Symbol Name", setting))
                        if (prop.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(setting.SymbolName))
                            MainForm.AddRoot(setting.SymbolName, MainForm.SelectedDataNode.Accessor);
                }
            };

            MenuItems.Add(export);
            MenuItems.Add(import);
            MenuItems.Add(delete);
            MenuItems.Add("-");
            MenuItems.Add(addRootReference);
            MenuItems.Add("-");
        }

        public class RootNameCreator
        {
            public string SymbolName { get; set; } = "";
        }
    }
}
