using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.IO;

namespace MeleeLib.DAT
{
    /// <summary>
    /// DATA OBJECT files that hold Material and Polygon information
    /// </summary>
    public class DatDOBJ : DatNode
    {
        /// <summary>
        /// <see cref="DatMaterial"/> for this Node
        /// </summary>
        public DatMaterial Material = new DatMaterial();

        /// <summary>
        /// List of <see cref="DatPolygon"/> that belong to this Node
        /// </summary>
        public List<DatPolygon> Polygons = new List<DatPolygon>();

        /// <summary>
        /// Parent JOBJ
        /// </summary>
        public DatJOBJ Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                if (_parent != null) _parent.DataObjects.Remove(this);
                _parent = value;
                if (_parent != null) _parent.DataObjects.Add(this);
            }
        }
        private DatJOBJ _parent;

        /// <summary>
        /// Reads this node from a file
        /// </summary>
        /// <param name="d">FileData</param>
        /// <param name="Root">The <see cref="DATRoot"/> this object belongs to</param>
        public void Deserialize(DATReader d, DATRoot Root, DatJOBJ jobj)
        {
            Parent = jobj;

            int StringOffset = d.Int();
            int nextD = d.Int();
            int MatOff = d.Int();
            int PolyOff = d.Int();

            if (StringOffset != 0)
                Text = d.String(StringOffset);

            if(nextD != 0)
            {
                d.Seek(nextD);
                DatDOBJ next = new DatDOBJ();
                next.Deserialize(d, Root, jobj);
            }

            if(MatOff != 0)
            {
                d.Seek(MatOff);
                Material.Deserialize(d, Root);
            }

            if (PolyOff != 0)
            {
                d.Seek(PolyOff);
                new DatPolygon().Deserialize(d, Root, this);
            }
        }

        /// <summary>
        /// Saves this node to a file
        /// </summary>
        /// <param name="Node"><see cref="DATWriter"/></param>
        public override void Serialize(DATWriter Node)
        {
            Node.AddObject(this);
            Node.Int(0);
            if (this.Parent.DataObjects.IndexOf(this) + 1 < this.Parent.DataObjects.Count)
                Node.Object(this.Parent.DataObjects[this.Parent.DataObjects.IndexOf(this) + 1]);
            else
                Node.Int(0);

            Node.Object(Material);
            if (Polygons.Count > 0)
                Node.Object(Polygons[0]);
            else
                Node.Int(0);
        }

        /// <summary>
        /// Saves this node's Sub Data to a file
        /// </summary>
        /// <param name="Node"><see cref="DATWriter"/></param>
        public void SerializeData(DATWriter Node)
        {
            foreach (DatPolygon p in Polygons)
                p.SerializeAttributes(Node);

            //Node.Align(0x20);
            
            foreach (DatPolygon p in Polygons)
                p.SerializeDisplayList(Node);

            foreach (DatPolygon p in Polygons)
                p.Serialize(Node);

            Serialize(Node);
        }
    }
}
