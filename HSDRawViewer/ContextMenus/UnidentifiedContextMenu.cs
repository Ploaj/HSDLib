using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus
{
    public class UnidentifiedContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(HSDAccessor) };

        public UnidentifiedContextMenu() : base()
        {
            MenuItem OpenAsJOBJ = new MenuItem("Open As JOBJ");
            OpenAsJOBJ.Click += (sender, args) => MainForm.Instance.SelectNode(new HSD_JOBJ());
            MenuItems.Add(OpenAsJOBJ);

            MenuItem OpenAsAJ = new MenuItem("Open As AnimJoint");
            OpenAsAJ.Click += (sender, args) => MainForm.Instance.SelectNode(new HSD_AnimJoint());
            MenuItems.Add(OpenAsAJ);
        }

    }
}
