using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.DAT;
using MeleeLib.IO;
using MeleeLib.DAT.Animation;
using MeleeLib.DAT.Helpers;
using MeleeLib.GCX;

namespace Modlee
{
    public class Tests
    {
        public static void TestAnimationBuild(DATFile File, DatAnimation a, string outfile)
        {
            {

                //AnimationHelperTrack[] tracks = AnimationKeyFrameHelper.DecodeKeyFrames(a.Nodes[3]);


                StringBuilder smd = new StringBuilder();
                smd.AppendLine("version 1");
                List<DatJOBJ> joints = new List<DatJOBJ>();
                joints.AddRange(File.Roots[0].GetJOBJinOrder());
                smd.AppendLine("nodes");
                int i = 0;
                foreach (DatJOBJ j in joints)
                {
                    smd.AppendLine(i + " \"Node_" + i + "\" " + (j.Parent == null ? -1 : joints.IndexOf(j.Parent)));
                    i++;
                }
                smd.AppendLine("end");


                AnimationHelperTrack[] tracks2 = AnimationKeyFrameHelper.DecodeKeyFrames(a.Nodes[48]);
                foreach(AnimationHelperTrack t in tracks2)
                {
                    if(t.TrackType == AnimTrackType.ZROT)
                    {
                        foreach(AnimationHelperKeyFrame kf in t.KeyFrames)
                        {
                            Console.WriteLine(kf.Frame + " " + kf.InterpolationType + " " + kf.Tan + " " + kf.Value);
                        }
                    }
                }

                smd.AppendLine("skeleton");
                for (int t = 0; t < a.FrameCount; t++)
                {
                    smd.AppendLine("time " + t);
                    i = 0;
                    foreach (DatJOBJ j in joints)
                    {
                        float X = j.TX, Y = j.TY, Z = j.TZ, RX = j.RX, RY = j.RY, RZ = j.RZ;
                        if(t > 11 && t < 13)
                        Console.WriteLine("Bone " + i + "----------------------");
                        AnimationHelperTrack[] tracks = AnimationKeyFrameHelper.DecodeKeyFrames(a.Nodes[i]);

                        {
                            foreach (AnimationHelperTrack track in tracks)
                            {
                                float Value = track.GetValueAt(t);
                                switch (track.TrackType)
                                {
                                    case AnimTrackType.XPOS: X = Value; break;
                                    case AnimTrackType.YPOS: Y = Value; break;
                                    case AnimTrackType.ZPOS: Z = Value; break;
                                    case AnimTrackType.XROT: RX = Value; break;
                                    case AnimTrackType.YROT: RY = Value; break;
                                    case AnimTrackType.ZROT: RZ = Value; break;
                                }
                            }
                        }

                        smd.AppendLine(i + " " + X + " " + Y + " " + Z + " " + RX + " " + RY + " " + RZ);
                        i++;
                    }
                }
                smd.AppendLine("end");

                System.IO.File.WriteAllText(outfile, smd.ToString());
            }
        }


        public static void ExportDatToSMD(string fname, DATFile d)
        {
            StringBuilder o = new StringBuilder();


            o.AppendLine("version 1");

            List<DatJOBJ> jobjs = new List<DatJOBJ>();
            jobjs.AddRange(d.Roots[0].GetJOBJinOrder());

            o.AppendLine("nodes");
            int id = 0;
            foreach (DatJOBJ j in jobjs)
            {
                o.AppendLine(id + " \"Node_" + id + "\" " + (j.Parent == null ? -1 : jobjs.IndexOf(j.Parent)));
                id++;
            }
            o.AppendLine("end");

            o.AppendLine("skeleton");
            o.AppendLine("time 0");
            id = 0;
            foreach (DatJOBJ j in jobjs)
            {
                o.AppendLine(id + " " + j.TX + " " + j.TY + " " + j.TZ + " " + j.RX +" " + j.RY + " " + j.RZ);
                id++;
            }
            o.AppendLine("end");

            /*o.AppendLine("triangles");
            VertexBaker b = new VertexBaker();
            b.SetRoot(d);
            int dataid = 0;
            foreach (DatDOBJ data in d.Roots[0].GetDataObjects())
            {
                int polyid = 0;
                foreach (DatPolygon poly in data.Polygons)
                {
                    GXVertex[] verts = b.GetFormattedVertices(poly);
                    List<GXVertex> TriangleOutput = new List<GXVertex>();
                    int off = 0;
                    foreach(GXDisplayList dl in poly.DisplayLists)
                    {
                        switch (dl.PrimitiveType)
                        {
                            case GXPrimitiveType.TriangleStrip:
                                
                                for (int k = 0; k < dl.Indices.Length-2; k ++)
                                {
                                    if((k & 1) > 0)
                                    {
                                        TriangleOutput.Add(verts[off + k]);
                                        TriangleOutput.Add(verts[off + k + 1]);
                                        TriangleOutput.Add(verts[off + k + 2]);
                                    }
                                    else
                                    {
                                        TriangleOutput.Add(verts[off + k]);
                                        TriangleOutput.Add(verts[off + k + 2]);
                                        TriangleOutput.Add(verts[off + k + 1]);
                                    }
                                }
                                break;
                            case GXPrimitiveType.Triangles:
                                for (int k = 0; k < dl.Indices.Length; k += 3)
                                {
                                    TriangleOutput.Add(verts[off + k]);
                                    TriangleOutput.Add(verts[off + k + 1]);
                                    TriangleOutput.Add(verts[off + k + 2]);
                                }
                                break;
                            case GXPrimitiveType.Quads:
                                for(int k = 0; k < dl.Indices.Length; k+=4)
                                {
                                    TriangleOutput.Add(verts[off + k]);
                                    TriangleOutput.Add(verts[off + k + 1]);
                                    TriangleOutput.Add(verts[off + k + 2]);
                                    TriangleOutput.Add(verts[off + k + 2]);
                                    TriangleOutput.Add(verts[off + k + 3]);
                                    TriangleOutput.Add(verts[off + k]);
                                }
                                break;
                        }
                        off += dl.Indices.Length;
                    }

                    for(int i = 0; i < TriangleOutput.Count; i += 3)
                    {
                        o.AppendLine("poly_" + dataid + "_" + polyid);
                        for(int j =0; j < 3;j++)
                        {
                            GXVertex v = TriangleOutput[i+j];
                            string boneList = "";
                            for(int k = 0; k < v.N.Length; k++)
                            {
                                boneList = v.N[k] + " " + v.W[k];
                            }
                            o.AppendLine("0 " + v.Pos.X + " " + v.Pos.Y + " " + v.Pos.Z + " " + v.Nrm.X + " " + v.Nrm.Y + " " + v.Nrm.Z + " " + v.TX0.X + " " + v.TX0.Y + " " + v.N.Length + " " + boneList);
                        }
                    }
                    polyid++;
                }
                dataid++;
            }

            o.AppendLine("end");*/

            System.IO.File.WriteAllText(fname, o.ToString());
        }
    }
}
