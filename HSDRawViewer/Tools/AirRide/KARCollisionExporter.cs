using HSDRaw;
using HSDRaw.AirRide.Gr;
using HSDRaw.AirRide.Gr.Data;
using HSDRaw.GX;
using HSDRawViewer.Converters;
using IONET.Core;
using IONET.Core.Model;
using IONET.Core.Skeleton;
using System;

namespace HSDRawViewer.Tools.AirRide
{
    public class KARCollisionExporter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static IOSkeleton CreateDummySkeleton()
        {
            IOSkeleton ioskel = new();
            IOBone ioroot = new()
            {
                Name = "Root"
            };
            ioskel.RootBones.Add(ioroot);
            return ioskel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static IOSkeleton GetSkeletonFromGrModelDat(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) ||
                !System.IO.File.Exists(filePath))
            {
                return CreateDummySkeleton();
            }

            HSDRawFile f = new(filePath);
            KAR_grModel grmodel = f.Roots[0].Data as KAR_grModel;

            if (grmodel == null)
            {
                return CreateDummySkeleton();
            }

            ModelExporter exp = new(
                grmodel.MainModel.RootNode,
                new ModelExportSettings()
                {
                    ExportBindPose = false,
                    ExportMesh = false,
                },
                new Animation.JointMap());
            return exp.Scene.Models[0].Skeleton;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vert"></param>
        /// <param name="triangle"></param>
        /// <param name="bone"></param>
        /// <returns></returns>
        private static IOVertex CreateVertex(GXVector3 vert, KAR_CollisionTriangle triangle, IOBone bone)
        {
            // create vertex
            IOVertex iovert = new()
            {
                Position = System.Numerics.Vector3.Transform(new System.Numerics.Vector3(vert.X, vert.Y, vert.Z), bone.WorldTransform),
            };

            // add weight
            iovert.Envelope.Weights.Add(
                new IOBoneWeight()
                {
                    BoneName = bone.Name,
                    Weight = 1,
                    BindMatrix = System.Numerics.Matrix4x4.Identity,
                });


            return iovert;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vert"></param>
        /// <param name="triangle"></param>
        /// <param name="bone"></param>
        /// <returns></returns>
        private static IOVertex CreateVertex(GXVector3 vert, KAR_ZoneCollisionTriangle triangle, IOBone bone)
        {
            // create vertex
            IOVertex iovert = new()
            {
                Position = System.Numerics.Vector3.Transform(new System.Numerics.Vector3(vert.X, vert.Y, vert.Z), bone.WorldTransform),
            };

            // add weight
            //iovert.Envelope.Weights.Add(
            //    new IOBoneWeight()
            //    {
            //        BoneName = bone.Name,
            //        Weight = 1,
            //        BindMatrix = System.Numerics.Matrix4x4.Identity,
            //    });

            return iovert;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="data"></param>
        public static void ExportCollisionModel(string filepath, string modeldatpath, KAR_grData data)
        {
            // get collision joint
            KAR_grCollisionNode coll = data.CollisionNode;

            GXVector3[] _vertices = coll.Vertices;
            KAR_CollisionTriangle[] _triangles = coll.Triangles;
            KAR_CollisionJoint[] _joints = coll.Joints;

            // generate io model
            IOModel iomodel = new();

            // setup skeleton
            iomodel.Skeleton = GetSkeletonFromGrModelDat(modeldatpath);

            // export joints
            int index = 0;
            foreach (KAR_CollisionJoint j in _joints)
            {
                // get bone name
                IOBone bone = iomodel.Skeleton.GetBoneByIndex(j.BoneID);

                // create mesh group
                IOMesh iomesh = new()
                {
                    Name = $"Mesh_{index}_{bone.Name}"
                };
                iomodel.Meshes.Add(iomesh);

                // add triangles
                IOPolygon iopoly_wall = new()
                {
                    PrimitiveType = IOPrimitive.TRIANGLE,
                    MaterialName = "wall"
                };

                IOPolygon iopoly_ceil = new()
                {
                    PrimitiveType = IOPrimitive.TRIANGLE,
                    MaterialName = "ceil"
                };

                IOPolygon iopoly_floor = new()
                {
                    PrimitiveType = IOPrimitive.TRIANGLE,
                    MaterialName = "floor"
                };

                IOPolygon iopoly_unknown = new()
                {
                    PrimitiveType = IOPrimitive.TRIANGLE,
                    MaterialName = "Unknown"
                };


                for (int i = j.FaceStart; i < j.FaceStart + j.FaceSize; i++)
                {
                    KAR_CollisionTriangle triangle = _triangles[i];

                    IOPolygon iopoly;

                    switch (triangle.Flags)
                    {
                        case KCCollFlag.Wall:
                            iopoly = iopoly_wall;
                            break;
                        case KCCollFlag.Ceiling:
                            iopoly = iopoly_ceil;
                            break;
                        case KCCollFlag.Floor:
                            iopoly = iopoly_floor;
                            break;
                        default:
                            iopoly = iopoly_unknown;
                            break;
                    }

                    if (triangle.V3 < j.VertexStart ||
                        triangle.V3 >= j.VertexStart + j.VertexSize ||
                        triangle.V2 < j.VertexStart ||
                        triangle.V2 >= j.VertexStart + j.VertexSize ||
                        triangle.V1 < j.VertexStart ||
                        triangle.V1 >= j.VertexStart + j.VertexSize)
                    {
                        throw new Exception("Vertex out of range");
                    }

                    iopoly.Indicies.Add(iomesh.Vertices.Count);
                    iomesh.Vertices.Add(CreateVertex(_vertices[triangle.V3], triangle, bone));

                    iopoly.Indicies.Add(iomesh.Vertices.Count);
                    iomesh.Vertices.Add(CreateVertex(_vertices[triangle.V2], triangle, bone));

                    iopoly.Indicies.Add(iomesh.Vertices.Count);
                    iomesh.Vertices.Add(CreateVertex(_vertices[triangle.V1], triangle, bone));
                }

                if (iopoly_wall.Indicies.Count > 0)
                    iomesh.Polygons.Add(iopoly_wall);

                if (iopoly_ceil.Indicies.Count > 0)
                    iomesh.Polygons.Add(iopoly_ceil);

                if (iopoly_floor.Indicies.Count > 0)
                    iomesh.Polygons.Add(iopoly_floor);

                if (iopoly_unknown.Indicies.Count > 0)
                    iomesh.Polygons.Add(iopoly_unknown);

                // add mesh and root
                index++;
            }

            // export scene
            IOScene ioscene = new();
            ioscene.Materials.Add(new IOMaterial()
            {
                Name = "wall",
                DiffuseColor = new System.Numerics.Vector4(1, 0, 0, 1),
            });
            ioscene.Materials.Add(new IOMaterial()
            {
                Name = "ceiling",
                DiffuseColor = new System.Numerics.Vector4(0, 1, 0, 1),
            });
            ioscene.Materials.Add(new IOMaterial()
            {
                Name = "floor",
                DiffuseColor = new System.Numerics.Vector4(0, 0, 1, 1),
            });
            ioscene.Materials.Add(new IOMaterial()
            {
                Name = "Unknown",
                DiffuseColor = new System.Numerics.Vector4(1, 1, 0, 1),
            });

            ioscene.Models.Add(iomodel);
            IONET.IOManager.ExportScene(ioscene, filepath, new IONET.ExportSettings()
            {

            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="data"></param>
        public static void ExportZoneModel(string filepath, string modeldatpath, KAR_grData data)
        {
            // get collision joint
            KAR_grCollisionNode coll = data.CollisionNode;

            // generate io model
            IOModel iomodel = new();

            // setup skeleton
            iomodel.Skeleton = GetSkeletonFromGrModelDat(modeldatpath);

            GXVector3[] _zonevertices = coll.ZoneVertices;
            KAR_ZoneCollisionTriangle[] _zonetriangles = coll.ZoneTriangles;
            KAR_ZoneCollisionJoint[] _zonejoints = coll.ZoneJoints;
            int index = 0;
            foreach (KAR_ZoneCollisionJoint j in _zonejoints)
            {
                // get bone name
                IOBone bone = iomodel.Skeleton.GetBoneByIndex(j.BoneID);

                // create mesh group
                IOMesh iomesh = new()
                {
                    Name = $"Zone_{index}_{bone.Name}_{_zonetriangles[j.ZoneFaceStart].Type}"
                };
                iomodel.Meshes.Add(iomesh);

                // add triangles
                IOPolygon iopoly_wall = new()
                {
                    PrimitiveType = IOPrimitive.TRIANGLE,
                    MaterialName = "wall"
                };

                IOPolygon iopoly_ceil = new()
                {
                    PrimitiveType = IOPrimitive.TRIANGLE,
                    MaterialName = "ceil"
                };

                IOPolygon iopoly_floor = new()
                {
                    PrimitiveType = IOPrimitive.TRIANGLE,
                    MaterialName = "floor"
                };

                IOPolygon iopoly_unknown = new()
                {
                    PrimitiveType = IOPrimitive.TRIANGLE,
                    MaterialName = "Unknown"
                };

                for (int i = j.ZoneFaceStart; i < j.ZoneFaceStart + j.ZoneFaceSize; i++)
                {
                    KAR_ZoneCollisionTriangle triangle = _zonetriangles[i];

                    IOPolygon iopoly;

                    switch (triangle.CollFlags)
                    {
                        case KCCollFlag.Wall:
                            iopoly = iopoly_wall;
                            break;
                        case KCCollFlag.Ceiling:
                            iopoly = iopoly_ceil;
                            break;
                        case KCCollFlag.Floor:
                            iopoly = iopoly_floor;
                            break;
                        default:
                            iopoly = iopoly_unknown;
                            break;
                    }

                    if (triangle.V3 < j.ZoneVertexStart ||
                        triangle.V3 >= j.ZoneVertexStart + j.ZoneVertexSize ||
                        triangle.V2 < j.ZoneVertexStart ||
                        triangle.V2 >= j.ZoneVertexStart + j.ZoneVertexSize ||
                        triangle.V1 < j.ZoneVertexStart ||
                        triangle.V1 >= j.ZoneVertexStart + j.ZoneVertexSize)
                    {
                        throw new Exception("Vertex out of range");
                    }

                    iopoly.Indicies.Add(iomesh.Vertices.Count);
                    iomesh.Vertices.Add(CreateVertex(_zonevertices[triangle.V3], triangle, bone));

                    iopoly.Indicies.Add(iomesh.Vertices.Count);
                    iomesh.Vertices.Add(CreateVertex(_zonevertices[triangle.V2], triangle, bone));

                    iopoly.Indicies.Add(iomesh.Vertices.Count);
                    iomesh.Vertices.Add(CreateVertex(_zonevertices[triangle.V1], triangle, bone));
                }

                if (iopoly_wall.Indicies.Count > 0)
                    iomesh.Polygons.Add(iopoly_wall);

                if (iopoly_ceil.Indicies.Count > 0)
                    iomesh.Polygons.Add(iopoly_ceil);

                if (iopoly_floor.Indicies.Count > 0)
                    iomesh.Polygons.Add(iopoly_floor);

                if (iopoly_unknown.Indicies.Count > 0)
                    iomesh.Polygons.Add(iopoly_unknown);

                // add mesh and root
                index++;
            }


            // export scene
            IOScene ioscene = new();
            ioscene.Materials.Add(new IOMaterial()
            {
                Name = "wall",
                DiffuseColor = new System.Numerics.Vector4(1, 0, 0, 1),
            });
            ioscene.Materials.Add(new IOMaterial()
            {
                Name = "ceiling",
                DiffuseColor = new System.Numerics.Vector4(0, 1, 0, 1),
            });
            ioscene.Materials.Add(new IOMaterial()
            {
                Name = "floor",
                DiffuseColor = new System.Numerics.Vector4(0, 0, 1, 1),
            });
            ioscene.Materials.Add(new IOMaterial()
            {
                Name = "Unknown",
                DiffuseColor = new System.Numerics.Vector4(1, 1, 0, 1),
            });

            ioscene.Models.Add(iomodel);
            IONET.IOManager.ExportScene(ioscene, filepath, new IONET.ExportSettings()
            {

            });
        }
    }
}
