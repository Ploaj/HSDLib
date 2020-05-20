using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK;
using System.Runtime.ExceptionServices;

namespace HSDRawViewer.Rendering
{
    public enum SharedResourceStatus
    {
        Initialized,
        Failed,
        Uninitialized
    }

    public class OpenTKResources
    {
        public static string Renderer { get; internal set; }
        public static string OpenGLVersion { get; internal set; }
        public static string GLSLVersion { get; internal set; }

        public static SharedResourceStatus SetupStatus { get; private set; } = SharedResourceStatus.Uninitialized;

        // Keep a context around to avoid setting up after making each context.
        private static GameWindow dummyResourceWindow;

        public static bool MakeDummyCurrent()
        {
            if(SetupStatus == SharedResourceStatus.Initialized)
            {
                dummyResourceWindow.MakeCurrent();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        [HandleProcessCorruptedStateExceptions]
        public static void Init()
        {
            // Only setup once. This is checked multiple times to prevent crashes.
            if (SetupStatus == SharedResourceStatus.Initialized)
                return;

            try
            {
                // Make a permanent context to share resources.
                GraphicsContext.ShareContexts = true;
                dummyResourceWindow = CreateGameWindowContext();

                GetGLInfo();

                SetupStatus = SharedResourceStatus.Initialized;
            }
            catch (AccessViolationException)
            {
                // Context creation failed.
                SetupStatus = SharedResourceStatus.Failed;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void GetGLInfo()
        {
            Renderer = GL.GetString(StringName.Renderer);
            OpenGLVersion = GL.GetString(StringName.Version);
            GLSLVersion = GL.GetString(StringName.ShadingLanguageVersion);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private static GameWindow CreateGameWindowContext(int width = 640, int height = 480)
        {
            GraphicsMode mode = new GraphicsMode(new ColorFormat(8, 8, 8, 8), 24, 0, 0, ColorFormat.Empty, 1);

            GameWindow gameWindow = new GameWindow(width, height, mode, "", GameWindowFlags.Default)
            {
                Visible = false
            };
            gameWindow.MakeCurrent();
            return gameWindow;
        }
    }
}
