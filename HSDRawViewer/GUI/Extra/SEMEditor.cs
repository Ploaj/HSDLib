using HSDRaw;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Extra
{
    public enum AXCMD
    {
        CMD_SETUP = 0x00,
        CMD_DL_AND_VOL_MIX = 0x01,
        CMD_PB_ADDR = 0x02,
        CMD_PROCESS = 0x03,
        CMD_MIX_AUXA = 0x04,
        CMD_MIX_AUXB = 0x05,
        CMD_UPLOAD_LRS = 0x06,
        CMD_SET_LR = 0x07,
        CMD_UNK_08 = 0x08,
        CMD_MIX_AUXB_NOWRITE = 0x09,
        CMD_COMPRESSOR_TABLE_ADDR = 0x0A,
        CMD_UNK_0B = 0x0B,
        CMD_UNK_0C = 0x0C,
        CMD_MORE = 0x0D,
        CMD_OUTPUT = 0x0E,
        CMD_END = 0x0F,
        CMD_MIX_AUXB_LR = 0x10,
        CMD_SET_OPPOSITE_LR = 0x11,
        CMD_UNK_12 = 0x12,
        CMD_SEND_AUX_AND_MIX = 0x13,
    };

    public partial class SEMEditor : Form
    {
        public class SEMEntry
        {
            public BindingList<SEMSound> Sounds = new BindingList<SEMSound>();

            public int Index { get; set; }

            public override string ToString()
            {
                return $"Entry_{Index} : Count {Sounds.Count + 1}";
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

                for(uint i = 0; i < entryCount; i++)
                {
                    SEMEntry e = new SEMEntry();
                    e.Index = (int)i;
                    Entries.Add(e);

                    r.Seek(0x0C + i * 4);
                    var startIndex = r.ReadInt32();
                    var endIndex = r.ReadInt32();

                    
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
                    }
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
