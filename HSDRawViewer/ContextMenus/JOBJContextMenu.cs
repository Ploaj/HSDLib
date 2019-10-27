using HSDRaw.Common;
using HSDRawViewer.Converters;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus
{
    public class JOBJContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(HSD_JOBJ) };

        public JOBJContextMenu() : base()
        {
            MenuItem Import = new MenuItem("Import Model From File");
            Import.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_JOBJ root)
                {
                    MainForm.SelectedDataNode.Collapse();
                    ModelImporter.ReplaceModelFromFile(root);
                }
            };
            MenuItems.Add(Import);
        }
    }
}
