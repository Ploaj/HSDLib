using HSDLib.Common;
using System;
using System.Collections.Generic;

namespace HSDLib.KAR
{
    public class KAR_GrPositionNode : IHSDNode
    {
        public HSD_JOBJ PositionNodeRoot { get; set; }

        public KAR_GrPositionList StartPositions { get; set; }

        public KAR_GrPositionList EnemyPositions { get; set; }

        public KAR_GrPositionList Unknown1Positions { get; set; }

        public KAR_GrPositionList Unknown2Positions { get; set; }

        public KAR_GrPositionList Unknown3Positions { get; set; }

        public KAR_GrPositionList ItemPositions { get; set; }

        public KAR_GrPositionList CityEventPositions { get; set; }

        public KAR_GrPositionList Unknown4Positions { get; set; }

        public KAR_GrPositionList RespawnPositions { get; set; }

        public KAR_GrPositionList RespawnPositions2 { get; set; }

        public KAR_GrPositionList CommonPositions { get; set; }

        public KAR_GrPositionList ItemAreaPositions { get; set; }

        public KAR_GrPositionList VehicleAreaPositions { get; set; }
    }

    public class KAR_GrPositionList : IHSDNode
    {
        public List<int> BoneIDs = new List<int>();

        public List<HSD_Matrix3x3> Matrices = new List<HSD_Matrix3x3>();

        public override void Open(HSDReader Reader)
        {
            var boneTableOffset = Reader.ReadUInt32();
            var matrixTableOffset = Reader.ReadUInt32();
            var count = Reader.ReadUInt32();

            if(boneTableOffset != 0)
            {
                Reader.Seek(boneTableOffset);
                for (int i = 0; i < count; i++)
                {
                    BoneIDs.Add(Reader.ReadInt32());
                }
            }
            if (matrixTableOffset != 0)
            {
                Reader.Seek(matrixTableOffset);
                for (int i = 0; i < count; i++)
                {
                    HSD_Matrix3x3 mat = new HSD_Matrix3x3();
                    mat.Open(Reader);
                    Matrices.Add(mat);
                }
            }
        }

        public override void Save(HSDWriter Writer)
        {
            if(BoneIDs.Count > 0)
            {
                Writer.AddObject(BoneIDs);
                foreach (var b in BoneIDs)
                    Writer.Write(b);
            }
            if (Matrices.Count > 0)
            {
                Writer.AddObject(Matrices);
                foreach (var m in Matrices)
                    m.Save(Writer);
            }

            Writer.AddObject(this);
            if (BoneIDs.Count > 0)
                Writer.WritePointer(BoneIDs);
            else
                Writer.Write(0);
            if (Matrices.Count > 0)
                Writer.WritePointer(Matrices);
            else
                Writer.Write(0);

            Writer.Write(Math.Max(Matrices.Count, BoneIDs.Count));
        }
    }
}
