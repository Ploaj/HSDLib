using HSDRaw.Common;
using HSDRaw.GX;
using HSDRawViewer.Rendering.GX;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace HSDRawViewer.Rendering.Models
{
    public class MobjManager
    {
        private Dictionary<byte[], int> imageBufferTextureIndex = new Dictionary<byte[], int>();

        private TextureManager TextureManager = new TextureManager();

        /// <summary>
        /// 
        /// </summary>
        public void ClearRenderingCache()
        {
            TextureManager.ClearTextures();
            imageBufferTextureIndex.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public void PreLoadTexture(HSD_TOBJ tobj)
        {
            if (!imageBufferTextureIndex.ContainsKey(tobj.ImageData.ImageData))
            {
                var rawImageData = tobj.ImageData.ImageData;
                var width = tobj.ImageData.Width;
                var height = tobj.ImageData.Height;

                var rgbaData = tobj.GetDecodedImageData();

                var index = TextureManager.Add(rgbaData, width, height);

                imageBufferTextureIndex.Add(rawImageData, index);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mobj"></param>
        public void BindMOBJ(Shader shader, HSD_MOBJ mobj, HSD_JOBJ parentJOBJ, MatAnimManager animation)
        {
            if (mobj == null)
                return;

            GL.Enable(EnableCap.Texture2D);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Greater, 0f);

            // Pixel Processing
            shader.SetInt("alphaComp0", 7); // always
            shader.SetInt("alphaComp1", 7);

            var pp = mobj.PEDesc;
            if (pp != null)
            {
                GL.BlendFunc(GXTranslator.toBlendingFactor(pp.SrcFactor), GXTranslator.toBlendingFactor(pp.DstFactor));
                GL.DepthFunc(GXTranslator.toDepthFunction(pp.DepthFunction));

                shader.SetInt("alphaComp0", (int)pp.AlphaComp0);
                shader.SetInt("alphaComp1", (int)pp.AlphaComp1);
                shader.SetFloat("alphaRef0", pp.AlphaRef0 / 255f);
                shader.SetFloat("alphaRef1", pp.AlphaRef1 / 255f);
            }


            // Materials
            var color = mobj.Material;
            if (color != null)
            {
                if (animation != null)
                    color = animation.GetMaterialState(mobj);

                shader.SetVector4("ambientColor", color.AMB_R / 255f, color.AMB_G / 255f, color.AMB_B / 255f, color.AMB_A / 255f);
                shader.SetVector4("diffuseColor", color.DIF_R / 255f, color.DIF_G / 255f, color.DIF_B / 255f, color.DIF_A / 255f);
                shader.SetVector4("specularColor", color.SPC_R / 255f, color.SPC_G / 255f, color.SPC_B / 255f, color.SPC_A / 255f);
                shader.SetFloat("shinniness", color.Shininess);
                shader.SetFloat("alpha", color.Alpha);
            }

            var enableAll = mobj.RenderFlags.HasFlag(RENDER_MODE.DF_ALL);

            shader.SetBoolToInt("no_zupdate", mobj.RenderFlags.HasFlag(RENDER_MODE.NO_ZUPDATE));
            shader.SetBoolToInt("enableSpecular", parentJOBJ.Flags.HasFlag(JOBJ_FLAG.SPECULAR) && mobj.RenderFlags.HasFlag(RENDER_MODE.SPECULAR));
            shader.SetBoolToInt("enableDiffuse", parentJOBJ.Flags.HasFlag(JOBJ_FLAG.LIGHTING) && mobj.RenderFlags.HasFlag(RENDER_MODE.DIFFUSE));
            shader.SetBoolToInt("useConstant", mobj.RenderFlags.HasFlag(RENDER_MODE.CONSTANT));
            shader.SetBoolToInt("useVertexColor", mobj.RenderFlags.HasFlag(RENDER_MODE.VERTEX));

            // Textures
            shader.SetBoolToInt("hasTEX0", mobj.RenderFlags.HasFlag(RENDER_MODE.TEX0) || enableAll);
            shader.SetBoolToInt("hasTEX1", mobj.RenderFlags.HasFlag(RENDER_MODE.TEX1) || enableAll);
            shader.SetBoolToInt("hasTEX2", mobj.RenderFlags.HasFlag(RENDER_MODE.TEX2) || enableAll);
            shader.SetBoolToInt("hasTEX3", mobj.RenderFlags.HasFlag(RENDER_MODE.TEX3) || enableAll);

            shader.SetInt("BumpTexture", -1);

            //LoadTextureConstants(shader);

            // these are always uniform
            GL.Uniform1(GL.GetUniformLocation(shader.programId, "textures"), 4, new int[] { 0, 1, 2, 3 });

            // Bind Textures
            if (mobj.Textures != null)
            {
                var textures = mobj.Textures.List;
                for (int i = 0; i < textures.Count; i++)
                {
                    var tex = textures[i];
                    var displayTex = tex;

                    if (tex.ImageData == null)
                        continue;

                    var blending = tex.Blending;

                    var transform = Matrix4.CreateScale(tex.SX, tex.SY, tex.SZ) *
                        Matrix4.CreateFromQuaternion(Math3D.FromEulerAngles(tex.RZ, tex.RY, tex.RX)) *
                        Matrix4.CreateTranslation(tex.TX, tex.TY, tex.TZ);

                    if (tex.SY != 0 && tex.SX != 0 && tex.SZ != 0)
                        transform.Invert();

                    if (animation != null)
                    {
                        var state = animation.GetTextureAnimState(tex);
                        if (state != null)
                        {
                            displayTex = state.TOBJ;
                            blending = state.Blending;
                            transform = state.Transform;
                        }
                    }

                    // make sure texture is loaded
                    PreLoadTexture(displayTex);

                    // grab texture id
                    var texid = TextureManager.Get(imageBufferTextureIndex[displayTex.ImageData.ImageData]);

                    // set texture
                    GL.ActiveTexture(TextureUnit.Texture0 + i);
                    GL.BindTexture(TextureTarget.Texture2D, texid);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GXTranslator.toWrapMode(tex.WrapS));
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GXTranslator.toWrapMode(tex.WrapT));
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GXTranslator.toMagFilter(tex.MagFilter));

                    var wscale = tex.WScale;
                    var hscale = tex.HScale;

                    var mirrorX = tex.WrapS == GXWrapMode.MIRROR;
                    var mirrorY = tex.WrapT == GXWrapMode.MIRROR;

                    var flags = tex.Flags;

                    var lightType = 0; // ambient
                    if (flags.HasFlag(TOBJ_FLAGS.LIGHTMAP_DIFFUSE))
                        lightType = 1;
                    if (flags.HasFlag(TOBJ_FLAGS.LIGHTMAP_SPECULAR))
                        lightType = 2;
                    if (flags.HasFlag(TOBJ_FLAGS.LIGHTMAP_AMBIENT))
                        lightType = 3;
                    if (flags.HasFlag(TOBJ_FLAGS.LIGHTMAP_EXT))
                        lightType = 4;
                    if (flags.HasFlag(TOBJ_FLAGS.LIGHTMAP_SHADOW))
                        lightType = 5;

                    int coordType = (int)flags & 0xF;
                    int colorOP = ((int)flags >> 16) & 0xF;
                    int alphaOP = ((int)flags >> 20) & 0xF;

                    if (flags.HasFlag(TOBJ_FLAGS.BUMP))
                    {
                        shader.SetInt("BumpTexture", i);
                        lightType = 6;
                        colorOP = 4;
                    }

                    shader.SetInt($"TEX{i}.texIndex", i);
                    shader.SetInt($"TEX{i}.light_type", lightType);
                    shader.SetInt($"TEX{i}.color_operation", colorOP);
                    shader.SetInt($"TEX{i}.alpha_operation", alphaOP);
                    shader.SetInt($"TEX{i}.coord_type", coordType);
                    shader.SetFloat($"TEX{i}.blend", blending);
                    shader.SetBoolToInt($"TEX{i}.mirror_fix", mirrorY);
                    shader.SetVector2($"TEX{i}.uv_scale", wscale, hscale);
                    shader.SetMatrix4x4($"TEX{i}.transform", ref transform);

                    var tev = tex.TEV;
                    shader.SetBoolToInt($"hasTEX{i}Tev", tev != null);
                    if (tev != null)
                    {
                        shader.SetInt($"TEX{i}Tev.color_op", (int)tev.color_op);
                        shader.SetInt($"TEX{i}Tev.color_bias", (int)tev.color_bias);
                        shader.SetInt($"TEX{i}Tev.color_scale", (int)tev.color_scale);
                        shader.SetBoolToInt($"TEX{i}Tev.color_clamp", tev.color_clamp);
                        shader.SetInt($"TEX{i}Tev.color_a", (int)tev.color_a_in);
                        shader.SetInt($"TEX{i}Tev.color_b", (int)tev.color_b_in);
                        shader.SetInt($"TEX{i}Tev.color_c", (int)tev.color_c_in);
                        shader.SetInt($"TEX{i}Tev.color_d", (int)tev.color_d_in);

                        shader.SetInt($"TEX{i}Tev.alpha_op", (int)tev.alpha_op);
                        shader.SetInt($"TEX{i}Tev.alpha_bias", (int)tev.alpha_bias);
                        shader.SetInt($"TEX{i}Tev.alpha_scale", (int)tev.alpha_scale);
                        shader.SetBoolToInt($"TEX{i}Tev.alpha_clamp", tev.alpha_clamp);
                        shader.SetInt($"TEX{i}Tev.alpha_a", (int)tev.alpha_a_in);
                        shader.SetInt($"TEX{i}Tev.alpha_b", (int)tev.alpha_b_in);
                        shader.SetInt($"TEX{i}Tev.alpha_c", (int)tev.alpha_c_in);
                        shader.SetInt($"TEX{i}Tev.alpha_d", (int)tev.alpha_d_in);

                        shader.SetColor($"TEX{i}Tev.konst", tev.constant, tev.constantAlpha);
                        shader.SetColor($"TEX{i}Tev.tev0", tev.tev0, tev.tev0Alpha);
                        shader.SetColor($"TEX{i}Tev.tev1", tev.tev1, tev.tev1Alpha);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shader"></param>
        private static void LoadTextureConstants(Shader shader)
        {
            var transform = Matrix4.Identity;
            for (int i = 0; i < 4; i++)
            {
                shader.SetInt($"TEX{i}.texIndex", 0);
                shader.SetInt($"TEX{i}.light_type", 0);
                shader.SetInt($"TEX{i}.color_operation", 0);
                shader.SetInt($"TEX{i}.alpha_operation", 0);
                shader.SetInt($"TEX{i}.coord_type", 0);
                shader.SetFloat($"TEX{i}.blend", 0);
                shader.SetBoolToInt($"TEX{i}.mirror_fix", false);
                shader.SetVector2($"TEX{i}.uv_scale", 1, 1);
                shader.SetMatrix4x4($"TEX{i}.transform", ref transform);

                shader.SetBoolToInt($"hasTEX{i}Tev", false);
                shader.SetInt($"TEX{i}Tev.color_op", 0);
                shader.SetInt($"TEX{i}Tev.color_bias", 0);
                shader.SetInt($"TEX{i}Tev.color_scale", 0);
                shader.SetBoolToInt($"TEX{i}Tev.color_clamp", false);
                shader.SetInt($"TEX{i}Tev.color_a", 0);
                shader.SetInt($"TEX{i}Tev.color_b", 0);
                shader.SetInt($"TEX{i}Tev.color_c", 0);
                shader.SetInt($"TEX{i}Tev.color_d", 0);

                shader.SetInt($"TEX{i}Tev.alpha_op", 0);
                shader.SetInt($"TEX{i}Tev.alpha_bias", 0);
                shader.SetInt($"TEX{i}Tev.alpha_scale", 0);
                shader.SetBoolToInt($"TEX{i}Tev.alpha_clamp", false);
                shader.SetInt($"TEX{i}Tev.alpha_a", 0);
                shader.SetInt($"TEX{i}Tev.alpha_b", 0);
                shader.SetInt($"TEX{i}Tev.alpha_c", 0);
                shader.SetInt($"TEX{i}Tev.alpha_d", 0);

                shader.SetColor($"TEX{i}Tev.konst", System.Drawing.Color.White, 255);
                shader.SetColor($"TEX{i}Tev.tev0", System.Drawing.Color.White, 255);
                shader.SetColor($"TEX{i}Tev.tev1", System.Drawing.Color.White, 255);
            }
        }
    }
}
