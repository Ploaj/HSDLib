using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.IO;

namespace MeleeLib.DAT.MatAnim
{
    public class DatMatAnimDataInformation : DatNode
    {
        public uint Flags;
        public float Speed;
        public List<DatMatAnimTrack> Tracks = new List<DatMatAnimTrack>();
        public List<DatMatAnimTextureData> Textures = new List<DatMatAnimTextureData>();

        public void Deserialize(DATReader d, DATRoot Root)
        {
            Flags = (uint)d.Int(); // String offset?
            Speed = d.Float();
            int DataOffset = d.Int();
            int TextureListOffset = d.Int(); // 

            if(DataOffset != 0)
            {
                d.Seek(DataOffset);
                DatMatAnimTrack Region = new DatMatAnimTrack();
                Region.Deserialize(d, Root, Tracks);
            }

            if(TextureListOffset > 0)
            {
                d.Seek(TextureListOffset);
                int off = d.Int();
                while(off != 0)
                {
                    int temp = d.Pos();
                    d.Seek(off);
                    DatMatAnimTextureData Texture = new DatMatAnimTextureData();
                    Texture.Deserialize(d, Root);
                    Textures.Add(Texture);
                    d.Seek(temp);
                    off = d.Int();
                }
            }

            //if (Unk2 != 0) throw new Exception("Format not implemented " + Unk2.ToString("x"));
        }

        public override void Serialize(DATWriter Node)
        {
            foreach (DatMatAnimTrack r in Tracks)
                r.SerializeData(Node);

            foreach (DatMatAnimTrack r in Tracks)
                r.Serialize(Node, Tracks);

            foreach(DatMatAnimTextureData t in Textures)
            {
                t.Serialize(Node);
            }

            //Texture Table
            Node.AddObject(Textures);
            foreach (DatMatAnimTextureData t in Textures)
            {
                Node.Object(t);
            }

            Node.AddObject(this);
            Node.Int((int)Flags);
            Node.Float(Speed);
            if (Tracks.Count > 0)
                Node.Object(Tracks[0]);
            else
                Node.Int(0);
            if (Textures.Count > 0)
                Node.Object(Textures);
            else
                Node.Int(0);
        }
    }

    /// <summary>
    /// Is a node hierarchy that contains the material animations for a given node
    /// </summary>
    public class DatMatAnim : DatNode
    {
        public List<DatMatAnimGroup> Groups = new List<DatMatAnimGroup>();
        private List<DatMatAnim> Children = new List<DatMatAnim>();

        public DatMatAnim[] GetChildren()
        {
            return Children.ToArray();
        }

        public void Deserialize(DATReader d, DATRoot Root, List<DatMatAnim> Anims)
        {
            Anims.Add(this);

            //Console.WriteLine(d.Pos().ToString("X"));
            int Child = d.Int();
            int Next = d.Int();
            int Data = d.Int();

            //Console.WriteLine(Child + " " + Next);

            if (Child > 0)
            {
                d.Seek(Child);
                DatMatAnim m = new DatMatAnim();
                m.Deserialize(d, Root, Children);
            }
            if (Next > 0)
            {
                d.Seek(Next);
                DatMatAnim m = new DatMatAnim();
                m.Deserialize(d, Root, Anims);
            }
            if (Data > 0)
            {
                d.Seek(Data);
                DatMatAnimGroup m = new DatMatAnimGroup();
                m.Parent = this;
                m.Deserialize(d, Root);
            }
        }

        public void SerializeData(DATWriter Node)
        {
            foreach (DatMatAnimGroup g in Groups)
                g.Serialize(Node);
            foreach (DatMatAnim a in Children)
                a.SerializeData(Node);
        }

        public void Serialize(DATWriter Node, List<DatMatAnim> Anims)
        {
            Node.AddObject(this);
            
            if (Children.Count > 0)
                Node.Object(Children[0]);
            else
                Node.Int(0);

            int next = Anims.IndexOf(this) + 1;
            if (Anims.Count > next)
                Node.Object(Anims[next]);
            else
                Node.Int(0);

            if (Groups.Count > 0)
                Node.Object(Groups[0]);
            else
                Node.Int(0);

            foreach (DatMatAnim a in Children)
                a.Serialize(Node, Children);

            foreach (DatMatAnimGroup g in Groups)
                g.Serialize(Node);
        }

    }
}
