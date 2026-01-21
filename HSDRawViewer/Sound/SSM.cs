using HSDRaw;
using System;
using System.IO;

namespace HSDRawViewer.Sound
{
    public class SSM
    {
        public string Name;

        public int StartIndex { get; set; } = 0;

        public int Flag { get; set; } = 0;

        public int GroupFlags { get; set; } = 0;

        public DSP[] Sounds { get; set; } = new DSP[0];

        /// <summary>
        /// 
        /// </summary>
        public SSM()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public void AddSound(DSP value)
        {
            DSP[] ar = Sounds;
            Array.Resize(ref ar, ar.Length + 1);
            ar[ar.Length - 1] = value;
            Sounds = ar;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public void Open(string filePath)
        {
            using FileStream fs = new(filePath, FileMode.Open);
            Open(Path.GetFileName(filePath), fs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public void Open(string name, Stream s)
        {
            Name = name;
            using BinaryReaderExt r = new(s);
            r.BigEndian = true;

            int headerLength = r.ReadInt32() + 0x10;
            int dataOff = r.ReadInt32();
            int soundCount = r.ReadInt32();
            StartIndex = r.ReadInt32();

            Sounds = new DSP[soundCount];

            for (int i = 0; i < soundCount; i++)
            {
                DSP sound = new();
                int ChannelCount = r.ReadInt32();
                sound.Frequency = r.ReadInt32();

                sound.Channels.Clear();
                for (int j = 0; j < ChannelCount; j++)
                {
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

                    channel.NibbleCount = LoopEndOffset - CurrentAddress;
                    channel.LoopStart = LoopStartOffset - CurrentAddress;

                    sound.Channels.Add(channel);

                    int DataOffset = headerLength + (int)Math.Ceiling(CurrentAddress / 2d) - 1;

                    channel.Data = r.GetSection((uint)DataOffset, (int)Math.Ceiling(channel.NibbleCount / 2d) + 1);

                }

                Sounds[i] = sound;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string filePath)
        {
            Save(filePath, out int bf);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string filePath, out int bufferSize)
        {
            using FileStream stream = new(filePath, FileMode.Create);
            WriteToStream(stream, out bufferSize);
        }

        /// <summary>
        /// Writes SSM file to stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bufferSize">Size of the buffer</param>
        public void WriteToStream(Stream stream, out int bufferSize)
        {
            using BinaryWriterExt w = new(stream);
            w.BigEndian = true;

            w.Write(0);
            w.Write(0);
            w.Write(Sounds.Length);
            w.Write(StartIndex);

            int headerSize = 0;
            foreach (DSP s in Sounds)
            {
                headerSize += 8 + s.Channels.Count * 0x40;
            }

            int projData = headerSize + 0x20;
            foreach (DSP s in Sounds)
            {
                w.Write(s.Channels.Count);
                w.Write(s.Frequency);

                foreach (DSPChannel channel in s.Channels)
                {
                    int sa = (projData - (headerSize + 0x20) + 1) * 2;

                    projData += channel.Data.Length;
                    if (projData % 0x8 != 0)
                        projData += 0x08 - projData % 0x08;

                    int en = sa + channel.NibbleCount;

                    w.Write(channel.LoopFlag);
                    w.Write(channel.Format);
                    w.Write(sa + channel.LoopStart);
                    w.Write(en);
                    w.Write(sa);
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
                }

            }

            long start = w.BaseStream.Position;
            foreach (DSP s in Sounds)
            {
                foreach (DSPChannel c in s.Channels)
                {
                    w.Write(c.Data);
                    if (w.BaseStream.Position % 0x08 != 0)
                        w.Write(new byte[0x08 - w.BaseStream.Position % 0x08]);
                }

            }

            // align 0x20
            if (w.BaseStream.Position % 0x20 != 0)
                w.Write(new byte[0x20 - w.BaseStream.Position % 0x20]);

            long DataSize = w.BaseStream.Position - start;

            if (DataSize % 0x20 != 0)
            {
                w.Write(new byte[0x20 - DataSize % 0x20]);
                w.Write(0);
                w.Write(0);
                DataSize += 0x20 - DataSize % 0x20;
            }

            w.Seek(0);
            w.Write(headerSize);
            w.Write((int)DataSize);

            bufferSize = (int)DataSize;
        }
    }
}
