using HSDLib.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDLib.Melee
{
    public class SBM_GrMapPLIT : IHSDNode
    {
        public List<HSD_Light> Lights { get; set; } = new List<HSD_Light>();

        public override void Open(HSDReader Reader)
        {
            var off = Reader.ReadUInt32();
            while(off != 0)
            {
                var temp = Reader.Position();
                Reader.Seek(off);
                HSD_Light light = new HSD_Light();
                light.Open(Reader);
                Lights.Add(light);
                Reader.Seek(temp);
                off = Reader.ReadUInt32();
            }
        }

        public override void Save(HSDWriter Writer)
        {
            foreach(var v in Lights)
            {
                v.Save(Writer);
            }

            Writer.AddObject(this);
            foreach(var v in Lights)
            {
                Writer.WritePointer(v);
            }
            Writer.Write(0);
        }
    }
}
