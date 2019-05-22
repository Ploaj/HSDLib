using System.Collections.Generic;
using HSDLib.MaterialAnimation;

namespace HSDLib.Animation
{
    public class HSD_FigaTree : IHSDNode
    {
        public int Type { get; set; }
        
        public float FrameCount { get; set; }
        
        public HSD_MatAnimJoint MatAnimJoint { get; set; }

        public List<HSD_AnimNode> Nodes = new List<HSD_AnimNode>();

        public override void Open(HSDReader Reader)
        {
            Type = Reader.ReadInt32();
            if (Type == -1)
                MatAnimJoint = Reader.ReadObject<HSD_MatAnimJoint>(Reader.ReadUInt32());
            else
                Reader.ReadInt32(); // nothing
            FrameCount = Reader.ReadSingle();
            uint TrackCountOffset = Reader.ReadUInt32();
            uint TrackOffset = Reader.ReadUInt32();

            Reader.Seek(TrackCountOffset);
            List<byte> TrackCounts = new List<byte>();
            byte TrackCount;
            while ((TrackCount = Reader.ReadByte()) != 0xFF)
            {
                TrackCounts.Add(TrackCount);
            }

            int track = 0;
            for (int i = 0; i < TrackCounts.Count; i++)
            {
                HSD_AnimNode Node = new HSD_AnimNode();
                Nodes.Add(Node);
                for (int j = 0; j < TrackCounts[i]; j++)
                {
                    Reader.Seek((uint)(TrackOffset + 0xC * track++));
                    HSD_Track t = new HSD_Track();
                    t.Open(Reader);
                    Node.Tracks.Add(t);
                }
            }
        }

        public override void Save(HSDWriter Writer)
        {
            if (MatAnimJoint == null)
                Writer.WriteObject(MatAnimJoint);

            foreach (HSD_AnimNode node in Nodes)
            {
                foreach (HSD_Track t in node.Tracks)
                    t.Save(Writer);
            }

            object array = new object();
            Writer.AddObject(array);
            HSD_Track FirstTrack = null;
            foreach(HSD_AnimNode node in Nodes)
            {
                Writer.Write((byte)node.Tracks.Count);
                foreach (HSD_Track t in node.Tracks)
                    if (FirstTrack == null)
                        FirstTrack = t;
            }
            Writer.Write((byte)0xFF);
            Writer.Align(4);

            Writer.AddObject(this);
            Writer.Write(MatAnimJoint == null ? 1 : -1);
            if (MatAnimJoint == null)
                Writer.Write(0);
            else
                Writer.WritePointer(MatAnimJoint);
            Writer.Write(FrameCount);
            Writer.WritePointer(array);
            Writer.WritePointer(FirstTrack);
        }
    }
}
