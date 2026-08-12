using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Rendering;
using IONET.Collada.Core.Transform;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;

namespace HSDRawViewer.GUI.Plugins.GrTool.Render
{
    public class GrSplineCache
    {
        private Vector3[] buffer = Array.Empty<Vector3>();

        private int sampleCount;

        public GrSplineCache() { }


        public void BuildCache(KdSpline spline, int sample_size = 256)
        {
            if (spline.Kind == KdSplineKind.Linear)
            {
                sampleCount = spline.Points.Count;
                buffer = new Vector3[spline.Points.Count];

                if (spline.SegLengths.Count < spline.Points.Count)
                    spline.RebuildArcLengthData();

                for (int i = 0; i < spline.Points.Count; i++)
                {
                    buffer[i] = spline.ArcLengthPoint(spline.SegLengths[i]).ToTKVector();
                }
            }
            else
            {
                sampleCount = sample_size;
                buffer = new Vector3[sample_size];

                for (int i = 0; i < sample_size; i++)
                {
                    double t =
                        (double)i /
                        (sample_size - 1);

                    buffer[i] = spline.ArcLengthPoint(t).ToTKVector();
                }
            }
        }

        public void DrawSplinePoints(
            KdSpline spline,
            object selected_object,
            Vector4 color,
            Vector4 selectedColor,
            float scale)
        {

            GL.PointSize(4.0f * scale);

            GL.Begin(PrimitiveType.Points);

            foreach (var p in spline.Points)
            {
                if (selected_object == p)
                    GL.Color4(selectedColor);
                else
                    GL.Color4(color);
                GL.Vertex3(p.X, p.Y, p.Z);
            }

            GL.End();
        }

        public void DrawSpline(
            KdSpline spline,
            double start,
            double end,
            Vector4 color,
            float scale)
        {
            if (start < 0.0 ||
                end > 1.0 ||
                start > end)
                throw new ArgumentOutOfRangeException();

            // --------------------------------------------------------
            // Find start sample.
            // --------------------------------------------------------

            Vector3 startPoint = spline.ArcLengthPoint(start).ToTKVector();

            int startIndex = 0;

            if (start != 0.0)
            {
                for (int i = 0; i < sampleCount; i++)
                {
                    double samplePosition =
                        spline.Kind == KdSplineKind.Linear
                            ? spline.SegLengths[i]
                            : i / 255.0;

                    if (start <= samplePosition)
                        break;

                    startIndex++;
                }

                if (startIndex >= sampleCount)
                    startIndex = sampleCount - 1;
            }


            // --------------------------------------------------------
            // Find end sample.
            // --------------------------------------------------------

            Vector3 endPoint = spline.ArcLengthPoint(end).ToTKVector();

            int endIndex = sampleCount - 1;

            if (end != 1.0)
            {
                endIndex = sampleCount - 1;

                while (endIndex >= 0)
                {
                    double samplePosition =
                        spline.Kind == KdSplineKind.Linear
                            ? spline.SegLengths[endIndex]
                            : endIndex / 255.0;

                    if (samplePosition <= end)
                        break;

                    endIndex--;
                }

                if (endIndex < 0)
                    endIndex = 0;
            }


            GL.PushAttrib(AttribMask.AllAttribBits);
            GL.DepthMask(false);

            // --------------------------------------------------------
            // Draw start point.
            // --------------------------------------------------------

            GL.Color4(color);

            GL.PointSize(4.0f * scale);

            GL.Begin(PrimitiveType.Points);
            GL.Vertex3(startPoint);
            GL.End();


            // --------------------------------------------------------
            // Draw end point.
            // --------------------------------------------------------

            GL.PointSize(8.0f * scale);

            GL.Begin(PrimitiveType.Points);
            GL.Vertex3(endPoint);
            GL.End();


            // --------------------------------------------------------
            // Draw cached spline samples.
            // --------------------------------------------------------

            if (endIndex < startIndex)
                return;

            GL.LineWidth(2.0f * scale);
            GL.Begin(PrimitiveType.Lines);

            GL.Vertex3(startPoint);
            GL.Vertex3(buffer[startIndex]);

            for (int i = startIndex; i < endIndex; i++)
            {
                GL.Vertex3(buffer[i]);
                GL.Vertex3(buffer[i + 1]);
            }

            GL.Vertex3(buffer[endIndex]);
            GL.Vertex3(endPoint);

            GL.End();

            GL.PopAttrib();
        }

        public bool TryPickLines(PickInformation pick, out float distance)
        {
            distance = float.PositiveInfinity;
            bool picked = false;
            for (int i = 0; i < buffer.Length - 1; i++)
            {
                if (pick.CheckScreenLine(buffer[i], buffer[i + 1], 3.0f, out float d))
                {
                    if (d < distance)
                    {
                        distance = d;
                        picked = true;
                    }
                }
            }
            return picked;
        }
    }
}
