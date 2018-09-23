using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.IO;

namespace MeleeLib.DAT.Animation
{
    public enum AnimTrackType
    {
        NONE = 0,
        XROT = 1,
        YROT = 2,
        ZROT = 3,
        WROT = 4,
        XPOS = 5,
        YPOS = 6,
        ZPOS = 7,
        XSCA = 8,
        YSCA = 9,
        ZSCA = 10,
        UNK = 0
    }

    public enum GXAnimDataFormat
    {
        Float = 0x00,
        Short = 0x20,
        UShort = 0x40,
        SByte = 0x60,
        Byte = 0x80
    }

    public enum InterpolationType
    {
        Step = 1,
        Linear = 2,
        HermiteValue = 3,
        Hermite = 4,
        HermiteCurve = 5,
        Constant = 6
    }

    public class DatAnimationTrack : DatNode
    {
        public AnimTrackType AnimationType;
        public GXAnimDataFormat ValueFormat;
        public GXAnimDataFormat TanFormat;
        public int ValueScale;
        public int TanScale;
        public ushort TrackFlags;
        public byte Unk;

        public byte[] Data;

        public void Deserialize(DATReader d, DATRoot Root)
        {
            int size = d.Short();
            TrackFlags = (ushort)d.Short();
            AnimationType = (AnimTrackType)d.Byte();
            int Flag1 = d.Byte();
            int Flag2 = d.Byte();
            ValueFormat = (GXAnimDataFormat)(Flag1 & 0xF0);
            TanFormat = (GXAnimDataFormat)(Flag2 & 0xF0);
            ValueScale = (int)Math.Pow(2, Flag1 & 0x1F);
            TanScale = (int)Math.Pow(2, Flag2 & 0x1F);
            Unk = d.Byte();

            int DataOffset = d.Int();

            Data = d.getSection(DataOffset, size);

            //Console.WriteLine(AnimationType.ToString() + " " + ValueFormat.ToString("x") + " " + TanFormat.ToString("x"));

            //if (TrackFlags != 0) Console.WriteLine("Warning: Unknown animation data 0x" + Unk.ToString("x"));
            //if (Unk != 0) Console.WriteLine("Warning: Unknown animation data 0x" + Unk.ToString("x"));
        }

        public void SerializeData(DATWriter Node)
        {
            Node.AddObject(Data);
            Node.Bytes(Data);
            Node.Align(4);
        }

        public override void Serialize(DATWriter Node)
        {
            Node.AddObject(this);
            Node.Short((short)Data.Length);
            Node.Short((short)TrackFlags);
            Node.Byte((byte)AnimationType);
            Node.Byte((byte)(((int)ValueFormat) | (int)Math.Log(ValueScale, 2)));
            Node.Byte((byte)(((int)TanFormat) | (int)Math.Log(TanScale, 2)));
            Node.Byte(Unk);
            Node.Object(Data);
        }
    }

    public class DatAnimationNode : DatNode
    {
        public List<DatAnimationTrack> Tracks = new List<DatAnimationTrack>();
        public int KeyFrameCount = 0;
    }

    public class DatAnimation : DatNode
    {
        public int Unk1 = 1;
        public int Unk2 = 0;
        public float FrameCount = 0;

        public List<DatAnimationNode> Nodes = new List<DatAnimationNode>();
    
        public void Deserialize(DATReader d, DATRoot Root)
        {
            Unk1 = d.Int();
            Unk2 = d.Int();
            FrameCount = d.Float();
            int FrameCountOffset = d.Int();
            int AnimDataOffset = d.Int();

            List<int> KeyFrameCounts = new List<int>();

            if (FrameCountOffset != 0)
            {
                d.Seek(FrameCountOffset);
                int count = d.Byte();
                do
                {
                    KeyFrameCounts.Add(count);
                    count = d.Byte();
                } while (count != 0xFF);
            }

            d.Seek(AnimDataOffset);
            for (int i = 0; i < KeyFrameCounts.Count; i++)
            {
                DatAnimationNode D = new DatAnimationNode();
                D.KeyFrameCount = KeyFrameCounts[i];
                Nodes.Add(D);
                for (int j = 0; j < KeyFrameCounts[i]; j++)
                {
                    DatAnimationTrack T = new DatAnimationTrack();
                    T.Deserialize(d, Root);
                    D.Tracks.Add(T);
                }
            }
        }

        public override void Serialize(DATWriter Node)
        {
            DatAnimationTrack first = null;
            foreach(DatAnimationNode node in Nodes)
            {
                foreach (DatAnimationTrack track in node.Tracks)
                {
                    if (first == null)
                        first = track;
                    track.SerializeData(Node);
                }
            }
            foreach (DatAnimationNode node in Nodes)
                foreach (DatAnimationTrack track in node.Tracks)
                    track.Serialize(Node);

            Node.AddObject(Nodes);
            foreach (DatAnimationNode node in Nodes)
                Node.Byte((byte)node.Tracks.Count);
            Node.Byte(0xFF);

            Node.AddObject(this);
            Node.Int(Unk1);
            Node.Int(Unk2);
            Node.Float(FrameCount);
            Node.Object(Nodes);
            
            if (first != null)
                Node.Object(first);
            else
                Node.Int(0);
        }

    }
}
