using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using HSDRawViewer.GUI.Dialog;
using HSDRawViewer.Rendering;
using HSDRawViewer.Tools.Animation;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace HSDRawViewer.Converters
{
    public class ConvMayaAnim
    {
        public class ExportSettings
        {
            [DisplayName("Use Radians"), Description("")]
            public bool UseRadians { get; set; } = true;


            [DisplayName("Bake Animation"), Description("")]
            public bool BakeAnimation { get; set; } = false;
        }

        private static readonly ExportSettings MayaSettings = new();

        private static readonly Dictionary<JointTrackType, MayaAnim.TrackType> jointTrackToMayaTrack = new()
        {
            { JointTrackType.HSD_A_J_ROTX, MayaAnim.TrackType.rotateX },
            { JointTrackType.HSD_A_J_ROTY, MayaAnim.TrackType.rotateY },
            { JointTrackType.HSD_A_J_ROTZ, MayaAnim.TrackType.rotateZ },
            { JointTrackType.HSD_A_J_SCAX, MayaAnim.TrackType.scaleX },
            { JointTrackType.HSD_A_J_SCAY, MayaAnim.TrackType.scaleY },
            { JointTrackType.HSD_A_J_SCAZ, MayaAnim.TrackType.scaleZ },
            { JointTrackType.HSD_A_J_TRAX, MayaAnim.TrackType.translateX },
            { JointTrackType.HSD_A_J_TRAY, MayaAnim.TrackType.translateY },
            { JointTrackType.HSD_A_J_TRAZ, MayaAnim.TrackType.translateZ },
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="nodes"></param>
        public static void ExportToMayaAnim(string filePath, JointAnimManager animation, JointMap jointMap)
        {
            using (PropertyDialog d = new("Maya Settings", MayaSettings))
            {
                if (d.ShowDialog() != DialogResult.OK)
                    return;
            }

            ExportToMayaAnim(filePath, animation, jointMap, MayaSettings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="nodes"></param>
        public static void ExportToMayaAnim(string filePath, JointAnimManager animation, JointMap jointMap, ExportSettings exportSettings)
        {
            MayaAnim a = new();

            if (!exportSettings.UseRadians)
                a.header.angularUnit = "deg";

            int nodeIndex = 0;
            int frameCount = 0;
            foreach (AnimNode n in animation.Nodes)
            {
                MayaAnim.MayaNode mnode = new();
                if (jointMap != null && !string.IsNullOrEmpty(jointMap[nodeIndex]))
                    mnode.name = jointMap[nodeIndex];
                else
                    mnode.name = "JOBJ_" + nodeIndex;
                a.Nodes.Add(mnode);

                foreach (FOBJ_Player t in n.Tracks)
                {
                    if (!jointTrackToMayaTrack.ContainsKey(t.JointTrackType))
                        continue;

                    MayaAnim.MayaTrack mtrack = new();
                    mnode.atts.Add(mtrack);

                    mtrack.type = jointTrackToMayaTrack[t.JointTrackType];

                    if (mtrack.IsAngular())
                        mtrack.output = MayaAnim.OutputType.angular;

                    FOBJAnimState prevState = null;
                    if (exportSettings.BakeAnimation)
                    {
                        for (int i = 0; i <= t.FrameCount; i++)
                        {
                            mtrack.keys.Add(new MayaAnim.AnimKey()
                            {
                                input = i,
                                output = t.GetValue(i),
                                outtan = "linear",
                                intan = "linear",
                            });
                        }
                    }
                    else
                    {
                        for (int i = 0; i < t.Keys.Count; i++)
                        {
                            // get maximum frame to use as framecount
                            frameCount = (int)Math.Max(frameCount, t.Keys[i].Frame);

                            // get current state at this key frame
                            FOBJAnimState state = t.GetState(t.Keys[i].Frame);
                            bool nextSlope = i + 1 < t.Keys.Count && t.Keys[i + 1].InterpolationType == GXInterpolationType.HSD_A_OP_SLP;

                            if (t.Keys[i].InterpolationType == GXInterpolationType.HSD_A_OP_SLP)
                                continue;


                            // assuming last frame
                            // if last frame shift frame information over
                            if (t.Keys[i].Frame == state.t1)
                            {
                                state.t0 = state.t1;
                                state.p0 = state.p1;
                                state.d0 = state.d1;
                                //state.op_intrp = state.op;
                            }

                            // generate key with time and value
                            MayaAnim.AnimKey animkey = new()
                            {
                                input = state.t0,
                                output = state.p0,
                            };

                            // nothing to do for linear
                            //if (op_intrp == GXInterpolationType.HSD_A_OP_LIN)

                            // set step type for constant and key
                            if (state.op_intrp == GXInterpolationType.HSD_A_OP_CON ||
                                state.op_intrp == GXInterpolationType.HSD_A_OP_KEY)
                            {
                                animkey.intan = "auto";
                                animkey.outtan = "step";
                            }

                            // set tangents for weighted slopes
                            if (state.op_intrp == GXInterpolationType.HSD_A_OP_SLP
                                || state.op_intrp == GXInterpolationType.HSD_A_OP_SPL0
                                 || state.op_intrp == GXInterpolationType.HSD_A_OP_SPL)
                            {
                                animkey.t1 = state.d0;
                                animkey.t2 = state.d0;
                                if (nextSlope && prevState != null)
                                    animkey.t1 = prevState.d1;
                                animkey.intan = "spline";
                                animkey.outtan = "spline";
                            }

                            prevState = state;

                            animkey.t1 = (float)MathHelper.RadiansToDegrees(Math.Atan(animkey.t1));
                            animkey.t2 = (float)MathHelper.RadiansToDegrees(Math.Atan(animkey.t2));

                            if (mtrack.IsAngular() && !exportSettings.UseRadians)
                            {
                                animkey.output = MathHelper.RadiansToDegrees(animkey.output);
                                animkey.t1 = MathHelper.RadiansToDegrees(animkey.t1);
                                animkey.t2 = MathHelper.RadiansToDegrees(animkey.t2);
                            }

                            // add final key
                            mtrack.keys.Add(animkey);
                        }
                    }
                }

                nodeIndex++;
            }

            // set framecount
            a.header.endTime = animation.FrameCount;
            a.header.startTime = 0;

            // save to file
            a.Save(filePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public static JointAnimManager ImportFromMayaAnim(string filePath, JointMap jointMap)
        {
            MayaAnim mayaFile = new();
            mayaFile.Open(filePath);

            JointAnimManager animation = new();
            animation.FrameCount = mayaFile.header.endTime - mayaFile.header.startTime;

            int nodeCount = jointMap == null || jointMap.Count == 0 ? mayaFile.Nodes.Count : Math.Max(mayaFile.Nodes.Count, jointMap.Count);

            for (int i = 0; i < nodeCount; i++)
                animation.Nodes.Add(new AnimNode());

            // process and encode FOBJ keys
            int index = 0;
            foreach (MayaAnim.MayaNode mNode in mayaFile.Nodes)
            {
                AnimNode node = animation.Nodes[index++];
                //System.Diagnostics.Debug.WriteLine(mNode.name);

                if (jointMap != null && jointMap.Count > 0)
                {
                    int nodeIndex = jointMap.IndexOf(mNode.name);

                    if (nodeIndex != -1)
                        node = animation.Nodes[nodeIndex];
                    else
                        continue;
                }

                foreach (MayaAnim.MayaTrack mTrack in mNode.atts)
                {
                    FOBJ_Player t = new();
                    t.Keys = new List<FOBJKey>();
                    t.JointTrackType = jointTrackToMayaTrack.FirstOrDefault(e => e.Value == mTrack.type).Key;

                    //System.Diagnostics.Debug.WriteLine("\t" + mTrack.type);

                    bool degrees = mayaFile.header.angularUnit == "deg";
                    bool trackUnit = (mTrack.IsAngular() && degrees);

                    for (int i = 0; i < mTrack.keys.Count; i++)
                    {
                        MayaAnim.AnimKey mKey = mTrack.keys[i];
                        MayaAnim.AnimKey mKeyNext = i + 1 < mTrack.keys.Count ? mTrack.keys[i + 1] : mTrack.keys[i];

                        FOBJKey k = new();
                        k.Frame = mKey.input - mayaFile.header.startTime;
                        k.Value = trackUnit ? MathHelper.DegreesToRadians(mKey.output) : mKey.output;
                        switch (mKey.outtan)
                        {
                            case "auto":
                            case "linear":
                                k.InterpolationType = GXInterpolationType.HSD_A_OP_LIN;

                                t.Keys.Add(k);
                                break;
                            case "step":
                                if (mTrack.keys.Count == 1)
                                    k.InterpolationType = GXInterpolationType.HSD_A_OP_KEY;
                                else
                                    k.InterpolationType = GXInterpolationType.HSD_A_OP_CON;

                                t.Keys.Add(k);
                                break;
                            case "fixed":
                            case "spline":
                                k.InterpolationType = GXInterpolationType.HSD_A_OP_SPL;
                                k.Tan = AngleToTan(mKey.t1, degrees);

                                if ((mKeyNext.input - mKey.input) <= 1) // optimization
                                {
                                    //k.InterpolationType = GXInterpolationType.HSD_A_OP_LIN;
                                    t.Keys.Add(k);
                                }
                                else
                                if (mKey.t2 == 0)
                                {
                                    k.InterpolationType = GXInterpolationType.HSD_A_OP_SPL0;
                                    t.Keys.Add(k);
                                }
                                else
                                if (mKey.t1 != mKey.t2)
                                {
                                    t.Keys.Add(k);

                                    FOBJKey slp = new();
                                    slp.Frame = mKeyNext.input - 1;
                                    slp.InterpolationType = GXInterpolationType.HSD_A_OP_SLP;
                                    slp.Tan = AngleToTan(mKey.t2, degrees);
                                    t.Keys.Add(slp);
                                }
                                else
                                {
                                    t.Keys.Add(k);
                                }

                                break;
                            default:
                                Console.WriteLine(mKey.outtan + " not supported!");
                                break;
                        }

                        if (mTrack.keys.Count == 1)
                            k.InterpolationType = GXInterpolationType.HSD_A_OP_KEY;

                        //foreach (var key in t.Keys)
                        //    Debug.WriteLine($"\t\t{key.Frame} {key.Value}");

                    }

                    node.Tracks.Add(t);
                }

            }

            return animation;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static float AngleToTan(float angle, bool degree)
        {
            return (float)(Math.Tan(angle * Math.PI / 180) * (degree ? Math.PI / 180 : 1));
        }
    }

    public class MayaAnim
    {
        public enum InfinityType
        {
            constant,
            linear,
            cycle,
            cycleRelative,
            oscillate
        }

        public enum InputType
        {
            time,
            unitless
        }

        public enum OutputType
        {
            time,
            linear,
            angular,
            unitless
        }

        public enum ControlType
        {
            translate,
            rotate,
            scale,
            visibility,
            MaxHandle,
        }

        public enum TrackType
        {
            translateX,
            translateY,
            translateZ,
            rotateX,
            rotateY,
            rotateZ,
            rotateW,
            scaleX,
            scaleY,
            scaleZ,
            visibility,
            MaxHandle,
        }

        public class Header
        {
            public float animVersion;
            public string mayaVersion;
            public float startTime;
            public float endTime;
            public float startUnitless;
            public float endUnitless;
            public string timeUnit;
            public string linearUnit;
            public string angularUnit;

            public Header()
            {
                animVersion = 1.1f;
                mayaVersion = "2015";
                startTime = 1;
                endTime = 1;
                startUnitless = 0;
                endUnitless = 0;
                timeUnit = "ntscf";
                linearUnit = "cm";
                angularUnit = "rad";
            }
        }

        public class AnimKey
        {
            public float input, output;
            public string intan, outtan;
            public float t1 = 0, w1 = 1;
            public float t2 = 0, w2 = 1;

            public AnimKey()
            {
                intan = "linear";
                outtan = "linear";
            }
        }

        public class MayaTrack
        {
            public ControlType controlType;
            public TrackType type
            {
                get => _type;
                set
                {
                    _type = value;
                    switch (_type)
                    {
                        case TrackType.rotateX:
                        case TrackType.rotateY:
                        case TrackType.rotateZ:
                        case TrackType.rotateW:
                            controlType = ControlType.rotate;
                            break;
                        case TrackType.scaleX:
                        case TrackType.scaleY:
                        case TrackType.scaleZ:
                            controlType = ControlType.scale;
                            break;
                        case TrackType.translateX:
                        case TrackType.translateY:
                        case TrackType.translateZ:
                            controlType = ControlType.translate;
                            break;
                        case TrackType.visibility: // joint visibility
                            controlType = ControlType.visibility;
                            break;
                    }
                }
            }
            private TrackType _type;
            public InputType input;
            public OutputType output;
            public InfinityType preInfinity, postInfinity;
            public bool weighted = false;
            public List<AnimKey> keys = new();

            public MayaTrack()
            {
                input = InputType.time;
                output = OutputType.linear;
                preInfinity = InfinityType.constant;
                postInfinity = InfinityType.constant;
                weighted = false;
            }

            public bool IsAngular()
            {
                if (controlType == ControlType.rotate)
                    return true;
                return false;
            }
        }

        public class MayaNode
        {
            public string name;
            public List<MayaTrack> atts = new();
        }

        public Header header;
        public List<MayaNode> Nodes = new();

        public MayaAnim()
        {
            header = new Header();
        }

        public void Open(string fileName)
        {
            using StreamReader r = new(new FileStream(fileName, FileMode.Open));
            MayaTrack currentData = null;
            while (!r.EndOfStream)
            {
                string line = r.ReadLine();
                string[] args = line.Trim().Replace(";", "").Split(' ');

                switch (args[0])
                {
                    case "animVersion":
                        header.animVersion = float.Parse(args[1]);
                        break;
                    case "mayaVersion":
                        header.mayaVersion = args[1];
                        break;
                    case "timeUnit":
                        header.timeUnit = args[1];
                        break;
                    case "linearUnit":
                        header.linearUnit = args[1];
                        break;
                    case "angularUnit":
                        header.angularUnit = args[1];
                        break;
                    case "startTime":
                        header.startTime = float.Parse(args[1]);
                        break;
                    case "endTime":
                        header.endTime = float.Parse(args[1]);
                        break;
                    case "anim":
                        string nodeName = "";
                        if (args.Length == 7)
                            nodeName = args[3];
                        else
                            nodeName = args[1];
                        MayaNode currentNode = Nodes.Find(e => e.name.Equals(nodeName));
                        if (currentNode == null)
                        {
                            currentNode = new MayaNode();
                            currentNode.name = nodeName;
                            Nodes.Add(currentNode);
                        }
                        currentData = new MayaTrack();
                        if (args.Length == 7)
                        {
                            //currentData.controlType = (ControlType)Enum.Parse(typeof(ControlType), args[1].Split('.')[0]);
                            currentData.type = (TrackType)Enum.Parse(typeof(TrackType), args[2]);
                            currentNode.atts.Add(currentData);
                        }
                        break;
                    case "animData":
                        if (currentData == null)
                            continue;
                        string dataLine = r.ReadLine();
                        while (!dataLine.Contains("}"))
                        {
                            string[] dataArgs = dataLine.Trim().Replace(";", "").Split(' ');
                            switch (dataArgs[0])
                            {
                                case "input":
                                    currentData.input = (InputType)Enum.Parse(typeof(InputType), dataArgs[1]);
                                    break;
                                case "output":
                                    currentData.output = (OutputType)Enum.Parse(typeof(OutputType), dataArgs[1]);
                                    break;
                                case "weighted":
                                    currentData.weighted = dataArgs[1] == "1";
                                    break;
                                case "preInfinity":
                                    currentData.preInfinity = (InfinityType)Enum.Parse(typeof(InfinityType), dataArgs[1]);
                                    break;
                                case "postInfinity":
                                    currentData.postInfinity = (InfinityType)Enum.Parse(typeof(InfinityType), dataArgs[1]);
                                    break;
                                case "keys":
                                    string keyLine = r.ReadLine();
                                    while (!keyLine.Contains('}'))
                                    {
                                        string[] keyArgs = keyLine.Trim().Replace(";", "").Split(' ');

                                        AnimKey key = new()
                                        {
                                            input = float.Parse(keyArgs[0]),
                                            output = float.Parse(keyArgs[1])
                                        };

                                        if (keyArgs.Length >= 7)
                                        {
                                            key.intan = keyArgs[2];
                                            key.outtan = keyArgs[3];
                                        }

                                        if (keyArgs.Length > 8)
                                        {
                                            key.t1 = float.Parse(keyArgs[7]);
                                            key.w1 = float.Parse(keyArgs[8]);
                                        }
                                        if (keyArgs.Length > 10)
                                        {
                                            key.t2 = float.Parse(keyArgs[9]);
                                            key.w2 = float.Parse(keyArgs[10]);
                                        }

                                        if (key.input <= header.endTime)
                                            currentData.keys.Add(key);

                                        keyLine = r.ReadLine();
                                    }
                                    break;

                            }
                            dataLine = r.ReadLine();
                        }
                        break;
                }
            }
        }

        public void Save(string fileName)
        {
            using System.IO.StreamWriter file = new(fileName);
            file.WriteLine("animVersion " + header.animVersion + ";");
            file.WriteLine("mayaVersion " + header.mayaVersion + ";");
            file.WriteLine("timeUnit " + header.timeUnit + ";");
            file.WriteLine("linearUnit " + header.linearUnit + ";");
            file.WriteLine("angularUnit " + header.angularUnit + ";");
            file.WriteLine("startTime " + header.startTime + ";");
            file.WriteLine("endTime " + header.endTime + ";");

            int Row = 0;

            foreach (MayaNode animBone in Nodes)
            {
                int TrackIndex = 0;
                if (animBone.atts.Count == 0)
                {
                    file.WriteLine($"anim {animBone.name} 0 1 {TrackIndex++};");
                }
                foreach (MayaTrack animData in animBone.atts)
                {
                    file.WriteLine($"anim {animData.controlType}.{animData.type} {animData.type} {animBone.name} 0 1 {TrackIndex++};");
                    file.WriteLine("animData {");
                    file.WriteLine($" input {animData.input};");
                    file.WriteLine($" output {animData.output};");
                    file.WriteLine($" weighted {(animData.weighted ? 1 : 0)};");
                    file.WriteLine($" preInfinity {animData.preInfinity};");
                    file.WriteLine($" postInfinity {animData.postInfinity};");

                    file.WriteLine(" keys {");
                    foreach (AnimKey key in animData.keys)
                    {
                        string tanin = key.intan == "spline" || key.intan == "fixed" || key.intan == "auto" ? " " + key.t1 + " " + key.w1 : "";
                        string tanout = key.intan == "spline" || key.outtan == "fixed" || key.outtan == "auto" ? " " + key.t2 + " " + key.w2 : "";
                        file.WriteLine($" {key.input} {key.output:N6} {key.intan} {key.outtan} 1 1 0{tanin}{tanout};");
                    }
                    file.WriteLine(" }");

                    file.WriteLine("}");
                }
                Row++;
            }
        }

    }
}