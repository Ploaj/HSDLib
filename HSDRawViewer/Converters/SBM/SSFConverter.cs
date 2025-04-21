﻿using HSDRaw.Melee.Gr;
using HSDRawViewer.GUI.Plugins.Melee;
using System;
using System.Collections.Generic;

namespace HSDRawViewer.Converters.Melee
{
    public class SSFConverter
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collData"></param>
        public static void ExportCollDataToSSF(SBM_Coll_Data collData)
        {
            string f = Tools.FileIO.SaveFile("Smash Stage File (SSF)|*.ssf");

            if (f == null)
                return;

            SSF ssf = new();

            List<CollLineGroup> LineGroups = new();
            List<CollLine> Lines = new();

            CollDataBuilder.LoadCollData(collData, LineGroups, Lines);

            int groupIndex = 0;
            foreach (CollLineGroup g in LineGroups)
            {
                SSFGroup group = new();
                group.Name = $"Group_{groupIndex++}";

                ssf.Groups.Add(group);

                Dictionary<CollVertex, int> vertexToIndex = new();

                // bone is unknown to coll_data

                foreach (CollLine l in Lines)
                {
                    if (l.Group == g)
                    {
                        SSFLine line = new();

                        if (l.Flag.HasFlag(CollProperty.DropThrough))
                            line.Flags |= SSFLineFlag.DropThrough;

                        if (l.Flag.HasFlag(CollProperty.LedgeGrab))
                            line.Flags |= SSFLineFlag.LeftLedge; //TODO: proper ledge

                        line.Material = l.Material.ToString().ToLower();

                        if (!vertexToIndex.ContainsKey(l.v1))
                        {
                            vertexToIndex.Add(l.v1, group.Vertices.Count);
                            group.Vertices.Add(new SSFVertex() { X = l.X1, Y = l.Y1 });
                        }

                        if (!vertexToIndex.ContainsKey(l.v2))
                        {
                            vertexToIndex.Add(l.v2, group.Vertices.Count);
                            group.Vertices.Add(new SSFVertex() { X = l.X2, Y = l.Y2 });
                        }

                        line.Vertex1 = vertexToIndex[l.v1];
                        line.Vertex2 = vertexToIndex[l.v2];

                        group.Lines.Add(line);
                    }
                }
            }

            // TODO: general points

            ssf.Save(f);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="collData"></param>
        /// <param name="ssf"></param>
        public static void ImportCollDataFromSSF(SBM_Coll_Data collData, SSF ssf)
        {
            List<CollLine> lines = new();
            List<CollLineGroup> groups = new();

            foreach (SSFGroup v in ssf.Groups)
            {
                CollLineGroup group = new();
                List<CollVertex> vertices = new();
                foreach (SSFVertex vert in v.Vertices)
                {
                    vertices.Add(new CollVertex(vert.X, vert.Y));
                }
                foreach (SSFLine l in v.Lines)
                {
                    CollLine line = new();
                    line.v1 = vertices[l.Vertex1];
                    line.v2 = vertices[l.Vertex2];

                    float slope = line.Slope;

                    line.Group = group;
                    if (l.Flags.HasFlag(SSFLineFlag.LeftLedge) || l.Flags.HasFlag(SSFLineFlag.RightLedge))
                        line.Flag |= CollProperty.LedgeGrab;

                    if (l.Flags.HasFlag(SSFLineFlag.DropThrough))
                        line.Flag |= CollProperty.DropThrough;

                    line.Material = MaterialTranslate(l.Material);
                    line.GuessCollisionFlag();
                    lines.Add(line);
                }

                group.CalcuateRange(vertices);
                groups.Add(group);
            }

            CollDataBuilder.GenerateCollData(lines, groups, collData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        private static CollMaterial MaterialTranslate(string mat)
        {
            if (Enum.TryParse(mat, true, out CollMaterial mater))
                return mater;

            switch (mat.ToLower())
            {
                case "brick":
                case "rock":
                    return CollMaterial.Rock;
                case "wood":
                    return CollMaterial.Wood;
                case "soil":
                    return CollMaterial.Dirt;
                case "lightmetal":
                    return CollMaterial.LightMetal;
                case "heavymetal":
                    return CollMaterial.HeavyMetal;
                case "cloud":
                default:
                    return CollMaterial.Basic;
            }
        }
    }
}
