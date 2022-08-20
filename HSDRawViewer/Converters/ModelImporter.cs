using HSDRaw.Common;
using HSDRaw.GX;
using HSDRaw.Tools;
using HSDRawViewer.GUI.Dialog;
using HSDRawViewer.GUI.Extra;
using HSDRawViewer.Rendering.GX;
using IONET;
using IONET.Core;
using IONET.Core.Model;
using IONET.Core.Skeleton;
using OpenTK;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace HSDRawViewer.Converters
{
    public class ModelImporter : ProgressClass
    {
        // location to look for external files like textures
        public string FolderPath;

        // main model settings
        private ModelImportSettings Settings;

        // lookup mesh settings from iomesh
        private Dictionary<IOMesh, MeshImportSettings> meshSettings = new Dictionary<IOMesh, MeshImportSettings>();

        // lookup material settings from iomaterial
        private Dictionary<IOMaterial, MaterialImportSettings> materialSettings = new Dictionary<IOMaterial, MaterialImportSettings>();

        // cache the jobj names to their respective jobj
        private Dictionary<string, HSD_JOBJ> NameToJOBJ = new Dictionary<string, HSD_JOBJ>();

        // Indicates jobjs that need the SKELETON flag set along with inverted transform
        private List<HSD_JOBJ> EnvelopedJOBJs = new List<HSD_JOBJ>();

        // for cleaning root
        private Matrix4 CleanRotMatrix = Matrix4.Identity;

        // 
        private Dictionary<HSD_JOBJ, Matrix4> jobjToWorldTransform = new Dictionary<HSD_JOBJ, Matrix4>();

        // cache textures
        private Dictionary<string, HSD_TOBJ> pathToTObj = new Dictionary<string, HSD_TOBJ>();

        /// <summary>
        /// 
        /// </summary>
        private HSD_JOBJ NewModel;

        /// <summary>
        /// 
        /// </summary>
        private IOScene scene;

        /// <summary>
        /// 
        /// </summary>
        private IOModel model;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static HSD_JOBJ ImportModelFromFile(string f)
        {
            if (f != null)
            {
                var scene = IONET.IOManager.LoadScene(f, new IONET.ImportSettings()
                {
                    Triangulate = true,
                });

                using (ModelImportDialog d = new ModelImportDialog(scene, scene.Models[0]))
                {
                    if (d.ShowDialog() == DialogResult.OK)
                    {
                        ModelImporter imp = new ModelImporter(Path.GetDirectoryName(f), scene, scene.Models[0], d.settings, d.GetMeshSettings(), d.GetMaterialSettings());

                        using (ProgressBarDisplay pb = new ProgressBarDisplay(imp))
                        {
                            pb.DoWork();
                            pb.ShowDialog();
                        }

                        return imp.NewModel;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toReplace"></param>
        public static HSD_JOBJ ImportModelFromFile()
        {
            return ImportModelFromFile(Tools.FileIO.OpenFile(IOManager.GetModelImportFileFilter()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toReplace"></param>
        public static void ReplaceModelFromFile(HSD_JOBJ toReplace)
        {
            var model = ImportModelFromFile();

            if (model != null)
                toReplace._s.SetFromStruct(model._s);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="scene"></param>
        /// <param name="settings"></param>
        /// <param name="meshSettings"></param>
        /// <param name="materialSettings"></param>
        public ModelImporter(string folderPath, IOScene scene, IOModel model, ModelImportSettings settings, IEnumerable<MeshImportSettings> meshSettings, IEnumerable<MaterialImportSettings> materialSettings)
        {
            this.scene = scene;
            this.model = model;
            this.FolderPath = folderPath;
            this.Settings = settings;

            // generate mesh lookup
            foreach (var m in meshSettings)
                this.meshSettings.Add(m._poly, m);

            // generate material lookup
            foreach (var m in materialSettings)
                this.materialSettings.Add(m._material, m);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="w"></param>
        public override void Work(BackgroundWorker w)
        {
            try
            {
                Import(w);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to import model: {e.ToString()}");
                w.ReportProgress(100);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="w"></param>
        private void Import(BackgroundWorker w)
        {
            // settings
            if (Settings == null)
                Settings = new ModelImportSettings();

            // create pobj generate
            POBJ_Generator POBJGen = new POBJ_Generator()
            {
                UseTriangleStrips = Settings.UseStrips,
                VertexColorFormat = (GXCompType)Settings.VertexColorFormat,
            };

            // apply settings to model
            if (Settings.SmoothNormals)
                model.SmoothNormals();

            // process nodes
            ProgressStatus = "Processing Joints...";
            w.ReportProgress(30);
            HSD_JOBJ root = null;
            foreach (var r in model.Skeleton.RootBones)
            {
                if (r.Name == "Armature")
                    continue;

                if (root == null)
                    root = IOBoneToJOBJ(r);
                else
                    root.Add(IOBoneToJOBJ(r));
            }
            if (root == null)
            {
                root = IOBoneToJOBJ(
                    new IOBone()
                    {
                        Name = "Root"
                    });
            }


            // process mesh
            ProgressStatus = "Processing Mesh...";
            foreach (var mesh in model.Meshes)
            {
                ProcessMesh(scene, mesh, root, ref POBJGen);

                if (root.Dobj != null)
                {
                    ProgressStatus = $"Processing Mesh {root.Dobj.List.Count} {model.Meshes.Count + 1}...";
                    w.ReportProgress((int)(30 + 60 * (root.Dobj.List.Count / (float)model.Meshes.Count)));
                }
            }

            // calculate inverse binds
            if (Settings.EnvelopeAll)
            {
                foreach (var v in jobjToWorldTransform)
                    v.Key.InverseWorldTransform = v.Value.Inverted().ToHsdMatrix();
            }
            else
            {
                foreach (var jobj in EnvelopedJOBJs)
                {
                    ProgressStatus = "Generating Inverse Transforms...";
                    if (jobjToWorldTransform.ContainsKey(jobj))
                        jobj.InverseWorldTransform = jobjToWorldTransform[jobj].Inverted().ToHsdMatrix();
                }
            }


            // SAVE POBJ buffers
            ProgressStatus = "Generating and compressing vertex buffers...";
            w.ReportProgress(90);
            POBJGen.SaveChanges();

            // done
            NewModel = root;

            // auto update flags
            NewModel.UpdateFlags();

            // apply classical scale flag
            if (Settings.ClassicalScaling)
                NewModel.Flags |= JOBJ_FLAG.CLASSICAL_SCALING;

#if DEBUG
            //
            if (Settings.Merge)
                JOBJExtensions.MergeIntoOneObject(NewModel);
#endif

            ProgressStatus = "Done!";
            w.ReportProgress(100);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bone"></param>
        /// <returns></returns>
        private HSD_JOBJ IOBoneToJOBJ(IOBone bone)
        {
#if DEBUG
            //
            if (Settings.CleanRoot)
            {
                // if this is root bone calculate the clean matrix
                if (bone.Parent == null)
                {
                    CleanRotMatrix = Matrix4.CreateFromQuaternion(new Quaternion(bone.Rotation.X, bone.Rotation.Y, bone.Rotation.Z, bone.Rotation.W));
                    bone.Rotation = System.Numerics.Quaternion.Identity;
                }
                else // otherwise apply the cleaning to current bone
                {
                    var pos = Vector3.TransformNormal(new Vector3(bone.TranslationX, bone.TranslationY, bone.TranslationZ), CleanRotMatrix);
                    bone.Translation = new System.Numerics.Vector3(pos.X, pos.Y, pos.Z);
                }

                // clean scale
                bone.Scale = new System.Numerics.Vector3(1, 1, 1);
            }

            // melee-ify model if flag is set
            if (bone.Parent == null && Settings.Meleeify)
            {
                bone.Meleeify();
            }
#endif
            // create jobj
            HSD_JOBJ jobj = new HSD_JOBJ()
            {
                TX = bone.TranslationX,
                TY = bone.TranslationY,
                TZ = bone.TranslationZ,
                RX = bone.RotationEuler.X,
                RY = bone.RotationEuler.Y,
                RZ = bone.RotationEuler.Z,
                SX = bone.ScaleX,
                SY = bone.ScaleY,
                SZ = bone.ScaleZ,
            };

            // optional import bone name
            if (Settings.ImportBoneNames)
                jobj.ClassName = bone.Name;

            // add to lookup
            NameToJOBJ.Add(bone.Name, jobj);

            // add world transform lookup
            jobjToWorldTransform.Add(jobj, bone.WorldTransform.ToTKMatrix());

            // recursivly add children
            foreach (var child in bone.Children)
            {
                jobj.AddChild(IOBoneToJOBJ(child));
            }

            return jobj;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void ProcessMesh(IOScene scene, IOMesh mesh, HSD_JOBJ rootnode, ref POBJ_Generator POBJGen)
        {
            // get settings if they exist
            MeshImportSettings settings;
            if (meshSettings.ContainsKey(mesh))
                settings = meshSettings[mesh];
            else
                settings = new MeshImportSettings(mesh);

            // apply settings
            if (settings.FlipUVs)
                mesh.FlipUVs();

            if (settings.FlipFaces)
                mesh.FlipWindingOrder();

            // determine parent
            HSD_JOBJ parent = rootnode;
            if (mesh.ParentBone != null && NameToJOBJ.ContainsKey(mesh.ParentBone.Name))
                parent = NameToJOBJ[mesh.ParentBone.Name];

            Console.WriteLine("Processing " + mesh.Name);

            if (mesh.Name.Contains("BUMP"))
            {
                mesh.CalculateTangentBitangent();

                foreach (var v in mesh.Vertices)
                {
                    v.Tangent /= 100;
                    v.Binormal /= 100;
                }
            }

            // begin processing polygons
            HSD_DOBJ root = null;
            HSD_DOBJ prev = null;
            foreach (var poly in mesh.Polygons)
            {
                // Skip Empty Polygon
                if (poly.Indicies.Count == 0)
                    continue;

                // convert to triangles
                poly.ToTriangles(mesh);
                if (poly.PrimitiveType != IOPrimitive.TRIANGLE)
                    continue;

                // Generate DOBJ
                HSD_DOBJ dobj = new HSD_DOBJ();

                if (Settings.ImportMeshNames)
                    dobj.ClassName = mesh.Name;

                if (root == null)
                    root = dobj;
                else
                    prev.Next = dobj;
                prev = dobj;


                // generate material
                var material = scene.Materials.Find(e => e.Name == poly.MaterialName);
                dobj.Mobj = GenerateMaterial(settings, material);


                Console.WriteLine(mesh.Name + " " + material?.Name);


                // reflective mobjs do not use uvs
                var hasReflection = settings.IsReflective;
#if DEBUG
                if (Settings.MetalModel)
                    hasReflection = true;
#endif

                // bump maps need tangents and bitangents
                var hasBump = settings.GenerateTB;

                // Assess additional attributes based on the material MOBJ
                if (dobj.Mobj.Textures != null)
                {
                    foreach (var t in dobj.Mobj.Textures.List)
                    {
                        if (t.Flags.HasFlag(TOBJ_FLAGS.COORD_REFLECTION))
                            hasReflection = true;
                        if (t.Flags.HasFlag(TOBJ_FLAGS.BUMP))
                            hasBump = true;
                    }
                }

                // assess attributes
                List<GXAttribName> Attributes = new List<GXAttribName>();

                // determine if position normal matrix is needed
                if (mesh.HasEnvelopes() && Settings.ImportSkinning && !settings.SingleBind)
                {
                    Attributes.Add(GXAttribName.GX_VA_PNMTXIDX);

                    if (hasReflection)
                    {
                        Attributes.Add(GXAttribName.GX_VA_TEX0MTXIDX);

                        if (mesh.Name.Contains("REFLECTIVE2"))
                            Attributes.Add(GXAttribName.GX_VA_TEX1MTXIDX);
                        else
                        if (dobj.Mobj.Textures != null)
                        {
                            if (dobj.Mobj.Textures.List.Count > 1)
                                Attributes.Add(GXAttribName.GX_VA_TEX1MTXIDX);

                            if (dobj.Mobj.Textures.List.Count > 2)
                                Attributes.Add(GXAttribName.GX_VA_TEX2MTXIDX);
                        }
#if DEBUG
                        if (Settings.MetalModel && !Attributes.Contains(GXAttribName.GX_VA_TEX1MTXIDX))
                            Attributes.Add(GXAttribName.GX_VA_TEX1MTXIDX);
#endif
                    }
                }

                // always add position attribute
                Attributes.Add(GXAttribName.GX_VA_POS);

                // add normal attribute if needed
                if (hasBump)
                    Attributes.Add(GXAttribName.GX_VA_NBT);
                else
                if (mesh.HasNormals && settings.ImportNormals)
                    Attributes.Add(GXAttribName.GX_VA_NRM);

                // add vertex color attribute if needed
                if (mesh.HasColorSet(0) && settings.ImportVertexColor)
                    Attributes.Add(GXAttribName.GX_VA_CLR0);

                if (mesh.HasColorSet(1) && settings.ImportVertexColor)
                    Attributes.Add(GXAttribName.GX_VA_CLR1);

                // add texture coordinate uvs
#if DEBUG
                if (!Settings.MetalModel)
                {
#endif

                    if (mesh.HasUVSet(0))
                        Attributes.Add(GXAttribName.GX_VA_TEX0);

                    if ((mesh.HasUVSet(1) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 1)))
                        Attributes.Add(GXAttribName.GX_VA_TEX1);

                    if ((mesh.HasUVSet(2) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 2)))
                        Attributes.Add(GXAttribName.GX_VA_TEX2);

                    if ((mesh.HasUVSet(3) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 3)))
                        Attributes.Add(GXAttribName.GX_VA_TEX3);

                    if ((mesh.HasUVSet(4) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 4)))
                        Attributes.Add(GXAttribName.GX_VA_TEX4);

                    if ((mesh.HasUVSet(5) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 5)))
                        Attributes.Add(GXAttribName.GX_VA_TEX5);

                    if ((mesh.HasUVSet(6) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 6)))
                        Attributes.Add(GXAttribName.GX_VA_TEX6);

                    if ((mesh.HasUVSet(7) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 7)))
                        Attributes.Add(GXAttribName.GX_VA_TEX7);

#if DEBUG
                }
#endif

                // being processing mesh
                var vertices = new List<GX_Vertex>();
                var jobjList = new List<HSD_JOBJ[]>();
                var weightList = new List<float[]>();

                // generarte vertex list
                foreach (var face in poly.Indicies)
                {
                    var v = mesh.Vertices[face];

                    GX_Vertex vertex = new GX_Vertex();

                    var tkvert = new Vector3(v.Position.X, v.Position.Y, v.Position.Z);
                    var tknrm = new Vector3(v.Normal.X, v.Normal.Y, v.Normal.Z);
                    var tktan = new Vector3(v.Tangent.X, v.Tangent.Y, v.Tangent.Z);
                    var tkbitan = new Vector3(v.Binormal.X, v.Binormal.Y, v.Binormal.Z);

                    // transform by inverse of parent
                    if (jobjToWorldTransform[parent] != Matrix4.Identity)
                    {
                        var parentTransform = jobjToWorldTransform[parent].Inverted();
                        tkvert = Vector3.TransformPosition(tkvert, parentTransform);
                        tknrm = Vector3.TransformNormal(tknrm, parentTransform).Normalized();
                        tktan = Vector3.TransformNormal(tktan, parentTransform);
                        tkbitan = Vector3.TransformNormal(tkbitan, parentTransform);
                    }

                    // additional processing for skin
                    if (mesh.HasEnvelopes() && Settings.ImportSkinning)
                    {
                        // create weighting lists
                        List<float> weight = new List<float>();
                        List<HSD_JOBJ> bones = new List<HSD_JOBJ>();

                        // single bind if there are no weights
                        if (v.Envelope.Weights.Count == 0)
                        {
                            weight.Add(1);
                            bones.Add(rootnode);
                        }

                        // check for too many weights
                        if (v.Envelope.Weights.Count > 6)
                            throw new Exception($"Too many weights! {v.Envelope.Weights.Count} in {mesh.Name}");

                        // process weights
                        foreach (var bw in v.Envelope.Weights)
                        {
                            // check if skeleton actually contains bone
                            if (NameToJOBJ.ContainsKey(bw.BoneName))
                            {
                                // add envelope
                                bones.Add(NameToJOBJ[bw.BoneName]);
                                weight.Add(bw.Weight);

                                // indicate enveloped jobjs
                                if (!EnvelopedJOBJs.Contains(NameToJOBJ[bw.BoneName]))
                                    EnvelopedJOBJs.Add(NameToJOBJ[bw.BoneName]);
                            }
                            else
                            {
                                throw new Exception($"Bone not found \"{bw.BoneName}\" Weight: {bw.Weight} in {mesh.Name}");
                            }
                        }

                        // add to jobj and weight list
                        jobjList.Add(bones.ToArray());
                        weightList.Add(weight.ToArray());

                        // invert single binds
                        if (v.Envelope.Weights.Count == 1)
                        {
                            var inv = jobjToWorldTransform[NameToJOBJ[v.Envelope.Weights[0].BoneName]].Inverted();
                            tkvert = Vector3.TransformPosition(tkvert, inv);
                            tknrm = Vector3.TransformNormal(tknrm, inv).Normalized();
                            tktan = Vector3.TransformNormal(tktan, inv);
                            tkbitan = Vector3.TransformNormal(tkbitan, inv);
                        }
                    }

                    // set final vertex data
                    vertex.POS = GXTranslator.fromVector3(tkvert);
                    vertex.NRM = GXTranslator.fromVector3(tknrm.Normalized());
                    vertex.TAN = GXTranslator.fromVector3(tktan);
                    vertex.BITAN = GXTranslator.fromVector3(tkbitan);

                    if (settings.InvertNormals)
                    {
                        vertex.NRM.X *= -1;
                        vertex.NRM.Y *= -1;
                        vertex.NRM.Z *= -1;
                        vertex.TAN.X *= -1;
                        vertex.TAN.Y *= -1;
                        vertex.TAN.Z *= -1;
                        vertex.BITAN.X *= -1;
                        vertex.BITAN.Y *= -1;
                        vertex.BITAN.Z *= -1;
                    }

                    if (mesh.HasUVSet(0))
                        vertex.TEX0 = new GXVector2(v.UVs[0].X, v.UVs[0].Y);

                    if (mesh.HasUVSet(1))
                        vertex.TEX1 = new GXVector2(v.UVs[1].X, v.UVs[1].Y);

                    if (mesh.HasUVSet(2))
                        vertex.TEX2 = new GXVector2(v.UVs[2].X, v.UVs[2].Y);

                    if (mesh.HasUVSet(3))
                        vertex.TEX3 = new GXVector2(v.UVs[3].X, v.UVs[3].Y);

                    if (mesh.HasUVSet(4))
                        vertex.TEX4 = new GXVector2(v.UVs[4].X, v.UVs[4].Y);

                    if (mesh.HasUVSet(5))
                        vertex.TEX5 = new GXVector2(v.UVs[5].X, v.UVs[5].Y);

                    if (mesh.HasUVSet(6))
                        vertex.TEX6 = new GXVector2(v.UVs[6].X, v.UVs[6].Y);

                    if (mesh.HasUVSet(7))
                        vertex.TEX7 = new GXVector2(v.UVs[7].X, v.UVs[7].Y);

                    if (mesh.HasColorSet(0))
                        vertex.CLR0 = new GXColor4(
                        v.Colors[0].X,
                        v.Colors[0].Y,
                        v.Colors[0].Z,
                        v.Colors[0].W);

                    if (mesh.HasColorSet(1))
                        vertex.CLR1 = new GXColor4(
                        v.Colors[1].X,
                        v.Colors[1].Y,
                        v.Colors[1].Z,
                        v.Colors[1].W);

                    vertices.Add(vertex);
                }


                // generate pobjs
                HSD_POBJ pobj = null;

                if (mesh.HasEnvelopes() && Settings.ImportSkinning && !settings.SingleBind)
                    pobj = POBJGen.CreatePOBJsFromTriangleList(vertices, Attributes.ToArray(), jobjList, weightList);
                else
                    pobj = POBJGen.CreatePOBJsFromTriangleList(vertices, Attributes.ToArray(), null);

                if (settings.SingleBind && jobjList.Count > 0 && jobjList[0].Length > 0)
                    parent = jobjList[0][0];

                if (pobj != null)
                {
                    if (dobj.Pobj == null)
                        dobj.Pobj = pobj;
                    else
                        dobj.Pobj.Add(pobj);
                }

            }

            if (parent.Dobj == null)
                parent.Dobj = root;
            else
                parent.Dobj.Add(root);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string GetFullTexturePath(string path)
        {
            var texturePath = path;

            if (texturePath.Contains("file://"))
                texturePath = texturePath.Replace("file://", "");

            if (File.Exists(Path.Combine(FolderPath, texturePath)))
                texturePath = Path.Combine(FolderPath, texturePath);

            if (File.Exists(path))
                texturePath = path;

            if (File.Exists(texturePath + ".png"))
                texturePath = texturePath + ".png";

            return texturePath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="material"></param>
        /// <returns></returns>
        private HSD_MOBJ GenerateMaterial(MeshImportSettings mesh, IOMaterial material)
        {
            // create blank mobj
            var Mobj = new HSD_MOBJ();
            Mobj.Material = new HSD_Material()
            {
                AMB_A = 0xFF,
                AMB_R = 0x7F,
                AMB_G = 0x7F,
                AMB_B = 0x7F,
                DiffuseColor = System.Drawing.Color.White,
                SpecularColor = System.Drawing.Color.White,
                Shininess = 50,
                Alpha = 1
            };

            // no material data
            if (material == null)
                return Mobj;

            // get settings if they exist
            MaterialImportSettings settings;
            if (materialSettings.ContainsKey(material))
                settings = materialSettings[material];
            else
                settings = new MaterialImportSettings(material);

            // optional mobj loading
            if (settings.ImportMOBJ && 
                settings.ImportTexture &&
                material.DiffuseMap != null && 
                !string.IsNullOrEmpty(material.DiffuseMap.FilePath))
            {
                var textPath = GetFullTexturePath(material.DiffuseMap.FilePath);
                var mobjPath = Path.Combine(Path.GetDirectoryName(textPath), Path.GetFileNameWithoutExtension(textPath) + ".mobj");

                if (File.Exists(mobjPath))
                {
                    Mobj._s = new HSDRaw.HSDRawFile(mobjPath).Roots[0].Data._s;
                    return Mobj;
                }
            }

            // detect and set flags
            if (mesh.ImportVertexColor)
                Mobj.RenderFlags |= RENDER_MODE.VERTEX;

            if (settings.EnableDiffuse)
                Mobj.RenderFlags |= RENDER_MODE.DIFFUSE;

            if (settings.EnableConstant)
                Mobj.RenderFlags |= RENDER_MODE.CONSTANT;

            // Properties
            if (settings.ImportMaterialInfo)
            {
                Mobj.Material.Shininess = settings.Shininess;
                Mobj.Material.Alpha = settings.Alpha;
                Mobj.Material.AmbientColor = settings.AmbientColor;
                Mobj.Material.DiffuseColor = settings.DiffuseColor;
                Mobj.Material.SpecularColor = settings.SpecularColor;
            }

            // Textures
            if (settings.ImportTexture)
            {
                if (material.DiffuseMap != null && !string.IsNullOrEmpty(material.DiffuseMap.FilePath))
                {
                    var texturePath = GetFullTexturePath(material.DiffuseMap.FilePath);

                    if (File.Exists(texturePath) && (texturePath.ToLower().EndsWith(".png") || texturePath.ToLower().EndsWith(".bmp")))
                    {
                        Mobj.RenderFlags |= RENDER_MODE.TEX0;

                        if (pathToTObj.ContainsKey(texturePath))
                        {
                            Mobj.Textures = HSDRaw.HSDAccessor.DeepClone<HSD_TOBJ>(pathToTObj[texturePath]);
                        }
                        else
                        {
                            var tobj = TOBJConverter.ImportTOBJFromFile(texturePath, settings.TextureFormat, settings.PaletteFormat);
                            tobj.Flags = TOBJ_FLAGS.LIGHTMAP_DIFFUSE | TOBJ_FLAGS.COORD_UV | TOBJ_FLAGS.COLORMAP_MODULATE;

                            tobj.GXTexGenSrc = GXTexGenSrc.GX_TG_TEX0;
                            tobj.TexMapID = GXTexMapID.GX_TEXMAP0;

                            tobj.WrapS = material.DiffuseMap.WrapS.ToGXWrapMode();
                            tobj.WrapT = material.DiffuseMap.WrapT.ToGXWrapMode();

                            if (TOBJConverter.IsTransparent(tobj))
                            {
                                Mobj.RenderFlags |= RENDER_MODE.XLU;
                                tobj.Flags |= TOBJ_FLAGS.ALPHAMAP_MODULATE;
                            }

                            Mobj.Textures = tobj;

                            pathToTObj.Add(texturePath, tobj);
                        }
                    }
                }
            }

            return Mobj;
        }
    }
}
