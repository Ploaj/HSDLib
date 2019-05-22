using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSDLib.Common;

namespace HSDLib.KAR
{
    public class KAR_VcModelData : IHSDNode
    {
        public HSD_JOBJ JOBJRoot { get; set; }
        
        public int Unk1 { get; set; }
        
        public byte BoneCount { get; set; }
        
        public byte Unk2 { get; set; }
        
        public byte Unk4 { get; set; }
        
        public byte Unk5 { get; set; }
        
        public int Unk3 { get; set; }
        
        public KAR_ModelUnk1 ModelStruct1 { get; set; }
        
        public KAR_ModelUnk1 ModelStruct2 { get; set; }
        
        public KAR_ModelUnk1 ModelStruct3 { get; set; }
        
        public KAR_ModelUnk1 ModelStruct4 { get; set; }
        
        public KAR_ModelUnk1 ModelStruct5 { get; set; }
        
        public KAR_ModelUnk1 ModelStruct6 { get; set; }
        
        public HSD_JOBJ ShadowJOBJRoot { get; set; }
    }

    public class KAR_ModelUnk1 : IHSDNode
    {
        public uint HasData { get; set; }
        
        public KAR_ModelUnk1_1 ModelData { get; set; }
    }

    public class KAR_ModelUnk1_1 : IHSDNode
    {
        public uint DataLength { get; set; }

        [System.ComponentModel.Browsable(false)]
        public uint Offset { get; set; }

        public byte[] Data;

        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);
            
            Data = Reader.ReadBuffer(Offset, (int)DataLength);
        }

        public override void Save(HSDWriter Writer)
        {
            Writer.WriteBuffer(Data);

            base.Save(Writer);

            Writer.WritePointerAt((int)(Writer.BaseStream.Position - 4), Data);
        }
    }
}
