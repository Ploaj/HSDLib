using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    internal class KdZone
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("parent")]
        public int Parent { get; set; } = -1;

        [JsonPropertyName("vertices")]
        public List<List<float>> Vertices { get; set; }

        [JsonPropertyName("triangles")]
        public List<KdZoneTriangle> Triangles { get; set; }

        // param type 1

        // param type 2

        // matrix ??
    }
}
