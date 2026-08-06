using OpenTK.Mathematics;
using System.Collections.Generic;
using System.ComponentModel;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool.Converters
{
    public class EdgeAccessor
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public VertexAccessor Vertex1 { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public VertexAccessor Vertex2 { get; set; }

        public EdgeAccessor(List<float> v1, List<float> v2)
        {
            Vertex1 = new VertexAccessor(v1);
            Vertex2 = new VertexAccessor(v2);
        }

        public Vector3 MidPoint
        {
            get => (new Vector3(Vertex1.X, Vertex1.Y, Vertex1.Z) +
                    new Vector3(Vertex2.X, Vertex2.Y, Vertex2.Z)) * 0.5f;
        }

        public void SetMidpoint(Vector3 midpoint)
        {
            Vector3 offset = midpoint - MidPoint;

            Vertex1.X += offset.X;
            Vertex1.Y += offset.Y;
            Vertex1.Z += offset.Z;

            Vertex2.X += offset.X;
            Vertex2.Y += offset.Y;
            Vertex2.Z += offset.Z;
        }
    }
}
