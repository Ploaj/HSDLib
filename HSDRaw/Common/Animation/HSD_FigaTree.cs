using HSDRaw.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HSDRaw.Common.Animation
{
    public class FigaTreeNode
    {
        public List<HSD_Track> Tracks = new List<HSD_Track>();
    }

    public class HSD_FigaTree : HSDAccessor
    {
        public override int TrimmedSize => 0x14;

        public int Type { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public float FrameCount { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        //TODO: material animation joint if type == -1

        public List<FigaTreeNode> Nodes
        {
            get
            {
                List<FigaTreeNode> nodes = new List<FigaTreeNode>();
                for (int i = 0; i < NodeCount; i++)
                    nodes.Add(new FigaTreeNode(){Tracks=GetTracksAt(i)});
                return nodes;
            }
            set
            {
                // create count table
                _s.References.Clear();

                var n = _s.GetCreateReference<HSDAccessor>(0x0C);

                int length = value.Count + 1;
                if (length % 4 != 0)
                    length += 4 - (length % 4);
                byte[] table = new byte[length];
                for (int i = 0; i < value.Count; i++)
                    table[i] = (byte)value[i].Tracks.Count;
                table[value.Count] = 0xFF;
                n._s.SetData(table);

                // create track structure
                
                var tracks = _s.GetCreateReference<HSDAccessor>(0x10);

                tracks._s.SetData(new byte[TrackCount * 0xC]);

                int trackindex = 0;
                for (int i = 0; i < value.Count; i++)
                    for (int j = 0; j < value[i].Tracks.Count; j++)
                        tracks._s.SetEmbededStruct(0xC * (trackindex++), value[i].Tracks[j]._s);
            }
        }

        public int NodeCount
        {
            get
            {
                var n = _s.GetReference<HSDAccessor>(0x0C);
                if (n == null)
                    return 0;
                int length = 0;
                foreach (var b in n._s.GetData())
                {
                    if (b == 0xFF)
                        break;
                    length++;
                }
                return length;
            }
        }

        public int TrackCount
        {
            get
            {
                var n = _s.GetReference<HSDAccessor>(0x0C);
                if (n == null)
                    return 0;
                int length = 0;
                foreach (var b in n._s.GetData())
                {
                    if (b == 0xFF)
                        break;
                    length += b;
                }
                return length;
            }
        }

        /// <summary>
        /// gets the tracks at index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private List<HSD_Track> GetTracksAt(int index)
        {
            if (index < 0 || index > NodeCount)
                throw new IndexOutOfRangeException("Track index out of range");

            var n = _s.GetReference<HSDAccessor>(0x0C);
            int offset = 0;
            for(int i = 0; i < index; i++)
            {
                offset += n._s.GetByte(i);
            }
            int trackCount = n._s.GetByte(index);

            var track = new List<HSD_Track>();

            for (int t = 0; t < trackCount; t++)
            {
                track.Add(GetTrackAt(offset + t));
            }

            return track;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private HSD_Track GetTrackAt(int index)
        {
            var tracks = _s.GetReference<HSDAccessor>(0x10);
            
            HSD_Track track = new HSD_Track();
            track._s = tracks._s.GetEmbeddedStruct(index * 0x0C, 0x0C);

            return track;
        }
    }
}
