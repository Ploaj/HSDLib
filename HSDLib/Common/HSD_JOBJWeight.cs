using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDLib.Common
{
    public class HSD_JOBJWeight : IHSDNode
    {
        public List<HSD_JOBJ> JOBJs = new List<HSD_JOBJ>();
        public List<float> Weights = new List<float>();

        public override bool Equals(object obj)
        {
            if (!(obj is HSD_JOBJWeight))
                return false;

            return JOBJs.SequenceEqual(((HSD_JOBJWeight)obj).JOBJs) && Weights.SequenceEqual(((HSD_JOBJWeight)obj).Weights);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void Open(HSDReader Reader)
        {
            uint Offset = Reader.ReadUInt32();
            float Weight = Reader.ReadSingle();
            JOBJs = new List<HSD_JOBJ>();
            Weights = new List<float>();
            while (Offset != 0)
            {
                JOBJs.Add(Reader.ReadObject<HSD_JOBJ>(Offset));
                Weights.Add(Weight);
                Offset = Reader.ReadUInt32();
                Weight = Reader.ReadSingle();
            }
        }

        public override void Save(HSDWriter Writer)
        {
            Writer.AddObject(this);
            for (int i = 0; i < JOBJs.Count; i++)
            {
                Writer.WritePointer(JOBJs[i]);
                Writer.Write(Weights[i]);
            }
            Writer.Write(0);
            Writer.Write(0);
        }
    }
}
