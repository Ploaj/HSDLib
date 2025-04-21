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
        public int BufferCount { get => dobjToVBO.Count; }

        private readonly Dictionary<HSD_DOBJ, int> dobjToVBO = new();

        private readonly Dictionary<HSD_DOBJ, int> dobjToVAO = new();

        private readonly Dictionary<HSD_DOBJ, int[]> dobjToShapes = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        public void AddBuffer(HSD_DOBJ dobj, GX_Vertex[] vertices)
        {
            // make sure dobj is not already in a vbo
            if (dobjToVBO.ContainsKey(dobj))
                return;

            // generate buffer
            int _vbo = GL.GenBuffer();
            if (_vbo != -1)
                dobjToVBO.Add(dobj, _vbo);
            else
                return;

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer,
                vertices.Length * System.Runtime.InteropServices.Marshal.SizeOf(typeof(GX_Vertex)),
                vertices,
                BufferUsageHint.StaticDraw);

            // generate buffer
            int _vao = GL.GenVertexArray();
            if (_vao != -1)
                dobjToVAO.Add(dobj, _vao);
            else
                return;
            GL.BindVertexArray(_vao);

            System.Diagnostics.Debug.WriteLine(_vao + " " + _vbo + " " + GL.GetError());

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

            GL.EnableVertexAttribArray(10);
            GL.VertexAttribPointer(10, 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 8);

            GL.EnableVertexAttribArray(11);
            GL.VertexAttribPointer(11, 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 20);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
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
                foreach (int v in dobjToShapes[dobj])
                    GL.DeleteBuffer(v);

                dobjToShapes[dobj] = new int[shapes.Count];
            }

            int si = 0;
            foreach (List<GX_Shape> shape in shapes)
            {
                // convert to array
                GX_Shape[] arr = shape.ToArray();

                // generate buffer
                GL.GenBuffers(1, out int buf);
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
            foreach (KeyValuePair<HSD_DOBJ, int> v in dobjToVBO)
                GL.DeleteBuffer(v.Value);
            dobjToVBO.Clear();

            foreach (KeyValuePair<HSD_DOBJ, int[]> s in dobjToShapes)
                foreach (int v in s.Value)
                    GL.DeleteBuffer(v);
            dobjToShapes.Clear();

            foreach (KeyValuePair<HSD_DOBJ, int> v in dobjToVAO)
                GL.DeleteVertexArray(v.Value);
            dobjToVAO.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dobj"></param>
        /// <returns></returns>
        public bool EnableBuffers(Shader GXShader, HSD_DOBJ dobj, int shapeset1, int shapeset2, float blend)
        {
            if (!dobjToVBO.ContainsKey(dobj))
                return false;

            // set blend shape
            GXShader.SetFloat("shape_blend", blend);

            // 
            int _vao = dobjToVAO[dobj];
            GL.BindVertexArray(_vao);

            // normal attributes
            //GL.BindBuffer(BufferTarget.ArrayBuffer, attributeToBuffer[dobj]);

            //GL.EnableVertexAttribArray(0);
            //GL.VertexAttribPointer(0, 1, VertexAttribPointerType.Short, false, GX_Vertex.Stride, 0);

            //GL.EnableVertexAttribArray(1);
            //GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 8);

            //GL.EnableVertexAttribArray(2);
            //GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 20);

            //GL.EnableVertexAttribArray(3);
            //GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 32);

            //GL.EnableVertexAttribArray(4);
            //GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 44);

            //GL.EnableVertexAttribArray(5);
            //GL.VertexAttribPointer(5, 2, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 88);

            //GL.EnableVertexAttribArray(6);
            //GL.VertexAttribPointer(6, 2, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 96);

            //GL.EnableVertexAttribArray(7);
            //GL.VertexAttribPointer(7, 2, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 104);

            //GL.EnableVertexAttribArray(8);
            //GL.VertexAttribPointer(8, 2, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 112);

            //GL.EnableVertexAttribArray(9);
            //GL.VertexAttribPointer(9, 4, VertexAttribPointerType.Float, true, GX_Vertex.Stride, 56);

            //GL.EnableVertexAttribArray(10);
            //GL.VertexAttribPointer(10, 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 8);

            //GL.EnableVertexAttribArray(11);
            //GL.VertexAttribPointer(11, 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 20);

            //shape sets(if exists)
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

            return true;
        }
    }
}
