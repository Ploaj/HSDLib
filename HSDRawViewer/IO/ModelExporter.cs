using HSDRaw;
using HSDRaw.Common;
using HSDRaw.GX;
using HSDRaw.Tools;
using HSDRawViewer.Extensions;
using HSDRawViewer.GUI.Dialog;
using HSDRawViewer.IO.Model;
using HSDRawViewer.Rendering;
using HSDRawViewer.Tools;
using HSDRawViewer.Tools.Animation;
using IONET;
using IONET.Core;
using IONET.Core.Model;
using IONET.Core.Skeleton;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HSDRawViewer.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public class ModelExportSettings
    {
        public string Directory;

        public bool Optimize { get; set; } = true;

        public bool FlipUVs { get; set; } = true;

        public bool ExportBindPose { get; set; } = true;

        public bool ExportMesh { get; set; } = true;

        public bool ExportTextures { get; set; } = true;

        public bool ExportMOBJs { get; set; } = false;

        public bool ExportTransformedUVs { get => ModelExporter.TransformUVS; set => ModelExporter.TransformUVS = value; }

        public bool ExportScaledUVs { get => ModelExporter.ScaleUVs; set => ModelExporter.ScaleUVs = value; }

        public bool ExportTextureInfo { get; set; } = true;

        public bool ExportModelInfoSheet { get; set; } = false;

        //public bool BlenderExportMode { get; set; } = false;
    }

    /// <summary>
    /// 
    /// </summary>
    public class ModelExporter
    {
        public static bool TransformUVS { get; set; }

        public static bool ScaleUVs { get; set; }

        private static readonly string ModelFileFilter = @"Support Formats|*.dae;*.smd;*.obj;*.hsdm;";

        /// <summary>
        /// Exports JOBJ to file
        /// </summary>
        /// <param name="rootJOBJ"></param>
        /// <param name="boneLabels"></param>
        public static void ExportFile(HSD_JOBJ rootJOBJ, JointMap jointMap = null)
        {
            string f = FileIO.SaveFile(ModelFileFilter);

            if (f != null)
            {
                if (Path.GetExtension(f).ToLower() == ".hsdm")
                {
                    var path = Path.GetDirectoryName(f);
                    var skl = new HsdSkl(rootJOBJ);

                    // set bones names
                    if (jointMap != null)
                        for(int i = 0; i < skl.Bones.Count; i++)
                        {
                            var name = jointMap[i];
                            if (name != null)
                                skl.Bones[i].Name = name;
                        }

                    // export textures
                    HashSet<int> exported = new HashSet<int>();
                    foreach (var j in rootJOBJ.TreeList)
                    {
                        if (j.Dobj == null)
                            continue;

                        foreach (var d in j.Dobj.List)
                        {
                            if (d.Mobj.Textures == null)
                                continue;

                            foreach (var t in d.Mobj.Textures.List)
                            {
                                var hash = ImporterExtensions.ComputeHash(t.ImageData.ImageData);

                                if (exported.Contains(hash))
                                    continue;

                                var filename = $"t{hash.ToString("X8")}.png";

                                t.SaveImagePNG(Path.Combine(path, filename));
                            }
                        }
                    }
                    HsdJsonHelper.Export(f, skl);
                }
                else
                {
                    ModelExportSettings settings = new();
                    using PropertyDialog d = new("Model Export Options", settings);
                    if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        ExportFile(f, rootJOBJ, settings, jointMap);
                    }
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="rootJOBJ"></param>
        public static void ExportFile(string filePath, HSD_JOBJ rootJOBJ, ModelExportSettings settings = null, JointMap jointMap = null)
        {
            settings.Directory = System.IO.Path.GetDirectoryName(filePath) + "\\";

            if (settings == null)
                settings = new ModelExportSettings();

            ModelExporter exp = new(rootJOBJ, settings, jointMap);

            ExportSettings exportsettings = new()
            {
                FlipUVs = settings.FlipUVs,
                Optimize = settings.Optimize,
                FlipWindingOrder = true,
                ExportTextureInfo = settings.ExportTextureInfo,
                //BlenderMode = settings.BlenderExportMode
            };

            if (settings.ExportModelInfoSheet)
            {
                ModelInfoSheet.Export(settings.Directory + "model_sheet.json", rootJOBJ);
            }

            IOManager.ExportScene(exp.Scene, filePath, exportsettings);
        }

        // parameters

        public IOScene Scene { get; internal set; } = new IOScene();
        private readonly IOModel _model = new();
        private readonly ModelExportSettings _settings;
        private readonly Dictionary<HSD_JOBJ, IOBone> jobjToBone = new();

        private readonly Dictionary<byte[], string> imageToName = new();
        private readonly HSD_JOBJ _root;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        /// <param name="settings"></param>
        /// <param name="boneLabels"></param>
        /// <returns></returns>
        public ModelExporter(HSD_JOBJ jobj, ModelExportSettings settings, JointMap jointMap)
        {
            _root = jobj;
            _settings = settings;

            Scene.Models.Add(_model);

            _model.Skeleton.RootBones.Add(ProcessJoints(jobj, jointMap, System.Numerics.Matrix4x4.Identity));

            if (settings.ExportMesh)
                ProcessDOBJs();
        }


        /// <summary>
        /// 
        /// </summary>
        private IOBone ProcessJoints(HSD_JOBJ jobj, JointMap jointMap, System.Numerics.Matrix4x4 transform)
        {
            // create iobone
            IOBone root = new()
            {
                Name = jointMap[jobjToBone.Count] != null ? jointMap[jobjToBone.Count] : "JOBJ_" + jobjToBone.Count,
                Scale = new System.Numerics.Vector3(jobj.SX, jobj.SY, jobj.SZ),
                RotationEuler = new System.Numerics.Vector3(jobj.RX, jobj.RY, jobj.RZ),
                Translation = new System.Numerics.Vector3(jobj.TX, jobj.TY, jobj.TZ)
            };

            // map jobj to bone
            jobjToBone.Add(jobj, root);

            // check if name is stored in jobj itself and it doesn't have a label
            if (jointMap[jobjToBone.Count] != null &&
                !string.IsNullOrEmpty(jobj.ClassName))
                root.Name = jobj.ClassName;

            // bind pose
            if (jobj.InverseWorldTransform != null && _settings.ExportBindPose)
            {
                HSD_Matrix4x3 mat = jobj.InverseWorldTransform;
                System.Numerics.Matrix4x4.Invert(new System.Numerics.Matrix4x4(
                    mat.M11, mat.M21, mat.M31, 0,
                    mat.M12, mat.M22, mat.M32, 0,
                    mat.M13, mat.M23, mat.M33, 0,
                    mat.M14, mat.M24, mat.M34, 1), out System.Numerics.Matrix4x4 inv);

                System.Numerics.Matrix4x4.Invert(transform, out System.Numerics.Matrix4x4 parinv);

                root.LocalTransform = inv * parinv;
            }

            // 
            transform = root.LocalTransform * transform;

            // process children
            foreach (HSD_JOBJ c in jobj.Children)
                root.AddChild(ProcessJoints(c, jointMap, transform));

            return root;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerable<byte[]> EnumerateStructs(HSDStruct str, HashSet<HSDStruct> hashes = null)
        {
            if (hashes != null &&
                hashes.Contains(str))
                yield break;

            if (hashes == null)
                hashes = new HashSet<HSDStruct>();

            yield return str.GetData();

            foreach (KeyValuePair<int, HSDStruct> r in str.References)
            {
                foreach (byte[] v in EnumerateStructs(r.Value, hashes))
                {
                    yield return v;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public uint GenerateHash32(HSDStruct str)
        {
            const uint FNV_OFFSET_BASIS = 0x811C9DC5;
            const uint FNV_PRIME = 0x01000193;

            uint hash = FNV_OFFSET_BASIS;

            foreach (byte[] arr in EnumerateStructs(str))
            {
                foreach (byte b in arr)
                {
                    hash ^= b;
                    hash *= FNV_PRIME;
                }
            }

            return hash;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ProcessDOBJs()
        {
            int jIndex = 0;
            foreach (HSD_JOBJ j in _root.TreeList)
            {
                int dIndex = 0;
                if (j.Dobj != null)
                {
                    foreach (HSD_DOBJ dobj in j.Dobj.List)
                    {
                        // create mesh
                        IOMesh mesh = new();
                        mesh.Name = $"Joint_{jIndex}_Object_{dIndex}";

                        bool reflective = false;
                        bool single = j != _root;

                        if (!string.IsNullOrEmpty(dobj.ClassName))
                            mesh.Name = dobj.ClassName;

                        // process and export material
                        string matName = $"Material_{GenerateHash32(dobj.Mobj._s):X8}";
                        if (!Scene.Materials.Exists(e => e.Name.Equals(matName)))
                        {
                            IOMaterial m = new();
                            m.Name = matName;
                            m.AmbientColor = new System.Numerics.Vector4(
                                dobj.Mobj.Material.AMB_R / 255f,
                                dobj.Mobj.Material.AMB_G / 255f,
                                dobj.Mobj.Material.AMB_B / 255f,
                                dobj.Mobj.Material.AMB_A / 255f);
                            m.DiffuseColor = new System.Numerics.Vector4(
                                dobj.Mobj.Material.DIF_R / 255f,
                                dobj.Mobj.Material.DIF_G / 255f,
                                dobj.Mobj.Material.DIF_B / 255f,
                                dobj.Mobj.Material.DIF_A / 255f);
                            m.SpecularColor = new System.Numerics.Vector4(
                                dobj.Mobj.Material.SPC_R / 255f,
                                dobj.Mobj.Material.SPC_G / 255f,
                                dobj.Mobj.Material.SPC_B / 255f,
                                dobj.Mobj.Material.SPC_A / 255f);
                            m.Shininess = dobj.Mobj.Material.Shininess;
                            m.Alpha = dobj.Mobj.Material.Alpha;

                            // optionally export mobj
                            if (_settings.ExportMOBJs)
                            {
                                HSDRawFile mobjFile = new();
                                mobjFile.Roots.Add(new HSDRootNode()
                                {
                                    Name = matName,
                                    Data = dobj.Mobj
                                });
                                mobjFile.Save(_settings.Directory + matName + ".mobj");
                            }

                            // process and export textures
                            if (dobj.Mobj.Textures != null)
                            {
                                foreach (HSD_TOBJ t in dobj.Mobj.Textures.List)
                                {
                                    if (t.ImageData != null && t.ImageData.ImageData != null && !imageToName.ContainsKey(t.ImageData.ImageData))
                                    {
                                        string name = $"Texture_{imageToName.Count}_{t.ImageData.Format}";

                                        if (GXImageConverter.IsPalettedFormat(t.ImageData.Format))
                                            name += $"_{t.TlutData.Format}";

                                        if (_settings.ExportTextures)
                                        {
                                            t.SaveImagePNG(_settings.Directory + name + ".png");
                                        }

                                        imageToName.Add(t.ImageData.ImageData, name);
                                    }

                                    if (t.DiffuseLightmap)
                                    {
                                        m.DiffuseMap = new IOTexture()
                                        {
                                            Name = System.IO.Path.GetFileNameWithoutExtension(imageToName[dobj.Mobj.Textures.ImageData.ImageData]),
                                            FilePath = imageToName[dobj.Mobj.Textures.ImageData.ImageData] + ".png"
                                        };
                                    }

                                    if (t.SpecularLightmap)
                                    {
                                        m.SpecularMap = new IOTexture()
                                        {
                                            Name = System.IO.Path.GetFileNameWithoutExtension(imageToName[dobj.Mobj.Textures.ImageData.ImageData]),
                                            FilePath = imageToName[dobj.Mobj.Textures.ImageData.ImageData] + ".png"
                                        };
                                    }
                                }

                            }
                            Scene.Materials.Add(m);
                        }

                        // additional attribtues
                        if (single)
                            mesh.Name += "_SINGLE";

                        if (reflective)
                            mesh.Name += "_REFLECTIVE";

                        // process polygons
                        if (dobj.Pobj != null)
                            foreach (HSD_POBJ pobj in dobj.Pobj.List)
                            {
                                if (pobj.HasAttribute(GXAttribName.GX_TEX_MTX_ARRAY))
                                    reflective = true;

                                if (pobj.HasAttribute(GXAttribName.GX_VA_PNMTXIDX))
                                    single = false;

                                IOPolygon poly = ProcessPOBJ(pobj, j, pobj.SingleBoundJOBJ, dobj.Mobj, mesh, _root);
                                poly.MaterialName = matName;
                                mesh.Polygons.Add(poly);
                            }


                        // done
                        _model.Meshes.Add(mesh);

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
        private IOPolygon ProcessPOBJ(HSD_POBJ pobj, HSD_JOBJ parent, HSD_JOBJ singleBind, HSD_MOBJ mobj, IOMesh mesh, HSD_JOBJ _root)
        {
            IOPolygon poly = new();

            GX_DisplayList dl = pobj.ToDisplayList();
            HSD_Envelope[] envelopes = pobj.EnvelopeWeights;

            System.Numerics.Matrix4x4 parentTransform = System.Numerics.Matrix4x4.Identity;
            if (parent != null && jobjToBone.ContainsKey(parent))
                parentTransform = jobjToBone[parent].WorldTransform;

            System.Numerics.Matrix4x4 singleBindTransform = System.Numerics.Matrix4x4.Identity;
            if (singleBind != null)
                singleBindTransform = jobjToBone[singleBind].WorldTransform;


            int offset = 0;
            foreach (GX_PrimitiveGroup prim in dl.Primitives)
            {
                List<GX_Vertex> verts = dl.Vertices.GetRange(offset, prim.Count);
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

                foreach (GX_Vertex v in verts)
                {
                    // add index
                    poly.Indicies.Add(mesh.Vertices.Count);

                    IOVertex vertex = new();
                    mesh.Vertices.Add(vertex);

                    // joint parented fake rigging
                    if (parent != null && parent != _root && !parent.Flags.HasFlag(JOBJ_FLAG.ENVELOPE_MODEL) && jobjToBone.ContainsKey(parent))
                    {
                        IOBoneWeight vertexWeight = new();
                        vertexWeight.BoneName = jobjToBone[parent].Name;
                        vertexWeight.Weight = 1;
                        vertex.Envelope.Weights.Add(vertexWeight);
                    }

                    // single bind rigging
                    if (singleBind != null)
                    {
                        IOBoneWeight vertexWeight = new();
                        vertexWeight.BoneName = jobjToBone[singleBind].Name;
                        vertexWeight.Weight = 1;
                        vertex.Envelope.Weights.Add(vertexWeight);
                    }

                    //
                    System.Numerics.Matrix4x4 singleMatrix = System.Numerics.Matrix4x4.Identity;
                    bool hasEnvelopes = pobj.HasAttribute(GXAttribName.GX_VA_PNMTXIDX);

                    // process attributes
                    foreach (GX_Attribute a in pobj.ToGXAttributes())
                    {
                        switch (a.AttributeName)
                        {
                            case GXAttribName.GX_VA_PNMTXIDX:

                                if (pobj.Flags.HasFlag(POBJ_FLAG.UNKNOWN2))
                                {
                                    int en = v.PNMTXIDX / 3;

                                    IOBone bone = jobjToBone[parent];
                                    if (en == 1)
                                        bone = jobjToBone[_root.TreeList.Find(e => e.Children.Contains(parent))];

                                    IOBoneWeight vertexWeight = new()
                                    {
                                        BoneName = bone.Name,
                                        Weight = 1
                                    };
                                    vertex.Envelope.Weights.Clear();
                                    vertex.Envelope.Weights.Add(vertexWeight);
                                    singleMatrix = bone.WorldTransform;
                                }
                                else
                                {
                                    HSD_Envelope en = envelopes[v.PNMTXIDX / 3];
                                    for (int w = 0; w < en.EnvelopeCount; w++)
                                    {
                                        IOBoneWeight vertexWeight = new();
                                        if (jobjToBone.ContainsKey(en.JOBJs[w]))
                                        {
                                            vertexWeight.BoneName = jobjToBone[en.JOBJs[w]].Name;
                                        }
                                        else
                                        {
                                            vertexWeight.BoneName = "JOBJ_0";
                                        }
                                        vertexWeight.Weight = en.Weights[w];
                                        vertex.Envelope.Weights.Add(vertexWeight);
                                    }

                                    if (en.EnvelopeCount > 0 && en.GetWeightAt(0) == 1 && jobjToBone.ContainsKey(en.JOBJs[0]))
                                        singleMatrix = jobjToBone[en.JOBJs[0]].WorldTransform;
                                    else
                                        singleMatrix = parentTransform;
                                }

                                break;
                            case GXAttribName.GX_VA_POS:
                                {
                                    System.Numerics.Vector3 vec = new(v.POS.X, v.POS.Y, v.POS.Z);

                                    if (!pobj.Flags.HasFlag(POBJ_FLAG.SHAPESET_AVERAGE) && !hasEnvelopes)
                                        vec = System.Numerics.Vector3.Transform(vec, parentTransform);

                                    if (parent.Flags.HasFlag(JOBJ_FLAG.SKELETON) ||
                                        parent.Flags.HasFlag(JOBJ_FLAG.SKELETON_ROOT) ||
                                        pobj.Flags.HasFlag(POBJ_FLAG.UNKNOWN2))
                                        vec = System.Numerics.Vector3.Transform(vec, singleMatrix);

                                    vec = System.Numerics.Vector3.Transform(vec, singleBindTransform);

                                    vertex.Position = vec;
                                }
                                break;
                            case GXAttribName.GX_VA_NRM:
                                {
                                    System.Numerics.Vector3 vec = new(v.NRM.X, v.NRM.Y, v.NRM.Z);

                                    if (!pobj.Flags.HasFlag(POBJ_FLAG.SHAPESET_AVERAGE) && !hasEnvelopes)
                                        vec = System.Numerics.Vector3.TransformNormal(vec, parentTransform);

                                    if (parent.Flags.HasFlag(JOBJ_FLAG.SKELETON) ||
                                        parent.Flags.HasFlag(JOBJ_FLAG.SKELETON_ROOT) ||
                                        pobj.Flags.HasFlag(POBJ_FLAG.UNKNOWN2))
                                        vec = System.Numerics.Vector3.TransformNormal(vec, singleMatrix);

                                    vec = System.Numerics.Vector3.TransformNormal(vec, singleBindTransform);

                                    vertex.Normal = System.Numerics.Vector3.Normalize(vec);
                                }
                                break;
                            case GXAttribName.GX_VA_CLR0:
                                vertex.SetColor(v.CLR0.R, v.CLR0.G, v.CLR0.B, v.CLR0.A, 0);
                                break;
                            case GXAttribName.GX_VA_CLR1:
                                vertex.SetColor(v.CLR1.R, v.CLR1.G, v.CLR1.B, v.CLR1.A, 1);
                                break;
                            case GXAttribName.GX_VA_TEX0:
                                {
                                    Vector3 uv = ProcessUVTransform(v.TEX0, mobj, 0);
                                    vertex.SetUV(uv.X, uv.Y, 0);
                                }
                                break;
                            case GXAttribName.GX_VA_TEX1:
                                {
                                    Vector3 uv = ProcessUVTransform(v.TEX1, mobj, 1);
                                    vertex.SetUV(uv.X, uv.Y, 1);
                                }
                                break;
                            case GXAttribName.GX_VA_TEX2:
                                {
                                    Vector3 uv = ProcessUVTransform(v.TEX2, mobj, 2);
                                    vertex.SetUV(uv.X, uv.Y, 2);
                                }
                                break;
                            case GXAttribName.GX_VA_TEX3:
                                {
                                    Vector3 uv = ProcessUVTransform(v.TEX3, mobj, 3);
                                    vertex.SetUV(uv.X, uv.Y, 3);
                                }
                                break;
                            case GXAttribName.GX_VA_TEX4:
                                {
                                    Vector3 uv = ProcessUVTransform(v.TEX4, mobj, 4);
                                    vertex.SetUV(uv.X, uv.Y, 4);
                                }
                                break;
                            case GXAttribName.GX_VA_TEX5:
                                {
                                    Vector3 uv = ProcessUVTransform(v.TEX5, mobj, 5);
                                    vertex.SetUV(uv.X, uv.Y, 5);
                                }
                                break;
                            case GXAttribName.GX_VA_TEX6:
                                {
                                    Vector3 uv = ProcessUVTransform(v.TEX6, mobj, 6);
                                    vertex.SetUV(uv.X, uv.Y, 6);
                                }
                                break;
                            case GXAttribName.GX_VA_TEX7:
                                {
                                    Vector3 uv = ProcessUVTransform(v.TEX7, mobj, 7);
                                    vertex.SetUV(uv.X, uv.Y, 7);
                                }
                                break;
                        }
                    }
                }
            }

            return poly;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static Vector3 ProcessUVTransform(GXVector2 gvVec, HSD_MOBJ mobj, int texIndex)
        {
            if (mobj == null || mobj.Textures == null || texIndex >= mobj.Textures.List.Count)
                return new Vector3(gvVec.X, gvVec.Y, 1);

            HSD_TOBJ tex = mobj.Textures.List[texIndex];

            Vector3 uv = new(gvVec.X, gvVec.Y, 0);

            if (TransformUVS)
            {
                Matrix4 transform = Matrix4.CreateScale(tex.SX, tex.SY, tex.SZ) *
                    Math3D.CreateMatrix4FromEuler(tex.RX, tex.RY, tex.RZ) *
                    Matrix4.CreateTranslation(tex.TX, tex.TY, tex.TZ);

                transform.Invert();

                uv = Vector3.TransformPosition(uv, transform);
            }

            if (ScaleUVs)
            {

                Vector2 scale = new(tex.RepeatS, tex.RepeatT);
                uv.Xy *= scale;
            }

            return new Vector3(uv.X, uv.Y, 1);
        }

    }
}
