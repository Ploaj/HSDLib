using HSDRaw.AirRide.Gr.Data;
using IONET.Core;
using IONET.Core.Model;
using IONET.Core.Skeleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDRawViewer.Tools.AirRide
{
    public class KARCollisionExporter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="data"></param>
        public static void ExportCollisionModel(string filepath, KAR_grData data)
        {
            // get collision joint
            var coll = data.CollisionNode;

            var _vertices = coll.Vertices;
            var _triangles = coll.Triangles;
            var _joints = coll.Joints;

            //int rough_num = _triangles.Count(e => (e.Material & 0x3) != 0);
            //foreach (var f in data.PartitionNode.Partition.CollidableTriangles)
            //{
            //    if ((_triangles[f].Material & 0x3) != 0)
            //    {
            //        Console.WriteLine("Rough Indexed: " + f);
            //    }
            //}

            // generate io model
            var iomodel = new IOModel();

            // setup skeleton
            var ioroot = new IOBone()
            {
                Name = "Root"
            };
            iomodel.Skeleton = new IOSkeleton();
            iomodel.Skeleton.RootBones.Add(ioroot);

            // setup mesh
            var iomesh = new IOMesh()
            {
                Name = $"Mesh"
            };
            iomodel.Meshes.Add(iomesh);

            // add and remap points
            foreach (var vert in _vertices)
            {
                // create vertex
                var iovert = new IOVertex()
                {
                    Position = new System.Numerics.Vector3(vert.X, vert.Y, vert.Z),
                };

                // add to mesh and remap
                iomesh.Vertices.Add(iovert);
            }

            // export joints
            int index = 0;
            foreach (var j in _joints)
            {
                var iobone = new IOBone()
                {
                    Name = $"Joint_{index}_{j.Flags.ToString("X8")}",
                    Parent = ioroot,
                    Scale = System.Numerics.Vector3.One,
                };

                // set weighting
                for (int i = j.VertexStart; i < j.VertexStart + j.VertexSize; i++)
                {
                    var iovert = iomesh.Vertices[i];

                    // add weight
                    iovert.Envelope.Weights.Add(
                        new IOBoneWeight()
                        {
                            BoneName = iobone.Name,
                            Weight = 1,
                            BindMatrix = System.Numerics.Matrix4x4.Identity,
                        });
                }

                // add triangles
                var iopoly = new IOPolygon()
                {
                    PrimitiveType = IOPrimitive.TRIANGLE,
                };
                iomesh.Polygons.Add(iopoly);
                for (int i = j.FaceStart; i < j.FaceStart + j.FaceSize; i++)
                {
                    var triangle = _triangles[i];

                    iopoly.Indicies.Add(triangle.V3);
                    iopoly.Indicies.Add(triangle.V2);
                    iopoly.Indicies.Add(triangle.V1);

                    // TODO: separate based on flags
                }

                // add mesh and root
                index++;
            }

            // export scene
            IOScene ioscene = new IOScene();
            ioscene.Models.Add(iomodel);
            IONET.IOManager.ExportScene(ioscene, filepath, new IONET.ExportSettings()
            {

            });
        }
    }
}
