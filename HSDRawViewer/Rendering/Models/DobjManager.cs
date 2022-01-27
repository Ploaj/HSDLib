using HSDRaw.Common;
using HSDRaw.GX;
using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using HSDRawViewer.Rendering.GX;
using HSDRaw;

namespace HSDRawViewer.Rendering.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class DOBJManager
    {
        public int DOBJCount { get => DOBJtoPOBJCache.Count; }

        public bool RenderTextures { get; set; } = true;

        public bool RenderVertexColor { get; set; } = true;

        public bool OnlyRenderSelected { get; set; } = false;

        public HSD_DOBJ SelectedDOBJ;

        // Attributes
        public Vector3 OverlayColor = Vector3.One;

        public float ShapeBlend = 0;

        public List<HSD_DOBJ> HiddenDOBJs { get; internal set; } = new List<HSD_DOBJ>();

        // Resource Managers
        private VertexBufferManager BufferManager = new VertexBufferManager();
        private MobjManager MOBJManager = new MobjManager();

        // Rendering Cache
        private Dictionary<HSD_DOBJ, List<CachedPOBJ>> DOBJtoPOBJCache = new Dictionary<HSD_DOBJ, List<CachedPOBJ>>();

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

        /// <summary>
        /// 
        /// </summary>
        public void ClearRenderingCache()
        {
            // TODO: shader is currently static
            //if(GXShader != null)
            //    GXShader.Delete();
            //GXShader = null;

            BufferManager.ClearRenderingCache();
            MOBJManager.ClearRenderingCache();

            DOBJtoPOBJCache.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="dobj"></param>
        /// <param name="parentJOBJ"></param>
        /// <param name="jobjManager"></param>
        public void RenderDOBJShader(Shader shader, HSD_DOBJ dobj, HSD_JOBJ parentJOBJ, JOBJManager jobjManager, MatAnimManager matAnim, bool selected = false)
        {
            // check if shader is ready
            if (shader == null)
                return;

            // check if dobj has polygons to render
            if (dobj.Pobj == null)
                return;

            // check if dobj is hidden
            if (HiddenDOBJs.Contains(dobj) || (selected && OnlyRenderSelected))
                return;

            // check selection mode
            if (OnlyRenderSelected && SelectedDOBJ != null && SelectedDOBJ._s != dobj._s)
                return;

            // process model if dobj is missing
            if (!DOBJtoPOBJCache.ContainsKey(dobj))
                if(!LoadModel(jobjManager))
                    return;

            // setup skeleton flag
            shader.SetBoolToInt("isSkeleton", parentJOBJ.Flags.HasFlag(JOBJ_FLAG.SKELETON_ROOT) || parentJOBJ.Flags.HasFlag(JOBJ_FLAG.SKELETON));

            // setup single bind
            Matrix4 single = Matrix4.Identity;
            if (parentJOBJ != null && jobjManager != null)
                single = jobjManager.GetWorldTransform(parentJOBJ);
            GL.UniformMatrix4(shader.GetVertexAttributeUniformLocation("singleBind"), false, ref single);

            // overlay params
            shader.SetVector3("overlayColor", OverlayColor);
            shader.SetBoolToInt("colorOverride", selected);

            // bind material
            if(!selected)
                MOBJManager.BindMOBJ(shader, dobj.Mobj, parentJOBJ, matAnim);

            // bind buffer
            if(BufferManager.BindBuffer(shader, dobj, (int)ShapeBlend, (int)ShapeBlend + 1, ShapeBlend - (int)ShapeBlend))
            {
                // render pobjs
                foreach (var p in DOBJtoPOBJCache[dobj])
                {
                    // load envelopes
                    var en = p.Envelopes;
                    GL.Uniform4(shader.GetVertexAttributeUniformLocation("envelopeIndex"), p.Envelopes.Length, ref p.Envelopes[0].X);

                    // load weights
                    var we = p.Weights;
                    GL.Uniform4(shader.GetVertexAttributeUniformLocation("weights"), p.Weights.Length, ref p.Weights[0].X);

                    // set uniform flag information
                    shader.SetBoolToInt("hasEnvelopes", p.HasWeighting);
                    shader.SetBoolToInt("enableParentTransform", !p.Flag.HasFlag(POBJ_FLAG.UNKNOWN0));

                    // set culling
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

                    // draw display lists
                    foreach (var dl in p.DisplayLists)
                        GL.DrawArrays(dl.PrimType, dl.Offset, dl.Count);
                }

                // unbind buffer
                BufferManager.Unbind(shader);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private bool LoadModel(JOBJManager manager)
        {
            var root = manager.GetJOBJ(0);

            // no model to render
            if (root == null)
                return false;

            // clear cache
            ClearRenderingCache();

            // get all dobjs
            List<HSD_DOBJ> dobj = new List<HSD_DOBJ>(0);

            foreach (var j in root.BreathFirstList)
                if (j.Dobj != null)
                    dobj.AddRange(j.Dobj.List);

            // 
            Dictionary<HSDStruct, GX_Attribute[]> structToAttrs = new Dictionary<HSDStruct, GX_Attribute[]>();
            foreach(var v in dobj)
            {
                if(v.Pobj != null)
                {
                    foreach(var pobj in v.Pobj.List)
                    {
                        var attrs = pobj.ToGXAttributes();
                        var attr = pobj._s.GetReference<HSDAccessor>(0x08)._s;

                        // only add complete attributes
                        // complete attributes end in null attribute
                        if(!structToAttrs.ContainsKey(attr) && 
                            attrs[attrs.Length - 1].AttributeName == GXAttribName.GX_VA_NULL)
                            structToAttrs.Add(attr, attrs);
                    }
                }
            }

            // prepare dobjs for rendering

            foreach (var v in dobj)
                if (!LoadDOBJ(v, manager, structToAttrs))
                {
                    ClearRenderingCache();
                    return false;
                }

            // success
            return true;
        }

        /// <summary>
        /// Prepares DOBJ for rendering by loading relevant information into a cache
        /// </summary>
        private bool LoadDOBJ(HSD_DOBJ dobj, JOBJManager jobjManager, Dictionary<HSDStruct, GX_Attribute[]> structToAttrs)
        {
            if (dobj.Pobj == null)
                return true;

            List<CachedPOBJ> pobjs = new List<CachedPOBJ>();
            List<GX_Vertex> vertices = new List<GX_Vertex>();
            List<List<GX_Shape>> shapesets = new List<List<GX_Shape>>();

            int off = 0;
            foreach (var pobj in dobj.Pobj.List)
            {
                if (!pobj._s.References.ContainsKey(0x08))
                    continue;

                var attrStruct = pobj._s.References[0x08];

                // if attributes not found skip this pobj
                if (!structToAttrs.ContainsKey(attrStruct))
                    continue;

                // get display list
                var dl = pobj.ToDisplayList(structToAttrs[attrStruct]);

                // add vertices
                vertices.AddRange(dl.Vertices);

                // shape sets
                for (int i = 0; i < dl.ShapeSets.Count; i++)
                {
                    while (i >= shapesets.Count)
                        shapesets.Add(new List<GX_Shape>());

                    shapesets[i].AddRange(dl.ShapeSets[i]);
                }
                

                // create pobj cache
                var pobjCache = new CachedPOBJ()
                {
                    POBJ = pobj
                };

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

                        var jobj = v.GetJOBJAt(i);

                        w[i] = v.GetWeightAt(i);
                        b[i] = jobjManager.IndexOf(jobj);

                        if (jobj != null && jobj.InverseWorldTransform == null || b[i] == -1)
                            Console.WriteLine("Warning: Inverse Matrix not set");

                        if (jobj != null && jobj.InverseWorldTransform != null && !jobj.Flags.HasFlag(JOBJ_FLAG.SKELETON))
                            Console.WriteLine("Skeleton flag not set");
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

            // convert list to array
            BufferManager.AddBuffer(dobj, vertices.ToArray());
            BufferManager.AddShapeSets(dobj, shapesets);
            DOBJtoPOBJCache.Add(dobj, pobjs);

            return true;
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
                            MOBJManager.PreLoadTexture(texture);
        }
    }
}
