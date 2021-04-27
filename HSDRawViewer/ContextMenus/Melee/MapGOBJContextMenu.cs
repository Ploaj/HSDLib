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
            MenuItem genJointAnim = new MenuItem("Create Joint Animation Bank");
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
            MenuItems.Add(genJointAnim);


            MenuItem genMatAnim = new MenuItem("Create Material Animation Bank");
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
            MenuItems.Add(genMatAnim);


            MenuItem addFog = new MenuItem("Add Fog Struct");
            addFog.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is SBM_Map_GOBJ gobj)
                {
                    gobj.Fog = new HSD_FogDesc();
                    MainForm.SelectedDataNode.Refresh();
                }
            };
            MenuItems.Add(addFog);
        }
    }
}
