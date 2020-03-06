using HSDRaw.Melee.Gr;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HSDRawViewer.GUI.Plugins.Melee
{

    public class CollDataBuilder
    {
        /// <summary>
        /// Dumps all the collision information in the <see cref="SBM_Coll_Data"/> structure
        /// </summary>
        public static void GenerateCollData(IEnumerable<CollLine> Lines, IEnumerable<CollLineGroup> LineGroups, SBM_Coll_Data CollData)
        {
            //TODO: Optimize

            // gather all tops
            var topCount = Lines.Count(e => e.CollisionFlag == CollPhysics.Top && !e.DynamicCollision);

            // gather all bottoms
            var bottomCount = Lines.Count(e => e.CollisionFlag == CollPhysics.Bottom && !e.DynamicCollision);

            // gather all rights
            var leftCount = Lines.Count(e => e.CollisionFlag == CollPhysics.Left && !e.DynamicCollision);

            // gather all lefts
            var rightCount = Lines.Count(e => e.CollisionFlag == CollPhysics.Right && !e.DynamicCollision);

            // gather all dynamic collisions
            var dynamicCount = Lines.Count(e => e.DynamicCollision);

            int topOffset = 0;
            int bottomOffset = topCount;
            int rightOffset = bottomOffset + bottomCount;
            int leftOffset = rightOffset + rightCount;
            int dynamicOffset = leftOffset + leftCount;
            int totalLines = dynamicOffset + dynamicCount;

            // create each group creating vertices and links as necessary

            // TODO: 

            SBM_CollLine[] newLines = new SBM_CollLine[totalLines];
            List<SBM_CollVertex> vertices = new List<SBM_CollVertex>();

            // cache
            Dictionary<CollLine, SBM_CollLine> lineToCollLine = new Dictionary<CollLine, SBM_CollLine>();
            Dictionary<SBM_CollLine, int> collLineToIndex = new Dictionary<SBM_CollLine, int>();
            Dictionary<SBM_CollLine, CollLine> lineToAltNext = new Dictionary<SBM_CollLine, CollLine>();
            Dictionary<SBM_CollLine, CollLine> lineToAltPrev = new Dictionary<SBM_CollLine, CollLine>();

            // groups
            List<SBM_CollLineGroup> groups = new List<SBM_CollLineGroup>();
            int boff = 0, toff = 0, loff = 0, roff = 0, doff = 0;
            foreach (var g in LineGroups)
            {
                var lines = Lines.Where(e => e.Group == g).ToList();
                var groupBottom = lines.Count(e => e.CollisionFlag == CollPhysics.Bottom && !e.DynamicCollision);
                var groupTop = lines.Count(e => e.CollisionFlag == CollPhysics.Top && !e.DynamicCollision);
                var groupRight = lines.Count(e => e.CollisionFlag == CollPhysics.Right && !e.DynamicCollision);
                var groupLeft = lines.Count(e => e.CollisionFlag == CollPhysics.Left && !e.DynamicCollision);
                var groupDynamic = lines.Count(e => e.DynamicCollision);

                var group = new SBM_CollLineGroup();
                group.XMin = g.Range.X;
                group.YMin = g.Range.Y;
                group.XMax = g.Range.Z;
                group.YMax = g.Range.W;
                groups.Add(group);

                if (groupTop > 0)
                    group.TopLineIndex = (short)(topOffset + toff);
                group.TopLineCount = (short)groupTop;
                if (groupBottom > 0)
                    group.BottomLineIndex = (short)(bottomOffset + boff);
                group.BottomLineCount = (short)groupBottom;
                if (groupRight > 0)
                    group.RightLineIndex = (short)(rightOffset + roff);
                group.RightLineCount = (short)groupRight;
                if (groupLeft > 0)
                    group.LeftLineIndex = (short)(leftOffset + loff);
                group.LeftLineCount = (short)groupLeft;
                if (groupDynamic > 0)
                    group.DynamicLineIndex = (short)(dynamicOffset + doff);
                group.DynamicLineCount = (short)groupDynamic;

                Dictionary<CollLine, CollLine> nextPointToLine = new Dictionary<CollLine, CollLine>();
                Dictionary<CollLine, CollLine> prevPointToLine = new Dictionary<CollLine, CollLine>();
                List<Vector2> groupVertices = new List<Vector2>();
                foreach (var l in lines)
                {
                    var line = new SBM_CollLine();

                    line.NextLineAltGroup = -1;
                    line.NextLine = -1;
                    line.PreviousLine = -1;
                    line.PreviousLineAltGroup = -1;

                    var v1 = l.v1.ToVector2();
                    var v2 = l.v2.ToVector2();

                    var prevPoint = lines.Find(e => e != l && e.v1 == l.v2);
                    var nextPoint = lines.Find(e => e != l && e.v2 == l.v1);
                    if (prevPoint != null)
                        prevPointToLine.Add(l, prevPoint);
                    if (nextPoint != null)
                        nextPointToLine.Add(l, nextPoint);

                    if (l.AltNext != null && Lines.Contains(l.AltNext))
                        lineToAltNext.Add(line, l.AltNext);

                    if (l.AltPrevious != null && Lines.Contains(l.AltPrevious))
                        lineToAltPrev.Add(line, l.AltPrevious);

                    // set vertices
                    // TODO: ew, clean this
                    if (!groupVertices.Contains(v1))
                        groupVertices.Add(v1);

                    if (!groupVertices.Contains(v2))
                        groupVertices.Add(v2);

                    line.VertexIndex1 = (short)(vertices.Count + groupVertices.IndexOf(v1));
                    line.VertexIndex2 = (short)(vertices.Count + groupVertices.IndexOf(v2));
                    line.Material = l.Material;
                    line.Flag = l.Flag;
                    line.CollisionFlag = l.CollisionFlag;

                    // set the index

                    int lineIndex = -1;

                    if (l.DynamicCollision)
                        lineIndex = dynamicOffset + doff++;
                    else
                        switch (l.CollisionFlag)
                        {
                            case CollPhysics.Bottom:
                                lineIndex = bottomOffset + boff++;
                                break;
                            case CollPhysics.Top:
                                lineIndex = topOffset + toff++;
                                break;
                            case CollPhysics.Left:
                                lineIndex = leftOffset + loff++;
                                break;
                            case CollPhysics.Right:
                                lineIndex = rightOffset + roff++;
                                break;
                        }

                    collLineToIndex.Add(line, lineIndex);
                    lineToCollLine.Add(l, line);
                    newLines[lineIndex] = line;
                }

                // Update Links
                foreach (var l in lines)
                {
                    var line = lineToCollLine[l];

                    if (prevPointToLine.ContainsKey(l))
                        line.PreviousLine = (short)collLineToIndex[lineToCollLine[prevPointToLine[l]]];

                    if (nextPointToLine.ContainsKey(l))
                        line.NextLine = (short)collLineToIndex[lineToCollLine[nextPointToLine[l]]];
                }

                group.VertexStart = (short)vertices.Count;
                group.VertexCount = (short)groupVertices.Count;

                foreach (var v in groupVertices)
                    vertices.Add(new SBM_CollVertex() { X = v.X, Y = v.Y });
            }

            // update alt group links
            foreach (var v in newLines)
            {
                if (lineToAltNext.ContainsKey(v))
                    v.NextLineAltGroup = (short)collLineToIndex[lineToCollLine[lineToAltNext[v]]];

                if (lineToAltPrev.ContainsKey(v))
                    v.PreviousLineAltGroup = (short)collLineToIndex[lineToCollLine[lineToAltPrev[v]]];
            }

            // dump to file

            if (topCount != 0)
            {
                CollData.TopLinksOffset = (short)topOffset;
                CollData.TopLinksCount = (short)topCount;
            }
            if (bottomCount != 0)
            {
                CollData.BottomLinksOffset = (short)bottomOffset;
                CollData.BottomLinksCount = (short)bottomCount;
            }
            if (rightCount != 0)
            {
                CollData.RightLinksOffset = (short)rightOffset;
                CollData.RightLinksCount = (short)rightCount;
            }
            if (leftCount != 0)
            {
                CollData.LeftLinksOffset = (short)leftOffset;
                CollData.LeftLinksCount = (short)leftCount;
            }
            if (dynamicCount != 0)
            {
                CollData.DynamicLinksOffset = (short)dynamicOffset;
                CollData.DynamicLinksCount = (short)dynamicCount;
            }

            CollData.Vertices = vertices.ToArray();
            CollData.Links = newLines.ToArray();
            CollData.LineGroups = groups.ToArray();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CollLineGroup
    {
        public Vector4 Range = new Vector4();

        public float MinX { get => Range.X; set => Range.X = value; }
        public float MinY { get => Range.Y; set => Range.Y = value; }
        public float MaxX { get => Range.Z; set => Range.Z = value; }
        public float MaxY { get => Range.W; set => Range.W = value; }

        public void CalcuateRange(IList<CollVertex> vertices)
        {
            Vector4 newRange = new Vector4(float.MaxValue, float.MaxValue, float.MinValue, float.MinValue);

            foreach (var v in vertices)
            {
                newRange.X = Math.Min(newRange.X, v.X);
                newRange.Y = Math.Min(newRange.Y, v.Y);
                newRange.Z = Math.Max(newRange.Z, v.X);
                newRange.W = Math.Max(newRange.W, v.Y);
            }
            // add 8 in each direction
            newRange += new Vector4(-8, -8, 8, 8);

            Range = newRange;
        }

        public override string ToString()
        {
            return "Polygon Group";
        }
    }

    public class CollLine
    {
        public CollVertex v1;
        public CollVertex v2;

        public CollLineGroup Group = null;

        public CollLine AltNext = null;
        public CollLine AltPrevious = null;

        public float X1 { get => v1.X; set => v1.X = value; }
        public float Y1 { get => v1.Y; set => v1.Y = value; }
        public float X2 { get => v2.X; set => v2.X = value; }
        public float Y2 { get => v2.Y; set => v2.Y = value; }

        public bool DynamicCollision { get; set; } = false;

        public CollPhysics CollisionFlag { get; set; } = CollPhysics.Bottom;
        public CollProperty Flag { get; set; } = 0;
        public CollMaterial Material { get; set; } = CollMaterial.Basic;

        public float Slope { get => (Y2 - Y1) / (X2 - X1); }

        public override string ToString()
        {
            return $"Line: ({v1.X}, {v1.Y}) ({v2.X}, {v2.Y})";
        }
    }

    public class CollVertex
    {
        public float X { get; set; }
        public float Y { get; set; }

        public CollVertex(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }
    }

}
