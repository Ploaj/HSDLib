using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Melee.Gr;
using HSDRaw.Melee.Mn;
using HSDRaw.Melee.Pl;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace HSDRaw
{
    /// <summary>
    /// 
    /// </summary>
    public class HSDRootNode
    {
        public string Name { get; set; }
        public HSDAccessor Data { get; set; }
    }

    /// <summary>
    /// Holds raw HSD DAT structure information
    /// </summary>
    public class HSDRawFile
    {
        private char[] UnknownChars = new char[4];

        public List<HSDRootNode> Roots = new List<HSDRootNode>();
        public List<HSDRootNode> References = new List<HSDRootNode>();

        /// <summary>
        /// Trying to keep the order of structs intact if possible
        /// </summary>
        private List<HSDStruct> _structCache = new List<HSDStruct>();
        
        /// <summary>
        /// 
        /// </summary>
        public HSDRawFile()
        {

        }

        /// <summary>
        /// Loads dat data from file path
        /// </summary>
        /// <param name="filePath"></param>
        public HSDRawFile(string filePath)
        {
            Open(filePath);
        }

        /// <summary>
        /// Loads dat data from byte array
        /// </summary>
        /// <param name="data"></param>
        public HSDRawFile(byte[] data)
        {
            Open(new MemoryStream(data));
        }

        /// <summary>
        /// Opens dat file from path
        /// </summary>
        /// <param name="filePath"></param>
        public void Open(string filePath)
        {
            Open(new FileStream(filePath, FileMode.Open));
        }

        /// <summary>
        /// Opens dat file from stream
        /// </summary>
        /// <param name="stream"></param>
        public void Open(Stream stream)
        {
            using (BinaryReaderExt r = new BinaryReaderExt(stream))
            {
                r.BigEndian = true;

                // Parse Header -----------------------------
                r.ReadInt32(); // dat size
                int relocOffset = r.ReadInt32() + 0x20;
                int relocCount = r.ReadInt32();
                int rootCount = r.ReadInt32();
                int refCount = r.ReadInt32();
                UnknownChars = r.ReadChars(4);
                
                // Parse Relocation Table -----------------------------
                List<int> Offsets = new List<int>();
                HashSet<int> OffsetContain = new HashSet<int>();
                Dictionary<int, int> relocOffsets = new Dictionary<int, int>();
                Offsets.Add(relocOffset);

                r.BaseStream.Position = relocOffset;
                for (int i = 0; i < relocCount; i++)
                {
                    int offset = r.ReadInt32() + 0x20;

                    var temp = r.BaseStream.Position;

                    r.BaseStream.Position = offset;
                    var objectOff = r.ReadInt32() + 0x20;

                    relocOffsets.Add(offset, objectOff);

                    if (!OffsetContain.Contains(objectOff))
                    {
                        OffsetContain.Add(objectOff);
                        Offsets.Add(objectOff);
                    }

                    r.BaseStream.Position = temp;
                }

                // Parse Roots---------------------------------
                List<int> rootOffsets = new List<int>();
                List<string> rootStrings = new List<string>();
                List<int> refOffsets = new List<int>();
                List<string> refStrings = new List<string>();
                var stringStart = r.BaseStream.Position + (refCount + rootCount) * 8;
                for (int i = 0; i < rootCount; i++)
                {
                    rootOffsets.Add(r.ReadInt32() + 0x20);
                    rootStrings.Add(r.ReadString((int)stringStart + r.ReadInt32(), -1));
                }
                for (int i = 0; i < refCount; i++)
                {
                    refOffsets.Add(r.ReadInt32() + 0x20);
                    refStrings.Add(r.ReadString((int)stringStart + r.ReadInt32(), -1));
                }
                foreach (var v in rootOffsets)
                    if (!OffsetContain.Contains(v))
                    {
                        OffsetContain.Add(v);
                        Offsets.Add(v);
                    }
                foreach (var v in refOffsets)
                    if (!OffsetContain.Contains(v))
                    {
                        OffsetContain.Add(v);
                        Offsets.Add(v);
                    }


                // Split Raw Struct Data--------------------------
                Offsets.Sort();

                Dictionary<int, HSDStruct> offsetToStruct = new Dictionary<int, HSDStruct>();
                Dictionary<int, List<int>> offsetToOffsets = new Dictionary<int, List<int>>();
                Dictionary<int, List<int>> offsetToInnerOffsets = new Dictionary<int, List<int>>();

                var relockeys = relocOffsets.Keys.ToList();
                for (int i = 0; i < Offsets.Count - 1; i++)
                {
                    r.BaseStream.Position = Offsets[i];
                    byte[] data = r.ReadBytes(Offsets[i + 1] - Offsets[i]);

                    if (!offsetToOffsets.ContainsKey(Offsets[i]))
                    {
                        var relocKets = relockeys.FindAll(e => e >= Offsets[i] && e < Offsets[i + 1]);
                        var list = new List<int>();
                        foreach (var k in relocKets)
                            list.Add(relocOffsets[k]);
                        offsetToOffsets.Add(Offsets[i], list);
                        offsetToInnerOffsets.Add(Offsets[i], relocKets);
                    }

                    if (!offsetToStruct.ContainsKey(Offsets[i]))
                    {
                        var struture = new HSDStruct(data);

                        offsetToStruct.Add(Offsets[i], struture);
                    }
                }

                // set references-------------------------
                foreach(var str in offsetToStruct)
                {
                    var offsets = offsetToOffsets[str.Key];
                    var innerOffsets = offsetToInnerOffsets[str.Key];
                    for(int i = 0; i < offsets.Count; i++)
                    {
                        str.Value.SetReferenceStruct(innerOffsets[i] - str.Key, offsetToStruct[offsets[i]]);
                    }

                    _structCache.Add(str.Value);
                }

                // set roots
                for (int i = 0; i < rootOffsets.Count; i++)
                {
                    HSDStruct str = offsetToStruct[rootOffsets[i]];
                    HSDAccessor a = new HSDAccessor();
                    a._s = str;
                    if (rootStrings[i].EndsWith("matanim_joint"))
                    {
                        var acc = new HSD_MatAnimJoint();
                        acc._s = str;
                        a = acc;
                    }
                    else
                    if (rootStrings[i].EndsWith("_joint"))
                    {
                        var jobj = new HSD_JOBJ();
                        jobj._s = str;
                        a = jobj;
                    }
                    else
                    if (rootStrings[i].EndsWith("_animjoint"))
                    {
                        var jobj = new HSD_AnimJoint();
                        jobj._s = str;
                        a = jobj;
                    }
                    else
                    if (rootStrings[i].EndsWith("_figatree"))
                    {
                        var jobj = new HSD_FigaTree();
                        jobj._s = str;
                        a = jobj;
                    }
                    else
                    if (rootStrings[i].StartsWith("ftData"))
                    {
                        var acc = new SBM_PlayerData();
                        acc._s = str;
                        a = acc;
                    }
                    else
                    if (rootStrings[i].EndsWith("MnSelectChrDataTable"))
                    {
                        var acc = new SBM_SelectChrDataTable();
                        acc._s = str;
                        a = acc;
                    }
                    else
                    if (rootStrings[i].EndsWith("MnSelectStageDataTable"))
                    {
                        var acc = new SBM_MnSelectStageDataTable();
                        acc._s = str;
                        a = acc;
                    }
                    else
                    if (rootStrings[i].EndsWith("coll_data"))
                    {
                        var acc = new SBM_Coll_Data();
                        acc._s = str;
                        a = acc;
                    }
                    else
                    if (rootStrings[i].StartsWith("Sc"))
                    {
                        var acc = new HSD_SOBJ();
                        acc._s = str;
                        a = acc;
                    }
                    else
                    if (rootStrings[i].StartsWith("map_plit"))
                    {
                        var acc = new HSDNullPointerArrayAccessor<HSD_Light>();
                        acc._s = str;
                        a = acc;
                    }
                    else
                    if (rootStrings[i].StartsWith("map_head"))
                    {
                        var acc = new SBM_Map_Head();
                        acc._s = str;
                        a = acc;
                    }

                    Roots.Add(new HSDRootNode() { Name = rootStrings[i], Data = a });
                }


                // set references
                for (int i = 0; i < refOffsets.Count; i++)
                {
                    HSDStruct str = offsetToStruct[refOffsets[i]];
                    HSDAccessor a = new HSDAccessor();
                    a._s = str;
                    References.Add(new HSDRootNode() { Name = refStrings[i], Data = a });
                }
            }
        }

        /// <summary>
        /// Saves dat data to filepath
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName)
        {
            Save(new FileStream(fileName, FileMode.Create));
        }

        /// <summary>
        /// saves dat data to stream with optional alignment
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bufferAlign"></param>
        public void Save(Stream stream, bool bufferAlign = true)
        {
            // gather all structs
            var allStructs = new List<HSDStruct>();
            foreach (var r in Roots)
            {
                foreach(var sub in r.Data._s.GetSubStructs())
                    if(!allStructs.Contains(sub))
                        allStructs.Add(sub);
            }
            foreach (var r in References)
            {
                foreach (var sub in r.Data._s.GetSubStructs())
                    if (!allStructs.Contains(sub))
                        allStructs.Add(sub);
            }

            // struct cache cleanup

            // remove unused structs
            var unused = new List<HSDStruct>();
            //Console.WriteLine(_structCache.Count + " " + allStructs.Count);
            foreach (var s in _structCache)
            {
                if (!allStructs.Contains(s))
                    unused.Add(s);
            }
            foreach(var s in unused)
            {
                //TODO: this may be bugged
                Console.WriteLine("removing 0x" + s.GetData().Length.ToString("X"));
                _structCache.Remove(s);
            }

            // add missing structs
            foreach (var s in allStructs)
            {
                if(!_structCache.Contains(s))
                    _structCache.Add(s);
            }
            allStructs.Clear();

            using (BinaryWriterExt writer = new BinaryWriterExt(stream))
            {
                writer.BigEndian = true;

                writer.Write(new byte[0x20]);

                Dictionary<HSDStruct, int> structToOffset = new Dictionary<HSDStruct, int>(); 
                
                // write structs
                foreach (var s in _structCache)
                {
                    // align buffers and general alignment
                    // TODO: trim extra data? Not a problem aside from potential filesize
                    // no refereneces = buffer?
                    // textures need to be 0x20 aligned... but there is no way to detect which structures are textures
                    // this can result in unnessecary padding
                    if (s.Length > 0x50 && s.References.Count == 0 && bufferAlign) 
                        writer.Align(0x20);
                    else
                        writer.Align(4);

                    structToOffset.Add(s, (int)writer.BaseStream.Position - 0x20);
                    writer.Write(s.GetData());
                }
                
                writer.Align(4);
                var relocationOffset = writer.BaseStream.Position;

                List<int> relocationOffsets = new List<int>();

                // fix references
                foreach (var s in _structCache)
                {
                    var offset = structToOffset[s];
                    foreach(var v in s.References)
                    {
                        relocationOffsets.Add(offset + v.Key);
                        writer.Seek(offset + v.Key + 0x20, SeekOrigin.Begin);
                        writer.Write(structToOffset[v.Value]);
                    }
                }

                // write reloc table
                writer.Seek((int)relocationOffset, SeekOrigin.Begin);
                foreach (var v in relocationOffsets)
                {
                    writer.Write(v);
                }

                // write root and reference node info
                MemoryStream stringstream = new MemoryStream();
                using (BinaryWriterExt stringWriter = new BinaryWriterExt(stringstream))
                {
                    foreach(var root in Roots)
                    {
                        writer.Write(structToOffset[root.Data._s]);
                        writer.Write((uint)stringstream.Position);
                        stringWriter.Write(root.Name);
                    }
                    foreach (var root in References)
                    {
                        writer.Write(structToOffset[root.Data._s]);
                        writer.Write((uint)stringstream.Position);
                        stringWriter.Write(root.Name);
                    }
                    writer.Write(stringstream.ToArray());
                }

                // finalize
                writer.Seek(0, SeekOrigin.Begin);
                writer.Write((uint)writer.Length);
                writer.Write((uint)relocationOffset - 0x20);
                writer.Write(relocationOffsets.Count);
                writer.Write(Roots.Count);
                writer.Write(References.Count);
                writer.Write(UnknownChars);
            }
        }

    }
}
