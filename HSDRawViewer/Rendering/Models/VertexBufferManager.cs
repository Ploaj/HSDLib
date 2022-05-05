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
        public bool EnableBuffers(Shader GXShader, HSD_DOBJ dobj, int shapeset1, int shapeset2, float blend)
        {
            if (!attributeToBuffer.ContainsKey(dobj))
                return false;

            // set blend shape
            GXShader.SetFloat("shape_blend", blend);

            // normal attributes
            GL.BindBuffer(BufferTarget.ArrayBuffer, attributeToBuffer[dobj]);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 1, VertexAttribPointerType.Short, false, GX_Vertex.Stride, 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 8);

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 20);

            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 32);

            GL.EnableVertexAttribArray(4);
            GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 44);

            GL.EnableVertexAttribArray(5);
            GL.VertexAttribPointer(5, 2, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 88);

            GL.EnableVertexAttribArray(6);
            GL.VertexAttribPointer(6, 2, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 96);

            GL.EnableVertexAttribArray(7);
            GL.VertexAttribPointer(7, 2, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 104);

            GL.EnableVertexAttribArray(8);
            GL.VertexAttribPointer(8, 2, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 112);

            GL.EnableVertexAttribArray(9);
            GL.VertexAttribPointer(9, 4, VertexAttribPointerType.Float, true, GX_Vertex.Stride, 56);


            // shape sets (if exists)
            if (dobjToShapes.ContainsKey(dobj))
            {
                if (shapeset1 < dobjToShapes[dobj].Length)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, dobjToShapes[dobj][shapeset1]);

                    GL.EnableVertexAttribArray(1);
                    GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, GX_Shape.Stride, 0);

                    GL.EnableVertexAttribArray(2);
                    GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, GX_Shape.Stride, 12);
                }

                if (shapeset2 < dobjToShapes[dobj].Length)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, dobjToShapes[dobj][shapeset2]);

                    GL.EnableVertexAttribArray(10);
                    GL.VertexAttribPointer(10, 3, VertexAttribPointerType.Float, false, GX_Shape.Stride, 0);

                    GL.EnableVertexAttribArray(11);
                    GL.VertexAttribPointer(11, 3, VertexAttribPointerType.Float, false, GX_Shape.Stride, 12);
                }
            }
            else
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, attributeToBuffer[dobj]);

                GL.EnableVertexAttribArray(10);
                GL.VertexAttribPointer(10, 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 8);

                GL.EnableVertexAttribArray(11);
                GL.VertexAttribPointer(11, 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 20);

            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="GXShader"></param>
        public void Disable()
        {
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
            GL.DisableVertexAttribArray(3);
            GL.DisableVertexAttribArray(4);
            GL.DisableVertexAttribArray(5);
            GL.DisableVertexAttribArray(6);
            GL.DisableVertexAttribArray(7);
            GL.DisableVertexAttribArray(8);
            GL.DisableVertexAttribArray(9);

            GL.DisableVertexAttribArray(10);
            GL.DisableVertexAttribArray(11);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
