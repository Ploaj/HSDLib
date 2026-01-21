using System.Collections.Generic;

namespace HSDRawViewer.IO.Model
{
    public class HsdAnimTexture
    {
        public List<HsdAnimTrack> Tracks { get; set; } = new List<HsdAnimTrack>();

        public HsdAnimTexture() { }
    }
}
