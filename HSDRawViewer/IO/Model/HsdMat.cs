using HSDRaw.Common;
using HSDRaw.GX;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace HSDRawViewer.IO.Model
{
    public enum HsdCullMode
    {
        NONE,
        FRONT,
        BACK,
    }

    public class HsdPixelProcessing
    {
        public PIXEL_PROCESS_ENABLE Flags { get; set; } = (PIXEL_PROCESS_ENABLE)25;

        public byte AlphaRef0 { get; set; }

        public byte AlphaRef1 { get; set; }

        public byte DestinationAlpha { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GXBlendMode BlendMode { get; set; } = GXBlendMode.GX_BLEND;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GXBlendFactor SrcFactor { get; set; } = GXBlendFactor.GX_BL_SRCALPHA;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GXBlendFactor DstFactor { get; set; } = GXBlendFactor.GX_BL_INVSRCALPHA;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GXLogicOp BlendOp { get; set; } = GXLogicOp.GX_LO_SET;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GXCompareType DepthFunction { get; set; } = GXCompareType.LEqual;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GXCompareType AlphaComp0 { get; set; } = GXCompareType.Always;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GXAlphaOp AlphaOp { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GXCompareType AlphaComp1 { get; set; } = GXCompareType.Always;

        public HsdPixelProcessing() { }

        public HsdPixelProcessing(HSD_PEDesc d)
        {
            Flags = d.Flags;

            AlphaRef0 = d.AlphaRef0;
            AlphaRef1 = d.AlphaRef1;

            DestinationAlpha = d.DestinationAlpha;

            BlendMode = d.BlendMode;
            SrcFactor = d.SrcFactor;
            DstFactor = d.DstFactor;
            BlendOp = d.BlendOp;

            DepthFunction = d.DepthFunction;

            AlphaComp0 = d.AlphaComp0;
            AlphaOp = d.AlphaOp;
            AlphaComp1 = d.AlphaComp1;
        }

        public HSD_PEDesc ToPEDesc()
        {
            return new HSD_PEDesc()
            {
                Flags = Flags,
                AlphaRef0 = AlphaRef0,
                AlphaRef1 = AlphaRef1,
                DestinationAlpha = DestinationAlpha,
                BlendMode = BlendMode,
                SrcFactor = SrcFactor,
                DstFactor = DstFactor,
                BlendOp = BlendOp,
                DepthFunction = DepthFunction,
                AlphaComp0 = AlphaComp0,
                AlphaComp1 = AlphaComp1,
                AlphaOp = AlphaOp,
            };
        }
    }

    public class HsdMat
    {
        [JsonPropertyName("enable_constant")]
        public bool EnableConstant { get; set; } = false;

        [JsonPropertyName("enable_diffuse")]
        public bool EnableDiffuse { get; set; } = false;

        [JsonPropertyName("enable_specular")]
        public bool EnableSpecular { get; set; } = false;

        [JsonPropertyName("enable_vertex")]
        public bool EnableVertex { get; set; } = false;

        [JsonPropertyName("enable_xlu")]
        public bool EnableXLU { get; set; } = false;

        [JsonPropertyName("disable_z")]
        public bool DisableZUpdate { get; set; } = false;

        public HsdColor Ambient { get; set; } = new HsdColor(1f, 1f, 1f, 1f);

        public HsdColor Diffuse { get; set; } = new HsdColor(1f, 1f, 1f, 1f);

        public HsdColor Specular { get; set; } = new HsdColor(1f, 1f, 1f, 1f);

        public float Shininess { get; set; } = 50;

        public float Alpha { get; set; } = 1;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public HsdCullMode Culling { get; set; } = HsdCullMode.FRONT;

        public List<HsdTex> Textures { get; set; } = new List<HsdTex>();

        public HsdPixelProcessing Transparency { get; set; }

        public HsdMat()
        {

        }

        public HsdMat(HSD_MOBJ mobj)
        {
            EnableConstant  = mobj.RenderFlags.HasFlag(RENDER_MODE.CONSTANT);
            EnableDiffuse   = mobj.RenderFlags.HasFlag(RENDER_MODE.DIFFUSE);
            EnableSpecular  = mobj.RenderFlags.HasFlag(RENDER_MODE.SPECULAR);
            EnableVertex    = mobj.RenderFlags.HasFlag(RENDER_MODE.VERTEX);
            EnableXLU       = mobj.RenderFlags.HasFlag(RENDER_MODE.XLU);
            DisableZUpdate  = mobj.RenderFlags.HasFlag(RENDER_MODE.NO_ZUPDATE);

            Ambient         = new HsdColor(mobj.Material.AmbientColor);
            Diffuse         = new HsdColor(mobj.Material.DiffuseColor);
            Specular        = new HsdColor(mobj.Material.SpecularColor);
            Shininess       = mobj.Material.Shininess;
            Alpha           = mobj.Material.Alpha;

            if (mobj.PEDesc != null)
                Transparency = new HsdPixelProcessing(mobj.PEDesc);

            if (mobj.Textures != null)
            {
                foreach (var tex in mobj.Textures.List)
                {
                    Textures.Add(new HsdTex(tex));
                }
            }
        }

        internal HSD_MOBJ ToMObj(HsdImportHelper imp)
        {
            RENDER_MODE flags = 0;
            if (EnableConstant)
                flags |= RENDER_MODE.CONSTANT;
            if (EnableDiffuse)
                flags |= RENDER_MODE.DIFFUSE;
            if (EnableSpecular)
                flags |= RENDER_MODE.SPECULAR;
            if (EnableVertex)
                flags |= RENDER_MODE.VERTEX;
            if (EnableXLU)
                flags |= RENDER_MODE.XLU;
            if (DisableZUpdate)
                flags |= RENDER_MODE.NO_ZUPDATE;

            for (int i = 0; i < Textures.Count; i++)
            {
                if (i <= 7)
                    flags |= (RENDER_MODE)(1 << (4 + i));
            }

            var tex = Textures.Select(e => e.ToTObj(imp)).ToArray();
            for (int i = 1; i < tex.Length; i++)
                tex[0].Add(tex[i]);

            return new HSD_MOBJ()
            {
                RenderFlags = flags,
                Material = new HSD_Material()
                {
                    AmbientColor = Ambient.ToColor(),
                    DiffuseColor = Diffuse.ToColor(),
                    SpecularColor = Specular.ToColor(),
                    Shininess = Shininess,
                    Alpha = Alpha,
                },
                Textures = tex.Length > 0 ? tex[0] : null,
                PEDesc = Transparency?.ToPEDesc(),
            };
        }
    }
}
