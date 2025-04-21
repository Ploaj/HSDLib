using HSDRaw.Common;
using HSDRaw.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace HSDRawViewer.GUI.Controls.JObjEditor
{
    public class TextureAnimDesc
    {
        public List<HSD_TOBJ> Textures { get; internal set; } = new List<HSD_TOBJ>();

        public List<FOBJ_Player> Tracks { get; internal set; } = new List<FOBJ_Player>();
    }
    public enum CullMode
    {
        None,
        Front,
        Back,
        FrontAndBack
    }

    public class DObjProxy : MeshListItem
    {

        public int JOBJIndex;
        public int DOBJIndex;
        public HSD_JOBJ ParentJOBJ;
        public HSD_DOBJ DOBJ;

        public DObjProxy()
        {
        }

        [DisplayName("Class Name"), Category("General")]
        public string Name { get => DOBJ.ClassName; set => DOBJ.ClassName = value; }


        //[Editor(typeof(FlagEnumUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        //public RENDER_MODE MOBJFlags { get => DOBJ.Mobj.RenderFlags; set => DOBJ.Mobj.RenderFlags = value; }


        [DisplayName("Use Constant Color"), Category("Material Flags"), Description("")]
        public bool ConstantColor { get => DOBJ.Mobj.RenderFlags.HasFlag(RENDER_MODE.CONSTANT); set => DOBJ.Mobj.SetFlag(RENDER_MODE.CONSTANT, value); }

        [DisplayName("Use Vertex Color"), Category("Material Flags"), Description("Note: Melee is unable to use both vertex colors and diffuse lighting")]
        public bool VertexColor { get => DOBJ.Mobj.RenderFlags.HasFlag(RENDER_MODE.VERTEX); set => DOBJ.Mobj.SetFlag(RENDER_MODE.VERTEX, value); }


        [DisplayName("Enable Diffuse Shading"), Category("Material Flags"), Description("Note: Melee is unable to use both vertex colors and diffuse lighting")]
        public bool EnableDiffuse { get => DOBJ.Mobj.RenderFlags.HasFlag(RENDER_MODE.DIFFUSE); set => DOBJ.Mobj.SetFlag(RENDER_MODE.DIFFUSE, value); }

        [DisplayName("Enable Specular Shading"), Category("Material Flags"), Description("")]
        public bool EnableSpecular { get => DOBJ.Mobj.RenderFlags.HasFlag(RENDER_MODE.SPECULAR); set => DOBJ.Mobj.SetFlag(RENDER_MODE.SPECULAR, value); }


        [DisplayName("Shadow"), Category("Material Flags"), Description("Determines if this material is affected by shadows")]
        public bool UseShadow { get => DOBJ.Mobj.RenderFlags.HasFlag(RENDER_MODE.SHADOW); set => DOBJ.Mobj.SetFlag(RENDER_MODE.SHADOW, value); }


        [DisplayName("ZMode Always"), Category("Material Flags"), Description("This object will draw overtop of other objects regardless of depth")]
        public bool ZAlways { get => DOBJ.Mobj.RenderFlags.HasFlag(RENDER_MODE.ZMODE_ALWAYS); set => DOBJ.Mobj.SetFlag(RENDER_MODE.ZMODE_ALWAYS, value); }

        [DisplayName("No ZUpdate"), Category("Material Flags"), Description("This object will not update the depth buffer when drawn")]
        public bool NoZupdate { get => DOBJ.Mobj.RenderFlags.HasFlag(RENDER_MODE.NO_ZUPDATE); set => DOBJ.Mobj.SetFlag(RENDER_MODE.NO_ZUPDATE, value); }

        [DisplayName("Has Transparency"), Category("Material Flags"), Description("Indicates this material has transparent elements")]
        public bool XLU { get => DOBJ.Mobj.RenderFlags.HasFlag(RENDER_MODE.XLU); set => DOBJ.Mobj.SetFlag(RENDER_MODE.XLU, value); }

        //TOON = (1 << 12),
        //ALPHA_COMPAT = (0 << 13),
        //ALPHA_MAT = (1 << 13),
        //ALPHA_VTX = (2 << 13),
        //ALPHA_BOTH = (3 << 13),
        //ZOFST = (1 << 24),
        //EFFECT = (1 << 25),
        //DF_ALL = (1 << 28),

        [DisplayName("Ambient Color"), Category("Material Color"), Description("Color of the ambient light on this material")]
        public Color Ambient { get => DOBJ.Mobj.Material.AmbientColor; set => DOBJ.Mobj.Material.AmbientColor = value; }

        [DisplayName("Diffuse Color"), Category("Material Color"), Description("Color of the diffuse light on this material")]
        public Color Diffuse { get => DOBJ.Mobj.Material.DiffuseColor; set => DOBJ.Mobj.Material.DiffuseColor = value; }

        [DisplayName("Specular Color"), Category("Material Color"), Description("Color of the specular light on this material")]
        public Color Specular { get => DOBJ.Mobj.Material.SpecularColor; set => DOBJ.Mobj.Material.SpecularColor = value; }

        [DisplayName("Shinniness"), Category("Material Color"), Description("Power of the specular highlight")]
        public float Shinniness { get => DOBJ.Mobj.Material.Shininess; set => DOBJ.Mobj.Material.Shininess = value; }

        [DisplayName("Alpha"), Category("Material Color"), Description("Alpha transparency of material")]
        public float Alpha { get => DOBJ.Mobj.Material.Alpha; set => DOBJ.Mobj.Material.Alpha = value; }


        [DisplayName("Culling"), Category("Misc"), Description("")]
        public CullMode Culling
        {
            get
            {
                if (DOBJ.Pobj == null)
                    return CullMode.None;
                else
                {
                    if (DOBJ.Pobj.Flags.HasFlag(POBJ_FLAG.CULLBACK) && DOBJ.Pobj.Flags.HasFlag(POBJ_FLAG.CULLFRONT))
                        return CullMode.FrontAndBack;

                    if (DOBJ.Pobj.Flags.HasFlag(POBJ_FLAG.CULLBACK))
                        return CullMode.Back;

                    if (DOBJ.Pobj.Flags.HasFlag(POBJ_FLAG.CULLFRONT))
                        return CullMode.Front;

                    return CullMode.None;
                }
            }
            set
            {
                if (DOBJ.Pobj != null)
                    foreach (HSD_POBJ p in DOBJ.Pobj.List)
                    {
                        p.Flags &= ~(POBJ_FLAG.CULLBACK | POBJ_FLAG.CULLFRONT);
                        switch (value)
                        {
                            case CullMode.FrontAndBack:
                                p.Flags |= POBJ_FLAG.CULLBACK | POBJ_FLAG.CULLFRONT;
                                break;
                            case CullMode.Front:
                                p.Flags |= POBJ_FLAG.CULLFRONT;
                                break;
                            case CullMode.Back:
                                p.Flags |= POBJ_FLAG.CULLBACK;
                                break;
                        }
                    }
            }
        }

        [DisplayName("Enabled"), Category("Pixel Processing"), Description(""), RefreshProperties(RefreshProperties.All)]
        public bool EnablePP
        {
            get => DOBJ.Mobj.PEDesc != null;
            set
            {
                if (value)
                {
                    DOBJ.Mobj.PEDesc = _pixelProcess;
                }
                else
                {
                    DOBJ.Mobj.PEDesc = null;
                }
            }
        }

        [DisplayName("Pixel Processing Parameters"),
            Category("Pixel Processing"),
            TypeConverter(typeof(ExpandableObjectConverter))]
        public HSD_PEDesc PixelProcess { get => EnablePP ? _pixelProcess : null; set => _pixelProcess = value; }
        private HSD_PEDesc _pixelProcess;


        [Browsable(false)]
        public int PolygonCount { get => DOBJ.Pobj != null ? DOBJ.Pobj.List.Count : 0; }

        [Browsable(false)]
        public int TextureCount { get => DOBJ.Mobj == null ? -1 : (DOBJ.Mobj.Textures != null ? DOBJ.Mobj.Textures.List.Count : 0); }

        [Browsable(false)]
        public bool HasPixelProcessing { get => DOBJ.Mobj?.PEDesc != null; }

        [Browsable(false)]
        public bool HasTEV
        {
            get
            {
                if (DOBJ.Mobj != null && DOBJ.Mobj.Textures != null)
                {
                    if (DOBJ.Mobj.Textures.List.Any(e => e.TEV != null))
                        return true;
                }
                return false;
            }
        }

        [Browsable(false)]
        public bool HasMaterialColor { get => DOBJ.Mobj?.Material != null; }


        /// <summary>
        /// Animation Tracks
        /// </summary>
        [Browsable(false)]
        public List<FOBJ_Player> Tracks { get; internal set; } = new List<FOBJ_Player>();

        // 
        [Browsable(false)]
        public TextureAnimDesc[] TextureStates { get; internal set; } = new TextureAnimDesc[8];

        public float FrameCount
        {
            get
            {
                float fc = 0;
                if (Tracks.Count > 0)
                {
                    fc = Math.Max(fc, Tracks.Max(e => e.FrameCount));
                }
                foreach (TextureAnimDesc v in TextureStates)
                {
                    if (v.Tracks.Count > 0)
                        fc = Math.Max(fc, v.Tracks.Max(e => e.FrameCount));
                }
                return fc;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentJOBJ"></param>
        /// <param name="dOBJ"></param>
        /// <param name="jOBJIndex"></param>
        /// <param name="dOBJIndex"></param>
        public DObjProxy(HSD_JOBJ parentJOBJ, HSD_DOBJ dOBJ, int jOBJIndex, int dOBJIndex)
        {
            JOBJIndex = jOBJIndex;
            DOBJIndex = dOBJIndex;
            ParentJOBJ = parentJOBJ;
            DOBJ = dOBJ;

            // initialize texture state
            for (int i = 0; i < TextureStates.Length; i++)
                TextureStates[i] = new TextureAnimDesc();

            // initialize pixel processing data
            if (DOBJ != null && DOBJ.Mobj != null && DOBJ.Mobj.PEDesc != null)
            {
                PixelProcess = DOBJ.Mobj.PEDesc;
            }
            else
            {
                PixelProcess = new HSD_PEDesc()
                {
                    AlphaComp0 = HSDRaw.GX.GXCompareType.Always,
                    AlphaComp1 = HSDRaw.GX.GXCompareType.Always,
                    BlendMode = HSDRaw.GX.GXBlendMode.GX_BLEND,
                    BlendOp = HSDRaw.GX.GXLogicOp.GX_LO_SET,
                    DepthFunction = HSDRaw.GX.GXCompareType.LEqual,
                    SrcFactor = HSDRaw.GX.GXBlendFactor.GX_BL_SRCALPHA,
                    DstFactor = HSDRaw.GX.GXBlendFactor.GX_BL_INVSRCALPHA,
                    Flags = (PIXEL_PROCESS_ENABLE)25
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearAnimation()
        {
            // clear tracks
            Tracks.Clear();

            // clear texture anims
            foreach (TextureAnimDesc t in TextureStates)
            {
                t.Textures.Clear();
                t.Tracks.Clear();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Joint {JOBJIndex} : Object {DOBJIndex} : Polygons {PolygonCount} : Textures {TextureCount} {Name}";
        }
    }
}
