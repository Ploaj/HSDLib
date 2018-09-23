using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.GCX;
using MeleeLib.IO;

namespace MeleeLib.DAT
{
    /// <summary>
    /// Holds information about a DAT Texture's Data
    /// </summary>
    public class DatImage : DatNode
    {
        public ushort Width, Height;
        public TPL_TextureFormat Format;
        public int Mipmap;
        public float MinLOD;
        public float MaxLOD;
        public byte[] Data;

        /// <summary>
        /// Reads this node from a file
        /// </summary>
        /// <param name="d">FileData</param>
        /// <param name="Root">The <see cref="DATRoot"/> this object belongs to</param>
        public void Deserialize(DATReader d, DATRoot Root)
        {
            int Offset = d.Int();
            Width = (ushort)d.Short();
            Height = (ushort)d.Short();
            Format = (TPL_TextureFormat)d.Int();
            Mipmap = d.Int();
            MinLOD = d.Float();
            MaxLOD = d.Float();
            
            Data = d.getSection(Offset, TPL.textureByteSize((TPL_TextureFormat)Format, Width, Height));
        }

        /// <summary>
        /// Saves this node to a file
        /// </summary>
        /// <param name="Node"><see cref="DATWriter"/></param>
        public override void Serialize(DATWriter Node)
        {
            Node.AddObject(this);
            Node.Object(Data);
            Node.Short((short)Width);
            Node.Short((short)Height);
            Node.Int((int)Format);
            Node.Int(Mipmap);
            Node.Float(MinLOD);
            Node.Float(MaxLOD);
        }
    }
}
