using HSDRaw.Common;
using HSDRaw.GX;
using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace HSDRawViewer.Rendering
{
    /// <summary>
    /// 
    /// </summary>
    public class DOBJManager
    {
        public bool RenderTextures { get; set; } = true;

        public bool RenderVertexColor { get; set; } = true;

        public bool OutlineSelected = true;

        public bool OnlyRenderSelected = false;

        public HSD_DOBJ SelectedDOBJ;

        public int DOBJCount { get => DOBJtoBuffer.Count; }

        public List<HSD_DOBJ> HiddenDOBJs { get; internal set; } = new List<HSD_DOBJ>();
        
        private Dictionary<HSD_POBJ, GX_DisplayList> pobjToDisplayList = new Dictionary<HSD_POBJ, GX_DisplayList>();

        private Dictionary<byte[], int> imageBufferTextureIndex = new Dictionary<byte[], int>();

        private TextureManager TextureManager = new TextureManager();

        // Shader
        private static Shader GXShader;

        private Dictionary<HSD_DOBJ, int> DOBJtoBuffer = new Dictionary<HSD_DOBJ, int>();
        private Dictionary<HSD_DOBJ, List<CachedPOBJ>> DOBJtoPOBJCache = new Dictionary<HSD_DOBJ, List<CachedPOBJ>>();


        // Attributes
        public Vector3 OverlayColor = Vector3.One;


        public class CachedPOBJ
        {
            public POBJ_FLAG Flag
            {
                get => POBJ == null ? 0 : POBJ.Flags;
            }

            public HSD_POBJ POBJ;

            public int EnvelopeCount = 0;
            public Vector4[] Envelopes = new Vector4[10];
            public Vector4[] Weights = new Vector4[10];

            public bool HasWeighting = false;

            public List<CachedDL> DisplayLists = new List<CachedDL>();
        }

        public class CachedDL
        {
            public PrimitiveType PrimType;

            public int Offset;

            public int Count;
        }

        // END Shader

        /// <summary>
        /// 
        /// </summary>
        public void ClearRenderingCache()
        {
            TextureManager.ClearTextures();
            imageBufferTextureIndex.Clear();
            pobjToDisplayList.Clear();

            // TODO: shader is currently static
            //if(GXShader != null)
            //    GXShader.Delete();
            //GXShader = null;

            foreach(var v in DOBJtoBuffer)
                    GL.DeleteBuffer(v.Value);
            DOBJtoBuffer.Clear();

            DOBJtoPOBJCache.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="dobj"></param>
        /// <param name="parentJOBJ"></param>
        /// <param name="jobjManager"></param>
        public void RenderDOBJShader(Camera camera, HSD_DOBJ dobj, HSD_JOBJ parentJOBJ, JOBJManager jobjManager, MatAnimManager animation, bool selected = false)
        {
            if (dobj.Pobj == null)
                return;

            if (HiddenDOBJs.Contains(dobj) || (selected && OnlyRenderSelected))
                return;

            if (OnlyRenderSelected && SelectedDOBJ != null && SelectedDOBJ._s != dobj._s)
                return;

            var mobj = dobj.Mobj;
            var pobjs = dobj.Pobj.List;

            if (!DOBJtoBuffer.ContainsKey(dobj))
                LoadDOBJ(dobj, jobjManager);

            if (!DOBJtoBuffer.ContainsKey(dobj))
                return;

            if (GXShader == null)
            {
                GXShader = new Shader();
                GXShader.LoadShader(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Shader\gx.vert"));
                GXShader.LoadShader(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Shader\gx.frag"));
            }

            GL.UseProgram(GXShader.programId);

            var mvp = camera.MvpMatrix;
            GL.UniformMatrix4(GXShader.GetVertexAttributeUniformLocation("mvp"), false, ref mvp);
            
            var campos = (camera.RotationMatrix * new Vector4(camera.Translation, 1)).Xyz;
            GXShader.SetVector3("cameraPos", campos);

            //GXShader.SetBoolToInt("envelopeModel", parentJOBJ.Flags.HasFlag(JOBJ_FLAG.ENVELOPE_MODEL));

            Matrix4 single = Matrix4.Identity;
            if (parentJOBJ != null && jobjManager != null)
                single = jobjManager.GetWorldTransform(parentJOBJ);
            GL.UniformMatrix4(GXShader.GetVertexAttributeUniformLocation("singleBind"), false, ref single);

            GXShader.SetBoolToInt("isSkeleton", parentJOBJ.Flags.HasFlag(JOBJ_FLAG.SKELETON_ROOT) || parentJOBJ.Flags.HasFlag(JOBJ_FLAG.SKELETON));
            
            GXShader.SetWorldTransformBones(jobjManager.GetWorldTransforms());
            //GXShader.SetBindTransformBones(jobjManager.GetBindTransforms());

            GXShader.SetInt("selectedBone", jobjManager.IndexOf(jobjManager.SelectetedJOBJ));


            // lighting
            GXShader.SetBoolToInt("perPixelLighting", jobjManager.settings.UsePerPixelLighting);
            GXShader.SetBoolToInt("light.useCamera", jobjManager.settings.UseCameraLight);
            GXShader.SetVector3("light.position", jobjManager.settings.LightX, jobjManager.settings.LightY, jobjManager.settings.LightZ);
            GXShader.SetColor("light.ambient", jobjManager.settings.AmbientColor, 1);
            GXShader.SetColor("light.diffuse", jobjManager.settings.DiffuseColor, 1);
            //GXShader.SetColor("light.specular", jobjManager.settings.SpecularColor, 1);
            GXShader.SetFloat("light.ambientPower", jobjManager.settings.AmbientPower);
            GXShader.SetFloat("light.diffusePower", jobjManager.settings.DiffusePower);
            //GXShader.SetFloat("light.specularPower", jobjManager.settings.SpecularPower);

            //
            var tb = jobjManager.GetBindTransforms();
            if (tb.Length > 0)
                GXShader.SetMatrix4x4("binds", tb);

            GXShader.SetVector3("overlayColor", OverlayColor);
            GXShader.SetInt("renderOverride", (int)jobjManager.RenderMode);

            Matrix4 sphereMatrix = camera.ModelViewMatrix;
            sphereMatrix.Invert();
            sphereMatrix.Transpose();
            GXShader.SetMatrix4x4("sphereMatrix", ref sphereMatrix);

            if (mobj != null)
                BindMOBJ(GXShader, mobj, parentJOBJ, animation);

            GL.BindBuffer(BufferTarget.ArrayBuffer, DOBJtoBuffer[dobj]);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("PNMTXIDX"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("PNMTXIDX"), 1, VertexAttribPointerType.Short, false, GX_Vertex.Stride, 0);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_POS"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_POS"), 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 8);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_NRM"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_NRM"), 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 20);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_TAN"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_TAN"), 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 32);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_BTAN"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_BTAN"), 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 44);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_CLR0"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_CLR0"), 4, VertexAttribPointerType.Float, true, GX_Vertex.Stride, 56);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX0"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX0"), 2, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 88);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX1"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX1"), 2, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 96);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX2"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX2"), 2, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 104);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX3"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX3"), 2, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 112);

            GXShader.SetBoolToInt("colorOverride", selected);

            foreach (var p in DOBJtoPOBJCache[dobj])
            {
                var en = p.Envelopes;
                GL.Uniform4(GXShader.GetVertexAttributeUniformLocation("envelopeIndex"), p.Envelopes.Length, ref p.Envelopes[0].X);

                var we = p.Weights;
                GL.Uniform4(GXShader.GetVertexAttributeUniformLocation("weights"), p.Weights.Length, ref p.Weights[0].X);
                
                GXShader.SetBoolToInt("hasEnvelopes", p.HasWeighting);
                GXShader.SetBoolToInt("enableParentTransform", !p.Flag.HasFlag(POBJ_FLAG.UNKNOWN0));
                //GXShader.SetInt("envelopeCount", p.EnvelopeCount);

                GL.Enable(EnableCap.CullFace);
                if (selected)
                {
                    GL.PolygonMode(MaterialFace.Back, PolygonMode.Line);
                }
                else
                if (p.Flag.HasFlag(POBJ_FLAG.CULLFRONT))
                {
                    GL.CullFace(CullFaceMode.Front);
                    GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
                }
                else
                if (p.Flag.HasFlag(POBJ_FLAG.CULLBACK))
                {
                    GL.CullFace(CullFaceMode.Back);
                    GL.PolygonMode(MaterialFace.Back, PolygonMode.Fill);
                }
                else
                {
                    GL.Disable(EnableCap.CullFace);
                }

                foreach (var dl in p.DisplayLists)
                    GL.DrawArrays(dl.PrimType, dl.Offset, dl.Count);
            }
            
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("PNMTXIDX"));
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_POS"));
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_NRM"));
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_TAN"));
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_BTAN"));
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_CLR0"));
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX0"));
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX1"));
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX2"));
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX3"));

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.UseProgram(0);
        }

        /// <summary>
        /// Prepares DOBJ for rendering by loading relevant information into a cache
        /// </summary>
        private void LoadDOBJ(HSD_DOBJ dobj, JOBJManager jobjManager)
        {
            if(DOBJtoBuffer.ContainsKey(dobj))
            {
                GL.DeleteBuffer(DOBJtoBuffer[dobj]);
                DOBJtoBuffer.Remove(dobj);
            }

            List<CachedPOBJ> pobjs = new List<CachedPOBJ>();
            List<GX_Vertex> vertices = new List<GX_Vertex>();
            int off = 0;
            foreach(var pobj in dobj.Pobj.List)
            {
                var dl = pobj.ToDisplayList();

                vertices.AddRange(dl.Vertices);

                var pobjCache = new CachedPOBJ();

                pobjCache.POBJ = pobj;

                // build envelopes
                int eni = 0;
                foreach(var v in dl.Envelopes)
                {
                    Vector4 b = new Vector4();
                    Vector4 w = new Vector4();
                    for(int i = 0; i < v.EnvelopeCount; i++)
                    {
                        if (i >= 4)
                            break;
                        w[i] = v.GetWeightAt(i);
                        b[i] = jobjManager.IndexOf(v.GetJOBJAt(i));
                    }
                    pobjCache.Weights[eni] = w;
                    pobjCache.Envelopes[eni] = b;
                    eni++;
                    pobjCache.EnvelopeCount = v.EnvelopeCount;
                    pobjCache.HasWeighting = v.EnvelopeCount > 0;
                }

                // load display list
                foreach (var v in dl.Primitives)
                {
                    /*if (pobj.ShapeSet != null)
                    {
                        Console.WriteLine(dl.Vertices.Count + " " + pobj.ShapeSet.VertexIndexCount);
                    }*/
                    pobjCache.DisplayLists.Add(new CachedDL()
                    {
                        Offset = off,
                        Count = v.Count,
                        PrimType = GXTranslator.toPrimitiveType(v.PrimitiveType)
                    });
                    off += v.Count;
                }

                pobjs.Add(pobjCache);
            }
            
            var arr = vertices.ToArray();

            int buf;
            GL.GenBuffers(1, out buf);
            GL.BindBuffer(BufferTarget.ArrayBuffer, buf);
            GL.BufferData(BufferTarget.ArrayBuffer, arr.Length * GX_Vertex.Stride, arr, BufferUsageHint.StaticDraw);

            DOBJtoBuffer.Add(dobj, buf);
            DOBJtoPOBJCache.Add(dobj, pobjs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matAnim"></param>
        public void PreLoadMatAnim(MatAnimManager matAnim)
        {
            foreach (var n in matAnim.Nodes)
                foreach (var no in n.Nodes)
                    foreach (var t in no.TextureAnims)
                        foreach (var texture in t.Textures)
                            PreLoadTexture(texture);
        }

        /// <summary>
        /// 
        /// </summary>
        private void PreLoadTexture(HSD_TOBJ tobj)
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

        #region MOBJ
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mobj"></param>
        private void BindMOBJ(Shader shader, HSD_MOBJ mobj, HSD_JOBJ parentJOBJ, MatAnimManager animation)
        {
            GL.Enable(EnableCap.Texture2D);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Greater, 0f);
            
            if (mobj == null)
                return;

            var pp = mobj.PEDesc;
            if (pp != null)
            {
                GL.BlendFunc(GXTranslator.toBlendingFactor(pp.SrcFactor), GXTranslator.toBlendingFactor(pp.DstFactor));
                GL.DepthFunc(GXTranslator.toDepthFunction(pp.DepthFunction));

                //GL.AlphaFunc(GXTranslator.toAlphaFunction(pp.AlphaComp0), pp.AlphaRef0 / 255f);
                //GL.AlphaFunc(GXTranslator.toAlphaFunction(pp.AlphaComp1), pp.AlphaRef1 / 255f);
            }

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

            shader.SetBoolToInt("hasTEX0", mobj.RenderFlags.HasFlag(RENDER_MODE.TEX0) || enableAll);
            shader.SetBoolToInt("hasTEX1", mobj.RenderFlags.HasFlag(RENDER_MODE.TEX1) || enableAll);
            shader.SetBoolToInt("hasTEX2", mobj.RenderFlags.HasFlag(RENDER_MODE.TEX2) || enableAll);
            shader.SetBoolToInt("hasTEX3", mobj.RenderFlags.HasFlag(RENDER_MODE.TEX3) || enableAll);

            //LoadTextureConstants(shader);

            // these are always uniform
            GL.Uniform1(GL.GetUniformLocation(shader.programId, "textures"), 4, new int[] { 0, 1, 2, 3 });

            // Bind Textures
            if (mobj.Textures != null)
            {
                var textures = mobj.Textures.List;
                for(int i = 0; i < textures.Count; i++)
                {
                    var tex = textures[i];
                    var displayTex = tex;

                    if (tex.ImageData == null)
                        continue;

                    var blending = tex.Blending;

                    var transform = Matrix4.CreateScale(tex.SX, tex.SY, tex.SZ) *
                        Matrix4.CreateFromQuaternion(Math3D.FromEulerAngles(tex.RZ, tex.RY, tex.RX)) *
                        Matrix4.CreateTranslation(tex.TX, tex.TY, tex.TZ);

                    if(tex.SY != 0 && tex.SX != 0 && tex.SZ != 0)
                        transform.Invert();

                    if (animation != null)
                    {
                        var state = animation.GetTextureAnimState(tex);
                        if(state != null)
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
        #endregion
        

    }
}
