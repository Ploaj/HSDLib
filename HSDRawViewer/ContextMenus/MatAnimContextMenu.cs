using HSDRaw.Common.Animation;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus
{
    public class MatAnimContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(HSD_MatAnim) };

        public MatAnimContextMenu() : base()
        {
            ToolStripMenuItem AddMatAnim = new ToolStripMenuItem("Add Material Animation");
            AddMatAnim.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_MatAnim matanim)
                {
                    matanim.AnimationObject = new HSD_AOBJ();
                    matanim.AnimationObject.FObjDesc = new HSD_FOBJDesc();
                    MainForm.SelectedDataNode.Refresh();
                }
            };
            Items.Add(AddMatAnim);

            ToolStripMenuItem OpenAsAJ = new ToolStripMenuItem("Add Texture Animation");
            OpenAsAJ.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_MatAnim matanim)
                {
                    matanim.TextureAnimation = new HSD_TexAnim();
                    matanim.TextureAnimation.AnimationObject = new HSD_AOBJ();
                    matanim.TextureAnimation.AnimationObject.FObjDesc = new HSD_FOBJDesc();
                    MainForm.SelectedDataNode.Refresh();
                }
            };
            Items.Add(OpenAsAJ);
        }
    }
}
