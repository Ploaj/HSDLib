using System;
using System.Collections.Generic;
using Assimp;
using Assimp.Configs;
using HSDRaw.Common;
using OpenTK;
using HSDRawViewer.Rendering;
using System.IO;
using HSDRaw.Tools;
using HSDRaw.GX;
using System.ComponentModel;
using HSDRawViewer.GUI;

namespace HSDRawViewer.Converters
{
    public enum ForceGroupModes
    {
        None,
        Texture,
        MeshGroup
    }

    /// <summary>
    /// 
    /// </summary>
    public class ModelImportSettings
    {
        [Category("Importing Options"), DisplayName("Flip Faces"), Description("Flips direction of faces, useful if model imports inside out")]
        public bool FlipFaces { get; set; } = false;

        [Category("Importing Options"), DisplayName("Flip UVs"), Description("Flips UVs on the Y axis, useful if textures are upside down")]
        public bool FlipUVs { get; set; } = false;

        [Category("Importing Options"), DisplayName("Flip Normals"), Description("Flips direction of normals, useful if model is all black with textures")]
        public bool InvertNormals { get; set; } = false;

        //[Category("Importing Options"), DisplayName("Smooth Normals"), Description("Applies normal smoothing")]
        //public bool SmoothNormals { get; set; } = false;
        

        [Category("Importing Options"), DisplayName("Import Bone Names"), Description("Stores bone names in JOBJs")]
        public bool ImportBoneNames { get; set; } = false;

        [Category("Importing Options"), DisplayName("Import Mesh Names"), Description("Stores mesh names in DOBJs")]
        public bool ImportMeshNames { get; set; } = false;


        [Category("Importing Options"), DisplayName("Use Triangle Strips"), Description("Slower to import, but better optimized for game")]
        public bool UseStrips { get; set; } = true;

        [Category("Importing Options"), DisplayName("Force Merge Objects"), Description("Reduces number of DOBJs by forcing mesh groups to be grouped by material")]
        public ForceGroupModes ForceMergeObjects { get; set; } = ForceGroupModes.None;

        [Category("Importing Options"), DisplayName("Scale"), Description("Amount to scale model by when importing")]
        public float Scale { get; set; } = 1;

        [Category("Importing Options"), DisplayName("Set Joint Scale to 1"), Description("Sets all joint scaling to 1, 1, 1")]
        public bool SetScaleToOne { get; set; } = false;

        [Category("Importing Options"), DisplayName("Import Rigging"), Description("Import rigging from model file")]
        public bool ImportRigging { get; set; } = true;

        
        
        [Category("Material Options"), DisplayName("Import MOBJs"), Description("Imports .mobj files from file")]
        public bool ImportMOBJ { get; set; } = false;

        [Category("Material Options"), DisplayName("Import Material Info"), Description("Imports the material info from model file. NOT recommended")]
        public bool ImportMaterialInfo { get; set; } = false;

        [Category("Material Options"), DisplayName("Import Normals"), Description("")]
        public bool ImportNormals { get; set; } = true;
        
        [Category("Material Options"), DisplayName("Import Vertex Colors"), Description("")]
        public bool ImportVertexColor { get; set; } = false;

        [Category("Material Options"), DisplayName("Enable Diffuse"), Description("")]
        public bool EnableDiffuse { get; set; } = true;


        [Category("Material Vertex Color Options"), DisplayName("Import Vertex Alpha"), Description("Import the alpha color from vertex colors")]
        public bool ImportVertexAlpha { get; set; } = false;

        [Category("Material Vertex Color Options"), DisplayName("Multiply by 2"), Description("Multiplies vertex colors by 2")]
        public bool MultiplyVertexColorBy2 { get; set; } = false;



        [Category("Texture Options"), DisplayName("Import Textures"), Description("Imports textures from model file if they exist.")]
        public bool ImportTexture { get; set; } = true;

        [Category("Texture Options"), DisplayName("Texture Format"), Description("The format to store the texture data in.")]
        public GXTexFmt TextureFormat { get; set; } = GXTexFmt.CI8;

        [Category("Texture Options"), DisplayName("Palette Format"), Description("Palette format used with CI8 and CI4.")]
        public GXTlutFmt PaletteFormat { get; set; } = GXTlutFmt.RGB565;


        [Category("Misc"), DisplayName("Apply Fighter Transform (Melee Fighter Only)"), Description("Applies fighter transforms for use with Super Smash Bros. Melee")]
        public bool ZeroOutRotationsAndApplyFighterTransforms { get; set; } = false;
        
        [Category("Misc"), DisplayName("Apply Naruto GNT Materials"), Description("Applys Material Style used in Naruto Clash of Ninja games")]
        public bool ApplyNarutoMaterials { get; set; } = false;
    }


    /// <summary>
    /// 
    /// </summary>
    public class ModelProcessCache
    {
        // Folder path to search for external files like textures
        public string FolderPath;

        // this tool automatically splits triangle lists into stripped pobjs
        public POBJ_Generator POBJGen = new POBJ_Generator();

        // cache the jobj names to their respective jobj
        public Dictionary<string, HSD_JOBJ> NameToJOBJ = new Dictionary<string, HSD_JOBJ>();

        // SingleBoundJOBJ bound vertices need to be inverted by their parent bone
        public Dictionary<HSD_JOBJ, Matrix4> jobjToWorldTransform = new Dictionary<HSD_JOBJ, Matrix4>();

        //
        public Dictionary<HSD_JOBJ, Matrix4> jobjToNewTransform = new Dictionary<HSD_JOBJ, Matrix4>();

        // mesh nodes need to be processed after the jobjs
        public List<Node> MeshNodes = new List<Node>();

        // Indicates jobjs that need the SKELETON flag set along with inverted transform
        public List<HSD_JOBJ> EnvelopedJOBJs = new List<HSD_JOBJ>();

        // keeps matches texture path to dobj for better grouping options
        public Dictionary<string, HSD_DOBJ> TextureToDOBJ = new Dictionary<string, HSD_DOBJ>();

        public bool HasXLU = false;
    }



    /// <summary>
    /// Static class for importing 3d model information
    /// </summary>
    public class ModelImporter : ProgressClass
    {
        private string FilePath;
        private ModelImportSettings Settings;
        public HSD_JOBJ NewModel;

        public ModelImporter(string filePath, ModelImportSettings settings)
        {
            FilePath = filePath;
            Settings = settings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="w"></param>
        public override void Work(BackgroundWorker w)
        {
            // settings
            if (Settings == null)
                Settings = new ModelImportSettings();

            ModelProcessCache cache = new ModelProcessCache();

            cache.POBJGen.UseTriangleStrips = Settings.UseStrips;

            cache.FolderPath = Path.GetDirectoryName(FilePath);

            var processFlags = PostProcessPreset.TargetRealTimeMaximumQuality
                | PostProcessSteps.Triangulate
                | PostProcessSteps.LimitBoneWeights
                | PostProcessSteps.CalculateTangentSpace;

            if (!Settings.FlipFaces)
                processFlags |= PostProcessSteps.FlipWindingOrder;

            if (Settings.FlipUVs)
                processFlags |= PostProcessSteps.FlipUVs;

            //if (Settings.SmoothNormals)
            //    processFlags |= PostProcessSteps.GenerateSmoothNormals;
            
            ProgressStatus = "Importing Model with Assimp...";
            w.ReportProgress(0);
            // import
            AssimpContext importer = new AssimpContext();
            //if (Settings.SmoothNormals)
            //    importer.SetConfig(new NormalSmoothingAngleConfig(80.0f));
            importer.SetConfig(new VertexBoneWeightLimitConfig(4));
            var importmodel = importer.ImportFile(FilePath, processFlags);
            
            ProgressStatus = "Processing Nodes...";
            w.ReportProgress(30);
            // process nodes
            var rootNode = importmodel.RootNode;
            var rootjobj = RecursiveProcess(cache, Settings, importmodel, importmodel.RootNode);
            
            // get root of skeleton
            if(rootjobj.Child != null)
                rootjobj = rootjobj.Child;
            rootjobj.Flags = 0;

            rootjobj.Flags |= JOBJ_FLAG.SKELETON_ROOT;

            // no need for excess nodes
            //if (filePath.ToLower().EndsWith(".obj"))
            rootjobj.Next = null;

            // Clear rotations
            if (Settings.ZeroOutRotationsAndApplyFighterTransforms)
            {
                ProgressStatus = "Clearing Rotations...";
                cache.jobjToNewTransform = JOBJTools.ApplyMeleeFighterTransforms(rootjobj);
            }

            // process mesh
            ProgressStatus = "Processing Mesh...";
            foreach (var mesh in cache.MeshNodes)
            {
                ProcessMesh(cache, Settings, importmodel, mesh, rootjobj);

                ProgressStatus = $"Processing Mesh {rootjobj.Dobj.List.Count} {cache.MeshNodes.Count + 1}...";
                w.ReportProgress((int)(30 + 60 * (rootjobj.Dobj.List.Count / (float)cache.MeshNodes.Count)));
                
                //TODO:
                //if (c.Flags.HasFlag(JOBJ_FLAG.OPA) || c.Flags.HasFlag(JOBJ_FLAG.ROOT_OPA))
                //    jobj.Flags |= JOBJ_FLAG.ROOT_OPA;

                if (!rootjobj.Flags.HasFlag(JOBJ_FLAG.LIGHTING) && Settings.EnableDiffuse)
                {
                    rootjobj.Flags |= JOBJ_FLAG.LIGHTING;
                }
            }

            // SKELETON 
            if (cache.EnvelopedJOBJs.Count > 0)
                rootjobj.Flags |= JOBJ_FLAG.ENVELOPE_MODEL;

            // Transparency 
            if (cache.HasXLU)
                rootjobj.Flags |= JOBJ_FLAG.XLU;

            // Opa
            rootjobj.Flags |= JOBJ_FLAG.OPA;

            foreach (var jobj in cache.EnvelopedJOBJs)
            {
                ProgressStatus = "Generating Inverse Transforms...";
                jobj.Flags |= JOBJ_FLAG.SKELETON;
                if (cache.jobjToNewTransform.ContainsKey(jobj))
                    jobj.InverseWorldTransform = Matrix4ToHSDMatrix(cache.jobjToNewTransform[jobj].Inverted());
                else
                    jobj.InverseWorldTransform = Matrix4ToHSDMatrix(cache.jobjToWorldTransform[jobj].Inverted());
            }

            if (Settings.ApplyNarutoMaterials)
            {
                ProgressStatus = "Applying Naruto Materials...";
                ApplyNarutoMaterials(rootjobj);
            }

            // SAVE POBJ buffers
            ProgressStatus = "Generating and compressing vertex buffers...";
            w.ReportProgress(90);
            cache.POBJGen.SaveChanges();

            // done
            NewModel = rootjobj;
            ProgressStatus = "Done!";
            w.ReportProgress(100);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toReplace"></param>
        public static void ReplaceModelFromFile(HSD_JOBJ toReplace)
        {
            var f = Tools.FileIO.OpenFile("Supported Formats (*.dae, *.obj)|*.dae;*.obj;*.fbx;*.smd");

            if(f != null)
            {
                var settings = new ModelImportSettings();
                using (PropertyDialog d = new PropertyDialog("Model Import Options", settings))
                {
                    if(d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        ModelImporter imp = new ModelImporter(f, settings);

                        using (ProgressBarDisplay pb = new ProgressBarDisplay(imp))
                        {
                            pb.DoWork();
                            pb.ShowDialog();
                        }

                        var newroot = imp.NewModel;

                        toReplace._s.SetFromStruct(newroot._s);
                    }
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static int ComputeHash(params byte[] data)
        {
            unchecked
            {
                const int p = 16777619;
                int hash = (int)2166136261;

                for (int i = 0; i < data.Length; i++)
                    hash = (hash ^ data[i]) * p;

                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;
                return hash;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootjobj"></param>
        private static void ApplyNarutoMaterials(HSD_JOBJ rootjobj)
        {
            foreach (var j in rootjobj.BreathFirstList)
            {
                if (j.Dobj != null)
                    foreach (var d in j.Dobj.List)
                    {
                        d.Mobj.Material.SPC_A = 255;
                        d.Mobj.Material.SPC_B = 0;
                        d.Mobj.Material.SPC_G = 0;
                        d.Mobj.Material.SPC_R = 0;
                        d.Mobj.Material.Shininess = 50;
                    }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static HSD_Matrix4x3 Matrix4ToHSDMatrix(Matrix4 t)
        {
            var o = new HSD_Matrix4x3();
            o.M11 = t.M11;
            o.M12 = t.M21;
            o.M13 = t.M31;
            o.M14 = t.M41;
            o.M21 = t.M12;
            o.M22 = t.M22;
            o.M23 = t.M32;
            o.M24 = t.M42;
            o.M31 = t.M13;
            o.M32 = t.M23;
            o.M33 = t.M33;
            o.M34 = t.M43;
            return o;
        }

        /// <summary>
        /// Recursivly processing nodes and convert data into JOBJ
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private static HSD_JOBJ RecursiveProcess(ModelProcessCache cache, ModelImportSettings settings, Scene scene, Node node)
        {
            if (node.Name.EndsWith("_end"))
                return null;
            
            var t = node.Transform;
            var transform = new OpenTK.Matrix4(
                                        t.A1, t.B1, t.C1, t.D1,
                                        t.A2, t.B2, t.C2, t.D2,
                                        t.A3, t.B3, t.C3, t.D3,
                                        t.A4, t.B4, t.C4, t.D4);
            
            var translation = transform.ExtractTranslation();
            var scale = transform.ExtractScale();
            var rotation = Math3D.ToEulerAngles(transform.ExtractRotation().Inverted());

            if (settings.SetScaleToOne)
                scale = Vector3.One;

            translation *= settings.Scale;
            
            HSD_JOBJ jobj = new HSD_JOBJ();

            if (node.Parent != null)
            {
                transform = transform * cache.jobjToWorldTransform[cache.NameToJOBJ[node.Parent.Name]];
            }
            cache.NameToJOBJ.Add(node.Name, jobj);
            cache.jobjToWorldTransform.Add(jobj, transform);

            if(settings.ImportBoneNames)
                jobj.ClassName = node.Name;
            jobj.Flags = JOBJ_FLAG.CLASSICAL_SCALING;
            jobj.TX = RoundFloat(translation.X);
            jobj.TY = RoundFloat(translation.Y);
            jobj.TZ = RoundFloat(translation.Z);
            jobj.RX = RoundFloat(rotation.X);
            jobj.RY = RoundFloat(rotation.Y);
            jobj.RZ = RoundFloat(rotation.Z);
            jobj.SX = RoundFloat(scale.X);
            jobj.SY = RoundFloat(scale.Y);
            jobj.SZ = RoundFloat(scale.Z);

            if (node.HasMeshes)
                cache.MeshNodes.Add(node);

            foreach (var child in node.Children)
            {
                Console.WriteLine(child.Name);
                jobj.AddChild(RecursiveProcess(cache, settings, scene, child));
            }

            return jobj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private static float RoundFloat(float f)
        {
            if (Math.Abs(f) < 0.000001)
                return 0;
            else
                return f;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static void ProcessMesh(ModelProcessCache cache, ModelImportSettings settings, Scene scene, Node node, HSD_JOBJ rootnode)
        {
            HSD_JOBJ parent = rootnode;

            HSD_DOBJ root = null;
            HSD_DOBJ prev = null;

            var skeleton = rootnode.BreathFirstList;
            Console.WriteLine("Processing " + node.Name);

            bool singleBinded = node.Name.Contains("SINGLE");

            foreach (int index in node.MeshIndices)
            {
                Mesh mesh = scene.Meshes[index];
                var material = scene.Materials[mesh.MaterialIndex];

                Console.WriteLine(mesh.Name + " " + material.Name);
                //Console.WriteLine(cache.jobjToWorldTransform[cache.NameToJOBJ[node.Name]].ToString());

                // Generate DOBJ
                HSD_DOBJ dobj = new HSD_DOBJ();

                if(settings.ImportMeshNames)
                    dobj.ClassName = mesh.Name;

                // hack to make dobjs merged by texture
                if (settings.ImportTexture && 
                    settings.ForceMergeObjects == ForceGroupModes.Texture &&
                    material.HasTextureDiffuse &&
                    cache.TextureToDOBJ.ContainsKey(material.TextureDiffuse.FilePath))
                {
                    dobj = cache.TextureToDOBJ[material.TextureDiffuse.FilePath];
                }
                else
                {
                    if (root == null)
                        root = dobj;
                    else
                        prev.Next = dobj;
                    prev = dobj;
                    
                    dobj.Mobj = GenerateMaterial(cache, settings, material);
                    if(settings.ForceMergeObjects == ForceGroupModes.Texture &&
                        material.HasTextureDiffuse && 
                        settings.ImportTexture)
                        cache.TextureToDOBJ.Add(material.TextureDiffuse.FilePath, dobj);
                }
                
                if (root != null && settings.ForceMergeObjects == ForceGroupModes.MeshGroup)
                    dobj = root;

                // Assessment
                if (!mesh.HasFaces)
                    continue;
                
                // Assess needed attributes based on the material MOBJ

                // reflective mobjs do not use uvs
                var hasReflection = false;
                // bump maps need tangents and bitangents
                var hasBump = false;

                if (node.Name.Contains("REFLECTIVE"))
                    hasReflection = true;

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

                List<GXAttribName> Attributes = new List<GXAttribName>();

                // todo: rigging
                List<HSD_JOBJ>[] jobjs = new List<HSD_JOBJ>[mesh.Vertices.Count];
                List<float>[] weights = new List<float>[mesh.Vertices.Count];
                List<Matrix4>[] binds = new List<Matrix4>[mesh.Vertices.Count];
                List<Matrix4>[] worlds = new List<Matrix4>[mesh.Vertices.Count];
                if (mesh.HasBones && settings.ImportRigging)
                {
                    if(!singleBinded)
                        Attributes.Add(GXAttribName.GX_VA_PNMTXIDX);

                    foreach (var v in mesh.Bones)
                    {
                        var jobj = cache.NameToJOBJ[v.Name];

                        if (!skeleton.Contains(jobj))
                            jobj = skeleton[0];

                        if (!cache.EnvelopedJOBJs.Contains(jobj))
                            cache.EnvelopedJOBJs.Add(jobj);

                        var t = v.OffsetMatrix;
                        var invmat = new Matrix4(
                            t.A1, t.B1, t.C1, t.D1,
                            t.A2, t.B2, t.C2, t.D2,
                            t.A3, t.B3, t.C3, t.D3,
                            t.A4, t.B4, t.C4, t.D4);

                        var envjobj = cache.NameToJOBJ[v.Name];

                        if (v.HasVertexWeights)
                            foreach (var vw in v.VertexWeights)
                            {
                                if (jobjs[vw.VertexID] == null)
                                    jobjs[vw.VertexID] = new List<HSD_JOBJ>();
                                if (weights[vw.VertexID] == null)
                                    weights[vw.VertexID] = new List<float>();
                                if (binds[vw.VertexID] == null)
                                    binds[vw.VertexID] = new List<Matrix4>();
                                if (worlds[vw.VertexID] == null)
                                    worlds[vw.VertexID] = new List<Matrix4>();

                                if (vw.Weight > 0)
                                {
                                    jobjs[vw.VertexID].Add(envjobj);
                                    weights[vw.VertexID].Add(vw.Weight);

                                    if (cache.jobjToNewTransform.ContainsKey(envjobj))
                                        binds[vw.VertexID].Add(cache.jobjToNewTransform[envjobj].Inverted());
                                    else
                                        binds[vw.VertexID].Add(invmat);

                                    worlds[vw.VertexID].Add(invmat * cache.jobjToWorldTransform[envjobj]);
                                }
                            }
                    }
                }

                if (hasReflection)
                    Attributes.Add(GXAttribName.GX_VA_TEX0MTXIDX);


                if (mesh.HasVertices)
                    Attributes.Add(GXAttribName.GX_VA_POS);


                if (hasBump)
                    Attributes.Add(GXAttribName.GX_VA_NBT);
                else
                if (mesh.HasNormals && settings.ImportNormals)
                    Attributes.Add(GXAttribName.GX_VA_NRM);


                if (mesh.HasVertexColors(0) && settings.ImportVertexColor)
                    Attributes.Add(GXAttribName.GX_VA_CLR0);


                if (mesh.HasTextureCoords(0) && !hasReflection)
                    Attributes.Add(GXAttribName.GX_VA_TEX0);

                if ((mesh.HasTextureCoords(1) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 1)) && !hasReflection)
                    Attributes.Add(GXAttribName.GX_VA_TEX1);

                if ((mesh.HasTextureCoords(2) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 2)) && !hasReflection)
                    Attributes.Add(GXAttribName.GX_VA_TEX2);

                if ((mesh.HasTextureCoords(3) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 3)) && !hasReflection)
                    Attributes.Add(GXAttribName.GX_VA_TEX3);

                if ((mesh.HasTextureCoords(4) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 4)) && !hasReflection)
                    Attributes.Add(GXAttribName.GX_VA_TEX4);

                if ((mesh.HasTextureCoords(5) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 5)) && !hasReflection)
                    Attributes.Add(GXAttribName.GX_VA_TEX5);

                if ((mesh.HasTextureCoords(6) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 6)) && !hasReflection)
                    Attributes.Add(GXAttribName.GX_VA_TEX6);

                if ((mesh.HasTextureCoords(7) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 7)) && !hasReflection)
                    Attributes.Add(GXAttribName.GX_VA_TEX7);

                var vertices = new List<GX_Vertex>();
                var jobjList = new List<HSD_JOBJ[]>(vertices.Count);
                var wList = new List<float[]>(vertices.Count);

                foreach (var face in mesh.Faces)
                {
                    PrimitiveType faceMode;
                    switch (face.IndexCount)
                    {
                        case 1:
                            faceMode = PrimitiveType.Point;
                            break;
                        case 2:
                            faceMode = PrimitiveType.Line;
                            break;
                        case 3:
                            faceMode = PrimitiveType.Triangle;
                            break;
                        default:
                            faceMode = PrimitiveType.Polygon;
                            break;
                    }

                    if (faceMode != PrimitiveType.Triangle)
                    {
                        continue;
                    }

                    for (int i = 0; i < face.IndexCount; i++)
                    {
                        int indicie = face.Indices[i];

                        GX_Vertex vertex = new GX_Vertex();

                        var tkvert = new Vector3(mesh.Vertices[indicie].X, mesh.Vertices[indicie].Y, mesh.Vertices[indicie].Z) * settings.Scale;
                        var tknrm = new Vector3(mesh.Normals[indicie].X, mesh.Normals[indicie].Y, mesh.Normals[indicie].Z);
                        var tktan = Vector3.Zero;
                        var tkbitan = Vector3.Zero;

                        tkvert = Vector3.TransformPosition(tkvert, cache.jobjToWorldTransform[cache.NameToJOBJ[node.Name]]);
                        tknrm = Vector3.TransformNormal(tknrm, cache.jobjToWorldTransform[cache.NameToJOBJ[node.Name]]);

                        tkvert = Vector3.TransformPosition(tkvert, cache.jobjToWorldTransform[rootnode].Inverted());
                        tknrm = Vector3.TransformNormal(tknrm, cache.jobjToWorldTransform[rootnode].Inverted());

                        if (mesh.HasBones && settings.ImportRigging)
                        {
                            //  unbound verts
                            if (jobjs[indicie] == null) 
                            {
                                jobjs[indicie] = new List<HSD_JOBJ>();
                                weights[indicie] = new List<float>();
                                binds[indicie] = new List<Matrix4>();
                                worlds[indicie] = new List<Matrix4>();
                            }

                            jobjList.Add(jobjs[indicie].ToArray());
                            wList.Add(weights[indicie].ToArray());

                            // tan and bitan
                            // TODO; tan is weird
                            if (mesh.HasTangentBasis)
                            {
                                tktan = new Vector3(mesh.Tangents[indicie].X, mesh.Tangents[indicie].Y, mesh.Tangents[indicie].Z);
                                tkbitan = new Vector3(mesh.BiTangents[indicie].X, mesh.BiTangents[indicie].Y, mesh.BiTangents[indicie].Z);
                            }

                            if (weights[indicie].Count > 1)
                            {
                                var bindv = Vector3.Zero;
                                var bindvn = Vector3.Zero;
                                for (int k = 0; k < weights[indicie].Count; k++)
                                {
                                    bindv += Vector3.TransformPosition(tkvert, worlds[indicie][k]) * weights[indicie][k];
                                    bindvn += Vector3.TransformNormal(tknrm, worlds[indicie][k]) * weights[indicie][k];
                                }
                                tkvert = bindv;
                                tknrm = bindvn;
                            }

                            if (weights[indicie].Count == 1)
                            {
                                tkvert = Vector3.TransformPosition(tkvert, binds[indicie][0]);
                                tknrm = Vector3.TransformNormal(tknrm, binds[indicie][0]);
                            }
                        }

                        if (mesh.HasVertices)
                            vertex.POS = GXTranslator.fromVector3(tkvert);

                        if (mesh.HasNormals)
                            vertex.NRM = GXTranslator.fromVector3(tknrm);

                        if (mesh.HasTangentBasis)
                        {
                            vertex.TAN = GXTranslator.fromVector3(tktan);
                            vertex.BITAN = GXTranslator.fromVector3(tkbitan);
                        }

                        if (settings.InvertNormals)
                        {
                            vertex.NRM.X *= -1;
                            vertex.NRM.Y *= -1;
                            vertex.NRM.Z *= -1;
                        }
                        
                        if (mesh.HasTextureCoords(0))
                            vertex.TEX0 = new GXVector2(
                                mesh.TextureCoordinateChannels[0][indicie].X,
                                mesh.TextureCoordinateChannels[0][indicie].Y);

                        if (mesh.HasTextureCoords(1))
                            vertex.TEX1 = new GXVector2(
                                mesh.TextureCoordinateChannels[1][indicie].X,
                                mesh.TextureCoordinateChannels[1][indicie].Y);

                        if (mesh.HasTextureCoords(2))
                            vertex.TEX2 = new GXVector2(
                                mesh.TextureCoordinateChannels[2][indicie].X,
                                mesh.TextureCoordinateChannels[2][indicie].Y);

                        if (mesh.HasTextureCoords(3))
                            vertex.TEX3 = new GXVector2(
                                mesh.TextureCoordinateChannels[3][indicie].X,
                                mesh.TextureCoordinateChannels[3][indicie].Y);

                        if (mesh.HasTextureCoords(4))
                            vertex.TEX4 = new GXVector2(
                                mesh.TextureCoordinateChannels[4][indicie].X,
                                mesh.TextureCoordinateChannels[4][indicie].Y);

                        if (mesh.HasTextureCoords(5))
                            vertex.TEX5 = new GXVector2(
                                mesh.TextureCoordinateChannels[5][indicie].X,
                                mesh.TextureCoordinateChannels[5][indicie].Y);

                        if (mesh.HasTextureCoords(6))
                            vertex.TEX6 = new GXVector2(
                                mesh.TextureCoordinateChannels[6][indicie].X,
                                mesh.TextureCoordinateChannels[6][indicie].Y);

                        if (mesh.HasTextureCoords(7))
                            vertex.TEX7 = new GXVector2(
                                mesh.TextureCoordinateChannels[7][indicie].X,
                                mesh.TextureCoordinateChannels[7][indicie].Y);

                        if (mesh.HasVertexColors(0))
                            vertex.CLR0 = new GXColor4(
                                mesh.VertexColorChannels[0][indicie].R * (settings.MultiplyVertexColorBy2 ? 2 : 1),
                                mesh.VertexColorChannels[0][indicie].G * (settings.MultiplyVertexColorBy2 ? 2 : 1),
                                mesh.VertexColorChannels[0][indicie].B * (settings.MultiplyVertexColorBy2 ? 2 : 1),
                                settings.ImportVertexAlpha ? mesh.VertexColorChannels[0][indicie].A : 1);

                        if (mesh.HasVertexColors(1))
                            vertex.CLR0 = new GXColor4(
                                mesh.VertexColorChannels[1][indicie].R * (settings.MultiplyVertexColorBy2 ? 2 : 1),
                                mesh.VertexColorChannels[1][indicie].G * (settings.MultiplyVertexColorBy2 ? 2 : 1),
                                mesh.VertexColorChannels[1][indicie].B * (settings.MultiplyVertexColorBy2 ? 2 : 1),
                                settings.ImportVertexAlpha ? mesh.VertexColorChannels[1][indicie].A : 1);

                        vertices.Add(vertex);
                    }

                }

                HSD_POBJ pobj = null;

                if (mesh.HasBones && settings.ImportRigging && !singleBinded)
                    pobj = cache.POBJGen.CreatePOBJsFromTriangleList(vertices, Attributes.ToArray(), jobjList, wList);
                else
                    pobj = cache.POBJGen.CreatePOBJsFromTriangleList(vertices, Attributes.ToArray(), null);

                if (singleBinded && jobjList.Count > 0 && jobjList[0].Length > 0)
                    parent = jobjList[0][0];

                if(pobj != null)
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
        /// <param name="settings"></param>
        /// <param name="material"></param>
        /// <returns></returns>
        private static HSD_MOBJ GenerateMaterial(ModelProcessCache cache, ModelImportSettings settings, Material material)
        {
            var Mobj = new HSD_MOBJ();
            Mobj.Material = new HSD_Material();
            Mobj.Material.AMB_A = 0xFF;
            Mobj.Material.AMB_R = 0x7F;
            Mobj.Material.AMB_G = 0x7F;
            Mobj.Material.AMB_B = 0x7F;
            Mobj.Material.DiffuseColor = System.Drawing.Color.White;
            Mobj.Material.SpecularColor = System.Drawing.Color.White;
            Mobj.Material.Shininess = 50;
            Mobj.Material.Alpha = 1;

            if (settings.ImportVertexColor)
                Mobj.RenderFlags |= RENDER_MODE.VERTEX;

            if (settings.EnableDiffuse)
                Mobj.RenderFlags |= RENDER_MODE.DIFFUSE;

            // Properties
            if (settings.ImportMaterialInfo)
            {
                if (material.HasShininess)
                    Mobj.Material.Shininess = material.Shininess;

                if (material.HasColorAmbient)
                {
                    Mobj.Material.AMB_A = ColorFloatToByte(material.ColorAmbient.A);
                    Mobj.Material.AMB_R = ColorFloatToByte(material.ColorAmbient.R);
                    Mobj.Material.AMB_G = ColorFloatToByte(material.ColorAmbient.G);
                    Mobj.Material.AMB_B = ColorFloatToByte(material.ColorAmbient.B);
                }
                if (material.HasColorDiffuse)
                {
                    Mobj.Material.DIF_A = ColorFloatToByte(material.ColorDiffuse.A);
                    Mobj.Material.DIF_R = ColorFloatToByte(material.ColorDiffuse.R);
                    Mobj.Material.DIF_G = ColorFloatToByte(material.ColorDiffuse.G);
                    Mobj.Material.DIF_B = ColorFloatToByte(material.ColorDiffuse.B);
                }
                if (material.HasColorSpecular)
                {
                    Mobj.Material.SPC_A = ColorFloatToByte(material.ColorSpecular.A);
                    Mobj.Material.SPC_R = ColorFloatToByte(material.ColorSpecular.R);
                    Mobj.Material.SPC_G = ColorFloatToByte(material.ColorSpecular.G);
                    Mobj.Material.SPC_B = ColorFloatToByte(material.ColorSpecular.B);
                }
            }

            // Textures
            if(settings.ImportTexture)
            {
                if (material.HasTextureDiffuse)
                {
                    var texturePath = Path.Combine(cache.FolderPath, material.TextureDiffuse.FilePath);

                    if (File.Exists(material.TextureDiffuse.FilePath))
                        texturePath = material.TextureDiffuse.FilePath;

                    if (File.Exists(texturePath + ".png"))
                        texturePath = texturePath + ".png";

                    var mobjPath = Path.Combine(cache.FolderPath, Path.GetFileNameWithoutExtension(texturePath)) + ".mobj";
                    
                    if(settings.ImportMOBJ && File.Exists(mobjPath))
                    {
                        var dat = new HSDRaw.HSDRawFile(mobjPath);
                        Mobj._s = dat.Roots[0].Data._s;
                        return Mobj;
                    }
                    else
                    /// special mobj loading
                    if (Path.GetExtension(texturePath).ToLower() == ".mobj")
                    {
                        var dat = new HSDRaw.HSDRawFile(texturePath);
                        Mobj._s = dat.Roots[0].Data._s;
                        return Mobj;
                    }
                    else
                    if (File.Exists(texturePath) && (texturePath.ToLower().EndsWith(".png") || texturePath.ToLower().EndsWith(".bmp")))
                    {
                        Mobj.RenderFlags |= RENDER_MODE.TEX0;

                        var tobj = TOBJConverter.ImportTOBJFromFile(texturePath, settings.TextureFormat, settings.PaletteFormat);
                        tobj.Flags = TOBJ_FLAGS.LIGHTMAP_DIFFUSE | TOBJ_FLAGS.COORD_UV | TOBJ_FLAGS.COLORMAP_MODULATE;
                        
                        tobj.GXTexGenSrc = 4;
                        tobj.TexMapID = GXTexMapID.GX_TEXMAP0;

                        tobj.WrapS = ToGXWrapMode(material.TextureDiffuse.WrapModeU);
                        tobj.WrapT = ToGXWrapMode(material.TextureDiffuse.WrapModeV);

                        if (TOBJConverter.IsTransparent(tobj))
                        {
                            cache.HasXLU = true;
                            Mobj.RenderFlags |= RENDER_MODE.XLU | RENDER_MODE.NO_ZUPDATE;
                            tobj.Flags |= TOBJ_FLAGS.ALPHAMAP_MODULATE;
                        }

                        Mobj.Textures = tobj;
                    }
                }
            }

            return Mobj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        private static GXWrapMode ToGXWrapMode(TextureWrapMode mode)
        {
            switch (mode)
            {
                case TextureWrapMode.Clamp:
                    return GXWrapMode.CLAMP;
                case TextureWrapMode.Mirror:
                    return GXWrapMode.MIRROR;
                default:
                    return GXWrapMode.REPEAT;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private static byte ColorFloatToByte(float val)
        {
            return (byte)(val > 1.0f ? 255 : val * 256);
        }

        /// <summary>
        /// Converts from Assimps <see cref="Matrix4x4"/> to OpenTK's <see cref="Matrix4"/>
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static Matrix4 FromMatrix(Matrix4x4 mat)
        {
            Matrix4 m = new Matrix4();
            m.M11 = mat.A1;
            m.M12 = mat.A2;
            m.M13 = mat.A3;
            m.M14 = mat.A4;
            m.M21 = mat.B1;
            m.M22 = mat.B2;
            m.M23 = mat.B3;
            m.M24 = mat.B4;
            m.M31 = mat.C1;
            m.M32 = mat.C2;
            m.M33 = mat.C3;
            m.M34 = mat.C4;
            m.M41 = mat.D1;
            m.M42 = mat.D2;
            m.M43 = mat.D3;
            m.M44 = mat.D4;
            return m;
        }

    }
}