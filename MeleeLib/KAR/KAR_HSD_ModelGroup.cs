using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.DAT;
using MeleeLib.GCX;
using MeleeLib.IO;

namespace MeleeLib.KAR
{
    public class KAR_HSD_ModelData : DatNode
    {
        public int UnknownValue;
        public byte[] Data;

        public void Deserialize(DATReader r, DATRoot Root)
        {
            int unk = r.Int(); // has data?
            int ToDataSection = r.Int();

            //Console.WriteLine(unk);

            if(ToDataSection > 0)
            {
                r.Seek(ToDataSection);
                int Size = r.Int();
                int Offset = r.Int();
                Data = r.getSection(Offset, Size);
                //Console.WriteLine("\t" + Data.Length);
            }
        }

        public override void Serialize(DATWriter Node)
        {
            object temp = new object();
            if (Data != null)
            {
                Node.AddObject(Data);
                Node.Bytes(Data);
                Node.Align(4);
                
                Node.AddObject(temp);
                Node.Int(Data.Length);
                Node.Object(Data);
            }


            Node.AddObject(this);
            Node.Int(1);
            if (Data != null)
                Node.Object(temp);
            else
                Node.Int(0);
        }
    }

    public class KAR_HSD_ModelGroup : DatNode
    {
        public DATRoot ModelRoot = new DATRoot();
        public uint Flags;
        public int Unknown;
        public KAR_HSD_ModelData[] ModelData = new KAR_HSD_ModelData[6];
        public DATRoot ShadowModelRoot = new DATRoot();

        public void Deserialize(DATReader r, DATRoot Root)
        {
            int ModelOff = r.Int();
            if (r.Int() != 0) throw new Exception("This isn't padding");
            Flags = (uint)r.Int();
            Unknown = r.Int();
            for(int i = 0; i < 6; i++)
            {
                int temp = r.Pos()+4;
                r.Seek(r.Int());
                ModelData[i] = new KAR_HSD_ModelData();
                ModelData[i].Deserialize(r, Root);
                r.Seek(temp);
            }
            int ShadowModelOff = r.Int();

            if (ModelOff > 0)
            {
                r.Seek(ModelOff);
                DatJOBJ jobj = new DatJOBJ();
                jobj.Deserialize(r, ModelRoot);
                ModelRoot.Bones.Add(jobj);
            }
            if (ShadowModelOff > 0)
            {
                r.Seek(ShadowModelOff);
                DatJOBJ jobj = new DatJOBJ();
                jobj.Deserialize(r, ShadowModelRoot);
                ShadowModelRoot.Bones.Add(jobj);
            }
        }

        public override void Serialize(DATWriter Node)
        {
            foreach (KAR_HSD_ModelData d in ModelData)
            {
                d.Serialize(Node);
            }

            // Serializeing Buffers
            foreach (GXAttribGroup g in ModelRoot.Attributes)
            {
                foreach (GXAttr a in g.Attributes)
                    a.SerializeData(Node);
                g.Serialize(Node);
            }
            foreach (GXAttribGroup g in ShadowModelRoot.Attributes)
            {
                foreach (GXAttr a in g.Attributes)
                    a.SerializeData(Node);
                g.Serialize(Node);
            }

            foreach(byte[] b in ModelRoot.GetImageData())
            {
                if (!Node.ContainsObject(b))
                {
                    Node.AddObject(b);
                    Node.Align(0x20);
                    Node.Bytes(b);
                    Node.Align(0x20);
                }
            }
            foreach (byte[] b in ShadowModelRoot.GetImageData())
            {
                if (!Node.ContainsObject(b))
                {
                    Node.AddObject(b);
                    Node.Align(0x20);
                    Node.Bytes(b);
                    Node.Align(0x20);
                }
            }

            // Serializing Models....
            foreach (DatJOBJ j in ModelRoot.Bones)
                j.Serialize(Node);
            foreach (DatJOBJ j in ShadowModelRoot.Bones)
                j.Serialize(Node);

            Node.AddObject(this);
            Node.Object(ModelRoot.Bones[0]);
            Node.Int(0);
            Node.Int((int)Flags);
            Node.Int(Unknown);
            foreach(KAR_HSD_ModelData d in ModelData)
            {
                Node.Object(d);
            }
            Node.Object(ShadowModelRoot.Bones[0]);

        }

    }
}
