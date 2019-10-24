using HSDRaw;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using VGAudio.Codecs.GcAdpcm;

namespace HSDRawViewer.GUI.Extra
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

        public int LoopStart = 0;
        public int NibbleCount = 0;

        public override string ToString()
        {
            return "Channel";
        }
    }

    public class DSP
    {
        public int Frequency { get; set; }

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

        public BindingList<DSPChannel> Channels = new BindingList<DSPChannel>();

        /*public void FromHPS(byte[] data)
        {
            using (BinaryReaderExt r = new BinaryReaderExt(new MemoryStream(data)))
            {
                r.BigEndian = true;

                if (new string(r.ReadChars(7)) != " HALPST")
                    throw new NotSupportedException("Invalid HPS file");
                r.ReadByte();

                Frequency = r.ReadInt32();

                var channelCount = r.ReadInt32();

                for(int i = 0; i < channelCount; i++)
                {
                    var channel = new DSPChannel();

                    r.PrintPosition();

                    channel.LoopFlag = r.ReadInt16();
                    channel.Format = r.ReadInt16();
                    channel.SA = r.ReadInt32();
                    channel.EA = r.ReadInt32();
                    channel.CA = r.ReadInt32();
                    for (int k = 0; k < 0x10; k++)
                        channel.COEF[k] = r.ReadInt16();
                    channel.Gain = r.ReadInt16();
                    channel.Loopps = r.ReadInt16();
                    channel.Loopsh1 = r.ReadInt16();
                    channel.Loopsh2 = r.ReadInt16();
                    
                    Console.WriteLine((0x80 + (uint)Math.Ceiling(channel.SA / 2d) + 1).ToString("X") + " " + channel.NibbleCount.ToString("X"));
                    var dataOffset = 0x80 + (uint)Math.Ceiling(channel.SA / 2d) + 1;

                    channel.NibbleCount = channel.EA - channel.SA;
                    channel.Data = r.GetSection(0x80, 16 * 8000);

                    Console.WriteLine(channel.Data.Length + " " + channel.NibbleCount * 2);

                    Channels.Add(channel);
                    break;
                }
            }
        }

        public void ToHPS()
        {

        }*/

        public void FromWAVE(byte[] wavFile)
        {
            if (wavFile.Length < 0x2C)
                throw new NotSupportedException("File is not a valid WAVE file");

            using (BinaryReader r = new BinaryReader(new MemoryStream(wavFile)))
            {
                if (new string(r.ReadChars(4)) != "RIFF")
                    throw new NotSupportedException("File is not a valid WAVE file");

                r.BaseStream.Position = 0x14;
                var comp = r.ReadInt16();
                var channelCount = r.ReadInt16();
                Frequency = r.ReadInt32();
                r.ReadInt32();// block rate
                r.ReadInt16();// block align
                var bpp = r.ReadInt16();

                if (comp != 1)
                    throw new NotSupportedException("Compressed WAVE files not supported");

                if (bpp != 16)
                    throw new NotSupportedException("Only 16 bit WAVE formats accepted");

                r.BaseStream.Position = 0x28;
                var channelSizes = r.ReadInt32() / channelCount / 2;

                List<List<short>> channels = new List<List<short>>();

                for (int i = 0; i < channelCount; i++)
                    channels.Add(new List<short>());

                foreach (var v in channels)
                    for (int i = 0; i < channelSizes; i++)
                        v.Add(r.ReadInt16());

                Channels.Clear();
                foreach (var data in channels)
                {
                    var c = new DSPChannel();

                    var ss = data.ToArray();

                    c.COEF = GcAdpcmCoefficients.CalculateCoefficients(ss);

                    c.Data = GcAdpcmEncoder.Encode(ss, c.COEF);

                    c.NibbleCount = c.Data.Length * 2;

                    c.InitialPredictorScale = c.Data[0];

                    Channels.Add(c);
                }

            }
        }

        public byte[] ToWAVE()
        {
            var stream = new MemoryStream();
            using (BinaryWriter w = new BinaryWriter(stream))
            {
                w.Write("RIFF".ToCharArray());
                w.Write(0); // wave size

                w.Write("WAVE".ToCharArray());

                short BitsPerSample = 16;
                var byteRate = Frequency * Channels.Count * BitsPerSample / 8;
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
                var subchunkOffset = w.BaseStream.Position;
                w.Write(0);

                int subChunkSize = 0;
                if (Channels.Count == 1)
                {
                    short[] sound_data = GcAdpcmDecoder.Decode(Channels[0].Data, Channels[0].COEF);
                    subChunkSize += sound_data.Length * 2;
                    foreach (var s in sound_data)
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

        public override string ToString()
        {
            return $"DSP : Channels {Channels.Count} : Frequency {Frequency}Hz";
        }
    }
}
