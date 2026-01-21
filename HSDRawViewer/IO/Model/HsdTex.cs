using HSDRaw.Common;
using HSDRaw.GX;
using HSDRawViewer.Extensions;
using System.Text.Json.Serialization;

namespace HSDRawViewer.IO.Model
{
    public class HsdTev
    {
        public HsdColor Konst { get; set; } = new HsdColor();

        public HsdColor Color0 { get; set; } = new HsdColor();

        public HsdColor Color1 { get; set; } = new HsdColor();


        [JsonPropertyName("color_tev")]
        public bool EnableColor { get; set; } = false;

        [JsonPropertyName("color_op")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TevColorOp ColorOp { get; set; } = TevColorOp.GX_TEV_ADD;

        [JsonPropertyName("color_bias")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TevBias ColorBias { get; set; } = TevBias.GX_TB_ZERO;

        [JsonPropertyName("color_scale")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TevScale ColorScale { get; set; } = TevScale.GX_CS_SCALE_1;

        [JsonPropertyName("color_clamp")]
        public bool ColorClamp { get; set; } = true;

        [JsonPropertyName("color_a")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TOBJ_TEV_CC ColorA { get; set; } = TOBJ_TEV_CC.GX_CC_ZERO;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("color_b")]
        public TOBJ_TEV_CC ColorB { get; set; } = TOBJ_TEV_CC.GX_CC_ZERO;

        [JsonPropertyName("color_c")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TOBJ_TEV_CC ColorC { get; set; } = TOBJ_TEV_CC.GX_CC_ZERO;

        [JsonPropertyName("color_d")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TOBJ_TEV_CC ColorD { get; set; } = TOBJ_TEV_CC.GX_CC_TEXC;



        [JsonPropertyName("alpha_tev")]
        public bool EnableAlpha { get; set; } = false;

        [JsonPropertyName("alpha_op")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TevAlphaOp AlphaOp { get; set; } = TevAlphaOp.GX_TEV_ADD;

        [JsonPropertyName("alpha_bias")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TevBias AlphaBias { get; set; } = TevBias.GX_TB_ZERO;

        [JsonPropertyName("alpha_scale")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TevScale AlphaScale { get; set; } = TevScale.GX_CS_SCALE_1;

        [JsonPropertyName("alpha_clamp")]
        public bool AlphaClamp { get; set; } = true;

        [JsonPropertyName("alpha_a")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TOBJ_TEV_CA AlphaA { get; set; } = TOBJ_TEV_CA.GX_CC_ZERO;

        [JsonPropertyName("alpha_b")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TOBJ_TEV_CA AlphaB { get; set; } = TOBJ_TEV_CA.GX_CC_ZERO;

        [JsonPropertyName("alpha_c")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TOBJ_TEV_CA AlphaC { get; set; } = TOBJ_TEV_CA.GX_CC_ZERO;

        [JsonPropertyName("alpha_d")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TOBJ_TEV_CA AlphaD { get; set; } = TOBJ_TEV_CA.GX_CC_TEXA;

        public HsdTev()
        { 
        }

        public HsdTev(HSD_TOBJ_TEV t)
        {
            Konst = new HsdColor(t.constant);
            Color0 = new HsdColor(t.tev0);
            Color1 = new HsdColor(t.tev1);

            EnableColor = t.active.HasFlag(TOBJ_TEVREG_ACTIVE.COLOR_TEV);
            ColorOp     = t.color_op;
            ColorBias   = t.color_bias;
            ColorScale  = t.color_scale;
            ColorClamp  = t.color_clamp;
            ColorA      = t.color_a_in;
            ColorB      = t.color_b_in;
            ColorC      = t.color_c_in;
            ColorD      = t.color_d_in;

            EnableAlpha = t.active.HasFlag(TOBJ_TEVREG_ACTIVE.ALPHA_TEV);
            AlphaOp     = t.alpha_op;
            AlphaBias   = t.alpha_bias;
            AlphaScale  = t.alpha_scale;
            AlphaClamp  = t.alpha_clamp;
            AlphaA      = t.alpha_a_in;
            AlphaB      = t.alpha_b_in;
            AlphaC      = t.alpha_c_in;
            AlphaD      = t.alpha_d_in;
        }

        internal HSD_TOBJ_TEV ToTev()
        {
            var tev = new HSD_TOBJ_TEV()
            {
                constant = Konst.ToColor(),
                tev0 = Color0.ToColor(),
                tev1 = Color1.ToColor(),

                color_op = ColorOp,
                color_bias = ColorBias,
                color_scale = ColorScale,
                color_clamp = ColorClamp,
                color_a_in = ColorA,
                color_b_in = ColorB,
                color_c_in = ColorC,
                color_d_in = ColorD,

                alpha_op = AlphaOp,
                alpha_bias = AlphaBias,
                alpha_scale = AlphaScale,
                alpha_clamp = AlphaClamp,
                alpha_a_in = AlphaA,
                alpha_b_in = AlphaB,
                alpha_c_in = AlphaC,
                alpha_d_in = AlphaD,
            };

            if (EnableColor)
                tev.active |= TOBJ_TEVREG_ACTIVE.COLOR_TEV;

            if (EnableAlpha)
                tev.active |= TOBJ_TEVREG_ACTIVE.ALPHA_TEV;

            tev.AutoFlag();

            return tev;
        }
    }

    public class HsdTex
    {
        public string File { get; set; }

        [JsonPropertyName("fmt")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GXTexFmt ImageFormat { get; set; }

        [JsonPropertyName("fmt_pal")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GXTlutFmt TlutFormat { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GXTexMapID TexMapID { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("uv_src")]
        public GXTexGenSrc UVSource { get; set; }

        [JsonPropertyName("repeats")]
        public byte RepeatS { get; set; }

        [JsonPropertyName("repeatt")]
        public byte RepeatT { get; set; }


        [JsonPropertyName("tx")]
        public float TX { get; set; }

        [JsonPropertyName("ty")]
        public float TY { get; set; }

        [JsonPropertyName("tz")]
        public float TZ { get; set; }

        [JsonPropertyName("rx")]
        public float RX { get; set; }

        [JsonPropertyName("ry")]
        public float RY { get; set; }

        [JsonPropertyName("rz")]
        public float RZ { get; set; }

        [JsonPropertyName("sx")]
        public float SX { get; set; }

        [JsonPropertyName("sy")]
        public float SY { get; set; }

        [JsonPropertyName("sz")]
        public float SZ { get; set; }


        [JsonPropertyName("is_ambient")]
        public bool AmbientLightmap { get; set; }

        [JsonPropertyName("is_diffuse")]
        public bool DiffuseLightmap { get; set; }

        [JsonPropertyName("is_specular")]
        public bool SpecularLightmap { get; set; }

        [JsonPropertyName("is_shadow")]
        public bool ShadowLightmap { get; set; }

        [JsonPropertyName("is_ext")]
        public bool ExtLightmap { get; set; }


        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("uv_coord")]
        public COORD_TYPE CoordType { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("color_pass")]
        public COLORMAP ColorOperation { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("alpha_pass")]
        public ALPHAMAP AlphaOperation { get; set; }

        public bool BumpMap { get; set; }

        [JsonPropertyName("wraps")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GXWrapMode WrapS { get; set; }

        [JsonPropertyName("wrapt")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GXWrapMode WrapT { get; set; }

        [JsonPropertyName("blending")]
        public float Blending { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("mag_filter")]
        public GXTexFilter MagFilter { get; set; }

        public HsdTev tev { get; set; } = null;

        public HsdTex()
        {

        }

        public HsdTex(HSD_TOBJ tobj)
        {
            File = $"t{ImporterExtensions.ComputeHash(tobj.ImageData.ImageData).ToString("X8")}.png";
            ImageFormat = tobj.ImageData.Format;
            if (tobj.TlutData != null)
                TlutFormat = tobj.TlutData.Format;

            TexMapID    = tobj.TexMapID;
            UVSource    = tobj.GXTexGenSrc;
            RepeatS     = tobj.RepeatS;
            RepeatT     = tobj.RepeatT;

            TX = tobj.TX;
            TY = tobj.TY;
            TZ = tobj.TZ;
            RX = tobj.RX;
            RY = tobj.RY;
            RZ = tobj.RZ;
            SX = tobj.SX;
            SY = tobj.SY;
            SZ = tobj.SZ;

            DiffuseLightmap     = tobj.DiffuseLightmap;
            SpecularLightmap    = tobj.SpecularLightmap;
            AmbientLightmap     = tobj.AmbientLightmap;
            ShadowLightmap      = tobj.ShadowLightmap;
            ExtLightmap         = tobj.ExtLightmap;

            CoordType           = tobj.CoordType;
            ColorOperation      = tobj.ColorOperation;
            AlphaOperation      = tobj.AlphaOperation;
            BumpMap             = tobj.BumpMap;

            WrapS               = tobj.WrapS;
            WrapT               = tobj.WrapT;

            Blending            = tobj.Blending;

            MagFilter           = tobj.MagFilter;

            if (tobj.TEV != null)
                tev = new HsdTev(tobj.TEV);
        }

        internal HSD_TOBJ ToTObj(HsdImportHelper imp)
        {
            HSD_Image img = null;
            HSD_Tlut tlut = null;
            if (imp.TextureLookup.ContainsKey(File))
            {
                var t = imp.TextureLookup[File];
                img = t.ImageData;
                tlut = t.TlutData;
            }
            else
            {
                img = new HSD_Image()
                {
                    Format = GXTexFmt.I4,
                    Width = 8,
                    Height = 8,
                    ImageData = new byte[8 * 8 / 2],
                };
            }

            return new HSD_TOBJ()
            {
                ImageData = img,
                TlutData = tlut,

                TexMapID = TexMapID,
                GXTexGenSrc = UVSource,
                RepeatS = RepeatS,
                RepeatT = RepeatT,
                TX = TX,
                TY = TY,
                TZ = TZ,
                RX = RX,
                RY = RY,
                RZ = RX,
                SX = SX,
                SY = SY,
                SZ = SZ,
                DiffuseLightmap = DiffuseLightmap,
                SpecularLightmap = SpecularLightmap,
                AmbientLightmap = AmbientLightmap,
                ShadowLightmap = ShadowLightmap,
                ExtLightmap = ExtLightmap,

                CoordType = CoordType,
                ColorOperation = ColorOperation,
                AlphaOperation = AlphaOperation,
                BumpMap = BumpMap,
                WrapS = WrapS,
                WrapT = WrapT,
                Blending = Blending,
                MagFilter = MagFilter,

                TEV = tev?.ToTev(),
            };
        }
    }
}
