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
        [FieldData(typeof(HSD_JOBJ))]
        public HSD_JOBJ JOBJRoot { get; set; }

        [FieldData(typeof(int))]
        public int Unk1 { get; set; }
        
        [FieldData(typeof(byte))]
        public byte BoneCount { get; set; }

        [FieldData(typeof(byte))]
        public byte Unk2 { get; set; }

        [FieldData(typeof(byte))]
        public byte Unk4 { get; set; }

        [FieldData(typeof(byte))]
        public byte Unk5 { get; set; }

        [FieldData(typeof(int))]
        public int Unk3 { get; set; }
        
        [FieldData(typeof(KAR_ModelUnk1))]
        public KAR_ModelUnk1 ModelStruct1 { get; set; }

        [FieldData(typeof(KAR_ModelUnk1))]
        public KAR_ModelUnk1 ModelStruct2 { get; set; }

        [FieldData(typeof(KAR_ModelUnk1))]
        public KAR_ModelUnk1 ModelStruct3 { get; set; }

        [FieldData(typeof(KAR_ModelUnk1))]
        public KAR_ModelUnk1 ModelStruct4 { get; set; }

        [FieldData(typeof(KAR_ModelUnk1))]
        public KAR_ModelUnk1 ModelStruct5 { get; set; }

        [FieldData(typeof(KAR_ModelUnk1))]
        public KAR_ModelUnk1 ModelStruct6 { get; set; }

        [FieldData(typeof(HSD_JOBJ))]
        public HSD_JOBJ ShadowJOBJRoot { get; set; }
    }

    public class KAR_ModelUnk1 : IHSDNode
    {
        [FieldData(typeof(uint))]
        public uint HasData { get; set; }

        [FieldData(typeof(KAR_ModelUnk1_1))]
        public KAR_ModelUnk1_1 ModelData { get; set; }
    }

    public class KAR_ModelUnk1_1 : IHSDNode
    {
        [FieldData(typeof(uint))]
        public uint DataLength { get; set; }

        [FieldData(typeof(uint), Viewable: false)]
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
