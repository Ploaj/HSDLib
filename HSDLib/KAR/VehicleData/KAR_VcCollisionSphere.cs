using System;

namespace HSDLib.KAR
{
    public class KAR_VcCollisionSphere : IHSDNode
    {
        public uint Unk1 { get; set; }
        
        public uint Padding { get; set; }
        
        public float Size { get; set; }
        
        public float X { get; set; }
        
        public float Y { get; set; }
        
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
