using System;
using HSDRaw.GX;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace HSDRawViewer.Rendering.Renderers
{
    /// <summary>
    /// 
    /// </summary>
    public class ParticleShader
    {
        // Shader
        private static Shader _shader;

        private static int _vb_pos;

        public GXAlphaOp AlphaOp = GXAlphaOp.And;
        public GXCompareType AlphaComp0 = GXCompareType.Always;
        public GXCompareType AlphaComp1 = GXCompareType.Always;
        public float AlphaRef0 = 0;
        public float AlphaRef1 = 0;

        public bool EnablePrimEnv = false;

        public Vector4 PrimitiveColor = Vector4.One;

        public Vector4 EnvironmentColor = Vector4.One;

        public bool HasTexture = false;

        public Matrix4 MVP = Matrix4.Identity;

        public float TexScaleX = 1;

        public float TexScaleY = 1;

        /// <summary>
        /// 
        /// </summary>
        private void Bind()
        {
            // load shader if it's not ready yet
            if (_shader == null)
            {
                _shader = new Shader();
                _shader.LoadShader(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Shader\ps.vert"));
                _shader.LoadShader(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Shader\gx_alpha_test.frag"));
                _shader.LoadShader(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Shader\ps.frag"));
                _shader.Link();

                if (!_shader.ProgramCreatedSuccessfully())
                    System.Windows.Forms.MessageBox.Show("Particle Shader failed to link or compile");

                GL.GenBuffers(1, out _vb_pos);
            }

            if (!_shader.ProgramCreatedSuccessfully())
                return;

            // bind shader
            GL.UseProgram(_shader.programId);

            // load model view matrix
            GL.UniformMatrix4(_shader.GetVertexAttributeUniformLocation("mvp"), false, ref MVP);

            // bind sampler
            _shader.SetInt("sampler0", 0);
            _shader.SetBoolToInt("use_texture", HasTexture);

            // bind alpha
            _shader.SetInt("alphaOp", (int)AlphaOp);
            _shader.SetInt("alphaComp0", (int)AlphaComp0);
            _shader.SetInt("alphaComp1", (int)AlphaComp1);
            _shader.SetFloat("alphaRef0", AlphaRef0);
            _shader.SetFloat("alphaRef1", AlphaRef1);

            // set colors
            _shader.SetVector4("primColor", PrimitiveColor);
            _shader.SetVector4("envColor", EnvironmentColor);
            _shader.SetBoolToInt("enablePrimEnv", EnablePrimEnv);

            // for mirror
            _shader.SetVector2("texscale", TexScaleX, TexScaleY);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="vertices"></param>
        /// <param name="uvs"></param>
        public void DrawDynamicPOS(Camera c, int count, ref float[] vertices)
        {
            Bind();

            int stride = sizeof(float) * 5;

            GL.EnableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vb_pos);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.VertexAttrib2(1, 0, 0);
            GL.VertexAttrib4(2, 1f, 1f, 1f, 1f);

            GL.DrawArrays(PrimitiveType.Quads, 0, count);

            GL.DisableVertexAttribArray(0);

            GL.UseProgram(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="vertices"></param>
        /// <param name="uvs"></param>
        public void DrawDynamicPOSTEX(int count, ref float[] vertices)
        {
            Bind();

            int stride = sizeof(float) * 5;

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vb_pos);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, sizeof(float) * 3);
            GL.VertexAttrib4(2, 1f, 1f, 1f, 1f);

            GL.DrawArrays(PrimitiveType.Quads, 0, count);

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);

            GL.UseProgram(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="vertices"></param>
        /// <param name="uvs"></param>
        public void DrawDynamicPOSTEXCLR(int count, ref float[] vertices)
        {
            Bind();

            int stride = sizeof(float) * 9;

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vb_pos);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 12);
            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, stride, 20);

            GL.DrawArrays(PrimitiveType.Quads, 0, count);

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);

            GL.UseProgram(0);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Unbind()
        {
            GL.UseProgram(0);
        }

    }
}
