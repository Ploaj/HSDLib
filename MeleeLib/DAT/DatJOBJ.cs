using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.IO;

namespace MeleeLib.DAT
{
    /// <summary>
    /// JOINT files that hold bone transformation information as well as <see cref="DatDOBJ"/> data
    /// </summary>
    public class DatJOBJ : DatNode
    {
        private int Padding = 0;
        private int Flags = 0;

        public float RX, RY, RZ;
        public float SX, SY, SZ;
        public float TX, TY, TZ;

        private int UnkEnd = 0;

        /// <summary>
        /// Inverse Transform Matrix of this Node
        /// </summary>
        public Matrix4x4 Inverse = null;

        public DatPath Path;

        public int ID;

        private List<DatJOBJ> Children = new List<DatJOBJ>();
        public DatJOBJ Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                if (_parent != null) _parent.Children.Remove(this);
                _parent = value;
                if (_parent != null) _parent.Children.Add(this);
            }
        }
        private DatJOBJ _parent;
        public List<DatDOBJ> DataObjects = new List<DatDOBJ>();

        public DatJOBJ()
        {
        }

        /// <summary>
        /// Gets the Children <see cref="DatJOBJ"/>s for this Node
        /// Read Only
        /// </summary>
        /// <returns>An array of <see cref="DatJOBJ"/>s</returns>
        public DatJOBJ[] GetChildren()
        {
            return Children.ToArray();
        }

        /// <summary>
        /// Adds a new <see cref="DatJOBJ"/>
        /// </summary>
        /// <param name="Child">The child <see cref="DatJOBJ"/> to add</param>
        public void AddChild(DatJOBJ Child)
        {
            Child.Parent = this;
        }

        /// <summary>
        /// Reads this node from a file
        /// </summary>
        /// <param name="d">FileData</param>
        /// <param name="Root">The <see cref="DATRoot"/> this object belongs to</param>
        public void Deserialize(DATReader d, DATRoot Root)
        {
            ID = d.Pos();
            Padding = d.Int(); // string index
            Flags = d.Int();
            Text += "_" + Flags.ToString("X");

            int ChildOffset = d.Int();
            int NextOffset = d.Int();
            int DOBJOffset = d.Int();

            RX = d.Float(); RY = d.Float(); RZ = d.Float();
            SX = d.Float(); SY = d.Float(); SZ = d.Float();
            TX = d.Float(); TY = d.Float(); TZ = d.Float();

            int InverseTransformOffset = d.Int();
            UnkEnd = d.Int();

            if (InverseTransformOffset != 0)
            {
                d.Seek(InverseTransformOffset);
                Matrix4x4 Inv = new Matrix4x4();
                Inv.Deserialize(d, Root);
                Inverse = Inv;
            }

            if (ChildOffset != 0)
            {
                d.Seek(ChildOffset);
                DatJOBJ Child = new DatJOBJ();
                AddChild(Child);
                Child.Deserialize(d, Root);
            }

            if (NextOffset != 0)
            {
                d.Seek(NextOffset);
                DatJOBJ Next = new DatJOBJ();
                Next.Parent = Parent;
                Next.Deserialize(d, Root);
            }

            //Data
            if (DOBJOffset != 0)
            {
                d.Seek(DOBJOffset);
                if ((Flags & 0x4000) != 0)
                {
                    Path = new DatPath();
                    Path.Deserialize(d, Root);
                }
                else
                {
                    DatDOBJ Next = new DatDOBJ();
                    Next.Deserialize(d, Root, this);
                }
            }
        }


        /// <summary>
        /// Saves this node to a file
        /// </summary>
        /// <param name="Node"><see cref="DATWriter"/></param>
        public override void Serialize(DATWriter Node)
        {
            foreach (DatDOBJ n in DataObjects)
                n.Material.Serialize(Node);

            foreach (DatDOBJ n in DataObjects)
                n.SerializeData(Node);

            foreach (DatNode n in Children)
                n.Serialize(Node);

            if (Path != null)
                Path.Serialize(Node);

            Node.AddObject(this);
            Node.Int(Padding);
            Node.Int(Flags);
            
            if (Children.Count > 0)
                Node.Object(Children[0]);
            else Node.Int(0);

            if(_parent != null && _parent.Children.IndexOf(this)+1 < _parent.Children.Count)
                Node.Object(_parent.Children[_parent.Children.IndexOf(this) + 1]);
            else Node.Int(0);

            if(Path != null)
            {
                Node.Object(Path);
            }
            else
            if(DataObjects.Count > 0)
                Node.Object(DataObjects[0]);
            else
                Node.Int(0);

            Node.Float(RX); Node.Float(RY); Node.Float(RZ);
            Node.Float(SX); Node.Float(SY); Node.Float(SZ);
            Node.Float(TX); Node.Float(TY); Node.Float(TZ);

            if(Inverse == null)
            {
                Node.Int(0);
                Node.Int(0);
            }
            else
            {
                Node.Object(Inverse);
                Node.Int(0);
                Inverse.Serialize(Node);
            }
        }
    }

}
