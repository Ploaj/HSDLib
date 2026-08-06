using HSDRaw.AirRide.Gr;
using HSDRawViewer.GUI.Plugins.AirRide.GrTool.Converters;
using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Rendering.Renderers;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool
{
    public class GrRenderResource : IDisposable
    {
        public Camera Camera { get; set; }

        public int WindowWidth { get; set; }

        public int WindowHeight { get; set; }

        public LiveJObj Joints { get => RenderJObj != null ? RenderJObj.RootJObj : null; }

        public JobjDisplaySettings RenderSettings { get => RenderJObj._settings; }

        private readonly GLTextRenderer TextRenderer = new();


        private RenderJObj RenderJObj;

        public bool RenderBones { get => RenderJObj._settings.RenderBones; set => RenderJObj._settings.RenderBones = value; }

        public bool RenderModel { get; set; }

        public bool RenderBoneLabels { get; set; }

        public GrRenderResource()
        {
            RenderJObj = new RenderJObj();
        }

        public void GLInit()
        {
            RenderJObj.Invalidate();
            TextRenderer.InitializeRender(@"Consolas.bff");
        }

        public void GLFree()
        {
            RenderJObj.FreeResources();
            TextRenderer.Dispose();
        }

        public void BeginDraw()
        {
            GL.Clear(ClearBufferMask.DepthBufferBit);
        }

        public void BeginOverlay()
        {
            GL.Disable(EnableCap.DepthTest);
        }

        internal void LoadModel(KAR_grModel model)
        {
            RenderJObj.LoadJObj(model.MainModel.RootNode);
        }

        public void DrawModel(Camera cam)
        {
            if (RenderJObj != null && RenderModel)
            {
                RenderJObj.Render(cam, false);
            }
        }

        public void DrawBoneLabels(Camera cam)
        {
            if (RenderJObj != null && RenderBoneLabels)
            {
                int i = 0;
                foreach (var j in RenderJObj.RootJObj.Enumerate)
                {
                    TextRenderer.RenderText(cam, $"{i}", j.WorldTransform, dropShadow: true);
                    i++;
                }
            }
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


        public void DrawKdZone(KdZone mesh, bool is_selected, object selected_object)
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


            GL.LineWidth(2f);
            GL.Enable(EnableCap.CullFace);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);

            GL.Begin(PrimitiveType.Triangles);
            if (is_selected)
                GL.Color4(0.8f, 0, 0f, 1f);
            else
                GL.Color4(0.8f, 0.8f, 0.8f, 1f);
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


            if (is_selected && selected_object is EdgeAccessor edge)
            {
                GL.LineWidth(4f);
                GL.Disable(EnableCap.CullFace);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

                GL.Begin(PrimitiveType.Lines);
                GL.Vertex3(edge.Vertex1.X, edge.Vertex1.Y, edge.Vertex1.Z);
                GL.Vertex3(edge.Vertex2.X, edge.Vertex2.Y, edge.Vertex2.Z);
                GL.End();
            }

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

        public void Dispose()
        {
            TextRenderer.Dispose();
        }
    }
}
