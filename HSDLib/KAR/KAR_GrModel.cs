using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSDLib.Common;

namespace HSDLib.KAR
{
    public class KAR_GrModel : IHSDNode
    {
        [FieldData(typeof(KAR_GrMainModel))]
        public KAR_GrMainModel MainModel { get; set; }

        [FieldData(typeof(KAR_GrSkyboxModel))]
        public KAR_GrSkyboxModel SkyboxModel { get; set; }

        // 2 root unks TODO:
    }


    public class KAR_GrMainModel : IHSDNode
    {
        [FieldData(typeof(HSD_JOBJ))]
        public HSD_JOBJ JOBJRoot { get; set; }
        
        [FieldData(typeof(uint))]
        public uint Unk1 { get; set; }

        [FieldData(typeof(uint))]
        public uint Unk2 { get; set; }

        [FieldData(typeof(uint))]
        public uint Unk3 { get; set; }

        [FieldData(typeof(KAR_GrModel_Unk))]
        public KAR_GrModel_Unk UnkGroup { get; set; }
    }

    public class KAR_GrModel_Unk : IHSDNode
    {
        [FieldData(typeof(KAR_GrModel_Unk1), InLine: true)]
        public KAR_GrModel_Unk1 Group1 { get; set; }
        [FieldData(typeof(KAR_GrModel_Unk1), InLine: true)]
        public KAR_GrModel_Unk1 Group2 { get; set; }
        [FieldData(typeof(KAR_GrModel_Unk1), InLine: true)]
        public KAR_GrModel_Unk1 Group3 { get; set; }
        [FieldData(typeof(KAR_GrModel_Unk1), InLine: true)]
        public KAR_GrModel_Unk1 Group4 { get; set; }

        public override void Open(HSDReader Reader)
        {
            uint start = Reader.Position();
            Reader.Seek(start);
            Group1 = new KAR_GrModel_Unk1();
            Group1.Open(Reader);
            Reader.Seek(start + 8);
            Group2 = new KAR_GrModel_Unk1();
            Group2.Open(Reader);
            Reader.Seek(start + 16);
            Group3 = new KAR_GrModel_Unk1();
            Group3.Open(Reader);
            Reader.Seek(start + 24);
            Group4 = new KAR_GrModel_Unk1();
            Group4.Open(Reader);
        }
    }

    public class KAR_GrModel_Unk1 : IHSDNode
    {
        [FieldData(typeof(uint), Viewable: false)]
        public uint Offset { get; set; }

        [FieldData(typeof(ushort), Viewable: false)]
        public ushort UnkSize { get; set; }

        [FieldData(typeof(ushort), Viewable: false)]
        public ushort Padding { get; set; }

        public List<KAR_GrModel_UnkGroup> Groups { get; set; }

        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);

            Groups = new List<KAR_GrModel_UnkGroup>(UnkSize);
            //Reader.Seek(Offset);
            for (int i = 0; i < UnkSize; i++)
            {
                Groups.Add(Reader.ReadObject<KAR_GrModel_UnkGroup>((uint)(Offset + 0x20 * i)));
            }
        }

        public override void Save(HSDWriter Writer)
        {
            foreach (KAR_GrModel_UnkGroup group in Groups)
                Writer.WriteObject(group);

            Writer.AddObject(this);
            Writer.WritePointer(Groups != null ? Groups[0] : null);
            Writer.Write((ushort)Groups.Count);
            Writer.Write((ushort)0);
        }
    }

    public class KAR_GrModel_UnkGroup : IHSDNode
    {
        [FieldData(typeof(uint))]
        public uint Offset { get; set; }

        [FieldData(typeof(ushort))]
        public ushort UnkSize { get; set; }

        [FieldData(typeof(ushort), Viewable: false)]
        public ushort Padding { get; set; }
        
        [FieldData(typeof(float))]
        public float UnkFloat1 { get; set; }

        [FieldData(typeof(float))]
        public float UnkFloat2 { get; set; }

        [FieldData(typeof(float))]
        public float UnkFloat3 { get; set; }

        [FieldData(typeof(float))]
        public float UnkFloat4 { get; set; }

        [FieldData(typeof(float))]
        public float UnkFloat5 { get; set; }

        [FieldData(typeof(float))]
        public float UnkFloat6 { get; set; }
    }

    public class KAR_GrSkyboxModel : IHSDNode
    {
        [FieldData(typeof(HSD_JOBJ))]
        public HSD_JOBJ JOBJRoot { get; set; }

        [FieldData(typeof(uint))]
        public uint Unknown { get; set; }
    }
}
