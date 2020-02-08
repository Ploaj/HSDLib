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
using System.Linq;

namespace HSDRawViewer.Converters
{
    public enum ShadingType
    {
        None,
        Material,
        VertexColor
    }

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
        // Options
        [Category("Importing Options")]
        public bool FlipUVs { get; set; } = false;

        [Category("Importing Options")]
        public bool SmoothNormals { get; set; } = false;

        [Category("Importing Options")]
        public bool InvertNormals { get; set; } = false;

        [Category("Importing Options")]
        public bool SetScaleToOne { get; set; } = false;

        [Category("Importing Options")]
        public ShadingType ShadingType{ get; set; } = ShadingType.Material;

        [Category("Importing Options")]
        public bool ImportMaterialInfo { get; set; } = false;

        [Category("Importing Options")]
        public bool ImportTexture { get; set; } = true;

        [DisplayName("Use Triangle Strips"), Description("Slower to import, but better optimized for game"), Category("Importing Options")]
        public bool UseStrips { get; set; } = true;

        [DisplayName("Force Merge Objects"), Category("Importing Options"), Description("Reduces number of DOBJs by forces mesh groups to use one material")]
        public ForceGroupModes ForceMergeObjects { get; set; } = ForceGroupModes.None;

        [Category("Importing Options")]
        public float Scale { get; set; } = 1;

        //[Category("Importing Options"), Description("Attempts to keep JOBJ and DOBJ matching original model")]
        //public bool TryToUseExistingStructure { get; set; } = true;


        [Category("Vertex Attribute Options")]
        public bool ImportRigging { get; set; } = true;

        [Category("Vertex Attribute Options")]
        public bool ImportVertexAlpha { get; set; } = false;


        //[Category("Texture Options"), Description("Texture Type is inferred from filename i.e. Image_CMP.png")]
        //public bool TextureFormatFromName { get; set; } = true;

        [Category("Texture Options")]
        public GXTexFmt TextureFormat { get; set; } = GXTexFmt.RGB565;

        [Category("Texture Options")]
        public GXTlutFmt PaletteFormat { get; set; } = GXTlutFmt.RGB565;


        [DisplayName("Apply Fighter Transform (Melee Fighter Only)"), Category("Misc"), Description("Applies fighter transforms for use with Super Smash Bros. Melee")]
        public bool ZeroOutRotationsAndApplyFighterTransforms { get; set; } = false;
        
        [DisplayName("Apply Naruto GNT Materials"), Category("Misc"), Description("Applys Material Style used in Naruto Clash of Ninja games")]
        public bool ApplyNarutoMaterials { get; set; } = false;
    }

    /// <summary>
    /// Static class for importing 3d model information
    /// </summary>
    public class ModelImporter
    {
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
                        var newroot = ImportModel(f, settings);

                        toReplace._s.SetFromStruct(newroot._s);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class ProcessingCache
        {
            // Folder path to search for external files like textures
            public string FolderPath;
            
            // this tool automatically splits triangle lists into stripped pobjs
            public POBJ_Generator POBJGen = new POBJ_Generator();

            // cache the jobj names to their respective jobj
            public Dictionary<string, HSD_JOBJ> NameToJOBJ = new Dictionary<string, HSD_JOBJ>();

            // SingleBoundJOBJ bound vertices need to be inverted by their parent bone
            public Dictionary<HSD_JOBJ, Matrix4> jobjToInverseTransform = new Dictionary<HSD_JOBJ, Matrix4>();
            public Dictionary<HSD_JOBJ, Matrix4> jobjToWorldTransform = new Dictionary<HSD_JOBJ, Matrix4>();

            // mesh nodes need to be processed after the jobjs
            public List<Node> MeshNodes = new List<Node>();

            // Indicates jobjs that need the SKELETON flag set along with inverted transform
            public List<HSD_JOBJ> EnvelopedJOBJs = new List<HSD_JOBJ>();

            // keeps matches texture path to dobj for better grouping options
            public Dictionary<string, HSD_DOBJ> TextureToDOBJ = new Dictionary<string, HSD_DOBJ>();
        }

        /// <summary>
        /// Imports supported model format into Root JOBJ
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static HSD_JOBJ ImportModel(string filePath, ModelImportSettings settings = null)
        {
            // settings
            if(settings == null)
                settings = new ModelImportSettings();

            ProcessingCache cache = new ProcessingCache();

            cache.POBJGen.UseTriangleStrips = settings.UseStrips;

            cache.FolderPath = Path.GetDirectoryName(filePath);

            var processFlags = PostProcessPreset.TargetRealTimeMaximumQuality
                | PostProcessSteps.Triangulate
                | PostProcessSteps.LimitBoneWeights
                | PostProcessSteps.FlipWindingOrder
                | PostProcessSteps.CalculateTangentSpace;

            if (settings.FlipUVs)
                processFlags |= PostProcessSteps.FlipUVs;

            System.Diagnostics.Debug.WriteLine("Importing Model...");
            // import
            AssimpContext importer = new AssimpContext();
            if(settings.SmoothNormals)
                importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            importer.SetConfig(new VertexBoneWeightLimitConfig(4));
            var importmodel = importer.ImportFile(filePath, processFlags);

            System.Diagnostics.Debug.WriteLine("Processing Nodes...");
            // process nodes
            var rootNode = importmodel.RootNode;
            var rootjobj = RecursiveProcess(cache, settings, importmodel, importmodel.RootNode);

            // Clear rotations
            if(settings.ZeroOutRotationsAndApplyFighterTransforms)
                ZeroOutRotations(cache, rootjobj);

            // get root of skeleton
            rootjobj = rootjobj.Child;

            rootjobj.Flags |= JOBJ_FLAG.SKELETON_ROOT;

            // no need for excess nodes
            //if (filePath.ToLower().EndsWith(".obj"))
                rootjobj.Next = null;

            // process mesh
            System.Diagnostics.Debug.WriteLine("Processing Mesh...");
            foreach (var mesh in cache.MeshNodes)
            {
                var Dobj = GetMeshes(cache, settings, importmodel, mesh);

                if (rootjobj.Dobj == null)
                    rootjobj.Dobj = Dobj;
                else
                    rootjobj.Dobj.Add(Dobj);

                System.Diagnostics.Debug.WriteLine($"Processing Mesh {rootjobj.Dobj.List.Count} {cache.MeshNodes.Count + 1}...");

                rootjobj.Flags |= JOBJ_FLAG.OPA;

                //TODO:
                //if (c.Flags.HasFlag(JOBJ_FLAG.OPA) || c.Flags.HasFlag(JOBJ_FLAG.ROOT_OPA))
                //    jobj.Flags |= JOBJ_FLAG.ROOT_OPA;

                if (settings.ShadingType == ShadingType.Material)
                {
                    rootjobj.Flags |= JOBJ_FLAG.LIGHTING;
                    foreach (var dobj in rootjobj.Dobj.List)
                        dobj.Mobj.RenderFlags |= RENDER_MODE.DIFFUSE;
                }
            }
            
            // SKELETON 
            if (cache.EnvelopedJOBJs.Count > 0)
                rootjobj.Flags |= JOBJ_FLAG.ENVELOPE_MODEL;

            foreach (var jobj in cache.EnvelopedJOBJs)
            {
                jobj.Flags |= JOBJ_FLAG.SKELETON;
                jobj.InverseWorldTransform = Matrix4ToHSDMatrix(cache.jobjToInverseTransform[jobj]);
            }

            if (settings.ApplyNarutoMaterials)
            {
                ApplyNarutoMaterials(rootjobj);
            }

            // SAVE POBJ buffers
            System.Diagnostics.Debug.WriteLine("Saving Changes...");
            cache.POBJGen.SaveChanges();

            System.Diagnostics.Debug.WriteLine("Done!");

            // done
            return rootjobj;
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
            rootjobj.Flags = JOBJ_FLAG.SKELETON_ROOT | JOBJ_FLAG.ENVELOPE_MODEL | JOBJ_FLAG.LIGHTING | JOBJ_FLAG.OPA | JOBJ_FLAG.ROOT_OPA;

            foreach (var j in rootjobj.BreathFirstList)
            {
                if (j.Dobj != null)
                    foreach (var d in j.Dobj.List)
                    {
                        d.Mobj.RenderFlags = RENDER_MODE.ALPHA_COMPAT | RENDER_MODE.DIFFUSE;
                        if (d.Mobj.Textures != null)
                            d.Mobj.RenderFlags |= RENDER_MODE.TEX0;

                        d.Mobj.Material.SPC_A = 255;
                        d.Mobj.Material.SPC_B = 0;
                        d.Mobj.Material.SPC_G = 0;
                        d.Mobj.Material.SPC_R = 0;
                        d.Mobj.Material.Shininess = 50;

                        if (d.Mobj.Textures != null)
                        {
                            foreach (var t in d.Mobj.Textures.List)
                            {
                                t.Flags = TOBJ_FLAGS.COORD_UV | TOBJ_FLAGS.LIGHTMAP_DIFFUSE | TOBJ_FLAGS.COLORMAP_BLEND;
                            }
                        }
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
        /// 
        /// </summary>
        private static void ZeroOutRotations(ProcessingCache cache, HSD_JOBJ root)
        {
            Dictionary<HSD_JOBJ, Matrix4> newWorldMatrices = new Dictionary<HSD_JOBJ, Matrix4>();

            ZeroOutRotations(cache, newWorldMatrices, root, Matrix4.Identity, Matrix4.Identity);

            cache.jobjToWorldTransform = newWorldMatrices;
            cache.jobjToInverseTransform.Clear();
            foreach(var v in newWorldMatrices)
                cache.jobjToInverseTransform.Add(v.Key, v.Value.Inverted());
        }

        private static Dictionary<string, Vector3> FighterDefaults = new Dictionary<string, Vector3>() { { "TopN", new Vector3(0, 0, 0) },
{ "TransN", new Vector3(0, 0, 0) },
{ "XRotN", new Vector3(0, 0, 0) },
{ "YRotN", new Vector3(0, 0, 0) },
{ "HipN", new Vector3(0, 0, 0) },
{ "WaistN", new Vector3(0, 0, 0) },
{ "LLegJA", new Vector3(-1.570796f, 0, -1.570796f) },
{ "LLegJ", new Vector3(-0.0001849213f, -0.007395464f, 0.01190592f) },
{ "LKneeJ", new Vector3(0, 0, 0.01745588f) },
{ "LFootJA", new Vector3(0, 0, -1.570796f) },
{ "LFootJ", new Vector3(-0.01625728f, -0.003122046f, 0.04150072f) },
{ "RLegJA", new Vector3(-1.570796f, 0, -1.570796f) },
{ "RLegJ", new Vector3(-0.0003749531f, 0.01237885f, 0.01931265f) },
{ "RKneeJ", new Vector3(0, 0, 0.01745588f) },
{ "RFootJA", new Vector3(0, 0, -1.570796f) },
{ "RFootJ", new Vector3(0.04340675f, 0.006765825f, 0.03351802f) },
{ "BustN", new Vector3(0, 0, 0) },
{ "LShoulderN", new Vector3(0, 0, 0) },
{ "LShoulderJA", new Vector3(-1.570796f, 0, 0) },
{ "LShoulderJ", new Vector3(-0.0008534141f, -0.0001975292f, 0.0004313609f) },
{ "LArmJ", new Vector3(0, 0, -0.01745588f) },
{ "LHandN", new Vector3(0, 0, -0.001827065f) },
{ "L1stNa", new Vector3(0, -0.0299967f, 0) },
{ "L1stNb", new Vector3(0, 0, 0) },
{ "L2ndNa", new Vector3(0, -0.0299967f, 0) },
{ "L2ndNb", new Vector3(0, 0, 0) },
{ "L3rdNa", new Vector3(0, -0.0299967f, 0) },
{ "L3rdNb", new Vector3(0, 0, 0) },
{ "L4thNa", new Vector3(0, -0.0299967f, 0) },
{ "L4thNb", new Vector3(0, 0, 0) },
{ "LHaveN", new Vector3(0, 0, -0.001827065f) },
{ "LThumbNa", new Vector3(0.02769301f, 0.03113901f, -0.3017421f) },
{ "LThumbNb", new Vector3(0, 0, 0.159777f) },
{ "NeckN", new Vector3(0, 0, 0) },
{ "HeadN", new Vector3(0, 0, 0) },
{ "RShoulderN", new Vector3(0, 0, 0) },
{ "RShoulderJA", new Vector3(-1.570796f, 0, 3.141592f) },
{ "RShoulderJ", new Vector3(-0.0001569438f, -0.008347717f, 0.01229164f) },
{ "RArmJ", new Vector3(0, 0, -0.0213731f) },
{ "RHandN", new Vector3(0, 0, 0) },
{ "R1stNa", new Vector3(0, 0, 0) },
{ "R1stNb", new Vector3(0, 0, 0) },
{ "R2ndNa", new Vector3(0, 0, 0) },
{ "R2ndNb", new Vector3(0, 0, 0) },
{ "R3rdNa", new Vector3(0, 0, 0) },
{ "R3rdNb", new Vector3(0, 0, 0) },
{ "R4thNa", new Vector3(0, 0, 0) },
{ "R4thNb", new Vector3(0, 0, 0) },
{ "RHaveN", new Vector3(0, -1.570796f, 3.127518f) },
{ "RThumbNa", new Vector3(-0.006011998f, -0.006951f, -0.3686029f) },
{ "RThumbNb", new Vector3(0, 0, 0.1282514f) },
{ "ThrowN", new Vector3(0, 0, 0) },
{ "Extra", new Vector3(0, 0, 0) }};

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="newWorldMatrices"></param>
        /// <param name="root"></param>
        /// <param name="parentTransform"></param>
        private static void ZeroOutRotations(ProcessingCache cache, Dictionary<HSD_JOBJ, Matrix4> newWorldMatrices, HSD_JOBJ root, Matrix4 oldParent, Matrix4 parentTransform)
        {
            var targetPoint = Vector3.TransformPosition(Vector3.Zero, cache.jobjToWorldTransform[root]);

            //targetPoint -= Vector3.TransformPosition(Vector3.Zero, oldParent);
            var trimName = root.ClassName.Replace("Armature_", "");
            
            if (FighterDefaults.ContainsKey(trimName))
            {
                root.TX = 0;
                root.TY = 0;
                root.TZ = 0;
                root.RX = FighterDefaults[trimName].X;
                root.RY = FighterDefaults[trimName].Y;
                root.RZ = FighterDefaults[trimName].Z;
                root.SX = 1;
                root.SY = 1;
                root.SZ = 1;
            }
            else
            {
                root.TX = 0;
                root.TY = 0;
                root.TZ = 0;
                root.RX = 0;
                root.RY = 0;
                root.RZ = 0;
                root.SX = 1;
                root.SY = 1;
                root.SZ = 1;
            }

            Matrix4 currentTransform =
                Matrix4.CreateScale(root.SX, root.SY, root.SZ) *
                Matrix4.CreateFromQuaternion(Math3D.FromEulerAngles(root.RZ, root.RY, root.RX)) *
                parentTransform;
            
            var relPoint = Vector3.TransformPosition(targetPoint, parentTransform.Inverted());

            root.TX = relPoint.X;
            root.TY = relPoint.Y;
            root.TZ = relPoint.Z;
            
            if (trimName.Equals("TransN")) // special case
            {
                root.TX = 0;
                root.TY = 0;
                root.TZ = 0;
            }

            var finalTransform = 
                Matrix4.CreateScale(root.SX, root.SY, root.SZ) *
                Matrix4.CreateFromQuaternion(Math3D.FromEulerAngles(root.RZ, root.RY, root.RX)) *
                Matrix4.CreateTranslation(root.TX, root.TY, root.TZ) * parentTransform;

            newWorldMatrices.Add(root, finalTransform);

            foreach (var c in root.Children)
                ZeroOutRotations(cache, newWorldMatrices, c, cache.jobjToWorldTransform[root], finalTransform);
        }

        /// <summary>
        /// Recursivly processing nodes and convert data into JOBJ
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private static HSD_JOBJ RecursiveProcess(ProcessingCache cache, ModelImportSettings settings, Scene scene, Node node)
        {
            if (node.Name.EndsWith("_end"))
                return null;

            // node transform's translation is bugged
            Vector3D tr, s;
            Assimp.Quaternion q;
            node.Transform.Decompose(out s, out q, out tr);
            var translation = new Vector3(tr.X, tr.Y, tr.Z);
            var scale = new Vector3(s.X, s.Y, s.Z);
            var rotation = Math3D.ToEulerAngles(new OpenTK.Quaternion(q.X, q.Y, q.Z, q.W).Inverted());

            if (settings.SetScaleToOne)
                scale = Vector3.One;

            translation *= settings.Scale;

            var t = Matrix4.CreateScale(scale) 
                * Matrix4.CreateFromQuaternion(new OpenTK.Quaternion(q.X, q.Y, q.Z, q.W)) 
                * Matrix4.CreateTranslation(translation);

            HSD_JOBJ jobj = new HSD_JOBJ();

            if (node.Parent != null)
            {
                t = t * cache.jobjToWorldTransform[cache.NameToJOBJ[node.Parent.Name]];
            }
            cache.NameToJOBJ.Add(node.Name, jobj);
            cache.jobjToWorldTransform.Add(jobj, t);
            cache.jobjToInverseTransform.Add(jobj, t.Inverted());

            jobj.ClassName = node.Name;
            jobj.Flags = JOBJ_FLAG.CLASSICAL_SCALING;
            jobj.TX = translation.X;
            jobj.TY = translation.Y;
            jobj.TZ = translation.Z;
            jobj.RX = rotation.X;
            jobj.RY = rotation.Y;
            jobj.RZ = rotation.Z;
            jobj.SX = scale.X;
            jobj.SY = scale.Y;
            jobj.SZ = scale.Z;

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
        /// <returns></returns>
        private static HSD_DOBJ GetMeshes(ProcessingCache cache, ModelImportSettings settings, Scene scene, Node node)
        {
            HSD_DOBJ root = null;
            HSD_DOBJ prev = null;

            Console.WriteLine("Processing " + node.Name);

            foreach (int index in node.MeshIndices)
            {
                Mesh mesh = scene.Meshes[index];
                var material = scene.Materials[mesh.MaterialIndex];

                Console.WriteLine(mesh.Name + " " + material.Name);

                // Generate DOBJ
                HSD_DOBJ dobj = new HSD_DOBJ();

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
                if (mesh.HasBones)
                {
                    Attributes.Add(GXAttribName.GX_VA_PNMTXIDX);

                    foreach (var v in mesh.Bones)
                    {
                        var jobj = cache.NameToJOBJ[v.Name];

                        if (!cache.EnvelopedJOBJs.Contains(jobj))
                            cache.EnvelopedJOBJs.Add(jobj);

                        if (v.HasVertexWeights)
                            foreach (var vw in v.VertexWeights)
                            {
                                if (jobjs[vw.VertexID] == null)
                                    jobjs[vw.VertexID] = new List<HSD_JOBJ>();
                                if (weights[vw.VertexID] == null)
                                    weights[vw.VertexID] = new List<float>();
                                if(vw.Weight > 0)
                                {
                                    jobjs[vw.VertexID].Add(jobj);
                                    weights[vw.VertexID].Add(vw.Weight);
                                }
                            }
                    }
                }

                if (hasReflection)
                    Attributes.Add(GXAttribName.GX_VA_TEX0MTXIDX);

                if (mesh.HasVertices)
                    Attributes.Add(GXAttribName.GX_VA_POS);
                
                if (mesh.HasVertexColors(0) && settings.ShadingType == ShadingType.VertexColor)
                    Attributes.Add(GXAttribName.GX_VA_CLR0);

                //if (mesh.HasVertexColors(1) && settings.ImportVertexColors)
                //    Attributes.Add(GXAttribName.GX_VA_CLR1);

                if (!hasBump && mesh.HasNormals && settings.ShadingType == ShadingType.Material)
                    Attributes.Add(GXAttribName.GX_VA_NRM);

                if(hasBump)
                    Attributes.Add(GXAttribName.GX_VA_NBT);

                if (mesh.HasTextureCoords(0) && !hasReflection)
                    Attributes.Add(GXAttribName.GX_VA_TEX0);

                //if (mesh.HasTextureCoords(1))
                //    Attributes.Add(GXAttribName.GX_VA_TEX1);

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
                        //throw new NotSupportedException($"Non triangle primitive types not supported at this time {faceMode}");
                    }

                    for (int i = 0; i < face.IndexCount; i++)
                    {
                        int indicie = face.Indices[i];

                        GX_Vertex vertex = new GX_Vertex();

                        if (mesh.HasBones)
                        {
                            if (jobjs[indicie] == null) //  unbound verts
                            {
                                jobjs[indicie] = new List<HSD_JOBJ>();
                                weights[indicie] = new List<float>();
                            }

                            jobjList.Add(jobjs[indicie].ToArray());
                            wList.Add(weights[indicie].ToArray());

                            // Single Binds Get Inverted
                            var tkvert = new Vector3(mesh.Vertices[indicie].X, mesh.Vertices[indicie].Y, mesh.Vertices[indicie].Z) * settings.Scale;
                            var tknrm = new Vector3(mesh.Normals[indicie].X, mesh.Normals[indicie].Y, mesh.Normals[indicie].Z);

                            Vector3 tktan = Vector3.Zero;
                            Vector3 tkbitan = Vector3.Zero;

                            if (mesh.HasTangentBasis)
                            {
                                tktan = new Vector3(mesh.Tangents[indicie].X, mesh.Tangents[indicie].Y, mesh.Tangents[indicie].Z);
                                tkbitan = new Vector3(mesh.BiTangents[indicie].X, mesh.BiTangents[indicie].Y, mesh.BiTangents[indicie].Z);
                            }

                            if (jobjs[indicie].Count == 1 || (weights[indicie].Count > 0 && weights[indicie][0] == 1))
                            {
                                tkvert = Vector3.TransformPosition(tkvert, cache.jobjToInverseTransform[jobjs[indicie][0]]);
                                tknrm = Vector3.TransformNormal(tknrm, cache.jobjToInverseTransform[jobjs[indicie][0]]);
                                
                                if (mesh.HasTangentBasis)
                                {
                                    tktan = Vector3.TransformNormal(tktan, cache.jobjToInverseTransform[jobjs[indicie][0]]);
                                    tkbitan = Vector3.TransformNormal(tkbitan, cache.jobjToInverseTransform[jobjs[indicie][0]]);
                                }
                            }

                            vertex.POS = GXTranslator.fromVector3(tkvert);
                            vertex.NRM = GXTranslator.fromVector3(tknrm);
                            vertex.TAN = GXTranslator.fromVector3(tktan);
                            vertex.BITAN = GXTranslator.fromVector3(tkbitan);
                        }
                        else
                        {
                            if (mesh.HasVertices)
                                vertex.POS = new GXVector3(mesh.Vertices[indicie].X * settings.Scale, mesh.Vertices[indicie].Y * settings.Scale, mesh.Vertices[indicie].Z * settings.Scale) ;
                            
                            if (mesh.HasNormals)
                                vertex.NRM = new GXVector3(mesh.Normals[indicie].X, mesh.Normals[indicie].Y, mesh.Normals[indicie].Z);

                            if (mesh.HasTangentBasis)
                            {
                                vertex.TAN = new GXVector3(mesh.Tangents[indicie].X, mesh.Tangents[indicie].Y, mesh.Tangents[indicie].Z);
                                vertex.BITAN = new GXVector3(mesh.BiTangents[indicie].X, mesh.BiTangents[indicie].Y, mesh.BiTangents[indicie].Z);
                            }
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

                        if (mesh.HasVertexColors(0))
                            vertex.CLR0 = new GXColor4(
                                mesh.VertexColorChannels[0][indicie].R,
                                mesh.VertexColorChannels[0][indicie].G,
                                mesh.VertexColorChannels[0][indicie].B,
                                settings.ImportVertexAlpha ? mesh.VertexColorChannels[0][indicie].A : 1);

                        if (mesh.HasVertexColors(1))
                            vertex.CLR0 = new GXColor4(
                                mesh.VertexColorChannels[1][indicie].R,
                                mesh.VertexColorChannels[1][indicie].G,
                                mesh.VertexColorChannels[1][indicie].B,
                                settings.ImportVertexAlpha ? mesh.VertexColorChannels[1][indicie].A : 1);

                        vertices.Add(vertex);
                    }

                }

                HSD_POBJ pobj = null;
                if (mesh.HasBones)
                {
                    pobj = cache.POBJGen.CreatePOBJsFromTriangleList(vertices, Attributes.ToArray(), jobjList, wList);
                }
                else
                    pobj = cache.POBJGen.CreatePOBJsFromTriangleList(vertices, Attributes.ToArray(), null);
                if(pobj != null)
                {
                    if (dobj.Pobj == null)
                        dobj.Pobj = pobj;
                    else
                        dobj.Pobj.Add(pobj);
                }

            }

            return root;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="material"></param>
        /// <returns></returns>
        private static HSD_MOBJ GenerateMaterial(ProcessingCache cache, ModelImportSettings settings, Material material)
        {
            var Mobj = new HSD_MOBJ();
            Mobj.Material = new HSD_Material();
            Mobj.Material.AmbientColorRGBA = 0x7F7F7FFF;
            Mobj.Material.DiffuseColorRGBA = 0xFFFFFFFF;
            Mobj.Material.SpecularColorRGBA = 0xFFFFFFFF;
            Mobj.Material.Shininess = 1;
            Mobj.Material.Alpha = 1;
            Mobj.RenderFlags = RENDER_MODE.ALPHA_COMPAT | RENDER_MODE.DIFFUSE_VTX;

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

                    /// special mobj loading
                    if(Path.GetExtension(texturePath).ToLower() == ".mobj")
                    {
                        var dat = new HSDRaw.HSDRawFile(texturePath);
                        Mobj._s = dat.Roots[0].Data._s;
                        return Mobj;
                    }
                    else
                    if (File.Exists(texturePath))
                    {
                        Mobj.RenderFlags |= RENDER_MODE.TEX0;

                        var tobj = TOBJConverter.ImportTOBJFromFile(texturePath, settings.TextureFormat, settings.PaletteFormat);
                        tobj.Flags = TOBJ_FLAGS.LIGHTMAP_DIFFUSE | TOBJ_FLAGS.COORD_UV;

                        if(settings.ShadingType == ShadingType.VertexColor || settings.ShadingType == ShadingType.Material)
                        {
                            tobj.Flags |= TOBJ_FLAGS.COLORMAP_MODULATE;
                        }
                        else
                        {
                            tobj.Flags |= TOBJ_FLAGS.COLORMAP_REPLACE;
                        }

                        tobj.GXTexGenSrc = 4;
                        tobj.TexMapID = GXTexMapID.GX_TEXMAP0;

                        tobj.WrapS = ToGXWrapMode(material.TextureDiffuse.WrapModeU);
                        tobj.WrapT = ToGXWrapMode(material.TextureDiffuse.WrapModeV);

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