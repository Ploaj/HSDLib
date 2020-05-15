using HSDRaw.Melee.Gr;
using HSDRawViewer.GUI.Plugins.Melee;
using System.Collections.Generic;

namespace HSDRawViewer.Converters.Melee
{
    public class SSFConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collData"></param>
        /// <param name="ssf"></param>
        public static void ImportCollDataFromSSF(SBM_Coll_Data collData, SSF ssf)
        {
            List<CollLine> lines = new List<CollLine>();
            List<CollLineGroup> groups = new List<CollLineGroup>();

            foreach (var v in ssf.Groups)
            {
                var group = new CollLineGroup();
                List<CollVertex> vertices = new List<CollVertex>();
                foreach (var vert in v.Vertices)
                {
                    vertices.Add(new CollVertex(vert.X, vert.Y));
                }
                foreach (var l in v.Lines)
                {
                    var line = new CollLine();
                    line.v1 = vertices[l.Vertex1];
                    line.v2 = vertices[l.Vertex2];

                    var slope = line.Slope;

                    line.Group = group;
                    if (l.Flags.HasFlag(SSFLineFlag.LeftLedge) || l.Flags.HasFlag(SSFLineFlag.RightLedge))
                        line.Flag |= CollProperty.LedgeGrab;

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
