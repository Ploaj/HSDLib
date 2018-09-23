using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.DAT;
using MeleeLib.DAT.Animation;
using MeleeLib.DAT.MatAnim;
using MeleeLib.IO;

namespace MeleeLib.KAR
{
    public class KAR_HSD_AnimationGroup
    {
        public float Value;
        public DatAnimation Animation;
        public List<DatMatAnim> MatAnim = new List<DatMatAnim>();
    }

    public class KAR_HSD_VehicleAnimationBank : DatNode
    {
        public KAR_HSD_AnimationGroup[] Animations;
        public HSD_ParamArray Params = new HSD_ParamArray();

        public void Deserialize(DATReader r, DATRoot Root, int Size)
        {
            Console.WriteLine("AnimList " + r.Pos().ToString("X"));

            Animations = new KAR_HSD_AnimationGroup[Size];
            for(int i = 0; i < Size; i++)
            {
                int AnimOffset = r.Int();
                int MatAnimOffset = r.Int();
                int temp = r.Pos();
                Animations[i] = new KAR_HSD_AnimationGroup();
                if(AnimOffset > 0)
                {
                    r.Seek(AnimOffset);
                    Animations[i].Animation = new DatAnimation();
                    Animations[i].Animation.Deserialize(r, Root);
                }
                if(MatAnimOffset > 0)
                {
                    r.Seek(MatAnimOffset);
                    Animations[i].MatAnim = new List<DatMatAnim>();
                    DatMatAnim matAnim = new DatMatAnim();
                    matAnim.Deserialize(r, Root, Animations[i].MatAnim);
                }
                r.Seek(temp);
            }

            Params.Deserialize(r, Root, Size == 6 ? 0x2C : 0x18);

            for (int i = 0; i < Size; i++)
            {
                Animations[i].Value = r.Float();
            }

            if (Size == 6)
            {
                if (r.Int() != -1) throw new Exception("Error reading animation bank");
                if (r.Int() != -1) throw new Exception("Error reading animation bank");
            }
        }

        public override void Serialize(DATWriter Node)
        {
            Node.AddObject(this);
            foreach(KAR_HSD_AnimationGroup g in Animations)
            {
                if (g.Animation != null)
                    Node.Object(g.Animation);
                else
                    Node.Int(0);
                if (g.MatAnim.Count > 0)
                    Node.Object(g.MatAnim[0]);
                else
                    Node.Int(0);
            }
            Params.Serialize(Node);

            foreach (KAR_HSD_AnimationGroup g in Animations)
            {
                Node.Float(g.Value);
            }

            Node.Int(-1);
            Node.Int(-1);

            // Serialize Data

            foreach (KAR_HSD_AnimationGroup g in Animations)
            {
                if(g.Animation != null)
                    g.Animation.Serialize(Node);
                if (g.MatAnim.Count > 0)
                    foreach(DatMatAnim a in g.MatAnim)
                        a.Serialize(Node, g.MatAnim);
            }
        }
    }
}
