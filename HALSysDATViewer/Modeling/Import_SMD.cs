using HALSysDATViewer.Rendering;
using HSDLib.Common;
using HSDLib.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace HALSysDATViewer.Modeling
{
    public class Import_SMD
    {
        public class SMDBone
        {
            public int ID;
            public string Name;
            public int ParentID;

            public float X, Y, Z;
            public float RX, RY, RZ;
        }

        public class SMDTriangle
        {
            public string Material;
            public SMDVertex v1;
            public SMDVertex v2;
            public SMDVertex v3;
        }

        public class SMDVertex
        {
            public int BoneID;
            public float X, Y, Z;
            public float NX, NY, NZ;
            public float UVX, UVY;
            public List<int> Bones;
            public List<float> Weights;
        }

        public List<SMDBone> Bones = new List<SMDBone>();
        public List<SMDTriangle> Triangles = new List<SMDTriangle>();

        public Import_SMD(string fileName)
        {
            using (StreamReader r = new StreamReader(new FileStream(fileName, FileMode.Open)))
            {
                var line = r.ReadLine();
                while (!r.EndOfStream)
                {
                    switch (line)
                    {
                        case "nodes":
                            ParseNodeSection(r);
                            break;
                        case "skeleton":
                            ParseNodeSection(r);
                            break;
                        case "triangles":
                            ParseTriangleSection(r);
                            break;
                    }
                    line = r.ReadLine();
                }
            }
        }

        private void ParseNodeSection(StreamReader r)
        {
            var line = r.ReadLine();
            while (!r.EndOfStream && line != "end")
            {
                var args = Regex.Replace(line.Trim(), @"\s+", " ").Split(' ');
                if(args.Length == 3)
                {
                    SMDBone b = new SMDBone();
                    b.ID = int.Parse(args[0]);
                    b.Name = args[1].Replace("\"", "");
                    b.ParentID = int.Parse(args[2]);
                    Bones.Add(b);
                }

                line = r.ReadLine();
            }
        }


        private void ParseSkeletonSection(StreamReader r)
        {
            var line = r.ReadLine();
            while (!r.EndOfStream && line != "end")
            {
                var args = Regex.Replace(line.Trim(), @"\s+", " ").Split(' ');

                if (args.Length == 7)
                {
                    int boneIndex = int.Parse(args[0]);
                    Bones[boneIndex].X = float.Parse(args[1]);
                    Bones[boneIndex].Y = float.Parse(args[2]);
                    Bones[boneIndex].Z = float.Parse(args[3]);
                    Bones[boneIndex].RZ = float.Parse(args[4]);
                    Bones[boneIndex].RY = float.Parse(args[5]);
                    Bones[boneIndex].RZ = float.Parse(args[6]);
                }

                line = r.ReadLine();
            }
        }


        private void ParseTriangleSection(StreamReader r)
        {
            var line = r.ReadLine();
            while (!r.EndOfStream && line != "end")
            {
                var triangle = new SMDTriangle();
                triangle.Material = line;

                for(int i = 0; i < 3; i++)
                {
                    line = r.ReadLine();
                    var args = Regex.Replace(line.Trim(), @"\s+", " ").Split(' ');

                    var vert = new SMDVertex()
                    {
                        BoneID = int.Parse(args[0]),
                        X = float.Parse(args[1]),
                        Y = float.Parse(args[2]),
                        Z = float.Parse(args[3]),
                        NX = float.Parse(args[4]),
                        NY = float.Parse(args[5]),
                        NZ = float.Parse(args[6]),
                        UVX = float.Parse(args[7]),
                        UVY = float.Parse(args[8]),
                        Bones = new List<int>(),
                        Weights = new List<float>()
                    };

                    int boneCount = int.Parse(args[9]);
                    for(int j = 0; j < boneCount; j++)
                    {
                        vert.Bones.Add(int.Parse(args[10 + j * 2]));
                        vert.Weights.Add(float.Parse(args[10 + j * 2 + 1]));
                    }

                    if (i == 0)
                        triangle.v1 = vert;
                    if (i == 1)
                        triangle.v2 = vert;
                    if (i == 2)
                        triangle.v3 = vert;
                }

                Triangles.Add(triangle);

                line = r.ReadLine();
            }
        }

        public GXVertex[] GetTriangles(HSDLib.Common.HSD_JOBJ jobjRoot, out List<HSD_JOBJWeight> weights)
        {
            var jobjList = jobjRoot.DepthFirstList;
            var JOBJWeights = new List<HSD_JOBJWeight>();
            // todo: remap bones

            List<GXVertex> vertices = new List<GXVertex>();

            foreach(var tri in Triangles)
            {
                vertices.Add(ConvertVertex(tri.v1, jobjList, ref JOBJWeights));
                vertices.Add(ConvertVertex(tri.v2, jobjList, ref JOBJWeights));
                vertices.Add(ConvertVertex(tri.v3, jobjList, ref JOBJWeights));
            }

            weights = JOBJWeights;

            return vertices.ToArray();
        }

        private GXVertex ConvertVertex(SMDVertex v1, List<HSD_JOBJ> jobjList, ref List<HSD_JOBJWeight> JOBJWeights)
        {
            GXVertex v = new GXVertex()
            {
                Pos = new GXVector3(v1.X, v1.Y, v1.Z),
                Nrm = new GXVector3(v1.NX, v1.NY, v1.NZ),
                TEX0 = new GXVector2(v1.UVX, v1.UVY),
                PMXID = (ushort)(JOBJWeights.Count * 3)
            };

            var weightObject = new HSD_JOBJWeight();
            weightObject.Weights.AddRange(v1.Weights);
            foreach (var b in v1.Bones)
            {
                var bindex = int.Parse(Bones[b].Name.Split('_')[1]);
                weightObject.JOBJs.Add(jobjList[bindex]);
            }
            var index = JOBJWeights.IndexOf(weightObject);
            if (index == -1)
            {
                index = JOBJWeights.Count;
                JOBJWeights.Add(weightObject);
            }
            v.PMXID = (ushort)(index * 3);

            return v;
        }
    }
}
