using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using MeleeLib.IO;

namespace MeleeLib.DAT
{
    /// <summary>
    /// MATERIAL Data that holds information about a <see cref="DatDOBJ"/>'s Material
    /// </summary>
    public class DatMaterial : DatNode
    {
        public int unk1 = 0;
        public int Flags = 0;
        public int unk2 = 0;

        public DatTexture[] Textures
        {
            get
            {
                return _textures.ToArray();
            }
        }
        private List<DatTexture> _textures = new List<DatTexture>();

        public DatMaterialColor MaterialColor;
        public DatPixelProcessing PixelProcessing;

        public DatMaterial()
        {

        }

        public void AddTexture(DatTexture t)
        {
            if (t.Parent != null)
                t.Parent.RemoveTexture(t);
            t.Parent = this;
            _textures.Add(t);
        }

        public void RemoveTexture(DatTexture t)
        {
            t.Parent = null;
            _textures.Remove(t);
        }

        public void Deserialize(DATReader d, DATRoot Root)
        {
            unk1 = d.Int();
            Flags = d.Int();

            int TexOff = d.Int();
            int matOff = d.Int();
            unk2 = d.Int();
            int PixelProcessOffset = d.Int();
            
            if (TexOff != 0)
            {
                d.Seek(TexOff);
                DatTexture t = new DatTexture();
                t.Deserialize(d, Root, this);
            }
            if(matOff != 0)
            {
                d.Seek(matOff);
                MaterialColor = new DatMaterialColor();
                MaterialColor.Deserialize(d, Root);
            }
            if(PixelProcessOffset != 0)
            {
                d.Seek(PixelProcessOffset);
                PixelProcessing = new DatPixelProcessing();
                PixelProcessing.Deserialize(d, Root);
            }

            if (unk1 != 0 || unk2 != 0) throw new Exception("TODO: Unsupported render struct for material");
        }

        public override void Serialize(DATWriter Node)
        {
            foreach(DatTexture t in _textures)
                t.Serialize(Node);

            if (MaterialColor != null)
                MaterialColor.Serialize(Node);

            if (PixelProcessing != null)
                PixelProcessing.Serialize(Node);

            Node.AddObject(this);
            Node.Int(unk1);
            Node.Int(Flags);

            if (_textures.Count > 0)
                Node.Object(_textures[0]);
            else
                Node.Int(0);

            if (MaterialColor == null)
                Node.Int(0);
            else
                Node.Object(MaterialColor);

            Node.Int(unk2);

            if (PixelProcessing == null)
                Node.Int(0);
            else
                Node.Object(PixelProcessing);
        }
    }
}
