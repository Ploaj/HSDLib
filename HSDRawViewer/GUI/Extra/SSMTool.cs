using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using HSDRaw;
using VGAudio.Codecs.GcAdpcm;
using System.Media;

namespace HSDRawViewer.GUI.Extra
{
    public partial class SSMTool : Form
    {
        #region Classes

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
                if(wavFile.Length < 0x2C)
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
                    var channelSizes = r.ReadInt32() / channelCount;
                    
                    // TODO:
                    // need a dll to encode coefs

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
                    if(Channels.Count == 1)
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
                        for(int  i =0; i < sound_data1.Length; i++)
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

#endregion

        private string FilePath;

        private int Unknown;

        public BindingList<DSP> Sounds = new BindingList<DSP>();

        public ContextMenu CMenu = new ContextMenu();

        private SoundPlayer SoundPlayer;

        public SSMTool()
        {
            InitializeComponent();

            listBox1.DataSource = Sounds;

            listBox1.ContextMenu = CMenu;

            FormClosing += (sender, args) =>
            {
                if (args.CloseReason == CloseReason.UserClosing)
                {
                    args.Cancel = true;
                    Hide();
                }
            };
        }

        #region Controls

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var file = Tools.FileIO.OpenFile("SSM (*.ssm)|*.ssm");

            if(file != null)
            {
                Sounds.Clear();
                FilePath = file;
                using (BinaryReaderExt r = new BinaryReaderExt(new FileStream(file, FileMode.Open)))
                {
                    r.BigEndian = true;

                    var headerLength = r.ReadInt32();
                    var dataOff = r.ReadInt32();
                    var soundCount = r.ReadInt32();
                    Unknown = r.ReadInt32();

                    for(int i = 0; i < soundCount; i++)
                    {
                        var sound = new DSP();
                        var ChannelCount = r.ReadInt32();
                        sound.Frequency = r.ReadInt32();

                        sound.Channels.Clear();
                        for(int j = 0; j < ChannelCount; j++)
                        {
                            var channel = new DSPChannel();

                            channel.LoopFlag = r.ReadInt16();
                            channel.Format = r.ReadInt16();
                            channel.SA = r.ReadInt32();
                            channel.EA = r.ReadInt32();
                            channel.CA = r.ReadInt32();
                            for (int k = 0; k < 0x10; k++)
                                channel.COEF[k] = r.ReadInt16();
                            channel.Gain = r.ReadInt16();
                            channel.InitPs = r.ReadInt16();
                            channel.InitSh1 = r.ReadInt16();
                            channel.InitSh2 = r.ReadInt16();
                            channel.Loopps = r.ReadInt16();
                            channel.Loopsh1 = r.ReadInt16();
                            channel.Loopsh2 = r.ReadInt16();
                            r.ReadInt16(); //  padding
                            
                            channel.NibbleCount = channel.EA - channel.SA;

                            sound.Channels.Add(channel);

                            var DataOffset = (headerLength + 0x18 + (int)Math.Ceiling(channel.SA / 2d));
                            DataOffset += DataOffset % 0x08 != 0 ? 0x08 - DataOffset % 0x08 : 0;

                            // note: some dsps have data past their nibble count
                            // this may be garbage from when they were created
                            // if we want to get all of it including garbage:
                            // DataOffset + channel.NibbleCount
                            // then align to 0x08

                            channel.Data = r.GetSection((uint)DataOffset, (int)Math.Ceiling(channel.NibbleCount / 2d) + 1);
                        }
                        
                        Sounds.Add(sound);
                    }
                }
                if(listBox1.Items.Count > 0)
                    listBox1.SelectedIndex = 0;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject= listBox1.SelectedItem;

            if(listBox1.SelectedItem != null && listBox1.SelectedItem is DSP dsp)
            {
                buttonPlay.Enabled = true;
                buttonReplace.Enabled = true;
                buttonExport.Enabled = true;
                buttonDelete.Enabled = true;

                listBox2.DataSource = dsp.Channels;
            }
            else
            {
                listBox2.DataSource = null;
                propertyGrid1.SelectedObject = null;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(FilePath == null)
                FilePath = Tools.FileIO.SaveFile("SSM (*.ssm)|*.ssm");

            if (FilePath != null)
                Save(FilePath);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var filePath = Tools.FileIO.SaveFile("SSM (*.ssm)|*.ssm");

            if (filePath != null)
            {
                Save(filePath);
            }
        }


        private void buttonExport_Click(object sender, EventArgs e)
        {
            var file = Tools.FileIO.SaveFile("Supported (*.wav*.dsp)|*.wav;*.dsp");

            if (file != null && listBox1.SelectedItem != null)
            {
                if (file.EndsWith(".dsp"))
                    SaveSoundAsDSP(file, listBox1.SelectedItem as DSP);
                if (file.EndsWith(".wav"))
                {
                    File.WriteAllBytes(file, (listBox1.SelectedItem as DSP).ToWAVE());
                }
            }
        }

        private void importDSPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var file = Tools.FileIO.OpenFile("DSP (*.dsp)|*.dsp");

            if (file != null)
            {
                var dsp = ImportDSP(file);

                Sounds.Add(dsp);
            }
        }

        private void buttonReplace_Click(object sender, EventArgs e)
        {
            var file = Tools.FileIO.OpenFile("DSP (*.dsp)|*.dsp");

            if (file != null && listBox1.SelectedItem is DSP dsp)
            {
                var newdsp = ImportDSP(file);

                dsp.Frequency = newdsp.Frequency;
                dsp.Channels = newdsp.Channels;

                listBox1.SelectedItem = listBox1.SelectedItem;
                Sounds.ResetBindings();
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem is DSP dsp)
                Sounds.Remove(dsp);
        }


        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void listBox2_DoubleClick(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem is DSPChannel channel)
            {
                propertyGrid1.SelectedObjects = new object[] { channel };
            }
        }

        #endregion

        #region Functions

        private void Save(string filePath)
        {
            FilePath = filePath;

            using (BinaryWriterExt w = new BinaryWriterExt(new FileStream(filePath, FileMode.Create)))
            {
                w.BigEndian = true;
                
                w.Write(0);
                w.Write(0);
                w.Write(Sounds.Count);
                w.Write(Unknown);
                
                int headerSize = 0;
                foreach (var s in Sounds)
                {
                    headerSize += 8 + s.Channels.Count * 0x40;
                }

                var projData = headerSize + 0x20;
                foreach(var s in Sounds)
                {
                    w.Write(s.Channels.Count);
                    w.Write(s.Frequency);

                    foreach(var channel in s.Channels)
                    {
                        var sa = (projData - (headerSize + 0x20) + 1) * 2;

                        projData += channel.Data.Length;
                        if (projData % 0x8 != 0)
                            projData += 0x08 - projData % 0x08;
                        
                        var en = sa + channel.NibbleCount;

                        w.Write(channel.LoopFlag);
                        w.Write(channel.Format);
                        w.Write(sa);
                        w.Write(en);
                        w.Write(sa);
                        foreach (var v in channel.COEF)
                            w.Write(v);
                        w.Write(channel.Gain);
                        w.Write(channel.InitPs);
                        w.Write(channel.InitSh1);
                        w.Write(channel.InitSh2);
                        w.Write(channel.Loopps);
                        w.Write(channel.Loopsh1);
                        w.Write(channel.Loopsh2);
                        w.Write((short)0);
                    }
                    
                }

                w.Write(0);
                w.Write(0);
                w.Write(0);
                w.Write(0);

                var start = w.BaseStream.Position;
                foreach (var s in Sounds)
                {
                    foreach(var c in s.Channels)
                    {
                        w.Write(c.Data);
                        if (w.BaseStream.Position % 0x08 != 0)
                            w.Write(new byte[0x08 - w.BaseStream.Position % 0x08]);
                    }
                    
                }
                var DataSize = w.BaseStream.Position - start;

                w.Seek(0);
                w.Write(headerSize);
                w.Write((int)DataSize);
            }
        }

        public void MoveUp()
        {
            MoveItem(-1);
        }

        public void MoveDown()
        {
            MoveItem(1);
        }

        public void MoveItem(int direction)
        {
            // Checking selected item
            if (listBox1.SelectedItem == null || listBox1.SelectedIndex < 0)
                return; // No selected item - nothing to do

            // Calculate new index using move direction
            int newIndex = listBox1.SelectedIndex + direction;

            // Checking bounds of the range
            if (newIndex < 0 || newIndex >= listBox1.Items.Count)
                return; // Index out of range - nothing to do

            object selected = listBox1.SelectedItem;

            // Removing removable element
            listBox1.Items.Remove(selected);
            // Insert it in new position
            listBox1.Items.Insert(newIndex, selected);
            // Restore selection
            listBox1.SetSelected(newIndex, true);
        }

        private void SaveSoundAsDSP(string filePath, DSP dsp)
        {
            if(dsp.Channels.Count == 1)
            {
                // mono
                SaveChannelAsDSP(filePath, dsp.Channels[0], dsp.Frequency);
            }
            else
            {
                // stereo or more
                var head = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath);
                var ext = Path.GetExtension(filePath);

                for(int i = 0; i < dsp.Channels.Count; i++)
                {

                    SaveChannelAsDSP(head + $"_channel_{i}" + ext, dsp.Channels[i], dsp.Frequency);
                }
            }
        }


        private void SaveChannelAsDSP(string filePath, DSPChannel channel, int frequency)
        {
            using (BinaryWriterExt w = new BinaryWriterExt(new FileStream(filePath, FileMode.Create)))
            {
                w.BigEndian = true;

                var samples = channel.NibbleCount * 7 / 8;

                w.Write(samples);
                w.Write(channel.NibbleCount);
                w.Write(frequency);

                w.Write(channel.LoopFlag);
                w.Write(channel.Format);
                w.Write(2);
                w.Write(channel.NibbleCount - 2);
                w.Write(2);
                foreach (var v in channel.COEF)
                    w.Write(v);
                w.Write(channel.Gain);
                w.Write(channel.InitPs);
                w.Write(channel.InitSh1);
                w.Write(channel.InitSh2);
                w.Write(channel.Loopps);
                w.Write(channel.Loopsh1);
                w.Write(channel.Loopsh2);
                w.Write((short)0);

                w.Write(new byte[0x14]);

                w.Write(channel.Data);

                if (w.BaseStream.Position % 0x8 != 0)
                    w.Write(new byte[0x08 - w.BaseStream.Position % 0x08]);

                w.BaseStream.Close();
            }
        }

        private DSP ImportDSP(string filePath)
        {
            using (BinaryReaderExt r = new BinaryReaderExt(new FileStream(filePath, FileMode.Open)))
            {
                r.BigEndian = true;

                var dsp = new DSP();

                r.ReadInt32();
                var nibbleCount = r.ReadInt32();
                dsp.Frequency = r.ReadInt32();

                var channel = new DSPChannel();

                channel.LoopFlag = r.ReadInt16();
                channel.Format = r.ReadInt16();
                channel.SA = r.ReadInt32();
                channel.EA = r.ReadInt32();
                channel.CA = r.ReadInt32();
                for (int k = 0; k < 0x10; k++)
                    channel.COEF[k] = r.ReadInt16();
                channel.Gain = r.ReadInt16();
                channel.InitPs = r.ReadInt16();
                channel.InitSh1 = r.ReadInt16();
                channel.InitSh2 = r.ReadInt16();
                channel.Loopps = r.ReadInt16();
                channel.Loopsh1 = r.ReadInt16();
                channel.Loopsh2 = r.ReadInt16();
                r.ReadInt16(); //  padding

                r.Seek(0x60);
                channel.NibbleCount = nibbleCount;
                channel.Data = r.ReadBytes((int)Math.Ceiling(nibbleCount / 2d));

                dsp.Channels.Add(channel);

                r.BaseStream.Close();

                return dsp;
            }
        }

#endregion
        
        private void PlaySound(DSP dsp)
        {
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

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedItem is DSP dsp)
            {
                PlaySound(dsp);
            }
        }
    }
}
