using HSDRaw.AirRide.Gr.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace HSDRawViewer.GUI.Plugins.AirRide
{
    public class AirRideGrCollisionManager
    {
        /// <summary>
        /// 
        /// </summary>
        public class ZoneJointCollision : JointCollision
        {
            public KAR_ZoneCollisionJoint ZoneJoint;
        }

        /// <summary>
        /// 
        /// </summary>
        public class ZoneCollisionFace : CollisionFace
        {
            public int UnknownZone { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public class JointCollision
        {
            public int Bone { get; set; }
            public CollisionFace[] Faces { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public class CollisionFace
        {
            public Vector3 v1 { get; set; }
            public Vector3 v2 { get; set; }
            public Vector3 v3 { get; set; }
            public int Flags { get; set; }
            public int Unknown { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public JointCollision[] Joints { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ZoneJointCollision[] ZoneJoints { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coll"></param>
        public void LoadCollision(KAR_grCollisionNode coll)
        {
            // Load Collisions
            var tris = coll.Triangles;
            var verts = coll.Vertices;

            Joints = new JointCollision[coll.JointCount];
            for(int i = 0; i < coll.JointCount; i++)
            {
                var joint = coll.Joints[i];
                Joints[i] = new JointCollision()
                {
                    Bone = joint.BoneID,
                    Faces = new CollisionFace[joint.FaceSize]
                };

                for (int j = joint.FaceStart; j < joint.FaceStart + joint.FaceSize; j++)
                {
                    var face = tris[j];

                    var v1 = verts[face.V1];
                    var v2 = verts[face.V2];
                    var v3 = verts[face.V3];

                    Joints[i].Faces[j - joint.FaceStart] = new CollisionFace()
                    {
                        Flags = face.Flags,
                        Unknown = face.Unknown,
                        v1 = new Vector3(v1.X, v1.Y, v1.Z),
                        v2 = new Vector3(v2.X, v2.Y, v2.Z),
                        v3 = new Vector3(v3.X, v3.Y, v3.Z),
                    };
                }
            }

            // Load Zones
            var ztris = coll.ZoneTriangles;
            verts = coll.ZoneVertices;

            ZoneJoints = new ZoneJointCollision[coll.ZoneJointCount];
            for (int i = 0; i < coll.ZoneJointCount; i++)
            {
                var joint = coll.ZoneJoints[i];
                ZoneJoints[i] = new ZoneJointCollision()
                {
                    Bone = joint.BoneID,
                    Faces = new ZoneCollisionFace[joint.ZoneFaceSize],
                    ZoneJoint = joint
                };

                for (int j = 0; j < joint.ZoneFaceSize; j++)
                {
                    var face = ztris[j + joint.ZoneFaceStart];

                    var v1 = verts[face.V1];
                    var v2 = verts[face.V2];
                    var v3 = verts[face.V3];

                    Joints[i].Faces[j] = new ZoneCollisionFace()
                    {
                        Flags = face.Color,
                        Unknown = face.Unknown,
                        UnknownZone = face.UnknownZone,
                        v1 = new Vector3(v1.X, v1.Y, v1.Z),
                        v2 = new Vector3(v2.X, v2.Y, v2.Z),
                        v3 = new Vector3(v3.X, v3.Y, v3.Z),
                    };
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public KAR_grCollisionNode GenerateCollision()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public void RenderPrimitives(bool renderCollisions, bool renderZones)
        {
            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.Color3(1f, 1f, 1f);
            
            GL.Begin(PrimitiveType.Triangles);
            if (renderCollisions)
                foreach (var j in Joints)
                {
                    foreach (var face in j.Faces)
                    {
                        GL.Vertex3(face.v1);
                        GL.Vertex3(face.v2);
                        GL.Vertex3(face.v3);
                    }
                }
            if (renderZones)
                foreach (var j in ZoneJoints)
                {
                    foreach (var face in j.Faces)
                    {
                        GL.Vertex3(face.v1);
                        GL.Vertex3(face.v2);
                        GL.Vertex3(face.v3);
                    }
                }
            GL.End();

            GL.Disable(EnableCap.DepthTest);

            GL.Color3(0, 0, 0);

            GL.Begin(PrimitiveType.Lines);
            if (renderCollisions)
                foreach (var j in Joints)
                {
                    foreach (var face in j.Faces)
                    {
                        GL.Vertex3(face.v1);
                        GL.Vertex3(face.v2);

                        GL.Vertex3(face.v2);
                        GL.Vertex3(face.v3);

                        GL.Vertex3(face.v3);
                        GL.Vertex3(face.v1);
                    }
                }
            if (renderZones)
                foreach (var j in ZoneJoints)
                {
                    foreach (var face in j.Faces)
                    {
                        GL.Vertex3(face.v1);
                        GL.Vertex3(face.v2);

                        GL.Vertex3(face.v2);
                        GL.Vertex3(face.v3);

                        GL.Vertex3(face.v3);
                        GL.Vertex3(face.v1);
                    }
                }
            GL.End();

            GL.PopAttrib();
        }

    }
}
