using System.Collections.Generic;

namespace HSDRawViewer.IO.Model
{
    public class HsdAnimMaterial
    {
        public List<HsdAnimTexture> Textures { get; set; } = new List<HsdAnimTexture>();

        public List<HsdAnimTrack> Tracks { get; set; } = new List<HsdAnimTrack>();

        public HsdAnimMaterial() { }
    }
}
