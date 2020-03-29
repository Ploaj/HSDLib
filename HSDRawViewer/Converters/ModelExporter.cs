using System;
using System.Collections.Generic;
using System.Text;
using Assimp;
using HSDRaw.Common;
using HSDRawViewer.GUI;
using OpenTK;
using HSDRawViewer.Rendering;
using HSDRaw.GX;
using System.Drawing;
using HSDRaw;

namespace HSDRawViewer.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public class ModelExportSettings
    {
        public bool Optimize { get; set; } = true;

        public bool FlipUVs { get; set; } = false;

        public bool ExportMOBJs { get; set; } = false;

        public bool ExportTransformedUVs { get => ModelExporter.TransformUVS; set => ModelExporter.TransformUVS = value; }

        public bool ExportScaledUVs { get => ModelExporter.ScaleUVs; set => ModelExporter.ScaleUVs = value; }

        public string Directory;
    }

    /// <summary>
    /// 
    /// </summary>
    public class ModelExporter
    {
        public static readonly string[] SupportedFormats = { ".dae" };

        public static bool TransformUVS { get; set; }
        public static bool ScaleUVs { get; set; }

        public static void ExportFile(HSD_JOBJ rootJOBJ, Dictionary<int, string> boneLabels = null)
        {
            StringBuilder sup = new StringBuilder();

            AssimpContext importer = new AssimpContext();
            var length = importer.GetSupportedExportFormats().Length;
            var index = 0;
            foreach (var v in importer.GetSupportedExportFormats())
            {
                sup.Append($"{v.Description} (*.{v.FileExtension})|*.{v.FileExtension};");
                index++;
                if (index != length)
                    sup.Append("|");
            }

            var f = Tools.FileIO.SaveFile(sup.ToString());

            if (f != null)
            {
                var settings = new ModelExportSettings();
                using (PropertyDialog d = new PropertyDialog("Model Import Options", settings))
                {
                    if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        ExportFile(f, rootJOBJ, settings, boneLabels);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="rootJOBJ"></param>
        public static void ExportFile(string filePath, HSD_JOBJ rootJOBJ, ModelExportSettings settings = null, Dictionary<int, string> boneLabels = null)
        {
            ModelExporter mex = new ModelExporter();
            AssimpContext importer = new AssimpContext();

            Dictionary<string, string> extToId = new Dictionary<string, string>();

            foreach (var v in importer.GetSupportedExportFormats())
                if(!extToId.ContainsKey("." + v.FileExtension))
                    extToId.Add("." + v.FileExtension, v.FormatId);

            PostProcessSteps postProcess = PostProcessSteps.FlipWindingOrder;

            if (settings.Optimize)
                postProcess |= PostProcessSteps.JoinIdenticalVertices;

            if (settings.FlipUVs)
                postProcess |= PostProcessSteps.FlipUVs;

            settings.Directory = System.IO.Path.GetDirectoryName(filePath) + "\\";

            Dictionary<Node, HSD_JOBJ> nodeToJOBJ = new Dictionary<Node, HSD_JOBJ>();
            if (System.IO.Path.GetExtension(filePath).ToLower() == ".dae")
            {
                var sc = mex.WriteRootNode(rootJOBJ, settings, boneLabels, nodeToJOBJ);
                /*var scn = Scene.ToUnmanagedScene(sc);
                scn = AssimpLibrary.Instance.ApplyPostProcessing(scn, postProcess);
                var scene = Scene.FromUnmanagedScene(scn);
                Scene.FreeUnmanagedScene(scn);*/
                ExportCustomDAE(filePath, sc, settings, nodeToJOBJ);
            }
            else
                importer.ExportFile(mex.WriteRootNode(rootJOBJ, settings, boneLabels, nodeToJOBJ), filePath, extToId[System.IO.Path.GetExtension(filePath)], postProcess);

            importer.Dispose();
        }

        /// <summary>
        /// Converts JOBJ to Assimp Scene
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static Scene JOBJtoScene(HSD_JOBJ root)
        {
            Dictionary<Node, HSD_JOBJ> nodeToJOBJ = new Dictionary<Node, HSD_JOBJ>();
            ModelExporter mex = new ModelExporter();
            return mex.WriteRootNode(root, new ModelExportSettings(), new Dictionary<int, string>(), nodeToJOBJ);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="scene"></param>
        /// <param name="settings"></param>
        /// <param name="nodeToJOBJ"></param>
        private static void ExportCustomDAE(string filePath, Scene scene, ModelExportSettings settings, Dictionary<Node, HSD_JOBJ> nodeToJOBJ)
        {
            using (DAEWriter writer = new DAEWriter(filePath, settings.Optimize))
            {
                var path = System.IO.Path.GetDirectoryName(filePath) + '\\';

                writer.WriteAsset();

                //DialogResult dialogResult = MessageBox.Show("Export Materials and Textures?", "DAE Exporter", MessageBoxButtons.YesNo);
                //if (dialogResult == DialogResult.Yes)
                {
                    List<string> TextureNames = new List<string>();

                    foreach (var tex in scene.Materials)
                    {
                        if(!TextureNames.Contains(tex.TextureDiffuse.FilePath))
                            TextureNames.Add(tex.TextureDiffuse.FilePath);
                    }

                    writer.WriteLibraryImages(TextureNames.ToArray(), ".png");

                    writer.StartMaterialSection();
                    foreach (var mat in scene.Materials)
                    {
                        writer.WriteMaterial(mat.Name);
                    }
                    writer.EndMaterialSection();

                    writer.StartEffectSection();
                    foreach (var mat in scene.Materials)
                    {
                        writer.WriteEffect(mat.Name, mat.TextureDiffuse.FilePath);
                    }
                    writer.EndEffectSection();

                }
                //else
                //    writer.WriteLibraryImages();

                var rootSkeleton = scene.RootNode.Children[0];

                RecursivlyWriteDAEJoints(writer, rootSkeleton, "", Matrix4.Identity, nodeToJOBJ);

                writer.StartGeometrySection();
                for(int i = 1; i < scene.RootNode.Children.Count; i++)
                {
                    var node = scene.RootNode.Children[i];

                    writer.StartGeometryMesh(node.Name);
                    
                    List<uint> triangles = new List<uint>();
                    List<float> vertices = new List<float>();
                    List<float> nrm = new List<float>();
                    List<float> uv0 = new List<float>();
                    List<float> uv1 = new List<float>();
                    List<float> clr0 = new List<float>();
                    List<float> clr1 = new List<float>();

                    List<List<int>> BoneIndicies = new List<List<int>>();
                    List<List<float>> BoneWeights = new List<List<float>>();

                    foreach (var meshIndex in node.MeshIndices)
                    {
                        var vertexOffset = vertices.Count / 3;

                        var mesh = scene.Meshes[meshIndex];

                        if (!mesh.HasFaces || !mesh.HasVertices)
                            continue;
                        
                        if(scene.Materials.Count != 0)
                            writer.CurrentMaterial = scene.Materials[mesh.MaterialIndex].Name;

                        foreach (var face in mesh.Faces)
                        {
                            for (var k = 0; k < 3; k++)
                            {
                                triangles.Add((uint)(vertexOffset + face.Indices[2-k]));
                            }
                        }

                        if (mesh.HasBones)
                        {
                            for (int v = 0; v < mesh.VertexCount; v++)
                            {
                                BoneIndicies.Add(new List<int>());
                                BoneWeights.Add(new List<float>());
                            }

                            foreach (var bone in mesh.Bones)
                            {
                                if (bone.HasVertexWeights)
                                {
                                    foreach(var w in bone.VertexWeights)
                                    {
                                        BoneIndicies[w.VertexID + vertexOffset].Add(writer.GetJointIndex(bone.Name));
                                        BoneWeights[w.VertexID + vertexOffset].Add(w.Weight);
                                    }
                                }
                            }
                        }

                        vertices.AddRange(ToFloatArray(mesh.Vertices));

                        if (mesh.HasNormals)
                            nrm.AddRange(ToFloatArray(mesh.Normals));

                        if (mesh.HasTextureCoords(0))
                            uv0.AddRange(ToUVFloatArray(mesh.TextureCoordinateChannels[0], settings.FlipUVs));

                        if (mesh.HasTextureCoords(1))
                            uv1.AddRange(ToUVFloatArray(mesh.TextureCoordinateChannels[1], settings.FlipUVs));

                        if (mesh.HasVertexColors(0))
                            clr0.AddRange(ToFloatArray(mesh.VertexColorChannels[0]));

                        if (mesh.HasVertexColors(1))
                            clr0.AddRange(ToFloatArray(mesh.VertexColorChannels[1]));

                    }

                    // invert normals
                    for (int n = 0; n < nrm.Count; n++)
                        nrm[n] *= -1;
                    
                    var triArr = triangles.ToArray();
                    
                    writer.WriteGeometrySource(node.Name, DAEWriter.VERTEX_SEMANTIC.POSITION, vertices.ToArray(), triArr);

                    if (nrm.Count > 0)
                        writer.WriteGeometrySource(node.Name, DAEWriter.VERTEX_SEMANTIC.NORMAL, nrm.ToArray(), triArr);

                    if (uv0.Count > 0)
                        writer.WriteGeometrySource(node.Name, DAEWriter.VERTEX_SEMANTIC.TEXCOORD, uv0.ToArray(), triArr, 0);

                    if (uv1.Count > 0)
                        writer.WriteGeometrySource(node.Name, DAEWriter.VERTEX_SEMANTIC.TEXCOORD, uv1.ToArray(), triArr, 1);

                    if (clr0.Count > 0)
                        writer.WriteGeometrySource(node.Name, DAEWriter.VERTEX_SEMANTIC.COLOR, clr0.ToArray(), triArr, 0);

                    if (clr1.Count > 0)
                        writer.WriteGeometrySource(node.Name, DAEWriter.VERTEX_SEMANTIC.COLOR, clr0.ToArray(), triArr, 1);

                    writer.AttachGeometryController(BoneIndicies, BoneWeights);

                    writer.EndGeometryMesh();

                }
                writer.EndGeometrySection();
            }
        }

        private static float[] ToUVFloatArray(List<Vector3D> list, bool flip)
        {
            float[] f = new float[list.Count * 2];

            var i = 0;
            foreach (var v in list)
            {
                f[i++] = v.X;
                f[i++] = flip ? 1 - v.Y : v.Y;
            }

            return f;
        }

        private static float[] ToFloatArray(List<Color4D> list)
        {
            float[] f = new float[list.Count * 4];

            var i = 0;
            foreach (var v in list)
            {
                f[i++] = v.R;
                f[i++] = v.G;
                f[i++] = v.B;
                f[i++] = v.A;
            }

            return f;
        }

        private static float[] ToFloatArray(List<Vector3D> list)
        {
            float[] f = new float[list.Count * 3];

            var i = 0;
            foreach (var v in list)
            {
                f[i++] = v.X;
                f[i++] = v.Y;
                f[i++] = v.Z;
            }

            return f;
        }

        private static void RecursivlyWriteDAEJoints(DAEWriter writer, Node joint, string parentName, Matrix4 parentTransform, Dictionary<Node, HSD_JOBJ> nodeToJOBJ)
        {
            var transform = ModelImporter.FromMatrix(joint.Transform);
            float[] Transform = new float[] { transform.M11, transform.M21, transform.M31, transform.M41,
                    transform.M12, transform.M22, transform.M32, transform.M42,
                    transform.M13, transform.M23, transform.M33, transform.M43,
                    transform.M14, transform.M24, transform.M34, transform.M44 };

            Matrix4 InvWorldTransform = (transform * parentTransform).Inverted();
            float[] InvTransform = new float[] { InvWorldTransform.M11, InvWorldTransform.M21, InvWorldTransform.M31, InvWorldTransform.M41,
                    InvWorldTransform.M12, InvWorldTransform.M22, InvWorldTransform.M32, InvWorldTransform.M42,
                    InvWorldTransform.M13, InvWorldTransform.M23, InvWorldTransform.M33, InvWorldTransform.M43,
                    InvWorldTransform.M14, InvWorldTransform.M24, InvWorldTransform.M34, InvWorldTransform.M44 };

            var jobj = nodeToJOBJ[joint];
            writer.AddJoint(joint.Name, parentName, Transform, InvTransform, 
                new float[] {
                jobj.TX, jobj.TY, jobj.TZ,
                jobj.RX, jobj.RY, jobj.RZ,
                jobj.SX, jobj.SY, jobj.SZ,
                });

            foreach (var child in joint.Children)
                RecursivlyWriteDAEJoints(writer, child, joint.Name, transform * parentTransform, nodeToJOBJ);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private static Matrix4x4 Matrix4ToMatrix4x4(Matrix4 m)
        {
            return new Matrix4x4()
            {
                A1 = m.M11,
                A2 = m.M12,
                A3 = m.M13,
                A4 = m.M14,
                B1 = m.M21,
                B2 = m.M22,
                B3 = m.M23,
                B4 = m.M24,
                C1 = m.M31,
                C2 = m.M32,
                C3 = m.M33,
                C4 = m.M34,
                D1 = m.M41,
                D2 = m.M42,
                D3 = m.M43,
                D4 = m.M44,
            };
        }

        private Dictionary<HSD_JOBJ, int> jobjToIndex = new Dictionary<HSD_JOBJ, int>();
        private List<HSD_JOBJ> Jobjs = new List<HSD_JOBJ>();
        private List<Matrix4> WorldTransforms = new List<Matrix4>();
        private List<Node> JobjNodes = new List<Node>();
        private Scene Scene = new Scene();
        private Node RootNode = new Node() { Name = "Root" };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private Scene WriteRootNode(HSD_JOBJ root, ModelExportSettings settings, Dictionary<int, string> boneLabels, Dictionary<Node, HSD_JOBJ> nodeToJOBJ)
        {
            Scene.RootNode = RootNode;

            RecursiveExport(root, RootNode, Matrix4.Identity, boneLabels, nodeToJOBJ);

            WriteDOBJNodes(settings);

            return Scene;
        }

        /// <summary>
        /// 
        /// </summary>
        private void RecursiveExport(HSD_JOBJ jobj, Node parent, Matrix4 parentTransform, Dictionary<int, string> indexToName, Dictionary<Node, HSD_JOBJ> nodeToJOBJ)
        {
            Node root = new Node();
            nodeToJOBJ.Add(root, jobj);
            root.Name = "JOBJ_" + Jobjs.Count;
            if (indexToName.ContainsKey(Jobjs.Count))
                root.Name = indexToName[Jobjs.Count];
            else
            if (!string.IsNullOrEmpty(jobj.ClassName))
                root.Name = jobj.ClassName;
            jobjToIndex.Add(jobj, Jobjs.Count);
            
            Matrix4 Transform = Matrix4.CreateScale(jobj.SX, jobj.SY, jobj.SZ) *
                Matrix4.CreateFromQuaternion(Math3D.FromEulerAngles(jobj.RZ, jobj.RY, jobj.RX)) *
                Matrix4.CreateTranslation(jobj.TX, jobj.TY, jobj.TZ);

            var worldTransform = Transform * parentTransform;
            
            JobjNodes.Add(root);
            Jobjs.Add(jobj);
            WorldTransforms.Add(worldTransform);

            root.Transform = Matrix4ToMatrix4x4(Transform);

            parent.Children.Add(root);

            foreach (var c in jobj.Children)
            {
                RecursiveExport(c, root, worldTransform, indexToName, nodeToJOBJ);
            }
        }

        Dictionary<byte[], string> imageToName = new Dictionary<byte[], string>();

        /// <summary>
        /// 
        /// </summary>
        private void WriteDOBJNodes(ModelExportSettings settings)
        {
            var jIndex = 0;
            foreach(var j in Jobjs)
            {
                var dIndex = 0;
                if(j.Dobj != null)
                {
                    foreach(var dobj in j.Dobj.List)
                    {
                        Node dobjNode = new Node();
                        dobjNode.Name = $"JOBJ_{jIndex}_DOBJ_{dIndex}";

                        if (dobj.Pobj != null)
                        {
                            var pindex = 0;
                            foreach (var pobj in dobj.Pobj.List)
                            {
                                dobjNode.MeshIndices.Add(Scene.Meshes.Count);
                                var mesh = ProcessPOBJ(pobj, j, pobj.SingleBoundJOBJ, dobj.Mobj);
                                mesh.Name = dobjNode.Name + "_POBJ_" + pindex++;
                                mesh.MaterialIndex = Scene.MaterialCount;
                                Scene.Meshes.Add(mesh);
                            }
                        }

                        // process and export textures
                        
                        Material m = new Material();
                        m.Name = $"JOBJ_{jIndex}_DOBJ_{dIndex}_MOBJ_{dIndex}";
                        m.Shininess = dobj.Mobj.Material.Shininess;
                        m.Opacity = dobj.Mobj.Material.Alpha;

                        if (dobj.Mobj.Textures != null)
                        {
                            foreach(var t in dobj.Mobj.Textures.List)
                            {
                                if(t.ImageData != null && t.ImageData.ImageData != null && !imageToName.ContainsKey(t.ImageData.ImageData))
                                {
                                    var name = $"TOBJ_{imageToName.Count}";
                                    using (Bitmap img = TOBJConverter.ToBitmap(t))
                                        img.Save(settings.Directory + name + ".png");
                                    imageToName.Add(t.ImageData.ImageData, name);

                                    if (settings.ExportMOBJs)
                                    {
                                        HSDRawFile mobjFile = new HSDRawFile();
                                        mobjFile.Roots.Add(new HSDRootNode()
                                        {
                                            Name = name,
                                            Data = dobj.Mobj
                                        });
                                        mobjFile.Save(settings.Directory + name + ".mobj");
                                    }
                                }
                            }

                            var dif = new TextureSlot();
                            dif.TextureType = TextureType.Diffuse;
                            dif.UVIndex = 0;
                            dif.FilePath = imageToName[dobj.Mobj.Textures.ImageData.ImageData];
                            m.TextureDiffuse = dif;
                        }
                        Scene.Materials.Add(m);

                        RootNode.Children.Add(dobjNode);
                        dIndex++;
                    }
                }
                jIndex++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pobj"></param>
        private Mesh ProcessPOBJ(HSD_POBJ pobj, HSD_JOBJ parent, HSD_JOBJ singleBind, HSD_MOBJ mobj)
        {
            Mesh m = new Mesh();
            m.Name = "pobj";
            m.PrimitiveType = PrimitiveType.Triangle;

            m.MaterialIndex = 0;

            var dl = pobj.ToDisplayList();
            var envelopes = pobj.EnvelopeWeights;

            var parentTransform = Matrix4.Identity;
            if(parent != null)
                parentTransform = WorldTransforms[jobjToIndex[parent]];
            
            var singleBindTransform = Matrix4.Identity;
            if (singleBind != null)
                singleBindTransform = WorldTransforms[jobjToIndex[singleBind]];

            Dictionary<HSD_JOBJ, Bone> jobjToBone = new Dictionary<HSD_JOBJ, Bone>();

            //if (envelopes != null)
            {
                foreach(var jobj in Jobjs)
                {
                    var bone = new Bone();
                    bone.Name = JobjNodes[jobjToIndex[jobj]].Name;
                    bone.OffsetMatrix = Matrix4ToMatrix4x4(WorldTransforms[jobjToIndex[jobj]].Inverted());
                    m.Bones.Add(bone);
                    jobjToBone.Add(jobj, bone);
                }
            }
                /*foreach (var en in envelopes)
                {
                    foreach (var jobj in en.JOBJs)
                    {
                        if (!jobjToBone.ContainsKey(jobj))
                        {
                            var bone = new Bone();
                            bone.Name = JobjNodes[jobjToIndex[jobj]].Name;
                            bone.OffsetMatrix = Matrix4ToMatrix4x4(WorldTransforms[jobjToIndex[jobj]].Inverted());
                            m.Bones.Add(bone);
                            jobjToBone.Add(jobj, bone);
                        }
                    }
                }*/


            if (singleBind != null && !jobjToBone.ContainsKey(singleBind))
            {
                var bone = new Bone();
                bone.Name = JobjNodes[jobjToIndex[singleBind]].Name;
                bone.OffsetMatrix = Matrix4ToMatrix4x4(WorldTransforms[jobjToIndex[singleBind]].Inverted());
                m.Bones.Add(bone);
                jobjToBone.Add(singleBind, bone);
            }

            int offset = 0;
            var vIndex = -1;
            foreach (var prim in dl.Primitives)
            {
                var verts = dl.Vertices.GetRange(offset, prim.Count);
                offset += prim.Count;

                switch (prim.PrimitiveType)
                {
                    case GXPrimitiveType.Quads:
                        verts = TriangleConverter.QuadToList(verts);
                        break;
                    case GXPrimitiveType.TriangleStrip:
                        verts = TriangleConverter.StripToList(verts);
                        break;
                    case GXPrimitiveType.Triangles:
                        break;
                    default:
                        Console.WriteLine(prim.PrimitiveType);
                        break;
                }

                for (int i = m.VertexCount; i < m.VertexCount + verts.Count; i += 3)
                {
                    var f = new Face();
                    f.Indices.Add(i);
                    f.Indices.Add(i + 1);
                    f.Indices.Add(i + 2);
                    m.Faces.Add(f);
                }

                foreach (var v in verts)
                {
                    vIndex++;
                    if (parent != null && jobjToIndex[parent] != 0)
                    {
                        var vertexWeight = new VertexWeight();
                        vertexWeight.VertexID = vIndex;
                        vertexWeight.Weight = 1;
                        jobjToBone[parent].VertexWeights.Add(vertexWeight);
                    }
                    if (singleBind != null)
                    {
                        var vertexWeight = new VertexWeight();
                        vertexWeight.VertexID = vIndex;
                        vertexWeight.Weight = 1;
                        jobjToBone[singleBind].VertexWeights.Add(vertexWeight);
                    }
                    Matrix4 weight = Matrix4.Identity;
                    foreach (var a in pobj.Attributes)
                    {
                        switch (a.AttributeName)
                        {
                            case GXAttribName.GX_VA_PNMTXIDX:

                                var en = envelopes[v.PNMTXIDX / 3];
                                for (int w = 0; w < en.EnvelopeCount; w++)
                                {
                                    var vertexWeight = new VertexWeight();
                                    vertexWeight.VertexID = vIndex;
                                    vertexWeight.Weight = en.Weights[w];
                                    jobjToBone[en.JOBJs[w]].VertexWeights.Add(vertexWeight);
                                }

                                if (en.EnvelopeCount == 1) 
                                    weight = WorldTransforms[jobjToIndex[en.JOBJs[0]]];

                                break;
                            case GXAttribName.GX_VA_POS:
                                var vert = Vector3.TransformPosition(GXTranslator.toVector3(v.POS), pobj.Flags.HasFlag(POBJ_FLAG.PARENTTRANSFORM) ? parentTransform : Matrix4.Identity);
                                vert = Vector3.TransformPosition(vert, weight);
                                vert = Vector3.TransformPosition(vert, singleBindTransform);
                                m.Vertices.Add(new Vector3D(vert.X, vert.Y, vert.Z));
                                break;
                            case GXAttribName.GX_VA_NRM:
                                var nrm = Vector3.TransformNormal(GXTranslator.toVector3(v.NRM), parentTransform);
                                nrm = Vector3.TransformNormal(nrm, weight);
                                nrm = Vector3.TransformNormal(nrm, singleBindTransform);
                                m.Normals.Add(new Vector3D(nrm.X, nrm.Y, nrm.Z));
                                break;
                            case GXAttribName.GX_VA_CLR0:
                                m.VertexColorChannels[0].Add(new Color4D(v.CLR0.R, v.CLR0.G, v.CLR0.B, v.CLR0.A));
                                break;
                            case GXAttribName.GX_VA_CLR1:
                                m.VertexColorChannels[1].Add(new Color4D(v.CLR1.R, v.CLR1.G, v.CLR1.B, v.CLR1.A));
                                break;
                            case GXAttribName.GX_VA_TEX0:
                                m.TextureCoordinateChannels[0].Add(ProcessUVTransform(v.TEX0, mobj, 0));
                                break;
                            case GXAttribName.GX_VA_TEX1:
                                m.TextureCoordinateChannels[1].Add(ProcessUVTransform(v.TEX1, mobj, 1));
                                break;
                            case GXAttribName.GX_VA_TEX2:
                                m.TextureCoordinateChannels[2].Add(ProcessUVTransform(v.TEX2, mobj, 2));
                                break;
                            case GXAttribName.GX_VA_TEX3:
                                m.TextureCoordinateChannels[3].Add(ProcessUVTransform(v.TEX3, mobj, 3));
                                break;
                            case GXAttribName.GX_VA_TEX4:
                                m.TextureCoordinateChannels[4].Add(ProcessUVTransform(v.TEX4, mobj, 4));
                                break;
                            case GXAttribName.GX_VA_TEX5:
                                m.TextureCoordinateChannels[5].Add(ProcessUVTransform(v.TEX5, mobj, 5));
                                break;
                            case GXAttribName.GX_VA_TEX6:
                                m.TextureCoordinateChannels[6].Add(ProcessUVTransform(v.TEX6, mobj, 6));
                                break;
                            case GXAttribName.GX_VA_TEX7:
                                m.TextureCoordinateChannels[7].Add(ProcessUVTransform(v.TEX7, mobj, 7));
                                break;
                        }
                    }
                }
            }

            return m;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static Vector3D ProcessUVTransform(GXVector2 gvVec, HSD_MOBJ mobj, int texIndex)
        {
            if (mobj == null)
                return new Vector3D(gvVec.X, gvVec.Y, 1);

            var tex = mobj.Textures.List[texIndex];

            var uv = new Vector3(gvVec.X, gvVec.Y, 0);

            if (TransformUVS)
            {
                var transform = Matrix4.CreateScale(tex.SX, tex.SY, tex.SZ) *
                    Matrix4.CreateFromQuaternion(Math3D.FromEulerAngles(tex.RZ, tex.RY, tex.RX)) *
                    Matrix4.CreateTranslation(tex.TX, tex.TY, tex.TZ);

                transform.Invert();

                uv = Vector3.TransformPosition(uv, transform);
            }

            if (ScaleUVs)
            {

                var scale = new Vector2(tex.WScale, tex.HScale);
                uv.Xy *= scale;
            }

            return new Vector3D(uv.X, uv.Y, 1);
        }

    }
}
