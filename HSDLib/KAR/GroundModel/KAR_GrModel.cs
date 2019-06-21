using HSDLib.Common;
using System.Collections.Generic;
using System.ComponentModel;

namespace HSDLib.KAR
{
    public class KAR_GrModel : IHSDNode
    {
        public KAR_GrMainModel MainModel { get; set; }
        
        public KAR_GrSkyboxModel SkyboxModel { get; set; }
        
        public KAR_GrUnk2Model Unk2Model { get; set; }
        
        public KAR_GrUnk3Model Unk3Model { get; set; }
    }
    
    public class KAR_GrUnk2Model : IHSDNode
    {
        [Browsable(false)]
        public uint Offset { get; set; }

        [Browsable(false)]
        public ushort Size { get; set; }
        
        [Browsable(false)]
        public ushort Padding { get; internal set; }

        public HSD_Array<KAR_GrUnk2_1Model> GroupsUnk2_1;

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
                    Writer.AddObject(Unk2_1D.DOBJIndices);
                    foreach (var v in Unk2_1D.DOBJIndices)
                        Writer.Write(v);
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
        public ushort ID { get; set; }

        [Browsable(false)]
        public ushort Padding { get; set; }

        [Browsable(false)]
        public uint Offset { get; internal set; }

        [Browsable(false)]
        public ushort Size { get; set; }

        [Browsable(false)]
        public ushort Padding2 { get; set; }

        public HSD_Array<KAR_GrUnk2_1DModel> GroupsUnk2_1D;

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
        public float Unk { get; set; }

        [Browsable(false)]
        public uint Offset { get; set; }

        [Browsable(false)]
        public ushort Size { get; set; }

        [Browsable(false)]
        public ushort Padding { get; set; }

        public List<ushort> DOBJIndices = new List<ushort>();

        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);
            uint BaseOffset = Reader.Position();

            DOBJIndices.Clear();
            Reader.Seek(Offset);
            for (int i = 0; i < Size; i++)
                DOBJIndices.Add(Reader.ReadUInt16());

            Reader.Seek(BaseOffset);
        }

        public override void Save(HSDWriter Writer)
        {
            Writer.AddObject(this);

            Writer.Write(Unk);
            Writer.Write((ushort)0);
            Writer.WritePointer(DOBJIndices);
            Writer.Write((ushort)DOBJIndices.Count);
            Writer.Write((ushort)0);
        }
    }

    public class KAR_GrUnk3Model : IHSDNode
    {
        public uint Offset { get; set; }
        
        public ushort Size { get; set; }

        [Browsable(false)]
        public ushort Padding { get; set; }

        public HSD_Array<KAR_GrUnk3DModel> GroupsUnk3D;

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
                Writer.AddObject(Unk3D.DOBJIndices);
                foreach (var v in Unk3D.DOBJIndices)
                    Writer.Write(v);
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
        [Browsable(false)]
        public uint Offset { get; set; }

        [Browsable(false)]
        public ushort Size { get; set; }

        [Browsable(false)]
        public ushort Padding { get; set; }

        public List<ushort> DOBJIndices = new List<ushort>();

        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);
            uint BaseOffset = Reader.Position();
            
            DOBJIndices.Clear();
            Reader.Seek(Offset);
            for (int i = 0; i < Size; i++)
                DOBJIndices.Add(Reader.ReadUInt16());

            Reader.Seek(BaseOffset);
        }

        public override void Save(HSDWriter Writer)
        {
            Writer.AddObject(this);

            Writer.WritePointer(DOBJIndices);
            Writer.Write((ushort)DOBJIndices.Count);
            Writer.Write((ushort)0);
        }
    }

}
