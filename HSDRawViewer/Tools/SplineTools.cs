using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using HSDRawViewer.IO.AirRide.DataFormat;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace HSDRawViewer.Tools
{
    public static class SplineTools
    {
        public static HSD_AnimJoint GenerateAnimJoint(
            KdSpline spline,
            int frameCount)
        {
            if (spline == null)
                throw new ArgumentNullException(nameof(spline));

            if (frameCount < 2)
                throw new ArgumentException(
                    "Frame count must be at least 2.",
                    nameof(frameCount));

            spline.RebuildArcLengthData();

            List<FOBJKey> x = new();
            List<FOBJKey> y = new();
            List<FOBJKey> z = new();
            List<FOBJKey> rx = new();
            List<FOBJKey> ry = new();
            List<FOBJKey> rz = new();

            Vector3 previousRotation = Vector3.Zero;

            for (int frame = 0; frame < frameCount; frame++)
            {
                // Normalized distance along the spline.
                float arcLength = frame / (float)(frameCount - 1);

                Vector3 position = spline.ArcLengthPoint(arcLength).ToTKVector();

                // Sample the tangent from the spline.
                const float tangentOffset = 0.001f;

                float before = MathF.Max(0.0f, arcLength - tangentOffset);
                float after = MathF.Min(1.0f, arcLength + tangentOffset);

                Vector3 p0 = spline.ArcLengthPoint(before).ToTKVector();
                Vector3 p1 = spline.ArcLengthPoint(after).ToTKVector();

                Vector3 direction = p1 - p0;

                if (direction.LengthSquared > 0.000001f)
                    direction = Vector3.Normalize(direction);
                else
                    direction = Vector3.UnitZ;

                Vector3 rotation =
                    ConvertDirectionToEulerAngles(direction);

                rotation.Y -= MathF.PI / 2.0f;

                // Prevent Euler angle discontinuities.
                rotation.X = UnwrapAngle(
                    rotation.X,
                    previousRotation.X);

                rotation.Y = UnwrapAngle(
                    rotation.Y,
                    previousRotation.Y);

                rotation.Z = UnwrapAngle(
                    rotation.Z,
                    previousRotation.Z);

                previousRotation = rotation;

                x.Add(new FOBJKey()
                {
                    Frame = frame,
                    Value = position.X,
                    InterpolationType = GXInterpolationType.HSD_A_OP_LIN
                });

                y.Add(new FOBJKey()
                {
                    Frame = frame,
                    Value = position.Y,
                    InterpolationType = GXInterpolationType.HSD_A_OP_LIN
                });

                z.Add(new FOBJKey()
                {
                    Frame = frame,
                    Value = position.Z,
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

            HSD_AnimJoint joint = new();

            joint.AOBJ = new HSD_AOBJ()
            {
                EndFrame = frameCount - 1
            };

            HSD_FOBJDesc prev = null;

            foreach (var v in new[]
            {
        (x,  JointTrackType.HSD_A_J_TRAX),
        (y,  JointTrackType.HSD_A_J_TRAY),
        (z,  JointTrackType.HSD_A_J_TRAZ),
        (rx, JointTrackType.HSD_A_J_ROTX),
        (ry, JointTrackType.HSD_A_J_ROTY),
        (rz, JointTrackType.HSD_A_J_ROTZ),
    })
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

        private static float UnwrapAngle(float angle, float previous)
        {
            while (angle - previous > MathF.PI)
                angle -= MathF.Tau;

            while (angle - previous < -MathF.PI)
                angle += MathF.Tau;

            return angle;
        }

        private static Vector3 ConvertDirectionToEulerAngles(Vector3 direction)
        {
            direction = Vector3.Normalize(direction);

            float pitch = MathF.Asin(-direction.Y);
            float yaw = MathF.Atan2(direction.X, direction.Z);

            return new Vector3(pitch, yaw, 0);
        }

        public static HSD_AnimJoint GenerateAnimJoint(HSD_Spline spline)
        {
            List<FOBJKey> x = new();
            List<FOBJKey> y = new();
            List<FOBJKey> z = new();
            List<FOBJKey> rx = new();
            List<FOBJKey> ry = new();
            List<FOBJKey> rz = new();

            HSD_Vector3[] points = spline.CV;
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
    }
}
