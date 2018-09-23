using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.IO;
using MeleeLib.DAT.Animation;
using MeleeLib.DAT.MatAnim;

namespace MeleeLib.DAT.Melee
{

    public class Map_BoneID
    {
        public int BoneID;
        public int InfoID;
    }

    public class Map_Object_Group : DatNode
    {
        public DATRoot BoneRoot;

        public List<Map_BoneID> BoneIDs = new List<Map_BoneID>();

        public void Deserialize(DATReader r, DATRoot Root)
        {
            int JOBJOffset = r.Int();
            int BoneIDOffset = r.Int();
            int BoneIDCount = r.Int();

            BoneRoot = new DATRoot();
            if (JOBJOffset != 0)
            {
                r.Seek(JOBJOffset);
                DatJOBJ JOBJ = new DatJOBJ();
                BoneRoot.Bones.Add(JOBJ);
                JOBJ.Deserialize(r, BoneRoot);
            }

            r.Seek(BoneIDOffset);
            for(int i = 0; i < BoneIDCount; i++)
            {
                Map_BoneID bid = new Map_BoneID();
                bid.BoneID = r.Short();
                bid.InfoID = r.Short();
            }
        }
    }

    public class Map_Animation_Quake : DatNode
    {
        public List<Map_Animation_Quake> Children = new List<Map_Animation_Quake>();

        public Map_Animation_Quake Parent
        {
            get{
                return _parent;
            }
            set{
                if (_parent != null)
                    _parent.Children.Remove(this);
                _parent = value;
                if (_parent != null)
                    _parent.Children.Add(this);
            }
        }
        public Map_Animation_Quake _parent;

        public int Unk1;
        public int Unk2;
        public Map_Animation_Data NodeData;

        public void Deserialize(DATReader r, DATRoot Root, Map_Animation_Quake Parent)
        {
            this.Parent = Parent;
            int Child = r.Int();
            int Next = r.Int();
            int Data = r.Int();
            Unk1 = r.Int();
            Unk2 = r.Int();

            //Console.WriteLine(Data.ToString("X") + " " + Unk1 + " " + Unk2);

            if (Next > 0)
            {
                r.Seek(Next);
                Map_Animation_Quake q = new Map_Animation_Quake();
                q.Deserialize(r, Root, Parent);
            }

            if (Child > 0)
            {
                r.Seek(Child);
                Map_Animation_Quake q = new Map_Animation_Quake();
                q.Deserialize(r, Root, this);
            }

            if(Data > 0)
            {
                r.Seek(Data);
                NodeData = new Map_Animation_Data();
                NodeData.Deserialize(r, Root);
            }
        }

        public Map_Animation_Quake[] GetNodesInOrder()
        {
            Queue<Map_Animation_Quake> Que = new Queue<Map_Animation_Quake>();
            foreach (Map_Animation_Quake j in Children)
                Enqueue(j, Que);
            List<Map_Animation_Quake> JOBJS = new List<Map_Animation_Quake>();
            while (Que.Count > 0)
                JOBJS.Add(Que.Dequeue());
            return JOBJS.ToArray();
        }

        private void Enqueue(Map_Animation_Quake jobj, Queue<Map_Animation_Quake> Que)
        {
            Que.Enqueue(jobj);
            foreach (Map_Animation_Quake j in jobj.Children)
                Enqueue(j, Que);
        }
    }

    public class Map_Animation_Data : DatAnimationNode
    {
        public float FrameCount;
        public int Unk1;
        public DatJOBJ JOBJ;

        public void Deserialize(DATReader r, DATRoot Root)
        {
            Unk1 = r.Int();
            FrameCount = r.Float();
            int DataOffset = r.Int();
            int JOBJPointer = r.Int();
            
            Tracks = new List<DatAnimationTrack>();

            if (DataOffset != 0)
            {
                r.Seek(DataOffset);
                Map_Animation_Track t = new Map_Animation_Track();
                t.Deserialize(r, Root, this);
            }

            if(JOBJPointer > 0)
            {
                r.Seek(JOBJPointer);
                JOBJ = new DatJOBJ();
                JOBJ.Deserialize(r, Root);
            }
        }
    }

    public class Map_Animation_Track : DatAnimationTrack
    {
        public int Flags;
        public void Deserialize(DATReader r, DATRoot Root, Map_Animation_Data Parent)
        {
            Parent.Tracks.Add(this);
            int Next = r.Int();
            int DataSize = r.Int();
            Flags = r.Int();
            AnimationType = (AnimTrackType)r.Byte();
            int Flag1 = r.Byte();
            int Flag2 = r.Byte();
            ValueFormat = (GXAnimDataFormat)(Flag1 & 0xF0);
            TanFormat = (GXAnimDataFormat)(Flag2 & 0xF0);
            ValueScale = (int)Math.Pow(2, Flag1 & 0x1F);
            TanScale = (int)Math.Pow(2, Flag2 & 0x1F);
            r.Byte();
            int DataOffset = r.Int();

            Data = r.getSection(DataOffset, DataSize);

            if (Next != 0)
            {
                r.Seek(Next);
                Map_Animation_Track nt = new Map_Animation_Track();
                nt.Deserialize(r, Root, Parent);
            }
        }
    }

    public class Map_Model_Group
    {
        public DATRoot BoneRoot;
        public Map_Animation_Quake QuakeRoot = new Map_Animation_Quake();
        public DatMatAnim MatAnim;

        public void Deserialize(DATReader r, DATRoot Root)
        {
            int JOBJOffset = r.Int();
            int AnimationOff = r.Int();
            int MatAnimOff = r.Int();

            // 12 unknown offsets

            BoneRoot = new DATRoot();
            if (JOBJOffset > 0)
            {
                r.Seek(JOBJOffset);
                DatJOBJ JOBJ = new DatJOBJ();
                BoneRoot.Bones.Add(JOBJ);
                JOBJ.Deserialize(r, BoneRoot);
            }

            if(AnimationOff > 0)
            {
                r.Seek(AnimationOff);
                int Anim = r.Int();
                while(Anim != 0)
                {
                    int temp = r.Pos();
                    r.Seek(Anim);
                    Map_Animation_Quake q = new Map_Animation_Quake();
                    q.Deserialize(r, BoneRoot, QuakeRoot);
                    r.Seek(temp);
                    Anim = r.Int();
                }
            }

            if(MatAnimOff > 0)
            {
                r.Seek(MatAnimOff);
                //MatAnim = new DatMatAnim();
                //MatAnim.Deserialize(r, Root);
            }
        }
    }

    public class Map_Head : DatNode
    {
        public List<Map_Object_Group> NodeObjects = new List<Map_Object_Group>();
        public List<Map_Model_Group> ModelObjects =  new List<Map_Model_Group>();

        public void Deserialize(DATReader r, DATRoot Root)
        {
            Root.Map_Head = this;

            int NodeOff = r.Int();
            int NodeCount = r.Int();
            int ModelOff = r.Int();
            int ModelCount = r.Int();
            /*
             * 
    word  "unknown10" 
    word  "unknown14" 
    array "MHE" (MapHeadE) 
    word  "MHEn" 
    word  "unknown20" 
    word  "unknown24" 
    word  "unknown28" 
    word  "unknown2c" 
             * */

            if (NodeCount > 0)
            {
                NodeObjects = new List<Map_Object_Group>();
                for (int i = 0; i < NodeCount; i++)
                {
                    r.Seek(NodeOff + 0xC * i);
                    Map_Object_Group g = new Map_Object_Group();
                    g.Deserialize(r, Root);
                    NodeObjects.Add(g);
                }
            }
            if (ModelCount > 0)
            {
                ModelObjects = new List<Map_Model_Group>();
                for (int i = 0; i < ModelCount; i++)
                {
                    r.Seek(ModelOff + 52 * i);
                    Map_Model_Group g = new Map_Model_Group();
                    g.Deserialize(r, Root);
                    ModelObjects.Add(g);
                }
            }


        }

        public override void Serialize(DATWriter Node)
        {
        }
    }
}
