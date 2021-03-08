using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HSDRawViewer.Rendering
{
    /// <summary>
    /// 
    /// </summary>
    public interface IJointFrameModifier
    {
        bool OverrideAnim(float frame, int boneIndex, HSD_JOBJ jobj, ref float TX, ref float TY, ref float TZ, ref float RX, ref float RY, ref float RZ, ref float SX, ref float SY, ref float SZ);
    }

    /// <summary>
    /// 
    /// </summary>
    public class JointAnimManager : AnimManager
    {
        public override int NodeCount => Nodes.Count;

        public List<AnimNode> Nodes { get; internal set; } = new List<AnimNode>();

        private int index = 0;

        public List<IJointFrameModifier> FrameModifier = new List<IJointFrameModifier>();

        /// <summary>
        /// 
        /// </summary>
        public JointAnimManager()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public JointAnimManager(int jointCount)
        {
            for (int i = 0; i < jointCount; i++)
                Nodes.Add(new AnimNode() { Tracks = new List<FOBJ_Player>() });
        }

        /// <summary>
        /// 
        /// </summary>
        public JointAnimManager(HSD_FigaTree tree)
        {
            FromFigaTree(tree);
        }

        /// <summary>
        /// 
        /// </summary>
        public JointAnimManager(HSD_AnimJoint joint)
        {
            FromAnimJoint(joint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="boneIndex"></param>
        /// <param name="jobj"></param>
        /// <returns></returns>
        public virtual Matrix4 GetAnimatedMatrix(float frame, int boneIndex, HSD_JOBJ jobj)
        {
            GetAnimatedState(frame, boneIndex, jobj, out float TX, out float TY, out float TZ, out float RX, out float RY, out float RZ, out float SX, out float SY, out float SZ);

            return Matrix4.CreateScale(SX, SY, SZ) *
                Matrix4.CreateFromQuaternion(Math3D.FromEulerAngles(RZ, RY, RX)) *
                Matrix4.CreateTranslation(TX, TY, TZ);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="boneIndex"></param>
        /// <param name="jobj"></param>
        /// <returns></returns>
        public virtual void GetAnimatedState(float frame, int boneIndex, HSD_JOBJ jobj, out float TX, out float TY, out float TZ, out float RX, out float RY, out float RZ, out float SX, out float SY, out float SZ)
        {
            TX = jobj.TX;
            TY = jobj.TY;
            TZ = jobj.TZ;
            RX = jobj.RX;
            RY = jobj.RY;
            RZ = jobj.RZ;
            SX = jobj.SX;
            SY = jobj.SY;
            SZ = jobj.SZ;

            if (boneIndex < Nodes.Count)
            {
                AnimNode node = Nodes[boneIndex];
                foreach (FOBJ_Player t in node.Tracks)
                {
                    switch (t.JointTrackType)
                    {
                        case JointTrackType.HSD_A_J_ROTX: RX = t.GetValue(frame); break;
                        case JointTrackType.HSD_A_J_ROTY: RY = t.GetValue(frame); break;
                        case JointTrackType.HSD_A_J_ROTZ: RZ = t.GetValue(frame); break;
                        case JointTrackType.HSD_A_J_TRAX: TX = t.GetValue(frame); break;
                        case JointTrackType.HSD_A_J_TRAY: TY = t.GetValue(frame); break;
                        case JointTrackType.HSD_A_J_TRAZ: TZ = t.GetValue(frame); break;
                        case JointTrackType.HSD_A_J_SCAX: SX = t.GetValue(frame); break;
                        case JointTrackType.HSD_A_J_SCAY: SY = t.GetValue(frame); break;
                        case JointTrackType.HSD_A_J_SCAZ: SZ = t.GetValue(frame); break;
                    }
                }
            }

            foreach (var fm in FrameModifier)
                fm.OverrideAnim(frame, boneIndex, jobj, ref TX, ref TY, ref TZ, ref RX, ref RY, ref RZ, ref SX, ref SY, ref SZ);
        }

        #region AnimationTypeLoading

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public void FromFigaTree(HSD_FigaTree tree)
        {
            if (tree == null)
                return;

            FrameCount = tree.FrameCount;
            Nodes = new List<AnimNode>();
            foreach (var tracks in tree.Nodes)
            {
                AnimNode n = new AnimNode();
                foreach (HSD_Track t in tracks.Tracks)
                {
                    n.Tracks.Add(new FOBJ_Player(t.FOBJ));
                }
                Nodes.Add(n);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="frameCount"></param>
        /// <returns></returns>
        public HSD_FigaTree ToFigaTree()
        {
            HSD_FigaTree tree = new HSD_FigaTree();
            tree.FrameCount = FrameCount;
            tree.Type = 1;
            tree.Nodes = Nodes.Select(e =>
            {
                var fn = new FigaTreeNode();

                foreach (var t in e.Tracks)
                {
                    HSD_Track track = new HSD_Track();
                    HSD_FOBJ fobj = t.ToFobj();
                    track.FOBJ = fobj;
                    fn.Tracks.Add(track);
                }

                return fn;
            }).ToList();
            return tree;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joint"></param>
        public void FromAnimJoint(HSD_AnimJoint joint)
        {
            Nodes.Clear();
            FrameCount = 0;

            if (joint == null)
                return;

            foreach (var j in joint.BreathFirstList)
            {
                AnimNode n = new AnimNode();
                if (j.AOBJ != null)
                {
                    FrameCount = (int)Math.Max(FrameCount, j.AOBJ.EndFrame + 1);

                    foreach (var fdesc in j.AOBJ.FObjDesc.List)
                    {
                        var players = new FOBJ_Player(fdesc);
                        if(players.Keys != null && players.Keys.Count > 0)
                            FrameCount = Math.Max(FrameCount, players.Keys.Max(e => e.Frame + 1));
                        n.Tracks.Add(players);
                    }
                }

                Nodes.Add(n);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="nodes"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public HSD_AnimJoint ToAnimJoint(HSD_JOBJ root, AOBJ_Flags flags)
        {
            index = 0;
            return ToAnimJointRecursive(root, flags);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="nodes"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        private HSD_AnimJoint ToAnimJointRecursive(HSD_JOBJ root, AOBJ_Flags flags)
        {
            HSD_AnimJoint joint = new HSD_AnimJoint();
            joint.Flags = 1;

            AnimNode n = null;

            if (index >= Nodes.Count)
                n = new AnimNode();
            else
                n = Nodes[index++];

            // set flags
            if (n.Tracks.Count > 0)
            {
                joint.AOBJ = new HSD_AOBJ();
                joint.AOBJ.Flags = flags;
            }

            // add all tracks
            foreach (var t in n.Tracks)
            {
                joint.AOBJ.EndFrame = Math.Max(joint.AOBJ.EndFrame, t.FrameCount);

                HSD_FOBJDesc fobj = t.ToFobjDesc();

                if (joint.AOBJ.FObjDesc == null)
                    joint.AOBJ.FObjDesc = fobj;
                else
                    joint.AOBJ.FObjDesc.Add(fobj);
            }

            // set particle flag
            if (n.Tracks.Any(e=>e.JointTrackType == JointTrackType.HSD_A_J_PTCL))
            {
                joint.AOBJ.EndFrame += 0.1f;
            }

            foreach (var c in root.Children)
            {
                joint.AddChild(ToAnimJointRecursive(c, flags));
            }

            return joint;
        }

        #endregion

        #region Tools

        /// <summary>
        /// Scales animation frames to be new size
        /// </summary>
        /// <param name="newFrameCount"></param>
        public void ScaleToLength(int newFrameCount, int startFrame, int endFrame)
        {
            if (newFrameCount == FrameCount || newFrameCount == 0)
                return;

            ScaleBy(newFrameCount / FrameCount, startFrame, endFrame);
        }

        /// <summary>
        /// Scales the frame keys by given factor
        /// </summary>
        /// <param name="value"></param>
        public void ScaleBy(float value, int startFrame, int endFrame)
        {
            if (value == 1)
                return;

            var adjust = (endFrame - startFrame) - value * (endFrame - startFrame);
            FrameCount -= adjust;
            foreach (var n in Nodes)
            {
                foreach (var t in n.Tracks)
                {
                    int keyIndex = 0;
                    foreach (var k in t.Keys)
                    {
                        // scale frames in range
                        if (k.Frame >= startFrame && k.Frame <= endFrame)
                        {
                            k.Frame = (float)Math.Round(k.Frame * value);
                            k.Tan = (float)(Math.Atan2(Math.Tan(k.Tan), value) * Math.PI / 180);
                        }
                        else
                        // adjust range
                        if (k.Frame > endFrame)
                            k.Frame -= adjust;
                        keyIndex++;
                    }

                    // remove keys that share frames
                    t.Keys = t.Keys.GroupBy(x => x.Frame).Select(g => g.First()).ToList();
                }
            }
        }

        /// <summary>
        /// Trims frames to given region
        /// </summary>
        /// <param name="startFrame"></param>
        /// <param name="endFrame"></param>
        public virtual void Trim(int startFrame, int endFrame)
        {
            if (startFrame == 0 && endFrame == FrameCount)
                return;

            FrameCount = endFrame - startFrame;

            foreach (var n in Nodes)
            {
                foreach (var t in n.Tracks)
                {

                    // the new set of keys after trimming
                    List<FOBJKey> newKeys = t.Keys.Where(e => e.Frame >= startFrame && e.Frame <= endFrame).ToList();

                    // if there's not a key on the startframe, it'll create a new one
                    if (!t.Keys.Any(k => k.Frame == startFrame))
                    {
                        FOBJKey start = new FOBJKey()
                        {
                            Frame = startFrame,
                            Tan = 0,
                            Value = t.GetValue(startFrame),
                            InterpolationType = GXInterpolationType.HSD_A_OP_LIN
                        };
                        newKeys.Insert(0, start);
                    }

                    // same but for the last frame
                    if (!t.Keys.Any(k => k.Frame == endFrame))
                    {
                        FOBJKey end = new FOBJKey()
                        {
                            Frame = endFrame,
                            Tan = 0,
                            Value = t.GetValue(endFrame),
                            InterpolationType = GXInterpolationType.HSD_A_OP_LIN
                        };
                        newKeys.Add(end);
                    }

                    // finally shifts all the keys
                    foreach (var k in newKeys)
                    {
                        k.Frame -= startFrame;
                    }

                    // assigns the new keys
                    t.Keys = newKeys;
                }
            }
        }

        #endregion
    }
}