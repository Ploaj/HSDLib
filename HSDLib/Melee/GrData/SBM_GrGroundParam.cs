using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace HSDLib.Melee
{
    public class SBM_GrGroundParam : IHSDNode
    {
        public float StageScale { get; set; }

        public float Unknown1 { get; set; }

        public float Unknown2 { get; set; }

        public int Unknown3 { get; set; }

        public int Unknown4 { get; set; }

        public int TiltScale { get; set; }

        public float HorizontalRotation { get; set; }

        public float VerticalRotation { get; set; }

        public float Fixedness { get; set; }

        public float BubbleMultiplier { get; set; }

        public float CameraSpeedSmoothness { get; set; }

        public int Unknown5 { get; set; }

        public int PauseMinZ { get; set; }

        public int PauseInitialZ { get; set; }

        public int PauseMaxZ { get; set; }

        public int Unknown6 { get; set; }

        public float PauseMaxAngleUp { get; set; }

        public float PauseMaxAngleLeft { get; set; }

        public float PauseMaxAngleRight { get; set; }

        public float PauseMaxAngleDown { get; set; }

        public float Unknown7 { get; set; }

        public float Unknown8 { get; set; }

        public float Unknown9 { get; set; }

        public float Unknown10 { get; set; }

        public float Unknown11 { get; set; }

        public float Unknown12 { get; set; }

        // 36 shorts

        public short UnknownShort0 { get; set; }

        public short UnknownShort1 { get; set; }

        public short UnknownShort2 { get; set; }

        public short UnknownShort3 { get; set; }

        public short UnknownShort4 { get; set; }

        public short UnknownShort5 { get; set; }

        public short UnknownShort6 { get; set; }

        public short UnknownShort7 { get; set; }

        public short UnknownShort8 { get; set; }

        public short UnknownShort9 { get; set; }

        public short UnknownShort10 { get; set; }

        public short UnknownShort11 { get; set; }

        public short UnknownShort12 { get; set; }

        public short UnknownShort13 { get; set; }

        public short UnknownShort14 { get; set; }

        public short UnknownShort15 { get; set; }

        public short UnknownShort16 { get; set; }

        public short UnknownShort17 { get; set; }

        public short UnknownShort18 { get; set; }

        public short UnknownShort19 { get; set; }

        public short UnknownShort20 { get; set; }

        public short UnknownShort21 { get; set; }

        public short UnknownShort22 { get; set; }

        public short UnknownShort23 { get; set; }

        public short UnknownShort24 { get; set; }

        public short UnknownShort25 { get; set; }

        public short UnknownShort26 { get; set; }

        public short UnknownShort27 { get; set; }

        public short UnknownShort28 { get; set; }

        public short UnknownShort29 { get; set; }

        public short UnknownShort30 { get; set; }

        public short UnknownShort31 { get; set; }

        public short UnknownShort32 { get; set; }

        public short UnknownShort33 { get; set; }

        public short UnknownShort34 { get; set; }

        public short UnknownShort35 { get; set; }

        [Browsable(false)]
        public uint TableOffset { get; set; }

        [Browsable(false)]
        public int TableCount { get; set; }

        public uint BubbleColorTopLeft { get; set; }

        public uint BubbleColorTopMiddle { get; set; }

        public uint BubbleColorTopRight { get; set; }

        public uint BubbleColorSidesTop { get; set; }

        public uint BubbleColorSidesMiddle { get; set; }

        public uint BubbleColorSidesBottom { get; set; }

        public uint BubbleColorBottomLeft { get; set; }

        public uint BubbleColorBottomMiddle { get; set; }

        public uint BubbleColorBottomRight { get; set; }

        public List<SBM_GrGroundParamTable> Tables = new List<SBM_GrGroundParamTable>();

        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);

            Reader.Seek(TableOffset);
            for(int i = 0; i < TableCount; i++)
            {
                var table = new SBM_GrGroundParamTable();
                table.Open(Reader);
                Tables.Add(table);
            }
        }

        public override void Save(HSDWriter Writer)
        {
            Writer.AddObject(Tables);
            foreach(var v in Tables)
            {
                v.Save(Writer);
            }
            TableCount = Tables.Count;

            base.Save(Writer);
            if(Writer.Mode == WriterWriteMode.NORMAL)
                Writer.WritePointerAt((int)(Writer.BaseStream.Position - 0x2C), Tables.Count == 0 ? null : Tables[0]);
        }
    }

    public class SBM_GrGroundParamTable : IHSDNode
    {
        public int ID { get; set; }

        public int UnknownCount1 { get; set; }

        public int UnknownCount2 { get; set; }

        public int UnknownCount3 { get; set; }

        public int UnknownCount4 { get; set; }
        
        public short UnknownShort0 { get; set; }

        public short UnknownShort1 { get; set; }

        public short UnknownShort2 { get; set; }

        public short UnknownShort3 { get; set; }

        public short UnknownShort4 { get; set; }

        public short UnknownShort5 { get; set; }

        public short UnknownShort6 { get; set; }

        public short UnknownShort7 { get; set; }

        public short UnknownShort8 { get; set; }

        public short UnknownShort9 { get; set; }

        public short UnknownShort10 { get; set; }

        public short UnknownShort11 { get; set; }

        public short UnknownShort12 { get; set; }

        public short UnknownShort13 { get; set; }

        public short UnknownShort14 { get; set; }

        public short UnknownShort15 { get; set; }

        public short UnknownShort16 { get; set; }

        public short UnknownShort17 { get; set; }

        public short UnknownShort18 { get; set; }

        public short UnknownShort19 { get; set; }

        public short UnknownShort20 { get; set; }

        public short UnknownShort21 { get; set; }

        public short UnknownShort22 { get; set; }

        public short UnknownShort23 { get; set; }

        public short UnknownShort24 { get; set; }

        public short UnknownShort25 { get; set; }

        public short UnknownShort26 { get; set; }

        public short UnknownShort27 { get; set; }

        public short UnknownShort28 { get; set; }

        public short UnknownShort29 { get; set; }

        public short UnknownShort30 { get; set; }

        public short UnknownShort31 { get; set; }

        public short UnknownShort32 { get; set; }

        public short UnknownShort33 { get; set; }

        public short UnknownShort34 { get; set; }

        public short UnknownShort35 { get; set; }

        public short UnknownShort36 { get; set; }

        public short UnknownShort37 { get; set; }

        public short UnknownShort38 { get; set; }

        public short UnknownShort39 { get; set; }
    }

}
