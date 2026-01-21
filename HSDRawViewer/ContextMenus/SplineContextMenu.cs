using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using YamlDotNet.Core;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace HSDRawViewer.ContextMenus
{
    public class SplineContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(HSD_Spline) };

        public SplineContextMenu() : base()
        {
            ToolStripMenuItem importobj = new("Import OBJ");
            importobj.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_Spline spline)
                {
                    string f = Tools.FileIO.OpenFile("Wavefront OBJ(*.obj)|*.obj");

                    if (f != null)
                    {
                        SplineOBJ obj = new();
                        obj.Open(f);

                        if (obj.Polys.Count == 0)
                        {
                            MessageBox.Show("No splines found!", "OBJ Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        if (obj.Polys.Count > 1)
                        {
                            MessageBox.Show($"Multiple splines found\nUsing {obj.Polys[0].Name}", "OBJ Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        Poly poly = obj.Polys[0];

                        // connect the lines to form one line
                        List<HSD_Vector3> lpoints = new();
                        foreach (int i in poly.GetConnectedLine())
                        {
                            lpoints.Add(obj.Vertices[i]);
                        }

                        // calculate total length
                        float total_length = 0;
                        for (int i = 1; i < lpoints.Count; i++)
                        {
                            float length = (lpoints[i] - lpoints[i - 1]).Length;
                            total_length += length;
                        }

                        // calculate lengths
                        float[] lengths = new float[lpoints.Count];
                        float t = 0;
                        for (int i = 0; i < lpoints.Count; i++)
                        {
                            if (i == 0)
                            {
                                lengths[0] = 0;
                            }
                            else
                            {
                                float length = (lpoints[i] - lpoints[i - 1]).Length;
                                t += length;
                                lengths[i] = t / total_length;
                            }
                        }

                        // dump to spline
                        spline.TotalLength = total_length;
                        spline.Points = lpoints.ToArray();
                        spline.Lengths = new HSDRaw.HSDFloatArray() { Array = lengths };

                        return;
                    }
                }
            };
            Items.Add(importobj);


            ToolStripMenuItem Exportobj = new("Export OBJ");
            Exportobj.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_Spline spline)
                {
                    string f = Tools.FileIO.SaveFile("Wavefront OBJ(*.obj)|*.obj");

                    if (f != null)
                    {
                        using FileStream stream = new(f, FileMode.Create);
                        using StreamWriter s = new(stream);
                        HSD_Vector3[] points = spline.Points;

                        foreach (HSD_Vector3 p in points)
                        {
                            s.WriteLine($"v {p.X} {p.Y} {p.Z}");
                        }

                        int i = 1;
                        s.WriteLine($"o spline");
                        s.Write($"l ");
                        foreach (HSD_Vector3 p in points)
                        {
                            s.Write($"{i++} ");
                        }
                    }
                }
            };
            Items.Add(Exportobj);


            ToolStripMenuItem anim = new("Generate Anim Joint");
            anim.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_Spline spline)
                {
                    string f = Tools.FileIO.SaveFile(ApplicationSettings.HSDFileFilter);

                    if (f != null)
                    {
                        HSDRaw.HSDRawFile file = new();

                        file.Roots.Add(new HSDRaw.HSDRootNode()
                        {
                            Name = "_animjoint",
                            Data = GenerateAnimJoint(spline)
                        });

                        file.Save(f);
                    }
                }
            };
            Items.Add(anim);

#if DEBUG

            ToolStripMenuItem validate = new("Debug Validate");
            validate.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_Spline spline)
                {
                    var p = spline.Points;
                    var len = spline.Lengths;

                    float total_length = 0;
                    for (int i = 0; i < p.Length - 1; i++)
                    {
                        var a = p[i];
                        var b = p[i + 1];
                        var dis = Vector3.Distance(
                                new Vector3(a.X, a.Y, a.Z),
                                new Vector3(b.X, b.Y, b.Z));
                        total_length += dis;
                        System.Diagnostics.Debug.WriteLine($"{dis} {total_length}");
                    }

                    float l = 0;
                    for (int i = 0; i < p.Length-1; i++)
                    {
                        var a = p[i];
                        var b = p[i + 1];
                        var dis = Vector3.Distance(
                                new Vector3(a.X, a.Y, a.Z),
                                new Vector3(b.X, b.Y, b.Z));
                        System.Diagnostics.Debug.WriteLine((l / total_length) + " " + len[i]);
                        l += dis;
                    }

                    if (spline.SegPolys != null)
                    {
                        ValidateSegPoly(spline);
                    }
                }
            };
            Items.Add(validate);
#endif
        }

        static float[] Solve5x5(float[,] A, float[] B)
        {
            const int N = 5;
            float[,] M = new float[N, N + 1];

            // Build augmented matrix
            for (int r = 0; r < N; r++)
            {
                for (int c = 0; c < N; c++)
                    M[r, c] = A[r, c];

                M[r, N] = B[r];
            }

            // Forward elimination
            for (int i = 0; i < N; i++)
            {
                // Pivot
                int maxRow = i;
                float maxVal = Math.Abs(M[i, i]);

                for (int r = i + 1; r < N; r++)
                {
                    float v = Math.Abs(M[r, i]);
                    if (v > maxVal)
                    {
                        maxVal = v;
                        maxRow = r;
                    }
                }

                if (maxVal < 1e-8f)
                    return new float[5]; // Degenerate, return zeros

                // Swap
                if (maxRow != i)
                {
                    for (int c = i; c <= N; c++)
                    {
                        float tmp = M[i, c];
                        M[i, c] = M[maxRow, c];
                        M[maxRow, c] = tmp;
                    }
                }

                // Normalize row
                float invPivot = 1f / M[i, i];
                for (int c = i; c <= N; c++)
                    M[i, c] *= invPivot;

                // Eliminate
                for (int r = 0; r < N; r++)
                {
                    if (r == i) continue;

                    float factor = M[r, i];
                    for (int c = i; c <= N; c++)
                        M[r, c] -= factor * M[i, c];
                }
            }

            // Extract solution
            float[] x = new float[5];
            for (int i = 0; i < N; i++)
                x[i] = M[i, N];

            return x;
        }


        static float[] FitQuartic(float[] s, float[] t)
        {
            int n = s.Length;
            if (t.Length != n)
                throw new ArgumentException("s and t must have same length");

            // Build normal equation matrices
            // M = Aᵀ A (5x5)
            // B = Aᵀ y (5)
            float[,] M = new float[5, 5];
            float[] B = new float[5];

            for (int i = 0; i < n; i++)
            {
                float s1 = s[i];
                float s2 = s1 * s1;
                float s3 = s2 * s1;
                float s4 = s2 * s2;

                float[] row =
                {
                    s4, // s^4
                    s3, // s^3
                    s2, // s^2
                    s1, // s
                    1f
                };

                for (int r = 0; r < 5; r++)
                {
                    B[r] += row[r] * t[i];

                    for (int c = 0; c < 5; c++)
                        M[r, c] += row[r] * row[c];
                }
            }

            return Solve5x5(M, B);
        }


        static Vector3 EvalSegment(
            Vector3 P0,
            Vector3 P1,
            Vector3 T0,
            Vector3 T1,
            float t)
        {
            // Hermite basis (unit t)
            float t2 = t * t;
            float t3 = t2 * t;

            return
                (2 * t3 - 3 * t2 + 1) * P0 +
                (t3 - 2 * t2 + t) * T0 +
                (-2 * t3 + 3 * t2) * P1 +
                (t3 - t2) * T1;
        }


        const int SAMPLES = 8;

        static float[] BuildSegPoly(
            Func<float, Vector3> evalPos,
            float segLength)
        {
            float[] s = new float[SAMPLES];
            float[] t = new float[SAMPLES];

            Vector3 prev = evalPos(0f);
            s[0] = 0f;
            t[0] = 0f;

            for (int i = 1; i < SAMPLES; i++)
            {
                t[i] = i / (float)(SAMPLES - 1);
                Vector3 p = evalPos(t[i]);
                s[i] = s[i - 1] + Vector3.Distance(prev, p);
                prev = p;
            }

            // Normalize s → [0, segLength]
            float scale = segLength / s[SAMPLES - 1];
            for (int i = 0; i < SAMPLES; i++)
                s[i] *= scale;

            // Fit polynomial: t = f(s)
            return FitQuartic(s, t); // 5 floats
        }

        static float ComputeSegmentLength(
            Vector3 P0,
            Vector3 P1,
            Vector3 T0,
            Vector3 T1,
            int samples = 16)
        {
            float length = 0f;
            Vector3 prev = EvalSegment(P0, P1, T0, T1, 0f);

            for (int i = 1; i <= samples; i++)
            {
                float t = i / (float)samples;
                Vector3 p = EvalSegment(P0, P1, T0, T1, t);
                length += Vector3.Distance(prev, p);
                prev = p;
            }

            return length;
        }

        static Vector3 ComputeTangent(
            IReadOnlyList<Vector3> cv,
            int i,
            bool closed,
            float tension)
        {
            int count = cv.Count;

            int i0 = i - 1;
            int i1 = i + 1;

            if (closed)
            {
                i0 = (i0 + count) % count;
                i1 = (i1 + count) % count;
            }
            else
            {
                i0 = Math.Max(i0, 0);
                i1 = Math.Min(i1, count - 1);
            }

            return (1.0f - tension) * 0.5f * (cv[i1] - cv[i0]);
        }


        static void ValidateSegPoly(HSD_Spline spline)
        {
            foreach (var v in spline.SegPolys.Array)
            {
                System.Diagnostics.Debug.WriteLine(v.Value1 + v.Value2 + v.Value3 + v.Value4 + v.Value5);
            }

            float t = 0.048788f;
            float s = 9472.271f * t * t * t * t
                    - 24099.334f * t * t * t
                    + 22302.805f * t * t
                    - 7782.5044f * t
                    + 6729.586f;

            System.Diagnostics.Debug.WriteLine("huh " + s); // This gives 1606.1036 ≈ segLength

            //var closed = false;
            //var cv = spline.Points.Select(v => new Vector3(v.X, v.Y, v.Z)).ToArray();
            //var tension = spline.Tension;
            //int segmentCount = closed ? cv.Length : cv.Length - 1;

            //float[] segLength = new float[segmentCount];
            //float[][] segPoly = new float[5][];

            //for (int k = 0; k < 5; k++)
            //    segPoly[k] = new float[segmentCount];

            //for (int i = 0; i < segmentCount; i++)
            //{
            //    int i0 = i;
            //    int i1 = (i + 1) % cv.Length;

            //    Vector3 P0 = cv[i0];
            //    Vector3 P1 = cv[i1];

            //    Vector3 T0 = ComputeTangent(cv, i0, closed, tension);
            //    Vector3 T1 = ComputeTangent(cv, i1, closed, tension);

            //    // 1️⃣ compute length
            //    float L = ComputeSegmentLength(P0, P1, T0, T1);
            //    segLength[i] = L;

            //    // 2️⃣ build segPoly (THIS is the usage)
            //    float[] poly = BuildSegPoly(
            //        t => EvalSegment(P0, P1, T0, T1, t),
            //        L
            //    );

            //    // 3️⃣ store into HSD layout
            //    for (int k = 0; k < 5; k++)
            //        segPoly[k][i] = poly[k];

            //    System.Diagnostics.Debug.WriteLine(string.Join(", ", poly));
            //}
        }

        private static HSD_AnimJoint GenerateAnimJoint(HSD_Spline spline)
        {
            List<FOBJKey> x = new();
            List<FOBJKey> y = new();
            List<FOBJKey> z = new();
            List<FOBJKey> rx = new();
            List<FOBJKey> ry = new();
            List<FOBJKey> rz = new();

            HSD_Vector3[] points = spline.Points;
            for (int i = 1; i < points.Length; i++)
            {
                Vector3 p1 = new(points[i - 1].X, points[i - 1].Y, points[i - 1].Z);
                Vector3 p2 = new(points[i].X, points[i].Y, points[i].Z);
                Vector3 direction = (p2 - p1).Normalized();
                Vector3 rotation = ConvertDirectionToEulerAngles(direction);
                rotation.Y -= (float)Math.PI / 2;

                float frame = i * 10;

                x.Add(new FOBJKey()
                {
                    Frame = frame,
                    Value = p1.X,
                    InterpolationType = GXInterpolationType.HSD_A_OP_LIN
                });
                y.Add(new FOBJKey()
                {
                    Frame = frame,
                    Value = p1.Y,
                    InterpolationType = GXInterpolationType.HSD_A_OP_LIN
                });
                z.Add(new FOBJKey()
                {
                    Frame = frame,
                    Value = p1.Z,
                    InterpolationType = GXInterpolationType.HSD_A_OP_LIN
                });
                rx.Add(new FOBJKey()
                {
                    Frame = frame,
                    Value = rotation.X,
                    InterpolationType = GXInterpolationType.HSD_A_OP_LIN
                });
                ry.Add(new FOBJKey()
                {
                    Frame = frame,
                    Value = rotation.Y,
                    InterpolationType = GXInterpolationType.HSD_A_OP_LIN
                });
                rz.Add(new FOBJKey()
                {
                    Frame = frame,
                    Value = rotation.Z,
                    InterpolationType = GXInterpolationType.HSD_A_OP_LIN
                });

            }

            // generate anim joint
            HSD_AnimJoint joint = new();
            joint.AOBJ = new HSD_AOBJ()
            {
                EndFrame = (points.Length - 1) * 10
            };

            HSD_FOBJDesc prev = null;
            foreach (Tuple<List<FOBJKey>, JointTrackType> v in new Tuple<List<FOBJKey>, JointTrackType>[]
            {
                new(x, JointTrackType.HSD_A_J_TRAX),
                new(y, JointTrackType.HSD_A_J_TRAY),
                new(z, JointTrackType.HSD_A_J_TRAZ),
                new(rx, JointTrackType.HSD_A_J_ROTX),
                new(ry, JointTrackType.HSD_A_J_ROTY),
                new(rz, JointTrackType.HSD_A_J_ROTZ),
            }
            )
            {
                HSD_FOBJDesc desc = new();
                desc.SetKeys(v.Item1, (byte)v.Item2);

                if (prev != null)
                {
                    prev.Next = desc;
                }
                else
                {
                    joint.AOBJ.FObjDesc = desc;
                }
                prev = desc;
            }

            return joint;
        }

        public static Vector3 ConvertDirectionToEulerAngles(Vector3 direction)
        {
            // Ensure the direction vector is normalized
            direction.Normalize();

            // Calculate the pitch (rotation around the X-axis)
            float pitch = (float)Math.Asin(-direction.Y);

            // Calculate the yaw (rotation around the Y-axis)
            float yaw = (float)Math.Atan2(direction.X, direction.Z);

            // Adjust the angle to be between 0 and TwoPi
            while (yaw < 0)
                yaw += MathHelper.TwoPi;

            while (yaw > MathHelper.TwoPi)
                yaw -= MathHelper.TwoPi;

            return new Vector3(pitch, yaw, 0f);
        }

        /// <summary>
        /// 
        /// </summary>
        private class SplineOBJ
        {
            public List<HSD_Vector3> Vertices = new();
            public List<Poly> Polys = new();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="filePath"></param>
            public void Open(string filePath)
            {
                using FileStream stream = new(filePath, FileMode.Open);
                using StreamReader s = new(stream);
                Poly poly = null;

                while (!s.EndOfStream)
                {
                    string[] line = s.ReadLine()?.Split(' ');

                    if (line == null || line.Length == 0)
                        continue;

                    switch (line[0])
                    {
                        case "o":
                            poly = new Poly()
                            {
                                Name = line.Length > 1 ? line[1] : string.Empty
                            };
                            Polys.Add(poly);
                            break;
                        case "v":
                            {
                                if (line.Length >= 4 &&
                                    float.TryParse(line[1], out float x) &&
                                    float.TryParse(line[2], out float y) &&
                                    float.TryParse(line[3], out float z))
                                {
                                    Vertices.Add(new HSD_Vector3(x, y, z));
                                }
                            }
                            break;
                        case "l":
                            if (poly != null)
                            {
                                PolyLine polyLine = new();
                                for (int i = 1; i < line.Length; i++)
                                {
                                    if (int.TryParse(line[i], out int index))
                                    {
                                        // Adjust indices to be zero-based
                                        polyLine.Indices.Add(index - 1);
                                    }
                                }
                                poly.Lines.Add(polyLine);
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class Poly
        {
            public string Name;
            public List<PolyLine> Lines = new();

            public IEnumerable<int> GetConnectedLine()
            {
                if (Lines.Count == 0)
                {
                    yield break;
                }

                HashSet<int> visitedIndices = new();
                Queue<int> indexQueue = new();

                // Start with the first line
                PolyLine firstLine = Lines[0];

                foreach (int index in firstLine.Indices)
                {
                    visitedIndices.Add(index);
                    indexQueue.Enqueue(index);
                    yield return index;
                }

                while (indexQueue.Count > 0)
                {
                    int currentIndex = indexQueue.Dequeue();

                    foreach (PolyLine line in Lines)
                    {
                        if (line.Indices.Contains(currentIndex))
                        {
                            foreach (int newIndex in line.Indices)
                            {
                                if (!visitedIndices.Contains(newIndex))
                                {
                                    visitedIndices.Add(newIndex);
                                    indexQueue.Enqueue(newIndex);
                                    yield return newIndex;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class PolyLine
        {
            public List<int> Indices = new();
        }
    }
}
