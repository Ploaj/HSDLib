using HSDRaw.Common;
using HSDRaw.GX;
using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace HSDRawViewer.Rendering
{
    // TODO: shader cache rendering would be much faster
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
            public POBJ_FLAG Flag;

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

            if(GXShader != null)
                GXShader.Delete();
            GXShader = null;

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
        public void RenderDOBJShader(Camera camera, HSD_DOBJ dobj, HSD_JOBJ parentJOBJ, JOBJManager jobjManager, bool selected = false)
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
                GXShader.LoadShader(@"Shader\gx.vert");
                GXShader.LoadShader(@"Shader\gx.frag");
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
            
            GXShader.SetWorldTransformBones(jobjManager.GetWorldTransforms());
            //GXShader.SetBindTransformBones(jobjManager.GetBindTransforms());

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
                BindMOBJ(GXShader, mobj, parentJOBJ);

            GL.BindBuffer(BufferTarget.ArrayBuffer, DOBJtoBuffer[dobj]);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("PNMTXIDX"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("PNMTXIDX"), 1, VertexAttribPointerType.Short, false, GX_Vertex.Stride, 0);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_POS"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_POS"), 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 8);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_NRM"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_NRM"), 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 20);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_CLR0"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_CLR0"), 4, VertexAttribPointerType.Float, true, GX_Vertex.Stride, 56);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX0"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX0"), 2, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 88);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX1"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX1"), 2, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 96);
            
            GXShader.SetBoolToInt("colorOverride", selected);

            foreach (var p in DOBJtoPOBJCache[dobj])
            {
                var en = p.Envelopes;
                GL.Uniform4(GXShader.GetVertexAttributeUniformLocation("envelopeIndex"), p.Envelopes.Length, ref p.Envelopes[0].X);

                var we = p.Weights;
                GL.Uniform4(GXShader.GetVertexAttributeUniformLocation("weights"), p.Weights.Length, ref p.Weights[0].X);
                
                GXShader.SetBoolToInt("hasEnvelopes", p.HasWeighting);
                GXShader.SetBoolToInt("enableParentTransform", !p.Flag.HasFlag(POBJ_FLAG.PARENTTRANSFORM));
                //GXShader.SetInt("envelopeCount", p.EnvelopeCount);

                GL.Enable(EnableCap.CullFace);
                if (selected)
                    GL.PolygonMode(MaterialFace.Back, PolygonMode.Line);
                else
                if (p.Flag.HasFlag(POBJ_FLAG.CULLFRONT))
                    GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
                else
                if (p.Flag.HasFlag(POBJ_FLAG.CULLBACK))
                    GL.PolygonMode(MaterialFace.Back, PolygonMode.Fill);
                else
                {
                    GL.Disable(EnableCap.CullFace);
                    //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                }

                foreach (var dl in p.DisplayLists)
                    GL.DrawArrays(dl.PrimType, dl.Offset, dl.Count);
            }
            
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("PNMTXIDX"));
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_POS"));
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_NRM"));
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX0"));
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX1"));

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

                pobjCache.Flag = pobj.Flags;

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

        #region MOBJ
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mobj"></param>
        private void BindMOBJ(Shader shader, HSD_MOBJ mobj, HSD_JOBJ parentJOBJ)
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

            var id = Matrix4.Identity;

            for(int index = 0; index < 2; index++)
            {
                shader.SetInt($"TEX{index}", index);
                shader.SetInt($"TEX{index}LightType", 0);
                shader.SetInt($"TEX{index}ColorOperation", 0);
                shader.SetInt($"TEX{index}AlphaOperation", 0);
                shader.SetInt($"TEX{index}CoordType", 0);
                shader.SetInt($"TEX{index}Blend", 1);
                shader.SetBoolToInt($"TEX{index}MirrorFix", false);
                shader.SetVector2($"TEX{index}UVScale", 1, 1);
                shader.SetMatrix4x4($"TEX{index}Transform", ref id);
            }

            // Bind Textures
            if (mobj.Textures != null)
            {
                int index = -1;
                foreach (var tex in mobj.Textures.List)
                {
                    index++;
                    if (index > 1)
                        break;
                    if (tex.ImageData == null)
                        continue;

                    if (!imageBufferTextureIndex.ContainsKey(tex.ImageData.ImageData))
                    {
                        imageBufferTextureIndex.Add(tex.ImageData.ImageData, TextureManager.TextureCount);
                        TextureManager.Add(tex.GetDecodedImageData(), tex.ImageData.Width, tex.ImageData.Height);
                    }

                    var texid = TextureManager.Get(imageBufferTextureIndex[tex.ImageData.ImageData]);

                    GL.ActiveTexture(TextureUnit.Texture0 + index);
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

                    var transform = Matrix4.CreateScale(tex.SX, tex.SY, tex.SZ) *
                        Matrix4.CreateFromQuaternion(Math3D.FromEulerAngles(tex.RZ, tex.RY, tex.RX)) *
                        Matrix4.CreateTranslation(tex.TX, tex.TY, tex.TZ);

                    transform.Invert();
                    
                    shader.SetInt($"TEX{index}", index);
                    shader.SetInt($"TEX{index}LightType", lightType);
                    shader.SetInt($"TEX{index}ColorOperation", colorOP);
                    shader.SetInt($"TEX{index}AlphaOperation", alphaOP);
                    shader.SetInt($"TEX{index}CoordType", coordType);
                    shader.SetFloat($"TEX{index}Blend", tex.Blending);
                    shader.SetBoolToInt($"TEX{index}MirrorFix", mirrorY);
                    shader.SetVector2($"TEX{index}UVScale", wscale, hscale);
                    shader.SetMatrix4x4($"TEX{index}Transform", ref transform);
                }
            }
        }
        #endregion
        

    }
}
