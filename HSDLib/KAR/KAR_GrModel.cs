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

        [FieldData(typeof(uint), Viewable: false)]
        public uint ArrayOffset { get; set; }

        public HSD_Array<KAR_GrModel> UnkGroup { get; set; }

        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);

            Reader.Seek(ArrayOffset);
            //UnkGroup = new HSD_Array<KAR_GrModel>(4);
            //UnkGroup.Open(Reader);
        }

        public override void Save(HSDWriter Writer)
        {
            UnkGroup.Save(Writer);

            base.Save(Writer);

            Writer.WritePointerAt((int)Writer.BaseStream.Position - 4, UnkGroup);
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
            Reader.PrintPosition();
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
