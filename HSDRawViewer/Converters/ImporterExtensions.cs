using HSDRaw.Common;
using HSDRaw.GX;
using IONET.Core.Model;
using IONET.Core.Skeleton;
using OpenTK;
using System;
using System.Collections.Generic;

namespace HSDRawViewer.Converters
{
    public static class ImporterExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int ComputeHash(params byte[] data)
        {
            unchecked
            {
                const int p = 16777619;
                int hash = (int)2166136261;

                for (int i = 0; i < data.Length; i++)
                    hash = (hash ^ data[i]) * p;

                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;
                return hash;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Matrix4 ToTKMatrix(this System.Numerics.Matrix4x4 t)
        {
            return new Matrix4(
                t.M11, t.M12, t.M13, t.M14,
                t.M21, t.M22, t.M23, t.M24,
                t.M31, t.M32, t.M33, t.M34,
                t.M41, t.M42, t.M43, t.M44);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static float RoundFloat(float f)
        {
            return (float)Math.Round(f, 4);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        public static void Meleeify(this IOBone root)
        {
            Dictionary<IOBone, System.Numerics.Matrix4x4> worldTransform = new Dictionary<IOBone, System.Numerics.Matrix4x4>();
            Queue<IOBone> queue = new Queue<IOBone>();

            // gather final positions
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                var bone = queue.Dequeue();

                worldTransform.Add(bone, bone.WorldTransform);

                foreach (var child in bone.Children)
                    queue.Enqueue(child);
            }

            // reset skeleton
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                var bone = queue.Dequeue();

                // reset rotation and scale
                List<string> specialBones = new List<string>() { "LShoulderJA", "RShoulderJA",
    "RHaveN", "LHaveN",
    "LLegJA", "LFootJA",
    "RLegJA", "RFootJA"};

                List<string> commonBones = new List<string>()
                {
                    "TopN", "TransN", "XRotN", "YRotN", "HipN", "WaistN", "LLegJA", "LLegJ", "LKneeJ", "LFootJA", "LFootJ", "RLegJA", "RLegJ", "RKneeJ", "RFootJA", "RFootJ", "BustN", "LShoulderN", "LShoulderJA", "LShoulderJ", "LArmJ", "LHandN", "L1stNa", "L1stNb", "L2ndNa", "L2ndNb", "L3rdNa", "L3rdNb", "L4thNa", "L4thNb", "LThumbNa", "LThumbNb", "LHandNb", "NeckN", "HeadN", "RShoulderN", "RShoulderJA", "RShoulderJ", "RArmJ", "RHandN", "R1stNa", "R1stNb", "R2ndNa", "R2ndNb", "R3rdNa", "R3rdNb", "R4thNa", "R4thNb", "RThumbNa", "RThumbNb", "RHandNb", "ThrowN", "TransN2"
                };

                Vector3[] specialBoneRotations = new Vector3[]
                {
                    new Vector3(-90, 0, 0), new Vector3(-90, 0,-180),
                    new Vector3(0, -90, -180), new Vector3(-90, -90, 270),
                    new Vector3(-90, 0, -90), new Vector3(0, 0, -90),
                    new Vector3(-90, 0, -90), new Vector3(0, 0, -90)
                };

                var boneIndex = specialBones.IndexOf(bone.Name);

                if (boneIndex != -1)
                {
                    var r = specialBoneRotations[boneIndex];
                    bone.RotationEuler = new System.Numerics.Vector3((float)(r.X * Math.PI / 180), (float)(r.Y * Math.PI / 180), (float)(r.Z * Math.PI / 180));
                }
                else
                if (commonBones.Contains(bone.Name))
                {
                    bone.Rotation = new System.Numerics.Quaternion(0, 0, 0, 1);
                }
                bone.Scale = new System.Numerics.Vector3(1, 1, 1);

                bone.Translation = new System.Numerics.Vector3(0, 0, 0);

                // calcuate new translation
                if (bone.Parent != null)
                {
                    var currentPoint = System.Numerics.Vector3.Transform(System.Numerics.Vector3.Zero, bone.WorldTransform);
                    var targetPoint = System.Numerics.Vector3.Transform(System.Numerics.Vector3.Zero, worldTransform[bone]);
                    var dis = System.Numerics.Vector3.Subtract(targetPoint, currentPoint);

                    if (System.Numerics.Matrix4x4.Invert(bone.Parent.WorldTransform, out System.Numerics.Matrix4x4 inverse))
                    {
                        bone.Translation = System.Numerics.Matrix4x4.Multiply(worldTransform[bone], inverse).Translation;

                        Console.WriteLine(bone.Name + " " + bone.Translation.ToString());
                    }
                }

                foreach (var child in bone.Children)
                    queue.Enqueue(child);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static GXWrapMode ToGXWrapMode(this WrapMode mode)
        {
            switch (mode)
            {
                case WrapMode.CLAMP:
                    return GXWrapMode.CLAMP;
                case WrapMode.MIRROR:
                    return GXWrapMode.MIRROR;
                default:
                    return GXWrapMode.REPEAT;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static byte ColorFloatToByte(float val)
        {
            return (byte)(val > 1.0f ? 255 : val * 256);
        }
    }
}
