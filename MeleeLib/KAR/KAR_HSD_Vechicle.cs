using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.DAT;
using MeleeLib.IO;

namespace MeleeLib.KAR
{
    public class KAR_HSD_Vehicle : DatNode
    {
        public HSD_ParamArray               VehicleParams = new HSD_ParamArray();
        public KAR_HSD_ModelGroup           Models = new KAR_HSD_ModelGroup();
        public KAR_HSD_VehicleUnknown       Unknown = new KAR_HSD_VehicleUnknown();
        public HSD_ParamArray               CollisionParams = new HSD_ParamArray();
        public KAR_HSD_CollisionBubble      CollisionBubble = new KAR_HSD_CollisionBubble();
        public HSD_ParamArray               HandlingParams = new HSD_ParamArray();
        public KAR_HSD_VehicleAnimationBank AnimationBank = new KAR_HSD_VehicleAnimationBank();

        public int AnimGroupSize = 6;
        
        public void Deserialize(DATReader r, DATRoot Root)
        {
            Root.ExtraNodes.Add(this);

            int VehicleInfoOff = r.Int();
            int ModelInfoOff = r.Int();
            int unk1 = r.Int();
            int CollisionInfoOff = r.Int();
            int GreySphereAttrOff = r.Int();
            int HandlingInfoOff = r.Int();
            int AnimationInfoOff = r.Int();

            if(VehicleInfoOff > 0)
            {
                r.Seek(VehicleInfoOff);
                VehicleParams.Deserialize(r, Root, 0x1F0);
            }
            if(ModelInfoOff > 0)
            {
                r.Seek(ModelInfoOff);
                Models.Deserialize(r, Root);
            }
            if (unk1 > 0)
            {
                r.Seek(unk1);
                Unknown.Deserialize(r, Root);
            }
            if(CollisionInfoOff > 0)
            {
                // TODO:
                r.Seek(CollisionInfoOff);
                CollisionParams.Deserialize(r, Root, 0x38);
            }
            if (GreySphereAttrOff > 0)
            {
                r.Seek(GreySphereAttrOff);
                CollisionBubble.Deserialize(r, Root);
            }
            if (HandlingInfoOff> 0)
            {
                // TODO:
                r.Seek(HandlingInfoOff);
                HandlingParams.Deserialize(r, Root, 0xF8);
            }
            if (AnimationInfoOff > 0)
            {
                r.Seek(AnimationInfoOff);
                AnimationBank.Deserialize(r, Root, AnimGroupSize);
            }

        }

        public override void Serialize(DATWriter Node)
        {
            if (VehicleParams != null)
                VehicleParams.Serialize(Node);
            if (Models != null)
                Models.Serialize(Node);
            if (Unknown != null)
                Unknown.Serialize(Node);
            if (CollisionParams != null)
                CollisionParams.Serialize(Node);
            if(CollisionBubble != null)
                CollisionBubble.Serialize(Node);
            if (HandlingParams != null)
                HandlingParams.Serialize(Node);
            if (AnimationBank != null)
                AnimationBank.Serialize(Node);

            Node.AddObject(this);
            if (VehicleParams != null)
                Node.Object(VehicleParams);
            else Node.Int(0);
            if (Models != null)
                Node.Object(Models);
            else Node.Int(0);
            if (Unknown != null)
                Node.Object(Unknown);
            else Node.Int(0);
            if (CollisionParams != null)
                Node.Object(CollisionParams);
            else Node.Int(0);
            if (CollisionBubble != null)
                Node.Object(CollisionBubble);
            else Node.Int(0);
            if (HandlingParams != null)
                Node.Object(HandlingParams);
            else Node.Int(0);
            if (AnimationBank != null)
                Node.Object(AnimationBank);
            else Node.Int(0);
        }

        public override List<DATRoot> GetRoots()
        {
            List<DATRoot> Roots = new List<DATRoot>();
            Roots.Add(Models.ModelRoot);
            Roots.Add(Models.ShadowModelRoot);
            return Roots;
        }
    }
}
