using HSDRaw.Melee.Pl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HSDRaw.Tools.Melee
{
    /// <summary>
    /// Manager interface for melee's fighter AJ files
    /// </summary>
    public class FighterAJManager
    {
        private class AnimationIndexingData
        {
            public string Symbol { get; internal set; } = "";

            public int Offset = -1;

            public int Size { get => Data == null ? 0 : Data.Length; }

            public byte[] Data
            {
                get => _data;
                set
                {
                    var symbols = GetSymbols(value);

                    if (symbols.Length != 0)
                    {
                        Symbol = symbols[0];
                        _data = value;

                    }
                }
            }
            private byte[] _data = new byte[0];

            /// <summary>
            /// 
            /// </summary>
            /// <param name="data"></param>
            public AnimationIndexingData(byte[] data)
            {
                Data = data;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="newSymbol"></param>
            public void Rename(string newSymbol)
            {
                HSDRawFile f = new HSDRawFile(_data);
                f.Roots[0].Name = newSymbol;
                using (MemoryStream output = new MemoryStream())
                {
                    f.Save(output);
                    Data = output.ToArray();
                }
            }

            /// <summary>
            /// return true if animation symbol is okay
            /// </summary>
            /// <returns></returns>
            private static string[] GetSymbols(byte[] data)
            {
                if (data == null || data.Length < 0x20)
                    return new string[0];

                using (MemoryStream stream = new MemoryStream(data))
                using (BinaryReaderExt r = new BinaryReaderExt(stream))
                {
                    r.BigEndian = true;
                    var fs = r.ReadInt32();

                    if (fs != data.Length)
                        return new string[0];

                    try
                    {
                        var reloc_off = r.ReadInt32() + 0x20;
                        var reloc_count = r.ReadInt32();
                        var sym_count = r.ReadInt32();

                        string[] symbols = new string[sym_count];

                        var string_table = reloc_off + (reloc_count + sym_count + 1) * 4;

                        for (int i = 0; i < sym_count; i++)
                        {
                            r.Position = (uint)(reloc_off + (reloc_count + i + 1) * 4);
                            symbols[i] = r.ReadString(r.ReadInt32() + string_table, -1);
                        }

                        return symbols;
                    }
                    catch
                    {
                        return new string[0];
                    }

                }
            }
        }

        private List<AnimationIndexingData> Animations = new List<AnimationIndexingData>();

        //private byte[] AJFile;
        //private byte[] ResultFile; 0-9
        //private byte[] IntroFile; 10-11
        //private byte[] EndingFile; 12
        //private byte[] WaitFile; 13
        // 14 and 15 are mario and luigi exclusive

        /*
         * Get list of all animations by symbol
         * get animation data by symbol (validate)
         * set animation data by symbol (as all animation types + file)
         * rename symbol
         * rebuild aj file, result file, and intro file and save changes to fighter data struct
         * 
         */

        /// <summary>
        /// 
        /// </summary>
        public FighterAJManager()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public FighterAJManager(byte[] data)
        {
            ScanAJData(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] RebuildAJFile(string[] symbols, bool storeUnused)
        {
            // collect 
            var usedAnims = Animations
                .GroupBy(p => p.Symbol)
                .Select(g => g.First())
                .ToList();
            
            // optional not store unused
            if (!storeUnused)
                usedAnims = usedAnims.Where(e => symbols.Contains(e.Symbol)).ToList();

            // clear anim offsets
            foreach (var a in Animations)
                a.Offset = -1;

            // build new AJ file
            using (MemoryStream stream = new MemoryStream())
            {
                foreach (var a in usedAnims)
                {
                    // update anim offset
                    a.Offset = (int)stream.Position;

                    // write file data
                    stream.Write(a.Data, 0, a.Size);

                    // align 0x20
                    while (stream.Position % 0x20 != 0)
                        stream.WriteByte(0xFF);
                }

                Edited = false;

                return stream.ToArray();
            }
        }

        /// <summary>
        /// Must Call Rebuild First
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public Tuple<int, int> GetOffsetSize(string symbol)
        {
            if (symbol == null || Animations == null)
                return new Tuple<int, int>(0, 0);

            var data = Animations.Find(e => e.Symbol == symbol);

            if (data == null)
                return new Tuple<int, int>(0, 0);
            else
                return new Tuple<int, int>(data.Offset, data.Size);
        }

        /// <summary>
        /// Returns the symbol that is scanned
        /// </summary>
        /// <param name="filePath"></param>
        public string ScanAJFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                HSDRawFile f = new HSDRawFile(filePath);
                if (f.Roots.Count > 0)
                {
                    ScanAJData(f.Roots[0].Data._s.GetData());
                    return f.Roots[0].Name;
                }
            }

            return null;
        }

        /// <summary>
        /// Manually scans aj file and sets animations
        /// </summary>
        /// <param name="data"></param>
        public void ScanAJData(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            using (BinaryReaderExt r = new BinaryReaderExt(stream))
            {
                r.BigEndian = true;

                while (r.Position < stream.Length)
                {
                    // read dat file
                    var fsize = r.ReadInt32();
                    r.Position -= 4;

                    if (fsize > 0)
                    {
                        var file = r.ReadBytes(fsize);

                        // align in order to move to next
                        if (r.Position % 0x20 != 0)
                            r.Position += 0x20 - (r.Position % 0x20);

                        // add symbol data
                        Animations.Add(new AnimationIndexingData(file));
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Returns all loaded animation symbols
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAnimationSymbols()
        {
            return Animations.Select(e => e.Symbol);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public byte[] GetAnimationData(string symbol)
        {
            if (symbol == null || Animations == null)
                return null;

            var data = Animations.Find(e => e.Symbol == symbol);

            if (data == null)
                return null;
            else
                return data.Data;
        }

        public bool Edited { get; internal set; } = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="newsymbol"></param>
        /// <returns></returns>
        public bool RenameAnimation(string symbol, string newsymbol)
        {
            if (symbol == null || Animations == null)
                return false;

            var data = Animations.Find(e => e.Symbol == symbol);

            if (data == null)
                return false;
            else
            {
                data.Rename(newsymbol);
                Edited = true;
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void RemoveAnimation(string symbol)
        {
            Animations.RemoveAll(e => e.Symbol == symbol);
            Edited = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="data"></param>
        public void SetAnimation(string symbol, byte[] data)
        {
            if (symbol == null || Animations == null)
                return;

            var indexdata = Animations.Find(e => e.Symbol == symbol);
            Edited = true;

            if (indexdata == null)
                Animations.Add(new AnimationIndexingData(data));
            else
                indexdata.Data = data;
        }

    }
}
