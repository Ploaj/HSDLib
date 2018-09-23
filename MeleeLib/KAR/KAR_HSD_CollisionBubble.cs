using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.DAT;
using MeleeLib.IO;

namespace MeleeLib.KAR
{
    public class KAR_HSD_CollisionBubble : DatNode
    {
        public int BoneID;
        public float Size;
        public float X, Y, Z;
        public void Deserialize(DATReader r, DATRoot Root)
        {
            BoneID = r.Int();
            if (r.Int() != 0) throw new Exception("Error Reading Collision  Bubble");
            Size = r.Float();
            X = r.Float();
            Y = r.Float();
            Z = r.Float();
        }

        public override void Serialize(DATWriter Node)
        {
            Node.AddObject(this);
            Node.Int(BoneID);
            Node.Int(0);
            Node.Float(Size);
            Node.Float(X);
            Node.Float(Y);
            Node.Float(Z);
        }
    }
}
