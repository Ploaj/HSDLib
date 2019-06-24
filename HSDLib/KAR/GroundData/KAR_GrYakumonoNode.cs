using System.Collections.Generic;

namespace HSDLib.KAR
{
    public class KAR_GrYakumonoNode : IHSDNode
    {
        public List<KAR_GrYakumonoData> YakuData { get; set; } = new List<KAR_GrYakumonoData>();
        public List<KAR_GrYakumonoData> YakuStaticData { get; set; } = new List<KAR_GrYakumonoData>();
        public List<KAR_GrYakumonoCommonData> YakuCommonData { get; set; } = new List<KAR_GrYakumonoCommonData>();

        public override void Open(HSDReader Reader)
        {
            var dataoff = Reader.ReadUInt32();
            var datacount = Reader.ReadInt32();
            var datastaticoff = Reader.ReadUInt32();
            var datastaticcount = Reader.ReadInt32();
            var datacommonoff = Reader.ReadUInt32();
            var datacommoncount = Reader.ReadInt32();

            Reader.Seek(dataoff);
            for(int i = 0; i <datacount; i++)
            {
                var off = Reader.ReadUInt32();
                var temp = Reader.Position();
                Reader.Seek(off);
                var v = new KAR_GrYakumonoData();
                v.Open(Reader);
                YakuData.Add(v);
                Reader.Seek(temp);
            }

            Reader.Seek(datastaticoff);
            for (int i = 0; i < datastaticcount; i++)
            {
                var off = Reader.ReadUInt32();
                var temp = Reader.Position();
                Reader.Seek(off);
                var v = new KAR_GrYakumonoData();
                v.Open(Reader);
                YakuStaticData.Add(v);
                Reader.Seek(temp);
            }

            for (int i = 0; i < datacommoncount; i++)
            {
                Reader.Seek(datacommonoff + (uint)(i * 12));
                var com = new KAR_GrYakumonoCommonData();
                com.Open(Reader);
                YakuCommonData.Add(com);
            }
        }

        public override void Save(HSDWriter Writer)
        {
            foreach(var v in YakuData)
                v.Save(Writer);
            foreach (var v in YakuStaticData)
                v.Save(Writer);

            Writer.AddObject(YakuData);
            foreach (var v in YakuData)
                Writer.WritePointer(v);

            Writer.AddObject(YakuStaticData);
            foreach (var v in YakuStaticData)
                Writer.WritePointer(v);

            Writer.AddObject(YakuCommonData);
            foreach (var v in YakuCommonData)
                v.Save(Writer);


            Writer.AddObject(this);
            if (YakuData.Count == 0)
                Writer.Write(0);
            else
                Writer.WritePointer(YakuData);
            Writer.Write(YakuData.Count);
            
            if (YakuData.Count == 0)
                Writer.Write(0);
            else
                Writer.WritePointer(YakuStaticData);
            Writer.Write(YakuStaticData.Count);
            
            if (YakuData.Count == 0)
                Writer.Write(0);
            else
                Writer.WritePointer(YakuCommonData);
            Writer.Write(YakuCommonData.Count);
        }
    }
    
    //TODO: this section is sooo inconsistant
    public class KAR_GrYakumonoData : IHSDNode
    {

    }

    public enum YakuCommonType
    {
        BoostPadBigFast = 0,
        BoostPadSmallFast = 1,
        BoostPadBigSlow = 2,
        BoostPadSmallSlow = 3,
        OneWayBoostPadBigFast = 4,
        OneWayBoostPadSmallFast = 5,
        OneWayBoostPadBigSlow = 6,
        OneWayBoostPadSmallSlow = 7,
        SpinnerPadBigCounterClockwise = 8,
        SpinnerPadSmallCounterClockwise = 9,
        SpinnerPadBigClockwise = 10,
        SpinnerPadSmallClockwise = 11,
        AbilitySlowPadBig = 12,
        AbilitySlowPadSmall = 13,
        JumperPadBig = 14,
        JumperPadSmall = 15
    }

    public class KAR_GrYakumonoCommonData : IHSDNode
    {
        public YakuCommonType Type { get; set; }
        public int BoneID { get; set; }
        public int PadID { get; set; }
    }
}
