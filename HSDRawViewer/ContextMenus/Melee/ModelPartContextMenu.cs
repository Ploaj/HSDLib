using HSDRaw;
using HSDRaw.Common.Animation;
using HSDRaw.Melee.Pl;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus.Melee
{
    internal class ModelPartContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(SBM_ModelPart) };

        public ModelPartContextMenu() : base()
        {
            ToolStripMenuItem ImportPose = new("Import Pose");
            ImportPose.Click += (sender, args) =>
            {
                // check type
                if (MainForm.SelectedDataNode.Accessor is not SBM_ModelPart part)
                    return;

                // load file
                string af = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);
                if (af == null)
                    return;

                // load animation
                HSDAccessor a = new HSDRawFile(af).Roots[0].Data;
                if (a is not HSD_AnimJoint aj)
                    return;

                // check bone list
                var list = aj.TreeList;
                if (part.StartingBone + part.Count >= list.Count)
                    return;

                // extract anim part
                part.Anims.Add(list[part.StartingBone]);
            };
            Items.Add(ImportPose);
        }
    }
}
