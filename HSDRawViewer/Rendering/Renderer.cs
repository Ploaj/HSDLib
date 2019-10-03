using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Melee.Gr;
using HSDRawViewer.Rendering.Renderers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace HSDRawViewer.Rendering
{
    /// <summary>
    /// A special renderer that attempts to translate the GX rendering from JOBJ into the OpenGL equivalent
    /// </summary>
    public class Renderer
    {
        private HSDAccessor currentAccessor;

        private RendererJOBJ JOBJRenderer = new RendererJOBJ();
        
        /// <summary>
        /// Clears all rendering cache
        /// </summary>
        public void ClearCache()
        {
            JOBJRenderer.ClearCache();
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

                if (accessor is HSD_TexGraphic)
                    vp.AddToolStrip(TexGraphicRenderer.ToolStrip);
                if (accessor is HSD_TexAnim)
                    vp.AddToolStrip(RendererTexAnim.ToolStrip);
                if (accessor is HSD_TOBJ)
                    vp.AddToolStrip(RendererTOBJ.ToolStrip);
            }

            GL.PushAttrib(AttribMask.AllAttribBits);

            if (accessor is HSD_TOBJ tobj)
                RendererTOBJ.Render(tobj, vp.ViewportWidth, vp.ViewportHeight);
            else
            if (accessor is HSD_TexGraphic image)
                TexGraphicRenderer.Render(image, vp.ViewportWidth, vp.ViewportHeight);
            else
            if (accessor is HSD_TexAnim texanim)
                RendererTexAnim.Render(texanim, vp.ViewportWidth, vp.ViewportHeight);
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
