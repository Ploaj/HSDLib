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

        [FieldData(typeof(KAR_GrUnk2Model))]
        public KAR_GrUnk2Model Unk2Model { get; set; }

        [FieldData(typeof(KAR_GrUnk3Model))]
        public KAR_GrUnk3Model Unk3Model { get; set; }
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

        [FieldData(typeof(KAR_GrModel_ModelUnk1))]
        public KAR_GrModel_ModelUnk1 ModelUnk1 { get; set; }
    }

    public class KAR_GrModel_ModelUnk1 : IHSDNode
    {
        [FieldData(typeof(uint), Viewable: false)]
        public uint OffsetUnk1_1 { get; set; }

        [FieldData(typeof(ushort), Viewable: false)]
        public ushort SizeUnk1_1 { get; set; }

        [FieldData(typeof(ushort), Viewable: false)]
        public ushort Padding { get; set; }

        public HSD_Array<KAR_GrModel_ModelUnk1_1> GroupsUnk1_1 { get; set; }

        [FieldData(typeof(uint), Viewable: false)]
        public uint OffsetUnk1_2 { get; set; }

        [FieldData(typeof(ushort), Viewable: false)]
        public ushort SizeUnk1_2 { get; set; }

        [FieldData(typeof(ushort), Viewable: false)]
        public ushort Padding2 { get; set; }

        public HSD_Array<KAR_GrModel_ModelUnk1_2> GroupsUnk1_2 { get; set; }

        [FieldData(typeof(uint), Viewable: false)]
        public uint OffsetUnk1_3 { get; set; }

        [FieldData(typeof(ushort), Viewable: false)]
        public ushort SizeUnk1_3 { get; set; }

        [FieldData(typeof(ushort), Viewable: false)]
        public ushort Padding3 { get; set; }

        public HSD_Array<KAR_GrModel_ModelUnk1_3> GroupsUnk1_3 { get; set; }

        [FieldData(typeof(uint), Viewable: false)]
        public uint OffsetUnk1_4 { get; set; }

        [FieldData(typeof(ushort), Viewable: false)]
        public ushort SizeUnk1_4 { get; set; }

        [FieldData(typeof(ushort), Viewable: false)]
        public ushort Padding4 { get; set; }

        public HSD_Array<KAR_GrModel_ModelUnk1_4> GroupsUnk1_4 { get; set; }

        public override void Open(HSDReader Reader)
        {
            Reader.PrintPosition();
            base.Open(Reader);

            GroupsUnk1_1 = new HSD_Array<KAR_GrModel_ModelUnk1_1>(SizeUnk1_1);
            Reader.Seek(OffsetUnk1_1);
            GroupsUnk1_1.Open(Reader);

            GroupsUnk1_2 = new HSD_Array<KAR_GrModel_ModelUnk1_2>(SizeUnk1_2);
            Reader.Seek(OffsetUnk1_2);
            GroupsUnk1_2.Open(Reader);

            GroupsUnk1_3 = new HSD_Array<KAR_GrModel_ModelUnk1_3>(SizeUnk1_3);
            Reader.Seek(OffsetUnk1_3);
            GroupsUnk1_3.Open(Reader);

            GroupsUnk1_4 = new HSD_Array<KAR_GrModel_ModelUnk1_4>(SizeUnk1_4);
            Reader.Seek(OffsetUnk1_4);
            GroupsUnk1_4.Open(Reader);
        }
        
        public override void Save(HSDWriter Writer)
        {
            foreach (KAR_GrModel_ModelUnk1_1 Unk1_1 in GroupsUnk1_1.Elements)
            {
                Unk1_1.GroupsUnk1_4.Save(Writer);
                Writer.Align(4);
            }

            foreach (KAR_GrModel_ModelUnk1_2 Unk1_2 in GroupsUnk1_2.Elements)
            {
                Unk1_2.GroupsUnk1_4.Save(Writer);
                Writer.Align(4);
            }

            GroupsUnk1_1.Save(Writer);
            Writer.Align(4);
            GroupsUnk1_2.Save(Writer);
            Writer.Align(4);
            GroupsUnk1_3.Save(Writer);
            Writer.Align(4);
            GroupsUnk1_4.Save(Writer);
            Writer.Align(4);

            Writer.AddObject(this);

            Writer.WritePointer(GroupsUnk1_1.Size > 0 ? GroupsUnk1_1.Elements[0] : null);
            Writer.Write((ushort)GroupsUnk1_1.Size);
            Writer.Write((ushort)0);

            Writer.WritePointer(GroupsUnk1_2.Size > 0 ? GroupsUnk1_2.Elements[0] : null);
            Writer.Write((ushort)GroupsUnk1_2.Size);
            Writer.Write((ushort)0);

            Writer.WritePointer(GroupsUnk1_3.Size > 0 ? GroupsUnk1_3.Elements[0] : null);
            Writer.Write((ushort)GroupsUnk1_3.Size);
            Writer.Write((ushort)0);

            Writer.WritePointer(GroupsUnk1_4.Size > 0 ? GroupsUnk1_4.Elements[0] : null);
            Writer.Write((ushort)GroupsUnk1_4.Size);
            Writer.Write((ushort)0);
        }
    }

    public class KAR_GrModel_ModelUnk1_1 : IHSDNode
    {
        [FieldData(typeof(uint))]
        public uint Offset { get; set; }

        [FieldData(typeof(ushort))]
        public ushort Size { get; set; }

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

        public HSD_Array<KAR_GrModel_ModelUnk1_4> GroupsUnk1_4 { get; set; }

        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);
            uint BaseOffset = Reader.Position();
            
            GroupsUnk1_4 = new HSD_Array<KAR_GrModel_ModelUnk1_4>(Size);
            Reader.Seek(Offset);
            GroupsUnk1_4.Open(Reader);

            Reader.Seek(BaseOffset);
        }

        public override void Save(HSDWriter Writer)
        {
            Writer.AddObject(this);

            Writer.WritePointer(GroupsUnk1_4.Size > 0 ? GroupsUnk1_4.Elements[0] : null);
            Writer.Write((ushort)GroupsUnk1_4.Size);
            Writer.Write((ushort)0);

            Writer.Write(UnkFloat1);
            Writer.Write(UnkFloat2);
            Writer.Write(UnkFloat3);
            Writer.Write(UnkFloat4);
            Writer.Write(UnkFloat5);
            Writer.Write(UnkFloat6);
        }
    }

    public class KAR_GrModel_ModelUnk1_2 : IHSDNode
    {
        [FieldData(typeof(uint))]
        public uint Offset { get; set; }

        [FieldData(typeof(ushort))]
        public ushort Size { get; set; }

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

        [FieldData(typeof(uint))]
        public uint UnkUInt7 { get; set; }

        public HSD_Array<KAR_GrModel_ModelUnk1_4> GroupsUnk1_4 { get; set; }

        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);
            uint BaseOffset = Reader.Position();

            GroupsUnk1_4 = new HSD_Array<KAR_GrModel_ModelUnk1_4>(Size);
            Reader.Seek(Offset);
            GroupsUnk1_4.Open(Reader);

            Reader.Seek(BaseOffset);
        }

        public override void Save(HSDWriter Writer)
        {
            Writer.AddObject(this);

            Writer.WritePointer(GroupsUnk1_4.Size > 0 ? GroupsUnk1_4.Elements[0] : null);
            Writer.Write((ushort)GroupsUnk1_4.Size);
            Writer.Write((ushort)0);

            Writer.Write(UnkFloat1);
            Writer.Write(UnkFloat2);
            Writer.Write(UnkFloat3);
            Writer.Write(UnkFloat4);
            Writer.Write(UnkFloat5);
            Writer.Write(UnkFloat6);
            Writer.Write(UnkUInt7);
        }
    }

    public class KAR_GrModel_ModelUnk1_3 : IHSDNode
    {
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

    public class KAR_GrModel_ModelUnk1_4 : IHSDNode
    {
        [FieldData(typeof(ushort))]
        public ushort Unk { get; set; }
    }

    public class KAR_GrSkyboxModel : IHSDNode
    {
        [FieldData(typeof(HSD_JOBJ))]
        public HSD_JOBJ JOBJRoot { get; set; }

        [FieldData(typeof(uint))]
        public uint Unknown { get; set; }
    }


    public class KAR_GrUnk2Model : IHSDNode
    {
        [FieldData(typeof(uint))]
        public uint Offset { get; set; }

        [FieldData(typeof(ushort))]
        public ushort Size { get; set; }

        [FieldData(typeof(ushort), Viewable: false)]
        public ushort Padding { get; set; }

        public HSD_Array<KAR_GrUnk2_1Model> GroupsUnk2_1 { get; set; }

        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);

            GroupsUnk2_1 = new HSD_Array<KAR_GrUnk2_1Model>(Size);
            Reader.Seek(Offset);
            GroupsUnk2_1.Open(Reader);
        }

        public override void Save(HSDWriter Writer)
        {
            foreach (KAR_GrUnk2_1Model Unk2_1 in GroupsUnk2_1.Elements)
            {
                foreach (KAR_GrUnk2_1DModel Unk2_1D in Unk2_1.GroupsUnk2_1D.Elements)
                {
                    Unk2_1D.GroupsUnk1_4.Save(Writer);
                    Writer.Align(4);
                }
            }

            foreach (KAR_GrUnk2_1Model Unk2_1 in GroupsUnk2_1.Elements)
            {
                Unk2_1.GroupsUnk2_1D.Save(Writer);
            }

            GroupsUnk2_1.Save(Writer);

            Writer.AddObject(this);

            Writer.WritePointer(GroupsUnk2_1.Size > 0 ? GroupsUnk2_1.Elements[0] : null);
            Writer.Write((ushort)GroupsUnk2_1.Size);
            Writer.Write((ushort)0);
        }
    }

    public class KAR_GrUnk2_1Model : IHSDNode
    {
        [FieldData(typeof(ushort))]
        public ushort ID { get; set; }

        [FieldData(typeof(ushort), Viewable: false)]
        public ushort Padding { get; set; }

        [FieldData(typeof(uint))]
        public uint Offset { get; set; }

        [FieldData(typeof(ushort))]
        public ushort Size { get; set; }

        [FieldData(typeof(ushort), Viewable: false)]
        public ushort Padding2 { get; set; }

        public HSD_Array<KAR_GrUnk2_1DModel> GroupsUnk2_1D { get; set; }

        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);
            uint BaseOffset = Reader.Position();

            GroupsUnk2_1D = new HSD_Array<KAR_GrUnk2_1DModel>(Size);
            Reader.Seek(Offset);
            GroupsUnk2_1D.Open(Reader);

            Reader.Seek(BaseOffset);
        }

        public override void Save(HSDWriter Writer)
        {
            Writer.AddObject(this);

            Writer.Write(ID);
            Writer.Write((ushort)0);
            Writer.WritePointer(GroupsUnk2_1D.Size > 0 ? GroupsUnk2_1D.Elements[0] : null);
            Writer.Write((ushort)GroupsUnk2_1D.Size);
            Writer.Write((ushort)0);
        }
    }

    public class KAR_GrUnk2_1DModel : IHSDNode
    {
        [FieldData(typeof(float))]
        public float Unk { get; set; }

        [FieldData(typeof(uint))]
        public uint Offset { get; set; }

        [FieldData(typeof(ushort))]
        public ushort Size { get; set; }

        [FieldData(typeof(ushort), Viewable: false)]
        public ushort Padding { get; set; }

        public HSD_Array<KAR_GrModel_ModelUnk1_4> GroupsUnk1_4 { get; set; }

        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);
            uint BaseOffset = Reader.Position();

            GroupsUnk1_4 = new HSD_Array<KAR_GrModel_ModelUnk1_4>(Size);
            Reader.Seek(Offset);
            GroupsUnk1_4.Open(Reader);

            Reader.Seek(BaseOffset);
        }

        public override void Save(HSDWriter Writer)
        {
            Writer.AddObject(this);

            Writer.Write(Unk);
            Writer.Write((ushort)0);
            Writer.WritePointer(GroupsUnk1_4.Size > 0 ? GroupsUnk1_4.Elements[0] : null);
            Writer.Write((ushort)GroupsUnk1_4.Size);
            Writer.Write((ushort)0);
        }
    }

    public class KAR_GrUnk3Model : IHSDNode
    {
        [FieldData(typeof(uint))]
        public uint Offset { get; set; }

        [FieldData(typeof(ushort))]
        public ushort Size { get; set; }

        [FieldData(typeof(ushort), Viewable: false)]
        public ushort Padding { get; set; }

        public HSD_Array<KAR_GrUnk3DModel> GroupsUnk3D { get; set; }

        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);

            GroupsUnk3D = new HSD_Array<KAR_GrUnk3DModel>(Size);
            Reader.Seek(Offset);
            GroupsUnk3D.Open(Reader);
        }

        public override void Save(HSDWriter Writer)
        {
            foreach (KAR_GrUnk3DModel Unk3D in GroupsUnk3D.Elements)
            {
                Unk3D.GroupsUnk1_4.Save(Writer);
                Writer.Align(4);
            }

            GroupsUnk3D.Save(Writer);

            Writer.AddObject(this);

            Writer.WritePointer(GroupsUnk3D.Size > 0 ? GroupsUnk3D.Elements[0] : null);
            Writer.Write((ushort)GroupsUnk3D.Size);
            Writer.Write((ushort)0);
        }
    }

    public class KAR_GrUnk3DModel : IHSDNode
    {
        [FieldData(typeof(uint))]
        public uint Offset { get; set; }

        [FieldData(typeof(ushort))]
        public ushort Size { get; set; }

        [FieldData(typeof(ushort), Viewable: false)]
        public ushort Padding { get; set; }

        public HSD_Array<KAR_GrModel_ModelUnk1_4> GroupsUnk1_4 { get; set; }

        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);
            uint BaseOffset = Reader.Position();

            GroupsUnk1_4 = new HSD_Array<KAR_GrModel_ModelUnk1_4>(Size);
            Reader.Seek(Offset);
            GroupsUnk1_4.Open(Reader);

            Reader.Seek(BaseOffset);
        }

        public override void Save(HSDWriter Writer)
        {
            Writer.AddObject(this);

            Writer.WritePointer(GroupsUnk1_4.Size > 0 ? GroupsUnk1_4.Elements[0] : null);
            Writer.Write((ushort)GroupsUnk1_4.Size);
            Writer.Write((ushort)0);
        }
    }

}
