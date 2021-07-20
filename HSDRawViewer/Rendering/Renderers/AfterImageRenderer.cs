using HSDRawViewer.Rendering.Models;
using OpenTK;
using OpenTK.Graphics.OpenGL;

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

        }

    }
}
