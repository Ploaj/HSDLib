﻿using CSCore;
using CSCore.Codecs.MP3;
using HSDRaw;
using System;
using System.Collections.Generic;
using System.IO;
using VGAudio.Codecs.GcAdpcm;

namespace HSDRawViewer.Sound
{
    public class DSPChannel
    {
        public short LoopFlag { get; set; }
        public short Format { get; set; }
        public short[] COEF = new short[0x10];
        public short Gain { get; set; }
        public short InitialPredictorScale { get; set; }
        public short InitialSampleHistory1 { get; set; }
        public short InitialSampleHistory2 { get; set; }
        public short LoopPredictorScale { get; set; }
        public short LoopSampleHistory1 { get; set; }
        public short LoopSampleHistory2 { get; set; }

        public byte[] Data;

        public int LoopStart { get; set; } = 0;
        public int NibbleCount = 0;

        public override string ToString()
        {
            return "Channel";
        }
    }

    public class DSP
    {
        public static string SupportedImportFilter { get; } = "Supported (*.dsp*.wav*.hps*.mp3*.aiff*.wma*.m4a)|*.dsp;*.wav;*.hps;*.mp3;*.aiff;*.wma;*.m4a";

        public static string SupportedExportFilter { get; } = "Supported Types(*.wav*.dsp*.hps)|*.wav;*.dsp;*.hps";

        public int Frequency { get; set; }

        public string LoopPoint
        {
            get
            {
                if (Channels.Count == 0)
                    return "0:00";
                int sec = (int)Math.Ceiling(Channels[0].LoopStart / 2 / (double)Frequency * 1.75f * 1000);
                return TimeSpan.FromMilliseconds(sec).ToString();
            }
            set
            {
                if (TimeSpan.TryParse(value, out TimeSpan ts))
                {
                    SetLoopFromTimeSpan(ts);
                }
            }
        }

        public string Length
        {
            get
            {
                if (Channels.Count == 0)
                    return "0:00";
                int sec = (int)Math.Ceiling(Channels[0].Data.Length / (double)Frequency * 1.75f * 1000);
                return TimeSpan.FromMilliseconds(sec).ToString();
            }
        }

        public string ChannelType
        {
            get
            {
                if (Channels == null)
                    return "";
                if (Channels.Count == 1)
                    return "Mono";
                if (Channels.Count == 2)
                    return "Stereo";
                return "";
            }
        }

        public List<DSPChannel> Channels = new();

        public void SetLoopFromTimeSpan(TimeSpan s)
        {
            double sec = (s.TotalMilliseconds / 1.75f / 1000f) * 2 * Frequency;

            foreach (DSPChannel c in Channels)
                c.LoopStart = (int)sec;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public void FromFile(string filePath)
        {
            FromFormat(File.ReadAllBytes(filePath), Path.GetExtension(filePath));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="format"></param>
        public void FromFormat(byte[] data, string format)
        {
            format = format.Replace(".", "").ToLower();

            switch (format)
            {
                case "wav":
                    FromWAVE(data);
                    break;
                case "dsp":
                    FromDSP(data);
                    break;
                case "mp3":
                    FromMP3(data);
                    break;
                case "aiff":
                    FromAIFF(data);
                    break;
                case "wma":
                    FromWMA(data);
                    break;
                case "m4a":
                    FromM4A(data);
                    break;
                case "hps":
                    FromHPS(data);
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public void ExportFormat(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLower();

            switch (ext)
            {
                case ".wav":
                    File.WriteAllBytes(filePath, ToWAVE());
                    break;
                case ".dsp":
                    ExportDSP(filePath);
                    break;
                case ".hps":
                    HPS.SaveDSPAsHPS(this, filePath);
                    break;
            }
        }

        #region DSP

        private void FromDSP(byte[] data)
        {
            Channels.Clear();
            using BinaryReaderExt r = new(new MemoryStream(data));
            r.BigEndian = true;

            r.ReadInt32();
            int nibbleCount = r.ReadInt32();
            Frequency = r.ReadInt32();

            DSPChannel channel = new();

            channel.LoopFlag = r.ReadInt16();
            channel.Format = r.ReadInt16();
            int LoopStartOffset = r.ReadInt32();
            int LoopEndOffset = r.ReadInt32();
            int CurrentAddress = r.ReadInt32();
            for (int k = 0; k < 0x10; k++)
                channel.COEF[k] = r.ReadInt16();
            channel.Gain = r.ReadInt16();
            channel.InitialPredictorScale = r.ReadInt16();
            channel.InitialSampleHistory1 = r.ReadInt16();
            channel.InitialSampleHistory2 = r.ReadInt16();
            channel.LoopPredictorScale = r.ReadInt16();
            channel.LoopSampleHistory1 = r.ReadInt16();
            channel.LoopSampleHistory2 = r.ReadInt16();
            r.ReadInt16(); //  padding

            r.Seek(0x60);
            channel.NibbleCount = nibbleCount;
            channel.LoopStart = LoopStartOffset - CurrentAddress;
            channel.Data = r.ReadBytes((int)Math.Ceiling(nibbleCount / 2d));

            Channels.Add(channel);

            r.BaseStream.Close();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        private void ExportDSP(string filePath)
        {
            if (Channels.Count == 1)
            {
                // mono
                ExportDSPChannel(filePath, Channels[0]);
            }
            else
            {
                // stereo or more
                string head = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath);
                string ext = Path.GetExtension(filePath);

                for (int i = 0; i < Channels.Count; i++)
                {

                    ExportDSPChannel(head + $"_channel_{i}" + ext, Channels[i]);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="channel"></param>
        private void ExportDSPChannel(string filePath, DSPChannel channel)
        {
            using BinaryWriterExt w = new(new FileStream(filePath, FileMode.Create));
            w.BigEndian = true;

            int samples = channel.NibbleCount * 7 / 8;

            w.Write(samples);
            w.Write(channel.NibbleCount);
            w.Write(Frequency);

            w.Write(channel.LoopFlag);
            w.Write(channel.Format);
            w.Write(2);
            w.Write(channel.NibbleCount - 2);
            w.Write(2);
            foreach (short v in channel.COEF)
                w.Write(v);
            w.Write(channel.Gain);
            w.Write(channel.InitialPredictorScale);
            w.Write(channel.InitialSampleHistory1);
            w.Write(channel.InitialSampleHistory2);
            w.Write(channel.LoopPredictorScale);
            w.Write(channel.LoopSampleHistory1);
            w.Write(channel.LoopSampleHistory2);
            w.Write((short)0);

            w.Write(new byte[0x14]);

            w.Write(channel.Data);

            if (w.BaseStream.Position % 0x8 != 0)
                w.Write(new byte[0x08 - w.BaseStream.Position % 0x08]);

            w.BaseStream.Close();
        }

        #endregion

        #region HPS

        private void FromHPS(byte[] data)
        {
            DSP dsp = HPS.ToDSP(data);
            Channels = dsp.Channels;
            Frequency = dsp.Frequency;
        }

        /*public void ToHPS()
        {

        }*/

        #endregion

        #region WAVE

        private void FromWAVE(byte[] wavFile)
        {
            if (wavFile.Length < 0x2C)
                throw new NotSupportedException("File is not a valid WAVE file");

            Channels.Clear();

            using BinaryReader r = new(new MemoryStream(wavFile));
            if (new string(r.ReadChars(4)) != "RIFF")
                throw new NotSupportedException("File is not a valid WAVE file");

            r.BaseStream.Position = 0x14;
            short comp = r.ReadInt16();
            short channelCount = r.ReadInt16();
            Frequency = r.ReadInt32();
            r.ReadInt32();// block rate
            r.ReadInt16();// block align
            short bpp = r.ReadInt16();

            if (comp != 1)
                throw new NotSupportedException("Compressed WAVE files not supported");

            if (bpp != 16)
                throw new NotSupportedException("Only 16 bit WAVE formats accepted");

            while (r.ReadByte() == 0) ;
            r.BaseStream.Seek(-1, SeekOrigin.Current);

            while (new string(r.ReadChars(4)) != "data")
            {
                int skip = r.ReadInt32();
                r.BaseStream.Position += skip;
            }

            int channelSizes = r.ReadInt32() / channelCount / 2;

            List<List<short>> channels = new();

            for (int i = 0; i < channelCount; i++)
                channels.Add(new List<short>());

            for (int i = 0; i < channelSizes; i++)
            {
                foreach (List<short> v in channels)
                {
                    v.Add(r.ReadInt16());
                }
            }

            Channels.Clear();
            foreach (List<short> data in channels)
            {
                DSPChannel c = new();

                short[] ss = data.ToArray();

                c.COEF = GcAdpcmCoefficients.CalculateCoefficients(ss);

                c.Data = GcAdpcmEncoder.Encode(ss, c.COEF);

                c.NibbleCount = c.Data.Length * 2;

                c.InitialPredictorScale = c.Data[0];

                Channels.Add(c);
            }
        }

        /// <summary>
        /// Wraps the decoded DSP data into a WAVE file stored as a byte array
        /// </summary>
        /// <returns></returns>
        public byte[] ToWAVE()
        {
            using MemoryStream stream = new();
            using (BinaryWriter w = new(stream))
            {
                w.Write("RIFF".ToCharArray());
                w.Write(0); // wave size

                w.Write("WAVE".ToCharArray());

                short BitsPerSample = 16;
                int byteRate = Frequency * Channels.Count * BitsPerSample / 8;
                short blockAlign = (short)(Channels.Count * BitsPerSample / 8);

                w.Write("fmt ".ToCharArray());
                w.Write(16); // chunk size
                w.Write((short)1); // compression
                w.Write((short)Channels.Count);
                w.Write(Frequency);
                w.Write(byteRate);
                w.Write(blockAlign);
                w.Write(BitsPerSample);

                w.Write("data".ToCharArray());
                long subchunkOffset = w.BaseStream.Position;
                w.Write(0);

                int subChunkSize = 0;
                if (Channels.Count == 1)
                {
                    short[] sound_data = GcAdpcmDecoder.Decode(Channels[0].Data, Channels[0].COEF);
                    subChunkSize += sound_data.Length * 2;
                    foreach (short s in sound_data)
                        w.Write(s);
                }
                if (Channels.Count == 2)
                {
                    short[] sound_data1 = GcAdpcmDecoder.Decode(Channels[0].Data, Channels[0].COEF);
                    short[] sound_data2 = GcAdpcmDecoder.Decode(Channels[1].Data, Channels[1].COEF);
                    subChunkSize += (sound_data1.Length + sound_data2.Length) * 2;
                    for (int i = 0; i < sound_data1.Length; i++)
                    {
                        w.Write(sound_data1[i]);
                        w.Write(sound_data2[i]);
                    }
                }

                w.BaseStream.Position = subchunkOffset;
                w.Write(subChunkSize);

                w.BaseStream.Position = 4;
                w.Write((int)(w.BaseStream.Length - 8));
            }
            return stream.ToArray();
        }

        #endregion

        #region Extra Formats

        private void FromMP3(byte[] data)
        {
            using MemoryStream s = new(data);
            using IWaveSource soundSource = new DmoMp3Decoder(s);
            using MemoryStream w = new();
            soundSource.WriteToWaveStream(w);
            FromWAVE(w.ToArray());
        }

        private void FromM4A(byte[] data)
        {
            using MemoryStream s = new(data);
            using IWaveSource soundSource = new CSCore.Codecs.DDP.DDPDecoder(s);
            using MemoryStream w = new();
            soundSource.WriteToWaveStream(w);
            FromWAVE(w.ToArray());
        }

        private void FromWMA(byte[] data)
        {
            using MemoryStream s = new(data);
            using IWaveSource soundSource = new CSCore.Codecs.WMA.WmaDecoder(s);
            using MemoryStream w = new();
            soundSource.WriteToWaveStream(w);
            FromWAVE(w.ToArray());
        }

        private void FromAIFF(byte[] data)
        {
            using MemoryStream s = new(data);
            using IWaveSource soundSource = new CSCore.Codecs.AIFF.AiffReader(s);
            using MemoryStream w = new();
            soundSource.WriteToWaveStream(w);
            FromWAVE(w.ToArray());
        }

        #endregion

        //[Browsable(false)]
        // public int Index { get; set; }

        public override string ToString()
        {
            return $"Channels {Channels.Count} : Frequency {Frequency}Hz";
        }
    }
}
