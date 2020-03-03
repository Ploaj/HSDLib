using HSDRaw.Common.Animation;
using HSDRawViewer.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace HSDRawViewer.Converters
{
    public class ConvMayaAnim
    {
        public class ExportSettings
        {
            [DisplayName("Use Radians"), Description("")]
            public bool UseRadians { get; set; } = true;
        }

        private static ExportSettings MayaSettings = new ExportSettings();

        private static Dictionary<JointTrackType, MayaAnim.TrackType> jointTrackToMayaTrack = new Dictionary<JointTrackType, MayaAnim.TrackType>()
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
        public static void ExportToMayaAnim(string filePath, List<AnimNode> nodes)
        {
            MayaAnim a = new MayaAnim();

            if (!MayaSettings.UseRadians)
                a.header.angularUnit = "deg";
            
            int nodeIndex = 0;
            int frameCount = 0;
            foreach(var n in nodes)
            {
                MayaAnim.MayaNode mnode = new MayaAnim.MayaNode();
                mnode.name = "JOBJ_" + nodeIndex;
                a.Nodes.Add(mnode);
                
                foreach(var t in n.Tracks)
                {
                    if (!jointTrackToMayaTrack.ContainsKey(t.TrackType))
                        continue;

                    MayaAnim.MayaTrack mtrack = new MayaAnim.MayaTrack();
                    mnode.atts.Add(mtrack);

                    mtrack.type = jointTrackToMayaTrack[t.TrackType];

                    if(mtrack.IsAngular())
                        mtrack.output = MayaAnim.OutputType.angular;
                    
                    for (int i = 0; i < t.Keys.Count; i++)
                    {
                        // get maximum frame to use as framecount
                        frameCount = (int)Math.Max(frameCount, t.Keys[i].Frame);

                        // get current state at this key frame
                        var state = t.GetState(t.Keys[i].Frame);

                        if (t.Keys[i].InterpolationType == GXInterpolationType.HSD_A_OP_SLP)
                            continue;

                        // Debug
                        /*if (mnode.name == "JOBJ_13" && mtrack.type == MayaAnim.TrackType.rotateZ)
                        {
                            Console.WriteLine($"{t.Keys[i].Frame} {t.Keys[i].Value} {t.Keys[i].Tan} {t.Keys[i].InterpolationType}");
                            Console.WriteLine($"{state.t0} {state.t1} {state.p0} {state.p1} {state.d0} {state.d1} {state.op_intrp}");
                        }*/

                        // assuming last frame
                        // if last frame shift frame information over
                        if (t.Keys[i].Frame == state.t1)
                        {
                            state.t0 = state.t1;
                            state.p0 = state.p1;
                            state.d0 = state.d1;
                            state.d1 = 0;
                            //state.op_intrp = state.op;
                        }
                        
                        // generate key with time and value
                        var animkey = new MayaAnim.AnimKey()
                        {
                            input = state.t0 + 1,
                            output = state.p0,
                        };
                        
                        // nothing to do for linear
                        //if (op_intrp == GXInterpolationType.HSD_A_OP_LIN)

                        // set step type for constant and key
                        if (state.op_intrp == GXInterpolationType.HSD_A_OP_CON || 
                            state.op_intrp == GXInterpolationType.HSD_A_OP_KEY)
                        {
                            animkey.intan = "stepped";
                            animkey.outtan = "stepped";
                        }

                        // set tangents for weighted slopes
                        if (state.op_intrp == GXInterpolationType.HSD_A_OP_SLP 
                            || state.op_intrp == GXInterpolationType.HSD_A_OP_SPL0
                             || state.op_intrp == GXInterpolationType.HSD_A_OP_SPL)
                        {
                            animkey.t1 = state.d0;
                            animkey.t2 = state.d1;
                            animkey.intan = "fixed";
                            animkey.outtan = "fixed";
                        }

                        // add final key
                        mtrack.keys.Add(animkey);
                    }
                }

                nodeIndex++;
            }

            // set framecount
            a.header.endTime = frameCount + 1;

            // save to file
            a.Save(filePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public static void ImportFromMayaAnim(string filePath)
        {
            var mayaFile = new MayaAnim();
            mayaFile.Open(filePath);

            // process and encode FOBJ keys

            // linear, const, and step are straight forward:
            // if only one value, use KEY

            // slope notes:
            // if output tan is not equal to next frames input, add SPL operation to use new spline
            // if output slope is 0, use SLP0? TODO: find slp0 to test with

            // return nodes
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
            visibility
        }

        public enum TrackType
        {
            translateX,
            translateY,
            translateZ,
            rotateX,
            rotateY,
            rotateZ,
            scaleX,
            scaleY,
            scaleZ,
            visibility
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
            public List<AnimKey> keys = new List<AnimKey>();

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
            public List<MayaTrack> atts = new List<MayaTrack>();
        }

        public Header header;
        public List<MayaNode> Nodes = new List<MayaNode>();

        public MayaAnim()
        {
            header = new Header();
        }

        public void Open(string fileName)
        {
            using (StreamReader r = new StreamReader(new FileStream(fileName, FileMode.Open)))
            {
                MayaTrack currentData = null;
                while (!r.EndOfStream)
                {
                    var line = r.ReadLine();
                    var args = line.Trim().Replace(";", "").Split(' ');

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
                            if (args.Length != 7)
                                continue;
                            var currentNode = Nodes.Find(e => e.name.Equals(args[3]));
                            if (currentNode == null)
                            {
                                currentNode = new MayaNode();
                                currentNode.name = args[3];
                                Nodes.Add(currentNode);
                            }
                            currentData = new MayaTrack();
                            currentData.controlType = (ControlType)Enum.Parse(typeof(ControlType), args[1].Split('.')[0]);
                            currentData.type = (TrackType)Enum.Parse(typeof(TrackType), args[2]);
                            currentNode.atts.Add(currentData);
                            break;
                        case "animData":
                            if (currentData == null)
                                continue;
                            string dataLine = r.ReadLine();
                            while (!dataLine.Contains("}"))
                            {
                                var dataArgs = dataLine.Trim().Replace(";", "").Split(' ');
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
                                        while (!keyLine.Contains("}"))
                                        {
                                            var keyArgs = keyLine.Trim().Replace(";", "").Split(' ');

                                            var key = new AnimKey()
                                            {
                                                input = float.Parse(keyArgs[0]),
                                                output = float.Parse(keyArgs[1])
                                            };

                                            if (keyArgs.Length >= 7)
                                            {
                                                key.intan = keyArgs[2];
                                                key.outtan = keyArgs[3];
                                            }

                                            if (key.intan == "fixed")
                                            {
                                                key.t1 = float.Parse(keyArgs[7]);
                                                key.w1 = float.Parse(keyArgs[8]);
                                            }
                                            if (key.outtan == "fixed" && keyArgs.Length > 9)
                                            {
                                                key.t2 = float.Parse(keyArgs[9]);
                                                key.w2 = float.Parse(keyArgs[10]);
                                            }

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
        }

        public void Save(string fileName)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
            {
                file.WriteLine("animVersion " + header.animVersion + ";");
                file.WriteLine("mayaVersion " + header.mayaVersion + ";");
                file.WriteLine("timeUnit " + header.timeUnit + ";");
                file.WriteLine("linearUnit " + header.linearUnit + ";");
                file.WriteLine("angularUnit " + header.angularUnit + ";");
                file.WriteLine("startTime " + 1 + ";");
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
                            string tanin = key.intan == "fixed" || key.intan == "auto" ? " " + key.t1 + " " + key.w1 : "";
                            string tanout = key.outtan == "fixed" || key.outtan == "auto" ? " " + key.t2 + " " + key.w2 : "";
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
}