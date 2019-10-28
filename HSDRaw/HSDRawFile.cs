using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Melee.Gr;
using HSDRaw.Melee.Mn;
using HSDRaw.Melee.Pl;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using HSDRaw.AirRide.Vc;
using HSDRaw.Melee.Ef;
using HSDRaw.AirRide.Gr;
using HSDRaw.AirRide.Gr.Data;

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
        private Dictionary<HSDStruct, int> _structCacheToOffset = new Dictionary<HSDStruct, int>();

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
                        if(offsetToStruct.ContainsKey(offsets[i]))
                            str.Value.SetReferenceStruct(innerOffsets[i] - str.Key, offsetToStruct[offsets[i]]);
                    }

                    _structCache.Add(str.Value);
                    _structCacheToOffset.Add(str.Value, str.Key);
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
                    /*else
                    if (rootStrings[i].StartsWith("grGroundParam"))
                    {
                        var acc = new SBM_GroundParam();
                        acc._s = str;
                        a = acc;
                    }*/
                    else
                    if (rootStrings[i].StartsWith("map_head"))
                    {
                        var acc = new SBM_Map_Head();
                        acc._s = str;
                        a = acc;
                    }
                    else
                    if (rootStrings[i].StartsWith("vcDataStar"))
                    {
                        var acc = new KAR_vcDataStar();
                        acc._s = str;
                        a = acc;
                    }
                    else
                    if (rootStrings[i].StartsWith("vcDataWheel"))
                    {
                        var acc = new KAR_vcDataWheel();
                        acc._s = str;
                        a = acc;
                    }
                    else
                    if (rootStrings[i].StartsWith("grModelMotion"))
                    {
                        var acc = new KAR_grModelMotion();
                        acc._s = str;
                        a = acc;
                    }
                    else
                    if (rootStrings[i].StartsWith("grModel"))
                    {
                        var acc = new KAR_grModel();
                        acc._s = str;
                        a = acc;
                    }
                    else
                    if (rootStrings[i].StartsWith("grData"))
                    {
                        var acc = new KAR_grData();
                        acc._s = str;
                        a = acc;
                    }
                    else
                    if (rootStrings[i].EndsWith("_texg"))
                    {
                        var acc = new HSD_TEXGraphicBank();
                        acc._s = str;
                        a = acc;
                    }
                    else
                    if (rootStrings[i].StartsWith("eff"))
                    {
                        var acc = new SBM_EffectTable();
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
        public void Save(string fileName, bool bufferAlign = true, bool optimize = true)
        {
            Save(new FileStream(fileName, FileMode.Create), bufferAlign, optimize);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public bool IsBuffer(HSDStruct a)
        {
            return a.References.Count == 0 && a.Length >= 0x50;
        }

        //https://stackoverflow.com/questions/16340/how-do-i-generate-a-hashcode-from-a-byte-array-in-c/16381
        private static int ComputeHash(params byte[] data)
        {
            unchecked
            {
                const int p = 16777619;
                int hash = (int)2166136261;

                for (int i = 0; i < data.Length; i++)
                    hash = (hash ^ data[i]) * p;

                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;
                return hash;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private bool IsRoot(HSDStruct s)
        {
            foreach (var v in Roots)
                if (v.Data._s == s)
                    return true;

            foreach (var v in References)
                if (v.Data._s == s)
                    return true;

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void RemoveDuplicateBuffers()
        {
            Dictionary<int, HSDStruct> hashToStruct = new Dictionary<int, HSDStruct>();
            var toRemove = new List<HSDStruct>();
            foreach (var v in _structCache)
            {
                if (IsBuffer(v) && !IsRoot(v))
                {
                    var hash = ComputeHash(v.GetData());
                    if (hashToStruct.ContainsKey(hash))
                    {
                        // correct references and remove this hash
                        foreach (var s in _structCache)
                        {
                            var keys = s.References.Keys.ToArray();
                            foreach (var re in keys)
                            {
                                if (s.References[re] == v)
                                    s.References[re] = hashToStruct[hash];
                            }
                        }
                        toRemove.Add(v);
                    }
                    else
                    {
                        hashToStruct.Add(hash, v);
                    }
                }
            }
            foreach (var v in toRemove)
                _structCache.Remove(v);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<HSDStruct> GetAllStructs()
        {
            var allStructs = new List<HSDStruct>();
            foreach (var r in Roots)
            {
                foreach (var sub in r.Data._s.GetSubStructs())
                    if (!allStructs.Contains(sub))
                        allStructs.Add(sub);
            }
            foreach (var r in References)
            {
                foreach (var sub in r.Data._s.GetSubStructs())
                    if (!allStructs.Contains(sub))
                        allStructs.Add(sub);
            }
            return allStructs;
        }

        /// <summary>
        /// saves dat data to stream with optional alignment
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bufferAlign"></param>
        public void Save(Stream stream, bool bufferAlign = true, bool optimize = true)
        {
            // gather all structs--------------------------------------------------------------------------
            var allStructs = GetAllStructs();

            // struct cache cleanup
            // remove unused structs--------------------------------------------------------------------------
            var unused = new List<HSDStruct>();

            foreach (var s in _structCache)
            {
                if (!allStructs.Contains(s))
                    unused.Add(s);
            }
            if(optimize)
            foreach(var s in unused)
            {
                //TODO: this may be bugged?
                if(_structCache.Contains(s))
                    _structCache.Remove(s);
            }
            // add missing structs--------------------------------------------------------------------------
            foreach (var s in allStructs)
            {
                if (!_structCache.Contains(s))
                {
                    if (IsBuffer(s))
                        _structCache.Insert(0, s);
                    else
                        _structCache.Add(s);
                }
            }
            allStructs.Clear();

            // remove duplicate buffers--------------------------------------------------------------------------
            if(optimize && Roots.Count > 0 && !(Roots[0].Data is SBM_PlayerData))
                RemoveDuplicateBuffers();

            // build file --------------------------------------------------------------------------
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
                    if (IsBuffer(s) && bufferAlign) 
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


        /// <summary>
        /// Returns the cached offset for <see cref="HSDStruct"/> for this file.
        /// If struct is not cached then returns -1
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public int GetOffsetFromStruct(HSDStruct str)
        {
            if (_structCacheToOffset.ContainsKey(str))
                return _structCacheToOffset[str];

            return -1;
        }


        /// <summary>
        /// Returns a list of all structs that reference the given struct
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public List<HSDStruct> GetAllStructsThatReference(HSDStruct str)
        {
            var structs = GetAllStructs();

            var found = new List<HSDStruct>();

            foreach(var v in structs)
            {
                if (v.References.ContainsValue(str))
                    found.Add(v);
            }

            return found;
        }
    }
}
