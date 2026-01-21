using OpenTK.Mathematics;
using System.Text.Json.Serialization;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    internal class KdBone
    {

        [JsonPropertyName("parent")]
        public int Parent { get; set; } = -1;

        [JsonPropertyName("transform")]
        public float[] Transform { get; set; } = null;

        public Matrix4 ToMatrix()
        {
            return new Matrix4(
                Transform[0], Transform[1], Transform[2], Transform[3],
                Transform[4], Transform[5], Transform[6], Transform[7],
                Transform[8], Transform[9], Transform[10], Transform[11],
                Transform[12], Transform[13], Transform[14], Transform[15]
                );
        }
    }
}
