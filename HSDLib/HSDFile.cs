using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace HSDLib
{
    /// <summary>
    /// Hals DAT sysdolphin format
    /// Used in Kirby Air Ride, Super Smash Bros. Melee, Mario Kart Arcade, and Pokemon Channel
    /// </summary>
    public class HSDFile
    {
        public List<HSDRoot> Roots;
        public List<HSDRoot> Resources;

        /// <summary>
        /// Header for HSD Files
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct HSDHeader
        {
            public uint FileSize;
            public uint RelocationTableOffset;
            public int RelocationTableCount;
            public int RootCount;
            public int ResourceCount;
            public uint Unknown;
            public uint Unk2;
            public uint Unk3;
        }

        /// <summary>
        /// Opens and reads the HSD file into a class
        /// </summary>
        /// <param name="FileName">The path to the file</param>
        /// <returns></returns>
        public HSDFile(string FileName)
        {
            Decompile(FileName);
        }

        public HSDFile()
        {
            Roots = new List<HSDRoot>();
            Resources = new List<HSDRoot>();
        }

        /// <summary>
        /// Reads data into class system from file
        /// </summary>
        /// <param name="FileName">The path to the file</param>
        /// <returns></returns>
        public void Decompile(string FileName)
        {
            Decompile(new FileStream(FileName, FileMode.Open));
        }

        /// <summary>
        /// Reads data into class system from byte array
        /// </summary>
        /// <param name="FileName">The path to the file</param>
        /// <returns></returns>
        public void Decompile(byte[] Data)
        {
            Decompile(new MemoryStream(Data));
        }

        private void Decompile(Stream stream)
        {
            HSDReader Reader = new HSDReader(stream);

            HSDHeader Header = Reader.ReadType<HSDHeader>(new HSDHeader());

            Roots = new List<HSDRoot>(Header.RootCount);
            Resources = new List<HSDRoot>(Header.ResourceCount);

            Reader.Seek(Header.RelocationTableOffset + 0x20);
            Reader.ReadRelocationTable(Header.RelocationTableCount);

            // Roots
            for (int i = 0; i < Header.RootCount; i++)
            {
                Roots.Add(new HSDRoot());
                Roots[i].Offset = Reader.ReadUInt32() + 0x20;
                Roots[i].NameOffset = Reader.ReadUInt32();
            }

            // Resources
            for (int i = 0; i < Header.ResourceCount; i++)
            {
                Resources.Add(new HSDRoot());
                Resources[i].Offset = Reader.ReadUInt32() + 0x20;
                Resources[i].NameOffset = Reader.ReadUInt32();
            }

            uint StringOffset = Reader.Position();

            foreach (HSDRoot r in Roots)
            {
                Reader.Seek(StringOffset + r.NameOffset);
                r.Name = Reader.ReadString();
                r.Open(Reader);
            }
            foreach (HSDRoot r in Resources)
            {
                Reader.Seek(StringOffset + r.NameOffset);
                r.Name = Reader.ReadString();
                r.Open(Reader);
            }

            //Clean up
            Reader.Close();
        }

        /// <summary>
        /// Saves the HSDFile as a .dat
        /// </summary>
        /// <param name="FileName"></param>
        public void Save(string FileName)
        {
            HSDWriter Writer = new HSDWriter(new FileStream(FileName, FileMode.Create));
            Writer.Write(0);
            Writer.Write(0);
            Writer.Write(0);
            Writer.Write(Roots.Count);
            Writer.Write(Resources.Count);
            Writer.Write(0);
            Writer.Write(0);
            Writer.Write(0);

            // Write Data
            foreach(HSDRoot r in Roots)
            {
                r.Save(Writer);
            }
            foreach (HSDRoot r in Resources)
            {
                r.Save(Writer);
            }
            //write relocation table
            uint RelocOffset = (uint)Writer.BaseStream.Position;
            uint RelocCount = (uint)Writer.WriteRelocationTable();

            // Write Strings
            int stringoff = 0;
            foreach (HSDRoot r in Roots)
            {
                Writer.WritePointer(r.Node);
                Writer.Write((uint)stringoff);
                stringoff += r.Name.Length + 1;
            }
            foreach (HSDRoot r in Resources)
            {
                Writer.WritePointer(r.Node);
                Writer.Write((uint)stringoff);
                stringoff += r.Name.Length + 1;
            }

            foreach (HSDRoot r in Roots)
            {
                Writer.WriteNullString(r.Name);
            }
            foreach (HSDRoot r in Resources)
            {
                Writer.WriteNullString(r.Name);
            }

            //write root offsets and strings
            Writer.WriteAt(4, RelocOffset - 0x20);
            Writer.WriteAt(8, RelocCount);
            Writer.WriteAt(0, (uint)Writer.BaseStream.Position);

            Writer.WriteRelocationTable(false);

            Writer.Close();
        }
    }
}
