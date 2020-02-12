using System.Media;
using System.IO;

namespace HSDRawViewer.Sound
{
    /// <summary>
    /// 
    /// </summary>
    public class DSPPlayer
    {
        private static SoundPlayer SoundPlayer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dsp"></param>
        public static void PlayDSP(DSP dsp)
        {
            if (dsp == null)
                return;

            // Stop the player if it is running.
            if (SoundPlayer != null)
            {
                SoundPlayer.Stop();
                SoundPlayer.Stream.Close();
                SoundPlayer.Stream.Dispose();
                SoundPlayer.Dispose();
                SoundPlayer = null;
            }

            // Make the new player for the WAV file.
            var wavFile = dsp.ToWAVE();
            var stream = new MemoryStream();
            stream.Write(wavFile, 0, wavFile.Length);
            stream.Position = 0;
            SoundPlayer = new SoundPlayer(stream);

            // Play.
            SoundPlayer.Play();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Stop()
        {
            if (SoundPlayer != null)
                SoundPlayer.Stop();
        }
    }
}
