using HSDRaw.AirRide.Gr.Data;
using HSDRaw.GX;
using HSDRawViewer.Rendering;
using IONET.Core.Model;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace HSDRawViewer.Tools.AirRide
{
    public class KARCollisionImporter
    {
        private readonly List<GXVector3> verts = new();
        private readonly List<KAR_CollisionTriangle> triangles = new();
        private readonly List<KAR_CollisionJoint> joints = new();

        private readonly List<GXVector3> zverts = new();
        private readonly List<KAR_ZoneCollisionTriangle> ztriangles = new();
        private readonly List<KAR_ZoneCollisionJoint> zjoints = new();

        /// <summary>
        /// 
        /// </summary>
        public KARCollisionImporter()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public KAR_grCollisionNode ImportFromModel(string filepath)
        {
            // import model scene
            IONET.Core.IOScene scene = IONET.IOManager.LoadScene(filepath, new IONET.ImportSettings());

            // calculate mesh bounding
            int meshIndex = 0;
            foreach (IOMesh mesh in scene.Models[0].Meshes)
            {
                mesh.MakeTriangles();

                if (mesh.Name.Contains("Zone"))
                {
                    ProcessZoneMesh(scene.Models[0], mesh);
                }
                else
                {
                    ProcessCollisionMesh(scene.Models[0], mesh);
                }

                meshIndex++;
            }

            // create new collision node
            return new KAR_grCollisionNode()
            {
                Vertices = verts.ToArray(),
                Triangles = triangles.ToArray(),
                Joints = joints.ToArray(),

                ZoneVertices = zverts.ToArray(),
                ZoneTriangles = ztriangles.ToArray(),
                ZoneJoints = zjoints.ToArray(),
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        private void ProcessCollisionMesh(IOModel model, IOMesh mesh)
        {
            string joint_name = null;
            Matrix4x4 inverse = Matrix4x4.Identity;
            foreach (IOVertex v in mesh.Vertices)
            {
                if (v.Envelope != null)
                {
                    foreach (IONET.Core.IOBoneWeight w in v.Envelope.Weights)
                    {
                        if (w.Weight > 0)
                        {
                            if (joint_name == null)
                            {
                                joint_name = w.BoneName;
                                Matrix4x4.Invert(model.Skeleton.GetBoneByName(joint_name).WorldTransform, out inverse);
                            }
                            else
                            {
                                if (!joint_name.Equals(w.BoneName))
                                {
                                    throw new Exception("Rigged collisions may only be rigged to a single bone");
                                }
                            }
                        }
                    }
                }
            }

            int vertStart = verts.Count;
            int faceStart = triangles.Count;

            foreach (IOVertex v in mesh.Vertices)
            {
                Vector3 p = Vector3.Transform(v.Position, inverse);
                verts.Add(new GXVector3() { X = p.X, Y = p.Y, Z = p.Z });
            }

            foreach (IOPolygon p in mesh.Polygons)
            {
                for (int i = 0; i < p.Indicies.Count; i += 3)
                {
                    KCCollFlag flag = 0;

                    if (p.MaterialName.ToLower().Contains("wall"))
                    {
                        flag = KCCollFlag.Wall;
                    }
                    else
                    if (p.MaterialName.ToLower().Contains("ceiling"))
                    {
                        flag = KCCollFlag.Ceiling;
                    }
                    else
                    if (p.MaterialName.ToLower().Contains("floor"))
                    {
                        flag = KCCollFlag.Ceiling;
                    }
                    else
                    {
                        IOVertex v3 = mesh.Vertices[p.Indicies[i + 2]];
                        IOVertex v2 = mesh.Vertices[p.Indicies[i + 1]];
                        IOVertex v1 = mesh.Vertices[p.Indicies[i]];

                        OpenTK.Mathematics.Vector3 normal = Math3D.CalculateSurfaceNormal(
                            new OpenTK.Mathematics.Vector3(v1.Position.X, v1.Position.Y, v1.Position.Z),
                            new OpenTK.Mathematics.Vector3(v2.Position.X, v2.Position.Y, v2.Position.Z),
                            new OpenTK.Mathematics.Vector3(v3.Position.X, v3.Position.Y, v3.Position.Z));

                        if (normal.Y > 0.8f)
                        {
                            flag = KCCollFlag.Floor;
                        }
                        else if (normal.Y < -0.8)
                        {
                            flag = KCCollFlag.Ceiling;
                        }
                        else
                        {
                            flag = KCCollFlag.Wall;
                        }
                    }

                    triangles.Add(new KAR_CollisionTriangle()
                    {
                        V1 = p.Indicies[i + 2] + vertStart,
                        V2 = p.Indicies[i + 1] + vertStart,
                        V3 = p.Indicies[i + 0] + vertStart,
                        Flags = flag,
                        SegmentMove = mesh.Name.Contains("MOVE"),
                    });
                }
            }

            joints.Add(new KAR_CollisionJoint()
            {
                BoneID = !string.IsNullOrEmpty(joint_name) ? model.Skeleton.IndexOf(model.Skeleton.GetBoneByName(joint_name)) : 0,
                FaceStart = faceStart,
                FaceSize = triangles.Count - faceStart,
                VertexStart = vertStart,
                VertexSize = verts.Count - vertStart,
            });
        }

        /// <summary>
        /// 
        /// </summary>
        private void ProcessZoneMesh(IOModel model, IOMesh mesh)
        {
            // zone mesh are always cubes with 6 sides
            // two triangles per side
            // so first we need to verify this

            foreach (IOVertex v in mesh.Vertices)
            {
                v.Normal = Vector3.Zero;
                v.UVs.Clear();
            }
            mesh.Optimize();

            if (mesh.Polygons.Count > 1)
            {
                throw new Exception("Too many polygons in zone");
            }

            if (mesh.Polygons.Count > 0 &&
                (mesh.Polygons[0].Indicies.Count / 3 != 12 ||
                mesh.Vertices.Count != 8))
            {
                throw new Exception($"Polygon has unexpected vertex or face count F: {mesh.Polygons[0].Indicies.Count / 3} V: {mesh.Vertices.Count}");
            }

            int vertStart = zverts.Count;
            int faceStart = ztriangles.Count;
            Matrix4x4 inverse = Matrix4x4.Identity;
            string joint_name = null;

            foreach (IOVertex v in mesh.Vertices)
            {
                Vector3 p = Vector3.Transform(v.Position, inverse);
                zverts.Add(new GXVector3() { X = p.X, Y = p.Y, Z = p.Z });
            }

            Dictionary<OpenTK.Mathematics.Vector3, int> normalToPoly = new();
            foreach (IOPolygon p in mesh.Polygons)
            {
                for (int i = 0; i < p.Indicies.Count; i += 3)
                {
                    IOVertex v3 = mesh.Vertices[p.Indicies[i + 2]];
                    IOVertex v2 = mesh.Vertices[p.Indicies[i + 1]];
                    IOVertex v1 = mesh.Vertices[p.Indicies[i]];

                    OpenTK.Mathematics.Vector3 normal = Math3D.CalculateSurfaceNormal(
                        new OpenTK.Mathematics.Vector3(v1.Position.X, v1.Position.Y, v1.Position.Z),
                        new OpenTK.Mathematics.Vector3(v2.Position.X, v2.Position.Y, v2.Position.Z),
                        new OpenTK.Mathematics.Vector3(v3.Position.X, v3.Position.Y, v3.Position.Z));

                    if (!normalToPoly.ContainsKey(normal))
                        normalToPoly.Add(normal, normalToPoly.Count);

                    ztriangles.Add(new KAR_ZoneCollisionTriangle()
                    {
                        V1 = p.Indicies[i + 2] + vertStart,
                        V2 = p.Indicies[i + 1] + vertStart,
                        V3 = p.Indicies[i + 0] + vertStart,
                        Type = 0,
                        PolyIndex = normalToPoly[normal],
                    });
                }
            }

            zjoints.Add(new KAR_ZoneCollisionJoint()
            {
                BoneID = !string.IsNullOrEmpty(joint_name) ? model.Skeleton.IndexOf(model.Skeleton.GetBoneByName(joint_name)) : 0,
                ZoneFaceStart = faceStart,
                ZoneFaceSize = ztriangles.Count - faceStart,
                ZoneVertexStart = vertStart,
                ZoneVertexSize = zverts.Count - vertStart,
                x14 = -1,
                x18 = -1,
            });

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static KAR_grCollisionNode GenerateCollisionNode(string filepath)
        {
            return new KARCollisionImporter().ImportFromModel(filepath);
        }
    }
}
