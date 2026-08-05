using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    public class KdMesh
    {
        [Browsable(false)]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [DisplayName("Parent Bone")]
        [JsonPropertyName("parent")]
        public int Parent { get; set; } = -1;

        [JsonPropertyName("vertices")]
        public List<List<float>> Vertices { get; set; } = new List<List<float>>();

        [JsonPropertyName("triangles")]
        public List<KdTriangle> Triangles { get; set; } = new List<KdTriangle>();

        [JsonPropertyName("materials")]
        public List<KdMaterial> Materials { get; set; } = new List<KdMaterial>();
    }
}
