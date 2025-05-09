﻿using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.GX;
using HSDRaw.Melee.Pl;
using HSDRaw.Tools;
using HSDRaw.Tools.Melee;
using HSDRawViewer.Tools;
using HSDRawViewer.Tools.Animation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HSDRawViewer
{
    class TestFunctions
    {
        public static void RebuildPOBJs(HSD_JOBJ rootJOBJ)
        {
            POBJ_Generator compressor = new();
            foreach (HSD_JOBJ jobj in rootJOBJ.TreeList)
            {
                if (jobj.Dobj != null)
                    foreach (HSD_DOBJ dobj in jobj.Dobj.List)
                    {
                        if (dobj.Pobj != null)
                        {
                            List<GX_Vertex> triList = new();
                            List<HSD_JOBJ[]> bones = new();
                            List<float[]> weights = new();

                            foreach (HSD_POBJ pobj in dobj.Pobj.List)
                            {
                                GX_DisplayList dl = pobj.ToDisplayList();
                                int off = 0;
                                foreach (GX_PrimitiveGroup pri in dl.Primitives)
                                {
                                    List<GX_Vertex> strip = dl.Vertices.GetRange(off, pri.Count);
                                    if (pri.PrimitiveType == GXPrimitiveType.TriangleStrip)
                                        TriangleConverter.StripToList(strip, out strip);
                                    if (pri.PrimitiveType == GXPrimitiveType.Quads)
                                        TriangleConverter.QuadToList(strip, out strip);
                                    off += pri.Count;

                                    //if(pobj.Flags.HasFlag(POBJ_FLAG.ENVELOPE))
                                    {
                                        triList.AddRange(strip);

                                        foreach (GX_Vertex v in strip)
                                        {
                                            if (dl.Envelopes.Count > 0)
                                            {
                                                HSD_Envelope en = dl.Envelopes[v.PNMTXIDX / 3];
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

                            dobj.Pobj = compressor.CreatePOBJsFromTriangleList(triList, dobj.Pobj.ToGXAttributes().Select(e => e.AttributeName).ToArray(), bones, weights);
                        }
                    }
            }
            compressor.SaveChanges();
        }

        public static void RebuildFigaTree(string path, string outpath)
        {
            HSDRawFile file = new(path);
            HSD_FigaTree oldTree = file.Roots[0].Data as HSD_FigaTree;

            HSDRawFile newFile = new();
            HSD_FigaTree newTree = new()
            {
                FrameCount = oldTree.FrameCount,
            };
            newFile.Roots = new List<HSDRootNode>();
            newFile.Roots.Add(new HSDRootNode() { Name = file.Roots[0].Name, Data = newTree });

            List<FigaTreeNode> newtracks = new();
            foreach (FigaTreeNode tracks in oldTree.Nodes)
            {
                List<HSD_Track> newt = new();
                foreach (HSD_Track track in tracks.Tracks)
                {
                    HSD_Track newtrack = new();
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
            HSDRawFile file = new(filePath);

            file.Roots.RemoveAll(e => e.Name.EndsWith("_image"));

            file.Save(filePath + "_noImage");
        }

        public static void Compare(string oldpath, string newpath)
        {
            HSDRawFile f1 = new(oldpath);
            HSDRawFile f2 = new(newpath);

            Console.WriteLine(f1.Roots.Count + " " + f2.Roots.Count);

            for (int i = 0; i < f1.Roots.Count; i++)
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
            foreach (KeyValuePair<int, HSDStruct> re in s2.References)
            {
                CompareNode(s1.References[re.Key], s2.References[re.Key], done, path + $"{re.Key.ToString("X8")}->");
            }
        }

        public static void DecompileAllPlDats(string path, string outputTxt)
        {
            using StreamWriter w = new(new FileStream(outputTxt, FileMode.Create));
            foreach (string f in Directory.GetFiles(path))
            {
                if (Regex.IsMatch(Path.GetFileName(f), @"Pl..\.dat"))
                {
                    HSDRawFile hsd = new(f);

                    foreach (HSDRootNode r in hsd.Roots)
                    {
                        if (r.Data is SBM_FighterData pl)
                        {
                            w.WriteLine(r.Name);

                            ActionDecompiler d = new();

                            SBM_FighterAction[] sa = pl.FighterActionTable.Commands;

                            int index = 0;
                            foreach (SBM_FighterAction v in sa)
                            {
                                w.WriteLine(d.Decompile("Function_" + index++ + v.Name != null ? "_" + v.Name : "", v));
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
            List<FigaTreeNode> sourceNodes = from.Nodes;
            List<FigaTreeNode> targetNodes = new();


            for (int i = 0; i < newBoneCount; i++)
            {
                FigaTreeNode node = new();
                targetNodes.Add(node);

                int remapIndex = mapFrom.IndexOf(mapTo[i]);
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


            HSD_FigaTree newft = new()
            {
                Type = 1,
                FrameCount = from.FrameCount,
                Nodes = targetNodes,
            };
            return newft;
        }

        public delegate void EditSubaction(SBM_FighterAction action);
        public delegate bool EditAnimation(HSD_FigaTree ft, string name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ftdat"></param>
        /// <param name="ajdat"></param>
        /// <param name="editAnim"></param>
        public static void EditFighterAnimations(string ftdat, string ajdat, EditAnimation editAnim)
        {
            FighterAJManager manager = new(File.ReadAllBytes(ajdat));

            foreach (string symbol in manager.GetAnimationSymbols())
            {
                if (symbol.Contains("Taro"))
                    continue;

                HSDRawFile ftFile = new(manager.GetAnimationData(symbol));

                if (ftFile[symbol] != null)
                {
                    HSD_FigaTree ft = ftFile[symbol].Data as HSD_FigaTree;
                    if (!editAnim(ft, symbol))
                        continue;
                    ftFile[symbol].Data = ft;

                    using MemoryStream stream = new();
                    ftFile.Save(stream);
                    manager.SetAnimation(symbol, stream.ToArray());
                }
            }

            byte[] newAJFile = manager.RebuildAJFile(manager.GetAnimationSymbols().ToArray(), true);

            HSDRawFile ftfile = new(ftdat);
            if (ftfile.Roots[0].Data is SBM_FighterData data)
            {
                SBM_FighterAction[] sa = data.FighterActionTable.Commands;

                foreach (SBM_FighterAction action in sa)
                {
                    if (action.SymbolName != null && !string.IsNullOrEmpty(action.SymbolName.Value))
                    {
                        Tuple<int, int> sizeOffset = manager.GetOffsetSize(action.SymbolName.Value);
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
            HSDRawFile ftfile = new(ftdat);
            if (ftfile.Roots[0].Data is SBM_FighterData data)
            {
                SBM_FighterAction[] sa = data.FighterActionTable.Commands;

                foreach (SBM_FighterAction action in sa)
                {
                    editAction(action);
                }

                data.FighterActionTable.Commands = sa;

                ftfile.TrimData();
                ftfile.Save(ftdat);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputPath"></param>
        /// <param name="ftdat"></param>
        /// <param name="ftnr"></param>
        /// <param name="costumeIndex"></param>
        public static void GenerateYml(string outputPath, string ftdat, string ftnr, int costumeIndex)
        {
            SBM_FighterData f = new HSDRaw.HSDRawFile(ftdat).Roots[0].Data as HSDRaw.Melee.Pl.SBM_FighterData;
            HSD_JOBJ joint = new HSDRaw.HSDRawFile(ftnr).Roots[0].Data as HSDRaw.Common.HSD_JOBJ;

            using FileStream s = new(outputPath, System.IO.FileMode.Create);
            using StreamWriter w = new(s);
            // determine high poly indices
            w.WriteLine("highpoly:");
            foreach (byte i in f.ModelLookupTables.CostumeVisibilityLookups.Array[costumeIndex].HighPoly[0].LookupEntries[0].Entries)
                w.WriteLine("- " + i);

            // determine low poly indices
            w.WriteLine("lowpoly:");
            foreach (byte i in f.ModelLookupTables.CostumeVisibilityLookups.Array[costumeIndex].LowPoly[0].LookupEntries[0].Entries)
                w.WriteLine("- " + i);

            // determine metal poly indices
            w.WriteLine("metalpoly:");
            if (f.ModelLookupTables.CostumeVisibilityLookups.Array[costumeIndex].MetalMainModel != null &&
                f.ModelLookupTables.CostumeVisibilityLookups.Array[costumeIndex].MetalMainModel[0].LookupEntries != null)
                foreach (byte i in f.ModelLookupTables.CostumeVisibilityLookups.Array[costumeIndex].MetalMainModel[0].LookupEntries[0].Entries)
                    w.WriteLine("- " + i);

            // determine texture counts
            Dictionary<int, int> tobjIndexToDobjIndex = new();
            w.WriteLine("objects:");
            int di = 0;
            int tobji = 0;
            List<HSD_JOBJ> jointList = joint.TreeList;
            foreach (HSD_JOBJ j in joint.TreeList)
            {
                if (j.Dobj != null)
                {
                    foreach (HSD_DOBJ dobj in j.Dobj.List)
                    {
                        int texCount = (dobj.Mobj.Textures == null ? 0 : dobj.Mobj.Textures.List.Count);
                        w.WriteLine($"- position: {di}");
                        w.WriteLine($"  count: {texCount}");
                        w.WriteLine($"  joint: {jointList.IndexOf(j)}");
                        for (int i = tobji; i < tobji + texCount; i++)
                            tobjIndexToDobjIndex.Add(i, di);
                        tobji += texCount;
                        di++;
                    }
                }
            }

            // determine specail positions
            w.WriteLine("positions:");
            {
                int pi = 0;
                foreach (SBM_LookupTable p in f.ModelLookupTables.CostumeVisibilityLookups.Array[costumeIndex].HighPoly.Array)
                {
                    int ti = 0;
                    if (p.LookupEntries != null)
                        foreach (SBM_LookupEntry t in p.LookupEntries.Array)
                        {
                            int ei = 0;
                            if (ti != 0)
                                foreach (byte i in t.Entries)
                                {
                                    w.WriteLine($"- name: Object_{pi}_{ti}_{ei}");
                                    w.WriteLine($"  position: {i}");
                                    ei++;
                                }
                            ti++;
                        }
                    pi++;
                }
            }
            {
                int pi = 0;
                foreach (SBM_LookupTable p in f.ModelLookupTables.CostumeVisibilityLookups.Array[costumeIndex].LowPoly.Array)
                {
                    int ti = 0;
                    if (p.LookupEntries != null)
                        foreach (SBM_LookupEntry t in p.LookupEntries.Array)
                        {
                            int ei = 0;
                            if (ti != 0)
                                foreach (byte i in t.Entries)
                                {
                                    w.WriteLine($"- name: Object_{pi}_{ti}_{ei}_LOW");
                                    w.WriteLine($"  position: {i}");
                                    ei++;
                                }
                            ti++;
                        }
                    pi++;
                }
            }

            // determine mat anim texture indices
            if (f.ModelLookupTables.CostumeMaterialLookups != null && f.ModelLookupTables.CostumeMaterialLookups[costumeIndex].Entries != null)
            {
                int mi = 0;
                foreach (ushort i in f.ModelLookupTables.CostumeMaterialLookups[costumeIndex].Entries.Array)
                {
                    w.WriteLine($"- name: MatAnim_{mi}");
                    w.WriteLine($"  position: {tobjIndexToDobjIndex[i]}");
                    mi++;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        public static void CheckFlags(string file)
        {
            HSDRawFile f = new(file);

            CheckFlags(f.Roots[0].Data as HSD_JOBJ);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        public static void CheckFlags(HSD_JOBJ jobj)
        {
            List<JOBJ_FLAG> originalFlags = jobj.TreeList.Select(e => e.Flags).ToList();

            foreach (HSD_JOBJ v in jobj.TreeList)
                v.Flags = 0;

            jobj.UpdateFlags();
            JOBJ_FLAG ignore = JOBJ_FLAG.CLASSICAL_SCALING | JOBJ_FLAG.BILLBOARD | JOBJ_FLAG.HBILLBOARD | JOBJ_FLAG.VBILLBOARD | JOBJ_FLAG.PTCL | JOBJ_FLAG.SPLINE;

            List<HSD_JOBJ> list = jobj.TreeList;
            for (int i = 0; i < originalFlags.Count; i++)
            {
                if ((originalFlags[i] & ~ignore) != (list[i].Flags & ~ignore))
                {
                    System.Diagnostics.Debug.WriteLine($"{i}");
                    System.Diagnostics.Debug.WriteLine($"\t{originalFlags[i]}");
                    System.Diagnostics.Debug.WriteLine($"\t{list[i].Flags}");
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="datFile"></param>
        /// <param name="ajFile"></param>
        public static void FigtherApplyDiscontinuityFilterToAllAnimations(string datFile, string ajFile)
        {
            List<string> filter = new();
            EditFighterAnimations(
                datFile,
                ajFile,
                (tree, symbol) =>
                {
                    bool filtered = false;
                    List<FigaTreeNode> nodes = tree.Nodes;
                    foreach (FigaTreeNode c in nodes)
                    {
                        List<FOBJ_Player> tracks = c.Tracks.Select(e => new FOBJ_Player(e.TrackType, e.GetKeys())).ToList();
                        if (Tools.KeyFilters.DiscontinuityFilter.Filter(tracks))
                        {
                            filtered = true;
                        }
                        else
                        {
                            continue;
                        }
                        c.Tracks = tracks.Select(e => new HSD_Track(e.ToFobj())).ToList();
                    }
                    tree.Nodes = nodes;
                    if (filtered)
                        filter.Add(symbol);
                    return filtered;
                });
            System.Diagnostics.Debug.WriteLine(string.Join("\n", filter));
        }
    }
}
