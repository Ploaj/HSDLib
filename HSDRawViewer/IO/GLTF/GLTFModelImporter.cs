using HSDRaw.Common;
using IONET.Core;
using IONET.Core.Model;
using IONET.Core.Skeleton;
using SharpGLTF.Schema2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace HSDRawViewer.IO.GLTF
{
    public class GLTFModelImporter
    {
        public static IOScene Import(string filePath, out Dictionary<IOMaterial, HSD_MOBJ> materialOverride)
        {
            var m = ModelRoot.Load(filePath);

            var scene = new IOScene();

            materialOverride = new Dictionary<IOMaterial, HSD_MOBJ>();

            foreach (var mat in m.LogicalMaterials)
            {
                var material = GTLFMaterialConverter.ImportMaterial(mat);
                scene.Materials.Add(material.Item1);
                materialOverride.TryAdd(material.Item1, material.Item2);
            }

            var model = new IOModel();
            foreach (var node in m.DefaultScene.VisualChildren)
            {
                ParseRoot(node, model, null, "");
            }
            scene.Models.Add(model);

            return scene;
        }

        private static void AddBoneWeight(
            IOVertex vertex,
            int boneIndex,
            float weight,
            IReadOnlyList<Node> joints)
        {
            if (weight <= 0)
                return;

            if (boneIndex < 0 || boneIndex >= joints.Count)
                return;

            var bone = joints[boneIndex];

            vertex.Envelope.Weights.Add(new IOBoneWeight
            {
                BoneName = bone.Name,
                Weight = weight,
                //BindMatrix = bone.Skin.GetInverseBindMatricesAccessor().AsMatrix4x4Array()[0]
            });
        }

        private static void ParseRoot(Node node, IOModel model, IOBone parent, string offset)
        {
            var is_mesh = node.Mesh != null;

            Debug.WriteLine($"{offset}: {node.Name} {is_mesh}");

            var bone = new IOBone()
            {
                Name = node.Name,
                Translation = node.LocalTransform.Translation,
                Rotation = node.LocalTransform.Rotation,
                Scale = node.LocalTransform.Scale,
            };

            if (is_mesh)
            {
                var m = new IOMesh()
                {
                    Name = node.Name,
                    ParentBone = parent,
                };

                var worldMatrix = node.WorldMatrix;
                var normalMatrix =
                    Matrix4x4.Invert(worldMatrix, out var inverse)
                        ? Matrix4x4.Transpose(inverse)
                        : Matrix4x4.Identity;

                IReadOnlyList<Node> skin = null;
                if (node.Skin != null)
                    skin = node.Skin.Joints;

                foreach (var primitive in node.Mesh.Primitives)
                {
                    // ------------------------------------------------------------
                    // Vertex attributes
                    // ------------------------------------------------------------

                    var positionAccessor = primitive.GetVertexAccessor("POSITION");

                    if (positionAccessor == null)
                        throw new InvalidOperationException(
                            "Mesh primitive does not contain POSITION.");

                    var positions = positionAccessor.AsVector3Array();

                    // Optional attributes
                    var normalAccessor = primitive.GetVertexAccessor("NORMAL");
                    Accessor[] uvAccessor =
                    {
                        primitive.GetVertexAccessor("TEXCOORD_0"),
                        primitive.GetVertexAccessor("TEXCOORD_1"),
                        primitive.GetVertexAccessor("TEXCOORD_2"),
                        primitive.GetVertexAccessor("TEXCOORD_3"),
                        primitive.GetVertexAccessor("TEXCOORD_4"),
                        primitive.GetVertexAccessor("TEXCOORD_5"),
                        primitive.GetVertexAccessor("TEXCOORD_6"),
                        primitive.GetVertexAccessor("TEXCOORD_7"),
                    };
                    Accessor[] colorAccessor = 
                    {
                        primitive.GetVertexAccessor("COLOR_0"),
                        primitive.GetVertexAccessor("COLOR_1"),
                    };
                    var jointsAccessor = primitive.GetVertexAccessor("JOINTS_0");
                    var weightsAccessor = primitive.GetVertexAccessor("WEIGHTS_0");

                    var normals = normalAccessor?.AsVector3Array();
                    var uvs = uvAccessor.Select(e => e?.AsVector2Array()).ToArray();
                    var colors = colorAccessor.Select(e => e?.AsVector4Array()).ToArray();
                    var weights = weightsAccessor?.AsVector4Array();

                    // JOINTS_0 is an integer accessor.
                    var joints = jointsAccessor?.AsVector4Array();

                    // ------------------------------------------------------------
                    // Build vertices
                    // ------------------------------------------------------------
                    var vertexOffset = m.Vertices.Count;
                    for (int i = 0; i < positions.Count; i++)
                    {
                        var vertex = new IOVertex
                        {
                            Position = Vector3.Transform(
                                positions[i],
                                worldMatrix),

                            Normal = normals != null && i < normals.Count
                                ? Vector3.Normalize(
                                    Vector3.TransformNormal(
                                        normals[i],
                                        normalMatrix))
                                : Vector3.UnitY
                        };

                        for (int j = 0; j < 8; j++)
                        {
                            if (uvs[j] != null && i < uvs[j].Count)
                            {
                                var v = uvs[j][i];
                                vertex.SetUV(v.X, 1 - v.Y, j);
                            }
                        }

                        for (int j = 0; j < 2; j++)
                        {
                            if (colors[j] != null && i < colors[j].Count)
                            {
                                var v = colors[j][i];
                                vertex.SetColor(v.X, v.Y, v.Z, v.W, j);
                            }
                        }

                        if (skin != null &&
                            joints != null && weights != null &&
                            i < joints.Count && i < weights.Count)
                        {
                            var boneIndices = joints[i];
                            var boneWeights = weights[i];

                            AddBoneWeight(vertex, (int)boneIndices.X, boneWeights.X, skin);
                            AddBoneWeight(vertex, (int)boneIndices.Y, boneWeights.Y, skin);
                            AddBoneWeight(vertex, (int)boneIndices.Z, boneWeights.Z, skin);
                            AddBoneWeight(vertex, (int)boneIndices.W, boneWeights.W, skin);
                        }

                        m.Vertices.Add(vertex);
                    }

                    // ------------------------------------------------------------
                    // Indices
                    // ------------------------------------------------------------

                    var indexAccessor = primitive.IndexAccessor;
                    var poly = new IOPolygon()
                    {
                        MaterialName = primitive.Material.Name,
                    };
                    m.Polygons.Add(poly);

                    switch (primitive.DrawPrimitiveType)
                    {
                        case PrimitiveType.TRIANGLES:
                            poly.PrimitiveType = IOPrimitive.TRIANGLE;
                            break;
                        case PrimitiveType.TRIANGLE_STRIP:
                            poly.PrimitiveType = IOPrimitive.TRISTRIP;
                            break;
                        case PrimitiveType.TRIANGLE_FAN:
                            poly.PrimitiveType = IOPrimitive.TRIFAN;
                            break;
                        default:
                            throw new NotSupportedException($"Primitive Type {primitive.DrawPrimitiveType} not supported");
                    }

                    if (indexAccessor == null)
                    {
                        // Non-indexed primitive.
                        //
                        // glTF vertices are already arranged sequentially as
                        // triangles for TRIANGLES primitives.
                        for (int i = 0; i < positions.Count; i++)
                            poly.Indicies.Add(vertexOffset + i);
                    }
                    else
                    {
                        foreach (var index in indexAccessor.AsIndicesArray())
                        {
                            poly.Indicies.Add(vertexOffset + (int)index);
                        }
                    }
                }

                if (m.Polygons.Count > 0)
                {
                    model.Meshes.Add(m);
                }
            }
            else
            {
                if (parent != null)
                    parent.AddChild(bone);
                else
                    model.Skeleton.RootBones.Add(bone);
            }


            foreach (var c in node.VisualChildren)
            {
                ParseRoot(c, model, bone, $"{offset}\t");
            }
        }
    }
}
