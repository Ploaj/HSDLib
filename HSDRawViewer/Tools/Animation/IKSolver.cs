using OpenTK.Mathematics;
using System;

namespace HSDRawViewer.Tools.Animation
{
    public class IKSolver
    {
        public struct Result
        {
            public Quaternion LegWorldRotation;    // Upper leg world transform
            public Quaternion KneeWorldRotation;   // Knee world transform
            public Vector3 TargetKneePos; // Debugging
            public Vector3 PlaneNormal;   // Debugging
        }

        /// <summary>
        /// 
        /// </summary>
        public static Vector3 CalculateBindNormal(Matrix4 legBindWorld, Matrix4 kneeBindWorld, Matrix4 footBindWorld)
        {
            Vector3 legDirBind = (kneeBindWorld.ExtractTranslation() - legBindWorld.ExtractTranslation());
            Vector3 footForwardBind = Vector3.Transform(Vector3.UnitX, footBindWorld.ExtractRotation()).Normalized();
            return Vector3.Normalize(Vector3.Cross(legDirBind, footForwardBind));
        }

        /// <summary>
        /// Solves a 2-bone leg IK chain in world space.
        /// </summary>
        /// <param name="legWorld">Current leg/hip world transform</param>
        /// <param name="kneeWorld">Current knee world transform</param>
        /// <param name="footWorld">Current foot world transform</param>
        /// <param name="targetWorldPos">Desired foot position in world space</param>
        public static Result Solve3BoneWorld(
            Matrix4 legWorld,
            Matrix4 kneeWorld,
            Matrix4 footWorld,
            Vector3 targetWorldPos,
            Vector3 BindNormal
        )
        {
            Vector3 hipPos = legWorld.ExtractTranslation();
            Vector3 kneePos = kneeWorld.ExtractTranslation();
            Vector3 footPos = footWorld.ExtractTranslation();

            float upperLen = (kneePos - hipPos).Length;
            float lowerLen = (footPos - kneePos).Length;

            // --- Direction to target ---
            Vector3 toTarget = targetWorldPos - hipPos;
            float dist = toTarget.Length;

            float maxReach = upperLen + lowerLen - 1e-5f;
            float minReach = MathF.Abs(upperLen - lowerLen) + 1e-5f;
            dist = MathHelper.Clamp(dist, minReach, maxReach);

            Vector3 dir = toTarget.Normalized();

            // --- Extract Rotations ---
            Quaternion legOrigWorldRot = legWorld.ExtractRotation();
            Quaternion kneeOrigWorldRot = kneeWorld.ExtractRotation();

            // --- Knee bend plane ---
            Vector3 planeNormal = Vector3.Transform(BindNormal, legOrigWorldRot).Normalized();

            // --- Law of cosines to find hip bend angle ---
            float cosHip = (upperLen * upperLen + dist * dist - lowerLen * lowerLen) / (2f * upperLen * dist);
            cosHip = MathHelper.Clamp(cosHip, -1f, 1f);
            float hipAngle = MathF.Acos(cosHip);

            // --- Compute desired knee position ---
            Vector3 bendAxis = Vector3.Cross(dir, planeNormal).Normalized();
            Quaternion hipSwing = Quaternion.FromAxisAngle(bendAxis, hipAngle);
            Vector3 desiredKneePos = hipPos + Vector3.Transform(dir * upperLen, hipSwing);

            // --- Compute world-space rotations ---
            Vector3 upperDir = (desiredKneePos - hipPos).Normalized();
            Vector3 lowerDir = (targetWorldPos - desiredKneePos).Normalized();

            // --- Compute final worldspace swing rotations ---

            Vector3 legForwardWorld =
                Vector3.Transform(Vector3.UnitX, legOrigWorldRot).Normalized();
            Vector3 kneeForwardWorld =
                Vector3.Transform(Vector3.UnitX, kneeOrigWorldRot).Normalized();

            Quaternion legSwing =
                FromToRotation(legForwardWorld, upperDir);
            Quaternion kneeSwing =
                FromToRotation(kneeForwardWorld, lowerDir);

            Quaternion legWorldRot = legSwing * legOrigWorldRot;
            Quaternion kneeWorldRot = kneeSwing * kneeOrigWorldRot;

            // --- Return final result ---

            return new Result
            {
                LegWorldRotation = legWorldRot,
                KneeWorldRotation = kneeWorldRot,
                TargetKneePos = desiredKneePos,
                PlaneNormal = planeNormal
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Quaternion FromToRotation(Vector3 from, Vector3 to)
        {
            from = from.Normalized();
            to = to.Normalized();

            float dot = Vector3.Dot(from, to);

            // Same direction
            if (dot > 0.999999f)
                return Quaternion.Identity;

            // Opposite direction (180°)
            if (dot < -0.999999f)
            {
                // Find any perpendicular axis
                Vector3 axis = Vector3.Cross(from, Vector3.UnitX);
                if (axis.LengthSquared < 1e-6f)
                    axis = Vector3.Cross(from, Vector3.UnitY);

                axis.Normalize();
                return Quaternion.FromAxisAngle(axis, MathF.PI);
            }

            Vector3 axisCross = Vector3.Cross(from, to);
            float angle = MathF.Acos(dot);

            return Quaternion.FromAxisAngle(axisCross.Normalized(), angle);
        }

    }
}
