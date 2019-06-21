using HSDLib.KAR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace HALSysDATViewer.Rendering
{
    public class GrDataRenderer
    {
        public KAR_GrData Data
        {
            set
            {
                _data = value;
            }
        }
        private KAR_GrData _data;

        public void Render()
        {
            if (_data == null)
                return;
            GL.Begin(PrimitiveType.Triangles);
            foreach(var f in _data.CollisionNode.Faces)
            {
                switch (f.Color)
                {
                    case 81:
                        GL.Color3(1f, 0, 0);
                        break;
                    case 82:
                        GL.Color3(0, 1f, 0);
                        break;
                    case 97:
                        GL.Color3(1f, 1f, 1f);
                        break;
                    case 113:
                        GL.Color3(0, 1f, 1f);
                        break;
                }

                var v1 = new Vector3(_data.CollisionNode.Vertices[f.V1].X, _data.CollisionNode.Vertices[f.V1].Y, _data.CollisionNode.Vertices[f.V1].Z);
                var v2 = new Vector3(_data.CollisionNode.Vertices[f.V2].X, _data.CollisionNode.Vertices[f.V2].Y, _data.CollisionNode.Vertices[f.V2].Z);
                var v3 = new Vector3(_data.CollisionNode.Vertices[f.V3].X, _data.CollisionNode.Vertices[f.V3].Y, _data.CollisionNode.Vertices[f.V3].Z);

                GL.Vertex3(v1);
                GL.Vertex3(v2);
                GL.Vertex3(v3);
            }
            GL.End();


            GL.LineWidth(1f);
            GL.Color3(1f, 1f, 1f);
            GL.Begin(PrimitiveType.Lines);
            foreach (var f in _data.CollisionNode.Faces)
            {
                var v1 = new Vector3(_data.CollisionNode.Vertices[f.V1].X, _data.CollisionNode.Vertices[f.V1].Y, _data.CollisionNode.Vertices[f.V1].Z);
                var v2 = new Vector3(_data.CollisionNode.Vertices[f.V2].X, _data.CollisionNode.Vertices[f.V2].Y, _data.CollisionNode.Vertices[f.V2].Z);
                var v3 = new Vector3(_data.CollisionNode.Vertices[f.V3].X, _data.CollisionNode.Vertices[f.V3].Y, _data.CollisionNode.Vertices[f.V3].Z);

                GL.Vertex3(v1);
                GL.Vertex3(v2);

                GL.Vertex3(v2);
                GL.Vertex3(v3);

                GL.Vertex3(v3);
                GL.Vertex3(v1);
            }
            GL.End();
            
            foreach (var p in _data.PartitionNode.Setup.Partitions)
            {
                    float depth = (p.Depth >> 24) / 10f;
                    DrawTools.DrawBoundingBox(new OpenTK.Vector3(p.MinX, p.MinY, p.MinZ), new OpenTK.Vector3(p.MaxX, p.MaxY, p.MaxZ), new OpenTK.Vector3(depth));
                
            }
        }
    }
}
