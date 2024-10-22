using System;
using System.IO;
using HSDRaw.Tools;
using HSDRaw.GX;
using System.ComponentModel;
using IONET.Core.Model;
using System.Linq;
using System.Drawing;
using YamlDotNet.Serialization;

namespace HSDRawViewer.Converters
{
    public enum ImportNormalSettings
    {
        No,
        Yes,
        YesIfNoVertexColor
    }
    /// <summary>
    /// 
    /// </summary>
    public class MeshImportSettings
    {
        [YamlIgnore, Browsable(false)]
        public IOMesh _poly { get; internal set; }

        [Category("1. Mesh Info"), DisplayName("Name"), Description("")]
        public string Name { get; internal set; }

        [Category("1. Mesh Info"), DisplayName("Polygon Count"), Description(""), YamlIgnore]
        public int PolyCount => _poly.Polygons.Count;

        [Category("1. Mesh Info"), DisplayName("Materials Used"), Description(""), YamlIgnore]
        public string Materials => String.Join(", ", _poly.Polygons.Select(e => e.MaterialName));


        [Category("2. Importing Options"), DisplayName("Single Bind"), Description("Imports this model to single joint. This mesh will be skinned to and appear under only that joint.")]
        public bool SingleBind { get; set; } = false;

        [Category("2. Importing Options"), DisplayName("Single Bind Joint Name"), Description("Name of the joint this mesh is bound to")]
        public string SingleBindJoint { get; internal set; }

        [Category("2. Importing Options"), DisplayName("Import Normals"), Description("Imports normals into model. Normals are used for lighting calcuations and reflections.")]
        public ImportNormalSettings ImportNormals { get; set; } = ImportNormalSettings.No;

        [Category("2. Importing Options"), DisplayName("Is Reflective"), Description("")]
        public bool IsReflective { get; set; } = false;

        [Category("2. Importing Options"), DisplayName("Generate Tangent/Bitangent"), Description("This is mainly used for BUMP mapping")]
        public bool GenerateTB { get; set; } = false;


        [Category("3. Vertex Color Options"), DisplayName("Import Vertex Colors"), Description("Enables importing of vertex colors")]
        public bool ImportVertexColor { get; set; } = false;


        [Category("4. Additional Options"), DisplayName("Flip UVs"), Description("Flips UVs on the Y axis, useful if textures are upside down")]
        public bool FlipUVs { get; set; } = true;

        [Category("4. Additional Options"), DisplayName("Flip Faces"), Description("Flips direction of faces, useful if model imports inside out")]
        public bool FlipFaces { get; set; } = false;

        [Category("4. Additional Options"), DisplayName("Flip Normals"), Description("Flips direction of normals, useful if model is all black with textures")]
        public bool InvertNormals { get; set; } = false;

        public bool IsDummy = false;

        public int ForceTextureCount = -1;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="jointName"></param>
        /// <returns></returns>
        public bool IsSingleBound(out string jointName)
        {
            jointName = null;

            foreach (var v in _poly.Vertices)
            {
                if (v.Envelope != null)
                    foreach (var e in v.Envelope.Weights)
                    {
                        // weight is not 1 so not single bound
                        if (e.Weight != 1)
                        {
                            jointName = null;
                            return false;
                        }

                        // first joint found
                        if (jointName == null)
                        {
                            jointName = e.BoneName;
                        }

                        // different bone found so not single bound
                        if (!jointName.Equals(e.BoneName))
                        {
                            jointName = null;
                            return false;
                        }
                    }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jointName"></param>
        /// <returns></returns>
        public bool UseVertexColor(int set)
        {
            foreach (var v in _poly.Vertices)
            {
                if (v.Colors[set].X != 1 || v.Colors[set].Y != 1 || v.Colors[set].Z != 1 || v.Colors[set].W != 1)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public MeshImportSettings()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public MeshImportSettings(IOMesh mesh)
        {
            SetMesh(mesh);

            Name = mesh.Name;

            if (mesh == null)
                return;

            IsReflective = mesh.Name.Contains("REFLECTIVE");

            GenerateTB = mesh.Name.Contains("BUMP");

            if (mesh.HasNormals)
                ImportNormals = ImportNormalSettings.Yes;

            if (mesh.HasColorSet(0))
            {
                ImportVertexColor = UseVertexColor(0);

                if (mesh.HasNormals)
                    ImportNormals = ImportNormalSettings.YesIfNoVertexColor;
            }

            if (mesh.Name.Contains("SINGLE"))
            {
                if (IsSingleBound(out string joint))
                {
                    SingleBind = true;
                    SingleBindJoint = joint;
                }
            }

            //if (!string.IsNullOrEmpty(poly.ParentBone.Name))
            //{
            //    SingleBind = true;
            //    SingleBindJoint = poly.ParentBone.Name;
            //}
            //else
            //if (IsSingleBound(out string joint2))
            //{
            //    SingleBind = true;
            //    SingleBindJoint = joint2;
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public void SetMesh(IOMesh mesh)
        {
            this._poly = mesh;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MaterialImportSettings
    {
        [YamlIgnore, Browsable(false)]
        public IOMaterial _material { get; internal set; }

        public class SerialColor
        {
            public byte R { get; set; }
            public byte G { get; set; }
            public byte B { get; set; }
            public byte A { get; set; }

            public SerialColor()
            {

            }

            public SerialColor(Color c)
            {
                R = c.R;
                B = c.B;
                G = c.G;
                A = c.A;
            }

            public Color ToColor()
            {
                return Color.FromArgb(A, R, G, B);
            }
        }


        [Category("Material"), DisplayName("Name"), Description("")]
        public string Name { get; internal set; }


        [Category("Material Params"), DisplayName("Import Material Params"), Description("Imports the material info from model file.")]
        public bool ImportMaterialInfo { get; set; } = false;

        [Category("Material Params"), DisplayName("Ambient Color"), Description("Color of ambient light"), YamlIgnore]
        public Color AmbientColor { get; set; } = Color.FromArgb(0xFF, 0x7f, 0x7f, 0x7f);

        [Category("Material Params"), DisplayName("Diffuse Color"), Description("Color of diffuse light"), YamlIgnore]
        public Color DiffuseColor { get; set; } = Color.White;

        [Category("Material Params"), DisplayName("Specular Color"), Description("Color of specular light"), YamlIgnore]
        public Color SpecularColor { get; set; } = Color.White;

        [Browsable(false)]
        public SerialColor ambient { get => new SerialColor(AmbientColor); set => AmbientColor = value.ToColor(); }
        [Browsable(false)]
        public SerialColor diffuse { get => new SerialColor(DiffuseColor); set => DiffuseColor = value.ToColor(); }
        [Browsable(false)]
        public SerialColor specular { get => new SerialColor(SpecularColor); set => SpecularColor = value.ToColor(); }

        [Category("Material Params"), DisplayName("Alpha"), Description("Material alpha transparency")]
        public float Alpha { get; set; } = 1;

        [Category("Material Params"), DisplayName("Shininess"), Description("The power of specular color used to give shine")]
        public float Shininess { get; set; } = 50;


        [Category("MObj Import Options"), DisplayName("Path to MObj"), Description(""), YamlIgnore]
        public string MobjPath { get => _material != null && _material.DiffuseMap != null ? $"{Path.GetFileNameWithoutExtension(_material.DiffuseMap.FilePath)}.mobj" : null; }

        //[Category("MObj Import Options"), DisplayName("MObj Found"), Description("")]
        //public bool MobjFound { get => _material.DiffuseMap != null ? File.Exists($"{Path.GetFileNameWithoutExtension(_material.DiffuseMap.FilePath)}.mobj") : false; }

        [Category("MObj Import Options"), DisplayName("Import MObj"), Description("Imports .mobj files from file if found")]
        public bool ImportMOBJ { get; set; } = true;


        [Category("Material Options"), DisplayName("Enable Diffuse"), Description("Enables DIFFUSE flag on materials.")]
        public bool EnableDiffuse { get; set; } = true;

        [Category("Material Options"), DisplayName("Enable Constant"), Description("Enables CONSTANT flag on materials. Material will have constant color.")]
        public bool EnableConstant { get; set; } = false;


        [Category("Texture Options"), DisplayName("Import Textures"), Description("Imports textures from model file if they exist")]
        public bool ImportTexture { get; set; } = true;

        [Category("Texture Options"), DisplayName("Diffuse Texture"), Description(""), YamlIgnore]
        public string DiffuseTexture
        {
            get
            {
                if (_material == null)
                    return null;

                if (_material.DiffuseMap != null)
                    return _material.DiffuseMap.Name;
                return null;
            }
            set
            {
                if (_material == null)
                    return;

                if (_material.DiffuseMap == null)
                    _material.DiffuseMap = new IOTexture();

                _material.DiffuseMap.Name = value;
            }
        }

        [Category("Texture Options"), DisplayName("Specular Texture"), Description(""), YamlIgnore]
        public string SpecularTexture
        {
            get
            {
                if (_material == null)
                    return null;

                if (_material.SpecularMap != null)
                    return _material.SpecularMap.Name;
                return null;
            }
            set
            {
                if (_material == null)
                    return;

                if (_material.SpecularMap == null)
                    _material.SpecularMap = new IOTexture();

                _material.SpecularMap.Name = value;
            }
        }

        [Category("Texture Options"), DisplayName("Texture Format"), Description("The format to store the texture data in")]
        public GXTexFmt TextureFormat { get; set; } = GXTexFmt.CMP;

        [Category("Texture Options"), DisplayName("Palette Format"), Description("Palette format used with CI8 and CI4")]
        public GXTlutFmt PaletteFormat { get; set; } = GXTlutFmt.RGB565;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        private static Color Vector4ToColor(System.Numerics.Vector4 vec)
        {
            return Color.FromArgb((byte)(vec.W * 255), (byte)(vec.X * 255), (byte)(vec.Y * 255), (byte)(vec.Z * 255));
        }

        /// <summary>
        /// 
        /// </summary>
        public MaterialImportSettings()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="poly"></param>
        public MaterialImportSettings(IOMaterial material)
        {
            SetMaterial(material);

            if (material == null)
                return;

            Name = material.Name;

            AmbientColor = Vector4ToColor(material.AmbientColor);
            DiffuseColor = Vector4ToColor(material.DiffuseColor);
            SpecularColor = Vector4ToColor(material.SpecularColor);
            Alpha = material.Alpha;
            Shininess = material.Shininess;

            // assume texture format from texture name
            if (material.DiffuseMap != null)
            {
                if (!string.IsNullOrEmpty(material.DiffuseMap.Name))
                {
                    var found = ((GXTexFmt[])Enum.GetValues(typeof(GXTexFmt)))
                        .Where(e => material.DiffuseMap.Name.Contains(e.ToString()))
                        .OrderBy(e => material.DiffuseMap.Name.IndexOf(e.ToString()));

                    if (found.Count() > 0)
                    {
                        TextureFormat = found.First();

                        if (GXImageConverter.IsPalettedFormat(TextureFormat))
                        {
                            var palfmt = ((GXTlutFmt[])Enum.GetValues(typeof(GXTlutFmt)))
                                .Where(e => material.DiffuseMap.Name.Contains(e.ToString()))
                                .OrderBy(e => material.DiffuseMap.Name.IndexOf(e.ToString()));

                            if (palfmt.Count() > 0)
                                PaletteFormat = palfmt.First();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="material"></param>
        public void SetMaterial(IOMaterial material)
        {
            this._material = material;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ModelImportSettings
    {
#if DEBUG
        [Category("Debug Options"), DisplayName("Merge"), Description("")]
        public bool Merge { get; set; } = false;

        [Category("Debug Options"), DisplayName("Metal Model"), Description("")]
        public bool MetalModel { get; set; } = false;

        [Category("Debug Options"), DisplayName("Clean Root Node"), Description("")]
        public bool CleanRoot { get; set; } = false;

        [Category("Debug Options"), DisplayName("Melee-ify"), Description("")]
        public bool Meleeify { get; set; } = false;
#endif

        [Category("Importing Options"), DisplayName("Classical Scaling"), Description("Leave this true if you don't know what it is.")]
        public bool ClassicalScaling { get; set; } = true;

        [Category("Importing Options"), DisplayName("Import Bone Names"), Description("Stores bone names in JOBJs; not recommended")]
        public bool ImportBoneNames { get; set; } = false;

        [Category("Importing Options"), DisplayName("Import Mesh Names"), Description("Stores mesh names in DOBJs; not recommended")]
        public bool ImportMeshNames { get; set; } = false;

        [Category("Importing Options"), DisplayName("Use Triangle Strips"), Description("Slower to import, but significantly better optimized for game")]
        public bool UseStrips { get; set; } = true;
        
        [Category("Importing Options"), DisplayName("Import Skinning"), Description("Import skin data")]
        public bool ImportSkinning { get; set; } = true;

        [Category("Importing Options"), DisplayName("Smooth Normals"), Description("Applies normal smoothing")]
        public bool SmoothNormals { get; set; } = false;

        [Category("Importing Options"), DisplayName("Envelope All Joints"), Description("")]
        public bool EnvelopeAll { get; set; } = false;

        [Category("Importing Options"), DisplayName("Vertex Color Format"), Description("The format to import vertex colors as. Select a format with A for alpha.")]
        public GXCompTypeClr VertexColorFormat { get; set; } = GXCompTypeClr.RGB565;
    }
}