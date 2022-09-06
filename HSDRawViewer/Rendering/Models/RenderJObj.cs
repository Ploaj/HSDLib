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
using HSDRaw.Common.Animation;
using HSDRaw.Tools;

namespace HSDRawViewer.Rendering.Models
{
    [Flags]
    public enum FrameFlags
    {
        None = 0,
        Joint = 1,
        Material = 2,
        Shape = 4,
        All = Joint | Material | Shape,
    }
    public class RenderJObj
    {
        private static int MAX_TEX { get; } = 4;
        private static int MAX_LIGHTS { get; } = 4;

        private GXShader _shader;

        public LiveJObj RootJObj { get; internal set; }

        private bool Initialized = false;

        public RenderMode RenderMode { get; set; }

        public RenderLObj[] _lights { get; } = new RenderLObj[MAX_LIGHTS];
        private RenderLObj cameraLight = new RenderLObj();
        private RenderLObj cameraAmbient = new RenderLObj();

        public GXFogParam _fogParam { get; internal set; } = new GXFogParam();

        public JobjDisplaySettings _settings { get; internal set; } = new JobjDisplaySettings();

        public float ModelScale { get; set; } = 1;

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

        /// <summary>
        /// 
        /// </summary>
        public JointAnimManager JointAnim { get; internal set; }

        /// <summary>
        /// 
        /// </summary>

        public MatAnimManager MatAnim { get; internal set; }

        private bool UpdateMaterialFrame = false;

        /// <summary>
        /// 
        /// </summary>
        public HSD_ShapeAnimJoint ShapeAnim { get; internal set; }


        public delegate void OnIntialize();
        public OnIntialize Initialize;


        /// <summary>
        /// 
        /// </summary>
        public RenderJObj()
        {
            cameraLight.Enabled = true;
            cameraLight.Type = LObjType.INFINITE;

            cameraAmbient.Enabled = true;
            cameraAmbient.Type = LObjType.AMBIENT;
            cameraAmbient.Color = new Vector4(179, 179, 179, 255) / 255f;

            for (int i = 0; i < MAX_LIGHTS; i++)
                _lights[i] = new RenderLObj();

            _lights[0].Enabled = true;
            _lights[0].Type = LObjType.AMBIENT;
            _lights[0].Color = new Vector4(255, 255, 255, 255) / 255f;

            _lights[1].Enabled = true;
            _lights[1].Type = LObjType.INFINITE;
            _lights[1].Position = new Vector3(0, 12, 9);
            _lights[1].Color = new Vector4(200, 200, 200, 255) / 255f;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="desc"></param>
        public RenderJObj(HSD_JOBJ desc) : base()
        {
            LoadJObj(desc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="state"></param>
        public void SetMaterialAnimation(int dobj_index, float frame, List<FOBJ_Player> tracks, IEnumerable<List<FOBJ_Player>> textureStates, List<List<HSD_TOBJ>> textures)
        {
            if (dobj_index >= 0 && dobj_index < RenderDobjs.Count)
            {
                RenderDobjs[dobj_index].MaterialState.ApplyAnim(tracks, frame);

                int ti = 0;
                foreach (var t in textureStates)
                {
                    if (ti >= RenderDobjs[dobj_index].TextureStates.Length)
                        break;

                    RenderDobjs[dobj_index].TextureStates[ti].ApplyAnim(textures[ti], t, frame);
                    ti++;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void RequestAnimationUpdate(FrameFlags flags, float frame)
        {
            if (flags.HasFlag(FrameFlags.Joint) && RootJObj != null && JointAnim != null)
            {
                JointAnim.ApplyAnimation(RootJObj, frame);
            }

            if (flags.HasFlag(FrameFlags.Material) && MatAnim != null)
            {
                // if frame is -1 don't actually update the frame values
                if (frame != -1)
                    MatAnim.SetAllFrames(frame);
                UpdateMaterialFrame = true;
            }

            // TODO: shape animation
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        public void ClearAnimation(FrameFlags flags)
        {
            if (flags.HasFlag(FrameFlags.Joint))
            {
                JointAnim = null;
                ResetDefaultStateJoints();
            }

            if (flags.HasFlag(FrameFlags.Material))
            {
                MatAnim = null;
                ResetDefaultStateMaterial();
            }

            // TODO: shape animation
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadAnimation(JointAnimManager joint, HSD_MatAnimJoint material, HSD_ShapeAnimJoint shape)
        {
            FrameFlags flags = FrameFlags.None;

            if (joint != null)
            {
                flags |= FrameFlags.Joint;
                JointAnim = joint;
            }

            if (material != null)
            {
                flags |= FrameFlags.Material;
                MatAnim = new MatAnimManager(material);
            }

            if (shape != null)
            {
                ShapeAnim = shape;
                flags |= FrameFlags.Shape;
            }

            // request anim update
            RequestAnimationUpdate(flags, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetDefaultStateAll()
        {
            ResetDefaultStateJoints();
            ResetDefaultStateMaterial();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetDefaultStateJoints()
        {
            // reset skeleton
            RootJObj?.ResetTransforms();
        }


        /// <summary>
        /// 
        /// </summary>
        public void ResetDefaultStateMaterial()
        {
            // reset materials
            foreach (var d in RenderDobjs)
                d.ResetMaterialState();
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadJObj(HSD_JOBJ desc)
        {
            ClearAnimation(FrameFlags.Material | FrameFlags.Shape);
            RootJObj = new LiveJObj(desc);
            Initialized = false;
        }

        /// <summary>
        /// This will free all opengl resources used for rendering
        /// </summary>
        public void FreeResources()
        {
            // initialize shader
            if (_shader != null)
            {
                _shader.Dispose();
                _shader = null;
            }

            // clear previous caches
            BufferManager.ClearRenderingCache();
            TextureManager.ClearTextures();
            imageBufferTextureIndex.Clear();
            RenderDobjs.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public void InvalidateDObjOrder()
        {
            // determine new order
            Dictionary<HSD_DOBJ, int> newOrder = new Dictionary<HSD_DOBJ, int>(RenderDobjs.Count);
            int i = 0;
            foreach (var j in RootJObj?.Enumerate)
            {
                if (j.Desc.Dobj == null)
                    continue;
                foreach (var d in j.Desc.Dobj.List)
                    newOrder.Add(d, i++);
            }

            // order render dobjs
            RenderDobjs = RenderDobjs.OrderBy(e => newOrder[e._dobj]).ToList();

            // update display index
            foreach (var d in RenderDobjs)
                d.DisplayIndex = d.Parent.Desc.Dobj.List.IndexOf(d._dobj);
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeRendering()
        {
            Initialized = true;

            // free old resources
            FreeResources();

            // initialize shader
            _shader = new GXShader();

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

            // re apply animation after invalidating
            RequestAnimationUpdate(FrameFlags.All, 0);

            // print diagnostic info
            System.Diagnostics.Debug.WriteLine($"Buffer Count: {BufferManager.BufferCount}");
            System.Diagnostics.Debug.WriteLine($"Texture Count: {TextureManager.TextureCount}");

            // callback done initializing
            Initialize?.Invoke();
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
            {
                var temp = RootJObj.Scale;
                RootJObj.Scale *= ModelScale;
                RootJObj.RecalculateTransforms(camera, true);
                RootJObj.Scale = temp;
            }

            // apply material animation update during render only
            DrawUpdateMaterialAnimation();

            // enable depth test
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            
            // prepare shader
            SetupShader();

            // render with shader
            _shader.Bind(camera, _fogParam);

            // lighting
            _shader.SetFloat("saturate", 1);
            _shader.SetBoolToInt("perPixelLighting", true);
            if (!_settings.UseCameraLight)
            {
                //for (int i = 0; i < MAX_LIGHTS; i++)
                //{
                //    if (i < TestingLights.Trophy1.Length)
                //        TestingLights.Trophy1[i].Bind(_shader, i);
                //    else
                //        _shader.SetBoolToInt($"light[{i}].enabled", false);
                //}

                for (int i = 0; i < MAX_LIGHTS; i++)
                    _lights[i].Bind(_shader, i);
            }
            else
            {
                for (int i = 0; i < MAX_LIGHTS; i++)
                {
                    if (i == 0)
                    {
                        cameraAmbient.Bind(_shader, i);
                    }
                    else
                    if (i == 1)
                    {
                        cameraLight.Position = camera.TransformedPosition;
                        cameraLight.Bind(_shader, i);
                    }
                    else
                    {
                        _shader.SetBoolToInt($"light[{i}].enabled", false);
                    }
                }
            }

            // Render DOBJS
            RenderJObjDisplay(camera);

            // unbind shader
            _shader.Unbind();

            // pop attribute for cleanup
            GL.PopAttrib();

            // render splines
            DrawSplines(camera);

            // draw lights
            //for (int i = 0; i < MAX_LIGHTS; i++)
            //{
            //    if (_lights[i].Enabled && _lights[i].Type != LObjType.AMBIENT)
            //        DrawShape.DrawSphere(Matrix4.CreateTranslation(_lights[i].Position), 1, 10, 10, _lights[i].Color.Xyz, 1);
            //}

            // bone overlay
            RenderBoneOverlay();
        }

        /// <summary>
        /// 
        /// </summary>
        private void DrawUpdateMaterialAnimation()
        {
            if (UpdateMaterialFrame)
            {
                // apply animation to all render dobjs
                foreach (var v in RenderDobjs)
                {
                    // get material state
                    LiveMaterial state = v.MaterialState;
                    MatAnim.GetMaterialState(v._dobj.Mobj, v.JointIndex, v.DisplayIndex, ref state);
                    v.MaterialState = state;

                    // get texture states
                    if (v._dobj.Mobj.Textures != null)
                    {
                        int ti = 0;
                        foreach (var i in v._dobj.Mobj.Textures.List)
                        {
                            if (ti < v.TextureStates.Length)
                            {
                                var ts = v.TextureStates[ti];

                                MatAnim.GetTextureAnimState(i.TexMapID, v.JointIndex, v.DisplayIndex, ref ts);

                                v.TextureStates[ti] = ts;
                            }
                            ti++;
                        }
                    }
                }

                UpdateMaterialFrame = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="visible"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        private bool DisplayObject(bool visible, bool selected)
        {
            switch (_settings.RenderObjects)
            {
                case ObjectRenderMode.All:
                    return true;
                case ObjectRenderMode.None:
                    return false;
                case ObjectRenderMode.Selected:
                    return selected;
                case ObjectRenderMode.Visible:
                    return visible;
            }

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
                if (DisplayObject(opa.Visible, opa.Selected) && opa.Parent.BranchVisible)
                    RenderDOBJShader(opa);
            }

            // render sorted xlu objects last
            foreach (var xlu in RenderDobjs.Where(e => e._dobj.Mobj.RenderFlags.HasFlag(RENDER_MODE.XLU) && e.Parent.Desc.Flags.HasFlag(JOBJ_FLAG.XLU)))
            {
                if (DisplayObject(xlu.Visible, xlu.Selected) && xlu.Parent.BranchVisible)
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
            _shader.SetBoolToInt("isSkeleton", jointFlags.HasFlag(JOBJ_FLAG.SKELETON_ROOT));// || jointFlags.HasFlag(JOBJ_FLAG.SKELETON));

            // setup single bind
            Matrix4 single = dobj.Parent.WorldTransform;
            GL.UniformMatrix4(_shader.GetVertexAttributeUniformLocation("singleBind"), false, ref single);

            // overlay params
            _shader.SetVector3("overlayColor", OverlayColor);
            _shader.SetBoolToInt("colorOverride", selected);

            // bind material
            if (!selected)
                SetupMObj(dobj);

            // get shape blending
            float shapeBlend = dobj.ShapeBlend;

            // bind buffer
            if (BufferManager.EnableBuffers(_shader, dobj._dobj, (int)shapeBlend, (int)shapeBlend + 1, shapeBlend - (int)shapeBlend))
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
            if (dobj == null && dobj._dobj != null && dobj._dobj.Mobj != null)
                return;

            var mobj = dobj._dobj.Mobj;
            var MaterialState = dobj.MaterialState;
            var textureStates = dobj.TextureStates;
            var parentJOBJ = dobj.Parent.Desc;

            // init GL state
            GL.Enable(EnableCap.Texture2D);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Greater, 0f);

            GL.DepthMask(!mobj.RenderFlags.HasFlag(RENDER_MODE.NO_ZUPDATE));
            GL.DepthFunc(DepthFunction.Lequal);

            // Pixel Processing
            _shader.SetInt("alphaOp", -1); // none
            _shader.SetInt("alphaComp0", 7); // always
            _shader.SetInt("alphaComp1", 7);

            // Materials
            _shader.SetVector4("ambientColor", MaterialState.Ambient);
            _shader.SetVector4("diffuseColor", MaterialState.Diffuse);
            _shader.SetVector4("specularColor", MaterialState.Specular);
            _shader.SetFloat("shinniness", MaterialState.Shininess);
            _shader.SetFloat("alpha", MaterialState.Alpha);

            // pixel processing
            var pp = mobj.PEDesc;
            if (pp != null)
            {
                GL.BlendFunc(GXTranslator.toBlendingFactor(pp.SrcFactor), GXTranslator.toBlendingFactor(pp.DstFactor));
                GL.DepthFunc(GXTranslator.toDepthFunction(pp.DepthFunction));

                _shader.SetInt("alphaOp", (int)pp.AlphaOp);
                _shader.SetInt("alphaComp0", (int)pp.AlphaComp0);
                _shader.SetInt("alphaComp1", (int)pp.AlphaComp1);
                _shader.SetFloat("alphaRef0", MaterialState.Ref0);
                _shader.SetFloat("alphaRef1", MaterialState.Ref1);
            }

            // all flag
            var enableAll = mobj.RenderFlags.HasFlag(RENDER_MODE.DF_ALL);

            _shader.SetBoolToInt("no_zupdate", mobj.RenderFlags.HasFlag(RENDER_MODE.NO_ZUPDATE));
            _shader.SetBoolToInt("enableSpecular", parentJOBJ.Flags.HasFlag(JOBJ_FLAG.SPECULAR) && mobj.RenderFlags.HasFlag(RENDER_MODE.SPECULAR));
            _shader.SetBoolToInt("enableDiffuse", parentJOBJ.Flags.HasFlag(JOBJ_FLAG.LIGHTING) && mobj.RenderFlags.HasFlag(RENDER_MODE.DIFFUSE));
            _shader.SetBoolToInt("useConstant", mobj.RenderFlags.HasFlag(RENDER_MODE.CONSTANT));
            _shader.SetBoolToInt("useVertexColor", mobj.RenderFlags.HasFlag(RENDER_MODE.VERTEX));
            _shader.SetBoolToInt("useToonShading", mobj.RenderFlags.HasFlag(RENDER_MODE.TOON));

            // Textures
            for (int i = 0; i < MAX_TEX; i++)
                _shader.SetBoolToInt($"hasTEX[{i}]", mobj.RenderFlags.HasFlag(RENDER_MODE.TEX0 + (i << 4)) || enableAll);

            // initialize bump texture to unused
            _shader.SetInt("BumpTexture", -1);

            // Bind Textures
            if (mobj.Textures != null)
            {
                var textures = mobj.Textures.List;
                for (int i = 0; i < textures.Count; i++)
                {
                    // make sure texture is not out of supported range
                    if (i > MAX_TEX)
                        break;

                    // get texture
                    var tex = textures[i];
                    var tev = tex.TEV;

                    // texture state info
                    HSD_TOBJ displayTex;
                    float blending;
                    Matrix4 transform;
                    Vector4 konst = Vector4.Zero;
                    Vector4 tev0 = Vector4.Zero;
                    Vector4 tev1 = Vector4.Zero;

                    // get texture state data if it exists
                    if (textureStates != null && i < textureStates.Length && textureStates[i] != null)
                    {
                        LiveTObj texState = textureStates[i];
                        displayTex = texState.TOBJ;
                        blending = texState.Blending;
                        transform = texState.Transform;
                        if (tev != null)
                        {
                            konst = texState.Konst;
                            tev0 = texState.Tev0;
                            tev1 = texState.Tev1;
                        }
                    }
                    else
                    {
                        displayTex = tex;
                        blending = tex.Blending;
                        transform = Matrix4.Identity;

                        if (tev != null)
                        {
                            konst = new Vector4(tev.constant.R / 255f, tev.constant.B / 255f, tev.constant.G / 255f, tev.constantAlpha / 255f);
                            tev0 = new Vector4(tev.tev0.R / 255f, tev.tev0.B / 255f, tev.tev0.G / 255f, tev.tev0Alpha / 255f);
                            tev1 = new Vector4(tev.tev1.R / 255f, tev.tev1.B / 255f, tev.tev1.G / 255f, tev.tev1Alpha / 255f);
                        }
                    }

                    // if texture image data is null skip setup?
                    if (tex.ImageData == null)
                        continue;

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

                    // optional texture mipmap coords
                    if (tex.LOD != null)
                    {
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureLodBias, tex.LOD.Bias); //640×548
                    }

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
                    _shader.SetMatrix4x4($"TEX[{i}].transform", ref transform);

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

                    _shader.SetVector4($"Tev[{i}].konst", konst);
                    _shader.SetVector4($"Tev[{i}].tev0", tev0);
                    _shader.SetVector4($"Tev[{i}].tev1", tev1);
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

            if (_shader == null)
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
                    DrawShape.RenderSpline(j.WorldTransform, j.Desc.Spline, Color.Yellow, Color.Blue);
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
                RenderDobjs[index].Visible = visible;
        }
    }
}
