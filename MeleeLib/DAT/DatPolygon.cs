using System;
using System.Collections.Generic;
using MeleeLib.GCX;
using MeleeLib.IO;

namespace MeleeLib.DAT
{
    public class DatBoneWeight : DatNode
    {
        public DatJOBJ jobj;
        public float Weight;
        public int Offset; // for backup
    }

    public class DatPolygon : DatNode
    {
        public int Unk1;
        public int Flags = 0x8000;

        public GXAttribGroup AttributeGroup;
        public List<GXDisplayList> DisplayLists = new List<GXDisplayList>();

        public DatDOBJ ParentDOBJ
        {
            get
            {
                return _parent;
            }
            set
            {
                if (_parent != null)
                    _parent.Polygons.Remove(this);
                _parent = value;
                if (_parent != null)
                    _parent.Polygons.Add(this);
            }
        }
        private DatDOBJ _parent;
        
        public List<List<DatBoneWeight>> BoneWeightList = new List<List<DatBoneWeight>>();

        public DatPolygon()
        {
        }

        public void Deserialize(DATReader d, DATRoot Root, DatDOBJ dobj)
        {
            ParentDOBJ = dobj;

            Unk1 = d.Int();
            int NextOff = d.Int();
            int VertAttOff = d.Int();
            Flags = d.Short();
            int DisplayListSize = d.Short()*32;
            int DisplayListOffset = d.Int();
            int WeightListOffset = d.Int();
            //if (Flags == 0xA001) Visible = true;

            if (VertAttOff != 0)
            {
                AttributeGroup = Root.GetAttribute(VertAttOff, d);
            }

            if (WeightListOffset != 0)
            {
                switch (Flags & 0x3000)
                {
                    case 0x0000: // SingleBound
                        {
                            var jobjs = new List<DatBoneWeight>();

                            DatBoneWeight bw = new DatBoneWeight();
                            bw.jobj = Root.GetJOBJ(WeightListOffset);
                            bw.Weight = 1;

                            jobjs.Add(bw);
                            BoneWeightList.Add(jobjs);
                        }
                        break;
                    case 0x1000:
                        {
                            Console.WriteLine("Unknown Flag " + Flags.ToString("x"));
                        }
                        break;
                    case 0x2000: // Bones + Weight List
                        {
                            d.Seek(WeightListOffset);
                            int offset = 0;
                            while ((offset = d.Int()) != 0)
                            {
                                var temp = d.Pos();
                                var jobjs = new List<DatBoneWeight>();

                                d.Seek(offset);

                                int off1 = d.Int();
                                float wei1 = d.Float();
                                while (off1 != 0)
                                {
                                    DatBoneWeight bw = new DatBoneWeight();
                                    jobjs.Add(bw);
                                    bw.jobj = Root.GetJOBJ(off1);
                                    bw.Weight = wei1;
                                    bw.Offset = off1;

                                    off1 = d.Int();
                                    wei1 = d.Float();
                                }

                                BoneWeightList.Add(jobjs);

                                d.Seek(temp);
                            }
                        }
                        break;
                }
            }

            if (DisplayListOffset != 0)
            {
                d.Seek(DisplayListOffset);
                while(d.Pos() < DisplayListOffset + DisplayListSize)
                {
                    int dl = d.Byte();
                    if (dl == 0) break;
                    d.Seek(d.Pos()-1);
                    GXDisplayList DL = new GXDisplayList();
                    DL.Deserialize(d, Root, AttributeGroup.Attributes);
                    DisplayLists.Add(DL);
                }
            }

            if (NextOff != 0)
            {
                d.Seek(NextOff);
                new DatPolygon().Deserialize(d, Root, dobj);
            }
        }


        public override void Serialize(DATWriter Node)
        {
            switch (Flags&0x3000)
            {
                case 0x1000:
                    break;
                case 0x2000:
                    foreach (List<DatBoneWeight> b in BoneWeightList)
                    {
                        Node.AddObject(b);
                        foreach(DatBoneWeight w in b)
                        {
                            Node.Object(w.jobj);
                            Node.Float(w.Weight);
                        }
                        Node.Int(0);
                        Node.Int(0);
                    }
                    Node.AddObject(BoneWeightList);
                    foreach (List<DatBoneWeight> b in BoneWeightList)
                    {
                        Node.Object(b);
                    }
                    Node.Int(0);
                    break;
            }

            Node.AddObject(this);
            Node.Int(Unk1);
            List<DatPolygon> polys = ParentDOBJ.Polygons;
            if (polys.IndexOf(this) + 1 < polys.Count)
                Node.Object(polys[polys.IndexOf(this) + 1]);
            else
                Node.Int(0);

            Node.Object(AttributeGroup);
            Node.Short((short)Flags);
            Node.Short((short)(DisplayListSize / 32));
            if (DisplayLists.Count > 0)
                Node.Object(DisplayLists[0]);
            else
                Node.Int(0);

            //Todo: non single bound
            //BoneList b = new BoneList();
            //Node.Object(b);
            //SerializeandcreateBoneList
            if (BoneWeightList.Count == 0 || BoneWeightList[0].Count == 0)
                Node.Int(0);
            else
            if ((Flags & 0x3000) == 0x0000)
            {
                Node.Object(BoneWeightList[0][0].jobj);
            }
            else
                Node.Object(BoneWeightList);
            
        }

        public void SerializeAttributes(DATWriter Node)
        {
            //Serialize Attributes
            if (!Node.ContainsObject(AttributeGroup))
            {
                Node.AddObject(AttributeGroup);
                foreach (GXAttr a in AttributeGroup.Attributes)
                {
                    a.Serialize(Node);
                }
                Node.Int((int)GXAttribName.GX_VA_NULL);
                Node.Int(2);
                Node.Int(0);
                Node.Int(4);
            }
        }

        private int DisplayListSize = 0;
        public void SerializeDisplayList(DATWriter Node)
        {
            //SerializeDisplayLists
            int start = Node.Size();
            foreach (GXDisplayList dl in DisplayLists)
            {
                dl.Serialize(Node, AttributeGroup.Attributes);
            }
            if(DisplayLists.Count > 0) Node.Byte(0);
            DisplayListSize = Node.Size() - start;
            while (DisplayListSize % 0x20 > 0)
            {
                Node.Byte(0);
                DisplayListSize = Node.Size() - start;
            }
        }
    }
}