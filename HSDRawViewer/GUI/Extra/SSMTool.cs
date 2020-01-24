using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using HSDRaw;
using System.Media;
using System.IO.Compression;

namespace HSDRawViewer.GUI.Extra
{
    public partial class SSMTool : Form
    {
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

            Text = "SSM Editor";

            FormClosing += (sender, args) =>
            {
                if (args.CloseReason == CloseReason.UserClosing)
                {
                    if (SoundPlayer != null)
                        SoundPlayer.Stop();
                    args.Cancel = true;
                    Hide();
                }
            };
        }

        #region Controls

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var file = Tools.FileIO.OpenFile("SSM (*.ssm, *.sdi)|*.ssm;*.sdi");

            if (file != null)
            {
                OpenFile(file);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = listBox1.SelectedItem;

            if (listBox1.SelectedItem != null && listBox1.SelectedItem is DSP dsp)
            {
                toolStrip1.Enabled = true;

                listBox2.DataSource = dsp.Channels;
            }
            else
            {
                toolStrip1.Enabled = false;
                listBox2.DataSource = null;
                propertyGrid1.SelectedObject = null;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FilePath == null)
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
            var files = Tools.FileIO.OpenFiles("DSP (*.dsp*.wav)|*.dsp;*.wav;*.hps");

            if (files != null)
            {
                foreach(var file in files)
                {
                    if (file.ToLower().EndsWith(".dsp"))
                    {
                        var dsp = ImportDSP(file);
                        Sounds.Add(dsp);
                    }
                    if (file.ToLower().EndsWith(".wav"))
                    {
                        var dsp = new DSP();
                        dsp.FromWAVE(File.ReadAllBytes(file));
                        Sounds.Add(dsp);
                    }
                    if (file.ToLower().EndsWith(".hps"))
                    {
                        var dsp = new DSP();
                        dsp.FromHPS(File.ReadAllBytes(file));
                        Sounds.Add(dsp);
                    }
                }
            }
        }

        private void buttonReplace_Click(object sender, EventArgs e)
        {
            var file = Tools.FileIO.OpenFile("DSP (*.dsp*.wav)|*.dsp;*.wav");

            if (file != null && listBox1.SelectedItem is DSP dsp)
            {
                if (file.ToLower().EndsWith(".dsp"))
                {
                    var newdsp = ImportDSP(file);

                    dsp.Frequency = newdsp.Frequency;
                    dsp.Channels = newdsp.Channels;
                }

                if (file.ToLower().EndsWith(".wav"))
                {
                    dsp.FromWAVE(File.ReadAllBytes(file));
                }

                listBox1.SelectedItem = listBox1.SelectedItem;
                Sounds.ResetBindings();
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {

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
            if (dsp.Channels.Count == 1)
            {
                // mono
                SaveChannelAsDSP(filePath, dsp.Channels[0], dsp.Frequency);
            }
            else
            {
                // stereo or more
                var head = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath);
                var ext = Path.GetExtension(filePath);

                for (int i = 0; i < dsp.Channels.Count; i++)
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
                var LoopStartOffset = r.ReadInt32();
                var LoopEndOffset = r.ReadInt32();
                var CurrentAddress = r.ReadInt32();
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
            PlaySound();
        }

        private void PlaySound()
        {
            if (listBox1.SelectedItem is DSP dsp)
            {
                PlaySound(dsp);
            }
        }

        public void OpenFile(string filePath)
        {
            Text = "SSM Editor - " + filePath;

            Sounds.Clear();
            FilePath = filePath;

            if(Path.GetExtension(filePath).ToLower() == ".ssm")
                OpenSSM(filePath);

            if (Path.GetExtension(filePath).ToLower() == ".sdi")
                OpenSDI(filePath);

            if (listBox1.Items.Count > 0)
                listBox1.SelectedIndex = 0;
        }

        /// <summary>
        /// Used in Eighting Engine Games
        /// </summary>
        /// <param name="filePath"></param>
        private void OpenSDI(string filePath)
        {
            var sam = filePath.Replace(".sdi", ".sam");
            if (!File.Exists(sam))
                return;

            using (BinaryReaderExt r = new BinaryReaderExt(new FileStream(filePath, FileMode.Open)))
            using (BinaryReaderExt d = new BinaryReaderExt(new FileStream(sam, FileMode.Open)))
            {
                r.BigEndian = true;
                
                while(true)
                {
                    var id = r.ReadInt32();
                    if (id == -1)
                        break;
                    var dataoffset = r.ReadUInt32();
                    var padding = r.ReadInt32();
                    var flags = r.ReadInt16();
                    var frequency = r.ReadInt16();
                    var value = r.ReadInt32();
                    r.Skip(8); // unknown
                    uint coefOffset = r.ReadUInt32();

                    DSP dsp = new DSP();
                    dsp.Frequency = frequency;

                    DSPChannel channel = new DSPChannel();
                    channel.NibbleCount = value;

                    var temp = r.Position;
                    var end = (uint)d.Length;
                    if(r.ReadInt32() != -1)
                        end = r.ReadUInt32();

                    r.Seek(coefOffset);
                    r.ReadInt32();
                    r.ReadInt32();

                    for (int i = 0; i < 0x10; i++)
                        channel.COEF[i] = r.ReadInt16();

                    r.Seek(temp);

                    d.Seek(dataoffset);
                    byte[] data = d.ReadBytes((int)(end - dataoffset));

                    channel.Data = data;
                    channel.InitialPredictorScale = data[0];
                    dsp.Channels.Add(channel);

                    Sounds.Add(dsp);
                }
            }
        }

        /// <summary>
        /// Melee's sound format
        /// </summary>
        /// <param name="filePath"></param>
        private void OpenSSM(string filePath)
        {
            using (BinaryReaderExt r = new BinaryReaderExt(new FileStream(filePath, FileMode.Open)))
            {
                r.BigEndian = true;

                var headerLength = r.ReadInt32() + 0x10;
                var dataOff = r.ReadInt32();
                var soundCount = r.ReadInt32();
                Unknown = r.ReadInt32();

                for (int i = 0; i < soundCount; i++)
                {
                    var sound = new DSP();
                    var ChannelCount = r.ReadInt32();
                    sound.Frequency = r.ReadInt32();

                    sound.Channels.Clear();
                    for (int j = 0; j < ChannelCount; j++)
                    {
                        var channel = new DSPChannel();

                        channel.LoopFlag = r.ReadInt16();
                        channel.Format = r.ReadInt16();
                        var LoopStartOffset = r.ReadInt32();
                        var LoopEndOffset = r.ReadInt32();
                        var CurrentAddress = r.ReadInt32();
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

                        var DataOffset = headerLength + (int)Math.Ceiling(CurrentAddress / 2d) - 1;

                        channel.Data = r.GetSection((uint)DataOffset, (int)Math.Ceiling(channel.NibbleCount / 2d) + 1);

                    }

                    Sounds.Add(sound);
                }
            }
        }


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
                foreach (var s in Sounds)
                {
                    w.Write(s.Channels.Count);
                    w.Write(s.Frequency);

                    foreach (var channel in s.Channels)
                    {
                        var sa = (projData - (headerSize + 0x20) + 1) * 2;

                        projData += channel.Data.Length;
                        if (projData % 0x8 != 0)
                            projData += 0x08 - projData % 0x08;

                        var en = sa + channel.NibbleCount;

                        w.Write(channel.LoopFlag);
                        w.Write(channel.Format);
                        w.Write(sa + channel.LoopStart);
                        w.Write(en);
                        w.Write(sa);
                        foreach (var v in channel.COEF)
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

                var start = w.BaseStream.Position;
                foreach (var s in Sounds)
                {
                    foreach (var c in s.Channels)
                    {
                        w.Write(c.Data);
                        if (w.BaseStream.Position % 0x08 != 0)
                            w.Write(new byte[0x08 - w.BaseStream.Position % 0x08]);
                    }

                }

                // align 0x20
                if (w.BaseStream.Position % 0x20 != 0)
                    w.Write(new byte[0x20 - w.BaseStream.Position % 0x20]);

                var DataSize = w.BaseStream.Position - start;
                
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
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (SoundPlayer != null)
                SoundPlayer.Stop();
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            PlaySound();
        }

        private void exportAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var folder = Tools.FileIO.OpenFolder();

            if(folder != null)
            {
                var sIndex = 0;
                foreach(var s in Sounds)
                {
                    File.WriteAllBytes(folder + "\\sound_" + sIndex++ + "_channels_" + s.Channels.Count + "_frequency_" + s.Frequency + ".wav", s.ToWAVE());
                }
            }
        }
    }
}
