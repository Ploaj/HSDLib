using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using HSDRawViewer.Converters;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HSDRawViewer.Rendering
{
    /// <summary>
    /// 
    /// </summary>
    public class AnimNode
    {
        public List<FOBJ_Player> Tracks = new List<FOBJ_Player>();
    }

    /// <summary>
    /// 
    /// </summary>
    public class AnimManager
    {
        public int NodeCount { get => Nodes.Count; }

        public List<AnimNode> Nodes { get; internal set; } = new List<AnimNode>();

        public float FrameCount = 0;

        private int index = 0;

        private MOT_FILE _motFile;
        private short[] _motJointTable;

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
                    foreach (var k in t.Keys)
                    {
                        // scale frames in range
                        if(k.Frame >= startFrame && k.Frame <= endFrame)
                        {
                            k.Frame = (float)Math.Round(k.Frame * value);
                            //TODO: How do you scale tangent?
                            k.Tan *= value;// Math.Sign(k.Tan) * (float)Math.Log(Math.Abs(k.Tan), value);
                        }
                        else
                        // adjust range
                        if (k.Frame > endFrame)
                            k.Frame -= adjust;
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
        public void Trim(int startFrame, int endFrame)
        {
            if (startFrame == 0 && endFrame == FrameCount)
                return;

            FrameCount = endFrame - startFrame;

            foreach(var n in Nodes)
            {
                foreach(var t in n.Tracks)
                {
                    t.Keys = t.Keys.Where(e => e.Frame >= startFrame && e.Frame <= startFrame + endFrame).ToList();
                    foreach (var k in t.Keys)
                        k.Frame -= startFrame;
                }
            }
        }

        #endregion

        #region AnimationTypeLoading

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jointTable"></param>
        /// <param name="file"></param>
        public void SetMOT(short[] jointTable, MOT_FILE file)
        {
            _motFile = file;
            _motJointTable = jointTable;
            FrameCount = (int)Math.Ceiling(_motFile.EndTime * 60);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="boneIndex"></param>
        /// <param name="jobj"></param>
        /// <returns></returns>
        public Matrix4 GetAnimatedState(float frame, int boneIndex, HSD_JOBJ jobj)
        {
            float TX = jobj.TX;
            float TY = jobj.TY;
            float TZ = jobj.TZ;
            float RX = jobj.RX;
            float RY = jobj.RY;
            float RZ = jobj.RZ;
            float SX = jobj.SX;
            float SY = jobj.SY;
            float SZ = jobj.SZ;

            Quaternion rotationOverride = Quaternion.Identity;
            bool overrideRotation = false;

            if(_motFile != null)
            {
                var joints = _motFile.Joints.FindAll(e => e.BoneID >= 0 && e.BoneID < _motJointTable.Length && _motJointTable[e.BoneID] == boneIndex);

                foreach (var j in joints)
                {
                    var key = j.GetKey(frame / 60f);

                    if (j.TrackFlag.HasFlag(MOT_FLAGS.TRANSLATE))
                    {
                        TX += key.X;
                        TY += key.Y;
                        TZ += key.Z;
                    }
                    if (j.TrackFlag.HasFlag(MOT_FLAGS.SCALE))
                    {
                        SX += key.X;
                        SY += key.Y;
                        SZ += key.Z;
                    }
                    if (j.TrackFlag.HasFlag(MOT_FLAGS.ROTATE))
                    {
                        overrideRotation = true;
                        rotationOverride = Math3D.FromEulerAngles(RZ, RY, RX);

                        var dir = new Vector3(key.X, key.Y, key.Z);
                        var angle = key.W;

                        float rot_angle = (float)Math.Acos(Vector3.Dot(Vector3.UnitX, dir));
                        if (Math.Abs(rot_angle) > 0.000001f)
                        {
                            Vector3 rot_axis = Vector3.Cross(Vector3.UnitX, dir).Normalized();
                            rotationOverride *= Quaternion.FromAxisAngle(rot_axis, rot_angle);
                        }

                        rotationOverride *= Quaternion.FromEulerAngles(angle * (float)Math.PI / 180, 0, 0);
                    }
                }
            }
            else if(boneIndex < Nodes.Count)
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

            return Matrix4.CreateScale(SX, SY, SZ) *
                Matrix4.CreateFromQuaternion(overrideRotation ? rotationOverride : Math3D.FromEulerAngles(RZ, RY, RX)) *
                Matrix4.CreateTranslation(TX, TY, TZ);
        }

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
                    HSD_FOBJ fobj = new HSD_FOBJ();
                    fobj.SetKeys(t.Keys, t.JointTrackType);
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
                    FrameCount = (int)Math.Max(FrameCount, j.AOBJ.EndFrame);

                    foreach (var fdesc in j.AOBJ.FObjDesc.List)
                        n.Tracks.Add(new FOBJ_Player(fdesc));
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
            var n = Nodes[index++];

            if (n.Tracks.Count > 0)
            {
                joint.AOBJ = new HSD_AOBJ();
                joint.AOBJ.Flags = flags;
            }
            foreach (var t in n.Tracks)
            {
                joint.AOBJ.EndFrame = Math.Max(joint.AOBJ.EndFrame, t.FrameCount);

                HSD_FOBJDesc fobj = new HSD_FOBJDesc();
                fobj.SetKeys(t.Keys, (byte)t.TrackType);

                if (joint.AOBJ.FObjDesc == null)
                    joint.AOBJ.FObjDesc = fobj;
                else
                    joint.AOBJ.FObjDesc.Add(fobj);
            }

            foreach (var c in root.Children)
            {
                joint.AddChild(ToAnimJointRecursive(c, flags));
            }

            return joint;
        }

        #endregion
    }
}