using System;
using OpenTK.Graphics.OpenGL;
using System.ComponentModel;
using YamlDotNet.Serialization;
using OpenTK.Mathematics;
using HSDRawViewer.Rendering.GX;

namespace HSDRawViewer.Rendering.Shaders
{
    public class GXShader : Shader
    {
        [YamlIgnore, Browsable(false)]
        public RenderMode RenderMode { get; set; }


        [YamlIgnore, Browsable(false)]
        public int SelectedBone { get; set; }

        public Matrix4[] WorldTransforms = new Matrix4[400];

        public GXShader()
        {
            LoadShader(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Shader\gx.vert"));
            LoadShader(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Shader\gx_uv.frag"));
            LoadShader(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Shader\gx_lightmap.frag"));
            LoadShader(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Shader\gx_material.frag"));
            LoadShader(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Shader\gx_alpha_test.frag"));
            LoadShader(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Shader\gx.frag"));
            Link();

            if (!ProgramCreatedSuccessfully())
                System.Windows.Forms.MessageBox.Show("Shader failed to link or compile");
        }

        /// <summary>
        /// 
        /// </summary>
        public void Bind(Camera camera, GXLightParam light, GXFogParam fog)
        {
            if (!ProgramCreatedSuccessfully())
                return;

            // bind shader
            GL.UseProgram(programId);


            // load model view matrix
            var mvp = camera.MvpMatrix;
            GL.UniformMatrix4(GetVertexAttributeUniformLocation("mvp"), false, ref mvp);

            // set camera position
            var campos = (camera.RotationMatrix * new Vector4(camera.Translation, 1)).Xyz;
            SetVector3("cameraPos", campos);

            // create sphere matrix
            Matrix4 sphereMatrix = camera.ModelViewMatrix;
            sphereMatrix.Invert();
            sphereMatrix.Transpose();
            SetMatrix4x4("sphereMatrix", ref sphereMatrix);

            // ui
            SetInt("selectedBone", SelectedBone);
            SetInt("renderOverride", (int)RenderMode);

            // setup bone binds
            SetWorldTransformBones(WorldTransforms);

            // lighting
            light.Bind(this);

            // fog
            fog.Bind(this);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }

    }
}
