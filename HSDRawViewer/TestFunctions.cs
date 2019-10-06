using HSDRaw.Common;
using HSDRaw;
using HSDRaw.GX;
using HSDRaw.Tools;
using System.Collections.Generic;
using HSDRawViewer.Rendering;
using HSDRaw.Common.Animation;
using System;
using System.IO;
using System.Text.RegularExpressions;
using HSDRaw.Melee.Pl;
using HSDRaw.Tools.Melee;

namespace HSDRawViewer
{
    class TestFunctions
    {
        /// <summary>
        /// Test for rebuilding pobjs from scratch
        /// </summary>
        /// <param name="path"></param>
        public static void RebuildPOBJs(string path)
        {
            HSDRawFile file = new HSDRawFile(path);

            var rootJOBJ = (HSD_JOBJ)(file.Roots[0].Data);

            var compressor = new GX_VertexCompressor();
            foreach (var jobj in rootJOBJ.DepthFirstList)
            {
                if (jobj.Dobj != null)
                    foreach (var dobj in jobj.Dobj.List)
                    {
                        if (dobj.Pobj != null)
                        {
                            GXAttribName[] attributes = null;
                            if (attributes == null)
                            {
                                attributes = new GXAttribName[dobj.Pobj.Attributes.Length - 1];
                                for (int i = 0; i < attributes.Length; i++)
                                    attributes[i] = dobj.Pobj.Attributes[i].AttributeName;
                            }

                            List<GX_Vertex> triList = new List<GX_Vertex>();
                            List<HSD_JOBJ[]> bones = new List<HSD_JOBJ[]>();
                            List<float[]> weights = new List<float[]>();

                            foreach (var pobj in dobj.Pobj.List)
                            {
                                var dl = pobj.ToDisplayList();
                                int off = 0;
                                foreach (var pri in dl.Primitives)
                                {
                                    var strip = dl.Vertices.GetRange(off, pri.Count);
                                    if (pri.PrimitiveType == GXPrimitiveType.TriangleStrip)
                                        TriangleConverter.StripToList(strip, out strip);
                                    if (pri.PrimitiveType == GXPrimitiveType.Quads)
                                        TriangleConverter.QuadToList(strip, out strip);
                                    off += pri.Count;

                                    //if(pobj.Flags.HasFlag(POBJ_FLAG.ENVELOPE))
                                    {
                                        triList.AddRange(strip);

                                        foreach (var v in strip)
                                        {
                                            if (dl.Envelopes.Count > 0)
                                            {
                                                var en = dl.Envelopes[v.PNMTXIDX / 3];
                                                HSD_JOBJ[] b = en.JOBJs;
                                                float[] w = en.Weights;
                                                bones.Add(b);
                                                weights.Add(w);
                                            }
                                            else
                                            {
                                                bones.Add(new HSD_JOBJ[0]);
                                                weights.Add(new float[0]);
                                            }
                                        }
                                    }
                                }
                            }

                            dobj.Pobj = compressor.CreatePOBJsFromTriangleList(triList, attributes, bones, weights);

                            
                            /*List<GX_Vertex> triList = new List<GX_Vertex>();
                            foreach (var pobj in dobj.Pobj.List)
                            {
                                var dl = pobj.DisplayList;
                                var newPrimGroup = new List<GX_PrimitiveGroup>();
                                int offset = 0;
                                foreach (var g in dl.Primitives)
                                {
                                    GX_Vertex[] strip = new GX_Vertex[g.Count];
                                    for (int i = 0; i < g.Count; i++)
                                        strip[i] = dl.Vertices[offset + i];
                                    newPrimGroup.Add(compressor.Compress(g.PrimitiveType, strip, pobj.Attributes));
                                    offset += g.Count;
                                }
                                dl.Primitives = newPrimGroup;
                                pobj.DisplayList = dl;
                            }*/
                        }
                    }
            }
            compressor.SaveChanges();
            file.Save(path + "_rebuilt.dat");
        }

        public static void RebuildFigaTree(string path, string outpath)
        {
            HSDRawFile file = new HSDRawFile(path);
            var oldTree = file.Roots[0].Data as HSD_FigaTree;
            
            HSDRawFile newFile = new HSDRawFile();
            HSD_FigaTree newTree = new HSD_FigaTree();
            newTree.FrameCount = oldTree.FrameCount;
            newFile.Roots = new List<HSDRootNode>();
            newFile.Roots.Add(new HSDRootNode() { Name = file.Roots[0].Name, Data = newTree });

            var newtracks = new List<FigaTreeNode>();
            foreach (var tracks in oldTree.Nodes)
            {
                var newt = new List<HSD_Track>();
                foreach(var track in tracks.Tracks)
                {
                    HSD_Track newtrack = new HSD_Track();
                    newtrack.FOBJ = FOBJFrameEncoder.EncodeFrames(track.GetKeys(), track.FOBJ.AnimationType);
                    newt.Add(newtrack);
                }
                newtracks.Add(new FigaTreeNode() { Tracks = newt });
            }
            newTree.Nodes = newtracks;

            newFile.Save(outpath);
        }

        public static void RemoveImageNodes(string filePath)
        {
            HSDRawFile file = new HSDRawFile(filePath);

            file.Roots.RemoveAll(e => e.Name.EndsWith("_image"));

            file.Save(filePath + "_noImage");
        }

        public static void Compare(string oldpath, string newpath)
        {
            var f1 = new HSDRawFile(oldpath);
            var f2 = new HSDRawFile(newpath);

            var ss1 = f1.Roots[0].Data._s.GetSubStructs();
            var ss2 = f2.Roots[0].Data._s.GetSubStructs();

            Console.WriteLine(f1.Roots[0].Data._s.GetSubStructs().Count + " " + f2.Roots[0].Data._s.GetSubStructs().Count);

            CompareNode(f1.Roots[0].Data._s, f2.Roots[0].Data._s, new HashSet<HSDStruct>());
        }

        public static void DecompileAllPlDats(string path, string outputTxt)
        {
            using (StreamWriter w = new StreamWriter(new FileStream(outputTxt, FileMode.Create)))
                foreach (var f in Directory.GetFiles(path))
                {
                    if (Regex.IsMatch(Path.GetFileName(f), @"Pl..\.dat"))
                    {
                        HSDRawFile hsd = new HSDRawFile(f);

                        foreach (var r in hsd.Roots)
                        {
                            if (r.Data is SBM_PlayerData pl)
                            {
                                w.WriteLine(r.Name);

                                ActionDecompiler d = new ActionDecompiler();

                                var sa = pl.SubActionTable.Subactions;

                                int index = 0;
                                foreach (var v in sa)
                                {
                                    w.WriteLine(d.Decompile("Function_"+index++ + v.Name != null ? "_" + v.Name : "", v));
                                }
                            }
                        }
                    }
                }
        }

        private static void CompareNode(HSDStruct s1, HSDStruct s2, HashSet<HSDStruct> done)
        {
            if (done.Contains(s1))
                return;
            done.Add(s1);

            //System.Console.WriteLine("Checking " + s1.Length + " " + s2.Length + " " + s1.References.Count + " "  + s2.References.Count);
            for (int i = 0; i < s1.Length; i++)
            {
                if (s1.GetByte(i) != s2.GetByte(i))
                {
                    Console.WriteLine("content mismatch");
                }
            }
            if (s1.Length != s2.Length)
            {
                Console.WriteLine("Size mismatch");
            }
            if(s1.References.Count != s2.References.Count)
            {
                Console.WriteLine("Reference mismatch");
            }
            foreach(var re in s2.References)
            {
                var re2 = s2.References[re.Key];
                CompareNode(re.Value, re2, done);
            }
        }
    }
}
