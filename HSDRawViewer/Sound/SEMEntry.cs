using HSDRaw;
using HSDRaw.MEX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HSDRawViewer.Sound
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SEMEntry
    {
        [YamlIgnore, Description("Unknown Flag"), TypeConverter(typeof(HexType))]
        public uint Flags { get => SoundBank == null ? 0 : (uint)SoundBank.Flag; set { if (SoundBank != null) SoundBank.Flag = (int)value; } }

        [YamlIgnore, DisplayName("Group Flags"), Description("Grouping Lookup information"), TypeConverter(typeof(HexType))]
        public uint GroupFlags { get => SoundBank == null ? 0 : (uint)SoundBank.GroupFlags; set { if (SoundBank != null) SoundBank.GroupFlags = (int)value; } }

        /// <summary>
        /// 
        /// </summary>
        public SEMScript[] Scripts { get; set; } = new SEMScript[0];

        [YamlIgnore]
        public SSM SoundBank;

        public override string ToString()
        {
            return SoundBank != null ? SoundBank.Name : $" Count {Scripts.Length}";
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddScript(SEMScript value)
        {
            var ar = Scripts;
            Array.Resize(ref ar, ar.Length + 1);
            ar[ar.Length - 1] = value;
            Scripts = ar;
        }

        /// <summary>
        /// Removes unused sounds from sound bank
        /// </summary>
        public void RemoveUnusedSoundsFromBank()
        {
            if (SoundBank == null)
                return;

            var usedSounds = Scripts.Select(e => e.SoundCommandIndex);

            List<DSP> newList = new List<DSP>();

            for (int i = 0; i < SoundBank.Sounds.Length; i++)
            {
                if (usedSounds.Contains(i))
                    newList.Add(SoundBank.Sounds[i]);
            }

            SoundBank.Sounds = newList.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static SEMEntry Deserialize(string data)
        {
            var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

            return deserializer.Deserialize<SEMEntry>(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        public void Serialize(string filepath)
        {
            var builder = new SerializerBuilder();
            builder.WithNamingConvention(CamelCaseNamingConvention.Instance);

            using (StreamWriter writer = File.CreateText(filepath))
            {
                builder.Build().Serialize(writer, this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadFromPackage(string fileName)
        {
            using (FileStream s = new FileStream(fileName, FileMode.Open))
            using (BinaryReaderExt r = new BinaryReaderExt(s))
            {
                if (s.Length < 0x14)
                    return;

                if (new string(r.ReadChars(4)) != "SPKG")
                    return;

                GroupFlags = r.ReadUInt32();
                Flags = r.ReadUInt32();

                var ssmSize = r.ReadInt32();
                Scripts = new SEMScript[r.ReadInt32()];

                for(int i = 0; i < Scripts.Length; i++)
                {
                    Scripts[i] = new SEMScript()
                    {
                        CommandData = r.GetSection(r.ReadUInt32(), r.ReadInt32())
                    };
                }

                r.PrintPosition();
                var name = r.ReadString(r.ReadByte());

                if (ssmSize == 0)
                {
                    SoundBank = null;
                }
                else
                {
                    if (SoundBank == null)
                        SoundBank = new SSM();
                    
                    using (MemoryStream ssmStream = new MemoryStream(r.ReadBytes(ssmSize)))
                    {
                        SoundBank.Open(name, ssmStream);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SaveAsPackage(string fileName)
        {
            using (FileStream s = new FileStream(fileName, FileMode.Create))
            using (BinaryWriter w = new BinaryWriter(s))
            {
                w.Write(new char[]{ 'S', 'P', 'K', 'G'});

                w.Write(GroupFlags);
                w.Write(Flags);
                w.Write(0);
                w.Write(Scripts.Length);
                
                w.Write(new byte[Scripts.Length * 8]);

                w.Write(SoundBank != null ? SoundBank.Name : "");

                if (SoundBank != null)
                    using (MemoryStream ssmFile = new MemoryStream())
                    {
                        SoundBank.WriteToStream(ssmFile, out int bs);
                        var ssm = ssmFile.ToArray();
                        w.Write(ssm);
                        var temp = s.Position;
                        s.Position = 0x0C;
                        w.Write(ssm.Length);
                        s.Position = temp;
                    }
            
                for (int i = 0; i < Scripts.Length; i++)
                {
                    var temp = s.Position;
                    s.Position = 0x14 + 8 * i;
                    w.Write((int)temp);
                    w.Write(Scripts[i].CommandData.Length);
                    s.Position = temp;

                    w.Write(Scripts[i].CommandData);
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SEMScript
    {
        /// <summary>
        /// 
        /// </summary>
        public byte[] CommandData = new byte[0];

        [YamlIgnore]
        public string Name { get; set; } = "";

        [YamlIgnore]
        public int SoundCommandIndex
        {
            get => GetOPCodeValue(0x01);
            set => SetOpCodeValue(0x01, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="opCode"></param>
        /// <returns></returns>
        public int GetOPCodeValue(byte opCode)
        {
            for (int i = 0; i < CommandData.Length; i += 4)
            {
                if (CommandData[i] == opCode)
                {
                    return (short)(((CommandData[i + 2] & 0xFF) << 8) | (CommandData[i + 3] & 0xFF));
                }
            }
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="opCode"></param>
        /// <param name="value"></param>
        public void SetOpCodeValue(byte opCode, int value)
        {
            for (int i = 0; i < CommandData.Length; i += 4)
            {
                if (CommandData[i] == opCode)
                {
                    CommandData[i + 1] = (byte)((value >> 16) & 0xFF);
                    CommandData[i + 2] = (byte)((value >> 8) & 0xFF);
                    CommandData[i + 3] = (byte)(value & 0xFF);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name} - ID: {SoundCommandIndex}";
        }
    }
}
