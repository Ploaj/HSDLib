using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Linq;

namespace HSDRawViewer.IO.Model
{
    public class HsdAnimJoint
    {
        public int Parent { get; set; } = -1;

        public List<HsdAnimMaterial> Materials { get; set; } = new List<HsdAnimMaterial>();

        public List<HsdAnimTrack> Tracks { get; set; } = new List<HsdAnimTrack>();

        [JsonIgnore]
        public float FrameMax => Tracks.Select(b => b.FrameMax).DefaultIfEmpty(0).Max();

        public HsdAnimJoint() { }
    }
}
