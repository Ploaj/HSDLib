using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace HSDRawViewer.Rendering
{
    public class OpenTKResources
    {
        public static string Renderer { get; internal set; }
        public static string OpenGLVersion { get; internal set; }
        public static string GLSLVersion { get; internal set; }

        //private static OpenTK.GLControl control { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static void MakeCurrentDummy()
        {
            //control.MakeCurrent();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Init()
        {
            // GraphicsContext.ShareContexts = true;
            //control = new OpenTK.GLControl();
            //control.MakeCurrent();
            GLFW.Init();
            GL.LoadBindings(new GLFWBindingsContext());

            //Renderer = GL.GetString(StringName.Renderer);
            //OpenGLVersion = GL.GetString(StringName.Version);
            //GLSLVersion = GL.GetString(StringName.ShadingLanguageVersion);
        }
    }
}
