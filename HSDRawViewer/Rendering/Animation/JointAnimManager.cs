using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using HSDRawViewer.Converters;
using HSDRawViewer.Converters.Animation;
using HSDRawViewer.GUI.Dialog;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Tools.Animation;
using IONET.Collada.Kinematics.Joints;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace HSDRawViewer.Rendering
{
    /// <summary>
    /// 
    /// </summary>
    public interface IJointFrameModifier
    {
        bool OverrideAnim(LiveJObj jobj, float frame);
    }

    /// <summary>
    /// 
    /// </summary>
    public class JointAnimManager : AnimManager
    {
        public override int NodeCount => Nodes.Count;

        public List<AnimNode> Nodes { get; internal set; } = new List<AnimNode>();

        private int index = 0;

        public List<IJointFrameModifier> FrameModifier { get; set; } = new List<IJointFrameModifier>();

        public bool EnableBoneLookup { get; set; } = false;

        public Dictionary<int, int> BoneLookup { get; internal set; } = new Dictionary<int, int>();

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
        /// <param name="jointIndex"></param>
        /// <returns></returns>
        public bool GetJointBranchState(float frame, int boneIndex, out float value)
        {
            // set keys to animated values
            if (boneIndex < Nodes.Count)
            {
                AnimNode node = Nodes[boneIndex];
                foreach (FOBJ_Player t in node.Tracks)
                {
                    switch (t.JointTrackType)
                    {
                        case JointTrackType.HSD_A_J_BRANCH:
                            value = t.GetValue(frame);
                            return true;
                    }
                }
            }

            value = 0;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        public virtual void ApplyAnimation(LiveJObj jobj, float frame)
        {
            if (jobj == null)
                return;

            foreach (LiveJObj j in jobj.Enumerate)
            {
                // use bone lookup if enabled
                int boneIndex = j.Index;
                if (EnableBoneLookup)
                {
                    if (BoneLookup.ContainsKey(boneIndex))
                        boneIndex = BoneLookup[boneIndex];
                    else
                        return;
                }

                // set keys to animated values
                if (boneIndex < Nodes.Count)
                {
                    AnimNode node = Nodes[boneIndex];
                    j.ApplyAnimation(node.Tracks, frame);
                }

                // TODO: apply frame modifiers
                foreach (IJointFrameModifier fm in FrameModifier)
                    fm.OverrideAnim(j, frame);
            }
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
            foreach (FigaTreeNode tracks in tree.Nodes)
            {
                AnimNode n = new();
                foreach (HSD_Track t in tracks.Tracks)
                {
                    n.Tracks.Add(new FOBJ_Player(t.TrackType, t.GetKeys()));
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
        public HSD_FigaTree ToFigaTree(float error = 0.0001f)
        {
            HSD_FigaTree tree = new();
            tree.FrameCount = FrameCount;
            tree.Type = 1;
            tree.Nodes = Nodes.Select(e =>
            {
                FigaTreeNode fn = new();

                foreach (FOBJ_Player t in e.Tracks)
                {
                    HSD_Track track = new();
                    track.FromFOBJ(t.ToFobj(error));
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

            foreach (HSD_AnimJoint j in joint.TreeList)
            {
                AnimNode n = new();
                if (j.AOBJ != null && j.AOBJ.FObjDesc != null)
                {
                    FrameCount = (int)Math.Max(FrameCount, j.AOBJ.EndFrame + 1);

                    foreach (HSD_FOBJDesc fdesc in j.AOBJ.FObjDesc.List)
                    {
                        FOBJ_Player players = new(fdesc);
                        if (players.Keys != null && players.Keys.Count > 0)
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
            HSD_AnimJoint joint = new();
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
            foreach (FOBJ_Player t in n.Tracks)
            {
                joint.AOBJ.EndFrame = Math.Max(joint.AOBJ.EndFrame, t.FrameCount);

                HSD_FOBJDesc fobj = t.ToFobjDesc();

                if (joint.AOBJ.FObjDesc == null)
                    joint.AOBJ.FObjDesc = fobj;
                else
                    joint.AOBJ.FObjDesc.Add(fobj);
            }

            // set particle flag
            if (n.Tracks.Any(e => e.JointTrackType == JointTrackType.HSD_A_J_PTCL))
            {
                joint.AOBJ.EndFrame += 0.1f;
            }

            foreach (HSD_JOBJ c in root.Children)
            {
                joint.AddChild(ToAnimJointRecursive(c, flags));
            }

            return joint;
        }

        #endregion

        #region Tools

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fsms"></param>
        public void ApplyFSMs(IEnumerable<FrameSpeedMultiplier> fsms, bool compress = true)
        {
            foreach (AnimNode n in Nodes)
                foreach (FOBJ_Player t in n.Tracks)
                    t.ApplyFSMs(fsms, compress);

            // calculate new frame count
            float frameRate = 1;
            float lastFrame = 0;
            float frameCount = 0;
            foreach (FrameSpeedMultiplier fsm in fsms)
            {
                float dis = fsm.Frame - lastFrame;
                frameCount += dis / frameRate;

                frameRate = fsm.Rate;
                lastFrame = fsm.Frame;
            }
            float finaldistance = FrameCount - lastFrame;
            frameCount += finaldistance / frameRate;

            FrameCount = frameCount;

            // recalculate frame count
            //FrameCount = 0;
            //foreach (var n in Nodes)
            //    foreach (var t in n.Tracks)
            //    {
            //        var maxFrame = t.Keys.Max(e => e.Frame);
            //        FrameCount = Math.Max(FrameCount, maxFrame);
            //    }
        }

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

            float adjust = (endFrame - startFrame) - value * (endFrame - startFrame);
            FrameCount -= adjust;
            foreach (AnimNode n in Nodes)
            {
                foreach (FOBJ_Player t in n.Tracks)
                {
                    int keyIndex = 0;
                    foreach (FOBJKey k in t.Keys)
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

        internal void Bake()
        {
            foreach (AnimNode n in Nodes)
            {
                foreach (FOBJ_Player t in n.Tracks)
                {
                    t.Bake();
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

            foreach (AnimNode n in Nodes)
            {
                foreach (FOBJ_Player t in n.Tracks)
                {
                    // the new set of keys after trimming
                    List<FOBJKey> newKeys = t.Keys.Where(e => e.Frame >= startFrame && e.Frame <= endFrame).ToList();

                    // if there's not a key on the startframe, it'll create a new one
                    if (!t.Keys.Any(k => k.Frame == startFrame))
                    {
                        FOBJKey start = new()
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
                        FOBJKey end = new()
                        {
                            Frame = endFrame,
                            Tan = 0,
                            Value = t.GetValue(endFrame),
                            InterpolationType = GXInterpolationType.HSD_A_OP_LIN
                        };
                        newKeys.Add(end);
                    }

                    // finally shifts all the keys
                    foreach (FOBJKey k in newKeys)
                    {
                        k.Frame -= startFrame;
                    }

                    // assigns the new keys
                    t.Keys = newKeys;
                }
            }
        }

        #endregion



        /// <summary>
        /// 
        /// </summary>
        public void Optimize(HSD_JOBJ jobj, bool applyDiscontinuityFilter, float error = 0.001f)
        {
            if (jobj == null)
                return;

            List<HSD_JOBJ> joints = jobj.TreeList;

            if (NodeCount > joints.Count)
            {
                Nodes.RemoveRange(joints.Count, NodeCount - joints.Count);
            }

            for (int i = 0; i < Math.Min(joints.Count, NodeCount); i++)
            {
                if (applyDiscontinuityFilter)
                    Tools.KeyFilters.DiscontinuityFilter.Filter(Nodes[i].Tracks);

                AnimationKeyCompressor.OptimizeJointTracks(joints[i], ref Nodes[i].Tracks, error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_jointMap"></param>
        /// <returns></returns>
        public static JointAnimManager LoadFromFile(JointMap _jointMap)
        {
            string f = Tools.FileIO.OpenFile(JointAnimationLoader.SupportedImportAnimFilter);

            if (f != null)
            {
                return JointAnimationLoader.LoadJointAnimFromFile(_jointMap, f);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ExportAsMayaAnim(JointMap _jointMap)
        {
            string f = Tools.FileIO.SaveFile("Supported Formats (*.anim)|*.anim");

            if (f != null)
                ConvMayaAnim.ExportToMayaAnim(f, this, _jointMap);
        }

        /// <summary>
        /// 
        /// </summary>
        public class AnimJointSettings
        {
            [DisplayName("Symbol Name"), Description("Should end in _animjoint")]
            public string Symbol { get; set; } = "_animjoint";

            [DisplayName("Flags"), Description("")]
            public AOBJ_Flags Flags { get; set; } = AOBJ_Flags.ANIM_LOOP;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ExportAsAnimJoint(HSD_JOBJ root)
        {
            string f = Tools.FileIO.SaveFile(ApplicationSettings.HSDFileFilter);

            AnimJointSettings setting = new();

            using PropertyDialog d = new("AnimJoint Settings", setting);
            if (f != null && d.ShowDialog() == DialogResult.OK)
            {
                HSDRawFile animFile = new();
                animFile.Roots.Add(new HSDRootNode()
                {
                    Data = ToAnimJoint(root, setting.Flags),
                    Name = setting.Symbol
                });
                animFile.Save(f);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class FigaTreeSettings
        {
            [DisplayName("Symbol Name"), Description("Name of animation used by the game")]
            public string Symbol { get; set; } = "_figatree";

            [DisplayName("Compression Error"), Description("A larger value will make a smaller file but with loss of accuracy")]
            public float CompressionError { get; set; } = 0.0001f;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ExportAsFigatree()
        {
            string f = Tools.FileIO.SaveFile(ApplicationSettings.HSDFileFilter);
            FigaTreeSettings setting = new();

            using PropertyDialog d = new("Figatree Settings", setting);
            if (f != null && d.ShowDialog() == DialogResult.OK)
            {
                HSDRawFile animFile = new();
                animFile.Roots.Add(new HSDRootNode()
                {
                    Data = ToFigaTree(setting.CompressionError),
                    Name = setting.Symbol
                });
                animFile.Save(f);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="joint"></param>
        /// <param name="player"></param>
        private static void FOBJPlayer_AddKey(FOBJ_Player track, float value, float end)
        {
            track.Keys.Add(new FOBJKey() { Frame = 0, Value = value, InterpolationType = GXInterpolationType.HSD_A_OP_LIN });
            track.Keys.Add(new FOBJKey() { Frame = end, Value = value, InterpolationType = GXInterpolationType.HSD_A_OP_LIN });
            track.Bake();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="joint"></param>
        /// <param name="player"></param>
        private static FOBJ_Player GenerateKey(HSD_JOBJ joint, JointTrackType type, float end)
        {
            var track = new FOBJ_Player() { TrackType = (byte)type };
            switch (type)
            {
                case JointTrackType.HSD_A_J_TRAX: FOBJPlayer_AddKey(track, joint.TX, end); break;
                case JointTrackType.HSD_A_J_TRAY: FOBJPlayer_AddKey(track, joint.TY, end); break;
                case JointTrackType.HSD_A_J_TRAZ: FOBJPlayer_AddKey(track, joint.TZ, end); break;
                case JointTrackType.HSD_A_J_ROTX: FOBJPlayer_AddKey(track, joint.RX, end); break;
                case JointTrackType.HSD_A_J_ROTY: FOBJPlayer_AddKey(track, joint.RY, end); break;
                case JointTrackType.HSD_A_J_ROTZ: FOBJPlayer_AddKey(track, joint.RZ, end); break;
                case JointTrackType.HSD_A_J_SCAX: FOBJPlayer_AddKey(track, joint.SX, end); break;
                case JointTrackType.HSD_A_J_SCAY: FOBJPlayer_AddKey(track, joint.SY, end); break;
                case JointTrackType.HSD_A_J_SCAZ: FOBJPlayer_AddKey(track, joint.SZ, end); break;
            }
            return track;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toAppend"></param>
        /// <param name="frame_count"></param>
        public void AppendAnimation(JointAnimManager toAppend, HSD_JOBJ model, bool no_bake = false, int frame_count = -1)
        {
            if (!no_bake)
                Bake();

            if (frame_count < 0)
                frame_count = (int)Math.Ceiling(toAppend.FrameCount);

            int i = 0;
            foreach (var c in toAppend.Nodes)
            {

                foreach (var t in c.Tracks)
                {
                    var track = Nodes[i].Tracks.Find(e => e.TrackType == t.TrackType);

                    if (track == null)
                    {
                        var j = model.TreeList[i];
                        track = GenerateKey(j, t.JointTrackType, FrameCount);
                        Nodes[i].Tracks.Add(track);
                    }

                    float frame = track.Keys.Max(e => e.Frame) + 1;
                    for (int j = 0; j < frame_count; j++)
                    {
                        track.Keys.Add(new FOBJKey()
                        {
                            Frame = frame + j,
                            Value = t.GetValue(j),
                            InterpolationType = GXInterpolationType.HSD_A_OP_LIN
                        });
                    }
                }
                i++;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="jointMap"></param>
        internal void ExportAsSMD(HSD_JOBJ root, JointMap jointMap)
        {
            string f = Tools.FileIO.SaveFile(ApplicationSettings.SMDFileFilter);
            if (f != null)
            {
                SMDConv.ExportAnimationToSMD(f, root, jointMap, this);
            }
        }
    }
}