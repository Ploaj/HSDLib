using HSDRaw;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            byteView.BringToFront();

            CenterToScreen();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void OpenSEMFile(string path)
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
                    //e.UniqueCount = largestEntry;
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
        private void entryList_MouseUp(object sender, MouseEventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void entryList_SelectedIndexChanged(object sender, EventArgs e)
        {
            soundList.DataSource = (entryList.SelectedItem as SEMEntry).Sounds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void soundList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (soundList.SelectedItem is SEMSound sound)
            {
                byteView.SetBytes(sound.CommandData);
                scriptBox.Text = DecompileScript(sound.CommandData);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox_MouseDown(object sender, MouseEventArgs e)
        {
            var listBox1 = sender as ListBox;
            if(sender == entryList)
                soundList.DataSource = (entryList.SelectedItem as SEMEntry)?.Sounds;
            if(sender == soundList)
            {
                if(soundList.SelectedItem is SEMSound sound)
                {
                    byteView.SetBytes(sound.CommandData);
                    scriptBox.Text = DecompileScript(sound.CommandData);
                }
            }
            if (listBox1.SelectedItem == null) return;
            listBox1.DoDragDrop(new object(), DragDropEffects.Move);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox_DragDrop(object sender, DragEventArgs e)
        {
            var listBox1 = sender as ListBox;
            Point point = listBox1.PointToClient(new Point(e.X, e.Y));
            int index = listBox1.IndexFromPoint(point);
            if (index < 0) index = listBox1.Items.Count - 1;
            if (index == listBox1.SelectedIndex)
                return;
            object data = listBox1.SelectedItem;
            var ds = listBox1.DataSource as IList;
            listBox1.BeginUpdate();
            ds.Remove(data);
            ds.Insert(index, data);
            listBox1.SelectedIndex = index;
            listBox1.EndUpdate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox_Remove(ListBox listBox1)
        {
            object data = listBox1.SelectedItem;
            var ds = listBox1.DataSource as IList;
            if(ds != null)
                ds.Remove(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            listBox_Remove(entryList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRemoveSound_Click(object sender, EventArgs e)
        {
            listBox_Remove(soundList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddEntry_Click(object sender, EventArgs e)
        {
            Entries.Add(new SEMEntry()
            {
                Index = Entries.Count,
                Sounds = new BindingList<SEMSound>()
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddSound_Click(object sender, EventArgs e)
        {
            object data = soundList.SelectedItem;
            var ds = soundList.DataSource as IList;
            if (ds != null)
            {
                ds.Add(new SEMSound()
                {
                    Index = ds.Count,
                    CommandData = new byte[] { 0x0E, 0, 0, 0}
                });
            }
        }

        private Dictionary<byte, string> OPCODES = new Dictionary<byte, string>()
        {
            {0x00, "UNKNOWN00" },
            {0x01, "SFXID" },
            {0x02, "UNKNOWN02" },
            {0x03, "UNKNOWN03" },
            {0x04, "UNKNOWN04" },
            {0x06, "UNKNOWN06" },
            {0x07, "UNKNOWN07" },
            {0x08, "CHANNEL" },
            {0x09, "UNKNOWN09" },
            {0x0C, "UNKNOWN0C" },
            {0x0D, "UNKNOWN0D" },
            {0x0E, "RETURN" },
            {0x0F, "UNKNOWN0F" },
            {0x10, "REVERB" },
            {0x11, "UNKNOWN11" },
            {0x14, "UNKNOWN14" },
            {0xFD, "SFXIDEXT" },
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private string DecompileScript(byte[] data)
        {
            StringBuilder s = new StringBuilder();
            for(int i = 0; i < data.Length;)
            {
                var op = OPCODES[data[i++]];

                s.AppendLine($".{op} 0x{data[i++].ToString("X2")}{data[i++].ToString("X2")}{data[i++].ToString("X2")}");
            }
            return s.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int CompileScript(string script, out byte[] data)
        {
            var cmd = Regex.Replace(script, @"\s+", string.Empty).Split('.');

            data = new byte[4 * (cmd.Length - 1)];

            int i = 0;
            foreach (var line in cmd)
            {
                var args = line.Split(new string[] { "0x" }, StringSplitOptions.None);

                if (args.Length != 2)
                    continue;

                Console.WriteLine(string.Join(", ", args));

                data[i++] = OPCODES.FirstOrDefault(x => x.Value == args[0].ToUpper()).Key;
                var d = int.Parse(args[1], System.Globalization.NumberStyles.HexNumber);
                data[i++] = (byte)((d >> 16) & 0xFF);
                data[i++] = (byte)((d >> 8) & 0xFF);
                data[i++] = (byte)(d & 0xFF);
            }


            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSaveScript_Click(object sender, EventArgs e)
        {
            if (soundList.SelectedItem is SEMSound sound)
            {
                byte[] d;
                if(CompileScript(scriptBox.Text, out d) == -1)
                    sound.CommandData = d;
                byteView.SetBytes(sound.CommandData);
                scriptBox.Text = DecompileScript(sound.CommandData);
            }
        }
    }
}
