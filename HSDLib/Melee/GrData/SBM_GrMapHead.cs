using HSDLib.Common;
using System;
using System.Collections.Generic;

namespace HSDLib.Melee
{
    public class SBM_GrMapHead : IHSDNode
    {
        public List<SBM_GrMapModelGroup> ModelGroups { get; set; } = new List<SBM_GrMapModelGroup>();

        //

        //

        //

        public List<SBM_GrMapLight> Lights { get; set; } = new List<SBM_GrMapLight>();

        //

        public override void Open(HSDReader Reader)
        {
            var modelGroupOffset = Reader.ReadUInt32();
            var modelGroupCount = Reader.ReadInt32();
            var unk1GroupOffset = Reader.ReadUInt32();
            var unk1GroupCount = Reader.ReadInt32();
            var unk2GroupOffset = Reader.ReadUInt32();
            var unk2GroupCount = Reader.ReadInt32();
            var lightGroupOffset = Reader.ReadUInt32();
            var lightGroupCount = Reader.ReadInt32();
            var unk4GroupOffset = Reader.ReadUInt32();
            var unk4GroupCount = Reader.ReadInt32();
            var unk5GroupOffset = Reader.ReadUInt32();
            var unk5GroupCount = Reader.ReadInt32();

            for(int i = 0; i < modelGroupCount; i++)
            {
                Reader.Seek(modelGroupOffset + (uint)(12 * i));
                SBM_GrMapModelGroup mg = new SBM_GrMapModelGroup();
                mg.Open(Reader);
                ModelGroups.Add(mg);
            }

            for (int i = 0; i < modelGroupCount; i++)
            {
                Reader.Seek(lightGroupOffset + (uint)(i * 8));
                SBM_GrMapLight mg = new SBM_GrMapLight();
                mg.Open(Reader);
                Lights.Add(mg);
            }
        }

        public override void Save(HSDWriter Writer)
        {
        }
    }

    public class SBM_GrMapLight : IHSDNode
    {
        public HSD_LOBJ LightObject { get; set; }

        public uint Flag { get; set; }
    }

    public class SBM_GrMapModelGroup : IHSDNode
    {
        public HSD_JOBJ RootNode { get; set; }

        public List<SBM_GrMapModelTable> Tables { get; set; } = new List<SBM_GrMapModelTable>();

        public override void Open(HSDReader Reader)
        {
            var rootNodeOffset = Reader.ReadUInt32(); 
            var offset = Reader.ReadUInt32();
            var count = Reader.ReadInt32();

            RootNode = Reader.ReadObject<HSD_JOBJ>(rootNodeOffset);

            Reader.Seek(offset);
            for(int i = 0; i < count; i++)
            {
                SBM_GrMapModelTable table = new SBM_GrMapModelTable();
                table.Open(Reader);
                Tables.Add(table);
            }
        }

        public override void Save(HSDWriter Writer)
        {
            Writer.AddObject(Tables);
            foreach (var v in Tables)
                v.Save(Writer);

            Writer.WriteObject(RootNode);

            Writer.AddObject(this);
            Writer.WritePointer(RootNode);
            if (Tables.Count > 0)
                Writer.WritePointer(Tables);
            else
                Writer.Write(0);
            Writer.Write(Tables.Count);
        }
    }

    public class SBM_GrMapModelTable : IHSDNode
    {
        public ushort Value1 { get; set; }
        public ushort Value2 { get; set; }
    }
}
