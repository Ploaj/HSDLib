using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDRawViewer.Converters
{
    public enum MexMenuAnimPresets
    {
        None,
        SlideInFromLeft,
        SlideInFromRight,
        SlideInFromTop,
        SlideInFromBottom,
        GrowFromNothing,
        SpinIn,
        FlipIn
    }

    public class MexMenuAnimation
    {
        //[Description("Animation Preset Types")]
        //public MexMenuAnimPresets Presets { get; set; } = MexMenuAnimPresets.None;

        [Description("Start Frame")]
        public int StartFrame { get; set; }

        [Description("End Frame")]
        public int EndFrame { get; set; }

        [Description("")]
        public float StartingPositionX { get; set; }

        [Description("")]
        public float StartingPositionY { get; set; }

        [Description("")]
        public float StartingPositionZ { get; set; }

        [Description("")]
        public float StartingRotationX { get; set; }

        [Description("")]
        public float StartingRotationY { get; set; }

        [Description("")]
        public float StartingRotationZ { get; set; }

        [Description("")]
        public float StartingScaleX { get; set; }

        [Description("")]
        public float StartingScaleY { get; set; }

        [Description("")]
        public float StartingScaleZ { get; set; }
    }

    public class MexMenuAnimationGenerator
    {/// <summary>
     /// 
     /// </summary>
     /// <param name="space"></param>
     /// <returns></returns>
        public static HSD_AnimJoint GenerateAnimJoint(MexMenuAnimation anim, HSD_JOBJ joint)
        {
            HSD_AnimJoint aj = new HSD_AnimJoint();

            if (anim == null || anim.StartFrame >= anim.EndFrame)
                return aj;
            
            var aobj = new HSD_AOBJ();
            aobj.EndFrame = 1600;
            aobj.Flags = AOBJ_Flags.FIRST_PLAY;

            if(anim.StartingPositionX != joint.TX)
                GenerateFOBJ(aobj, anim.StartingPositionX, joint.TX, anim.StartFrame, anim.EndFrame, JointTrackType.HSD_A_J_TRAX);
            
            if (anim.StartingPositionY != joint.TY)
                GenerateFOBJ(aobj, anim.StartingPositionY, joint.TY, anim.StartFrame, anim.EndFrame, JointTrackType.HSD_A_J_TRAY);

            if (anim.StartingPositionZ != joint.TZ)
                GenerateFOBJ(aobj, anim.StartingPositionZ, joint.TZ, anim.StartFrame, anim.EndFrame, JointTrackType.HSD_A_J_TRAZ);

            if (anim.StartingRotationX != joint.RX)
                GenerateFOBJ(aobj, anim.StartingRotationX, joint.RX, anim.StartFrame, anim.EndFrame, JointTrackType.HSD_A_J_ROTX);

            if (anim.StartingRotationY != joint.RY)
                GenerateFOBJ(aobj, anim.StartingRotationY, joint.RY, anim.StartFrame, anim.EndFrame, JointTrackType.HSD_A_J_ROTY);

            if (anim.StartingRotationZ != joint.RZ)
                GenerateFOBJ(aobj, anim.StartingRotationZ, joint.RZ, anim.StartFrame, anim.EndFrame, JointTrackType.HSD_A_J_ROTZ);

            if (anim.StartingScaleX != joint.SX)
                GenerateFOBJ(aobj, anim.StartingRotationX, joint.SX, anim.StartFrame, anim.EndFrame, JointTrackType.HSD_A_J_SCAX);

            if (anim.StartingScaleY != joint.SY)
                GenerateFOBJ(aobj, anim.StartingRotationY, joint.SY, anim.StartFrame, anim.EndFrame, JointTrackType.HSD_A_J_SCAY);

            if (anim.StartingScaleZ != joint.SZ)
                GenerateFOBJ(aobj, anim.StartingRotationZ, joint.SZ, anim.StartFrame, anim.EndFrame, JointTrackType.HSD_A_J_SCAZ);
            
            if(aobj.FObjDesc != null)
                aj.AOBJ = aobj;

            return aj;

            // starting frame-600
            // 1600 total frames
            /*if (space.AnimType == MexMapAnimType.SlideInFromRight)
                joint.AOBJ = GenerateAOBJ(36, space.JOBJ.TX, space.StartFrame, space.EndFrame, JointTrackType.HSD_A_J_TRAX);
            if (space.AnimType == MexMapAnimType.SlideInFromLeft)
                joint.AOBJ = GenerateAOBJ(-40, space.JOBJ.TX, space.StartFrame, space.EndFrame, JointTrackType.HSD_A_J_TRAX);
            if (space.AnimType == MexMapAnimType.SlideInFromTop)
                joint.AOBJ = GenerateAOBJ(36, space.JOBJ.TY, space.StartFrame, space.EndFrame, JointTrackType.HSD_A_J_TRAY);
            if (space.AnimType == MexMapAnimType.SlideInFromBottom)
                joint.AOBJ = GenerateAOBJ(-40, space.JOBJ.TY, space.StartFrame, space.EndFrame, JointTrackType.HSD_A_J_TRAY);
            if (space.AnimType == MexMapAnimType.GrowFromNothing)
            {
                joint.AOBJ = GenerateAOBJ(0, space.JOBJ.SX, space.StartFrame, space.EndFrame, JointTrackType.HSD_A_J_SCAX);
                joint.AOBJ.FObjDesc.Next = GenerateAOBJ(0, space.JOBJ.SY, space.StartFrame, space.EndFrame, JointTrackType.HSD_A_J_SCAY).FObjDesc;
            }
            if (space.AnimType == MexMapAnimType.SpinIn)
            {
                joint.AOBJ = GenerateAOBJ(0, space.JOBJ.SX, space.StartFrame, space.EndFrame, JointTrackType.HSD_A_J_SCAX);
                joint.AOBJ.FObjDesc.Add(GenerateAOBJ(0, space.JOBJ.SY, space.StartFrame, space.EndFrame, JointTrackType.HSD_A_J_SCAY).FObjDesc);
                joint.AOBJ.FObjDesc.Add(GenerateAOBJ(-4, 0, space.StartFrame, space.EndFrame, JointTrackType.HSD_A_J_ROTZ).FObjDesc);
            }
            if (space.AnimType == MexMapAnimType.FlipIn)
            {
                joint.AOBJ = GenerateAOBJ(0, space.JOBJ.SX, space.StartFrame, space.EndFrame, JointTrackType.HSD_A_J_SCAX);
                joint.AOBJ.FObjDesc.Add(GenerateAOBJ(0, space.JOBJ.SY, space.StartFrame, space.EndFrame, JointTrackType.HSD_A_J_SCAY).FObjDesc);
                joint.AOBJ.FObjDesc.Add(GenerateAOBJ(-4, 0, space.StartFrame, space.EndFrame, JointTrackType.HSD_A_J_ROTY).FObjDesc);
            }*/
        }

        /// <summary>
        /// Builds simple mex menu animation from anim joint using assumptions about the animation
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
        public static MexMenuAnimation FromAnimJoint(HSD_AnimJoint j, HSD_JOBJ jobj)
        {
            var anim = new MexMenuAnimation()
            {
                StartFrame = 0,
                EndFrame = 0,
                StartingPositionX = jobj.TX,
                StartingPositionY = jobj.TY,
                StartingPositionZ = jobj.TZ,
                StartingRotationX = jobj.RX,
                StartingRotationY = jobj.RY,
                StartingRotationZ = jobj.RZ,
                StartingScaleX = jobj.SX,
                StartingScaleY = jobj.SY,
                StartingScaleZ = jobj.SZ,
            };

            if (j.AOBJ != null && j.AOBJ.FObjDesc != null)
                foreach(var fobj in j.AOBJ.FObjDesc.List)
                {
                    // get when frame starts moving
                    // get frame
                    // get value
                    // that's all we, we assume final position is what the joint is currently at
                    var keys = fobj.GetDecodedKeys();
                    for(int i = 0; i < keys.Count; i++)
                    {
                        var k = keys[i];
                        if(k.InterpolationType != GXInterpolationType.HSD_A_OP_CON)
                        {
                            anim.StartFrame = (int)Math.Max(anim.StartFrame, k.Frame);
                            anim.EndFrame = (int)Math.Max(anim.EndFrame, keys[i + 1].Frame);
                            switch ((JointTrackType)fobj.TrackType)
                            {
                                case JointTrackType.HSD_A_J_TRAX: anim.StartingPositionX = k.Value; break;
                                case JointTrackType.HSD_A_J_TRAY: anim.StartingPositionY = k.Value; break;
                                case JointTrackType.HSD_A_J_TRAZ: anim.StartingPositionZ = k.Value; break;
                                case JointTrackType.HSD_A_J_ROTX: anim.StartingRotationX = k.Value; break;
                                case JointTrackType.HSD_A_J_ROTY: anim.StartingRotationY = k.Value; break;
                                case JointTrackType.HSD_A_J_ROTZ: anim.StartingRotationZ = k.Value; break;
                                case JointTrackType.HSD_A_J_SCAX: anim.StartingScaleX = k.Value; break;
                                case JointTrackType.HSD_A_J_SCAY: anim.StartingScaleY = k.Value; break;
                                case JointTrackType.HSD_A_J_SCAZ: anim.StartingScaleZ = k.Value; break;
                            }
                            break;
                        }
                    }
                }

            return anim;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endValue"></param>
        /// <param name="startFrame"></param>
        /// <param name="endFrame"></param>
        /// <param name="trackType"></param>
        /// <returns></returns>
        public static void GenerateFOBJ(HSD_AOBJ aobj, float startValue, float endValue, float startFrame, float endFrame, JointTrackType trackType)
        {
            List<FOBJKey> keys = new List<FOBJKey>();
            if (startFrame != 0)
                keys.Add(new FOBJKey() { Frame = 0, Value = startValue, InterpolationType = GXInterpolationType.HSD_A_OP_CON });

            keys.Add(new FOBJKey() { Frame = startFrame, Value = startValue, InterpolationType = GXInterpolationType.HSD_A_OP_LIN });

            keys.Add(new FOBJKey() { Frame = endFrame, Value = endValue, InterpolationType = GXInterpolationType.HSD_A_OP_CON });
            keys.Add(new FOBJKey() { Frame = 1600, Value = endValue, InterpolationType = GXInterpolationType.HSD_A_OP_CON });
            
            var fobj = new HSD_FOBJDesc();
            fobj.SetKeys(keys, (byte)trackType);

            if (aobj.FObjDesc == null)
                aobj.FObjDesc = fobj;
            else
                aobj.FObjDesc.Add(fobj);
        }
    }
}
