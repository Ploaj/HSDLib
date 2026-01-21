using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using System;
using System.Collections.Generic;

namespace HSDRawViewer.Rendering.Animation
{
    public class ShapeAnimJoint
    {
        public List<ShapeAnim> Nodes = new();
    }

    public class ShapeAnim
    {
        public List<FOBJ_Player> Tracks = new();

        public float Frame = 0;

        public void SetFrame(float frame)
        {
            Frame = frame;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ShapeAnimState
    {
        public float Blending { get; }

        public ShapeAnimState(float blending)
        {
            Blending = blending;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ShapeAnimManager : AnimManager
    {
        public override int NodeCount => Nodes.Count;

        public List<ShapeAnimJoint> Nodes { get; internal set; } = new List<ShapeAnimJoint>();

        public int JOBJIndex = 0;

        public int DOBJIndex = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mobj"></param>
        /// <returns></returns>
        public float GetBlending()
        {
            float blending = 0;

            if (Nodes.Count > JOBJIndex && Nodes[JOBJIndex].Nodes.Count > DOBJIndex)
            {
                ShapeAnim node = Nodes[JOBJIndex].Nodes[DOBJIndex];

                foreach (FOBJ_Player t in node.Tracks)
                {
                    switch ((ShapeTrackType)t.TrackType)
                    {
                        case ShapeTrackType.HSD_A_S_BLEND: blending = t.GetValue(node.Frame); break;
                    }
                }
            }

            return blending;
        }

        /// <summary>
        /// 
        /// </summary>
        public ShapeAnimManager FromShapeAnim(HSD_ShapeAnimJoint joint)
        {
            Nodes.Clear();
            FrameCount = 0;

            if (joint == null)
                return this;

            foreach (HSD_ShapeAnimJoint j in joint.TreeList)
            {
                ShapeAnimJoint matjoint = new();
                if (j.ShapeAnimation != null)
                    foreach (HSD_ShapeAnim a in j.ShapeAnimation.List)
                    {
                        ShapeAnim anm = new();

                        if (a.Animation != null)
                        {
                            FrameCount = (int)Math.Max(FrameCount, a.Animation.AnimationObject.EndFrame);

                            foreach (HSD_FOBJDesc fdesc in a.Animation.AnimationObject.FObjDesc.List)
                                anm.Tracks.Add(new FOBJ_Player(fdesc));
                        }

                        matjoint.Nodes.Add(anm);
                    }
                Nodes.Add(matjoint);
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        public void SetAllFrames(float frame)
        {
            foreach (ShapeAnimJoint v in Nodes)
                foreach (ShapeAnim n in v.Nodes)
                    n.SetFrame(frame);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ShapeAnim GetShapeAnimAtIndex(int index)
        {
            int i = 0;
            foreach (ShapeAnimJoint v in Nodes)
            {
                foreach (ShapeAnim n in v.Nodes)
                {
                    if (index == i)
                        return n;
                    i++;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="frame"></param>
        public void SetFrame(int mat_index, float frame)
        {
            ShapeAnim node = GetShapeAnimAtIndex(mat_index);
            if (node != null)
                node.SetFrame(frame);
        }
    }
}
