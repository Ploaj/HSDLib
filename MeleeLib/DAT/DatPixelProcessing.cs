using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.GCX;
using MeleeLib.IO;

namespace MeleeLib.DAT
{
    public class DatPixelProcessing : DatNode
    {
        public byte Flags;
        public byte AlphaRef0;
        public byte AlphaRef1;
        public byte DestinationAlpha;
        public GXBlendMode BlendMode;
        public GXBlendFactor SrcFactor;
        public GXBlendFactor DstFactor;
        public GXLogicOp BlendOp;
        public GXCompareType DepthFunction;
        public GXCompareType AlphaComp0;
        public GXAlphaOp AlphaOp;
        public GXCompareType AlphaComp1;

        public void Deserialize(DATReader r, DATRoot Root)
        {
            Flags = r.Byte();
            AlphaRef0 = r.Byte();
            AlphaRef1 = r.Byte();
            DestinationAlpha = r.Byte();
            BlendMode = (GXBlendMode)r.Byte();
            SrcFactor = (GXBlendFactor)r.Byte();
            DstFactor = (GXBlendFactor)r.Byte();
            BlendOp = (GXLogicOp)r.Byte();
            DepthFunction = (GXCompareType)r.Byte();
            AlphaComp0 = (GXCompareType)r.Byte();
            AlphaOp = (GXAlphaOp)r.Byte();
            AlphaComp1 = (GXCompareType)r.Byte();
        }

        public override void Serialize(DATWriter Node)
        {
            Node.AddObject(this);
            Node.Byte(Flags);
            Node.Byte(AlphaRef0);
            Node.Byte(AlphaRef1);
            Node.Byte(DestinationAlpha);
            Node.Byte((byte)BlendMode);
            Node.Byte((byte)SrcFactor);
            Node.Byte((byte)DstFactor);
            Node.Byte((byte)BlendOp);
            Node.Byte((byte)DepthFunction);
            Node.Byte((byte)AlphaComp0);
            Node.Byte((byte)AlphaOp);
            Node.Byte((byte)AlphaComp1);
        }
    }
}
