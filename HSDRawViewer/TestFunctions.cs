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
using System.Linq;
using HSDRawViewer.Tools;

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

            file.Save(path + "_rebuilt.dat");
        }

        public static void RebuildPOBJs(HSD_JOBJ rootJOBJ)
        {
            var compressor = new POBJ_Generator();
            foreach (var jobj in rootJOBJ.BreathFirstList)
            {
                if (jobj.Dobj != null)
                    foreach (var dobj in jobj.Dobj.List)
                    {
                        if (dobj.Pobj != null)
                        {
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

                            dobj.Pobj = compressor.CreatePOBJsFromTriangleList(triList, dobj.Pobj.ToGXAttributes().Select(e=>e.AttributeName).ToArray(), bones, weights);
                        }
                    }
            }
            compressor.SaveChanges();
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
                    newtrack.FromFOBJ(FOBJFrameEncoder.EncodeFrames(track.GetKeys(), track.JointTrackType));
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

            Console.WriteLine(f1.Roots.Count + " " + f2.Roots.Count);

            for(int i = 0; i < f1.Roots.Count; i++)
            {
                CompareNode(f1.Roots[i].Data._s, f2.Roots[i].Data._s, new HashSet<HSDStruct>(), $"{(i * 4).ToString("X8")}->");
            }
        }

        private static void CompareNode(HSDStruct s1, HSDStruct s2, HashSet<HSDStruct> done, string path)
        {
            if (done.Contains(s1))
                return;
            done.Add(s1);

            //System.Console.WriteLine("Checking " + s1.Length + " " + s2.Length + " " + s1.References.Count + " "  + s2.References.Count);
            if (s1.Length != s2.Length)
            {
                Console.WriteLine($"{path} Size mismatch");
            }
            else
                for (int i = 0; i < s1.Length; i++)
                {
                    if (s1.GetByte(i) != s2.GetByte(i))
                    {
                        Console.WriteLine($"{path} Content mismatch");
                    }
                }
            if (s1.References.Count != s2.References.Count)
            {
                Console.WriteLine($"{path} Reference mismatch");
            }
            foreach (var re in s2.References)
            {
                CompareNode(s1.References[re.Key], s2.References[re.Key], done, path + $"{re.Key.ToString("X8")}->");
            }
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
                            if (r.Data is SBM_FighterData pl)
                            {
                                w.WriteLine(r.Name);

                                ActionDecompiler d = new ActionDecompiler();

                                var sa = pl.FighterActionTable.Commands;

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

        public class RemapSettings
        {
            public bool IgnoreTranslation { get; set; } = true;
        }

        public static HSD_FigaTree RemapFigatree(RemapSettings settings, HSD_FigaTree from, int newBoneCount, JointMap mapFrom, JointMap mapTo)
        {
            var sourceNodes = from.Nodes;
            var targetNodes = new List<FigaTreeNode>();
            

            for (int i = 0; i < newBoneCount; i++)
            {
                FigaTreeNode node = new FigaTreeNode();
                targetNodes.Add(node);

                var remapIndex = mapFrom.IndexOf(mapTo[i]);
                if (remapIndex != -1)
                {
                    // port tracks
                    node.Tracks.AddRange(sourceNodes[remapIndex].Tracks);

                    if (settings.IgnoreTranslation)
                    {
                        node.Tracks.RemoveAll(
                            e => 
                            e.JointTrackType == JointTrackType.HSD_A_J_TRAX ||
                            e.JointTrackType == JointTrackType.HSD_A_J_TRAY || 
                            e.JointTrackType == JointTrackType.HSD_A_J_TRAZ);
                    }
                }
            }


            var newft = new HSD_FigaTree();
            newft.Type = 1;
            newft.FrameCount = from.FrameCount;
            newft.Nodes = targetNodes;
            return newft;
        }


        public delegate void EditSubaction(SBM_FighterAction action);
        public delegate void EditAnimation(HSD_FigaTree ft, string name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ftdat"></param>
        /// <param name="ajdat"></param>
        /// <param name="editAnim"></param>
        public static void EditFighterAnimations(string ftdat, string ajdat, EditAnimation editAnim)
        {
            FighterAJManager manager = new FighterAJManager(File.ReadAllBytes(ajdat));

            foreach (var symbol in manager.GetAnimationSymbols())
            {
                if (symbol.Contains("Taro"))
                    continue;

                var ftFile = new HSDRawFile(manager.GetAnimationData(symbol));

                if (ftFile[symbol] != null)
                {
                    var ft = ftFile[symbol].Data as HSD_FigaTree;
                    editAnim(ft, symbol);
                    ftFile[symbol].Data = ft;

                    using (MemoryStream stream = new MemoryStream())
                    {
                        ftFile.Save(stream);
                        manager.SetAnimation(symbol, stream.ToArray());
                    }
                }
            }

            var newAJFile = manager.RebuildAJFile(manager.GetAnimationSymbols().ToArray(), true);

            HSDRawFile ftfile = new HSDRawFile(ftdat);
            if (ftfile.Roots[0].Data is SBM_FighterData data)
            {
                var sa = data.FighterActionTable.Commands;

                foreach (var action in sa)
                {
                    if (action.SymbolName != null && !string.IsNullOrEmpty(action.SymbolName.Value))
                    {
                        var sizeOffset = manager.GetOffsetSize(action.SymbolName.Value);
                        action.AnimationOffset = sizeOffset.Item1;
                        action.AnimationSize = sizeOffset.Item2;
                    }
                }

                data.FighterActionTable.Commands = sa;

                ftfile.TrimData();
                ftfile.Save(ftdat);
                File.WriteAllBytes(ajdat, newAJFile);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ftdat"></param>
        /// <param name="ajdat"></param>
        /// <param name="editAnim"></param>
        public static void EditFighterActions(string ftdat, EditSubaction editAction)
        {
            HSDRawFile ftfile = new HSDRawFile(ftdat);
            if (ftfile.Roots[0].Data is SBM_FighterData data)
            {
                var sa = data.FighterActionTable.Commands;

                foreach (var action in sa)
                {
                    editAction(action);
                }

                data.FighterActionTable.Commands = sa;

                ftfile.TrimData();
                ftfile.Save(ftdat);
            }
        }
    }
}
