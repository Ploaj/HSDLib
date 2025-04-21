using HSDRawViewer.GUI.Dialog;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus
{
    public class CommonContextMenu : ContextMenuStrip
    {
        public virtual Type[] SupportedTypes { get; }

        public CommonContextMenu()
        {
            ToolStripMenuItem delete = new("Delete");
            delete.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode != null)
                {
                    MainForm.SelectedDataNode.Delete();
                }
            };
            ToolStripMenuItem export = new("Export");
            export.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode != null)
                {
                    MainForm.SelectedDataNode.Export();
                }
            };
            ToolStripMenuItem import = new("Replace");
            import.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode != null)
                {
                    MainForm.SelectedDataNode.Import();
                }
            };

            if (SupportedTypes != null)
                foreach (Type v in SupportedTypes)
                {
                    if (PluginManager.HasEditor(v))
                    {
                        ToolStripMenuItem editor = new("Open Editor");

                        editor.Click += (sender, args) =>
                        {
                            if (MainForm.SelectedDataNode != null)
                            {
                                MainForm.Instance.OpenEditor();
                            }
                        };

                        Items.Add(editor);
                        break;
                    }
                }

            ToolStripMenuItem addRootReference = new("Add Reference To Root");
            addRootReference.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode != null)
                {
                    RootNameCreator setting = new();
                    using PropertyDialog prop = new("Symbol Name", setting);
                    if (prop.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(setting.SymbolName))
                        MainForm.AddRoot(setting.SymbolName, MainForm.SelectedDataNode.Accessor);
                }
            };

            Items.Add(export);
            Items.Add(import);
            Items.Add(delete);
            Items.Add("-");
            Items.Add(addRootReference);
            Items.Add("-");
        }

        public class RootNameCreator
        {
            public string SymbolName { get; set; } = "";
        }
    }
}
