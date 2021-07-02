using HSDRaw.Tools;
using HSDRawViewer.Rendering.Models;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDRawViewer.Rendering.Renderers
{
    public class AfterImageDesc
    {
        public int Bone;
        public Vector3 Color1;
        public Vector3 Color2;
        public int Alpha1;
        public int Alpha2;
        public float Bottom;
        public float Top;
    }
    public class AfterImageRenderer
    {
        private class AfterImageKey
        {
            public Vector3 pos;
            public Quaternion rot;

            public Vector3 Transform(Vector3 input)
            {
                return pos + Vector3.TransformNormal(input, Matrix4.CreateFromQuaternion(rot));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="after_image"></param>
        public static void RenderAfterImage(JOBJManager manager, float frame, AfterImageDesc after_image)
        {
            if (manager.Animation == null)
                return;

            // process previous keys
            AfterImageKey[] keys = new AfterImageKey[3];

            for (int i = 0; i < keys.Length; i++)
            {
                manager.Frame = frame - i;
                manager.UpdateNoRender();

                var bone = manager.GetWorldTransform(after_image.Bone);

                keys[i] = new AfterImageKey()
                {
                    pos = bone.ExtractTranslation(),
                    rot = bone.ExtractRotation()
                };
            }

            //// render sword trail
            GL.PushAttrib(AttribMask.AllAttribBits);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // draw strips interpolated
            GL.Begin(PrimitiveType.TriangleStrip);

            var top = new Vector3(after_image.Top, 0, 0);
            var bot = new Vector3(after_image.Bottom, 0, 0);

            int slices = 10;
            float total_indices = (keys.Length - 1) * slices;
            float index = total_indices;
            for (int s = 0; s < keys.Length - 1; s++)
            {
                var key1 = keys[s];
                var key2 = keys[s + 1];

                float length = (keys[s].pos - keys[s + 1].pos).Length;

                for (float i = 0; i < length; i += length / slices)
                {
                    float alpha = index / total_indices;
                    index--;

                    float blend = i / length;

                    // lerp angle
                    var rot = Quaternion.Slerp(key1.rot, key2.rot, blend);
                    var pos = Vector3.Lerp(key1.pos, key2.pos, blend);
                    var mat = Matrix4.CreateFromQuaternion(rot) * Matrix4.CreateTranslation(pos);

                    var start_top = Vector3.TransformPosition(top, mat);
                    var start_bot = Vector3.TransformPosition(bot, mat);

                    GL.Color4(after_image.Color1.X, after_image.Color1.Y, after_image.Color1.Z, alpha);
                    GL.Vertex3(start_top);

                    GL.Color4(after_image.Color2.X, after_image.Color2.Y, after_image.Color2.Z, alpha);
                    GL.Vertex3(start_bot);
                }
            }

            GL.End();

            GL.PopAttrib();





            // calculate point positions
            //var top = new Vector3(after_image.Top, 0, 0);
            //var bot = new Vector3(after_image.Bottom, 0, 0);

            //Vector3[] points_top = new Vector3[keys.Length];
            //Vector3[] points_bottom = new Vector3[keys.Length];
            //for (int i = 0; i < keys.Length; i++)
            //{
            //    points_top[i] = keys[i].Transform(top);
            //    points_bottom[i] = keys[i].Transform(bot);
            //}

            //float term1 = 1 / (keys[0].pos - keys[1].pos).Length;
            //float term2 = 1 / (keys[1].pos - keys[2].pos).Length;
            //Vector3 tan_bottom = 
            //    new Vector3(
            //        CalculateTangent(term1, term2, points_bottom[0].X, points_bottom[1].X, points_bottom[2].X),
            //        CalculateTangent(term1, term2, points_bottom[0].Y, points_bottom[1].Y, points_bottom[2].Y),
            //        CalculateTangent(term1, term2, points_bottom[0].Z, points_bottom[1].Z, points_bottom[2].Z));
            //Vector3 tan_top =
            //    new Vector3(
            //        CalculateTangent(term1, term2, points_top[0].X, points_top[1].X, points_top[2].X),
            //        CalculateTangent(term1, term2, points_top[0].Y, points_top[1].Y, points_top[2].Y),
            //        CalculateTangent(term1, term2, points_top[0].Z, points_top[1].Z, points_top[2].Z));


            //// render sword trail
            //GL.PushAttrib(AttribMask.AllAttribBits);
            //GL.Disable(EnableCap.CullFace);
            //GL.Disable(EnableCap.DepthTest);
            //GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            //// draw strips interpolated
            //GL.Begin(PrimitiveType.TriangleStrip);

            //int slices = 10;
            //float total_indices = (keys.Length - 1) * slices;
            //float index = total_indices;
            //for (int s = 0; s < keys.Length - 1; s++)
            //{
            //    float length = (keys[s].pos - keys[s + 1].pos).Length;

            //    for (float i = 0; i < length; i += length / slices)
            //    {
            //        float alpha = index / total_indices;

            //        GL.Color4(after_image.Color1.X, after_image.Color1.Y, after_image.Color1.Z, alpha);
            //        GL.Vertex3(Herp(1 / length, i, points_top[s], points_top[s + 1], tan_top));

            //        GL.Color4(after_image.Color2.X, after_image.Color2.Y, after_image.Color2.Z, alpha);
            //        GL.Vertex3(Herp(1 / length, i, points_bottom[s], points_bottom[s + 1], tan_bottom));

            //        index--;
            //    }
            //    tan_bottom *= -1;
            //    tan_top *= -1;
            //}

            ///*
            //GL.Color3(after_image.Color1);
            //GL.Vertex3(keys[0].Transform(top));
            //GL.Color3(after_image.Color2);
            //GL.Vertex3(keys[0].Transform(bot));

            //GL.Color3(after_image.Color1);
            //GL.Vertex3(keys[1].Transform(top));
            //GL.Color3(after_image.Color2);
            //GL.Vertex3(keys[1].Transform(bot));

            //GL.Color3(after_image.Color1);
            //GL.Vertex3(keys[2].Transform(top));
            //GL.Color3(after_image.Color2);
            //GL.Vertex3(keys[2].Transform(bot));
            //*/

            //GL.End();

            //GL.PopAttrib();

        }

        private static Vector3 Herp(float term, float time, Vector3 p1, Vector3 p2, Vector3 tan)
        {
            return new Vector3()
            {
                X = AnimationInterpolationHelper.SplineGetHermite(term, time, p1.X, p2.X, -tan.X, tan.X),
                Y = AnimationInterpolationHelper.SplineGetHermite(term, time, p1.Y, p2.Y, -tan.Y, tan.Y),
                Z = AnimationInterpolationHelper.SplineGetHermite(term, time, p1.Z, p2.Z, -tan.Z, tan.Z),
            };
        }


        /// <summary>
        /// 
        /// </summary>
        private static float CalculateTangent(float term, float term2, float prev, float current, float next)
        {
            float Tan = 0;

            Tan += (current - prev) * term;
            Tan += (next - current) * term2;

            Tan /= 2;

            return Tan;
        }

    }
}
