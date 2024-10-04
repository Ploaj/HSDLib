using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Melee.Pl;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus.Melee
{
    public class ShieldContainerContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(SBM_ShieldModelContainer) };
        
        public ShieldContainerContextMenu() : base()
        {
            ToolStripMenuItem ImportPose = new ToolStripMenuItem("Import Pose");
            ImportPose.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is SBM_ShieldModelContainer figa)
                {
                    // load model
                    var mf = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);

                    if (mf != null)
                    {
                        var af = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);

                        if (af != null)
                        {
                            var a = new HSDRawFile(af).Roots[0].Data;

                            JointAnimManager anim = null;

                            if (a is HSD_FigaTree ft)
                                anim = new JointAnimManager(ft);

                            if (a is HSD_AnimJoint aj)
                                anim = new JointAnimManager(aj);

                            if (anim != null)
                            {
                                var model = new HSDRawFile(mf).Roots[0].Data as HSD_JOBJ;
                                var live = new LiveJObj(model);
                                anim.ApplyAnimation(live, 0);

                                foreach (var d in model.TreeList)
                                {
                                    // clear model
                                    d.Dobj = null;
                                    d.InverseWorldTransform = null;

                                    // set transforms
                                    var r = live.GetJObjFromDesc(d);
                                    d.TX = r.Translation.X;
                                    d.TY = r.Translation.Y;
                                    d.TZ = r.Translation.Z;
                                    d.RX = r.Rotation.X;
                                    d.RY = r.Rotation.Y;
                                    d.RZ = r.Rotation.Z;
                                    d.SX = r.Scale.X;
                                    d.SY = r.Scale.Y;
                                    d.SZ = r.Scale.Z;
                                }
                                model.UpdateFlags();

                                figa.ShieldPose = model;
                            }
                        }
                    }
                }
            };
            Items.Add(ImportPose);
        }


    }
}
