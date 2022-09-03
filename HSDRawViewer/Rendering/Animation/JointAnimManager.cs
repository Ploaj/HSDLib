using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using HSDRawViewer.Converters;
using HSDRawViewer.Converters.Animation;
using HSDRawViewer.GUI.Dialog;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Tools;
using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

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

            foreach (var j in jobj.Enumerate)
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
                foreach (var fm in FrameModifier)
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
            foreach (var tracks in tree.Nodes)
            {
                AnimNode n = new AnimNode();
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
            HSD_FigaTree tree = new HSD_FigaTree();
            tree.FrameCount = FrameCount;
            tree.Type = 1;
            tree.Nodes = Nodes.Select(e =>
            {
                var fn = new FigaTreeNode();

                foreach (var t in e.Tracks)
                {
                    HSD_Track track = new HSD_Track();
                    HSD_FOBJ fobj = t.ToFobj(error);
                    track.FromFOBJ(fobj);
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

            foreach (var j in joint.ToList)
            {
                AnimNode n = new AnimNode();
                if (j.AOBJ != null && j.AOBJ.FObjDesc != null)
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
        /// 
        /// </summary>
        /// <param name="fsms"></param>
        public void ApplyFSMs(IEnumerable<FrameSpeedMultiplier> fsms)
        {
            foreach (var n in Nodes)
                foreach (var t in n.Tracks)
                    t.ApplyFSMs(fsms);

            // calculate new frame count
            float frameRate = 1;
            float lastFrame = 0;
            float frameCount = 0;
            foreach (var fsm in fsms)
            {
                var dis = fsm.Frame - lastFrame;
                frameCount += dis / frameRate;

                frameRate = fsm.Rate;
                lastFrame = fsm.Frame;
            }
            var finaldistance = FrameCount - lastFrame;
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



        /// <summary>
        /// 
        /// </summary>
        public void Optimize(HSD_JOBJ jobj, float error = 0.001f)
        {
            if (jobj == null)
                return;

            var joints = jobj.ToList;

            if (joints.Count != NodeCount)
                return;

            for (int i = 0; i < NodeCount; i++)
                AnimationKeyCompressor.OptimizeJointTracks(joints[i], ref Nodes[i].Tracks, error);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_jointMap"></param>
        /// <returns></returns>
        public static JointAnimManager LoadFromFile(JointMap _jointMap)
        {
            var f = Tools.FileIO.OpenFile("FigaTree/AnimJoint/MayaAnim (*.dat*.anim*.chr0)|*.dat;*.anim;*.chr0;");
            //var f = Tools.FileIO.OpenFile("FigaTree/AnimJoint/MayaAnim/EightingMOT (*.dat*.anim*.mota*.gnta*.xml)|*.dat;*.anim;*.mota;*.gnta;*.chr0;*.xml");

            if (f != null)
            {
                return JointAnimationLoader.LoadJointAnimFromFile(_jointMap, f);
                //if (Path.GetExtension(f).ToLower().Equals(".mota") || Path.GetExtension(f).ToLower().Equals(".gnta") ||
                //    (Path.GetExtension(f).ToLower().Equals(".xml") && MOT_FILE.IsMotXML(f)))
                //{
                //    var jointTable = Tools.FileIO.OpenFile("Joint Connector Value (*.jcv)|*.jcv");

                //    if (jointTable != null)
                //        LoadAnimation(MOTLoader.GetJointTable(jointTable), new MOT_FILE(f));
                //}
                //else
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ExportAsMayaAnim(JointMap _jointMap)
        {
            var f = Tools.FileIO.SaveFile("Supported Formats (*.anim)|*.anim");

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
            var f = Tools.FileIO.SaveFile(ApplicationSettings.HSDFileFilter);

            var setting = new AnimJointSettings();

            using (PropertyDialog d = new PropertyDialog("AnimJoint Settings", setting))
                if (f != null && d.ShowDialog() == DialogResult.OK)
                {
                    HSDRawFile animFile = new HSDRawFile();
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
            var f = Tools.FileIO.SaveFile(ApplicationSettings.HSDFileFilter);
            var setting = new FigaTreeSettings();

            using (PropertyDialog d = new PropertyDialog("Figatree Settings", setting))
                if (f != null && d.ShowDialog() == DialogResult.OK)
                {
                    HSDRawFile animFile = new HSDRawFile();
                    animFile.Roots.Add(new HSDRootNode()
                    {
                        Data = ToFigaTree(setting.CompressionError),
                        Name = setting.Symbol
                    });
                    animFile.Save(f);
                }
        }
    }
}