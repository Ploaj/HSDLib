using System.Text.Json.Serialization;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    public class KdTriangle
    {
        [JsonPropertyName("v")]
        public int[] Indices { get; set; }

        [JsonPropertyName("mat")]
        public int Material { get; set; }
    }
}
