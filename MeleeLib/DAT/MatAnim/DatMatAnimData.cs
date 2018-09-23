using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.IO;

namespace MeleeLib.DAT.MatAnim
{
    public class DatMatAnimData : DatNode
    {
        public uint Flags; // TODO Figure out
        public DatMatAnimDataInformation Info;
        public List<DatMatAnimTextureData> Textures = new List<DatMatAnimTextureData>();
        public List<DatMatAnimPaletteData> Palettes = new List<DatMatAnimPaletteData>();

        public DatMatAnimGroup Parent;

        public void Deserialize(DATReader d, DATRoot Root, DatMatAnimGroup Group)
        {
            Parent = Group;
            Group.TextureData.Add(this);
            int start = d.Pos();
            int Next = d.Int(); 
            Flags = (uint)d.Int();
            int OffsetToInformation = d.Int();
            int OffsetToDataTableOffset = d.Int();
            int OffsetToPaletteTableOffset = d.Int();
            ushort Count = (ushort)d.Short();
            ushort PCount = (ushort)d.Short();

            if(Next > 0)
            {
                d.Seek(Next);
                DatMatAnimData next = new DatMatAnimData();
                next.Deserialize(d, Root, Group);
            }
            if (OffsetToInformation != 0)
            {
                d.Seek(OffsetToInformation);
                Info = new DatMatAnimDataInformation();
                Info.Deserialize(d, Root);
            }
            if (OffsetToDataTableOffset != 0)
            {
                d.Seek(OffsetToDataTableOffset);
                int[] offsets = new int[Count];
                for (int i = 0; i < Count; i++)
                    offsets[i] = d.Int();

                for (int i = 0; i < Count; i++)
                {
                    d.Seek(offsets[i]);
                    DatMatAnimTextureData Tex = new DatMatAnimTextureData();
                    Textures.Add(Tex);
                    Tex.Deserialize(d, Root);
                }
            }
            if (OffsetToPaletteTableOffset != 0)
            {
                d.Seek(OffsetToPaletteTableOffset);
                int[] offsets = new int[Count];
                for (int i = 0; i < PCount; i++)
                    offsets[i] = d.Int();

                for (int i = 0; i < PCount; i++)
                {
                    d.Seek(offsets[i]);
                    DatMatAnimPaletteData Tex = new DatMatAnimPaletteData();
                    Palettes.Add(Tex);
                    Tex.Deserialize(d, Root);
                }
            }
        }

        public override void Serialize(DATWriter Node)
        {
            Info.Serialize(Node);
            
            foreach (DatMatAnimTextureData d in Textures)
                d.Serialize(Node);

            foreach (DatMatAnimPaletteData d in Palettes)
                d.Serialize(Node);

            object temp = new object();
            Node.AddObject(temp);
            foreach (DatMatAnimTextureData d in Textures)
                Node.Object(d);

            object pals = new object();
            Node.AddObject(pals);
            foreach (DatMatAnimPaletteData d in Palettes)
                Node.Object(d);

            Node.AddObject(this);
            int nextindex = Parent.TextureData.IndexOf(this) + 1;
            if (Parent.TextureData.Count > nextindex)
                Node.Object(Parent.TextureData[nextindex]);
            else
                Node.Int(0);
            Node.Int((int)Flags);
            if (Info != null)
                Node.Object(Info);
            else
                Node.Int(0);
            if (Textures.Count > 0)
                Node.Object(temp);
            else
                Node.Int(0);
            if (Palettes.Count > 0)
                Node.Object(pals);
            else
                Node.Int(0);
            Node.Short((short)Textures.Count);
            Node.Short((short)Palettes.Count);
        }
    }
}
