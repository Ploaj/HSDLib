using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.WinForms;

namespace HSDRawViewer.Rendering
{
    public class OpenTKResources
    {
        public static string Renderer { get; internal set; }
        public static string OpenGLVersion { get; internal set; }
        public static string GLSLVersion { get; internal set; }

        private static GLControl control;

        //public static IGLFWGraphicsContext SharedContext { get; internal set; }// = (IGLFWGraphicsContext)control.Context;

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
            GLFW.Init();
            GL.LoadBindings(new GLFWBindingsContext());

            control = new GLControl();
            control.MakeCurrent();

            Renderer = GL.GetString(StringName.Renderer);
            OpenGLVersion = GL.GetString(StringName.Version);
            GLSLVersion = GL.GetString(StringName.ShadingLanguageVersion);


        }
    }
}
