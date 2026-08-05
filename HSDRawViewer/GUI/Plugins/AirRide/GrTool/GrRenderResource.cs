using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool
{
    public class GrRenderResource
    {
        public Camera Camera { get; set; }

        public int WindowWidth { get; set; }

        public int WindowHeight { get; set; }

        public LiveJObj Joints { get; set; }

        public void BeginDraw()
        {
            GL.Clear(ClearBufferMask.DepthBufferBit);
        }

        public void BeginOverlay()
        {
            GL.Disable(EnableCap.DepthTest);
        }

        public void DrawKdMesh(KdMesh mesh, bool is_selected)
        {
            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();

            if (Joints != null && mesh.Parent >= 0 && mesh.Parent < Joints.JointCount)
            {
                var t = Joints.GetJObjAtIndex(mesh.Parent).WorldTransform;
                GL.MultMatrix(ref t);
            }

            //GL.Disable(EnableCap.DepthTest);
            //GL.Clear(ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.CullFace);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);

            GL.Begin(PrimitiveType.Triangles);
            foreach (var t in mesh.Triangles)
            {
                if (t.Material < 0 || t.Material >= mesh.Materials.Count)
                    continue;

                var mat = mesh.Materials[t.Material];

                var color = Vector3.One;
                switch (mat.Type)
                {
                    case KdType.CEILING: color = Vector3.UnitX; break;
                    case KdType.FLOOR: color = Vector3.UnitY; break;
                    case KdType.WALL: color = Vector3.UnitZ; break;
                }

                if (!is_selected)
                    color *= 0.75f;

                GL.Color4(color.X, color.Y, color.Z, is_selected ? 1f : 0.75f);
                foreach (var i in t.Indices)
                {
                    var p = mesh.Vertices[i];
                    if (p.Count != 3) 
                        continue;
                    GL.Vertex3(p[0], p[1], p[2]);
                }
            }
            GL.End();


            GL.LineWidth(2f);
            GL.Disable(EnableCap.CullFace);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            GL.Begin(PrimitiveType.Triangles);
            if (is_selected)
                GL.Color3(Vector3.One);
            else
                GL.Color3(Vector3.Zero);
            foreach (var t in mesh.Triangles)
            {
                foreach (var i in t.Indices)
                {
                    var p = mesh.Vertices[i];
                    if (p.Count != 3) continue;
                    GL.Vertex3(p[0], p[1], p[2]);
                }
            }
            GL.End();

            GL.PopMatrix();
            GL.PopAttrib();
        }

        public void DrawKdSelectedTriangle(KdMesh mesh, KdTriangle tri)
        {
            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();

            if (Joints != null && mesh.Parent >= 0 && mesh.Parent < Joints.JointCount)
            {
                var t = Joints.GetJObjAtIndex(mesh.Parent).WorldTransform;
                GL.MultMatrix(ref t);
            }

            //GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            //GL.Enable(EnableCap.CullFace);
            //GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);

            //GL.Begin(PrimitiveType.Triangles);
            //GL.Color4(1f, 1f, 1f, 0.5f);
            //foreach (var i in tri.Indices)
            //{
            //    var p = mesh.Vertices[i];
            //    if (p.Count != 3) continue;
            //    GL.Vertex3(p[0], p[1], p[2]);
            //}
            //GL.End();

            GL.Disable(EnableCap.CullFace);

            GL.LineWidth(4f);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            GL.Begin(PrimitiveType.Triangles);
            GL.Color3(1f, 1f, 0f);
            foreach (var i in tri.Indices)
            {
                var p = mesh.Vertices[i];
                if (p.Count != 3) continue;
                GL.Vertex3(p[0], p[1], p[2]);
            }
            GL.End();

            GL.PopMatrix();
            GL.PopAttrib();
        }


        public void DrawKdZone(KdZone mesh, bool is_selected)
        {
            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();

            if (Joints != null && mesh.Parent >= 0 && mesh.Parent < Joints.JointCount)
            {
                var t = Joints.GetJObjAtIndex(mesh.Parent).WorldTransform;
                GL.MultMatrix(ref t);
            }

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.CullFace);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);

            GL.Begin(PrimitiveType.Triangles);
            if (is_selected)
                GL.Color4(1f, 1f, 0f, 0.7f);
            else
                GL.Color4(1f, 1f, 1f, 0.5f);
            foreach (var t in mesh.Triangles)
            {
                foreach (var i in t.Indices)
                {
                    var p = mesh.Vertices[i];
                    if (p.Count != 3)
                        continue;
                    GL.Vertex3(p[0], p[1], p[2]);
                }
            }
            GL.End();


            GL.PointSize(10f);
            GL.Disable(EnableCap.CullFace);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            GL.Begin(PrimitiveType.Points);
            GL.Color4(1f, 1f, 1f, 1f);
            foreach (var p in mesh.Vertices)
            {
                GL.Vertex3(p[0], p[1], p[2]);
            }
            GL.End();


            //GL.LineWidth(4f);
            //GL.Disable(EnableCap.CullFace);
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            //GL.Begin(PrimitiveType.Triangles);
            //if (is_selected)
            //    GL.Color4(1f, 0f, 1f, 1f);
            //else
            //    GL.Color4(1f, 0f, 1f, 0.5f);
            //foreach (var t in mesh.Triangles)
            //{
            //    foreach (var i in t.Indices)
            //    {
            //        var p = mesh.Vertices[i];
            //        if (p.Count != 3) continue;
            //        GL.Vertex3(p[0], p[1], p[2]);
            //    }
            //}
            //GL.End();

            GL.PopMatrix();
            GL.PopAttrib();
        }

    }
}
