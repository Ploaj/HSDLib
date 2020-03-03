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

            if(!DOBJtoBuffer.ContainsKey(dobj))
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

            Vector3 camPos = (camera.RotationMatrix * new Vector4(camera.Translation, 1)).Xyz;
            GXShader.SetVector3("cameraPos", camPos);

            Matrix4 single = Matrix4.Identity;
            if (parentJOBJ != null && jobjManager != null)
                single = jobjManager.GetWorldTransform(parentJOBJ);
            GL.UniformMatrix4(GXShader.GetVertexAttributeUniformLocation("singleBind"), false, ref single);

            var rootJOBJ = jobjManager.GetJOBJ(0);
            GXShader.SetBoolToInt("isRootBound", parentJOBJ?._s == rootJOBJ?._s);

            GXShader.SetWorldTransformBones(jobjManager.GetWorldTransforms());
            //GXShader.SetBindTransformBones(jobjManager.GetBindTransforms());

            var tb = jobjManager.GetBindTransforms();
            if (tb.Length > 0)
                GXShader.SetMatrix4x4("binds", tb);
            
            GL.Uniform3(GXShader.GetVertexAttributeUniformLocation("overlayColor"), OverlayColor);

            Matrix4 sphereMatrix = camera.ModelViewMatrix;
            sphereMatrix.Invert();
            sphereMatrix.Transpose();
            GXShader.SetMatrix4x4("sphereMatrix", ref sphereMatrix);

            float wscale = 1;
            float hscale = 1;
            bool mirrorX = false;
            bool mirrorY = false;
            if (mobj != null)
                BindMOBJ(GXShader, mobj, out wscale, out hscale, out mirrorX, out mirrorY);

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
            
            if (selected)
            {
                GL.Uniform1(GXShader.GetVertexAttributeUniformLocation("colorOverride"), 1);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            }
            else
            {
                GL.PolygonMode(MaterialFace.Back, PolygonMode.Fill);
                GL.Uniform1(GXShader.GetVertexAttributeUniformLocation("colorOverride"), 0);
            }

            foreach (var p in DOBJtoPOBJCache[dobj])
            {
                var en = p.Envelopes;
                GL.Uniform4(GXShader.GetVertexAttributeUniformLocation("envelopeIndex"), p.Envelopes.Length, ref p.Envelopes[0].X);

                var we = p.Weights;
                GL.Uniform4(GXShader.GetVertexAttributeUniformLocation("weights"), p.Weights.Length, ref p.Weights[0].X);
                
                GL.Uniform1(GXShader.GetVertexAttributeUniformLocation("hasEnvelopes"), p.HasWeighting ? 1 : 0);

                GL.Uniform1(GXShader.GetVertexAttributeUniformLocation("notInverted"), p.Flag.HasFlag(POBJ_FLAG.NOTINVERTED) ? 1 : 0);

                if (p.Flag.HasFlag(POBJ_FLAG.CULLFRONT))
                    GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);

                foreach (var dl in p.DisplayLists)
                    GL.DrawArrays(dl.PrimType, dl.Offset, dl.Count);
            }
            
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("PNMTXIDX"));
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_POS"));
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_NRM"));
            GL.DisableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX0"));

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
        private void BindMOBJ(Shader shader, HSD_MOBJ mobj, out float wscale, out float hscale, out bool mirrorX, out bool mirrorY)
        {
            GL.Enable(EnableCap.Texture2D);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Greater, 0f);
            
            wscale = 1;
            hscale = 1;
            mirrorX = false;
            mirrorY = false;
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

            shader.SetBoolToInt("enableTEX0", mobj.RenderFlags.HasFlag(RENDER_MODE.TEX0));
            shader.SetBoolToInt("dfNone", mobj.RenderFlags.HasFlag(RENDER_MODE.DF_NONE));
            shader.SetBoolToInt("enableSpecular", mobj.RenderFlags.HasFlag(RENDER_MODE.SPECULAR));
            shader.SetBoolToInt("enableDiffuse", mobj.RenderFlags.HasFlag(RENDER_MODE.DIFFUSE));
            shader.SetBoolToInt("enableMaterial", mobj.RenderFlags.HasFlag(RENDER_MODE.DIFFUSE_MAT));
            shader.SetBoolToInt("useVertexColor", mobj.RenderFlags.HasFlag(RENDER_MODE.DIFFUSE_VTX));

            shader.SetInt("enableTexDiffuse", 0);
            shader.SetInt("texDiffuse", 0);
            shader.SetInt("difColorType", 0);
            shader.SetInt("difAlphaType", 0);
            shader.SetInt("diffuseCoordType", 0);
            shader.SetVector2("diffuseUVScale", 1, 1);


            // Bind Textures
            if (mobj.Textures != null)
            {
                foreach (var tex in mobj.Textures.List)
                {
                    if (tex.ImageData == null)
                        continue;

                    if (!imageBufferTextureIndex.ContainsKey(tex.ImageData.ImageData))
                    {
                        imageBufferTextureIndex.Add(tex.ImageData.ImageData, TextureManager.TextureCount);
                        TextureManager.Add(tex.GetDecodedImageData(), tex.ImageData.Width, tex.ImageData.Height);
                    }

                    var texid = TextureManager.Get(imageBufferTextureIndex[tex.ImageData.ImageData]);

                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, texid);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GXTranslator.toWrapMode(tex.WrapS));
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GXTranslator.toWrapMode(tex.WrapT));
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GXTranslator.toMagFilter(tex.MagFilter));

                    wscale = tex.WScale;
                    hscale = tex.HScale;

                    mirrorX = tex.WrapS == GXWrapMode.MIRROR;
                    mirrorY = tex.WrapT == GXWrapMode.MIRROR;

                    int coordType = 0;
                    if (tex.Flags.HasFlag(TOBJ_FLAGS.COORD_REFLECTION))
                        coordType = 1;

                    shader.SetInt("enableTexDiffuse", 1);
                    shader.SetInt("diffuseTex", 0);
                    shader.SetInt("difColorType", 0);
                    shader.SetInt("difAlphaType", 0);
                    shader.SetInt("diffuseCoordType", coordType);
                    shader.SetBoolToInt("diffuseMirrorFix", mirrorY);
                    shader.SetVector2("diffuseUVScale", wscale, hscale);

                    break;
                }
            }
        }
        #endregion
    }
}
