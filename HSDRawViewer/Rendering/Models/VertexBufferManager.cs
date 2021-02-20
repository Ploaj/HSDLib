using HSDRaw;
using HSDRaw.Common;
using HSDRaw.GX;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace HSDRawViewer.Rendering.Models
{
    /// <summary>
    /// Manages pobj attribute buffer data
    /// </summary>
    public class VertexBufferManager
    {
        private Dictionary<HSD_DOBJ, int> attributeToBuffer = new Dictionary<HSD_DOBJ, int>();

        private Dictionary<HSD_DOBJ, int[]> dobjToShapes = new Dictionary<HSD_DOBJ, int[]>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        public void AddBuffer(HSD_DOBJ dobj, GX_Vertex[] vertices)
        {
            // generate buffer
            int buf;
            GL.GenBuffers(1, out buf);
            GL.BindBuffer(BufferTarget.ArrayBuffer, buf);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * GX_Vertex.Stride, vertices, BufferUsageHint.StaticDraw);

            if(buf != -1)
                attributeToBuffer.Add(dobj, buf);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        public void AddShapeSets(HSD_DOBJ dobj, List<List<GX_Shape>> shapes)
        {
            if (!dobjToShapes.ContainsKey(dobj))
                dobjToShapes.Add(dobj, new int[shapes.Count]);
            else
            {
                foreach (var v in dobjToShapes[dobj])
                    GL.DeleteBuffer(v);

                dobjToShapes[dobj] = new int[shapes.Count];
            }

            int si = 0;
            foreach(var shape in shapes)
            {
                // convert to array
                var arr = shape.ToArray();

                // generate buffer
                int buf;
                GL.GenBuffers(1, out buf);
                GL.BindBuffer(BufferTarget.ArrayBuffer, buf);
                GL.BufferData(BufferTarget.ArrayBuffer, arr.Length * GX_Shape.Stride, arr, BufferUsageHint.StaticDraw);

                if (buf != -1)
                    dobjToShapes[dobj][si] = buf;

                si++;
            }
        }

        /// <summary>
        /// Frees all resources used by the manager
        /// </summary>
        public void ClearRenderingCache()
        {
            foreach (var v in attributeToBuffer)
                GL.DeleteBuffer(v.Value);
            attributeToBuffer.Clear();

            foreach (var s in dobjToShapes)
                foreach (var v in s.Value)
                    GL.DeleteBuffer(v);
            dobjToShapes.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dobj"></param>
        /// <returns></returns>
        public bool BindBuffer(Shader GXShader, HSD_DOBJ dobj, int shapeset1, int shapeset2, float blend)
        {
            if (!attributeToBuffer.ContainsKey(dobj))
                return false;

            GXShader.SetFloat("shape_blend", blend);

            // normal attributes
            GL.BindBuffer(BufferTarget.ArrayBuffer, attributeToBuffer[dobj]);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("PNMTXIDX"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("PNMTXIDX"), 1, VertexAttribPointerType.Short, false, GX_Vertex.Stride, 0);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_POS"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_POS"), 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 8);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_NRM"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_NRM"), 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 20);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_BTAN"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_TAN"), 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 32);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_TAN"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_BTAN"), 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 44);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_CLR0"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_CLR0"), 4, VertexAttribPointerType.Float, true, GX_Vertex.Stride, 56);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX0"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX0"), 2, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 88);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX1"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX1"), 2, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 96);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX2"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX2"), 2, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 104);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX3"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX3"), 2, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 112);


            // shape sets (if exists)
            if(dobjToShapes.ContainsKey(dobj))
            {
                if (shapeset1 < dobjToShapes[dobj].Length)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, dobjToShapes[dobj][shapeset1]);

                    GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_POS"));
                    GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_POS"), 3, VertexAttribPointerType.Float, false, GX_Shape.Stride, 0);

                    GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_NRM"));
                    GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_NRM"), 3, VertexAttribPointerType.Float, false, GX_Shape.Stride, 12);
                }

                if (shapeset2 < dobjToShapes[dobj].Length)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, dobjToShapes[dobj][shapeset2]);

                    GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_POS_SHAPE"));
                    GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_POS_SHAPE"), 3, VertexAttribPointerType.Float, false, GX_Shape.Stride, 0);

                    GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_NRM_SHAPE"));
                    GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_NRM_SHAPE"), 3, VertexAttribPointerType.Float, false, GX_Shape.Stride, 12);
                }
            }
            else
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, attributeToBuffer[dobj]);

                GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_POS_SHAPE"));
                GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_POS_SHAPE"), 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 8);

                GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_NRM_SHAPE"));
                GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_NRM_SHAPE"), 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 20);

            }
            return true;
        }

        public void Unbind(Shader GXShader)
        {
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("PNMTXIDX"));
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_POS"));
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_NRM"));
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_TAN"));
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_BTAN"));
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_CLR0"));
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX0"));
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX1"));
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX2"));
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX3"));

            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_POS_SHAPE"));
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_NRM_SHAPE"));

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
