using System.Text.Json.Serialization;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    public class KdZoneTriangle
    {
        public int Index { get; set; }

        public int Flags { get; set; }


        [JsonPropertyName("v")]
        public int[] Indices { get; set; }

        public KdZoneTriangle()
        {
        }

        public KdZoneTriangle(int index, int flags, int[] indices)
        {
            Index = index;
            Flags = flags;
            Indices = indices;
        }
    }
}
