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
        public int Unknown1 { get; set; }
        public int Unknown2 { get; set; }
    }

    public class KAR_ZoneCollisionTriangle : IHSDNode
    {
        public int UnknownZone { get; set; }
        public int V1 { get; set; }
        public int V2 { get; set; }
        public int V3 { get; set; }
        public int Color { get; set; }
        public int Unknown { get; set; }
    }

    public class KAR_ZoneCollisionJoint : IHSDNode
    {
        public int BoneID { get; set; }
        public int ZoneVertexStart { get; set; }
        public int ZoneVertexSize { get; set; }
        public int ZoneFaceStart { get; set; }
        public int ZoneFaceSize { get; set; }
        public int UnknownPointer { get; set; }
        public int Pointer { get; set; }
        public int UnknownStart1 { get; set; }
        public int UnknownSize1 { get; set; }
        public int UnknownStart2 { get; set; }
        public int UnknownSize2 { get; set; }
        public int UnknownStart3 { get; set; }
        public int UnknownSize3 { get; set; }
        public int UnknownStart4 { get; set; }
        public int UnknownSize4 { get; set; }
        public int UnknownStart5 { get; set; }
        public int UnknownSize5 { get; set; }
        public int UnknownStart6 { get; set; }
        public int UnknownSize6 { get; set; }
    }

    public class KAR_GrCollisionNode : IHSDNode
    {
        public List<GXVector3> Vertices = new List<GXVector3>();
        public List<KAR_CollisionTriangle> Faces = new List<KAR_CollisionTriangle>();
        public List<KAR_CollisionJoint> Joints = new List<KAR_CollisionJoint>();

        public List<GXVector3> ZoneVertices = new List<GXVector3>();
        public List<KAR_ZoneCollisionTriangle> ZoneFaces = new List<KAR_ZoneCollisionTriangle>();
        public List<KAR_ZoneCollisionJoint> ZoneJoints = new List<KAR_ZoneCollisionJoint>();

        public override void Open(HSDReader Reader)
        {
            var vertexOffset = Reader.ReadUInt32();
            var vertexCount = Reader.ReadInt32();
            var faceOffset = Reader.ReadUInt32();
            var faceCount = Reader.ReadInt32();
            var jointOffset = Reader.ReadUInt32();
            var jointCount = Reader.ReadInt32();
            var zonevertexOffset = Reader.ReadUInt32();
            var zonevertexCount = Reader.ReadInt32();
            var zonefaceOffset = Reader.ReadUInt32();
            var zonefaceCount = Reader.ReadInt32();
            var zonejointOffset = Reader.ReadUInt32();
            var zonejointCount = Reader.ReadInt32();

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

            Reader.Seek(zonevertexOffset);
            for (int i = 0; i < zonevertexCount; i++)
                ZoneVertices.Add(new GXVector3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle()));

            Reader.Seek(zonefaceOffset);
            for (int i = 0; i < zonefaceCount; i++)
            {
                KAR_ZoneCollisionTriangle tri = new KAR_ZoneCollisionTriangle();
                tri.Open(Reader);
                ZoneFaces.Add(tri);
            }

            Reader.Seek(zonejointOffset);
            for (int i = 0; i < zonejointCount; i++)
            {
                KAR_ZoneCollisionJoint joint = new KAR_ZoneCollisionJoint();
                joint.Open(Reader);
                ZoneJoints.Add(joint);
            }
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

            Writer.AddObject(ZoneVertices);
            foreach (var v in ZoneVertices)
            {
                Writer.Write(v.X);
                Writer.Write(v.Y);
                Writer.Write(v.Z);
            }

            Writer.AddObject(ZoneFaces);
            foreach (var v in ZoneFaces)
            {
                Writer.WriteObject(v);
            }

            Writer.AddObject(ZoneJoints);
            foreach (var v in ZoneJoints)
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

            Writer.WritePointer(ZoneVertices);
            Writer.Write(ZoneVertices.Count);
            Writer.WritePointer(ZoneFaces);
            Writer.Write(ZoneFaces.Count);
            Writer.WritePointer(ZoneJoints);
            Writer.Write(ZoneJoints.Count);
        }
    }
}
