using HSDRaw.Common.Animation;
using HSDRaw.Melee.Gr;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using System;
using System.Linq;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Controls.MapHeadViewer
{
    /// <summary>
    /// A control that allows you to preview all map head gobj models
    /// </summary>
    public partial class MapHeadViewControl : UserControl, IDrawable
    {
        private class MapHeadGroup : TreeNode
        {
            public SBM_Map_GOBJ gobj;

            public RenderJObj renderJObj;

            public bool Loaded = false;

            public bool Dead = false;

            public float MaxFrame
            {
                get
                {
                    float max = 0;

                    foreach (MapHeadAnimation a in Nodes)
                    {
                        if (a.joint != null)
                            max = Math.Max(max, a.joint.BreathFirstList.Max(e => e.AOBJ != null ? e.AOBJ.EndFrame : 0));
                    }

                    return max;
                }
            }

            public MapHeadGroup(string name, SBM_Map_GOBJ gobj)
            {
                this.gobj = gobj;
                renderJObj = new RenderJObj();
                renderJObj._settings.RenderBones = false;
                renderJObj.LoadJObj(gobj.RootNode);

                Text = name;

                int states = 0;
                if (gobj.JointAnimations != null)
                    states = Math.Max(states, gobj.JointAnimations.Length);
                if (gobj.MaterialAnimations != null)
                    states = Math.Max(states, gobj.MaterialAnimations.Length);

                for (int i = 0; i < states; i++)
                {
                    HSD_AnimJoint joint = null;
                    HSD_MatAnimJoint matanim = null;

                    if (gobj.JointAnimations != null && i < gobj.JointAnimations.Length)
                    {
                        joint = gobj.JointAnimations[i];
                    }

                    if (gobj.MaterialAnimations != null && i < gobj.MaterialAnimations.Length)
                    {
                        matanim = gobj.MaterialAnimations[i];
                    }

                    if (joint != null || matanim != null)
                    {
                        Nodes.Add(new MapHeadAnimation($"Anim {i}", joint, matanim));
                    }
                }
            }

            public void Load()
            {
                renderJObj.Invalidate();
                Loaded = true;
            }

            public void Free()
            {
                renderJObj.FreeResources();
            }
        }

        private class MapHeadAnimation : TreeNode
        {
            public HSD_AnimJoint joint;
            public HSD_MatAnimJoint matanim;

            public MapHeadAnimation(string name, HSD_AnimJoint joint, HSD_MatAnimJoint matanim)
            {
                Text = name;
                this.joint = joint;
                this.matanim = matanim;
            }
        }

        public ViewportControl glViewport;

        public DrawOrder DrawOrder => DrawOrder.Last;

        /// <summary>
        /// 
        /// </summary>
        public MapHeadViewControl()
        {
            InitializeComponent();

            glViewport = new ViewportControl();
            glViewport.Dock = DockStyle.Fill;
            glViewport.DisplayGrid = true;
            glViewport.AnimationTrackEnabled = true;
            groupBox1.Controls.Add(glViewport);

            glViewport.FrameChange += (f) =>
            {
                foreach (MapHeadGroup v in treeView1.Nodes)
                    v.renderJObj.RequestAnimationUpdate(FrameFlags.All, f);
            };

            glViewport.AddRenderer(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="head"></param>
        public void LoadMapHead(SBM_Map_Head head)
        {
            float max = 0;
            int index = 0;
            foreach (var group in head.ModelGroups.Array)
            {
                var g = new MapHeadGroup($"Group_{index}", group);
                max = Math.Max(max, g.MaxFrame);
                treeView1.Nodes.Add(g);
                index++;
            }
            glViewport.MaxFrame = max;

            treeView1.ExpandAll();
        }

        /// <summary>
        /// 
        /// </summary>
        public void GLInit()
        {
            foreach (MapHeadGroup v in treeView1.Nodes)
            {
                if (v.Loaded)
                    v.Load();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void GLFree()
        {
            foreach (MapHeadGroup v in treeView1.Nodes)
            {
                v.Free();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            // remove dead
            for (int i = treeView1.Nodes.Count - 1; i >= 0; i--)
                if (treeView1.Nodes[i] is MapHeadGroup g &&  g.Dead)
                {
                    g.Free();
                    treeView1.Nodes.RemoveAt(i);
                }

            // render all groups
            foreach (MapHeadGroup v in treeView1.Nodes)
            {
                if (v.Checked)
                {
                    if (!v.Loaded)
                        v.Load();

                    v.renderJObj.Render(cam);
                }
            }
        }

        private bool UpdatingCheckState = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (UpdatingCheckState)
                return;

            if (e.Node is MapHeadAnimation anim)
            {
                UpdatingCheckState = true;
                // uncheck all siblings
                foreach (TreeNode n in anim.Parent.Nodes)
                {
                    if (n != anim)
                        n.Checked = false;
                }
                UpdatingCheckState = false;

                // load animation
                if (e.Node.Checked && e.Node.Parent is MapHeadGroup g)
                {
                    g.renderJObj.LoadAnimation(new JointAnimManager(anim.joint), anim.matanim, null);
                    g.renderJObj.RequestAnimationUpdate(FrameFlags.All, glViewport.Frame);
                }
            }

        }
    }
}
