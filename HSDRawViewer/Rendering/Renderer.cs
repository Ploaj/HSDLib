using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Melee.Gr;
using HSDRawViewer.Rendering.Renderers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HSDRawViewer.Rendering
{
    /// <summary>
    /// A special renderer that attempts to translate the GX rendering from JOBJ into the OpenGL equivalent
    /// </summary>
    public class Renderer
    {
        private HSDAccessor currentAccessor;

        private RendererJOBJ JOBJRenderer = new RendererJOBJ();

        private Dictionary<Type, IRenderer> TypeToRenderer
        {
            get
            {
                if (_typeToRenderer == null)
                    InitRendererCache();
                return _typeToRenderer;
            }
        }

        private Dictionary<Type, IRenderer> _typeToRenderer;

        private void InitRendererCache()
        {
            _typeToRenderer = new Dictionary<Type, IRenderer>();

            var types = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                    from assemblyType in domainAssembly.GetTypes()
                                    where typeof(IRenderer).IsAssignableFrom(assemblyType)
                                    select assemblyType).ToArray();

            foreach (var t in types)
            {
                if (t != typeof(IRenderer))
                {
                    var ren = (IRenderer)Activator.CreateInstance(t);

                    foreach(var v in ren.SupportedTypes)
                    {
                        _typeToRenderer.Add(v, ren);
                    }
                }
            }
        }
        
        /// <summary>
        /// Clears all rendering cache
        /// </summary>
        public void ClearCache()
        {
            JOBJRenderer.ClearCache();

            foreach(var v in TypeToRenderer)
            {
                v.Value.Clear();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joint"></param>
        public void SetAnimJoint(HSD_AnimJoint joint)
        {
            JOBJRenderer.SetAnimJoint(joint);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetFigaTree(HSD_FigaTree tree)
        {
            JOBJRenderer.SetFigaTree(tree);
        }

        /// <summary>
        /// Renders supported Accessors
        /// </summary>
        /// <param name="Camera"></param>
        /// <param name="accessor"></param>
        public void Render(Matrix4 Camera, HSDAccessor accessor, Viewport vp)
        {
            if(currentAccessor != accessor)
            {
                vp.ClearControls();
                ClearCache();
                currentAccessor = accessor;

                if (TypeToRenderer.ContainsKey(currentAccessor.GetType()))
                {
                    var ts = TypeToRenderer[currentAccessor.GetType()].ToolStrip;

                    if (ts != null)
                        vp.AddToolStrip(ts);
                }
            }

            GL.PushAttrib(AttribMask.AllAttribBits);

            if (TypeToRenderer.ContainsKey(currentAccessor.GetType()))
            {
                TypeToRenderer[currentAccessor.GetType()].Render(accessor, vp.ViewportWidth, vp.ViewportHeight);
            }
            else
            if (accessor is HSD_JOBJ jobj)
            {
                JOBJRenderer.Render(jobj);
                RenderFloor();
            }
            else
            if (accessor is SBM_Coll_Data coll)
            {
                CollDataRenderer.RenderColl_Data(coll);
                RenderFloor();
            }

            GL.PopAttrib();
        }


        private void RenderFloor()
        {
            GL.PushAttrib(AttribMask.AllAttribBits);

            int size = 50;
            int space = 5;

            GL.LineWidth(1f);
            GL.Color3(Color.White);
            GL.Begin(PrimitiveType.Lines);

            for (int i = -size; i <= size; i += space)
            {
                GL.Vertex3(-size, 0, i);
                GL.Vertex3(size, 0, i);

                GL.Vertex3(i, 0, -size);
                GL.Vertex3(i, 0, size);
            }

            GL.End();
            GL.PopAttrib();
        }


    }
}
