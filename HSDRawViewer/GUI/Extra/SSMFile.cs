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
        public int SA;
        public int EA;
        public int CA;
        public short[] COEF = new short[0x10];
        public short Gain { get; set; }
        public short InitPs { get; set; }
        public short InitSh1 { get; set; }
        public short InitSh2 { get; set; }
        public short Loopps { get; set; }
        public short Loopsh1 { get; set; }
        public short Loopsh2 { get; set; }

        public byte[] Data;

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
                    c.Data = Tools.DSPEncoder.EncodeDSP(data.ToArray(), out c.COEF);
                    c.NibbleCount = c.Data.Length * 2;
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
