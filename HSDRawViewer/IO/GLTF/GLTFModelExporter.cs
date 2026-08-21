using HSDRaw.Common;
using IONET.Core;
using IONET.Core.Model;
using IONET.Core.Skeleton;
using SharpGLTF.Memory;
using SharpGLTF.Schema2;
using SharpGLTF.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace HSDRawViewer.IO.GLTF
{
    public class GLTFModelExporter
    {

        public static void ExportToGLTF(IOScene ioscene, string filePath, Dictionary<IOMaterial, HSD_MOBJ> materialToMobj)
        {
            var model = ModelRoot.CreateModel();

            Dictionary<string, Material> materialNodes = new();
            foreach (var s in ioscene.Materials)
            {
                var material = GTLFMaterialConverter.ToMaterial(model, s, materialToMobj[s]);
                materialNodes.TryAdd(s.Name, material);
            }

            var scene = model.UseScene("Scene");

            // ------------------------------------------------------------
            // Create skeleton
            // ------------------------------------------------------------

            foreach (var iomodel in ioscene.Models)
            {
                var boneNodes = new Dictionary<IOBone, Node>();

                if (iomodel.Skeleton != null)
                {
                    foreach (var iobone in iomodel.Skeleton.RootBones)
                        ExportSkeleton(scene, iobone, null, boneNodes);
                }

                var skeleton = iomodel.Skeleton;
                Skin skin = null;
                Dictionary<string, int> boneLookup = new Dictionary<string, int>();
                if (skeleton != null && boneNodes.Count > 0)
                {
                    skin = model.CreateSkin();
                    skin.Name = $"{iomodel.Name}_Skin";

                    var joints = new List<Node>();

                    foreach (var bone in skeleton.BreathFirstOrder())
                    {
                        if (!boneNodes.TryGetValue(bone, out var node))
                            throw new InvalidOperationException(
                                $"Bone '{bone.Name}' was not exported.");

                        boneLookup.Add(node.Name, joints.Count);
                        joints.Add(node);
                    }

                    // BindJoints calculates the inverse bind matrices.
                    skin.BindJoints(joints.ToArray());

                    // Skeleton root
                    if (skeleton.RootBones.Count > 0 &&
                        boneNodes.TryGetValue(
                            skeleton.RootBones[0],
                            out var rootNode))
                    {
                        skin.Skeleton = rootNode;
                    }
                }

                foreach (var ioMesh in iomodel.Meshes)
                {
                    ExportMesh(
                        model,
                        scene,
                        skin,
                        ioMesh,
                        boneLookup,
                        materialNodes);
                }
            }

            model.SaveGLB(filePath);
        }

        private static void ExportSkeleton(Scene scene, IOBone iobone, Node parent, Dictionary<IOBone, Node> boneNodes)
        {
            if (scene == null) return;

            Node node = (parent == null) ? scene.CreateNode() : parent.CreateNode();
            node.Name = iobone.Name;

            node.LocalTransform = new AffineTransform(
                iobone.Scale,
                iobone.Rotation,
                iobone.Translation);

            boneNodes.Add(iobone, node);

            foreach (var c in iobone.Children)
            {
                ExportSkeleton(scene, c, node, boneNodes);
            }
        }

        private static Node ExportMesh(
            ModelRoot model,
            Scene scene,
            Skin skin,
            IOMesh ioMesh,
            Dictionary<string, int> boneLookups,
            Dictionary<string, Material> materialNodes)
        {
            var mesh = model.CreateMesh(ioMesh.Name);

            // Create all vertex accessors ONCE.
            var accessors = CreateMeshAccessors(
                model,
                ioMesh,
                boneLookups);

            foreach (var polygon in ioMesh.Polygons)
            {
                var primitive = mesh.CreatePrimitive();

                if (materialNodes.ContainsKey(polygon.MaterialName))
                    primitive.WithMaterial(materialNodes[polygon.MaterialName]);

                ExportPrimitive(
                    model,
                    primitive,
                    ioMesh,
                    polygon,
                    accessors);
            }

            // ------------------------------------------------------------
            // Create the scene node that actually references the mesh
            // ------------------------------------------------------------

            var meshNode = scene.CreateNode();
            meshNode.Name = ioMesh.Name;
            meshNode.Mesh = mesh;
            meshNode.Skin = skin;

            return meshNode;
        }


        // ================================================================
        // Primitive
        // ================================================================

        private static Accessor CreateVector4Accessor(
            ModelRoot model,
            IReadOnlyList<Vector4> values)
        {
            var accessor = model.CreateAccessor();

            var buffer = model.CreateBufferView(
                values.Count * sizeof(float) * 4);

            accessor.SetVertexData(
                buffer,
                0,
                values.Count,
                AttributeFormat.Float4);

            var data = accessor.AsVector4Array();

            for (int i = 0; i < values.Count; i++)
            {
                data[i] = values[i];
            }

            accessor.UpdateBounds();

            return accessor;
        }

        private static Accessor CreateVector3Accessor(
            ModelRoot model,
            IReadOnlyList<Vector3> values)
        {
            var accessor = model.CreateAccessor();

            var buffer = model.CreateBufferView(
                values.Count * sizeof(float) * 3);

            accessor.SetVertexData(
                buffer,
                0,
                values.Count,
                AttributeFormat.Float3);

            var data = accessor.AsVector3Array();

            for (int i = 0; i < values.Count; i++)
            {
                data[i] = values[i];
            }

            accessor.UpdateBounds();

            return accessor;
        }

        private static Accessor CreateVector2Accessor(
            ModelRoot model,
            IReadOnlyList<Vector2> values)
        {
            var accessor = model.CreateAccessor();

            var buffer = model.CreateBufferView(
                values.Count * sizeof(float) * 2);

            accessor.SetVertexData(
                buffer,
                0,
                values.Count,
                AttributeFormat.Float2);

            var data = accessor.AsVector2Array();

            for (int i = 0; i < values.Count; i++)
            {
                data[i] = values[i];
            }

            accessor.UpdateBounds();

            return accessor;
        }


        public static readonly AttributeFormat UShort4 = new AttributeFormat(DimensionType.VEC4, EncodingType.UNSIGNED_SHORT);

        private static Accessor CreateJointAccessor(
            ModelRoot model,
            IReadOnlyList<Vector4> values)
        {
            var accessor = model.CreateAccessor();

            var buffer = model.CreateBufferView(
                values.Count * sizeof(ushort) * 4);

            accessor.SetVertexData(
                buffer,
                0,
                values.Count,
                UShort4);

            var data = accessor.AsVector4Array();

            for (int i = 0; i < values.Count; i++)
            {
                data[i] = values[i];
            }

            return accessor;
        }

        private static Accessor CreateIndexAccessor(
            ModelRoot model,
            IReadOnlyList<uint> indices)
        {
            var accessor = model.CreateAccessor();

            var buffer = model.CreateBufferView(
                indices.Count * sizeof(uint));

            accessor.SetIndexData(
                buffer,
                0,
                indices.Count,
                IndexEncodingType.UNSIGNED_INT);

            var data = accessor.AsIndicesArray();

            for (int i = 0; i < indices.Count; i++)
            {
                data[i] = indices[i];
            }

            return accessor;
        }

        private sealed class MeshAccessors
        {
            public Accessor Position;
            public Accessor Normal;

            public Accessor[] UV = new Accessor[8];
            public Accessor[] Color = new Accessor[2];

            public List<Accessor> JointSets = new();
            public List<Accessor> WeightSets = new();
        }
        private static Vector4 SetComponent(
            Vector4 value,
            int component,
            float componentValue)
        {
            switch (component)
            {
                case 0:
                    value.X = componentValue;
                    break;
                case 1:
                    value.Y = componentValue;
                    break;
                case 2:
                    value.Z = componentValue;
                    break;
                case 3:
                    value.W = componentValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(component));
            }

            return value;
        }
        private static MeshAccessors CreateMeshAccessors(
            ModelRoot model,
            IOMesh mesh,
            Dictionary<string, int> boneLookups)
        {
            var result = new MeshAccessors();

            // ------------------------------------------------------------
            // Position
            // ------------------------------------------------------------

            var positions = new Vector3[mesh.Vertices.Count];
            var normals = new Vector3[mesh.Vertices.Count];
            var uvs = new Vector2[8][];
            var colors = new Vector4[2][];

            for (int i = 0; i < 8; i++)
            {
                uvs[i] = new Vector2[mesh.Vertices.Count];
            }
            for (int i = 0; i < 2; i++)
            {
                colors[i] = new Vector4[mesh.Vertices.Count];
            }

            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                var v = mesh.Vertices[i];

                positions[i] = v.Position;
                normals[i] = v.Normal;

                for (int j = 0; j < Math.Min(v.UVs.Count, 8); j++)
                {
                    uvs[j][i] = v.UVs[j];
                }

                for (int j = 0; j < Math.Min(v.Colors.Count, 2); j++)
                {
                    colors[j][i] = v.Colors[j];
                }
            }

            result.Position =
                CreateVector3Accessor(model, positions);

            result.Normal =
                CreateVector3Accessor(model, normals);

            for (int i = 0; i < 8; i++)
            {
                if (mesh.Vertices.Any(v => v.UVs.Count > i))
                {
                    result.UV[i] =
                        CreateVector2Accessor(model, uvs[i]);
                }
            }

            for (int i = 0; i < 2; i++)
            {
                if (mesh.Vertices.Any(v => v.Colors.Count > i))
                {
                    result.Color[i] =
                        CreateVector4Accessor(model, colors[i]);
                }
            }

            // ------------------------------------------------------------
            // Skinning
            // ------------------------------------------------------------

            int maxWeights = mesh.Vertices
                .Max(v => v.Envelope?.Weights.Count ?? 0);

            // Always create at least one set for a skinned mesh.
            int setCount = Math.Max(
                1,
                (maxWeights + 3) / 4);

            for (int set = 0; set < setCount; set++)
            {
                var joints =
                    new Vector4[mesh.Vertices.Count];

                var weights =
                    new Vector4[mesh.Vertices.Count];

                for (int vertexIndex = 0;
                     vertexIndex < mesh.Vertices.Count;
                     vertexIndex++)
                {
                    var influences =
                        mesh.Vertices[vertexIndex]
                            .Envelope?
                            .Weights;

                    if (influences == null)
                        continue;

                    for (int component = 0; component < 4; component++)
                    {
                        int influenceIndex = set * 4 + component;

                        if (influenceIndex >= influences.Count)
                            break;

                        var influence = influences[influenceIndex];

                        if (!boneLookups.TryGetValue(influence.BoneName, out int jointIndex))
                            continue;

                        joints[vertexIndex] = SetComponent(
                            joints[vertexIndex],
                            component,
                            jointIndex);

                        weights[vertexIndex] = SetComponent(
                            weights[vertexIndex],
                            component,
                            influence.Weight);
                    }
                }

                result.JointSets.Add(
                    CreateJointAccessor(
                        model,
                        joints));

                result.WeightSets.Add(
                    CreateVector4Accessor(
                        model,
                        weights));
            }

            return result;
        }

        private static void ExportPrimitive(
            ModelRoot model,
            MeshPrimitive primitive,
            IOMesh mesh,
            IOPolygon polygon,
            MeshAccessors accessors)
        {
            // ------------------------------------------------------------
            // Shared vertex attributes
            // ------------------------------------------------------------

            primitive.SetVertexAccessor(
                "POSITION",
                accessors.Position);

            if (accessors.Normal != null)
            {
                primitive.SetVertexAccessor(
                    "NORMAL",
                    accessors.Normal);
            }

            for (int i = 0; i < accessors.UV.Length; i++)
            {
                if (accessors.UV[i] != null)
                {
                    primitive.SetVertexAccessor(
                        $"TEXCOORD_{i}",
                        accessors.UV[i]);
                }
            }

            for (int i = 0; i < accessors.Color.Length; i++)
            {
                if (accessors.Color[i] != null)
                {
                    primitive.SetVertexAccessor(
                        $"COLOR_{i}",
                        accessors.Color[i]);
                }
            }

            // ------------------------------------------------------------
            // Skinning
            // ------------------------------------------------------------

            for (int i = 0;
                 i < accessors.JointSets.Count;
                 i++)
            {
                primitive.SetVertexAccessor(
                    $"JOINTS_{i}",
                    accessors.JointSets[i]);

                primitive.SetVertexAccessor(
                    $"WEIGHTS_{i}",
                    accessors.WeightSets[i]);
            }

            // ------------------------------------------------------------
            // Indices
            // ------------------------------------------------------------
            var indices = GTLFPrimitiveFlipper.ConvertIndices(polygon);

            primitive.IndexAccessor =
                CreateIndexAccessor(
                    model,
                    indices);

            switch (polygon.PrimitiveType)
            {
                case IOPrimitive.TRIANGLE:
                case IOPrimitive.QUAD:
                case IOPrimitive.TRISTRIP:
                case IOPrimitive.TRIFAN:
                    primitive.DrawPrimitiveType =
                        PrimitiveType.TRIANGLES;
                    break;

                case IOPrimitive.POINT:
                    primitive.DrawPrimitiveType =
                        PrimitiveType.POINTS;
                    break;

                case IOPrimitive.LINE:
                    primitive.DrawPrimitiveType =
                        PrimitiveType.LINES;
                    break;

                case IOPrimitive.LINESTRIP:
                    primitive.DrawPrimitiveType =
                        PrimitiveType.LINE_STRIP;
                    break;

                default:
                    throw new NotSupportedException(
                        $"Primitive type {polygon.PrimitiveType} is not supported.");
            }
        }
    }
}
