using System;
using System.Collections.Generic;

namespace HSDLib.Melee
{
    public class SBM_GrMapTexG : IHSDNode
    {
        public List<SBM_GrMapSpriteSheet> Sheets { get; set; } = new List<SBM_GrMapSpriteSheet>();
        
        public override void Open(HSDReader Reader)
        {
            var start = Reader.Position();
            var count = Reader.ReadInt32();

            for(int i = 0; i < count; i++)
            {
                var off = Reader.ReadUInt32();
                var temp = Reader.Position();
                
                Reader.Seek(start + off);
                SBM_GrMapSpriteSheet sheet = new SBM_GrMapSpriteSheet();
                sheet.Open(Reader, start);
                Sheets.Add(sheet);
                Reader.Seek(temp);
            }

            // there is sometimes padding with values in it
            // there is a 0xC section that has 0, 0x20, and some number
        }

        public override void Save(HSDWriter Writer)
        {
            throw new Exception("Saving TexMapG not supported");
        }
    }

    public class SBM_GrMapSpriteSheet : IHSDNode
    {
        public int Format { get; set; }

        public int Flag { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Depth { get; set; }

        public List<byte[]> Images { get; set; } = new List<byte[]>();

        public void Open(HSDReader Reader, uint offset)
        {
            var mipCount = Reader.ReadInt32();
            Format = Reader.ReadInt32();
            Flag = Reader.ReadInt32();
            Width = Reader.ReadInt32();
            Height = Reader.ReadInt32();
            Depth = Reader.ReadInt32();

            uint[] mipOffsets = new uint[mipCount];
            for (int i = 0; i < mipCount; i++)
                mipOffsets[i] = Reader.ReadUInt32();
        }

        public override void Save(HSDWriter Writer)
        {
            throw new Exception("Saving TexMapG not supported");
        }
    }
}
