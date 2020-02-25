using HSDRaw;
using System;
using System.Collections.Generic;
using System.IO;

namespace HSDRawViewer.Sound
{
    public class HPS
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DSP ToDSP(byte[] data)
        {
            DSP dsp = new DSP();
            dsp.Channels.Clear();
            using (BinaryReaderExt r = new BinaryReaderExt(new MemoryStream(data)))
            {
                r.BigEndian = true;

                if (new string(r.ReadChars(7)) != " HALPST")
                    throw new NotSupportedException("Invalid HPS file");
                r.ReadByte();

               dsp.Frequency = r.ReadInt32();

                var channelCount = r.ReadInt32();

                if (channelCount != 2)
                    throw new NotSupportedException("Only HPS with 2 channels are currently supported");

                for (int i = 0; i < channelCount; i++)
                {
                    var channel = new DSPChannel();

                    channel.LoopFlag = r.ReadInt16();
                    channel.Format = r.ReadInt16();
                    var SA = r.ReadInt32();
                    var EA = r.ReadInt32();
                    var CA = r.ReadInt32();
                    for (int k = 0; k < 0x10; k++)
                        channel.COEF[k] = r.ReadInt16();
                    channel.Gain = r.ReadInt16();
                    channel.InitialPredictorScale = r.ReadInt16();
                    channel.InitialSampleHistory1 = r.ReadInt16();
                    channel.InitialSampleHistory1 = r.ReadInt16();

                    channel.NibbleCount = EA - CA;
                    channel.LoopStart = SA - CA;

                    dsp.Channels.Add(channel);
                }

                // read blocks
                r.Position = 0x80;

                Dictionary<int, int> OffsetToLoopPosition = new Dictionary<int, int>();
                List<byte> channelData1 = new List<byte>();
                List<byte> channelData2 = new List<byte>();
                while (true)
                {
                    var pos = r.Position;
                    var length = r.ReadInt32();
                    var lengthMinusOne = r.ReadInt32();
                    var next = r.ReadInt32();
                    {
                        var initPS = r.ReadInt16();
                        var initsh1 = r.ReadInt16();
                        var initsh2 = r.ReadInt16();
                        var gain = r.ReadInt16();
                    }
                    {
                        var initPS = r.ReadInt16();
                        var initsh1 = r.ReadInt16();
                        var initsh2 = r.ReadInt16();
                        var gain = r.ReadInt16();
                    }
                    var extra = r.ReadInt32();
                    
                    OffsetToLoopPosition.Add((int)pos, channelData1.Count * 2);
                    channelData1.AddRange(r.ReadBytes(length / 2));
                    channelData2.AddRange(r.ReadBytes(length / 2));

                    if (next < r.Position || next == -1)
                    {
                        if (next != -1)
                        {
                            foreach (var c in dsp.Channels)
                            {
                                c.LoopStart = OffsetToLoopPosition[next];
                            }
                        }
                        break;
                    }
                    else
                        r.Position = (uint)next;
                }

                dsp.Channels[0].Data = channelData1.ToArray();
                dsp.Channels[1].Data = channelData2.ToArray();
            }
            return dsp;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dsp"></param>
        /// <param name="filePath"></param>
        public static void SaveDSPAsHPS(DSP dsp, string filePath)
        {
            using (BinaryWriterExt w = new BinaryWriterExt(new FileStream(filePath, FileMode.Create)))
            {
                w.BigEndian = true;

                w.Write(" HALPST".ToCharArray());
                w.Write((byte)0);
                w.Write(dsp.Frequency);
                w.Write(dsp.Channels.Count);

                // channel meta data
                foreach(var c in dsp.Channels)
                {
                    w.Write(c.LoopFlag);
                    w.Write(c.Format);
                    w.Write(2);
                    w.Write(dsp.Channels[0].Data.Length * 2);
                    w.Write(2);
                    for (int k = 0; k < 0x10; k++)
                        w.Write(c.COEF[k]);
                    w.Write(c.Gain);
                    w.Write(c.InitialPredictorScale);
                    w.Write(c.InitialSampleHistory1);
                    w.Write(c.InitialSampleHistory2);
                }


                // now divide into chunks taking into account loop point

                var loopStart = dsp.Channels[0].LoopStart / 2;
                if (loopStart % 56 == 0)
                    loopStart += 56 - (loopStart % 56);
                var loopPosition = -1;
                var nextPosition = 0;

                int[] history = new int[dsp.Channels.Count];
                
                int i;
                for (i = 0; i < dsp.Channels[0].Data.Length;)
                {
                    var chunkSize = 0x10000 / dsp.Channels.Count;

                    if (i < loopStart && i + chunkSize > loopStart && loopStart % chunkSize > 0)
                        chunkSize = loopStart % chunkSize;

                    if (i >= loopStart && loopPosition == -1)
                        loopPosition = (int)w.BaseStream.Position;

                    if (i + chunkSize > dsp.Channels[0].Data.Length)
                        chunkSize = dsp.Channels[0].Data.Length - i;

                    if (chunkSize % 0x20 != 0)
                        chunkSize += 0x20 - (chunkSize % 0x20);

                    var chunkStart = w.BaseStream.Position;
                    w.BaseStream.Position = nextPosition;
                    if(nextPosition != 0)
                        w.Write((int)chunkStart);
                    w.BaseStream.Position = chunkStart;

                    w.Write(chunkSize * 2);
                    w.Write(chunkSize * 2 - 1);
                    nextPosition = (int)w.BaseStream.Position;
                    w.Write(0); // next offsets
                    
                    for(var j = 0; j < history.Length; j++)
                    {
                        w.Write((short)dsp.Channels[j].Data[i]);
                        w.Write(history[j]);
                        w.Write((short)0);
                    }

                    w.Write(0); //padding

                    for (var j = 0; j < history.Length; j++)
                    {
                        var c = dsp.Channels[j];
                        byte[] chunk = new byte[chunkSize];
                        var unpaddedChunk = chunkSize;
                        if (i + unpaddedChunk > dsp.Channels[0].Data.Length)
                            unpaddedChunk = dsp.Channels[0].Data.Length - i;
                        Array.Copy(c.Data, i, chunk, 0, unpaddedChunk);
                        var dec = VGAudio.Codecs.GcAdpcm.GcAdpcmDecoder.Decode(chunk, c.COEF);
                        history[j] = (ushort)dec[dec.Length - 2] | ((ushort)dec[dec.Length - 1] << 16);
                        w.Write(chunk);
                    }

                    Console.WriteLine(chunkSize.ToString("X"));

                    i += chunkSize;
                }

                w.BaseStream.Position = nextPosition;
                w.Write(loopPosition);

            }
        }
    }
}
