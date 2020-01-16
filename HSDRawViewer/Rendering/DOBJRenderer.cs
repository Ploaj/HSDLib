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

        public HSD_DOBJ SelectedDOBJ = null;
        
        private Dictionary<HSD_POBJ, GX_DisplayList> pobjToDisplayList = new Dictionary<HSD_POBJ, GX_DisplayList>();

        private Dictionary<byte[], int> imageBufferTextureIndex = new Dictionary<byte[], int>();

        private TextureManager TextureManager = new TextureManager();


        // Shader
        private Shader GXShader;

        private Dictionary<HSD_DOBJ, int> DOBJtoBuffer = new Dictionary<HSD_DOBJ, int>();
        private Dictionary<HSD_DOBJ, List<CachedPOBJ>> DOBJtoPOBJCache = new Dictionary<HSD_DOBJ, List<CachedPOBJ>>();

        public class CachedPOBJ
        {
            public Vector4[] Envelopes = new Vector4[10];
            public Vector4[] Weights = new Vector4[10];

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
        public void RenderDOBJShader(Camera camera, HSD_DOBJ dobj, HSD_JOBJ parentJOBJ, JOBJManager jobjManager)
        {
            if (dobj.Pobj == null)
                return;

            if (SelectedDOBJ != null && !SelectedDOBJ.Equals(dobj))
                return;

            var selected = SelectedDOBJ == dobj;
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

            Matrix4 single = Matrix4.Identity;
            if (parentJOBJ != null && jobjManager != null)
                single = jobjManager.GetWorldTransform(parentJOBJ);
            GL.UniformMatrix4(GXShader.GetVertexAttributeUniformLocation("singleBind"), false, ref single);

            var t = jobjManager.GetBindTransforms();
            if (t.Length > 0)
                GL.UniformMatrix4(GXShader.GetVertexAttributeUniformLocation("transforms"), t.Length, false, ref t[0].Row0.X);

            GL.Uniform1(GXShader.GetVertexAttributeUniformLocation("tex0"), 0);
            
            float wscale = 1;
            float hscale = 1;
            bool mirrorX = false;
            bool mirrorY = false;
            if (mobj != null)
                BindMOBJ(mobj, out wscale, out hscale, out mirrorX, out mirrorY);

            GL.Uniform2(GXShader.GetVertexAttributeUniformLocation("UVScale"), wscale, hscale);

            GL.BindBuffer(BufferTarget.ArrayBuffer, DOBJtoBuffer[dobj]);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("PNMTXIDX"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("PNMTXIDX"), 1, VertexAttribPointerType.Short, false, GX_Vertex.Stride, 0);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_POS"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_POS"), 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 4);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_NRM"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_NRM"), 3, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 16);

            GL.EnableVertexAttribArray(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX0"));
            GL.VertexAttribPointer(GXShader.GetVertexAttributeUniformLocation("GX_VA_TEX0"), 2, VertexAttribPointerType.Float, false, GX_Vertex.Stride, 60);



            foreach (var p in DOBJtoPOBJCache[dobj])
            {
                var en = p.Envelopes;
                GL.Uniform4(GXShader.GetVertexAttributeUniformLocation("envelopeIndex"), p.Envelopes.Length, ref p.Envelopes[0].X);

                var we = p.Weights;
                GL.Uniform4(GXShader.GetVertexAttributeUniformLocation("weights"), p.Weights.Length, ref p.Weights[0].X);

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

                // build envelopes
                int eni = 0;
                foreach(var v in dl.Envelopes)
                {
                    Vector4 b = new Vector4();
                    Vector4 w = new Vector4();
                    for(int i = 0; i < v.EnvelopeCount; i++)
                    {
                        w[i] = v.GetWeightAt(i);
                        b[i] = jobjManager.IndexOf(v.GetJOBJAt(i));
                    }
                    pobjCache.Weights[eni] = w;
                    pobjCache.Envelopes[eni] = b;
                    eni++;
                }

                // load display list
                foreach (var v in dl.Primitives)
                {
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
        /// <param name="dobj"></param>
        /// <param name="jobjManager"></param>
        public void RenderDOBJ(HSD_DOBJ dobj, HSD_JOBJ parentJOBJ = null, JOBJManager jobjManager = null)
        {
            var Transform = Matrix4.Identity;

            if(parentJOBJ != null && jobjManager != null)
                Transform = jobjManager.GetWorldTransform(parentJOBJ);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.MultMatrix(ref Transform);

            GL.Enable(EnableCap.Blend);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            
            if (dobj.Pobj != null)
                RenderPOBJList(dobj, jobjManager);

            GL.PopMatrix();
        }


        #region POBJ

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pobj"></param>
        private void LoadPOBJ(HSD_POBJ pobj)
        {
            pobjToDisplayList.Add(pobj, pobj.ToDisplayList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pobjs"></param>
        /// <param name="jobjManager"></param>
        private void RenderPOBJList(HSD_DOBJ dobj, JOBJManager jobjManager)
        {
            if (dobj.Pobj == null)
                return;

            if (SelectedDOBJ != null && !SelectedDOBJ.Equals(dobj))
                return;

            var selected = SelectedDOBJ == dobj;
            var mobj = dobj.Mobj;
            var pobjs = dobj.Pobj.List;

            float wscale = 1;
            float hscale = 1;
            bool mirrorX = false;
            bool mirrorY = false;

            if(mobj != null)
                BindMOBJ(mobj, out wscale, out hscale, out mirrorX, out mirrorY);
            
            for(int po = 0; po < pobjs.Count; po++)
            {
                var p = pobjs[pobjs.Count - 1 - po];

                if (!pobjToDisplayList.ContainsKey(p))
                {
                    LoadPOBJ(p);
                }

                if (!pobjToDisplayList.ContainsKey(p))
                    continue;

                var dl = pobjToDisplayList[p];

                var envelopeWeights = p.EnvelopeWeights;

                var single = p.SingleBoundJOBJ;

                int offset = 0;
                foreach (var g in dl.Primitives)
                {
                    GL.Begin(GXTranslator.toPrimitiveType(g.PrimitiveType));
                    for (int i = 0; i < g.Count; i++)
                    {
                        // load vertex data
                        var vert = dl.Vertices[offset + i];

                        var pos = GXTranslator.toVector3(vert.POS);
                        var nrm = GXTranslator.toVector3(vert.NRM);
                        var tx0 = GXTranslator.toVector2(vert.TEX0);
                        var color = GXTranslator.toVector4(vert.CLR0);

                        if (color == Vector4.Zero || !RenderVertexColor)
                            color = Vector4.One;

                        // uv manip
                        tx0.X *= wscale;
                        tx0.Y *= hscale;
                        if (mirrorY)
                            tx0.Y += 1;

                        // skinning
                        if(single != null)
                        {
                            var t = jobjManager.GetWorldTransform(single);
                            pos = Vector3.TransformPosition(pos, t);
                            nrm = Vector3.TransformNormal(nrm, t);
                        }
                        if (envelopeWeights != null && jobjManager != null)
                        {
                            if (dl.Vertices[offset + i].PNMTXIDX / 3 >= envelopeWeights.Length)
                                throw new Exception((dl.Vertices[offset + i].PNMTXIDX / 3) + " " + envelopeWeights.Length);

                            var en = envelopeWeights[dl.Vertices[offset + i].PNMTXIDX / 3];

                            if (en.EnvelopeCount == 0)
                            {

                            }
                            else
                            if (en.EnvelopeCount == 1)
                            {
                                var t = jobjManager.GetWorldTransform(en.GetJOBJAt(0));
                                pos = Vector3.TransformPosition(pos, t);
                                nrm = Vector3.TransformNormal(nrm, t);
                            }
                            else
                            if (p.Flags.HasFlag(POBJ_FLAG.ENVELOPE))
                            {
                                Vector3 bindpos = Vector3.Zero;
                                Vector3 nrmpos = Vector3.Zero;
                                for (int j = 0; j < en.EnvelopeCount; j++)
                                {
                                    var bind = jobjManager.GetBindTransform(en.GetJOBJAt(j));
                                    bindpos += Vector3.TransformPosition(pos, bind) * en.GetWeightAt(j);
                                    nrmpos += Vector3.TransformNormal(nrm, bind) * en.GetWeightAt(j);
                                }
                                pos = bindpos;
                                nrm = nrmpos;
                            }
                        }

                        // final colorization
                        var colr = 0.2f + Math.Abs(Vector3.Dot(nrm, Vector3.UnitZ));
                        if (nrm == Vector3.Zero)
                            colr = 1;

                        var finalColor = new Vector4(colr, colr, colr, 1) * color;

                        finalColor.W = mobj.Material.Alpha;
                        
                        GL.TexCoord2(tx0);
                        GL.Color4(finalColor);
                        GL.Vertex3(pos);
                    }
                    GL.End();
                    offset += g.Count;
                }
            }
        }

#endregion

        #region MOBJ
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mobj"></param>
        private void BindMOBJ(HSD_MOBJ mobj, out float wscale, out float hscale, out bool mirrorX, out bool mirrorY)
        {
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
            }

            var color = mobj.Material;
            if (color != null)
            {
            }

            // Bind Textures
            if (mobj.Textures != null)
            {
                GL.Enable(EnableCap.Texture2D);
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
                }
            }
            else
            {
                GL.Disable(EnableCap.Texture2D);
            }
        }
        #endregion
    }
}
