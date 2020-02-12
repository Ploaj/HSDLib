﻿using HSDRaw;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace HSDRawViewer.Sound
{
    /// <summary>
    /// 
    /// </summary>
    public class SEM
    {
        private static Dictionary<byte, string> OPCODES = new Dictionary<byte, string>()
        {
            {0x00, "UNKNOWN00" },
            {0x01, "SFXID" },
            {0x02, "UNKNOWN02" },
            {0x03, "UNKNOWN03" },
            {0x04, "PRIORITY" },
            {0x05, "UNKNOWN05" }, // Unused
            {0x06, "UNKNOWN06" },
            {0x07, "UNKNOWN07" },
            {0x08, "CHANNEL" },
            {0x09, "UNKNOWN09" },
            {0x0A, "UNKNOWN0A" }, // Unused
            {0x0B, "UNKNOWN0B" }, // Unused
            {0x0C, "PITCH" },
            {0x0D, "UNKNOWN0D" },
            {0x0E, "END" },
            {0x0F, "LOOP" },
            {0x10, "REVERB" },
            {0x11, "UNKNOWN11" },
            {0x12, "UNKNOWN12" }, // Unused
            {0x13, "UNKNOWN13" }, // Unused
            {0x14, "UNKNOWN14" },
            {0x15, "UNKNOWN15" }, // Unused
            {0xFD, "NULL" },
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public static string DecompileSEMScript(byte[] data)
        {
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < data.Length;)
            {
                var op = OPCODES[data[i++]];
                i++;
                var val = (short)(((data[i++] & 0xFF) << 8) | ((data[i++] & 0xFF)));
                s.AppendLine($".{op} : {val}");
            }
            return s.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int CompileSEMScript(string script, out byte[] data)
        {
            var cmd = Regex.Replace(script, @"\s+", string.Empty).Split('.');

            data = new byte[4 * (cmd.Length - 1)];

            int i = 0;
            foreach (var line in cmd)
            {
                var args = line.Split(new string[] { ":" }, StringSplitOptions.None);

                if (args.Length != 2)
                    continue;

                data[i++] = OPCODES.FirstOrDefault(x => x.Value == args[0].ToUpper()).Key;
                var d = int.Parse(args[1]);
                data[i++] = (byte)((d >> 16) & 0xFF);
                data[i++] = (byte)((d >> 8) & 0xFF);
                data[i++] = (byte)(d & 0xFF);
            }
            
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<SEMEntry> ReadSEMFile(string filePath)
        {
            var o = MessageBox.Show("Load Sound Banks(SSM) as well?", "Load SSM Files?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            var Entries = new List<SEMEntry>();

            Dictionary<int, SSM> indexToSSM = new Dictionary<int, SSM>();

            if(o == DialogResult.Yes)
            {
                foreach(var f in Directory.GetFiles(Path.GetDirectoryName(filePath)))
                {
                    if (f.ToLower().EndsWith(".ssm"))
                    {
                        var ssm = new SSM();
                        ssm.Open(f);
                        indexToSSM.Add(ssm.StartIndex, ssm);
                    }
                }
            }

            if (o == DialogResult.Cancel)
                return Entries;

            using (BinaryReaderExt r = new BinaryReaderExt(new FileStream(filePath, FileMode.Open)))
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

                    var ssmStartIndex = int.MaxValue;
                    for (uint j = 0; j < endIndex - startIndex; j++)
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

                        ssmStartIndex = Math.Min(ssmStartIndex, s.SoundCommandIndex);
                    }
                    
                    if (o == DialogResult.Yes && indexToSSM.ContainsKey(ssmStartIndex))
                    {
                        e.SoundBank = indexToSSM[ssmStartIndex];
                        foreach (var v in e.Sounds)
                            v.SoundCommandIndex -= ssmStartIndex;
                    }
                }
            }
            return Entries;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void SaveSEMFile(string path, List<SEMEntry> Entries)
        {
            var soundOffset = 0;
            using (BinaryWriterExt w = new BinaryWriterExt(new FileStream(path, FileMode.Create)))
            {
                w.BigEndian = true;

                w.Write(0);
                w.Write(0);
                w.Write(Entries.Count);
                int index = 0;
                foreach (var e in Entries)
                {
                    w.Write(index);
                    index += e.Sounds.Count;
                }
                w.Write(index);

                var offset = w.BaseStream.Position + 4 * index + 4;
                var dataindex = 0;

                foreach (var e in Entries)
                {

                    foreach (var v in e.Sounds)
                    {
                        w.Write((int)(offset + dataindex));
                        dataindex += v.CommandData.Length;
                    }
                }

                w.Write(0);

                foreach (var e in Entries)
                {
                    // fix sound offset ids
                    if (e.SoundBank != null)
                    {
                        // set start offset in sem and save
                        e.SoundBank.StartIndex = soundOffset;
                        e.SoundBank.Save(Path.GetDirectoryName(path) + "\\" + e.SoundBank.Name);

                        // add sound offset
                        foreach (var v in e.Sounds)
                            v.SoundCommandIndex += soundOffset;

                        foreach (var v in e.Sounds)
                            w.Write(v.CommandData);

                        //return to normal
                        foreach (var v in e.Sounds)
                            v.SoundCommandIndex -= soundOffset;

                        soundOffset += e.SoundBank.Sounds.Count;
                    }
                    else
                    {
                        foreach (var v in e.Sounds)
                        {
                            w.Write(v.CommandData);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        private static void DumpUniqueCommands(string path, IEnumerable<SEMEntry> entries)
        {
            Dictionary<byte, List<int>> commands = new Dictionary<byte, List<int>>();

            foreach (var e in entries)
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
    }

    /// <summary>
    /// 
    /// </summary>
    public class SEMEntry
    {
        public BindingList<SEMSound> Sounds = new BindingList<SEMSound>();

        public SSM SoundBank;
        
        public int Index { get; set; }

        public override string ToString()
        {
            return SoundBank != null ? SoundBank.Name : $"Entry_{Index} : Count {Sounds.Count}";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SEMSound
    {
        public byte[] CommandData = new byte[0];

        public int Index { get; set; }

        public int SoundCommandIndex
        {
            get
            {
                for (int i = 0; i < CommandData.Length; i += 4)
                {
                    if (CommandData[i] == 0x01)
                    {
                        return ((CommandData[i + 1] & 0xFF) << 16) | ((CommandData[i + 2] & 0xFF) << 8) | (CommandData[i + 3] & 0xFF);
                    }
                }
                return -1;
            }
            set
            {
                for (int i = 0; i < CommandData.Length; i += 4)
                {
                    if (CommandData[i] == 0x01)
                    {
                        CommandData[i + 1] = (byte)((value >> 16) & 0xFF);
                        CommandData[i + 2] = (byte)((value >> 8) & 0xFF);
                        CommandData[i + 3] = (byte)(value & 0xFF);
                    }
                }
            }
        }
        
        public override string ToString()
        {
            return $"{Index.ToString("D6")} - Sound Bank ID: {SoundCommandIndex}";
        }
    }
}
