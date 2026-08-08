using HSDRawViewer.Rendering;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace HSDRawViewer.Tools.SpatialOrganizer
{
    public class SpatialBox
    {
        public static readonly int MAX_TRIANGLE_COUNT = 80;

        private readonly BoundingBox box;

        public float MinX => box.Min.X;
        public float MinY => box.Min.Y;
        public float MinZ => box.Min.Z;

        public float MaxX => box.Max.X;
        public float MaxY => box.Max.Y;
        public float MaxZ => box.Max.Z;

        public List<SpatialTriangle> _triangles = new();

        public int TriangleCount => _triangles.Count;

        public SpatialBox Child1 { get; internal set; }
        public SpatialBox Child2 { get; internal set; }

        public float Depth { get; internal set; }

        public SpatialBox(Vector3 min, Vector3 max)
        {
            box = new BoundingBox(min, max);
        }

        public bool AddTriangle(SpatialTriangle t)
        {
            if (!box.Intersects(t.p1, t.p2, t.p3))
                return false;

            _triangles.Add(t);
            return true;
        }

        public bool ContainsPoly(IEnumerable<SpatialTriangle> tris)
        {
            foreach (SpatialTriangle t in tris)
            {
                if (box.Intersects(t.p1, t.p2, t.p3))
                    return true;
            }

            return false;
        }

        private bool FindBestSplit(
            int axis,
            out float bestSplit,
            out int bestLeft,
            out int bestRight,
            out int bestOverlap)
        {
            bestSplit = 0;
            bestLeft = 0;
            bestRight = 0;
            bestOverlap = 0;

            int count = _triangles.Count;

            if (count < 2)
                return false;

            // Get the minimum and maximum coordinate of every triangle
            // along the axis being tested.
            float[] mins = new float[count];
            float[] maxs = new float[count];

            for (int i = 0; i < count; i++)
            {
                SpatialTriangle t = _triangles[i];

                float a = t.p1[axis];
                float b = t.p2[axis];
                float c = t.p3[axis];

                mins[i] = MathF.Min(a, MathF.Min(b, c));
                maxs[i] = MathF.Max(a, MathF.Max(b, c));
            }

            Array.Sort(mins);
            Array.Sort(maxs);

            /*
             * A triangle belongs to:
             *
             * Child1 when triangleMin <= split
             * Child2 when triangleMax >= split
             *
             * This works because both children span the complete
             * parent on the other two axes.
             */

            // All possible positions where membership can change.
            float[] candidates = new float[count * 2];

            Array.Copy(mins, 0, candidates, 0, count);
            Array.Copy(maxs, 0, candidates, count, count);

            Array.Sort(candidates);

            float boxMin = box.Min[axis];
            float boxMax = box.Max[axis];

            int bestMax = int.MaxValue;
            int bestTotal = int.MaxValue;
            int bestDifference = int.MaxValue;

            for (int i = 0; i < candidates.Length - 1; i++)
            {
                float a = candidates[i];
                float b = candidates[i + 1];

                // Skip duplicate event positions.
                if (a == b)
                    continue;

                // Don't split outside the parent.
                if (a <= boxMin || b >= boxMax)
                    continue;

                // Any position between these two coordinates produces
                // exactly the same triangle membership.
                float split = a + (b - a) * 0.5f;

                // Triangle min <= split
                int left = UpperBound(mins, split);

                // Triangle max >= split
                int right = count - LowerBound(maxs, split);

                if (left == 0 || right == 0)
                    continue;

                int maxChild = Math.Max(left, right);
                int total = left + right;
                int difference = Math.Abs(left - right);

                /*
                 * Primary objective:
                 *   Minimize the largest child.
                 *
                 * Secondary:
                 *   Minimize duplication.
                 *
                 * Tertiary:
                 *   Minimize imbalance.
                 */
                bool better =
                    maxChild < bestMax ||
                    (maxChild == bestMax && total < bestTotal) ||
                    (maxChild == bestMax &&
                     total == bestTotal &&
                     difference < bestDifference);

                if (!better)
                    continue;

                bestMax = maxChild;
                bestTotal = total;
                bestDifference = difference;

                bestSplit = split;
                bestLeft = left;
                bestRight = right;
                bestOverlap = total - count;
            }

            return bestMax != int.MaxValue;
        }

        private bool FindBestOverallSplit(
            out int bestAxis,
            out float bestSplit)
        {
            bestAxis = -1;
            bestSplit = 0;

            int bestMax = int.MaxValue;
            int bestTotal = int.MaxValue;
            int bestDifference = int.MaxValue;
            int bestOverlap = int.MaxValue;

            for (int axis = 0; axis < 3; axis++)
            {
                if (!FindBestSplit(
                        axis,
                        out float split,
                        out int left,
                        out int right,
                        out int overlap))
                {
                    continue;
                }

                int maxChild = Math.Max(left, right);
                int total = left + right;
                int difference = Math.Abs(left - right);

                bool better =
                    maxChild < bestMax ||
                    (maxChild == bestMax && total < bestTotal) ||
                    (maxChild == bestMax &&
                     total == bestTotal &&
                     difference < bestDifference) ||
                    (maxChild == bestMax &&
                     total == bestTotal &&
                     difference == bestDifference &&
                     overlap < bestOverlap);

                if (!better)
                    continue;

                bestAxis = axis;
                bestSplit = split;

                bestMax = maxChild;
                bestTotal = total;
                bestDifference = difference;
                bestOverlap = overlap;
            }

            return bestAxis != -1;
        }

        private static int LowerBound(float[] values, float value)
        {
            int low = 0;
            int high = values.Length;

            while (low < high)
            {
                int mid = low + ((high - low) >> 1);

                if (values[mid] < value)
                    low = mid + 1;
                else
                    high = mid;
            }

            return low;
        }

        private static int UpperBound(float[] values, float value)
        {
            int low = 0;
            int high = values.Length;

            while (low < high)
            {
                int mid = low + ((high - low) >> 1);

                if (values[mid] <= value)
                    low = mid + 1;
                else
                    high = mid;
            }

            return low;
        }

        public void Optimize()
        {
            if (TriangleCount <= MAX_TRIANGLE_COUNT)
                return;

            if (!FindBestOverallSplit(
                    out int axis,
                    out float split))
            {
                return;
            }

            /*
             * Make sure the split actually improves the largest child.
             *
             * If the best possible split still leaves a child containing
             * the same number of triangles as the parent, further splitting
             * is unlikely to make progress.
             */
            FindBestSplit(
                axis,
                out split,
                out int left,
                out int right,
                out _);

            if (Math.Max(left, right) >= TriangleCount)
                return;

            SplitOnAxis(axis, split);

            Child1?.Optimize();
            Child2?.Optimize();
        }

        private void SplitOnAxis(int axis, float split)
        {
            Vector3 min = box.Min;
            Vector3 max = box.Max;

            if (axis == 0)
            {
                Child1 = new SpatialBox(
                    new Vector3(min.X, min.Y, min.Z),
                    new Vector3(split, max.Y, max.Z));

                Child2 = new SpatialBox(
                    new Vector3(split, min.Y, min.Z),
                    new Vector3(max.X, max.Y, max.Z));
            }
            else if (axis == 1)
            {
                Child1 = new SpatialBox(
                    new Vector3(min.X, min.Y, min.Z),
                    new Vector3(max.X, split, max.Z));

                Child2 = new SpatialBox(
                    new Vector3(min.X, split, min.Z),
                    new Vector3(max.X, max.Y, max.Z));
            }
            else
            {
                Child1 = new SpatialBox(
                    new Vector3(min.X, min.Y, min.Z),
                    new Vector3(max.X, max.Y, split));

                Child2 = new SpatialBox(
                    new Vector3(min.X, min.Y, split),
                    new Vector3(max.X, max.Y, max.Z));
            }

            /*
             * Since the children cover the entire parent on the other
             * two axes, we only need to test the triangle's extent on
             * the split axis.
             */
            foreach (SpatialTriangle t in _triangles)
            {
                float a = t.p1[axis];
                float b = t.p2[axis];
                float c = t.p3[axis];

                float triMin = MathF.Min(a, MathF.Min(b, c));
                float triMax = MathF.Max(a, MathF.Max(b, c));

                // Triangle touches/intersects Child1.
                if (triMin <= split)
                    Child1._triangles.Add(t);

                // Triangle touches/intersects Child2.
                if (triMax >= split)
                    Child2._triangles.Add(t);
            }

            _triangles.Clear();

            Child1.Depth = Depth + 1;
            Child2.Depth = Depth + 1;
        }

        public override string ToString()
        {
            return $"{box.Min} {box.Max} {_triangles.Count}";
        }
    }

}
