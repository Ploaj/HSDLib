using HSDRaw.AirRide.Gr;
using HSDRaw.GX;
using HSDRawViewer.GUI.Plugins.AirRide.GrTool.Converters;
using HSDRawViewer.GUI.Plugins.AirRide.GrTool.Nodes;
using HSDRawViewer.GUI.Plugins.AirRide.GrTool.Render;
using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Properties;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Rendering.Renderers;
using IONET.Collada.FX.Texturing;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool
{
    public enum GrCollisionNodeRenderKind
    {
        Type,

        Material,
        Segment,
        Rough,
        Restitution1,
        Restitution2,
        Conveyer,

        Flag00002000,
        Flag00004000,
        Flag00008000,
        Flag00010000,
        Flag00020000,
        Flag00040000,
        Flag00080000,
        Flag00100000,
        Flag00200000,
        Flag00400000,
        Flag00800000,
    }

    public class GrRenderResource : IDisposable
    {
        public static GrDisplaySettings Settings { get; } = new GrDisplaySettings();

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

        public GrCollisionNodeRenderKind CollisionRenderKind { get; set; } = GrCollisionNodeRenderKind.Type;

        private TextureManager _textures { get; set; }

        public bool IsXRay { get; internal set; }

        public bool DrawWireframe { get; set; }


        private int tex_id;

        public GrRenderResource()
        {
            RenderJObj = new RenderJObj();
            _textures = new TextureManager();
        }

        public void GLInit()
        {
            RenderJObj.Invalidate();
            TextRenderer.InitializeRender(@"Consolas.bff");

            tex_id = _textures.Add(Resources.ico3d_sound);
        }

        public void GLFree()
        {
            RenderJObj.FreeResources();
            TextRenderer.Dispose();
            _textures.ClearTextures();
        }

        public void BeginDraw()
        {
            if (IsXRay)
            {
                GL.Clear(ClearBufferMask.DepthBufferBit);
                return;
            }

            GL.Enable(EnableCap.PolygonOffsetFill);
            GL.PolygonOffset(-1.0f, -1.0f);

            GL.Enable(EnableCap.PolygonOffsetLine);
            GL.PolygonOffset(-1.0f, -1.0f);

            GL.DepthFunc(DepthFunction.Lequal);
        }

        public void BeginOverlay()
        {
            GL.Disable(EnableCap.DepthTest);
        }

        public void EndDraw()
        {
            GL.Disable(EnableCap.PolygonOffsetFill);
            GL.Disable(EnableCap.PolygonOffsetLine);
        }

        public void DrawTexture(
            Camera cam,
            Vector3 position,
            float pixelWidth,
            float pixelHeight,
            bool constant_size)
        {
            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            _textures.BindTexture(tex_id, TextureMagFilter.Nearest, TextureMinFilter.Nearest);

            float width = pixelWidth;
            float height = pixelHeight;

            if (constant_size)
            {
                // Distance from camera
                float distance = (position - cam.TransformedPosition).Length;

                // Convert desired pixel size to world size
                float worldHeight =
                    2.0f * distance *
                    MathF.Tan(cam.FovRadians * 0.5f) *
                    (pixelHeight / cam.RenderHeight);

                float worldWidth =
                    2.0f * distance *
                    MathF.Tan(cam.FovRadians * 0.5f) *
                    cam.AspectRatio *
                    (pixelWidth / cam.RenderWidth);

                width = (worldWidth * 0.5f);
                height = (worldHeight * 0.5f);
            }

            Vector3 right = cam.Right * width;
            Vector3 up = cam.Up * height;

            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord2(1, 0);
            GL.Vertex3(position - right - up);

            GL.TexCoord2(0, 0);
            GL.Vertex3(position + right - up);

            GL.TexCoord2(0, 1);
            GL.Vertex3(position + right + up);

            GL.TexCoord2(1, 1);
            GL.Vertex3(position - right + up);

            GL.End();

            GL.PopAttrib();
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

        private static Vector3[] DebugColors =
        {
            new Vector3(1.0f, 0.2f, 0.2f), // Red
            new Vector3(0.2f, 1.0f, 0.4f), // Green
            new Vector3(0.2f, 0.6f, 1.0f), // Blue
            new Vector3(1.0f, 1.0f, 0.2f), // Yellow
            new Vector3(1.0f, 0.2f, 1.0f), // Magenta

            new Vector3(0.2f, 1.0f, 1.0f), // Cyan
            new Vector3(1.0f, 0.6f, 0.2f), // Orange
            new Vector3(0.6f, 0.2f, 1.0f), // Purple
            new Vector3(0.6f, 1.0f, 0.2f), // Lime
            new Vector3(1.0f, 0.4f, 0.6f), // Pink
        };

        private Vector3 GetMaterialColor(KdMaterial mat)
        {
            switch (CollisionRenderKind)
            {
                case GrCollisionNodeRenderKind.Type:
                    switch (mat.Type)
                    {
                        case KdType.FLOOR: return Vector3.UnitX;
                        case KdType.WALL: return Vector3.UnitY;
                        case KdType.CEILING: return Vector3.UnitZ;
                        case KdType.UNKNOWN: return new Vector3(1f, 0f, 1f);
                    }

                    break;
                case GrCollisionNodeRenderKind.Segment:
                    if (mat.SegmentMove)
                        return Vector3.UnitX;
                    else
                        return Vector3.UnitZ;
                case GrCollisionNodeRenderKind.Material:
                    break;
                case GrCollisionNodeRenderKind.Rough:
                    if (mat.Rough >= 0 && mat.Rough < DebugColors.Length)
                    {
                        return DebugColors[mat.Rough];
                    }
                    return Vector3.One;
                case GrCollisionNodeRenderKind.Conveyer:
                    var k = mat.ConveyorVertical | mat.ConveyorHorizontal;
                    var v = Vector3.Zero;
                    if (k.HasFlag(KdConveyor.FORWARD))  v.Z = 1;
                    if (k.HasFlag(KdConveyor.BACKWARD)) v.Z = 0;
                    if (k.HasFlag(KdConveyor.RIGHT))    v.X = 1;
                    if (k.HasFlag(KdConveyor.LEFT))     v.X = 0;
                    return v;
                case GrCollisionNodeRenderKind.Restitution1:
                    if (mat.PlayerRestitutionIndex >= 0 && mat.PlayerRestitutionIndex < DebugColors.Length)
                    {
                        return DebugColors[mat.PlayerRestitutionIndex];
                    }
                    return Vector3.One;
                case GrCollisionNodeRenderKind.Restitution2:
                    if (mat.ItemRestitutionIndex >= 0 && mat.ItemRestitutionIndex < DebugColors.Length)
                    {
                        return DebugColors[mat.ItemRestitutionIndex];
                    }
                    return Vector3.One;
                case GrCollisionNodeRenderKind.Flag00002000: return mat.Flag00002000 ? Vector3.UnitX : Vector3.UnitZ;
                case GrCollisionNodeRenderKind.Flag00004000: return mat.Flag00004000 ? Vector3.UnitX : Vector3.UnitZ;
                case GrCollisionNodeRenderKind.Flag00008000: return mat.Flag00008000 ? Vector3.UnitX : Vector3.UnitZ;
                case GrCollisionNodeRenderKind.Flag00010000: return mat.Flag00010000 ? Vector3.UnitX : Vector3.UnitZ;
                case GrCollisionNodeRenderKind.Flag00020000: return mat.Flag00020000 ? Vector3.UnitX : Vector3.UnitZ;
                case GrCollisionNodeRenderKind.Flag00040000: return mat.Flag00040000 ? Vector3.UnitX : Vector3.UnitZ;
                case GrCollisionNodeRenderKind.Flag00080000: return mat.Flag00080000 ? Vector3.UnitX : Vector3.UnitZ;
                case GrCollisionNodeRenderKind.Flag00100000: return mat.Flag00100000 ? Vector3.UnitX : Vector3.UnitZ;
                case GrCollisionNodeRenderKind.Flag00200000: return mat.Flag00200000 ? Vector3.UnitX : Vector3.UnitZ;
                case GrCollisionNodeRenderKind.Flag00400000: return mat.Flag00400000 ? Vector3.UnitX : Vector3.UnitZ;
                case GrCollisionNodeRenderKind.Flag00800000: return mat.Flag00800000 ? Vector3.UnitX : Vector3.UnitZ;
            }

            return Vector3.One;

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

            //GL.Clear(ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.CullFace);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);

            Vector4[] colors = new Vector4[mesh.Materials.Count];
            Vector4[] colors_sel = new Vector4[mesh.Materials.Count];
            for (int i = 0; i < colors.Length; i++)
            {
                var c = GetMaterialColor(mesh.Materials[i]);
                colors[i] = new Vector4(c, 1.0f) * Settings.CollisionOpacity;
                colors_sel[i] = new Vector4(c, 1.0f) * Settings.CollisionSelectedOpacity;
            }

            GL.Begin(PrimitiveType.Triangles);
            foreach (var t in mesh.Triangles)
            {
                if (t.Material < 0 || t.Material >= mesh.Materials.Count)
                    continue;

                var color = is_selected ? colors_sel[t.Material] : colors[t.Material];

                GL.Color4(color);
                foreach (var i in t.Indices)
                {
                    var p = mesh.Vertices[i];
                    if (p.Count != 3) 
                        continue;
                    GL.Vertex3(p[0], p[1], p[2]);
                }
            }
            GL.End();

            if (DrawWireframe)
            {
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
            }

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
                GL.Color4(1f, 1f, 0f, Settings.ZonesSelectedOpacity);
            else
                GL.Color4(1f, 1f, 1f, Settings.ZonesOpacity);
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


            if (DrawWireframe)
            {
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
            }


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
        public static Matrix4 CreateTransform(
            Vector3 position,
            Vector3 forward,
            Vector3 up)
        {
            forward = Vector3.Normalize(forward);

            // Make up perpendicular to forward
            Vector3 right = Vector3.Normalize(Vector3.Cross(up, forward));
            up = Vector3.Cross(forward, right);

            return new Matrix4(
                right.X, right.Y, right.Z, 0.0f,
                up.X, up.Y, up.Z, 0.0f,
                forward.X, forward.Y, forward.Z, 0.0f,
                position.X, position.Y, position.Z, 1.0f);
        }

        public static Vector3 ToTkVector(KdVector vec)
        {
            return new Vector3(vec.X, vec.Y, vec.Z);
        }

        public void DrawKdPosition(
            KdPosition mesh, 
            bool is_selected, 
            object selected_object, 
            Vector3 colorX,
            Vector3 colorY,
            Vector3 colorZ)
        {
            float s = Settings.PositionRadius;

            var m = CreateTransform(ToTkVector(mesh.Position), ToTkVector(mesh.Forward), ToTkVector(mesh.Up));

            var alpha = is_selected ? Settings.PositionSelectedOpacity : Settings.PositionOpacity;

            float width = is_selected ? 8 : 4;

            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.MultMatrix(ref m);

            GL.LineWidth(width);

            GL.Begin(PrimitiveType.Lines);

            GL.Color4(colorX.X, colorX.Y, colorX.Z, alpha);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(s, 0, 0);

            GL.Color4(colorY.X, colorY.Y, colorY.Z, alpha);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, s, 0);

            GL.Color4(colorZ.X, colorZ.Y, colorZ.Z, alpha);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, s);

            GL.End();

            GL.PopMatrix();
            GL.PopAttrib();
        }

        public static void DrawBoxOutline(
            Vector3 p1,
            Vector3 p2,
            Vector3 forward,
            Vector3 up,
            Vector4 color)
        {
            Vector3 center = (p1 + p2) * 0.5f;
            Vector3 half = (p2 - p1) * 0.5f;

            forward = Vector3.Normalize(forward);
            up = Vector3.Normalize(up);

            Vector3 right = Vector3.Normalize(
                Vector3.Cross(up, forward));

            up = Vector3.Normalize(
                Vector3.Cross(forward, right));

            Vector3[] c = new Vector3[8];

            // Bottom
            c[0] = center - right * half.X - up * half.Y - forward * half.Z;
            c[1] = center + right * half.X - up * half.Y - forward * half.Z;
            c[2] = center + right * half.X + up * half.Y - forward * half.Z;
            c[3] = center - right * half.X + up * half.Y - forward * half.Z;

            // Top
            c[4] = center - right * half.X - up * half.Y + forward * half.Z;
            c[5] = center + right * half.X - up * half.Y + forward * half.Z;
            c[6] = center + right * half.X + up * half.Y + forward * half.Z;
            c[7] = center - right * half.X + up * half.Y + forward * half.Z;

            GL.Color4(color);

            GL.Begin(PrimitiveType.Lines);

            // Bottom
            GL.Vertex3(c[0]);
            GL.Vertex3(c[1]);

            GL.Vertex3(c[1]);
            GL.Vertex3(c[2]);

            GL.Vertex3(c[2]);
            GL.Vertex3(c[3]);

            GL.Vertex3(c[3]);
            GL.Vertex3(c[0]);

            // Top
            GL.Vertex3(c[4]);
            GL.Vertex3(c[5]);

            GL.Vertex3(c[5]);
            GL.Vertex3(c[6]);

            GL.Vertex3(c[6]);
            GL.Vertex3(c[7]);

            GL.Vertex3(c[7]);
            GL.Vertex3(c[4]);

            // Vertical edges
            GL.Vertex3(c[0]);
            GL.Vertex3(c[4]);

            GL.Vertex3(c[1]);
            GL.Vertex3(c[5]);

            GL.Vertex3(c[2]);
            GL.Vertex3(c[6]);

            GL.Vertex3(c[3]);
            GL.Vertex3(c[7]);

            GL.End();
        }

        public static void DrawBox(
            Vector3 p1,
            Vector3 p2,
            Vector3 forward,
            Vector3 up,
            Vector4 color)
        {
            Vector3 center = (p1 + p2) * 0.5f;
            Vector3 half = (p2 - p1) * 0.5f;

            forward = Vector3.Normalize(forward);
            up = Vector3.Normalize(up);

            // Build the third axis.
            Vector3 right = Vector3.Normalize(
                Vector3.Cross(up, forward));

            // Re-orthogonalize up.
            up = Vector3.Normalize(
                Vector3.Cross(forward, right));

            // Local box corners.
            Vector3[] corners =
            {
                -right * half.X - up * half.Y - forward * half.Z,
                 right * half.X - up * half.Y - forward * half.Z,
                 right * half.X + up * half.Y - forward * half.Z,
                -right * half.X + up * half.Y - forward * half.Z,

                -right * half.X - up * half.Y + forward * half.Z,
                 right * half.X - up * half.Y + forward * half.Z,
                 right * half.X + up * half.Y + forward * half.Z,
                -right * half.X + up * half.Y + forward * half.Z,
            };

            for (int i = 0; i < corners.Length; i++)
                corners[i] += center;

            GL.Color4(color);

            GL.Begin(PrimitiveType.Quads);

            // -Z
            GL.Vertex3(corners[0]);
            GL.Vertex3(corners[1]);
            GL.Vertex3(corners[2]);
            GL.Vertex3(corners[3]);

            // +Z
            GL.Vertex3(corners[5]);
            GL.Vertex3(corners[4]);
            GL.Vertex3(corners[7]);
            GL.Vertex3(corners[6]);

            // -Y
            GL.Vertex3(corners[0]);
            GL.Vertex3(corners[4]);
            GL.Vertex3(corners[5]);
            GL.Vertex3(corners[1]);

            // +Y
            GL.Vertex3(corners[3]);
            GL.Vertex3(corners[2]);
            GL.Vertex3(corners[6]);
            GL.Vertex3(corners[7]);

            // -X
            GL.Vertex3(corners[4]);
            GL.Vertex3(corners[0]);
            GL.Vertex3(corners[3]);
            GL.Vertex3(corners[7]);

            // +X
            GL.Vertex3(corners[1]);
            GL.Vertex3(corners[5]);
            GL.Vertex3(corners[6]);
            GL.Vertex3(corners[2]);

            GL.End();
        }

        internal void DrawKdPositionArea(KdPositionArea p, bool isSelected, object selected_object, Vector3 color)
        {
            var alpha = isSelected ? Settings.PositionAreaSelectedOpacity : Settings.PositionAreaOpacity;

            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.CullFace);

            var p1 = ToTkVector(p.P1);
            var p2 = ToTkVector(p.P2);
            var forward = ToTkVector(p.Forward).Normalized();
            Vector3 up = Vector3.UnitY;

            DrawBox(p1, p2, forward, up, new Vector4(color, alpha));

            GL.PopMatrix();
            GL.PopAttrib();
        }

        public void DrawKdPositionAreaOverlay(KdPositionArea p, object selected_object)
        {
            var p1 = ToTkVector(p.P1);
            var p2 = ToTkVector(p.P2);
            var forward = ToTkVector(p.Forward).Normalized();
            Vector3 up = Vector3.UnitY;
            DrawBoxOutline(p1, p2, forward, up, Vector4.One);
        }

        public void Dispose()
        {
            TextRenderer.Dispose();
        }
    }
}
