using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSDLib.GX;

namespace HSDLib.Common
{
    [Flags]
    public enum POBJ_FLAG
    {
        SKIN = (0<<12),
        SHAPEANIM = (1<<12),
        ENVELOPE  = (2<<12),
        CULLFRONT = (1<<14),
        CULLBACK = (1<<15)
    }

    public class HSD_POBJ : IHSDList<HSD_POBJ>
    {
        [FieldData(typeof(POBJ_FLAG))]
        public POBJ_FLAG Flags { get; set; }

        [FieldData(typeof(HSD_AttributeGroup))]
        public HSD_AttributeGroup VertexAttributes { get; set; }
        public byte[] DisplayListBuffer;

        public HSD_JOBJ SingleBind;
        public HSD_Array<HSD_JOBJWeight> BindGroups;

        public override void Open(HSDReader Reader)
        {
            uint NameOffset = Reader.ReadUInt32();
            uint NextOff = Reader.ReadUInt32();
            uint VertexAttributeArray = Reader.ReadUInt32();
            Flags = (POBJ_FLAG)Reader.ReadUInt16();
            int DisplayListSize = Reader.ReadUInt16() * 32;
            uint DisplayListOffset = Reader.ReadUInt32();
            uint WeightListOffset = Reader.ReadUInt32();

            if (NextOff > 0)
                Next = Reader.ReadObject<HSD_POBJ>(NextOff);

            // Display List
            if (DisplayListOffset > 0)
                DisplayListBuffer = Reader.ReadBuffer(DisplayListOffset, DisplayListSize);

            if(VertexAttributeArray > 0)
                VertexAttributes = Reader.ReadObject<HSD_AttributeGroup>(VertexAttributeArray);
            
            //Skinning
            if(WeightListOffset > 0)
            {
                if (Flags.HasFlag(POBJ_FLAG.ENVELOPE))
                {
                    // offsets to array of bone-weight combinations
                    BindGroups = Reader.ReadObject<HSD_Array<HSD_JOBJWeight>>(WeightListOffset);
                }else
                if (Flags.HasFlag(POBJ_FLAG.SHAPEANIM))
                {
                    throw new Exception("Shape Anim not supported");//??
                }else
                if (Flags.HasFlag(POBJ_FLAG.SKIN))
                {
                    // Single bind to given bone
                    SingleBind = Reader.ReadObject<HSD_JOBJ>(WeightListOffset);
                }
            }
        }

        public override void Save(HSDWriter Writer)
        {
            if (Next != null)
                Next.Save(Writer);

            Writer.WriteObject(VertexAttributes);

            Writer.WriteBuffer(DisplayListBuffer);

            if (BindGroups != null)
                Writer.WriteObject(BindGroups);

            Writer.AddObject(this);
            Writer.Write(0);
            if (Next != null)
                Writer.WritePointer(Next);
            else
                Writer.Write(0);
            Writer.WritePointer(VertexAttributes);
            Writer.Write((ushort)Flags);
            Writer.Write((ushort)(DisplayListBuffer.Length / 32));
            Writer.WritePointer(DisplayListBuffer);
            Writer.WritePointer(BindGroups);
        }

    }

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

        public override void Open(HSDReader Reader)
        {
            uint Offset = Reader.ReadUInt32();
            float Weight = Reader.ReadSingle();
            JOBJs = new List<HSD_JOBJ>();
            Weights = new List<float>();
            while(Offset != 0)
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
            for(int i =0; i < JOBJs.Count; i++)
            {
                Writer.WritePointer(JOBJs[i]);
                Writer.Write(Weights[i]);
            }
            Writer.Write(0);
            Writer.Write(0);
        }
    }
}
