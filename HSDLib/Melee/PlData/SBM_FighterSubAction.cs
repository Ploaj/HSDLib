using HSDLib.Helpers;
using System.Collections.Generic;
using System.ComponentModel;

namespace HSDLib.Melee.PlData
{
    public class SBM_SubAction : IHSDNode
    {
        public byte[] Data;

        public override void Open(HSDReader Reader)
        {
            List<byte> data = new List<byte>();
            byte cmd = Reader.ReadByte();
            while(cmd != 0)
            {
                int size = MeleeScript.GetCommandSize(cmd);
                data.Add(cmd);
                data.AddRange(Reader.ReadBytes(size - 1));
                cmd = Reader.ReadByte();
            }
            Data = data.ToArray();
        }

        public override void Save(HSDWriter Writer)
        {
            if (Writer.Mode == WriterWriteMode.BUFFER && Data.Length > 0)
            {
                Writer.AddBuffer(this);
                Writer.Write(Data);
                if (Writer.BaseStream.Position % 4 > 0)
                    Writer.Write(4 - (Writer.BaseStream.Position % 4));
            }
        }
    }

    public class SBM_FighterSubAction : IHSDNode
    {
        public string Name { get; set; }

        public int AnimationOffset { get; set; }

        public int AnimationSize { get; set; }

        public SBM_SubAction SubAction { get; set; }

        public uint Flags { get; set; }

        [Browsable(false)]
        public int AnimationPointerLocation { get { return 0; } set { } }
    }
}
