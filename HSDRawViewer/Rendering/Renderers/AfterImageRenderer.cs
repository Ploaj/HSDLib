using HSDRawViewer.Rendering.Models;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace HSDRawViewer.Rendering.Renderers
{
    public class AfterImageDesc
    {
        public float x0;
        public float x4;
        public Vector3 InCol;
        public Vector3 OutCol;
        public byte AlphaStart;
        public byte AlphaEnd;
        public int Bone;
        public float Bottom;
        public float Top;
    }
    public class AfterImageKey
    {
        public Vector3 pos;
        public Vector3 rot;
    }

    public class AfterImageRenderer
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="after_image"></param>
        public static void RenderAfterImage(AfterImageKey[] keys, AfterImageDesc after_image)//, TextureManager tex)
        {
            int key_length = keys.Length - 1;

            float[] lengths = new float[key_length + 1];
            int li = 0;
            Vector3 prev_rot_pos_vec = Vector3.Zero;
            float total_length = 0;

            // calculate lengths in reverse order?
            for (int i = key_length; i >= 0; i--)
            {
                var key = keys[i];

                var rot_pos_vec = key.rot * after_image.Top - prev_rot_pos_vec;

                // don't record length 0
                if (i != key_length)
                {
                    total_length += rot_pos_vec.LengthFast;
                    lengths[li] = total_length;
                    li++;
                }

                prev_rot_pos_vec = rot_pos_vec;
            }

            // make sure total length isn't 0 I guess
            if (total_length < 0)
                total_length = 1;

            //// render sword trail
            GL.PushAttrib(AttribMask.AllAttribBits);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            //GL.Enable(EnableCap.Texture2D);
            //GL.ActiveTexture(TextureUnit.Texture0);
            //GL.BindTexture(TextureTarget.Texture2D, tex.GetGLID(0));

            GL.Begin(PrimitiveType.TriangleStrip);

            // calculate top offset;
            float offset_bot = after_image.x0 * (after_image.Top - after_image.Bottom) + after_image.Bottom;
            float offset_top = after_image.x4 * (after_image.Top - after_image.Bottom) + after_image.Bottom;
            
            float dif_bot = after_image.Bottom - offset_bot;
            float dif_top = after_image.Top - offset_top;

            float prev_length = 1;
            li = 0;

            // draw keys in reverse order
            for (int i = key_length; i >= 0; i--)
            {
                var key = keys[i];

                float top_length = (prev_length * dif_top + offset_top);
                float bot_length = (prev_length * dif_bot + offset_bot);

                byte alphaRange = (byte)(after_image.AlphaStart - after_image.AlphaEnd);

                float alpha = (prev_length * alphaRange) + after_image.AlphaEnd;

                float texx_top = i / (float)key_length;
                float texx_bottom = i / (float)key_length;

                //GL.TexCoord2(texx_bottom, 0);
                GL.Color4(after_image.InCol.X, after_image.InCol.Y, after_image.InCol.Z, alpha / 255f);
                GL.Vertex3(key.rot * bot_length + key.pos);

                //GL.TexCoord2(texx_top, 1);
                GL.Color4(after_image.OutCol.X, after_image.OutCol.Y, after_image.OutCol.Z, alpha / 255f);
                GL.Vertex3(key.rot * top_length + key.pos);

                if (i != 0)
                {
                    var key_next = keys[i - 1];

                    float rot_angle = Vector3.CalculateAngle(key_next.rot, key.rot);
                    float num_of_blend_step = rot_angle / 0.08726646f; // 5 deg to rad

                    prev_length = 1 - (lengths[li++] / total_length);

                    if (num_of_blend_step != 0)
                    {
                        float angle = 0;
                        var a = 1 / num_of_blend_step;

                        float angle_step = a * rot_angle;
                        float top_length_step = a * ((prev_length * dif_top + offset_top) - top_length);
                        float bot_length_step = a * ((prev_length * dif_bot + offset_bot) - bot_length);

                        float alpha_step = a * ((prev_length * alphaRange + after_image.AlphaEnd) - alpha);

                        float tex_step_top = a * (((i - 1f) / key_length) - texx_top);
                        float tex_step_bottom = a * (((i - 1f) / key_length) - texx_bottom);

                        Vector3 start_pos = new Vector3(key.pos);
                        Vector3 pos_step = a * (key_next.pos - key.pos);

                        var normal_cross = Vector3.Cross(key.rot, key_next.rot).Normalized();

                        for (int blendi = 0; blendi < num_of_blend_step - 2; blendi++)
                        {
                            angle += angle_step;
                            alpha += alpha_step;
                            start_pos += pos_step;

                            top_length += top_length_step;
                            bot_length += bot_length_step;

                            texx_top += tex_step_top;
                            texx_bottom += tex_step_bottom;

                            var rotation = new Vector3(key.rot);
                            rotation.RotateAboutUnitAxis(angle, normal_cross);

                            //GL.TexCoord2(angle, 0);
                            GL.Color4(after_image.InCol.X, after_image.InCol.Y, after_image.InCol.Z, alpha / 255f);
                            GL.Vertex3(rotation * bot_length + start_pos);

                            //GL.TexCoord2(angle, 1);
                            GL.Color4(after_image.OutCol.X, after_image.OutCol.Y, after_image.OutCol.Z, alpha / 255f);
                            GL.Vertex3(rotation * top_length + start_pos);
                        }
                    }
                }
            }

            GL.End();

            GL.PopAttrib();

        }

    }
}
