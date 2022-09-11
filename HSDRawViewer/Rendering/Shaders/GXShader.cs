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
            var path = AppDomain.CurrentDomain.BaseDirectory;
            LoadShader(System.IO.Path.Combine(path, @"Shader\gx_material.frag"), ShaderType.VertexShader);
            LoadShader(System.IO.Path.Combine(path, @"Shader\gx.vert"));
            LoadShader(System.IO.Path.Combine(path, @"Shader\gx_uv.frag"));
            LoadShader(System.IO.Path.Combine(path, @"Shader\gx_lightmap.frag"));
            LoadShader(System.IO.Path.Combine(path, @"Shader\gx_material.frag"));
            LoadShader(System.IO.Path.Combine(path, @"Shader\gx_alpha_test.frag"));
            LoadShader(System.IO.Path.Combine(path, @"Shader\gx.frag"));
            Link();

            if (!ProgramCreatedSuccessfully())
                System.Windows.Forms.MessageBox.Show("Shader failed to link or compile");
        }

        /// <summary>
        /// 
        /// </summary>
        public void Bind(Camera camera, GXFogParam fog)
        {
            if (!ProgramCreatedSuccessfully())
                return;

            // bind shader
            GL.UseProgram(programId);


            // load model view matrix
            var mvp = camera.MvpMatrix;
            GL.UniformMatrix4(GetVertexAttributeUniformLocation("mvp"), false, ref mvp);

            // set camera position
            //var campos = (camera.RotationMatrix * new Vector4(camera.Translation, 1)).Xyz;
            var campos = camera.TransformedPosition; // Vector4.TransformRow(new Vector4(0, 0, 0, 1), camera.ModelViewMatrix.Inverted());
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
