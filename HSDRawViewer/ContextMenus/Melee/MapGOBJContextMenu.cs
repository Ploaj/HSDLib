using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Melee.Gr;
using HSDRawViewer.Tools;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus.Melee
{
    public class MapGOBJContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(SBM_Map_GOBJ) };

        public MapGOBJContextMenu() : base()
        {
            ToolStripMenuItem genJointAnim = new ToolStripMenuItem("Create Joint Animation Bank");
            genJointAnim.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is SBM_Map_GOBJ gobj &&
                    gobj.JointAnimations == null &&
                    gobj.RootNode != null)
                {
                    gobj.JointAnimations = new HSDNullPointerArrayAccessor<HSDRaw.Common.Animation.HSD_AnimJoint>();
                    gobj.JointAnimations.Array = new HSDRaw.Common.Animation.HSD_AnimJoint[] { JOBJContextMenu.GenerateAnimJointFromJOBJ(gobj.RootNode) };

                    MainForm.SelectedDataNode.Refresh();
                }
            };
            Items.Add(genJointAnim);


            ToolStripMenuItem genMatAnim = new ToolStripMenuItem("Create Material Animation Bank");
            genMatAnim.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is SBM_Map_GOBJ gobj && 
                    gobj.MaterialAnimations == null &&
                    gobj.RootNode != null)
                {
                    gobj.MaterialAnimations = new HSDNullPointerArrayAccessor<HSDRaw.Common.Animation.HSD_MatAnimJoint>();
                    gobj.MaterialAnimations.Array = new HSDRaw.Common.Animation.HSD_MatAnimJoint[] { JOBJContextMenu.GenerateMatAnimJointFromJOBJ(gobj.RootNode) };
                    
                    MainForm.SelectedDataNode.Refresh();
                }
            };
            Items.Add(genMatAnim);


            ToolStripMenuItem addFog = new ToolStripMenuItem("Add Fog Struct");
            addFog.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is SBM_Map_GOBJ gobj)
                {
                    gobj.Fog = new HSD_FogDesc();
                    MainForm.SelectedDataNode.Refresh();
                }
            };
            Items.Add(addFog);
        }
    }
}
