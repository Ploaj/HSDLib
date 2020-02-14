using System.IO;
using CSCore.Streams.Effects;
using CSCore.Codecs.WAV;
using CSCore;
using System.Media;

namespace HSDRawViewer.Sound
{
    /// <summary>
    /// 
    /// </summary>
    public class DSPPlayer
    {

        private static SoundPlayer SoundPlayer;

        private static void FixHeader(ref byte[] wavFile)
        {
            // is this a bug?
            var length = wavFile.Length - 0x2C;
            wavFile[0x28] = (byte)(length & 0xFF);
            wavFile[0x29] = (byte)((length >> 8) & 0xFF);
            wavFile[0x2A] = (byte)((length >> 16) & 0xFF);
            wavFile[0x2B] = (byte)((length >> 24) & 0xFF);

            length = wavFile.Length - 0x08;
            wavFile[0x04] = (byte)(length & 0xFF);
            wavFile[0x05] = (byte)((length >> 8) & 0xFF);
            wavFile[0x06] = (byte)((length >> 16) & 0xFF);
            wavFile[0x07] = (byte)((length >> 24) & 0xFF);
        }

        private static byte[] MixWAVEPitch(byte[] wavFile, float pitch)
        {
            using (MemoryStream wavstream = new MemoryStream(wavFile))
            {
                using (IWaveSource r = new WaveFileReader(wavstream))
                {
                    PitchShifter ps = new PitchShifter(r.ToSampleSource());
                    ps.PitchShiftFactor = pitch;

                    using (MemoryStream streamout = new MemoryStream())
                    {
                        ps.ToWaveSource().WriteToWaveStream(streamout);
                        wavFile = streamout.ToArray();
                    }
                }
            }

            FixHeader(ref wavFile);

            return wavFile;
        }

        private static byte[] MixWAVEReverb(byte[] wavFile, float reverb)
        {
            //Contains the sound to play
            using (MemoryStream wavstream = new MemoryStream(wavFile))
            {
                using (IWaveSource r = new WaveFileReader(wavstream))
                {
                    using (MemoryStream streamout = new MemoryStream())
                    {
                        using (WaveWriter w = new WaveWriter(streamout, r.WaveFormat))
                        {
                            var effect = new DmoWavesReverbEffect(r);
                            effect.ReverbMix = -96 / 2 + (96 / 2 * reverb);
                            var buffer = new byte[r.WaveFormat.BytesPerSecond];
                            int read = 0;
                            while ((read = effect.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                w.Write(buffer, 0, read);
                            }
                            wavFile = streamout.ToArray();
                        }
                    }
                }
            }

            FixHeader(ref wavFile);

            return wavFile;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dsp"></param>
        public static void PlayDSP(DSP dsp, float reverb = 0, float pitch = 0)
        {
            if (dsp == null)
                return;

            byte[] wavFile = dsp.ToWAVE();

            // TODO:
            //if(pitch != 0)
            //    wavFile = MixWAVEPitch(wavFile, pitch);
            //if(reverb != 0)
            //    wavFile = MixWAVEReverb(wavFile, reverb);

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
            var stream = new MemoryStream();
            stream.Write(wavFile, 0, wavFile.Length);
            stream.Position = 0;
            SoundPlayer = new SoundPlayer(stream);

            // Play.
            SoundPlayer.Play();
        }
        

        public static void Stop()
        {
            if (SoundPlayer != null)
                SoundPlayer.Stop();
        }
        
    }
}
