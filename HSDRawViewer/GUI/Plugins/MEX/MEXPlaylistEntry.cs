using System.ComponentModel;

namespace HSDRawViewer.GUI.Plugins.MEX
{
    public class MEXPlaylistEntry
    {
        [DisplayName("Music ID:"), TypeConverter(typeof(MusicIDConverter))]
        public int MusicID { get; set; }

        [Browsable(false)]
        public short PlayChanceValue { get => playChance; }

        [DisplayName("Play Chance")]
        public string PlayChance {
            get
            {
                return playChance.ToString() + "%";
            }
            set
            {
                short v;
                if(short.TryParse(value.Replace("%", ""), out v))
                {
                    if (v > 100)
                        v = 100;
                    if (v < 0)
                        v = 0;
                    playChance = v;
                }
            }
        }
        
        private short playChance;

        public override string ToString()
        {
            return MEXConverter.musicIDValues[MusicID] + " - " + PlayChance;
        }
    }
}
