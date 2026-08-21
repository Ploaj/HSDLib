using HSDRaw.Common;
using HSDRaw.GX;
using HSDRawViewer.Rendering.GX;
using IONET.Core.Model;
using SharpGLTF.Memory;
using SharpGLTF.Schema2;
using System;
using System.Numerics;

namespace HSDRawViewer.IO.GLTF
{
    public static class GTLFMaterialConverter
    {
        public static (IOMaterial, HSD_MOBJ) ImportMaterial(Material mat)
        {
            var iomat = new IOMaterial()
            {
                Name = mat.Name,
                DiffuseColor = mat.GetDiffuseColor(Vector4.One),
            };

            //var diffuseTexture = mat.GetDiffuseTexture();
            //if (diffuseTexture != null &&
            //    diffuseTexture.PrimaryImage != null)
            //{
            //    var content = diffuseTexture.PrimaryImage.Content;

            //    if (content.IsPng)
            //    {
            //        iomat.DiffuseMap = new IOTexture()
            //        {
            //            Name = diffuseTexture.PrimaryImage.Name,
            //            FileBlob = content.Content.ToArray(),
            //            Type = ImageFileType.PNG
            //        };
            //    }
            //    else if (!string.IsNullOrEmpty(content.SourcePath))
            //    {
            //        iomat.DiffuseMap = new IOTexture()
            //        {
            //            FilePath = content.SourcePath,
            //        };
            //    }
            //}

            return (iomat, GenerateMObj(mat));
        }

        private static HSD_MOBJ GenerateMObj(Material mat)
        {
            var mobj = new HSD_MOBJ()
            {
                Material = new HSD_Material()
                {
                    AMB_A = 0xFF,
                    AMB_R = 0x7F,
                    AMB_G = 0x7F,
                    AMB_B = 0x7F,
                    DiffuseColor = ConvertColor(mat.GetDiffuseColor(Vector4.One)),
                    SpecularColor = System.Drawing.Color.White,
                    Alpha = 1,
                    Shininess = 20,
                },
                RenderFlags = RENDER_MODE.DIFFUSE,
            };

            var diffuseTexture = mat.GetDiffuseTexture();
            if (diffuseTexture != null &&
                diffuseTexture.PrimaryImage != null)
            {
                var content = diffuseTexture.PrimaryImage.Content;

                if (content.IsPng)
                {
                    var pngData = content.Content.ToArray();

                    var tobj = new HSD_TOBJ()
                    {
                        MagFilter = GXTexFilter.GX_LINEAR,
                        Flags = TOBJ_FLAGS.COORD_UV | TOBJ_FLAGS.LIGHTMAP_DIFFUSE | TOBJ_FLAGS.COLORMAP_MODULATE | TOBJ_FLAGS.ALPHAMAP_MODULATE,
                        RepeatT = 1,
                        RepeatS = 1,
                        WrapS = GXWrapMode.CLAMP,
                        WrapT = GXWrapMode.CLAMP,
                        SX = 1,
                        SY = 1,
                        SZ = 1,
                        GXTexGenSrc = GXTexGenSrc.GX_TG_TEX0,
                        Blending = 1,
                    };

                    tobj.FromPNG(pngData, GXTexFmt.CMP, GXTlutFmt.RGB5A3);
                    mobj.Textures = tobj;

                    mobj.SetFlag(RENDER_MODE.TEX0, true);
                }
            }

            return mobj;
        }

        public static Material ToMaterial(ModelRoot model, IOMaterial s, HSD_MOBJ mobj)
        {
            var material = model.CreateMaterial(s.Name);

            material.InitializePBRMetallicRoughness();

            var channel = material.FindChannel("BaseColor").Value;

            var materialColor = mobj.Material;
            Vector4 ambientColor = ConvertColor(materialColor.AmbientColor);
            Vector4 diffuseColor = ConvertColor(materialColor.DiffuseColor);
            Vector4 specularColor = ConvertColor(materialColor.SpecularColor);
            float shininess = materialColor.Shininess;
            float alpha = materialColor.Alpha;

            channel.Color = s.DiffuseColor; // ?? does texture override this; it's supposed to be multiplied by

            if (mobj.Textures != null)
            {
                foreach (var t in mobj.Textures.List)
                {
                    if (t.DiffuseLightmap)
                    {
                        // this map is supposed to be multiplied by diffuse color
                        var texture = channel.SetTexture(
                            texCoord: GetUVChannel(t),
                            primaryImg: CreateTexture(model, t),
                            ws: ToGLTFWrapMode(t.WrapS),
                            wt: ToGLTFWrapMode(t.WrapT),
                            mag: ToGLTFMagFilter(t.MagFilter));

                        ExportUVTransform(channel, t);

                    }
                    if (t.AmbientLightmap)
                    {
                        // TODO: this map is supposed to be multiplied by ambient color
                    }
                    if (t.SpecularLightmap)
                    {
                        // TODO: this map is supposed to be multiplied by specular color
                    }
                    if (t.ExtLightmap)
                    {
                        // TODO: this gets added to the end
                    }
                    if (t.BumpMap)
                    {
                        // TODO: this is more accurately an emboss map and is black and white
                    }
                }
            }

            return material;
        }

        private static TextureInterpolationFilter ToGLTFMagFilter(GXTexFilter magFilter)
        {
            return magFilter switch
            {
                GXTexFilter.GX_NEAR =>
                    TextureInterpolationFilter.NEAREST,

                GXTexFilter.GX_LINEAR =>
                    TextureInterpolationFilter.LINEAR,

                GXTexFilter.GX_LIN_MIP_NEAR =>
                    TextureInterpolationFilter.LINEAR,

                GXTexFilter.GX_NEAR_MIP_NEAR =>
                    TextureInterpolationFilter.NEAREST,

                GXTexFilter.GX_LIN_MIP_LIN =>
                    TextureInterpolationFilter.LINEAR,

                GXTexFilter.GX_NEAR_MIP_LIN =>
                    TextureInterpolationFilter.NEAREST,

                _ =>
                    TextureInterpolationFilter.LINEAR,
            };
        }

        private static int GetUVChannel(HSD_TOBJ tobj)
        {
            switch (tobj.GXTexGenSrc)
            {
                case GXTexGenSrc.GX_TG_TEX0:
                case GXTexGenSrc.GX_TG_TEX1:
                case GXTexGenSrc.GX_TG_TEX2:
                case GXTexGenSrc.GX_TG_TEX3:
                case GXTexGenSrc.GX_TG_TEX4:
                case GXTexGenSrc.GX_TG_TEX5:
                case GXTexGenSrc.GX_TG_TEX6:
                case GXTexGenSrc.GX_TG_TEX7:
                    return (int)(tobj.GXTexGenSrc - GXTexGenSrc.GX_TG_TEX0);
                case GXTexGenSrc.GX_TG_TEXCOORD0:
                case GXTexGenSrc.GX_TG_TEXCOORD1:
                case GXTexGenSrc.GX_TG_TEXCOORD2:
                case GXTexGenSrc.GX_TG_TEXCOORD3:
                case GXTexGenSrc.GX_TG_TEXCOORD4:
                case GXTexGenSrc.GX_TG_TEXCOORD5:
                case GXTexGenSrc.GX_TG_TEXCOORD6:
                    return (int)(tobj.GXTexGenSrc - GXTexGenSrc.GX_TG_TEXCOORD0);
            }
            return 0;
        }

        private static Vector4 ConvertColor(System.Drawing.Color c)
        {
            return new Vector4(c.R, c.G, c.B, c.A) / 255f;
        }

        private static System.Drawing.Color ConvertColor(Vector4 c)
        {
            return System.Drawing.Color.FromArgb(
                (byte)(Math.Clamp(c.W, 0f, 1f) * 255f),
                (byte)(Math.Clamp(c.X, 0f, 1f) * 255f),
                (byte)(Math.Clamp(c.Y, 0f, 1f) * 255f),
                (byte)(Math.Clamp(c.Z, 0f, 1f) * 255f));
        }

        private static Image CreateTexture(
            ModelRoot model,
            HSD_TOBJ tobj)
        {
            var image = new MemoryImage(tobj.ToPNG());

            var img = model.UseImageWithContent(image);

            return img;
        }

        private static void ExportUVTransform(
            MaterialChannel channel,
            HSD_TOBJ texture)
        {
            float offsetY = texture.TY;

            if (texture.WrapT == GXWrapMode.MIRROR)
            {
                offsetY +=
                    1f / (texture.RepeatT / texture.SY);
            }

            var offset = new Vector2(
                -texture.TX,
                -offsetY);

            var scale = new Vector2(
                Math.Abs(texture.SX) < float.Epsilon
                    ? 0
                    : texture.RepeatS / texture.SX,

                Math.Abs(texture.SY) < float.Epsilon
                    ? 0
                    : texture.RepeatT / texture.SY);

            channel.SetTransform(
                offset,
                scale,
                -texture.RZ);
        }

        private static void ImportUVTransform(
            MaterialChannel channel,
            HSD_TOBJ texture)
        {
            TextureTransform transform = channel.TextureTransform;

            texture.TX = -transform.Offset.X;

            float ty = -transform.Offset.Y;

            texture.SX = transform.Scale.X;
            texture.SY = transform.Scale.Y;

            texture.RepeatS = 1; //(byte)(transform.Scale.X * texture.SX);
            texture.RepeatT = 1; //(byte)(transform.Scale.Y * texture.SY);

            if (texture.WrapT == GXWrapMode.MIRROR)
            {
                ty -= texture.SY / texture.RepeatT;
            }

            texture.TY = ty;
            texture.RZ = -transform.Rotation;
        }

        private static TextureWrapMode ToGLTFWrapMode(
            GXWrapMode mode)
        {
            return mode switch
            {
                GXWrapMode.REPEAT =>
                    TextureWrapMode.REPEAT,

                GXWrapMode.MIRROR =>
                    TextureWrapMode.MIRRORED_REPEAT,

                GXWrapMode.CLAMP =>
                    TextureWrapMode.CLAMP_TO_EDGE,

                _ =>
                    TextureWrapMode.REPEAT
            };
        }
    }
}
