using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace HSDRawViewer.Converters.Melee
{
    [Flags]
    public enum SSFLineFlag
    {
        RightLedge = 1,
        LeftLedge = 2,
        DropThrough = 4
    }

    [Serializable]
    public class SSF
    {
        public List<SSFPoint> Points = new();

        public List<SSFGroup> Groups = new();

        public static SSF Open(string filePath)
        {
            using FileStream stream = System.IO.File.OpenRead(filePath);
            XmlSerializer serializer = new(typeof(SSF));
            return serializer.Deserialize(stream) as SSF;
        }

        public void Save(string filePath)
        {
            using TextWriter writer = new StreamWriter(filePath);
            XmlSerializer serializer = new(typeof(SSF));
            serializer.Serialize(writer, this);
        }
    }

    public class SSFPoint
    {
        public float X;
        public float Y;
        public string Tag;
    }

    public class SSFGroup
    {
        public string Name;
        public string Bone;
        public List<SSFLine> Lines = new();
        public List<SSFVertex> Vertices = new();
    }

    public class SSFLine
    {
        public int Vertex1;
        public int Vertex2;
        public string Material;
        public SSFLineFlag Flags;
        public string Tags;
    }

    public class SSFVertex
    {
        public float X;
        public float Y;
    }
}
