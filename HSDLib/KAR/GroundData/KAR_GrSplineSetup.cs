using HSDLib.Common;
using System;
using System.Collections.Generic;

namespace HSDLib.KAR
{
    public class KAR_GrSplineSetup : IHSDNode
    {
        public List<KAR_GrSpline> Splines { get; set; } = new List<KAR_GrSpline>();

        public override void Open(HSDReader Reader)
        {
            var offset = Reader.ReadUInt32();
            var count = Reader.ReadInt32();
            
            for (int i = 0; i < count; i++)
            {
                Reader.Seek(offset + (uint)(i * 0x24));
                var spline = new KAR_GrSpline();
                spline.Open(Reader);
                Splines.Add(spline);
            }
        }

        public override void Save(HSDWriter Writer)
        {
            Writer.AddObject(Splines);
            foreach (var p in Splines)
            {
                p.Save(Writer);
            }

            Writer.AddObject(this);
            Writer.WritePointer(Splines);
            Writer.Write(Splines.Count == 0 ? 1 : Splines.Count);
            Writer.Write(0);
        }
    }


    public class KAR_GrSpline : IHSDNode
    {
        public HSD_Spline Unknown1 { get; set; }
        public HSD_Spline Unknown2 { get; set; }
        public int Unknown3 { get; set; } = -1;
        public int Unknown4 { get; set; } = -1;
        public int Unknown5 { get; set; } = -1;
        public int Unknown6 { get; set; } = 0;
    }

    public class KAR_GrFlowSetup : IHSDNode
    {
        public KAR_GrCourseSpline Spline { get; set; }
        public int Unknown { get; set; }
    }

    public class KAR_GrCourseSplineSetup : IHSDNode
    {
        public KAR_GrCourseSpline CourseSplineList { get; set; }
        public KAR_GrCourseSplineTable UnknownList { get; set; }
        public KAR_GrCourseSplineTable UnknownList2 { get; set; }
        public KAR_GrCourseSplineTable UnknownList3 { get; set; }
        public int Loop { get; set; } = 1;
        public int UnknownPointer { get; set; }
        public int Unknown3 { get; set; }
        public KAR_GrCourseUnknownFloats UnknownFloats { get; set; }
        public int UnknownRuntime1 { get; set; } // runtime variable
        public int UnknownRuntime2 { get; set; } // runtime variable
        public int Unknown7 { get; set; }
        public int UnknownRuntime3 { get; set; } // runtime variable
        public int UnknownRuntime4 { get; set; } // runtime variable
        public int UnknownRuntime5 { get; set; } // runtime variable
    }

    public class KAR_GrCourseUnknownFloats : IHSDNode
    {
        public float V1 { get; set; }
        public float V2 { get; set; }
        public float V3 { get; set; }
        public float V4 { get; set; }
    }
}
