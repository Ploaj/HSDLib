using System;
using System.Collections.Generic;
using MeleeLib.IO;

namespace MeleeLib.DAT.MatAnim
{
    public class DatMatAnimGroup : DatNode
    {
        public DatMatAnim Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                if (_parent != null) _parent.Groups.Remove(this);
                _parent = value;
                if (_parent != null) _parent.Groups.Add(this);
            }
        }
        private DatMatAnim _parent;

        public List<DatMatAnimData> TextureData = new List<DatMatAnimData>();
        public DatMatAnimDataInformation InformationStruct;
        public List<DatMatAnimTrack> AnimTracks = new List<DatMatAnimTrack>();

        public void Deserialize(DATReader d, DATRoot Root)
        {
            int Next = d.Int();
            int DataInformation = d.Int(); // TODO: What even is this structure?
            int DataOffset = d.Int();
            int TrackOffset = d.Int();

            if (Next != 0)
            {
                d.Seek(Next);
                DatMatAnimGroup g = new DatMatAnimGroup();
                g.Parent = Parent;
                g.Deserialize(d, Root);
            }
            if (DataInformation > 0) // todo relook because yoshi
            {
                Console.WriteLine("YOSHI " + DataInformation.ToString("X") + " anim group");
                d.Seek(DataInformation);
                InformationStruct = new DatMatAnimDataInformation();
                InformationStruct.Deserialize(d, Root);
            }
            if (DataOffset != 0)
            {
                d.Seek(DataOffset);
                DatMatAnimData g = new DatMatAnimData();
                g.Deserialize(d, Root, this);
            }

            if(TrackOffset != 0)
            {
                d.Seek(TrackOffset);
                DatMatAnimTrack track = new DatMatAnimTrack();
                track.Deserialize(d, Root, AnimTracks);
            }
        }

        public override void Serialize(DATWriter Node)
        {
            foreach (DatMatAnimData d in TextureData)
            {
                d.Serialize(Node);
            }

            if(InformationStruct != null)
            {
                InformationStruct.Serialize(Node);
            }

            foreach (DatMatAnimTrack track in AnimTracks)
            {
                track.SerializeData(Node);
                track.Serialize(Node, AnimTracks);
            }

            Node.AddObject(this);
            if (Parent != null && Parent.Groups.IndexOf(this) + 1 < Parent.Groups.Count)
                Node.Object(Parent.Groups[Parent.Groups.IndexOf(this) + 1]);
            else
                Node.Int(0);

            if (InformationStruct != null)
                Node.Object(InformationStruct);
            else
                Node.Int(0);

            if (TextureData.Count > 0)
                Node.Object(TextureData[0]);
            else
                Node.Int(0);

            if (AnimTracks.Count > 0)
                Node.Object(AnimTracks[0]);
            else
                Node.Int(0);
        }
    }
}
