using HSDRaw;
using HSDRaw.AirRide.Gr.Data;
using HSDRaw.GX;
using HSDRawViewer.Rendering.GX;
using OpenTK.Graphics.OpenGL;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.Rendering.Renderers
{
    public class RendererGrCollision
    {
        public Type[] SupportedTypes => new Type[] { typeof(KAR_grCollisionNode) };

        public ToolStrip ToolStrip => null;

        private KAR_grCollisionNode Node = null;

        // cache all of these
        private GXVector3[] Vertices;

        private KAR_CollisionTriangle[] Triangles;

        private KAR_CollisionJoint[] Joints;

        private GXVector3[] ZoneVertices;

        private KAR_ZoneCollisionTriangle[] ZoneTriangles;

        private KAR_ZoneCollisionJoint[] ZoneJoints;

        public void Clear()
        {
            Node = null;
            Vertices = null;
            Triangles = null;
            Joints = null;
            ZoneVertices = null;
            ZoneTriangles = null;
            ZoneJoints = null;
        }

        public void Render(HSDAccessor a, int windowWidth, int windowHeight)
        {
            if (a is KAR_grCollisionNode cn && cn != Node)
            {
                Node = cn;
                Vertices = Node.Vertices;
                Triangles = Node.Triangles;
                Joints = Node.Joints;

                ZoneVertices = Node.ZoneVertices;
                ZoneTriangles = Node.ZoneTriangles;
                ZoneJoints = Node.ZoneJoints;
            }

            if (Node == null)
                return;

            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Begin(PrimitiveType.Triangles);

            foreach (KAR_CollisionTriangle t in Triangles)
            {
                if (((int)t.Flags & 0x1) == 0x1)
                    GL.Color4(1f, 0f, 0f, 0.5f);
                if (((int)t.Flags & 0x2) == 0x2)
                    GL.Color4(0f, 1f, 0f, 0.5f);
                if (((int)t.Flags & 0x4) == 0x4)
                    GL.Color4(0f, 0f, 1f, 0.5f);

                GL.Vertex3(GXTranslator.toVector3(Vertices[t.V1]));
                GL.Vertex3(GXTranslator.toVector3(Vertices[t.V2]));
                GL.Vertex3(GXTranslator.toVector3(Vertices[t.V3]));
            }

            GL.End();

            GL.PopAttrib();
        }
    }
}
