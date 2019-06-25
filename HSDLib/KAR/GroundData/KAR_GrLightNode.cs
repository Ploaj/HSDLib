using System;
using HSDLib.Common;
using System.Collections.Generic;

namespace HSDLib.KAR
{
    public class KAR_GrLightGroup : IHSDNode
    {
        public KAR_GrLightNode GlobalLightGroup { get; set; }

        public KAR_GrLightNode LightGroup1 { get; set; } // player light?

        public KAR_GrLightNode LightGroup2 { get; set; }
    }

    public class KAR_GrLightNode : IHSDNode
    {
        // null terminated pointer list
        public List<HSD_Light> Lights { get; set; } = new List<HSD_Light>();

        public override void Open(HSDReader Reader)
        {
            var off = Reader.ReadUInt32();
            while(off != 0)
            {
                var temp = Reader.Position();
                Reader.Seek(off);
                HSD_Light l = new HSD_Light();
                l.Open(Reader);
                Lights.Add(l);
                Reader.Seek(temp);
                off = Reader.ReadUInt32();
            }
        }

        public override void Save(HSDWriter Writer)
        {
            foreach (var v in Lights)
                v.Save(Writer);

            Writer.AddObject(this);
            foreach (var v in Lights)
                Writer.WritePointer(v);
            Writer.Write(0);
        }
    }

}
