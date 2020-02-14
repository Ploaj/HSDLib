using System.ComponentModel;

namespace HSDRaw.Melee
{
    public class smSoundTestLoadData : HSDAccessor
    {
        [DisplayName("Sound Modes"), Description("The two sound modes in the sound test")]
        public string[] SoundModes
        {
            get => _s.GetStringArray(0x04); set => _s.SetStringArray(0x04, value, 0x00);
        }

        [DisplayName("Sound Groups"), Description("The SSM filenames")]
        public string[] SoundBankNames
        {
            get => _s.GetStringArray(0x08); set => _s.SetStringArray(0x08, value);
        }

        [DisplayName("Sound Effect Names"), Description("The names for the sfx inside of the SSM sound banks")]
        public string[] SoundNames
        {
            get => _s.GetStringArray(0x0C); set => _s.SetStringArray(0x0C, value);
        }

        [DisplayName("Sound Effect IDs"), Description("The IDs used to lookup sounds in the SSM sound banks")]
        public int[] SoundIDs
        {
            get => _s.GetInt32Array(0x14); set => _s.SetInt32Array(0x14, value, 0x10);
        }

        [DisplayName("Sound Groups Counts"), Description("The number of sounds in the SSM sound banks")]
        public int[] SoundBankCount
        {
            get => _s.GetInt32Array(0x18); set => _s.SetInt32Array(0x18, value);
        }

        [DisplayName("Music Banks"), Description("The names of the files containing music")]
        public string[] MusicBanks
        {
            get => _s.GetStringArray(0x1C); set => _s.SetStringArray(0x1C, value);
        }
    }
}
