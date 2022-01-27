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
        private static int MAX_TEX { get; } = 4;

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

                List<byte[]> mips = new List<byte[]>();

                if(tobj.LOD != null && tobj.ImageData.MaxLOD != 0)
                {
                    for(int i = 0; i < tobj.ImageData.MaxLOD - 1; i++)
                        mips.Add(tobj.GetDecodedImageData(i));
                }
                else
                {
                    mips.Add(tobj.GetDecodedImageData());
                }

                var index = TextureManager.Add(mips, width, height);

                imageBufferTextureIndex.Add(rawImageData, index);
            }
        }

        private static MatAnimMaterialState MaterialState = new MatAnimMaterialState();

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

            GL.DepthMask(!mobj.RenderFlags.HasFlag(RENDER_MODE.NO_ZUPDATE));

            // Pixel Processing
            shader.SetInt("alphaOp", -1); // none
            shader.SetInt("alphaComp0", 7); // always
            shader.SetInt("alphaComp1", 7);

            // Materials
            var color = mobj.Material;
            if (color != null)
            {
                MaterialState.Ambient.X = color.AMB_R / 255f;
                MaterialState.Ambient.Y = color.AMB_G / 255f;
                MaterialState.Ambient.Z = color.AMB_B / 255f;
                MaterialState.Ambient.W = color.AMB_A / 255f;

                MaterialState.Diffuse.X = color.DIF_R / 255f;
                MaterialState.Diffuse.Y = color.DIF_G / 255f;
                MaterialState.Diffuse.Z = color.DIF_B / 255f;
                MaterialState.Diffuse.W = color.DIF_A / 255f;

                MaterialState.Specular.X = color.SPC_R / 255f;
                MaterialState.Specular.Y = color.SPC_G / 255f;
                MaterialState.Specular.Z = color.SPC_B / 255f;
                MaterialState.Specular.W = color.SPC_A / 255f;

                MaterialState.Shininess = color.Shininess;
                MaterialState.Alpha = color.Alpha;

                if (animation != null)
                    animation.GetMaterialState(mobj, ref MaterialState);

                shader.SetVector4("ambientColor", MaterialState.Ambient);
                shader.SetVector4("diffuseColor", MaterialState.Diffuse);
                shader.SetVector4("specularColor", MaterialState.Specular);
                shader.SetFloat("shinniness", MaterialState.Shininess);
                shader.SetFloat("alpha", MaterialState.Alpha);
            }

            var pp = mobj.PEDesc;
            if (pp != null)
            {
                MaterialState.Ref0 = pp.AlphaRef0 / 255f;
                MaterialState.Ref1 = pp.AlphaRef1 / 255f;

                if (animation != null)
                    animation.GetMaterialState(mobj, ref MaterialState);

                GL.BlendFunc(GXTranslator.toBlendingFactor(pp.SrcFactor), GXTranslator.toBlendingFactor(pp.DstFactor));
                GL.DepthFunc(GXTranslator.toDepthFunction(pp.DepthFunction));

                shader.SetInt("alphaOp", (int)pp.AlphaOp);
                shader.SetInt("alphaComp0", (int)pp.AlphaComp0);
                shader.SetInt("alphaComp1", (int)pp.AlphaComp1);
                shader.SetFloat("alphaRef0", MaterialState.Ref0);
                shader.SetFloat("alphaRef1", MaterialState.Ref1);
            }

            var enableAll = mobj.RenderFlags.HasFlag(RENDER_MODE.DF_ALL);

            shader.SetBoolToInt("no_zupdate", mobj.RenderFlags.HasFlag(RENDER_MODE.NO_ZUPDATE));
            shader.SetBoolToInt("enableSpecular", parentJOBJ.Flags.HasFlag(JOBJ_FLAG.SPECULAR) && mobj.RenderFlags.HasFlag(RENDER_MODE.SPECULAR));
            shader.SetBoolToInt("enableDiffuse", parentJOBJ.Flags.HasFlag(JOBJ_FLAG.LIGHTING) && mobj.RenderFlags.HasFlag(RENDER_MODE.DIFFUSE));
            shader.SetBoolToInt("useConstant", mobj.RenderFlags.HasFlag(RENDER_MODE.CONSTANT));
            shader.SetBoolToInt("useVertexColor", mobj.RenderFlags.HasFlag(RENDER_MODE.VERTEX));
            shader.SetBoolToInt("useToonShading", mobj.RenderFlags.HasFlag(RENDER_MODE.TOON));

            // Textures
            for (int i = 0; i < MAX_TEX; i++)
                shader.SetBoolToInt($"hasTEX[{i}]", mobj.RenderFlags.HasFlag(RENDER_MODE.TEX0 + (i << 4)) || enableAll);

            shader.SetInt("BumpTexture", -1);

            //LoadTextureConstants(shader);

            // Bind Textures
            if (mobj.Textures != null)
            {
                var textures = mobj.Textures.List;
                for (int i = 0; i < textures.Count; i++)
                {
                    if (i > MAX_TEX)
                        break;

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

                    int coordType = (int)flags & 0xF;
                    int colorOP = ((int)flags >> 16) & 0xF;
                    int alphaOP = ((int)flags >> 20) & 0xF;

                    if (flags.HasFlag(TOBJ_FLAGS.BUMP))
                    {
                        colorOP = 4;
                    }

                    shader.SetInt($"TEX[{i}].texture", i);
                    shader.SetBoolToInt($"TEX[{i}].is_ambient", flags.HasFlag(TOBJ_FLAGS.LIGHTMAP_AMBIENT));
                    shader.SetBoolToInt($"TEX[{i}].is_diffuse", flags.HasFlag(TOBJ_FLAGS.LIGHTMAP_DIFFUSE));
                    shader.SetBoolToInt($"TEX[{i}].is_specular", flags.HasFlag(TOBJ_FLAGS.LIGHTMAP_SPECULAR));
                    shader.SetBoolToInt($"TEX[{i}].is_ext", flags.HasFlag(TOBJ_FLAGS.LIGHTMAP_EXT));
                    shader.SetBoolToInt($"TEX[{i}].is_bump", flags.HasFlag(TOBJ_FLAGS.BUMP));
                    shader.SetInt($"TEX[{i}].color_operation", colorOP);
                    shader.SetInt($"TEX[{i}].alpha_operation", alphaOP);
                    shader.SetInt($"TEX[{i}].coord_type", coordType);
                    shader.SetFloat($"TEX[{i}].blend", blending);
                    shader.SetBoolToInt($"TEX[{i}].mirror_fix", mirrorY);
                    shader.SetVector2($"TEX[{i}].uv_scale", wscale, hscale);
                    shader.SetMatrix4x4($"TEX[{i}].transform", ref transform);

                    var tev = tex.TEV;
                    bool useTev = tev != null && tev.active.HasFlag(TOBJ_TEVREG_ACTIVE.COLOR_TEV);
                    shader.SetBoolToInt($"hasTev[{i}]", useTev);
                    if (useTev)
                    {
                        shader.SetInt($"Tev[{i}].color_op", (int)tev.color_op);
                        shader.SetInt($"Tev[{i}].color_bias", (int)tev.color_bias);
                        shader.SetInt($"Tev[{i}].color_scale", (int)tev.color_scale);
                        shader.SetBoolToInt($"Tev[{i}].color_clamp", tev.color_clamp);
                        shader.SetInt($"Tev[{i}].color_a", (int)tev.color_a_in);
                        shader.SetInt($"Tev[{i}].color_b", (int)tev.color_b_in);
                        shader.SetInt($"Tev[{i}].color_c", (int)tev.color_c_in);
                        shader.SetInt($"Tev[{i}].color_d", (int)tev.color_d_in);

                        shader.SetInt($"Tev[{i}].alpha_op", (int)tev.alpha_op);
                        shader.SetInt($"Tev[{i}].alpha_bias", (int)tev.alpha_bias);
                        shader.SetInt($"Tev[{i}].alpha_scale", (int)tev.alpha_scale);
                        shader.SetBoolToInt($"Tev[{i}].alpha_clamp", tev.alpha_clamp);
                        shader.SetInt($"Tev[{i}].alpha_a", (int)tev.alpha_a_in);
                        shader.SetInt($"Tev[{i}].alpha_b", (int)tev.alpha_b_in);
                        shader.SetInt($"Tev[{i}].alpha_c", (int)tev.alpha_c_in);
                        shader.SetInt($"Tev[{i}].alpha_d", (int)tev.alpha_d_in);

                        shader.SetColor($"Tev[{i}].konst", tev.constant, tev.constantAlpha);
                        shader.SetColor($"Tev[{i}].tev0", tev.tev0, tev.tev0Alpha);
                        shader.SetColor($"Tev[{i}].tev1", tev.tev1, tev.tev1Alpha);
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
