using HSDRaw.Common;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus
{
    public class JOBJDescContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(HSD_JOBJDesc) };

        public JOBJDescContextMenu() : base()
        {
            MenuItem add = new MenuItem("Add");
            
            {
                MenuItem nn = new MenuItem("Add Joint Anim Folder");
                nn.Click += (sender, args) =>
                {
                    if (MainForm.SelectedDataNode.Accessor is HSD_JOBJDesc desc && desc.JointAnimations == null)
                        desc.JointAnimations = new HSDRaw.HSDNullPointerArrayAccessor<HSDRaw.Common.Animation.HSD_AnimJoint>();
                };
                add.MenuItems.Add(nn);
            }

            {
                MenuItem nn = new MenuItem("Add Material Anim Folder");
                nn.Click += (sender, args) =>
                {
                    if (MainForm.SelectedDataNode.Accessor is HSD_JOBJDesc desc && desc.MaterialAnimations == null)
                        desc.MaterialAnimations = new HSDRaw.HSDNullPointerArrayAccessor<HSDRaw.Common.Animation.HSD_MatAnimJoint>();
                };
                add.MenuItems.Add(nn);
            }

            MenuItems.Add(add);
        }

    }
}
