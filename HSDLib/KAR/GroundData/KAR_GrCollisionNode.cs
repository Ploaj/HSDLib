using HSDLib.Helpers;
using System.Collections.Generic;

namespace HSDLib.KAR
{
    public class KAR_CollisionTriangle : IHSDNode
    {
        public int V1 { get; set; }
        public int V2 { get; set; }
        public int V3 { get; set; }
        public int Color { get; set; }
        public int Unknown { get; set; }
    }

    public class KAR_CollisionJoint : IHSDNode
    {
        public int BoneID { get; set; }
        public int VertexStart { get; set; }
        public int VertexSize { get; set; }
        public int FaceStart { get; set; }
        public int FaceSize { get; set; }
        public int UnknownStart { get; set; }
        public int UnknownSize { get; set; }
    }

    public class KAR_GrCollisionNode : IHSDNode
    {
        public List<GXVector3> Vertices = new List<GXVector3>();

        public List<KAR_CollisionTriangle> Faces = new List<KAR_CollisionTriangle>();

        public List<KAR_CollisionJoint> Joints = new List<KAR_CollisionJoint>();

        public override void Open(HSDReader Reader)
        {
            var vertexOffset = Reader.ReadUInt32();
            var vertexCount = Reader.ReadInt32();
            var faceOffset = Reader.ReadUInt32();
            var faceCount = Reader.ReadInt32();
            var jointOffset = Reader.ReadUInt32();
            var jointCount = Reader.ReadInt32();
            var unk1Offset = Reader.ReadUInt32();
            var unk1Count = Reader.ReadInt32();
            var unk2Offset = Reader.ReadUInt32();
            var unk2Count = Reader.ReadInt32();
            var unk3Offset = Reader.ReadUInt32();
            var unk3Count = Reader.ReadInt32();

            Reader.Seek(vertexOffset);
            for (int i = 0; i < vertexCount; i++)
                Vertices.Add(new GXVector3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle()));

            Reader.Seek(faceOffset);
            for (int i = 0; i < faceCount; i++)
            {
                KAR_CollisionTriangle tri = new KAR_CollisionTriangle();
                tri.Open(Reader);
                Faces.Add(tri);
            }

            Reader.Seek(jointOffset);
            for (int i = 0; i < jointCount; i++)
            {
                KAR_CollisionJoint joint = new KAR_CollisionJoint();
                joint.Open(Reader);
                Joints.Add(joint);
            }

            if (unk1Offset != 0 || unk2Offset != 0 || unk3Offset != 0)
                throw new System.NotSupportedException("File node supported");
        }

        public override void Save(HSDWriter Writer)
        {
            Writer.AddObject(Vertices);
            foreach(var v in Vertices)
            {
                Writer.Write(v.X);
                Writer.Write(v.Y);
                Writer.Write(v.Z);
            }

            Writer.AddObject(Faces);
            foreach (var v in Faces)
            {
                Writer.WriteObject(v);
            }

            Writer.AddObject(Joints);
            foreach (var v in Joints)
            {
                Writer.WriteObject(v);
            }

            Writer.AddObject(this);
            Writer.WritePointer(Vertices);
            Writer.Write(Vertices.Count);
            Writer.WritePointer(Faces);
            Writer.Write(Faces.Count);
            Writer.WritePointer(Joints);
            Writer.Write(Joints.Count);

            Writer.Write(0);
            Writer.Write(0);
            Writer.Write(0);
            Writer.Write(0);
            Writer.Write(0);
            Writer.Write(0);
        }
    }
}
