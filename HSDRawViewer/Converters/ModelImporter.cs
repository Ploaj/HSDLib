using System;
using System.Collections.Generic;
using HSDRaw.Common;
using OpenTK;
using HSDRawViewer.Rendering;
using System.IO;
using HSDRaw.Tools;
using HSDRaw.GX;
using System.ComponentModel;
using HSDRawViewer.GUI;
using IONET;
using IONET.Core.Skeleton;
using IONET.Core;
using IONET.Core.Model;
using System.Linq;
using System.Windows.Forms;

namespace HSDRawViewer.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public class ModelImportSettings
    {
        [Category("Importing Options"), DisplayName("Classical Scaling"), Description("Leave this true if you don't know what it is.")]
        public bool ClassicalScaling { get; set; } = true;

        [Category("Importing Options"), DisplayName("Flip Faces"), Description("Flips direction of faces, useful if model imports inside out")]
        public bool FlipFaces { get; set; } = false;

        [Category("Importing Options"), DisplayName("Flip UVs"), Description("Flips UVs on the Y axis, useful if textures are upside down")]
        public bool FlipUVs { get; set; } = false;

        [Category("Importing Options"), DisplayName("Flip Normals"), Description("Flips direction of normals, useful if model is all black with textures")]
        public bool InvertNormals { get; set; } = false;

        [Category("Importing Options"), DisplayName("Smooth Normals"), Description("Applies normal smoothing")]
        public bool SmoothNormals { get; set; } = false;
        

        [Category("Importing Options"), DisplayName("Import Bone Names"), Description("Stores bone names in JOBJs; not recommended")]
        public bool ImportBoneNames { get; set; } = false;

        [Category("Importing Options"), DisplayName("Import Mesh Names"), Description("Stores mesh names in DOBJs; not recommended")]
        public bool ImportMeshNames { get; set; } = false;


        [Category("Importing Options"), DisplayName("Use Triangle Strips"), Description("Slower to import, but significantly better optimized for game")]
        public bool UseStrips { get; set; } = true;
        
        [Category("Importing Options"), DisplayName("Import Rigging"), Description("Import rigging from model file")]
        public bool ImportRigging { get; set; } = true;

#if DEBUG
        [Category("Debug Options"), DisplayName("Merge"), Description("")]
        public bool Merge { get; set; } = false;

        [Category("Debug Options"), DisplayName("Metal Model"), Description("")]
        public bool MetalModel { get; set; } = false;
#endif


        [Category("Material Options"), DisplayName("Import MOBJs"), Description("Imports .mobj files from file if found")]
        public bool ImportMOBJ { get; set; } = false;

        [Category("Material Options"), DisplayName("Import Material Info"), Description("Imports the material info from model file. NOT recommended if you don't know what this is")]
        public bool ImportMaterialInfo { get; set; } = false;

        [Category("Material Options"), DisplayName("Import Normals"), Description("Imports normals from model file")]
        public bool ImportNormals { get; set; } = true;

        [Category("Material Options"), DisplayName("Enable Diffuse"), Description("Enables DIFFUSE flag on materials")]
        public bool EnableDiffuse { get; set; } = true;

        [Category("Material Options"), DisplayName("Enable Constant"), Description("Enables CONSTANT flag on materials. Material will have constant color.")]
        public bool EnableConstant { get; set; } = false;



        [Category("Vertex Color Options"), DisplayName("Import Vertex Colors"), Description("Enables importing of vertex colors")]
        public bool ImportVertexColor { get; set; } = false;

        [Category("Vertex Color Options"), DisplayName("Import Vertex Alpha"), Description("Import the alpha channel from vertex colors")]
        public bool ImportVertexAlpha { get; set; } = false;

        [Category("Vertex Color Options"), DisplayName("Multiply by 2"), Description("Multiplies vertex colors by 2")]
        public bool MultiplyVertexColorBy2 { get; set; } = false;



        [Category("Texture Options"), DisplayName("Import Textures"), Description("Imports textures from model file if they exist")]
        public bool ImportTexture { get; set; } = true;

        [Category("Texture Options"), DisplayName("Texture Format"), Description("The format to store the texture data in")]
        public GXTexFmt TextureFormat { get; set; } = GXTexFmt.CI8;

        [Category("Texture Options"), DisplayName("Palette Format"), Description("Palette format used with CI8 and CI4")]
        public GXTlutFmt PaletteFormat { get; set; } = GXTlutFmt.RGB565;
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
        
        // Indicates jobjs that need the SKELETON flag set along with inverted transform
        public List<HSD_JOBJ> EnvelopedJOBJs = new List<HSD_JOBJ>();
        
        // 
        public Dictionary<HSD_JOBJ, Matrix4> jobjToWorldTransform = new Dictionary<HSD_JOBJ, Matrix4>();

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
        private ImportSettings IOSettings;
        public HSD_JOBJ NewModel;
        ModelProcessCache _cache = new ModelProcessCache();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toReplace"></param>
        public static void ReplaceModelFromFile(HSD_JOBJ toReplace)
        {
            var f = Tools.FileIO.OpenFile(IOManager.GetModelImportFileFilter());

            if (f != null)
            {
                var settings = new ModelImportSettings();
                using (PropertyDialog d = new PropertyDialog("Model Import Options", settings))
                {
                    if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        ImportSettings ioSettings = new ImportSettings()
                        {
                            FlipUVs = settings.FlipUVs,
                            FlipWindingOrder = !settings.FlipFaces,
                            SmoothNormals = settings.SmoothNormals,
                            Triangulate = true,
                            //WeightLimit = true,
                        };

                        ModelImporter imp = new ModelImporter(f, settings, ioSettings);

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
        /// <param name="filePath"></param>
        /// <param name="settings"></param>
        /// <param name="iosettings"></param>
        public ModelImporter(string filePath, ModelImportSettings settings, ImportSettings iosettings)
        {
            FilePath = filePath;
            Settings = settings;
            IOSettings = iosettings;
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
        private void Import(BackgroundWorker w)
        {
            // settings
            if (Settings == null)
                Settings = new ModelImportSettings();

            if (IOSettings == null)
                IOSettings = new ImportSettings();

            _cache.POBJGen.UseTriangleStrips = Settings.UseStrips;
            _cache.POBJGen.UseVertexAlpha = Settings.ImportVertexAlpha;
            _cache.FolderPath = Path.GetDirectoryName(FilePath);


            // import model
            ProgressStatus = "Importing Model Data...";
            w.ReportProgress(0);
            var scene = IOManager.LoadScene(FilePath, IOSettings);

            var model = scene.Models[0];

            // process nodes
            ProgressStatus = "Processing Joints...";
            w.ReportProgress(30);
            HSD_JOBJ root = null;
            foreach (var r in model.Skeleton.RootBones)
            {
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


            // get root of skeleton
            root.Flags = JOBJ_FLAG.SKELETON_ROOT;


            // process mesh
            ProgressStatus = "Processing Mesh...";
            foreach (var mesh in model.Meshes)
            {
                ProcessMesh(scene, mesh, root);

                if (root.Dobj != null)
                {
                    ProgressStatus = $"Processing Mesh {root.Dobj.List.Count} {model.Meshes.Count + 1}...";
                    w.ReportProgress((int)(30 + 60 * (root.Dobj.List.Count / (float)model.Meshes.Count)));
                }
            }


            // set flags
            if (_cache.EnvelopedJOBJs.Count > 0)
                root.Flags |= JOBJ_FLAG.ENVELOPE_MODEL;

            // enable xlu if anything needs it
            if (_cache.HasXLU)
                root.Flags |= JOBJ_FLAG.XLU | JOBJ_FLAG.TEXEDGE;

            // just always enable opaque
            root.Flags |= JOBJ_FLAG.OPA;


            // calculate inverse binds
            foreach (var jobj in _cache.EnvelopedJOBJs)
            {
                ProgressStatus = "Generating Inverse Transforms...";
                if (_cache.jobjToWorldTransform.ContainsKey(jobj))
                    jobj.InverseWorldTransform = Matrix4ToHSDMatrix(_cache.jobjToWorldTransform[jobj].Inverted());
            }


            // SAVE POBJ buffers
            ProgressStatus = "Generating and compressing vertex buffers...";
            w.ReportProgress(90);
            _cache.POBJGen.SaveChanges();

            // done
            NewModel = root;

            // update flags
            JOBJTools.UpdateJOBJFlags(NewModel);

            if (Settings.ClassicalScaling)
                NewModel.Flags |= JOBJ_FLAG.CLASSICAL_SCALING;

#if DEBUG
            if (Settings.Merge)
                JOBJTools.MergeIntoOneObject(NewModel);
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

            if (Settings.ImportBoneNames)
                jobj.ClassName = bone.Name;

            Console.WriteLine(bone.Name + " " + bone.WorldTransform);

            _cache.NameToJOBJ.Add(bone.Name, jobj);
            _cache.jobjToWorldTransform.Add(jobj, MatrixNumericsToTKMatrix(bone.WorldTransform));

            foreach(var child in bone.Children)
            {
                jobj.AddChild(IOBoneToJOBJ(child));
            }

            return jobj;
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
        /// <param name="t"></param>
        /// <returns></returns>
        private static Matrix4 MatrixNumericsToTKMatrix(System.Numerics.Matrix4x4 t)
        {
            return new Matrix4(
                t.M11, t.M12, t.M13, t.M14,
                t.M21, t.M22, t.M23, t.M24,
                t.M31, t.M32, t.M33, t.M34,
                t.M41, t.M42, t.M43, t.M44);
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
        /// <param name="f"></param>
        /// <returns></returns>
        private static float RoundFloat(float f)
        {
            return (float)Math.Round(f, 4);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void ProcessMesh(IOScene scene, IOMesh mesh, HSD_JOBJ rootnode)
        {
            HSD_JOBJ parent = rootnode;

            HashSet<HSD_JOBJ> nodes = new HashSet<HSD_JOBJ>();
            foreach(var j in rootnode.BreathFirstList)
                nodes.Add(j);
            

            if (mesh.ParentBone != null && _cache.NameToJOBJ.ContainsKey(mesh.ParentBone.Name))
                parent = _cache.NameToJOBJ[mesh.ParentBone.Name];


            HSD_DOBJ root = null;
            HSD_DOBJ prev = null;

            //var skeleton = rootnode.BreathFirstList;
            Console.WriteLine("Processing " + mesh.Name);

            bool singleBinded = mesh.Name.Contains("SINGLE");

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

                if(Settings.ImportMeshNames)
                    dobj.ClassName = mesh.Name;

                if (root == null)
                    root = dobj;
                else
                    prev.Next = dobj;
                prev = dobj;


                // generate material
                var material = scene.Materials.Find(e => e.Name == poly.MaterialName);

                dobj.Mobj = GenerateMaterial(material);
                

                Console.WriteLine(mesh.Name + " " + material?.Name);

                
                // reflective mobjs do not use uvs
                var hasReflection = false;

                // bump maps need tangents and bitangents
                var hasBump = false;

                // Assess needed attributes based on the material MOBJ
                if (mesh.Name.Contains("REFLECTIVE") )
                    hasReflection = true;
#if DEBUG
                if(Settings.MetalModel)
                    hasReflection = true;
#endif

                if (mesh.Name.Contains("BUMP"))
                    hasBump = true;

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

                if (mesh.HasEnvelopes() && Settings.ImportRigging && !singleBinded)
                {
                    Attributes.Add(GXAttribName.GX_VA_PNMTXIDX);

                    if (hasReflection)
                    {
                        Attributes.Add(GXAttribName.GX_VA_TEX0MTXIDX);

                        if (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 1)
                            Attributes.Add(GXAttribName.GX_VA_TEX1MTXIDX);

#if DEBUG
                        if (Settings.MetalModel && !Attributes.Contains(GXAttribName.GX_VA_TEX1MTXIDX))
                            Attributes.Add(GXAttribName.GX_VA_TEX1MTXIDX);
#endif
                    }
                }

                
                Attributes.Add(GXAttribName.GX_VA_POS);


                if (hasBump)
                    Attributes.Add(GXAttribName.GX_VA_NBT);
                else
                if (mesh.HasNormals && Settings.ImportNormals)
                    Attributes.Add(GXAttribName.GX_VA_NRM);


                if (mesh.HasColorSet(0) && Settings.ImportVertexColor)
                    Attributes.Add(GXAttribName.GX_VA_CLR0);

                if (mesh.HasColorSet(1) && Settings.ImportVertexColor)
                    Attributes.Add(GXAttribName.GX_VA_CLR1);


                if (mesh.HasUVSet(0) && !hasReflection)
                    Attributes.Add(GXAttribName.GX_VA_TEX0);

                if ((mesh.HasUVSet(1) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 1)) && !hasReflection)
                    Attributes.Add(GXAttribName.GX_VA_TEX1);

                if ((mesh.HasUVSet(2) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 2)) && !hasReflection)
                    Attributes.Add(GXAttribName.GX_VA_TEX2);

                if ((mesh.HasUVSet(3) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 3)) && !hasReflection)
                    Attributes.Add(GXAttribName.GX_VA_TEX3);

                if ((mesh.HasUVSet(4) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 4)) && !hasReflection)
                    Attributes.Add(GXAttribName.GX_VA_TEX4);

                if ((mesh.HasUVSet(5) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 5)) && !hasReflection)
                    Attributes.Add(GXAttribName.GX_VA_TEX5);

                if ((mesh.HasUVSet(6) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 6)) && !hasReflection)
                    Attributes.Add(GXAttribName.GX_VA_TEX6);

                if ((mesh.HasUVSet(7) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 7)) && !hasReflection)
                    Attributes.Add(GXAttribName.GX_VA_TEX7);


                var vertices = new List<GX_Vertex>();
                var jobjList = new List<HSD_JOBJ[]>();
                var weightList = new List<float[]>();

                foreach (var face in poly.Indicies)
                {
                    var v = mesh.Vertices[face];

                    GX_Vertex vertex = new GX_Vertex();

                    var tkvert = new Vector3(v.Position.X, v.Position.Y, v.Position.Z);
                    var tknrm = new Vector3(v.Normal.X, v.Normal.Y, v.Normal.Z);
                    var tktan = new Vector3(v.Tangent.X, v.Tangent.Y, v.Tangent.Z);
                    var tkbitan = new Vector3(v.Binormal.X, v.Binormal.Y, v.Binormal.Z);
                    
                    var parentTransform = _cache.jobjToWorldTransform[parent].Inverted();
                    
                    if(_cache.jobjToWorldTransform[parent] != Matrix4.Identity)
                    {
                        tkvert = Vector3.TransformPosition(tkvert, parentTransform);
                        tknrm = Vector3.TransformNormal(tknrm, parentTransform).Normalized();
                        tktan = Vector3.TransformNormal(tktan, parentTransform).Normalized();
                        tkbitan = Vector3.TransformNormal(tkbitan, parentTransform).Normalized();
                    }


                    if (mesh.HasEnvelopes() && Settings.ImportRigging)
                    {
                        // create weighting lists
                        List<float> weight = new List<float>();
                        List<HSD_JOBJ> bones = new List<HSD_JOBJ>();

                        if(v.Envelope.Weights.Count == 0)
                        {
                            weight.Add(1);
                            bones.Add(rootnode);
                        }

                        if (v.Envelope.Weights.Count > 4)
                        {
                            throw new Exception($"Too many weights! {v.Envelope.Weights.Count} in {mesh.Name}");
                        }

                        foreach(var bw in v.Envelope.Weights)
                        {
                            // check if skeleton actually contains bone
                            if(nodes.Contains(_cache.NameToJOBJ[bw.BoneName]))
                            {
                                // add envelope
                                bones.Add(_cache.NameToJOBJ[bw.BoneName]);
                                weight.Add(bw.Weight);

                                // indicate enveloped jobjs
                                if (!_cache.EnvelopedJOBJs.Contains(_cache.NameToJOBJ[bw.BoneName]))
                                    _cache.EnvelopedJOBJs.Add(_cache.NameToJOBJ[bw.BoneName]);
                            }
                            else
                            {
                                throw new Exception($"Bone not found \"{bw.BoneName}\" Weight: {bw.Weight} in {mesh.Name}");
                            }
                        }

                        jobjList.Add(bones.ToArray());
                        weightList.Add(weight.ToArray());

                        // invert single binds
                        if (v.Envelope.Weights.Count == 1)
                        {
                            var inv = _cache.jobjToWorldTransform[_cache.NameToJOBJ[v.Envelope.Weights[0].BoneName]].Inverted();
                            tkvert = Vector3.TransformPosition(tkvert, inv);
                            tknrm = Vector3.TransformNormal(tknrm, inv).Normalized();
                            tktan = Vector3.TransformNormal(tknrm, inv).Normalized();
                            tkbitan = Vector3.TransformNormal(tknrm, inv).Normalized();
                        }
                    }
                    
                    vertex.POS = GXTranslator.fromVector3(tkvert);
                    vertex.NRM = GXTranslator.fromVector3(tknrm.Normalized());
                    vertex.TAN = GXTranslator.fromVector3(tktan);
                    vertex.BITAN = GXTranslator.fromVector3(tkbitan);

                    if (Settings.InvertNormals)
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
                            v.Colors[0].X * (Settings.MultiplyVertexColorBy2 ? 2 : 1),
                            v.Colors[0].Y * (Settings.MultiplyVertexColorBy2 ? 2 : 1),
                            v.Colors[0].Z * (Settings.MultiplyVertexColorBy2 ? 2 : 1),
                            Settings.ImportVertexAlpha ? v.Colors[0].W : 1);

                    if (mesh.HasColorSet(1))
                        vertex.CLR1 = new GXColor4(
                            v.Colors[1].X * (Settings.MultiplyVertexColorBy2 ? 2 : 1),
                            v.Colors[1].Y * (Settings.MultiplyVertexColorBy2 ? 2 : 1),
                            v.Colors[1].Z * (Settings.MultiplyVertexColorBy2 ? 2 : 1),
                            Settings.ImportVertexAlpha ? v.Colors[1].W : 1);

                    vertices.Add(vertex);
                }

                
                // generate pobjs
                HSD_POBJ pobj = null;

                if (mesh.HasEnvelopes() && Settings.ImportRigging && !singleBinded)
                    pobj = _cache.POBJGen.CreatePOBJsFromTriangleList(vertices, Attributes.ToArray(), jobjList, weightList);
                else
                    pobj = _cache.POBJGen.CreatePOBJsFromTriangleList(vertices, Attributes.ToArray(), null);

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
        private HSD_MOBJ GenerateMaterial(IOMaterial material)
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

            // detect and set flags
            if (Settings.ImportVertexColor)
                Mobj.RenderFlags |= RENDER_MODE.VERTEX;

            if (Settings.EnableDiffuse)
                Mobj.RenderFlags |= RENDER_MODE.DIFFUSE;

            if (Settings.EnableConstant)
                Mobj.RenderFlags |= RENDER_MODE.CONSTANT;

            // Properties
            if (material != null && Settings.ImportMaterialInfo)
            {
                Mobj.Material.Shininess = material.Shininess;
                Mobj.Material.Alpha = material.Alpha;

                Mobj.Material.AMB_R = (byte)(material.AmbientColor.X * 255);
                Mobj.Material.AMB_G = (byte)(material.AmbientColor.Y * 255);
                Mobj.Material.AMB_B = (byte)(material.AmbientColor.Z * 255);
                Mobj.Material.AMB_A = (byte)(material.AmbientColor.W * 255);

                Mobj.Material.DIF_R = (byte)(material.DiffuseColor.X * 255);
                Mobj.Material.DIF_G = (byte)(material.DiffuseColor.Y * 255);
                Mobj.Material.DIF_B = (byte)(material.DiffuseColor.Z * 255);
                Mobj.Material.DIF_A = (byte)(material.DiffuseColor.W * 255);

                Mobj.Material.SPC_R = (byte)(material.SpecularColor.X * 255);
                Mobj.Material.SPC_G = (byte)(material.SpecularColor.Y * 255);
                Mobj.Material.SPC_B = (byte)(material.SpecularColor.Z * 255);
                Mobj.Material.SPC_A = (byte)(material.SpecularColor.W * 255);
            }

            // Textures
            if(material != null && Settings.ImportTexture)
            {
                if (material.DiffuseMap != null && !string.IsNullOrEmpty(material.DiffuseMap.FilePath))
                {
                    var texturePath = material.DiffuseMap.FilePath;

                    if (texturePath.Contains("file://"))
                        texturePath = texturePath.Replace("file://", "");
                    
                    if (File.Exists(Path.Combine(_cache.FolderPath, texturePath)))
                        texturePath = Path.Combine(_cache.FolderPath, texturePath);
                    
                    if (File.Exists(material.DiffuseMap.FilePath))
                        texturePath = material.DiffuseMap.FilePath;

                    if (File.Exists(texturePath + ".png"))
                        texturePath = texturePath + ".png";


                    var mobjPath = Path.Combine(Path.GetDirectoryName(texturePath), Path.GetFileNameWithoutExtension(texturePath)) + ".mobj";
                    
                    if(Settings.ImportMOBJ && File.Exists(mobjPath))
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

                        var tobj = TOBJConverter.ImportTOBJFromFile(texturePath, Settings.TextureFormat, Settings.PaletteFormat);
                        tobj.Flags = TOBJ_FLAGS.LIGHTMAP_DIFFUSE | TOBJ_FLAGS.COORD_UV | TOBJ_FLAGS.COLORMAP_MODULATE;
                        
                        tobj.GXTexGenSrc = 4;
                        tobj.TexMapID = GXTexMapID.GX_TEXMAP0;

                        tobj.WrapS = ToGXWrapMode(material.DiffuseMap.WrapS);
                        tobj.WrapT = ToGXWrapMode(material.DiffuseMap.WrapT);

                        if (TOBJConverter.IsTransparent(tobj))
                        {
                            _cache.HasXLU = true;
                            Mobj.RenderFlags |= RENDER_MODE.XLU;
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
        private static GXWrapMode ToGXWrapMode(WrapMode mode)
        {
            switch (mode)
            {
                case WrapMode.CLAMP:
                    return GXWrapMode.CLAMP;
                case WrapMode.MIRROR:
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

    }
}