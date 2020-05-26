using System.ComponentModel;

namespace HSDRawViewer.GUI.MEX
{
    public class MEXPlaylistEntry
    {
        [DisplayName("Music ID:"), TypeConverter(typeof(MusicIDConverter))]
        public int MusicID { get; set; }

        [DisplayName("Play Chance")]
        public string PlayChance {
            get
            {
                return PlayChanceValue.ToString() + "%";
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
                    PlayChanceValue = v;
                }
            }
        }
        
        public short PlayChanceValue;

        public override string ToString()
        {
            return MEXConverter.musicIDValues[MusicID] + " - " + PlayChance;
        }
    }
}
