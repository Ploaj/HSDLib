using HSDRaw;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Extra
{
    public partial class SEMEditor : Form
    {
        public class SEMEntry
        {
            public BindingList<SEMSound> Sounds = new BindingList<SEMSound>();

            public int UniqueCount { get; set; }

            public int Index { get; set; }

            public override string ToString()
            {
                return $"Entry_{Index} : Count {Sounds.Count} - {UniqueCount}";
            }
        }

        public class SEMSound
        {
            public byte[] CommandData = new byte[0];

            public int Index { get; set; }

            public string Decompile()
            {
                if (CommandData.Length == 0)
                    return "";

                StringBuilder output = new StringBuilder();

                return output.ToString();
            }

            public void Compile(string code)
            {

            }

            public override string ToString()
            {
                return $"Sound_{Index} : Length {CommandData.Length}";
            }
        }

        private BindingList<SEMEntry> Entries = new BindingList<SEMEntry>();

        private ByteViewer byteView;

        public SEMEditor()
        {
            InitializeComponent();

            entryList.DataSource = Entries;

            byteView = new ByteViewer();
            byteView.Dock = DockStyle.Fill;
            groupBox1.Controls.Add(byteView);

            CenterToScreen();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        private void OpenSEMFile(string path)
        {
            using (BinaryReaderExt r = new BinaryReaderExt(new FileStream(path, FileMode.Open)))
            {
                r.BigEndian = true;

                r.Seek(8);
                var entryCount = r.ReadInt32();

                var offsetTableStart = r.Position + (entryCount + 1) * 4;
                
                for (uint i = 0; i < entryCount; i++)
                {
                    SEMEntry e = new SEMEntry();
                    e.Index = (int)i;
                    Entries.Add(e);

                    r.Seek(0x0C + i * 4);
                    var startIndex = r.ReadInt32();
                    var endIndex = r.ReadInt32();

                    HashSet<int> UniqueEntries = new HashSet<int>();
                    int largestEntry = 0;
                    for(uint j = 0; j < endIndex - startIndex; j++)
                    {
                        SEMSound s = new SEMSound();
                        s.Index = (int)j;
                        r.Seek((uint)(offsetTableStart + startIndex * 4 + j * 4));
                        var dataOffsetStart = r.ReadUInt32();
                        var dataOffsetEnd = r.ReadUInt32();

                        if (dataOffsetEnd == 0)
                            dataOffsetEnd = (uint)r.Length;

                        r.Seek(dataOffsetStart);
                        s.CommandData = r.ReadBytes((int)(dataOffsetEnd - dataOffsetStart));
                        e.Sounds.Add(s);

                        var entryId = ((s.CommandData[1] & 0xFF) << 16)
                            | ((s.CommandData[2] & 0xFF) << 8)
                            | ((s.CommandData[3] & 0xFF));

                        if (!UniqueEntries.Contains(entryId))
                            UniqueEntries.Add(entryId);

                    }
                    e.UniqueCount = UniqueEntries.Count;
                    e.UniqueCount = largestEntry;
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void DumpUniqueCommands(string path)
        {
            Dictionary<byte, List<int>> commands = new Dictionary<byte, List<int>>();

            foreach(var e in Entries)
            {
                foreach (var s in e.Sounds)
                {
                    int p = 0;
                    while (p < s.CommandData.Length)
                    {
                        var cmd = s.CommandData[p++];
                        var value = ((s.CommandData[p++] & 0xFF) << 16)
                        | ((s.CommandData[p++] & 0xFF) << 8)
                        | ((s.CommandData[p++] & 0xFF));

                        if (!commands.ContainsKey(cmd))
                            commands.Add(cmd, new List<int>());

                        if (!commands[cmd].Contains(value))
                            commands[cmd].Add(value);
                    }
                }
            }
            

            // Dump command Info
            foreach (var k in commands)
            {
                using (StreamWriter w = new StreamWriter(new FileStream(path + "\\" + k.Key.ToString("X2") + ".txt", FileMode.Create)))
                {
                    k.Value.Sort();
                    foreach (var v in k.Value)
                        w.WriteLine(v);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        private void SaveSEMFile(string path)
        {
            using (BinaryWriterExt w = new BinaryWriterExt(new FileStream(path, FileMode.Create)))
            {
                w.BigEndian = true;

                w.Write(0);
                w.Write(0);
                w.Write(Entries.Count);
                int index = 0;
                foreach(var e in Entries)
                {
                    w.Write(index);
                    index += e.Sounds.Count;
                }
                w.Write(index);

                var offset = w.BaseStream.Position + 4 * index + 4;
                var dataindex = 0;

                foreach (var e in Entries)
                {
                    foreach(var v in e.Sounds)
                    {
                        w.Write((int)(offset + dataindex));
                        dataindex += v.CommandData.Length;
                    }
                }

                w.Write(0);

                foreach (var e in Entries)
                {
                    foreach (var v in e.Sounds)
                    {
                        w.Write(v.CommandData);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openSEMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFile("SEM (*.sem)|*.sem");
            if(f != null)
            {
                OpenSEMFile(f);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportSEMToolStripMenuItem_Click(object sender, EventArgs e)
        {

            var f = Tools.FileIO.SaveFile("SEM (*.sem)|*.sem");
            if (f != null)
            {
                SaveSEMFile(f);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void entryList_SelectedIndexChanged(object sender, EventArgs e)
        {
            soundList.DataSource = (entryList.SelectedItem as SEMEntry)?.Sounds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void soundList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(soundList.SelectedItem is SEMSound s)
            {
                byteView.SetBytes(s.CommandData);
            }
            else
            {
                byteView.SetBytes(null);
            }
        }
    }
}
