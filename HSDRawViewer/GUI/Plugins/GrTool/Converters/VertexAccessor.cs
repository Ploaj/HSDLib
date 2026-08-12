using System.Collections.Generic;

namespace HSDRawViewer.GUI.Plugins.GrTool.Converters
{
    public class VertexAccessor
    {
        private List<float> vertices;

        public float X { get => GetValue(0); set => SetValue(0, value); }
        public float Y { get => GetValue(1); set => SetValue(1, value); }
        public float Z { get => GetValue(2); set => SetValue(2, value); }


        public VertexAccessor(List<float> vertices)
        {
            this.vertices = vertices;
        }

        private float GetValue(int i)
        {
            if (i < 0 || i >= vertices.Count) return 0;
            return vertices[i];
        }

        private void SetValue(int i, float value)
        {
            if (i < 0 || i >= vertices.Count) return;
            vertices[i] = value;
        }

        public override string ToString()
        {
            return $"Vertex";
        }
    }
}
