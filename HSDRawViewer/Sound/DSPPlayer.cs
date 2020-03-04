using CSCore.SoundOut;
using System.IO;
using CSCore;
using CSCore.Codecs.WAV;
using CSCore.CoreAudioAPI;
using System.Collections.Generic;
using System;
using CSCore.Streams.Effects;

namespace HSDRawViewer.Sound
{
    /// <summary>
    /// 
    /// </summary>
    public class DSPPlayer : IDisposable
    {
        private ISoundOut _soundOut;
        private IWaveSource _waveSource;
        private MemoryStream _memoryStream;

        public event EventHandler<PlaybackStoppedEventArgs> PlaybackStopped;

        public TimeSpan Position
        {
            get
            {
                if (_waveSource != null)
                    return _waveSource.GetPosition();
                return TimeSpan.Zero;
            }
            set
            {
                if (_waveSource != null && value < _waveSource.GetLength())
                    _waveSource.SetPosition(value);
            }
        }

        public TimeSpan Length
        {
            get
            {
                if (_waveSource != null)
                    return _waveSource.GetLength();
                return TimeSpan.Zero;
            }
        }
        
        public DSPPlayer()
        {

        }

        ~DSPPlayer()
        {
            Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        private void InitPlayback(IWaveSource src)
        {
            CleanupPlayback();

            _waveSource = src;

            _soundOut = new WasapiOut() { Latency = 100, Device = ApplicationSettings.DefaultDevice };
            _soundOut.Initialize(_waveSource);

            if (PlaybackStopped != null) _soundOut.Stopped += PlaybackStopped;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reverb"></param>
        public void ApplyReverb(float reverb)
        {
            if (_waveSource == null)
                return;

            if (reverb > 96)
                reverb = 96;

            var reverbPass = new DmoWavesReverbEffect(_waveSource)
            {
                ReverbTime = 1000,
                ReverbMix = -96 + reverb
            };

            InitPlayback(reverbPass);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reverb"></param>
        public void ApplyPitch(float pitch)
        {
            if (_waveSource == null)
                return;

            var pitchPass = new PitchShifter(_waveSource.ToSampleSource());

            pitchPass.PitchShiftFactor = pitch;

            InitPlayback(pitchPass.ToWaveSource());
        }

        /// <summary>
        /// Loads DSP for playback
        /// </summary>
        /// <param name="dsp"></param>
        public void LoadDSP(DSP dsp, float reverb = -1, float pitch = -1)
        {
            if (dsp == null)
                return;

            var data = dsp.ToWAVE();

            CleanUpSource();

            _memoryStream = new MemoryStream(data);
            var waveSource = new WaveFileReader(_memoryStream);

            InitPlayback(waveSource);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static ISoundOut GetSoundOut()
        {
            if (WasapiOut.IsSupportedOnCurrentPlatform)
                return new WasapiOut();
            else
                return new DirectSoundOut();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dsp"></param>
        public void Play()
        {
            if (_soundOut != null)
            {
                _soundOut.Play();
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            if (_soundOut != null)
            {
                _waveSource.SetPosition(TimeSpan.Zero);
                _soundOut.Stop();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Pause()
        {
            if (_soundOut != null)
                _soundOut.Pause();
        }

        /// <summary>
        /// 
        /// </summary>
        private void CleanupPlayback()
        {
            if (_soundOut != null)
            {
                _soundOut.Dispose();
                _soundOut = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CleanUpSource()
        {
            if (_memoryStream != null)
            {
                _memoryStream.Dispose();
                _memoryStream = null;
            }
            if (_waveSource != null)
            {
                _waveSource.Dispose();
                _waveSource = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            CleanUpSource();
            CleanupPlayback();
        }

        
    }
}
