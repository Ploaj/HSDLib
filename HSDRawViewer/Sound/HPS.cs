using HSDRaw;
using System;
using System.Collections.Generic;
using System.IO;

namespace HSDRawViewer.Sound
{
    public class HPS
    {
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
                        r.ReadInt16();
                    }
                    {
                        var initPS = r.ReadInt16();
                        var initsh1 = r.ReadInt16();
                        var initsh2 = r.ReadInt16();
                        r.ReadInt16();
                    }
                    r.ReadInt32();

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
    }
}
