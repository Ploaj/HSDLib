﻿using System;
using System.Collections.Generic;
using System.Linq;
using HSDRaw.Common;
using OpenTK;
using HSDRawViewer.Rendering;
using HSDRaw.GX;
using System.Drawing;
using HSDRaw;
using IONET;
using IONET.Core;
using IONET.Core.Model;
using IONET.Core.Skeleton;
using HSDRawViewer.Tools;
using HSDRaw.Tools;
using OpenTK.Mathematics;
using HSDRawViewer.GUI.Dialog;
using HSDRawViewer.Tools.Animation;
using System.Xml.Linq;

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

        /// <summary>
        /// Exports JOBJ to file
        /// </summary>
        /// <param name="rootJOBJ"></param>
        /// <param name="boneLabels"></param>
        public static void ExportFile(HSD_JOBJ rootJOBJ, JointMap jointMap = null)
        {
            var f = Tools.FileIO.SaveFile(IOManager.GetExportFileFilter(animation_support: false));

            if (f != null)
            {
                var settings = new ModelExportSettings();
                using (PropertyDialog d = new PropertyDialog("Model Export Options", settings))
                {
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

            ModelExporter exp = new ModelExporter(rootJOBJ, settings, jointMap);
            
            ExportSettings exportsettings = new ExportSettings()
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
        private IOModel _model = new IOModel();
        private ModelExportSettings _settings;
        private Dictionary<HSD_JOBJ, IOBone> jobjToBone = new Dictionary<HSD_JOBJ, IOBone>();

        private Dictionary<byte[], string> imageToName = new Dictionary<byte[], string>();
        private HSD_JOBJ _root;

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
            IOBone root = new IOBone()
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
                var mat = jobj.InverseWorldTransform;
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
            foreach (var c in jobj.Children)
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

            foreach (var r in str.References)
            {
                foreach (var v in EnumerateStructs(r.Value, hashes))
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

            foreach (var arr in EnumerateStructs(str))
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
            var jIndex = 0;
            foreach (var j in _root.TreeList)
            {
                var dIndex = 0;
                if (j.Dobj != null)
                {
                    foreach (var dobj in j.Dobj.List)
                    {
                        // create mesh
                        IOMesh mesh = new IOMesh();
                        mesh.Name = $"Joint_{jIndex}_Object_{dIndex}";

                        bool reflective = false;
                        var single = j != _root;

                        if (!string.IsNullOrEmpty(dobj.ClassName))
                            mesh.Name = dobj.ClassName;

                        // process and export material
                        string matName = $"Material_{GenerateHash32(dobj.Mobj._s):X8}";
                        if (!Scene.Materials.Exists(e=>e.Name.Equals(matName)))
                        {
                            IOMaterial m = new IOMaterial();
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
                                HSDRawFile mobjFile = new HSDRawFile();
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
                                foreach (var t in dobj.Mobj.Textures.List)
                                {
                                    if (t.ImageData != null && t.ImageData.ImageData != null && !imageToName.ContainsKey(t.ImageData.ImageData))
                                    {
                                        var name = $"Texture_{imageToName.Count}_{t.ImageData.Format}";

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
                            foreach (var pobj in dobj.Pobj.List)
                            {
                                if (pobj.HasAttribute(GXAttribName.GX_TEX_MTX_ARRAY))
                                    reflective = true;

                                if (pobj.HasAttribute(GXAttribName.GX_VA_PNMTXIDX))
                                    single = false;
                                
                                var poly = ProcessPOBJ(pobj, j, pobj.SingleBoundJOBJ, dobj.Mobj, mesh, _root);
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
            IOPolygon poly = new IOPolygon();

            var dl = pobj.ToDisplayList();
            var envelopes = pobj.EnvelopeWeights;

            var parentTransform = System.Numerics.Matrix4x4.Identity;
            if (parent != null && jobjToBone.ContainsKey(parent))
                parentTransform = jobjToBone[parent].WorldTransform;

            var singleBindTransform = System.Numerics.Matrix4x4.Identity;
            if (singleBind != null)
                singleBindTransform = jobjToBone[singleBind].WorldTransform;
            
            
            int offset = 0;
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

                foreach (var v in verts)
                {
                    // add index
                    poly.Indicies.Add(mesh.Vertices.Count);

                    IOVertex vertex = new IOVertex();
                    mesh.Vertices.Add(vertex);

                    // joint parented fake rigging
                    if (parent != null && parent != _root && !parent.Flags.HasFlag(JOBJ_FLAG.ENVELOPE_MODEL) && jobjToBone.ContainsKey(parent))
                    {
                        var vertexWeight = new IOBoneWeight();
                        vertexWeight.BoneName = jobjToBone[parent].Name;
                        vertexWeight.Weight = 1;
                        vertex.Envelope.Weights.Add(vertexWeight);
                    }

                    // single bind rigging
                    if (singleBind != null)
                    {
                        var vertexWeight = new IOBoneWeight();
                        vertexWeight.BoneName = jobjToBone[singleBind].Name;
                        vertexWeight.Weight = 1;
                        vertex.Envelope.Weights.Add(vertexWeight);
                    }

                    //
                    System.Numerics.Matrix4x4 singleMatrix = System.Numerics.Matrix4x4.Identity;
                    bool hasEnvelopes = pobj.HasAttribute(GXAttribName.GX_VA_PNMTXIDX);

                    // process attributes
                    foreach (var a in pobj.ToGXAttributes())
                    {
                        switch (a.AttributeName)
                        {
                            case GXAttribName.GX_VA_PNMTXIDX:

                                if (pobj.Flags.HasFlag(POBJ_FLAG.UNKNOWN2))
                                {
                                    var en = v.PNMTXIDX / 3;

                                    IOBone bone = jobjToBone[parent];
                                    if (en == 1)
                                        bone = jobjToBone[_root.TreeList.Find(e => e.Children.Contains(parent))];

                                    var vertexWeight = new IOBoneWeight()
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
                                    var en = envelopes[v.PNMTXIDX / 3];
                                    for (int w = 0; w < en.EnvelopeCount; w++)
                                    {
                                        var vertexWeight = new IOBoneWeight();
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
                                    var vec = new System.Numerics.Vector3(v.POS.X, v.POS.Y, v.POS.Z);

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
                                    var vec = new System.Numerics.Vector3(v.NRM.X, v.NRM.Y, v.NRM.Z);

                                    if (!pobj.Flags.HasFlag(POBJ_FLAG.SHAPESET_AVERAGE) && !hasEnvelopes)
                                        vec = System.Numerics.Vector3.TransformNormal(vec, parentTransform);

                                    if (parent.Flags.HasFlag(JOBJ_FLAG.SKELETON) || parent.Flags.HasFlag(JOBJ_FLAG.SKELETON_ROOT))
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
                                    var uv = ProcessUVTransform(v.TEX0, mobj, 0);
                                    vertex.SetUV(uv.X, uv.Y, 0);
                                }
                                break;
                            case GXAttribName.GX_VA_TEX1:
                                {
                                    var uv = ProcessUVTransform(v.TEX1, mobj, 1);
                                    vertex.SetUV(uv.X, uv.Y, 1);
                                }
                                break;
                            case GXAttribName.GX_VA_TEX2:
                                {
                                    var uv = ProcessUVTransform(v.TEX2, mobj, 2);
                                    vertex.SetUV(uv.X, uv.Y, 2);
                                }
                                break;
                            case GXAttribName.GX_VA_TEX3:
                                {
                                    var uv = ProcessUVTransform(v.TEX3, mobj, 3);
                                    vertex.SetUV(uv.X, uv.Y, 3);
                                }
                                break;
                            case GXAttribName.GX_VA_TEX4:
                                {
                                    var uv = ProcessUVTransform(v.TEX4, mobj, 4);
                                    vertex.SetUV(uv.X, uv.Y, 4);
                                }
                                break;
                            case GXAttribName.GX_VA_TEX5:
                                {
                                    var uv = ProcessUVTransform(v.TEX5, mobj, 5);
                                    vertex.SetUV(uv.X, uv.Y, 5);
                                }
                                break;
                            case GXAttribName.GX_VA_TEX6:
                                {
                                    var uv = ProcessUVTransform(v.TEX6, mobj, 6);
                                    vertex.SetUV(uv.X, uv.Y, 6);
                                }
                                break;
                            case GXAttribName.GX_VA_TEX7:
                                {
                                    var uv = ProcessUVTransform(v.TEX7, mobj, 7);
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

            var tex = mobj.Textures.List[texIndex];

            var uv = new Vector3(gvVec.X, gvVec.Y, 0);

            if (TransformUVS)
            {
                var transform = Matrix4.CreateScale(tex.SX, tex.SY, tex.SZ) *
                    Math3D.CreateMatrix4FromEuler(tex.RX, tex.RY, tex.RZ) *
                    Matrix4.CreateTranslation(tex.TX, tex.TY, tex.TZ);

                transform.Invert();

                uv = Vector3.TransformPosition(uv, transform);
            }

            if (ScaleUVs)
            {

                var scale = new Vector2(tex.RepeatS, tex.RepeatT);
                uv.Xy *= scale;
            }

            return new Vector3(uv.X, uv.Y, 1);
        }

    }
}
