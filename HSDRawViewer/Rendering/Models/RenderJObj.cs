using HSDRaw.Common;
using HSDRawViewer.Rendering.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using HSDRawViewer.Rendering.GX;
using OpenTK.Mathematics;
using System.Drawing;
using HSDRaw.GX;

namespace HSDRawViewer.Rendering.Models
{
    public class RenderJObj
    {
        private static int MAX_TEX { get; } = 4;

        private GXShader _shader;

        public LiveJObj RootJObj { get; internal set; }

        private bool Initialized = false;

        public RenderMode RenderMode { get; set; }

        public GXLightParam _lightParam { get; internal set; } = new GXLightParam();

        public GXFogParam _fogParam { get; internal set; } = new GXFogParam();

        public JobjDisplaySettings _settings { get; internal set; } = new JobjDisplaySettings();

        public float ShapeBlend { get; set; } = 1;

        public Vector3 OverlayColor { get; set; } = Vector3.One;


        private Dictionary<byte[], int> imageBufferTextureIndex = new Dictionary<byte[], int>();

        /// <summary>
        /// collection of renderable dobjs
        /// </summary>
        private List<RenderDObj> RenderDobjs = new List<RenderDObj>();

        /// <summary>
        /// For managing opengl buffers
        /// </summary>
        private VertexBufferManager BufferManager = new VertexBufferManager();

        /// <summary>
        /// For managing opengl textures
        /// </summary>
        private TextureManager TextureManager = new TextureManager();

        /// <summary>
        /// 
        /// </summary>
        public HSD_JOBJ SelectedJObj;

        private static MatAnimMaterialState MaterialState = new MatAnimMaterialState();

        /// <summary>
        /// 
        /// </summary>
        public RenderJObj()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="desc"></param>
        public RenderJObj(HSD_JOBJ desc)
        {
            LoadJObj(desc);
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadJObj(HSD_JOBJ desc)
        {
            RootJObj = new LiveJObj(desc);
            Initialized = false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeRendering()
        {
            Initialized = true;

            // initialize shader
            if (_shader != null)
            {
                _shader.Dispose();
                _shader = null;
            }
            _shader = new GXShader();

            // clear previous caches
            BufferManager.ClearRenderingCache();
            TextureManager.ClearTextures();
            imageBufferTextureIndex.Clear();

            // initial dobj cache
            foreach (var j in RootJObj?.Enumerate)
            {
                if (j.Desc.Dobj == null)
                    continue;

                // initialize all dobjs
                foreach (var d in j.Desc.Dobj.List)
                {
                    var dob = new RenderDObj(j, d);
                    dob.InitializeBufferData(BufferManager);
                    RenderDobjs.Add(dob);

                    // preload textures
                    if (d.Mobj != null && d.Mobj.Textures != null)
                    {
                        foreach (var t in d.Mobj.Textures.List)
                        {
                            PreLoadTexture(t);
                        }
                    }
                }
            }

            // print diagnostic info
            System.Diagnostics.Debug.WriteLine($"Buffer Count: {BufferManager.BufferCount}");
            System.Diagnostics.Debug.WriteLine($"Texture Count: {TextureManager.TextureCount}");
        }

        /// <summary>
        /// Signals jobj to reload data during next render update
        /// This operation is very slow!
        /// </summary>
        public void Invalidate()
        {
            Initialized = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="update"></param>
        public void Render(Camera camera, bool update = true)
        {
            // nothing to render if jobj is nul
            if (RootJObj == null)
                return;

            // initial rendering if not already
            if (!Initialized)
                InitializeRendering();

            //push attrib for cleanup
            GL.PushAttrib(AttribMask.AllAttribBits);

            // recalculate transforms
            if (update)
                RootJObj.RecalculateTransforms(camera, true);

            // enable depth test
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            
            // prepare shader
            SetupShader();

            // render with shader
            _shader.Bind(camera, _lightParam, _fogParam);

            // Render DOBJS
            RenderJObjDisplay(camera);

            // unbind shader
            _shader.Unbind();

            // pop attribute for cleanup
            GL.PopAttrib();

            // render splines
            DrawSplines(camera);

            // bone overlay
            RenderBoneOverlay();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool BranchIsVisible(HSD_JOBJ jobj, LiveJObj cache)
        {
            // TODO:
            //var parent = cache;
            //var visible = !jobj.Flags.HasFlag(JOBJ_FLAG.HIDDEN);

            //while (parent != null)
            //{
            //    if (Animation != null &&
            //        Animation.GetJointBranchState(Frame, parent.Index, out float branch))
            //        return branch != 0;

            //    parent = parent.Parent;
            //}

            //if (EnableHiddenFlag)
            //    return visible;
            //else
                return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void RenderJObjDisplay(Camera camera)
        {
            // render opaque dobjs first
            foreach (var opa in RenderDobjs.Where(e => !e._dobj.Mobj.RenderFlags.HasFlag(RENDER_MODE.XLU) && e.Parent.Desc.Flags.HasFlag(JOBJ_FLAG.OPA)))
            {
                if (opa.Visibile && opa.Parent.BranchVisible)
                    RenderDOBJShader(opa);
            }

            // render sorted xlu objects last
            foreach (var xlu in RenderDobjs.Where(e => e._dobj.Mobj.RenderFlags.HasFlag(RENDER_MODE.XLU) && e.Parent.Desc.Flags.HasFlag(JOBJ_FLAG.XLU)))
            {
                if (xlu.Visibile && xlu.Parent.BranchVisible)
                    RenderDOBJShader(xlu);
            }

            // render selection outline
            GL.DepthFunc(DepthFunction.Always);
            if (_settings.OutlineSelected)
                foreach (var i in RenderDobjs.Where(e => e.Selected))
                {
                    RenderDOBJShader(i, true);
                }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dobj"></param>
        /// <param name="selected"></param>
        private void RenderDOBJShader(RenderDObj dobj, bool selected = false)
        {
            // check if dobj has polygons to render
            if (dobj.PObjs.Count == 0)
                return;

            // get parent jobj
            JOBJ_FLAG jointFlags = dobj.Parent.Desc.Flags;

            // setup skeleton flag
            _shader.SetBoolToInt("isSkeleton", jointFlags.HasFlag(JOBJ_FLAG.SKELETON_ROOT) || jointFlags.HasFlag(JOBJ_FLAG.SKELETON));

            // setup single bind
            Matrix4 single = dobj.Parent.WorldTransform;
            GL.UniformMatrix4(_shader.GetVertexAttributeUniformLocation("singleBind"), false, ref single);

            // overlay params
            _shader.SetVector3("overlayColor", OverlayColor);
            _shader.SetBoolToInt("colorOverride", selected);

            // bind material
            if (!selected)
                SetupMObj(dobj);

            // bind buffer
            if (BufferManager.EnableBuffers(_shader, dobj._dobj, (int)ShapeBlend, (int)ShapeBlend + 1, ShapeBlend - (int)ShapeBlend))
            {
                // render pobjs
                foreach (var p in dobj.PObjs)
                {
                    // get flags
                    var pobjflags = p.pobj.Flags;

                    // load envelopes
                    GL.Uniform1(_shader.GetVertexAttributeUniformLocation("envelopeIndex"), p.Envelopes.Length, ref p.Envelopes[0]);

                    // load weights
                    GL.Uniform1(_shader.GetVertexAttributeUniformLocation("weights"), p.Weights.Length, ref p.Weights[0]);

                    // set uniform flag information
                    _shader.SetBoolToInt("hasEnvelopes", p.HasWeighting);
                    _shader.SetBoolToInt("enableParentTransform", !pobjflags.HasFlag(POBJ_FLAG.SHAPESET_AVERAGE));

                    // enable parent transform
                    if (p.pobj.Flags.HasFlag(POBJ_FLAG.UNKNOWN2))
                        _shader.SetInt("enableParentTransform", 2);

                    // set culling
                    GL.Enable(EnableCap.CullFace);
                    if (selected)
                    {
                        GL.CullFace(CullFaceMode.Front);
                        GL.PolygonMode(MaterialFace.Back, PolygonMode.Line);
                    }
                    else
                    if (pobjflags.HasFlag(POBJ_FLAG.CULLFRONT))
                    {
                        GL.CullFace(CullFaceMode.Front);
                        GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
                    }
                    else
                    if (pobjflags.HasFlag(POBJ_FLAG.CULLBACK))
                    {
                        GL.CullFace(CullFaceMode.Back);
                        GL.PolygonMode(MaterialFace.Back, PolygonMode.Fill);
                    }
                    else
                    {
                        GL.Disable(EnableCap.CullFace);
                    }

                    // draw display lists
                    foreach (var dl in p.DisplayLists)
                        GL.DrawArrays(dl.PrimType, dl.Offset, dl.Count);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mobj"></param>
        private void SetupMObj(RenderDObj dobj)
        {
            if (dobj._dobj.Mobj == null)
                return;

            var mobj = dobj._dobj.Mobj;
            MatAnimManager animation = null;

            GL.Enable(EnableCap.Texture2D);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Greater, 0f);

            GL.DepthMask(!mobj.RenderFlags.HasFlag(RENDER_MODE.NO_ZUPDATE));

            // Pixel Processing
            _shader.SetInt("alphaOp", -1); // none
            _shader.SetInt("alphaComp0", 7); // always
            _shader.SetInt("alphaComp1", 7);

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

                _shader.SetVector4("ambientColor", MaterialState.Ambient);
                _shader.SetVector4("diffuseColor", MaterialState.Diffuse);
                _shader.SetVector4("specularColor", MaterialState.Specular);
                _shader.SetFloat("shinniness", MaterialState.Shininess);
                _shader.SetFloat("alpha", MaterialState.Alpha);
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

                _shader.SetInt("alphaOp", (int)pp.AlphaOp);
                _shader.SetInt("alphaComp0", (int)pp.AlphaComp0);
                _shader.SetInt("alphaComp1", (int)pp.AlphaComp1);
                _shader.SetFloat("alphaRef0", MaterialState.Ref0);
                _shader.SetFloat("alphaRef1", MaterialState.Ref1);
            }

            var enableAll = mobj.RenderFlags.HasFlag(RENDER_MODE.DF_ALL);

            _shader.SetBoolToInt("no_zupdate", mobj.RenderFlags.HasFlag(RENDER_MODE.NO_ZUPDATE));
            _shader.SetBoolToInt("enableSpecular", dobj.Parent.Desc.Flags.HasFlag(JOBJ_FLAG.SPECULAR) && mobj.RenderFlags.HasFlag(RENDER_MODE.SPECULAR));
            _shader.SetBoolToInt("enableDiffuse", dobj.Parent.Desc.Flags.HasFlag(JOBJ_FLAG.LIGHTING) && mobj.RenderFlags.HasFlag(RENDER_MODE.DIFFUSE));
            _shader.SetBoolToInt("useConstant", mobj.RenderFlags.HasFlag(RENDER_MODE.CONSTANT));
            _shader.SetBoolToInt("useVertexColor", mobj.RenderFlags.HasFlag(RENDER_MODE.VERTEX));
            _shader.SetBoolToInt("useToonShading", mobj.RenderFlags.HasFlag(RENDER_MODE.TOON));

            // Textures
            for (int i = 0; i < MAX_TEX; i++)
                _shader.SetBoolToInt($"hasTEX[{i}]", mobj.RenderFlags.HasFlag(RENDER_MODE.TEX0 + (i << 4)) || enableAll);

            _shader.SetInt("BumpTexture", -1);

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
                        Math3D.CreateMatrix4FromEuler(tex.RX, tex.RY, tex.RY) *
                        Matrix4.CreateTranslation(tex.TX, tex.TY, tex.TZ);

                    if (tex.SY != 0 && tex.SX != 0 && tex.SZ != 0)
                        transform.Invert();

                    MatAnimTextureState texState = null;
                    if (animation != null)
                    {
                        texState = animation.GetTextureAnimState(tex);
                        if (texState != null)
                        {
                            displayTex = texState.TOBJ;
                            blending = texState.Blending;
                            transform = texState.Transform;
                        }
                    }

                    // make sure texture is loaded
                    PreLoadTexture(displayTex);

                    // grab texture id
                    var texid = TextureManager.GetGLID(imageBufferTextureIndex[displayTex.ImageData.ImageData]);

                    // set texture
                    GL.ActiveTexture(TextureUnit.Texture0 + i);
                    GL.BindTexture(TextureTarget.Texture2D, texid);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GXTranslator.toWrapMode(tex.WrapS));
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GXTranslator.toWrapMode(tex.WrapT));
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GXTranslator.toMagFilter(tex.MagFilter));
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureLodBias, 0); //640×548

                    if (tex.LOD != null)
                    {
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureLodBias, tex.LOD.Bias); //640×548
                    }

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

                    _shader.SetInt($"sampler{i}", i);
                    _shader.SetInt($"TEX[{i}].gensrc", (int)tex.GXTexGenSrc);
                    _shader.SetBoolToInt($"TEX[{i}].is_ambient", flags.HasFlag(TOBJ_FLAGS.LIGHTMAP_AMBIENT));
                    _shader.SetBoolToInt($"TEX[{i}].is_diffuse", flags.HasFlag(TOBJ_FLAGS.LIGHTMAP_DIFFUSE));
                    _shader.SetBoolToInt($"TEX[{i}].is_specular", flags.HasFlag(TOBJ_FLAGS.LIGHTMAP_SPECULAR));
                    _shader.SetBoolToInt($"TEX[{i}].is_ext", flags.HasFlag(TOBJ_FLAGS.LIGHTMAP_EXT));
                    _shader.SetBoolToInt($"TEX[{i}].is_bump", flags.HasFlag(TOBJ_FLAGS.BUMP));
                    _shader.SetInt($"TEX[{i}].color_operation", colorOP);
                    _shader.SetInt($"TEX[{i}].alpha_operation", alphaOP);
                    _shader.SetInt($"TEX[{i}].coord_type", coordType);
                    _shader.SetFloat($"TEX[{i}].blend", blending);
                    _shader.SetBoolToInt($"TEX[{i}].mirror_fix", mirrorY);
                    _shader.SetVector2($"TEX[{i}].uv_scale", wscale, hscale);
                    _shader.SetMatrix4x4($"TEX[{i}].transform", ref transform);

                    var tev = tex.TEV;
                    bool colorTev = tev != null && tev.active.HasFlag(TOBJ_TEVREG_ACTIVE.COLOR_TEV);
                    bool alphaTev = tev != null && tev.active.HasFlag(TOBJ_TEVREG_ACTIVE.ALPHA_TEV);
                    _shader.SetBoolToInt($"hasColorTev[{i}]", colorTev);
                    _shader.SetBoolToInt($"hasAlphaTev[{i}]", alphaTev);
                    if (colorTev)
                    {
                        _shader.SetInt($"Tev[{i}].color_op", (int)tev.color_op);
                        _shader.SetInt($"Tev[{i}].color_bias", (int)tev.color_bias);
                        _shader.SetInt($"Tev[{i}].color_scale", (int)tev.color_scale);
                        _shader.SetBoolToInt($"Tev[{i}].color_clamp", tev.color_clamp);
                        _shader.SetInt($"Tev[{i}].color_a", (int)tev.color_a_in);
                        _shader.SetInt($"Tev[{i}].color_b", (int)tev.color_b_in);
                        _shader.SetInt($"Tev[{i}].color_c", (int)tev.color_c_in);
                        _shader.SetInt($"Tev[{i}].color_d", (int)tev.color_d_in);
                    }
                    if (alphaTev)
                    {
                        _shader.SetInt($"Tev[{i}].alpha_op", (int)tev.alpha_op);
                        _shader.SetInt($"Tev[{i}].alpha_bias", (int)tev.alpha_bias);
                        _shader.SetInt($"Tev[{i}].alpha_scale", (int)tev.alpha_scale);
                        _shader.SetBoolToInt($"Tev[{i}].alpha_clamp", tev.alpha_clamp);
                        _shader.SetInt($"Tev[{i}].alpha_a", (int)tev.alpha_a_in);
                        _shader.SetInt($"Tev[{i}].alpha_b", (int)tev.alpha_b_in);
                        _shader.SetInt($"Tev[{i}].alpha_c", (int)tev.alpha_c_in);
                        _shader.SetInt($"Tev[{i}].alpha_d", (int)tev.alpha_d_in);
                    }
                    if (tev != null)
                    {
                        if ((tev.active & TOBJ_TEVREG_ACTIVE.TEV0) != 0)
                            _shader.SetColor($"Tev[{i}].tev0", tev.tev0, tev.tev0Alpha);

                        if ((tev.active & TOBJ_TEVREG_ACTIVE.TEV1) != 0)
                            _shader.SetColor($"Tev[{i}].tev1", tev.tev1, tev.tev1Alpha);

                        if ((tev.active & TOBJ_TEVREG_ACTIVE.KONST) != 0)
                        {
                            if (texState != null)
                            {
                                _shader.SetVector4($"Tev[{i}].konst", texState.Konst);
                            }
                            else
                            {
                                _shader.SetColor($"Tev[{i}].konst", tev.constant, tev.constantAlpha);
                            }
                        }
                    }
                }
            }
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

                if (tobj.LOD != null && tobj.ImageData.MaxLOD != 0)
                {
                    for (int i = 0; i < tobj.ImageData.MaxLOD - 1; i++)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shader"></param>
        public void SetupShader()
        {
            // nothing to render
            if (RootJObj == null)
                return;

            // update selected bone
            _shader.SelectedBone = RootJObj.GetIndexOfDesc(SelectedJObj);

            // set render mode
            _shader.RenderMode = RenderMode;

            // update shader bone transforms
            int i = 0;
            foreach (var v in RootJObj.Enumerate)
            {
                if (i < Shader.MAX_BONES)
                {
                    _shader.WorldTransforms[i] = v.WorldTransform;
                    _shader.WorldTransforms[i + Shader.MAX_BONES] = v.BindTransform;
                    i++;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RenderBoneOverlay()
        {
            if (!_settings.RenderBones)
                return;

            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.DepthTest);

            float mag = 0;

            if (_settings.RenderOrientation)
                mag = 2; //Vector3.TransformPosition(new Vector3(1, 0, 0), camera.MvpMatrix.Inverted()).Length / 30;

            foreach (var b in RootJObj.Enumerate)
                RenderBone(mag, b, b.Desc.Equals(SelectedJObj));

            GL.PopAttrib();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="parentTransform"></param>
        private void RenderBone(float mag, LiveJObj jobj, bool selected)
        {
            Matrix4 transform = jobj.WorldTransform;

            if (jobj.Parent != null)
            {
                Matrix4 parentTransform = jobj.Parent.WorldTransform;

                var bonePosition = transform.ExtractTranslation();
                var parentPosition = parentTransform.ExtractTranslation();

                GL.LineWidth(1f);
                GL.Begin(PrimitiveType.Lines);
                GL.Color3(0f, 1f, 0f);
                GL.Vertex3(parentPosition);
                GL.Color3(0f, 0f, 1f);
                GL.Vertex3(bonePosition);
                GL.End();
            }

            if (selected)
            {
                GL.Color3(1f, 1f, 0f);
                GL.PointSize(7f);
            }
            else
            {
                GL.Color3(1f, 0f, 0f);
                GL.PointSize(5f);
            }

            GL.PushMatrix();
            GL.MultMatrix(ref transform);

            GL.Begin(PrimitiveType.Points);
            GL.Vertex3(0, 0, 0);
            GL.End();

            if (_settings.RenderOrientation)
            {
                GL.LineWidth(1.5f);

                GL.Begin(PrimitiveType.Lines);
                GL.Color3(1f, 0f, 0f);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(mag, 0, 0);
                GL.Color3(0f, 1f, 0f);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0, mag, 0);
                GL.Color3(0f, 0f, 1f);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0, 0, mag);
                GL.End();
            }

            GL.PopMatrix();
        }

        /// <summary>
        /// 
        /// </summary>
        private void DrawSplines(Camera camera)
        {
            if (RootJObj == null)
                return;

            foreach (var j in RootJObj.Enumerate)
                if (j.Desc.Spline != null)
                    DrawShape.RenderSpline(j.Desc.Spline, Color.Yellow, Color.Blue);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearDObjSelection()
        {
            foreach (var d in RenderDobjs)
                d.Selected = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="selected"></param>
        public void SetDObjSelected(int index, bool selected)
        {
            if (index >= 0 && index < RenderDobjs.Count)
                RenderDobjs[index].Selected = selected;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="selected"></param>
        public void SetDObjVisible(int index, bool visible)
        {
            if (index >= 0 && index < RenderDobjs.Count)
                RenderDobjs[index].Visibile = visible;
        }
    }
}
