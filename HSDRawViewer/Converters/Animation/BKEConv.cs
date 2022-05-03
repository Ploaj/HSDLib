using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using HSDRawViewer.Rendering;
using HSDRawViewer.Tools;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDRawViewer.Converters.Animation
{
    public class BKEConv
    {
        public static JointAnimManager ToJointAnim(string filePath, JointMap jm)
        {
            var bke = BKE.Open(filePath);

            JointAnimManager anim = new JointAnimManager();
            anim.FrameCount = bke.CountFrames();
            for (int i = 0; i < bke.Nodes.Count; i++)
            {
                var node = new AnimNode();

                node.Tracks.Add(new FOBJ_Player() { JointTrackType = JointTrackType.HSD_A_J_TRAX, Keys = new List<FOBJKey>() });
                node.Tracks.Add(new FOBJ_Player() { JointTrackType = JointTrackType.HSD_A_J_TRAY, Keys = new List<FOBJKey>() });
                node.Tracks.Add(new FOBJ_Player() { JointTrackType = JointTrackType.HSD_A_J_TRAZ, Keys = new List<FOBJKey>() });

                node.Tracks.Add(new FOBJ_Player() { JointTrackType = JointTrackType.HSD_A_J_ROTX, Keys = new List<FOBJKey>() });
                node.Tracks.Add(new FOBJ_Player() { JointTrackType = JointTrackType.HSD_A_J_ROTY, Keys = new List<FOBJKey>() });
                node.Tracks.Add(new FOBJ_Player() { JointTrackType = JointTrackType.HSD_A_J_ROTZ, Keys = new List<FOBJKey>() });

                node.Tracks.Add(new FOBJ_Player() { JointTrackType = JointTrackType.HSD_A_J_SCAX, Keys = new List<FOBJKey>() });
                node.Tracks.Add(new FOBJ_Player() { JointTrackType = JointTrackType.HSD_A_J_SCAY, Keys = new List<FOBJKey>() });
                node.Tracks.Add(new FOBJ_Player() { JointTrackType = JointTrackType.HSD_A_J_SCAZ, Keys = new List<FOBJKey>() });

                anim.Nodes.Add(node);
            }

            foreach (var n in bke.Nodes)
            {
                if (jm.IndexOf(n.Name) == -1)
                    break;

                var node = anim.Nodes[jm.IndexOf(n.Name)];
                Console.WriteLine(n.Name + " " + n.Parent?.Name);

                for (int f = 0; f < anim.FrameCount; f++)
                {
                    var matrix = n.GetLocal(f);
                    var tra = matrix.ExtractTranslation();
                    var sca = matrix.ExtractScale();
                    var rot = matrix.ExtractRotationEuler();

                    node.Tracks.Find(e => e.JointTrackType == JointTrackType.HSD_A_J_TRAX).Keys.Add(new FOBJKey() { Frame = f, Value = tra.X, InterpolationType = GXInterpolationType.HSD_A_OP_LIN });
                    node.Tracks.Find(e => e.JointTrackType == JointTrackType.HSD_A_J_TRAY).Keys.Add(new FOBJKey() { Frame = f, Value = tra.Y, InterpolationType = GXInterpolationType.HSD_A_OP_LIN });
                    node.Tracks.Find(e => e.JointTrackType == JointTrackType.HSD_A_J_TRAZ).Keys.Add(new FOBJKey() { Frame = f, Value = tra.Z, InterpolationType = GXInterpolationType.HSD_A_OP_LIN });

                    node.Tracks.Find(e => e.JointTrackType == JointTrackType.HSD_A_J_ROTX).Keys.Add(new FOBJKey() { Frame = f, Value = rot.X, InterpolationType = GXInterpolationType.HSD_A_OP_LIN });
                    node.Tracks.Find(e => e.JointTrackType == JointTrackType.HSD_A_J_ROTY).Keys.Add(new FOBJKey() { Frame = f, Value = rot.Y, InterpolationType = GXInterpolationType.HSD_A_OP_LIN });
                    node.Tracks.Find(e => e.JointTrackType == JointTrackType.HSD_A_J_ROTZ).Keys.Add(new FOBJKey() { Frame = f, Value = rot.Z, InterpolationType = GXInterpolationType.HSD_A_OP_LIN });

                    node.Tracks.Find(e => e.JointTrackType == JointTrackType.HSD_A_J_SCAX).Keys.Add(new FOBJKey() { Frame = f, Value = sca.X, InterpolationType = GXInterpolationType.HSD_A_OP_LIN });
                    node.Tracks.Find(e => e.JointTrackType == JointTrackType.HSD_A_J_SCAY).Keys.Add(new FOBJKey() { Frame = f, Value = sca.Y, InterpolationType = GXInterpolationType.HSD_A_OP_LIN });
                    node.Tracks.Find(e => e.JointTrackType == JointTrackType.HSD_A_J_SCAZ).Keys.Add(new FOBJKey() { Frame = f, Value = sca.Z, InterpolationType = GXInterpolationType.HSD_A_OP_LIN });

                }
            }

            return anim;
        }
    }

    public class BKENode
    {
        public string Name;

        public BKENode Parent;

        public List<Matrix4> Keys { get; } = new List<Matrix4>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public Matrix4 GetLocal(int frame)
        {
            var matrix = Keys[frame];
            if (Parent != null)
            {
                //matrix = matrix * Parent.Keys[frame].Inverted();
                //matrix = matrix.ClearScale();
                //matrix *= Matrix4.CreateScale(Parent.Keys[frame].Inverted().ExtractScale());
            }
            return matrix;
        }
    }

    public class BKE
    {
        public List<BKENode> Nodes { get; } = new List<BKENode>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CountFrames()
        {
            return Nodes.Max(e => e.Keys.Count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BKENode GetCreateNode(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            var node = Nodes.Find(e => e.Name == name);

            if (node != null)
                return node;

            node = new BKENode()
            {
                Name = name,
            };
            Nodes.Add(node);
            return node;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static BKE Open(string filePath)
        {
            var bke = new BKE();

            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            using (StreamReader r = new StreamReader(stream))
            {
                BKENode current = null;
                while (!r.EndOfStream)
                {
                    var arg = r.ReadLine().TrimStart().Split(' ');

                    if (arg[0].Contains("{"))
                    {
                        while (!r.EndOfStream)
                        {
                            arg = r.ReadLine().TrimStart().Split(' ');

                            if (arg[0].Equals("}"))
                                break;

                            if (current != null)
                            {
                                float frame = float.Parse(arg[0]) - 1;

                                var matrix = new OpenTK.Matrix4(
                                    float.Parse(arg[1]), float.Parse(arg[2]), float.Parse(arg[3]), float.Parse(arg[4]),
                                    float.Parse(arg[5]), float.Parse(arg[6]), float.Parse(arg[7]), float.Parse(arg[8]),
                                    float.Parse(arg[9]), float.Parse(arg[10]), float.Parse(arg[11]), float.Parse(arg[12]),
                                    float.Parse(arg[13]), float.Parse(arg[14]), float.Parse(arg[15]), float.Parse(arg[16]));

                                current.Keys.Add(matrix);
                            }
                        }
                    }
                    else
                    {
                        current = bke.GetCreateNode(arg[0]);
                        if (arg.Length > 1)
                            current.Parent = bke.GetCreateNode(arg[1]);
                    }
                }
            }

            return bke;
        }
    }
}
