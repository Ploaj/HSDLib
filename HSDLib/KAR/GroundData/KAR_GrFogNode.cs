using System;
using System.Collections.Generic;
namespace HSDLib.KAR
{
    public class KAR_GrFogNode : IHSDNode
    {
        public KAR_GrFog FogInfo { get; set; }
        public KAR_GrFogRegionGroup FogRegionGroup { get; set; }
    }

    public class KAR_GrFog : IHSDNode
    {
        public KAR_GrFogColor FogColor { get; set; }
        public KAR_GrFogUnknown1 FogUnknown { get; set; }
        public int Unknown1 { get; set; }
        public int Unknown2 { get; set; }
        public int Unknown3 { get; set; }
        public int Unknown4 { get; set; }
        public int Unknown5 { get; set; }
        public int Unknown6 { get; set; }
    }

    public class KAR_GrFogColor : IHSDNode
    {
        public int UnknownCount { get; set; }
        public int Unknown { get; set; }
        public float UnknownFloat1 { get; set; }
        public float UnknownFloat2 { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }
    }

    public class KAR_GrFogUnknown1 : IHSDNode
    {
        public KAR_GrFogUnknown2 Unknown1 { get; set; }
        public int Unknown2 { get; set; }
    }

    public class KAR_GrFogUnknown2 : IHSDNode
    {
        public int Unknown { get; set; }
    }

    public class KAR_GrFogRegionGroup : IHSDNode
    {
        public List<KAR_GrFogRegion> FogRegions { get; set; } = new List<KAR_GrFogRegion>();

        public override void Open(HSDReader Reader)
        {
            var off1 = Reader.ReadUInt32();
            var count1 = Reader.ReadInt32();
            var count2 = Reader.ReadInt32();
            var count3 = Reader.ReadInt32();

            for (int i = 0; i < count1; i++)
            {
                Reader.Seek(off1 + (uint)(0x48 * i));
                var region = new KAR_GrFogRegion();
                region.Open(Reader);
                FogRegions.Add(region);
            }

            if (count2 != -1 || count3 != -1)
                throw new NotSupportedException("Fog format not supported");
        }

        public override void Save(HSDWriter Writer)
        {
            Writer.AddObject(FogRegions);
            foreach (var v in FogRegions)
            {
                v.Save(Writer);
            }

            Writer.AddObject(this);
            Writer.WritePointer(FogRegions);
            Writer.Write(FogRegions.Count);
            Writer.Write(-1);
            Writer.Write(-1);
        }
    }

    public class KAR_GrFogRegion : IHSDNode
    {
        public int Unknown1 { get; set; }
        public int Unknown2 { get; set; }
        public float Unknown3 { get; set; }
        public float Unknown4 { get; set; }
        public int Unknown5 { get; set; }
        public int Unknown6 { get; set; }
        public int Unknown7 { get; set; }
        public int Unknown8 { get; set; }
        public int Unknown9 { get; set; }
        public int Unknown10 { get; set; }
        public float Unknown11 { get; set; }
        public float Unknown12 { get; set; }
        public float Unknown13 { get; set; }
        public int Unknown14 { get; set; }
        public int Unknown15 { get; set; }
        public int Unknown16 { get; set; }
        public int Unknown17 { get; set; }
        public int Unknown18 { get; set; }
    }
}