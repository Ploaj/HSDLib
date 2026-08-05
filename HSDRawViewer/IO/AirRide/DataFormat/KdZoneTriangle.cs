using System.Text.Json.Serialization;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    public class KdZoneTriangle
    {
        public int UnknownIndex { get; set; }


        [JsonPropertyName("v")]
        public int[] Indices { get; set; }
    }
}
