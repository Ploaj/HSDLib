using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    public class KdZone
    {
        [Browsable(false)]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("flags")]
        public uint Flags { get; set; }

        [DisplayName("Parent Joint")]
        [JsonPropertyName("parent")]
        public int Parent { get; set; } = -1;

        [Browsable(false)]
        [JsonPropertyName("vertices")]
        public List<List<float>> Vertices { get; set; } = new List<List<float>>();

        [Browsable(false)]
        [JsonPropertyName("triangles")]
        public List<KdZoneTriangle> Triangles { get; set; } = new List<KdZoneTriangle>();

        [DisplayName("Linked Zone Index")]
        [Description("Specifies the index of the destination or connected zone. Used by warps and connected movement zones.")]
        public int LinkedZone { get; set; }

        // param type 2
        public int Type2 { get; set; }

        public object Type2Data { get; set; }

        // matrix ??

        public float[] Matrix { get; set; } = new float[12];
    }
}
