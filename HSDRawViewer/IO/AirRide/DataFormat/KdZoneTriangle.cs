using System.Text.Json.Serialization;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    internal class KdZoneTriangle
    {
        [JsonPropertyName("v")]
        public int[] Indices { get; set; }
    }
}
