using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.IO;
using MeleeLib.DAT.Animation;

namespace MeleeLib.DAT.MatAnim
{
    public class DatMatAnimTrack : DatAnimationTrack
    {
        public uint Flags;

        public void Deserialize(DATReader d, DATRoot Root, List<DatMatAnimTrack> Regions)
        {
            Regions.Add(this);
            int start = d.Pos();
            int NextOffset = d.Int();
            int DataSize = d.Int();
            Flags = (uint)d.Int();

            AnimationType = (AnimTrackType)d.Byte();
            int Flag1 = d.Byte();
            int Flag2 = d.Byte();
            ValueFormat = (GXAnimDataFormat)(Flag1 & 0xF0);
            TanFormat = (GXAnimDataFormat)(Flag2 & 0xF0);
            ValueScale = (int)Math.Pow(2, Flag1 & 0x1F);
            TanScale = (int)Math.Pow(2, Flag2 & 0x1F);
            d.Byte();
            
            int Offset = d.Int();
            Data = d.getSection(Offset, DataSize);
            
            if (NextOffset != 0)
            {
                d.Seek(NextOffset);
                DatMatAnimTrack Region = new DatMatAnimTrack();
                Region.Deserialize(d, Root, Regions);
            }
        }

        public void Serialize(DATWriter Node, List<DatMatAnimTrack> Regions)
        {
            Node.AddObject(this);
            if (Regions.IndexOf(this) + 1 < Regions.Count)
                Node.Object(Regions[Regions.IndexOf(this) + 1]);
            else
                Node.Int(0);
            Node.Int(Data.Length);
            Node.Int((int)Flags);
            Node.Byte((byte)AnimationType);
            Node.Byte((byte)(((int)ValueFormat) | (int)Math.Log(ValueScale, 2)));
            Node.Byte((byte)(((int)TanFormat) | (int)Math.Log(TanScale, 2)));
            Node.Byte(Unk);
            Node.Object(Data);
        }
    }
}
