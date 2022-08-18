using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDRawViewer.Tools
{
    /// <summary>
    /// Adapted from Blender "Simplify Curve"
    /// </summary>
    public class Simpoly
    {
        public class Options
        {
            public float k_thresh;      // 2
            public float pointsNr = 5;  // 3
            public float error = 0.01f; // 4
            public float degreeOut = 5; // 5
            public float dis_error;     // 6
        }


        public static void SimplyPoly(FOBJ_Player player, Options o)
        {
            var newVerts = new List<int>();
            var points = player.Keys.Select(e => new Vector3(e.Frame, e.Value, 0)).ToList();
            var pointCurva = new List<List<float>>();
            var curvatures = new List<float>();
            foreach (var p in points)
                pointCurva.Add(new List<float>());

            // get curvatures per vert
            for(int i = 0; i < points.Count - (o.pointsNr - 1); i++)
            {
                var BVerts = new List<Vector3>(0);

                for (int j = i; j < i + o.pointsNr; j++)
                    BVerts.Add(points[j]);

                for (int j = i; j < i + o.pointsNr; j++)
                {
                    var deriv1 = GetDerivative(BVerts, 1 / (o.pointsNr - 1), (int)(o.pointsNr - 1));
                    var deriv2 = GetDerivative(BVerts, 1 / (o.pointsNr - 1), (int)(o.pointsNr - 2));
                    var curva = GetCurvature(deriv1, deriv2);
                    pointCurva[j].Add(curva);
                }
            }

            // average the curvatures
            for(int i = 0; i < points.Count; i++)
            {
                var avgCurva = pointCurva[i].Sum() / (o.pointsNr - 1);
                curvatures.Add(avgCurva);
            }

            // get distance values per vert
            var distances = new List<float>();
            distances.Add(0);
            for(int i = 0; i < points.Count - 2; i++)
            {
                var dist = Altitude(points[i], points[i + 2], points[i + 1]);
                distances.Add(dist);
            }
            distances.Add(0);

            // gererate list of vert indices to keep
            newVerts.Add(0);
            for (int i = 0; i < curvatures.Count; i++)
            {
                if (curvatures[i] >= o.k_thresh * 0.01f || distances[i] >= o.dis_error * 0.1f)
                    newVerts.Add(i);
            }
            newVerts.Add(curvatures.Count - 1);


            // done
            // calculate auto tangents
            foreach(var v in newVerts)
            {
                System.Diagnostics.Debug.WriteLine(points[v].ToString());
            }
        }

        private static float Binom(int n, int m)
        {
            var b = new float[n + 1];
            b[0] = 1;
            for(int i = 0; i < n + 1; i++)
            {
                b[i] = 1;
                int j = i - 1;
                while(j > 0)
                {
                    b[j] += b[j - 1];
                    j -= 1;
                }
            }
            return b[m];
        }

        private static Vector3 GetDerivative(List<Vector3> verts, float t, int nth)
        {
            var order = verts.Count() - 1 - nth;
            var QVerts = new List<Vector3>();

            if (nth != 0)
            {
                for(int i = 0; i < nth; i++)
                {
                    if (QVerts.Count > 0)
                        verts = QVerts;

                    var derivVerts = new List<Vector3>();
                    for (int j = 0; j < verts.Count() - 1; j++)
                        derivVerts.Add(verts[j + 1] - verts[j]);
                    QVerts = derivVerts;
                }
            }
            else
                QVerts = verts;

            var point = new Vector3(0);

            for (int i = 0;  i < QVerts.Count; i++)
            {
                Console.WriteLine($"{i} {order} {QVerts.Count}");
                point += QVerts[i] * (float)(Binom(order, i) * Math.Pow(t, i) * Math.Pow(1 - t, order - i));
            }

            return point;
        }

        private static float GetCurvature(Vector3 deriv1, Vector3 deriv2)
        {
            if (deriv1.Length == 0)
                return 0;

            return (float)(Vector3.Cross(deriv1, deriv2).Length / Math.Pow(deriv1.Length, 3));
        }

        public static float Altitude(Vector3 point1, Vector3 point2, Vector3 pointn)
        {
            var edge1 = point2 - point1;
            var edge2 = pointn - point1;

            if (edge2.Length == 0)
                return 0;

            if (edge1.Length == 0)
                return edge2.Length;

            return (float)Math.Sin(Vector3.CalculateAngle(edge1, edge2)) * edge2.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void AutoTangent(FOBJ_Player p, int index)
        {
            if (index < 0 || index > p.Keys.Count - 1)
                return;

            if (p.Keys[index].InterpolationType != GXInterpolationType.HSD_A_OP_SPL)
                return;

            if (index == 0 || index == p.Keys.Count - 1)
            {
                p.Keys[index].Tan = 0;
                return;
            }

            var prev = p.Keys[index - 1];
            var current = p.Keys[index];
            var next = p.Keys[index + 1];

            float weightCount = 0;
            float tangent = 0;
            {
                tangent += (current.Value - prev.Value) / (current.Frame - prev.Frame);
                weightCount++;
            }
            {
                tangent += (next.Value - current.Value) / (next.Frame - next.Frame);
                weightCount++;
            }

            if (weightCount > 0)
                tangent /= weightCount;

            current.Tan = tangent;
        }
    }
}
