using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.GCX;
using OpenTK;
using System.IO;
using System.Text.RegularExpressions;

namespace Modlee
{
    public class OBJTriangleLoader
    {

        public List<OBJObject> objects;

        public OBJTriangleLoader()
        {
            objects = new List<OBJObject>();
        }

        public List<Vector3> v = new List<Vector3>();
        public List<Vector2> vt = new List<Vector2>();
        public List<Vector3> vn = new List<Vector3>();

        public static GXVertex[] GetTrianglesFromFile(string fname)
        {
            OBJTriangleLoader l = new OBJTriangleLoader();
            l.Read(fname);
            return l.GetTriangles();
        }

        public class OBJObject
        {
            public string name = "None";
            public List<OBJGroup> groups = new List<OBJGroup>();
        }

        public class OBJGroup
        {
            public List<int> v = new List<int>();
            public List<int> vt = new List<int>();
            public List<int> vn = new List<int>();
        }

        public void Read(string fname)
        {
            string input = File.ReadAllText(fname);

            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            input = regex.Replace(input, " ");

            string[] lines = input.Split('\n');

            Vector3 v;
            OBJObject o = null;
            OBJGroup g = null;
            for (int i = 0; i < lines.Length; i++)
            {
                string[] args = lines[i].Split(' ');
                switch (args[0])
                {
                    case "v":
                        if (o == null)
                        {
                            o = new OBJObject();
                            g = new OBJGroup();
                            o.groups.Add(g);
                            objects.Add(o);
                        }
                        v = new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
                        this.v.Add(v);
                        break;
                    case "vn":
                        v = new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
                        vn.Add(v);
                        break;
                    case "vt":
                        vt.Add(new Vector2(float.Parse(args[1]), float.Parse(args[2])));
                        break;
                    case "f":
                        g.v.Add(int.Parse(args[1].Split('/')[0]) - 1);
                        g.v.Add(int.Parse(args[2].Split('/')[0]) - 1);
                        g.v.Add(int.Parse(args[3].Split('/')[0]) - 1);
                        if (args[1].Split('/').Length > 1)
                        {
                            g.vt.Add(int.Parse(args[1].Split('/')[1]) - 1);
                            g.vt.Add(int.Parse(args[2].Split('/')[1]) - 1);
                            g.vt.Add(int.Parse(args[3].Split('/')[1]) - 1);
                        }
                        if (args[1].Split('/').Length > 2)
                        {
                            g.vn.Add(int.Parse(args[1].Split('/')[2]) - 1);
                            g.vn.Add(int.Parse(args[2].Split('/')[2]) - 1);
                            g.vn.Add(int.Parse(args[3].Split('/')[2]) - 1);
                        }
                        break;
                    case "o":
                        o = new OBJObject();
                        o.name = args[1];
                        objects.Add(o);
                        g = new OBJGroup();
                        o.groups.Add(g);
                        break;
                    case "g":
                        g = new OBJGroup();
                        if (o == null || args.Length > 1)
                        {
                            o = new OBJObject();
                            if (args.Length > 1)
                                o.name = args[1];
                            objects.Add(o);
                        }
                        o.groups.Add(g);
                        break;
                }
            }
        }

        public GXVertex[] GetTriangles()
        {
            List<GXVertex> Triangles = new List<GXVertex>();
            foreach (OBJObject o in objects)
            {
                foreach (OBJGroup g in o.groups)
                {
                    if (g.v.Count == 0) continue;

                    for (int i = 0; i < g.v.Count; i++)
                    {
                        GXVertex v = new GXVertex();
                        if (g.v.Count > i)
                            v.Pos = new GXVector3(this.v[g.v[i]].X, this.v[g.v[i]].Y, this.v[g.v[i]].Z);
                        if (g.vn.Count > i)
                            v.Nrm = new GXVector3(vn[g.v[i]].X, vn[g.v[i]].Y, vn[g.v[i]].Z);
                        if (g.vt.Count > i)
                            v.TX0 = new GXVector2(vt[g.v[i]].X, vt[g.v[i]].Y);
                        Triangles.Add(v);
                    }
                }
            }

            return Triangles.ToArray();
        }


    }
}
