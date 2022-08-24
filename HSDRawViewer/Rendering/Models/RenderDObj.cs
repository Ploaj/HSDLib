using HSDRaw.Common;
using HSDRaw.GX;
using HSDRawViewer.Rendering.GX;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDRawViewer.Rendering.Models
{
    internal class RenderDObj
    {
        public LiveJObj Parent { get; internal set; }

        public HSD_DOBJ _dobj { get; internal set; }

        public int JointIndex { get; internal set; }

        public int DisplayIndex { get; internal set; }

        public List<RenderPObj> PObjs { get; internal set; } = new List<RenderPObj>();

        public bool Selected { get; set; }

        public bool Visible { get; set; } = true;

        public float ShapeBlend { get; set; } = 1;

        public MatAnimMaterialState MaterialState { get; set; } = new MatAnimMaterialState();

        public MatAnimTextureState[] TextureStates { get; internal set; } = new MatAnimTextureState[8];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="dobj"></param>
        public RenderDObj(LiveJObj parent, HSD_DOBJ dobj)
        {
            Parent = parent;
            _dobj = dobj;

            // get indicies
            JointIndex = parent.Root.GetIndexOfDesc(parent.Desc);
            DisplayIndex = parent.Desc.Dobj.List.IndexOf(dobj);

            // initialize texture states
            for (int i = 0; i < TextureStates.Length; i++)
                TextureStates[i] = new MatAnimTextureState();

            // initialize material state
            ResetMaterialState();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetMaterialState()
        {
            if (_dobj == null || _dobj.Mobj == null)
                return;

            // initial material color
            MaterialState.Reset(_dobj.Mobj);

            // initialize texture data
            if (_dobj.Mobj.Textures != null)
            {
                int ti = 0;
                foreach (var t in _dobj.Mobj.Textures.List)
                {
                    // too many textures
                    if (ti >= TextureStates.Length)
                        break;

                    // get next texture state
                    var ts = TextureStates[ti];

                    // reset
                    ts.Reset(t);

                    //
                    ti++;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vb"></param>
        public void InitializeBufferData(VertexBufferManager vb)
        {
            if (_dobj == null)
                return;

            if (_dobj.Pobj == null)
                return;

            // List<LivePObj> pobjs = new List<LivePObj>();
            List<GX_Vertex> vertices = new List<GX_Vertex>();
            List<List<GX_Shape>> shapesets = new List<List<GX_Shape>>();
            PObjs.Clear();

            int off = 0;
            foreach (var pobj in _dobj.Pobj.List)
            {
                // skip if attributes are blank
                if (pobj.Attributes == null)
                    continue;

                // get attributes
                var attrs = pobj.ToGXAttributes();

                // skip if attributes are incomplete
                // incomplete attributes don't end with null
                if (attrs[attrs.Length - 1].AttributeName != GXAttribName.GX_VA_NULL)
                    continue;

                // get display list
                var dl = pobj.ToDisplayList(attrs);

                // add vertices
                vertices.AddRange(dl.Vertices);

                // add shape sets
                for (int i = 0; i < dl.ShapeSets.Count; i++)
                {
                    while (i >= shapesets.Count)
                        shapesets.Add(new List<GX_Shape>());

                    shapesets[i].AddRange(dl.ShapeSets[i]);
                }


                // create pobj cache
                var pobjCache = new RenderPObj(pobj);

                // init envelopes
                pobjCache.InitEnvelopes(dl, Parent);

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

                PObjs.Add(pobjCache);
            }

            // add buffers
            vb.AddBuffer(_dobj, vertices.ToArray());
            vb.AddShapeSets(_dobj, shapesets);
        }
    }

    internal class RenderPObj
    {
        private static int MAX_WEIGHTS = 6;
        private static int WEIGHT_STRIDE = 10;

        public HSD_POBJ pobj { get; internal set; }

        public int EnvelopeCount = 0;
        public int[] Envelopes = new int[MAX_WEIGHTS * WEIGHT_STRIDE];
        public float[] Weights = new float[MAX_WEIGHTS * WEIGHT_STRIDE];

        public bool HasWeighting = false;

        public List<CachedDL> DisplayLists = new List<CachedDL>();

        public RenderPObj(HSD_POBJ pobj)
        {
            this.pobj = pobj;
        }

        public void InitEnvelopes(GX_DisplayList dl, LiveJObj parent)
        {
            // get skeleton root
            LiveJObj root = parent.Root;

            // build envelopes
            if (pobj.Flags.HasFlag(POBJ_FLAG.UNKNOWN2))
            {
                Envelopes[0 * MAX_WEIGHTS + 0] = root.GetIndexOfDesc(parent.Desc);
                Envelopes[1 * MAX_WEIGHTS + 0] = parent.Index;

                EnvelopeCount = 2;
                HasWeighting = false;
            }
            else
            {
                int eni = 0;
                foreach (var v in dl.Envelopes)
                {
                    if (eni >= WEIGHT_STRIDE)
                        break;

                    for (int i = 0; i < v.EnvelopeCount; i++)
                    {
                        if (i >= MAX_WEIGHTS)
                            break;

                        var jobj = v.GetJOBJAt(i);
                        var jobjIndex = root.GetIndexOfDesc(jobj);

                        Envelopes[eni * MAX_WEIGHTS + i] = jobjIndex;
                        Weights[eni * MAX_WEIGHTS + i] = v.GetWeightAt(i);

                        if (jobj != null && jobj.InverseWorldTransform == null || jobjIndex == -1)
                            Console.WriteLine("Warning: Inverse Matrix not set");

                        if (jobj != null && jobj.InverseWorldTransform != null && !jobj.Flags.HasFlag(JOBJ_FLAG.SKELETON))
                            Console.WriteLine("Skeleton flag not set");
                    }
                    eni++;
                    EnvelopeCount = v.EnvelopeCount;
                    HasWeighting = v.EnvelopeCount > 0;
                }
            }
        }
    }

    internal struct CachedDL
    {
        public PrimitiveType PrimType;

        public int Offset;

        public int Count;
    }
}
