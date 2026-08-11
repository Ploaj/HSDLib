using HSDRaw.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    public enum KdSplineKind
    {
        Linear,
        Bezier,
        BSpline,
        Tension,
    }

    public class KdSegPoly
    {
        public float E { get; set; }

        public float D { get; set; }

        public float C { get; set; }

        public float B { get; set; }

        public float A { get; set; }

        public KdSegPoly()
        {

        }

        public KdSegPoly(float a, float b, float c, float d, float e)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            E = e;
        }
    }

    public class KdSpline
    {
        public KdSplineKind Kind { get; set; }

        public float Tension { get; set; }

        [Browsable(false)]
        public float TotalLength { get; set; }

        [Browsable(false)]
        public List<KdVector> Points { get; } = new();

        [Browsable(false)]
        public List<float> SegLengths { get; set; } = new();

        [Browsable(false)]
        public List<KdSegPoly> SegPoly { get; set; } = new();

        public KdSpline() { }

        public KdSpline(HSD_Spline s)
        {
            Kind = (KdSplineKind)s.Type;
            Tension = s.Tension;
            TotalLength = s.TotalLength;
            Points = s.CV.Select(e => new KdVector(e.X, e.Y, e.Z)).ToList();
            SegLengths = s.Lengths.Array.ToList();
            if (s.SegPolys != null)
                SegPoly = s.SegPolys.Array.Select(e => new KdSegPoly(e.Value1, e.Value2, e.Value3, e.Value4, e.Value5)).ToList();
        }

        public HSD_Spline ToHsdSpline()
        {
            return new HSD_Spline()
            {
                Type = (byte)Kind,
                Tension = Tension,
                TotalLength = TotalLength,
                CV = Points.Select(e => new HSD_Vector3(e.X, e.Y, e.Z)).ToArray(),
                NumCV = (short)Points.Count,
                Lengths = new HSDRaw.HSDFloatArray() { Array = SegLengths.ToArray() },
                SegPolys = SegPoly.Count > 0 ? new HSDRaw.HSDArrayAccessor<HSD_SegPoly>()
                {
                    Array = SegPoly.Select(e => new HSD_SegPoly()
                    {
                        Value1 = e.A,
                        Value2 = e.B,
                        Value3 = e.C,
                        Value4 = e.D,
                        Value5 = e.E,
                    }).ToArray()
                } : null,
            };
        }

        /// <summary>
        /// Evaluate the spline using normalized arc length [0,1].
        /// </summary>
        public Vector3 ArcLengthPoint(double arcLength)
        {
            double parameter = ArcLengthGetParameter(arcLength);
            return GetSplinePoint(parameter);
        }


        /// <summary>
        /// Convert normalized arc length [0,1] into normalized
        /// spline parameter [0,1].
        /// </summary>
        public double ArcLengthGetParameter(double arcLength)
        {
            if (Points.Count == 0)
                return 0.0;

            if (Points.Count == 1)
                return 0.0;

            if (arcLength <= 0.0)
                return 0.0;

            if (arcLength >= 1.0)
                return 1.0;

            int segment = FindSegment(arcLength);

            if (Kind == KdSplineKind.Linear)
            {
                float start = SegLengths[segment];
                float end = SegLengths[segment + 1];

                double localT =
                    (arcLength - start) /
                    (end - start);

                return
                    (segment + localT) /
                    (Points.Count - 1);
            }

            if (SegPoly.Count <= segment)
                throw new InvalidOperationException(
                    "SegPoly has not been generated.");

            const double epsilon = 1.0e-5;

            double low = 0.0;
            double high = 1.0;
            double mid = 0.0;

            // Convert the normalized distance within this segment
            // into actual world-space distance.
            double targetLength =
                TotalLength *
                (float)(arcLength - SegLengths[segment]);

            while (true)
            {
                double oldHigh = high;

                double difference = low - oldHigh;

                if (difference < 0.0)
                    difference = -difference;

                if (difference < epsilon)
                    break;

                mid = (float)((low + oldHigh) * 0.5);

                double length = IntegrateSegPoly(
                    low,
                    mid,
                    SegPoly[segment]);

                high = mid;

                if ((float)(epsilon + length) <= targetLength)
                {
                    targetLength =
                        (float)(targetLength - length);

                    high = oldHigh;
                    low = mid;
                }
            }

            return
                (segment + mid) /
                (double)GetSegmentCount();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private static Vector3 ToVector(KdVector v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        /// <summary>
        /// Evaluate the spline at normalized parameter [0,1].
        /// </summary>
        public Vector3 GetSplinePoint(double point)
        {
            if (Points.Count == 0)
                return Vector3.Zero;

            if (Points.Count == 1)
                return ToVector(Points[0]);

            if (point <= 0.0)
                point = 0.0;

            if (point >= 1.0)
                return GetSplineEndpoint();

            int segmentCount = GetSegmentCount();

            double scaled = point * segmentCount;

            int segment = (int)scaled;

            float t = (float)(scaled - segment);

            switch (Kind)
            {
                case KdSplineKind.Linear:
                    return EvaluateLinear(segment, t);

                case KdSplineKind.Bezier:
                    return EvaluateBezier(segment, t);

                case KdSplineKind.BSpline:
                    return EvaluateBSpline(segment, t);

                case KdSplineKind.Tension:
                    return EvaluateTension(segment, t);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        /// <summary>
        /// Rebuild SegPoly, SegLengths and TotalLength from Points.
        /// </summary>
        public void RebuildArcLengthData()
        {
            BuildSegPoly();
            CalculateLengths();
        }


        /// <summary>
        /// Generate SegPoly from the actual spline equations.
        /// </summary>
        public void BuildSegPoly()
        {
            SegPoly.Clear();

            if (Kind == KdSplineKind.Linear)
                return;

            int segmentCount = GetSegmentCount();

            for (int i = 0; i < segmentCount; i++)
            {
                GetCubicPolynomial(
                    i,
                    out Vector3 c0,
                    out Vector3 c1,
                    out Vector3 c2,
                    out Vector3 c3);

                SegPoly.Add(
                    MakeSegPoly(c0, c1, c2, c3));
            }
        }


        /// <summary>
        /// Calculate TotalLength and normalized SegLengths.
        ///
        /// This deliberately does NOT use SegPoly. It evaluates the
        /// analytic spline derivative directly.
        /// </summary>
        public void CalculateLengths()
        {
            SegLengths.Clear();

            if (Points.Count < 2)
            {
                TotalLength = 0.0f;
                return;
            }

            int segmentCount = GetSegmentCount();

            // Linear splines are exact.
            if (Kind == KdSplineKind.Linear)
            {
                float total = 0.0f;

                SegLengths.Add(0.0f);

                for (int i = 0; i < segmentCount; i++)
                {
                    total += Vector3.Distance(
                        ToVector(Points[i]),
                        ToVector(Points[i + 1]));

                    SegLengths.Add(total);
                }

                TotalLength = total;

                NormalizeSegmentLengths();

                return;
            }

            float cumulative = 0.0f;

            SegLengths.Add(0.0f);

            for (int i = 0; i < segmentCount; i++)
            {
                double length =
                    IntegrateSegmentLength(i);

                cumulative += (float)length;

                SegLengths.Add(cumulative);
            }

            TotalLength = cumulative;

            NormalizeSegmentLengths();
        }

        // ============================================================
        // Segment evaluation
        // ============================================================

        private Vector3 EvaluateLinear(
            int segment,
            float t)
        {
            Vector3 p0 = ToVector(Points[segment]);
            Vector3 p1 = ToVector(Points[segment + 1]);

            return Vector3.Lerp(p0, p1, t);
        }


        private Vector3 EvaluateBezier(
            int segment,
            float t)
        {
            int i = segment * 3;

            Vector3 p0 = ToVector(Points[i + 0]);
            Vector3 p1 = ToVector(Points[i + 1]);
            Vector3 p2 = ToVector(Points[i + 2]);
            Vector3 p3 = ToVector(Points[i + 3]);

            float omt = 1.0f - t;

            float t2 = t * t;
            float t3 = t2 * t;

            float omt2 = omt * omt;
            float omt3 = omt2 * omt;

            float b0 = omt3;
            float b1 = 3.0f * t * omt2;
            float b2 = 3.0f * t2 * omt;
            float b3 = t3;

            return 
                (p0 * b0) +
                (p1 * b1) +
                (p2 * b2) +
                (p3 * b3);
        }


        private Vector3 EvaluateBSpline(
            int segment,
            float t)
        {
            int i = segment;

            Vector3 p0 = ToVector(Points[i + 0]);
            Vector3 p1 = ToVector(Points[i + 1]);
            Vector3 p2 = ToVector(Points[i + 2]);
            Vector3 p3 = ToVector(Points[i + 3]);

            float t2 = t * t;
            float t3 = t2 * t;

            float omt = 1.0f - t;
            float omt2 = omt * omt;
            float omt3 = omt2 * omt;

            float b0 = omt3 / 6.0f;

            float b1 =
                (3.0f * t3 -
                 6.0f * t2 +
                 4.0f) / 6.0f;

            float b2 =
                (-3.0f * t3 +
                 3.0f * t2 +
                 3.0f * t +
                 1.0f) / 6.0f;

            float b3 = t3 / 6.0f;

            return
                (p0 * b0) +
                (p1 * b1) +
                (p2 * b2) +
                (p3 * b3);
        }


        private Vector3 EvaluateTension(
            int segment,
            float t)
        {
            int i = segment;

            Vector3 p0 = ToVector(Points[i + 0]);
            Vector3 p1 = ToVector(Points[i + 1]);
            Vector3 p2 = ToVector(Points[i + 2]);
            Vector3 p3 = ToVector(Points[i + 3]);

            float t2 = t * t;
            float t3 = t2 * t;

            float s = Tension;

            /*
             * These come directly from the decompiled code:
             *
             * fVar5 = 1
             *       + (2-s)t^3
             *       + (s-3)t^2
             *
             * fVar1 = s(2t^2 - t^3 - t)
             *
             * fVar2 = s(t^3 - t^2)
             *
             * fVar3 = st
             *       + (s-2)t^3
             *       - (2s-3)t^2
             *
             * Final:
             *
             * p3 * fVar2
             * + p2 * fVar3
             * + p0 * fVar1
             * + p1 * fVar5
             */

            float f0 =
                1.0f
                + (2.0f - s) * t3
                + (s - 3.0f) * t2;

            float f1 =
                s * (2.0f * t2 - t3 - t);

            float f2 =
                s * (t3 - t2);

            float f3 =
                s * t
                + (s - 2.0f) * t3
                - (2.0f * s - 3.0f) * t2;

            return 
                (p0 * f1) +
                (p1 * f0) +
                (p2 * f3) +
                (p3 * f2);
        }


        // ============================================================
        // Derivatives
        // ============================================================

        private Vector3 EvaluateDerivative(
            int segment,
            float t)
        {
            switch (Kind)
            {
                case KdSplineKind.Linear:
                    return ToVector(Points[segment + 1]) - ToVector(Points[segment]);

                case KdSplineKind.Bezier:
                    return EvaluateBezierDerivative(
                        segment,
                        t);

                case KdSplineKind.BSpline:
                    return EvaluateBSplineDerivative(
                        segment,
                        t);

                case KdSplineKind.Tension:
                    return EvaluateTensionDerivative(
                        segment,
                        t);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private Vector3 EvaluateBezierDerivative(
            int segment,
            float t)
        {
            int i = segment * 3;

            Vector3 p0 = ToVector(Points[i + 0]);
            Vector3 p1 = ToVector(Points[i + 1]);
            Vector3 p2 = ToVector(Points[i + 2]);
            Vector3 p3 = ToVector(Points[i + 3]);

            float omt = 1.0f - t;

            float b0 = 3.0f * omt * omt;
            float b1 = 6.0f * omt * t;
            float b2 = 3.0f * t * t;

            return ((p1 - p0) * b0) + ((p2 - p1) * b1) + ((p3 - p2) * b2);
        }


        private Vector3 EvaluateBSplineDerivative(
            int segment,
            float t)
        {
            int i = segment;

            Vector3 p0 = ToVector(Points[i + 0]);
            Vector3 p1 = ToVector(Points[i + 1]);
            Vector3 p2 = ToVector(Points[i + 2]);
            Vector3 p3 = ToVector(Points[i + 3]);

            float t2 = t * t;

            // Derivatives of:
            //
            // B0 = (1-t)^3 / 6
            // B1 = (3t^3 - 6t^2 + 4) / 6
            // B2 = (-3t^3 + 3t^2 + 3t + 1) / 6
            // B3 = t^3 / 6

            float b0 =
                -0.5f * (1.0f - t) * (1.0f - t);

            float b1 =
                1.5f * t2 - 2.0f * t;

            float b2 =
                -1.5f * t2 + t + 0.5f;

            float b3 =
                0.5f * t2;

            return 
                (p0 * b0) +
                (p1 * b1) +
                (p2 * b2) +
                (p3 * b3);
        }


        private Vector3 EvaluateTensionDerivative(
            int segment,
            float t)
        {
            int i = segment;

            Vector3 p0 = ToVector(Points[i + 0]);
            Vector3 p1 = ToVector(Points[i + 1]);
            Vector3 p2 = ToVector(Points[i + 2]);
            Vector3 p3 = ToVector(Points[i + 3]);

            float t2 = t * t;

            float s = Tension;

            /*
             * Derivatives of the exact type-3 basis functions.
             */

            // f0 =
            // 1 + (2-s)t^3 + (s-3)t^2

            float f0 =
                3.0f * (2.0f - s) * t2
                + 2.0f * (s - 3.0f) * t;

            // f1 =
            // s(2t^2 - t^3 - t)

            float f1 =
                s * (4.0f * t
                     - 3.0f * t2
                     - 1.0f);

            // f2 =
            // s(t^3 - t^2)

            float f2 =
                s * (3.0f * t2
                     - 2.0f * t);

            // f3 =
            // st + (s-2)t^3 - (2s-3)t^2

            float f3 =
                s
                + 3.0f * (s - 2.0f) * t2
                - 2.0f * (2.0f * s - 3.0f) * t;

            return 
                (p0 * f1) +
                (p1 * f0) +
                (p2 * f3) +
                (p3 * f2);
        }


        // ============================================================
        // Cubic polynomial representation
        // ============================================================

        /// <summary>
        /// Gets:
        ///
        /// P(t) = C0 + C1*t + C2*t² + C3*t³
        ///
        /// for the current segment.
        /// </summary>
        private void GetCubicPolynomial(
            int segment,
            out Vector3 c0,
            out Vector3 c1,
            out Vector3 c2,
            out Vector3 c3)
        {
            switch (Kind)
            {
                case KdSplineKind.Bezier:
                    GetBezierPolynomial(
                        segment,
                        out c0,
                        out c1,
                        out c2,
                        out c3);
                    return;

                case KdSplineKind.BSpline:
                    GetBSplinePolynomial(
                        segment,
                        out c0,
                        out c1,
                        out c2,
                        out c3);
                    return;

                case KdSplineKind.Tension:
                    GetTensionPolynomial(
                        segment,
                        out c0,
                        out c1,
                        out c2,
                        out c3);
                    return;

                default:
                    throw new InvalidOperationException(
                        "Linear splines do not have SegPoly.");
            }
        }


        private void GetBezierPolynomial(
            int segment,
            out Vector3 c0,
            out Vector3 c1,
            out Vector3 c2,
            out Vector3 c3)
        {
            int i = segment * 3;

            Vector3 p0 = ToVector(Points[i + 0]);
            Vector3 p1 = ToVector(Points[i + 1]);
            Vector3 p2 = ToVector(Points[i + 2]);
            Vector3 p3 = ToVector(Points[i + 3]);

            c0 = p0;
            c1 = 3.0f * (p1 - p0);
            c2 = 3.0f * (p0 - 2.0f * p1 + p2);
            c3 = -p0 + 3.0f * p1 - 3.0f * p2 + p3;
        }


        private void GetBSplinePolynomial(
            int segment,
            out Vector3 c0,
            out Vector3 c1,
            out Vector3 c2,
            out Vector3 c3)
        {
            int i = segment;

            Vector3 p0 = ToVector(Points[i + 0]);
            Vector3 p1 = ToVector(Points[i + 1]);
            Vector3 p2 = ToVector(Points[i + 2]);
            Vector3 p3 = ToVector(Points[i + 3]);

            const float inv6 = 1.0f / 6.0f;

            c0 = (p0 + 4.0f * p1 + p2 + p3) * inv6;

            c1 = (-3.0f * p0 + 3.0f * p2) * inv6;

            c2 = (3.0f * p0 - 6.0f * p1 + 3.0f * p2) * inv6;

            c3 = (-p0 + 3.0f * p1 - 3.0f * p2 + p3) * inv6;
        }


        private void GetTensionPolynomial(
            int segment,
            out Vector3 c0,
            out Vector3 c1,
            out Vector3 c2,
            out Vector3 c3)
        {
            int i = segment;

            Vector3 p0 = ToVector(Points[i + 0]);
            Vector3 p1 = ToVector(Points[i + 1]);
            Vector3 p2 = ToVector(Points[i + 2]);
            Vector3 p3 = ToVector(Points[i + 3]);

            float s = Tension;

            c0 = p1;

            c1 = s * (p2 - p0);

            c2 =
                2.0f * s * p0 +
                (s - 3.0f) * p1 -
                (2.0f * s - 3.0f) * p2 -
                s * p3;

            c3 =
                -s * p0 +
                (2.0f - s) * p1 +
                (s - 2.0f) * p2 +
                s * p3;
        }


        /// <summary>
        /// Converts the cubic polynomial into the quartic speed²
        /// polynomial used by the game.
        /// </summary>
        private static KdSegPoly MakeSegPoly(
            Vector3 c0,
            Vector3 c1,
            Vector3 c2,
            Vector3 c3)
        {
            // P(t)  = c0 + c1*t + c2*t² + c3*t³
            // P'(t) = c1 + 2*c2*t + 3*c3*t²

            Vector3 d0 = c1;
            Vector3 d1 = 2.0f * c2;
            Vector3 d2 = 3.0f * c3;

            return new KdSegPoly
            {
                A = Vector3.Dot(d2, d2),
                B = 2.0f * Vector3.Dot(d1, d2),
                C = Vector3.Dot(d1, d1) + 2.0f * Vector3.Dot(d0, d2),
                D = 2.0f * Vector3.Dot(d0, d1),
                E = Vector3.Dot(d0, d0)
            };
        }


        // ============================================================
        // Length calculation
        // ============================================================

        private double IntegrateSegmentLength(
            int segment)
        {
            const int intervals = 8;

            double h = 1.0 / intervals;

            double sum = 0.0;

            for (int i = 1; i < intervals; i++)
            {
                double t = i * h;

                double speed =
                    EvaluateDerivative(
                        segment,
                        (float)t).Length();

                sum += speed *
                       ((i & 1) != 0
                           ? 4.0
                           : 2.0);
            }

            double start =
                EvaluateDerivative(
                    segment,
                    0.0f).Length();

            double end =
                EvaluateDerivative(
                    segment,
                    1.0f).Length();

            return
                h / 3.0 *
                (start + sum + end);
        }


        private static double IntegrateSegPoly(
            double a,
            double b,
            KdSegPoly poly)
        {
            double h =
                (b - a) * 0.125;

            double t =
                a + h;

            double sum = 0.0;

            bool even = false;

            for (int i = 0; i < 7; i++)
            {
                t = (float)t;

                double value =
                    EvaluateSegPoly(poly, t);

                double speed =
                    Math.Sqrt(value);

                // Original alternates 4,2,4,2...
                speed *= even ? 2.0 : 4.0;

                sum = (float)(sum + speed);

                t += h;

                even = !even;
            }

            double start =
                Math.Sqrt(
                    EvaluateSegPoly(
                        poly,
                        a));

            double end =
                Math.Sqrt(
                    EvaluateSegPoly(
                        poly,
                        b));

            return
                (float)(
                    h *
                    (float)(
                        sum +
                        start +
                        end) /
                    3.0);
        }


        private static double EvaluateSegPoly(
            KdSegPoly poly,
            double t)
        {
            float tf = (float)t;

            float t2 = tf * tf;
            float t3 = t2 * tf;
            float t4 = t3 * tf;

            double value =
                poly.A * t4 +
                poly.B * t3 +
                poly.C * t2 +
                poly.D * tf +
                poly.E;

            return value;
        }


        // ============================================================
        // Helpers
        // ============================================================

        private int GetSegmentCount()
        {
            switch (Kind)
            {
                case KdSplineKind.Linear:
                    return Points.Count - 1;

                case KdSplineKind.Bezier:
                    return (Points.Count - 1) / 3;

                case KdSplineKind.BSpline:
                case KdSplineKind.Tension:
                    return Points.Count - 3;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private int FindSegment(double arcLength)
        {
            int count = SegLengths.Count - 1;

            if (count <= 0)
                return 0;

            int segment = 0;

            while (segment + 1 < SegLengths.Count &&
                   SegLengths[segment + 1] < arcLength)
            {
                segment++;
            }

            if (segment >= count)
                segment = count - 1;

            return segment;
        }


        private Vector3 GetSplineEndpoint()
        {
            int segmentCount = GetSegmentCount();

            switch (Kind)
            {
                case KdSplineKind.Linear:
                    return ToVector(Points[Points.Count - 1]);

                case KdSplineKind.Bezier:
                    {
                        int i = segmentCount * 3 + 3;
                        i = Math.Min(Points.Count - 1, i);
                        return ToVector(Points[i]);
                    }

                case KdSplineKind.BSpline:
                case KdSplineKind.Tension:
                    {
                        int i = segmentCount + 1;
                        i = Math.Min(Points.Count - 1, i);
                        return ToVector(Points[i]);
                    }

                default:
                    return ToVector(Points[Points.Count - 1]);
            }
        }


        private void NormalizeSegmentLengths()
        {
            if (TotalLength <= 0.0f)
                return;

            for (int i = 0; i < SegLengths.Count; i++)
                SegLengths[i] /= TotalLength;
        }

        internal void Validate()
        {
            var before = new List<KdSegPoly>();
            before.AddRange(SegPoly);
            RebuildArcLengthData();

            for (int i = 0; i < before.Count; i++)
            {
                Debug.WriteLine($"{before[i].A} - {SegPoly[i].A}");
            }
        }
    }
}
