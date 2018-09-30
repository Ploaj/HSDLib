using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDLib.KAR
{
    public class CollisionSphere : IHSDNode
    {
        [FieldData(typeof(uint))]
        public uint Unk1 { get; set; }

        [FieldData(typeof(uint))]
        public uint Padding { get; set; }

        [FieldData(typeof(float))]
        public float Size { get; set; }
        
        [FieldData(typeof(float))]
        public float X { get; set; }
        
        [FieldData(typeof(float))]
        public float Y { get; set; }

        [FieldData(typeof(float))]
        public float Z { get; set; }

        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);

            if(Unk1 != 0)
            {
                Console.WriteLine("Collision bubble unknown " + Unk1.ToString("X"));
            }
        }
    }
}
