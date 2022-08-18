using HSDRawViewer.GUI;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus
{
    public class CommonContextMenu : ContextMenuStrip
    {
        public virtual Type[] SupportedTypes { get; }

        public CommonContextMenu()
        {
            ToolStripMenuItem delete = new ToolStripMenuItem("Delete");
            delete.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode != null)
                {
                    MainForm.SelectedDataNode.Delete();
                }
            };
            ToolStripMenuItem export = new ToolStripMenuItem("Export");
            export.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode != null)
                {
                    MainForm.SelectedDataNode.Export();
                }
            };
            ToolStripMenuItem import = new ToolStripMenuItem("Replace");
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
                        ToolStripMenuItem editor = new ToolStripMenuItem("Open Editor");

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

            ToolStripMenuItem addRootReference = new ToolStripMenuItem("Add Reference To Root");
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
