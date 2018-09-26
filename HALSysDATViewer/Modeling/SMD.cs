using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSDLib.Common;
using HSDLib.GX;
using HSDLib.Helpers;
using System.IO;
using System.Text.RegularExpressions;
using OpenTK.Graphics.OpenGL;
using HALSysDATViewer.Modeling.TriangleConverter;

namespace HALSysDATViewer.Modeling
{
    public class SMDTriangle
    {
        public string Material;
        public GXVertex v1, v2, v3;
    }

    public class SMD
    {
        public HSD_JOBJ RootJOBJ;
        public List<SMDTriangle> Triangles;
        public List<HSD_JOBJWeight> BoneWeightList = new List<HSD_JOBJWeight>();

        public SMD()
        {
            Triangles = new List<SMDTriangle>();
        }

        public SMD(string fname)
        {
            Read(fname);
        }

        public void Read(string fname)
        {
            StreamReader reader = File.OpenText(fname);
            string line;

            string current = "";

            RootJOBJ = new HSD_JOBJ();
            Triangles = new List<SMDTriangle>();
            Dictionary<int, HSD_JOBJ> BoneList = new Dictionary<int, HSD_JOBJ>();

            int time = 0;

            List<HSD_JOBJ> JOBJBoneList;
            while ((line = reader.ReadLine()) != null)
            {
                line = Regex.Replace(line, @"\s+", " ");
                string[] args = line.Replace(";", "").TrimStart().Split(' ');

                if (args[0].Equals("triangles") || args[0].Equals("end") || args[0].Equals("skeleton") || args[0].Equals("nodes"))
                {
                    current = args[0];
                    continue;
                }

                if (current.Equals("nodes"))
                {
                    int id = int.Parse(args[0]);
                    HSD_JOBJ b = new HSD_JOBJ();
                    //b.Text = args[1].Replace('"', ' ').Trim();
                    int s = 2;
                    while (args[s].Contains("\""))
                        s++;
                    int ParentIndex = int.Parse(args[s]);
                    if (ParentIndex == -1)
                    {

                        RootJOBJ = b;
                        RootJOBJ.Flags |= JOBJ_FLAG.SKELETON_ROOT;
                    }
                    else
                    {
                        BoneList[ParentIndex].AddChild(b);
                    }
                    BoneList.Add(id, b);
                }

                if (current.Equals("skeleton"))
                {
                    if (args[0].Contains("time"))
                        time = int.Parse(args[1]);
                    else
                    {
                        if (time == 0)
                        {
                            HSD_JOBJ b = BoneList[int.Parse(args[0])];
                            b.Transforms = new HSD_Transforms();
                            b.Transforms.TX = float.Parse(args[1]);
                            b.Transforms.TY = float.Parse(args[2]);
                            b.Transforms.TZ = float.Parse(args[3]);
                            b.Transforms.RX = float.Parse(args[4]);
                            b.Transforms.RY = float.Parse(args[5]);
                            b.Transforms.RZ = float.Parse(args[6]);
                            b.Transforms.SX = 1f;
                            b.Transforms.SY = 1f;
                            b.Transforms.SZ = 1f;
                        }
                    }
                    JOBJBoneList = RootJOBJ.DepthFirstList;
                }

                if (current.Equals("triangles"))
                {
                    string meshName = args[0];
                    if (args[0].Equals(""))
                        continue;

                    SMDTriangle t = new SMDTriangle();
                    Triangles.Add(t);
                    t.Material = meshName;

                    for (int j = 0; j < 3; j++)
                    {
                        line = reader.ReadLine();
                        line = Regex.Replace(line, @"\s+", " ");
                        args = line.Replace(";", "").TrimStart().Split(' ');

                        int parent = int.Parse(args[0]);
                        GXVertex vert = new GXVertex();
                        vert.Pos = new GXVector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
                        vert.Nrm = new GXVector3(float.Parse(args[4]), float.Parse(args[5]), float.Parse(args[6]));
                        vert.TEX0 = new GXVector2(float.Parse(args[7]), float.Parse(args[8]));
                        if (args.Length > 9)
                        {
                            // eww, gross, please fix later
                            int wCount = int.Parse(args[9]);
                            int w = 10;
                            HSD_JOBJWeight bw = new HSD_JOBJWeight();
                            for (int i = 0; i < wCount; i++)
                            {
                                int bone = (int.Parse(args[w++]));
                                float weight = (float.Parse(args[w++]));
                                bw.JOBJs.Add(BoneList[bone]);
                                bw.Weights.Add(weight);
                            }
                            int mtxid = BoneWeightList.IndexOf(bw);
                            if(mtxid == -1)
                            {
                                mtxid = BoneWeightList.Count;
                                BoneWeightList.Add(bw);
                            }
                            vert.PMXID = (ushort)(mtxid * 3);
                        }
                        switch (j)
                        {
                            case 0: t.v1 = vert; break;
                            case 1: t.v2 = vert; break;
                            case 2: t.v3 = vert; break;
                        }
                    }
                }
            }
        }

        public GXVertex[] GetVertices()
        {
            List<GXVertex> Vertices = new List<GXVertex>(Triangles.Count * 3);
            foreach (SMDTriangle t in Triangles)
            {
                Vertices.Add(t.v3);
                Vertices.Add(t.v2);
                Vertices.Add(t.v1);
            }
            return Vertices.ToArray();
        }

        public List<HSD_JOBJWeight> PMXIDCreate(ref GXVertex[] Verts)
        {
            List<HSD_JOBJWeight> BoneWeightList = new List<HSD_JOBJWeight>();
            List<HSD_JOBJ> BoneList = RootJOBJ.DepthFirstList;

            for(int i = 0; i < Verts.Length; i++)
            {
                // map Bone is
                HSD_JOBJWeight bw = new HSD_JOBJWeight();
                /*if(Verts[i].W1 > 0)
                {
                    bw.JOBJs.Add(BoneList[Verts[i].B1]);
                    bw.Weights.Add(Verts[i].W1);
                }
                if (Verts[i].W2 > 0)
                {
                    bw.JOBJs.Add(BoneList[Verts[i].B2]);
                    bw.Weights.Add(Verts[i].W2);
                }
                if (Verts[i].W3 > 0)
                {
                    bw.JOBJs.Add(BoneList[Verts[i].B3]);
                    bw.Weights.Add(Verts[i].W3);
                }
                int index = BoneWeightList.IndexOf(bw);
                if (index != -1)
                {
                } else
                {
                    index = BoneWeightList.Count;
                    BoneWeightList.Add(bw);
                }
                Verts[i].PMXID = (ushort)index;*/
            }

            return BoneWeightList;
        }

        public void PrimitiveRender()
        {
            GL.Begin(PrimitiveType.Triangles);

            foreach(SMDTriangle tri in Triangles)
            {
                GL.Vertex3(tri.v1.Pos.X, tri.v1.Pos.Y, tri.v1.Pos.Z);
                GL.Vertex3(tri.v2.Pos.X, tri.v2.Pos.Y, tri.v2.Pos.Z);
                GL.Vertex3(tri.v3.Pos.X, tri.v3.Pos.Y, tri.v3.Pos.Z);
            }

            GL.End();
        }

        /*public void Save(string FileName)
        {
            StringBuilder o = new StringBuilder();

            o.AppendLine("version 1");

            if (Bones != null)
            {
                o.AppendLine("nodes");
                for (int i = 0; i < Bones.bones.Count; i++)
                    o.AppendLine("  " + i + " \"" + Bones.bones[i].Text + "\" " + Bones.bones[i].parentIndex);
                o.AppendLine("end");

                o.AppendLine("skeleton");
                o.AppendLine("time 0");
                for (int i = 0; i < Bones.bones.Count; i++)
                {
                    Bone b = Bones.bones[i];
                    o.AppendFormat("{0} {1} {2} {3} {4} {5} {6}\n", i, b.position[0], b.position[1], b.position[2], b.rotation[0], b.rotation[1], b.rotation[2]);
                }
                o.AppendLine("end");
            }

            if (Triangles != null)
            {
                o.AppendLine("triangles");
                foreach (SMDTriangle tri in Triangles)
                {
                    o.AppendLine(tri.Material);
                    WriteVertex(o, tri.v1);
                    WriteVertex(o, tri.v2);
                    WriteVertex(o, tri.v3);
                }
                o.AppendLine("end");
            }

            File.WriteAllText(FileName, o.ToString());
        }

        private void WriteVertex(StringBuilder o, SMDVertex v)
        {
            o.AppendFormat("{0} {1} {2} {3} {4} {5} {6} {7} {8} ",
                        v.Parent,
                        v.P.X, v.P.Y, v.P.Z,
                        v.N.X, v.N.Y, v.N.Z,
                        v.UV.X, v.UV.Y);
            if (v.Weights == null)
            {
                o.AppendLine("0");
            }
            else
            {
                string weights = v.Weights.Length + "";
                for (int i = 0; i < v.Weights.Length; i++)
                {
                    weights += " " + v.Bones[i] + " " + v.Weights[i];
                }
                o.AppendLine(weights);
            }*/
    }
}
