using HSDLib.Common;
using System.Collections.Generic;
using System.ComponentModel;

namespace HSDLib.KAR
{
    public class KAR_GrModel_Bounding : IHSDNode
    {
        [Browsable(false)]
        public uint OffsetUnk1_1 { get; internal set; }

        [Browsable(false)]
        public ushort SizeUnk1_1 { get; set; }

        [Browsable(false)]
        public ushort Padding { get; internal set; }

        public HSD_Array<KAR_GrViewRegion> ViewRegion;

        [Browsable(false)]
        public uint OffsetUnk1_2 { get; set; }

        [Browsable(false)]
        public ushort SizeUnk1_2 { get; set; }

        [Browsable(false)]
        public ushort Padding2 { get; set; }

        public HSD_Array<KAR_GrModel_ModelUnk1_2> GroupsUnk1_2;

        [Browsable(false)]
        public uint OffsetUnk1_3 { get; set; }

        [Browsable(false)]
        public ushort SizeUnk1_3 { get; set; }

        [Browsable(false)]
        public ushort Padding3 { get; set; }

        public HSD_Array<KAR_GrBoundingBox> GroupsUnk1_3;

        [Browsable(false)]
        public uint OffsetUnk1_4 { get; set; }

        [Browsable(false)]
        public ushort SizeUnk1_4 { get; set; }

        [Browsable(false)]
        public ushort Padding4 { get; set; }

        public List<ushort> DOBJIndices = new List<ushort>();

        public override void Open(HSDReader Reader)
        {
            Reader.PrintPosition();
            base.Open(Reader);

            ViewRegion = new HSD_Array<KAR_GrViewRegion>(SizeUnk1_1);
            Reader.Seek(OffsetUnk1_1);
            ViewRegion.Open(Reader);

            GroupsUnk1_2 = new HSD_Array<KAR_GrModel_ModelUnk1_2>(SizeUnk1_2);
            Reader.Seek(OffsetUnk1_2);
            GroupsUnk1_2.Open(Reader);

            GroupsUnk1_3 = new HSD_Array<KAR_GrBoundingBox>(SizeUnk1_3);
            Reader.Seek(OffsetUnk1_3);
            GroupsUnk1_3.Open(Reader);

            DOBJIndices.Clear();
            Reader.Seek(OffsetUnk1_4);
            for (int i = 0; i < SizeUnk1_4; i++)
                DOBJIndices.Add(Reader.ReadUInt16());
        }

        public override void Save(HSDWriter Writer)
        {
            foreach (KAR_GrViewRegion Unk1_1 in ViewRegion.Elements)
            {
                Writer.AddObject(Unk1_1.DOBJIndices);
                foreach (var v in Unk1_1.DOBJIndices)
                    Writer.Write(v);
                Writer.Align(4);
            }

            foreach (KAR_GrModel_ModelUnk1_2 Unk1_2 in GroupsUnk1_2.Elements)
            {
                Writer.AddObject(Unk1_2.DOBJIndices);
                foreach (var v in Unk1_2.DOBJIndices)
                    Writer.Write(v);
                Writer.Align(4);
            }

            ViewRegion.Save(Writer);
            Writer.Align(4);
            GroupsUnk1_2.Save(Writer);
            Writer.Align(4);
            GroupsUnk1_3.Save(Writer);
            Writer.Align(4);
            Writer.AddObject(DOBJIndices);
            foreach (var v in DOBJIndices)
                Writer.Write(v);
            Writer.Align(4);

            Writer.AddObject(this);

            Writer.WritePointer(ViewRegion.Size > 0 ? ViewRegion.Elements[0] : null);
            Writer.Write((ushort)ViewRegion.Size);
            Writer.Write((ushort)0);

            Writer.WritePointer(GroupsUnk1_2.Size > 0 ? GroupsUnk1_2.Elements[0] : null);
            Writer.Write((ushort)GroupsUnk1_2.Size);
            Writer.Write((ushort)0);

            Writer.WritePointer(GroupsUnk1_3.Size > 0 ? GroupsUnk1_3.Elements[0] : null);
            Writer.Write((ushort)GroupsUnk1_3.Size);
            Writer.Write((ushort)0);

            Writer.WritePointer(DOBJIndices);
            Writer.Write((ushort)DOBJIndices.Count);
            Writer.Write((ushort)0);
        }
    }
}
