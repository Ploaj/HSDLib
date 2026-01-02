using HSDRaw.Common;
using HSDRaw.GX;
using HSDRaw.Tools;
using HSDRawViewer.Extensions;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace HSDRawViewer.IO.Model
{
    public class HsdSkin
    {
        [JsonConverter(typeof(JsonInlineListConverter<List<int>>))]
        public List<List<int>> Bones { get; set; } = new();

        [JsonConverter(typeof(JsonInlineListConverter<List<float>>))]
        public List<List<float>> Weights { get; set; } = new();

        public HsdSkin()
        {
        }

        public HsdSkin(LiveJObj live, HSD_Envelope e)
        {
        }
    }

    public class HsdMsh
    {
        public string Name { get; set; }

        public HsdMat Material { get; set; } = new HsdMat();

        [JsonConverter(typeof(JsonInlineDictionaryListConverter<GXAttribName, float>))]
        public Dictionary<GXAttribName, List<float>> Data { get; set; } = new();

        public HsdSkin Skin { get; set; } = new();

        [JsonConverter(typeof(JsonInlineListConverter<int>))]
        public List<int> Triangles { get; set; } = new();

        public HsdMsh()
        {

        }

        public sealed class SegmentEpsilonComparer : IEqualityComparer<GX_Vertex>
        {
            private readonly float _epsilon;
            private readonly float _invEpsilon;

            public SegmentEpsilonComparer(float epsilon)
            {
                _epsilon = epsilon;
                _invEpsilon = 1f / epsilon;
            }

            public bool Equals(GX_Vertex x, GX_Vertex y)
            {
                return NearlyEqual(x.POS, y.POS) &&
                       NearlyEqual(x.NRM, y.NRM);
            }

            public int GetHashCode(GX_Vertex s)
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 31 + QuantizedHash(s.POS);
                    hash = hash * 31 + QuantizedHash(s.NRM);
                    return hash;
                }
            }

            private bool NearlyEqual(GXVector3 a, GXVector3 b)
            {
                return (a.ToTKVector() - b.ToTKVector()).LengthSquared <= _epsilon * _epsilon;
            }

            private int QuantizedHash(GXVector3 v)
            {
                int x = (int)MathF.Round(v.X * _invEpsilon);
                int y = (int)MathF.Round(v.Y * _invEpsilon);
                int z = (int)MathF.Round(v.Z * _invEpsilon);

                unchecked
                {
                    int hash = 17;
                    hash = hash * 31 + x;
                    hash = hash * 31 + y;
                    hash = hash * 31 + z;
                    return hash;
                }
            }
        }

        private void Optimize(List<GX_Vertex> verts, List<int> tri)
        {
            List<GX_Vertex> nv = new List<GX_Vertex>();
            List<int> nt = new List<int>();

            //float epsilon = 0.01f;
            Dictionary<GX_Vertex, int> vertToIndex = new Dictionary<GX_Vertex, int>(
                //new SegmentEpsilonComparer(epsilon)
            );

            foreach (var i in tri)
            {
                var v = verts[i];
                if (!vertToIndex.ContainsKey(v))
                {
                    vertToIndex.Add(v, nv.Count);
                    nv.Add(v);
                }
                nt.Add(vertToIndex[v]);
            }

            // remove degenerates?
            List<int> dg = new List<int>();
            for (int i = 0; i < nt.Count; i += 3)
            {
                int i1 = nt[i + 0];
                int i2 = nt[i + 1];
                int i3 = nt[i + 2];

                if (i1 == i2 || i2 == i3 || i3 == i1)
                    continue;

                dg.Add(i1);
                dg.Add(i2);
                dg.Add(i3);
            }

            System.Diagnostics.Debug.WriteLine($"{verts.Count} -> {nv.Count}");
            System.Diagnostics.Debug.WriteLine($"{nt.Count} -> {dg.Count}");

            verts.Clear();
            verts.AddRange(nv);

            tri.Clear();
            tri.AddRange(dg);
        }

        public HsdMsh(LiveJObj root, HSD_JOBJ parent, HSD_DOBJ dobj)
        {
            Material = new HsdMat(dobj.Mobj);

            if (dobj.Pobj == null)
                return;

            if (dobj.Pobj.Flags.HasFlag(POBJ_FLAG.CULLBACK))
                Material.Culling = HsdCullMode.BACK;
            else
            if (dobj.Pobj.Flags.HasFlag(POBJ_FLAG.CULLFRONT))
                Material.Culling = HsdCullMode.FRONT;
            else
                Material.Culling = HsdCullMode.NONE;

            List<GXAttribName> attrs = new List<GXAttribName>();
            List<GX_Vertex> vertices = new List<GX_Vertex>();
            foreach (var pobj in dobj.Pobj.List)
            {
                GX_DisplayList dl = pobj.ToDisplayList();
                HSD_Envelope[] envelopes = pobj.EnvelopeWeights;

                int offset = 0;
                foreach (GX_PrimitiveGroup prim in dl.Primitives)
                {
                    List<GX_Vertex> verts = dl.Vertices.GetRange(offset, prim.Count);
                    offset += prim.Count;

                    // generate triangle
                    var tri = new List<int>();
                    for (int i = 0; i < verts.Count; i++)
                        tri.Add(i + vertices.Count);

                    // convert to list
                    switch (prim.PrimitiveType)
                    {
                        case GXPrimitiveType.Quads:
                            tri = TriangleConverter.QuadToList(tri);
                            break;
                        case GXPrimitiveType.TriangleStrip:
                            tri = TriangleConverter.StripToList(tri);
                            break;
                        case GXPrimitiveType.Triangles:
                            break;
                        default:
                            throw new NotSupportedException($"Unsupported Primitive Type {prim.PrimitiveType}");
                    }

                    // get skin data
                    int bone_offset = Skin.Bones.Count;
                    if (envelopes != null && envelopes.Length > 0)
                    {
                        Skin.Bones.AddRange(envelopes.Select(e =>
                        {
                            return e.JOBJs.Select(e => root.GetIndexOfDesc(e)).ToList();
                        }));

                        Skin.Weights.AddRange(envelopes.Select(e =>
                        {
                            return e.Weights.ToList();
                        }));
                    }

                    // process vertex transforms
                    for (int i = 0; i < verts.Count; i++)
                    {
                        var v = verts[i];

                        if (envelopes != null && envelopes.Length > 0)
                        {
                            int mtx_id = v.PNMTXIDX / 3;
                            var e = envelopes[mtx_id];

                            if (e.EnvelopeCount == 1)
                            {
                                // transform by world transform
                                var joint = root.GetJObjFromDesc(e.GetJOBJAt(0));
                                v.POS = v.POS.TransformPosition(joint.WorldTransform);
                                v.NRM = v.NRM.TransformNormal(joint.WorldTransform);
                            }

                            v.PNMTXIDX = (ushort)((v.PNMTXIDX / 3f) + bone_offset);
                        }

                        //v.POS = v.POS.TransformPosition(root.GetJObjFromDesc(parent).WorldTransform.Inverted());
                        //v.NRM = v.NRM.TransformNormal(root.GetJObjFromDesc(parent).WorldTransform.Inverted());

                        verts[i] = v;
                    }

                    vertices.AddRange(verts);
                    Triangles.AddRange(tri);

                    foreach (var a in dl.Attributes)
                        if (!attrs.Contains(a.AttributeName))
                            attrs.Add(a.AttributeName);
                }
            }

            Optimize(vertices, Triangles);

            foreach (var attr in attrs)
            {
                switch (attr)
                {
                    case GXAttribName.GX_VA_PNMTXIDX:
                        AddVertexData(attr, vertices.Select(v => (float)v.PNMTXIDX));
                        break;
                    case GXAttribName.GX_VA_POS:
                        AddVertexData(attr, vertices.SelectMany(e => new float[] { e.POS.X, e.POS.Y, e.POS.Z }));
                        break;
                    case GXAttribName.GX_VA_NRM:
                        AddVertexData(attr, vertices.SelectMany(e => new float[] { e.NRM.X, e.NRM.Y, e.NRM.Z }));
                        break;
                    case GXAttribName.GX_VA_CLR0:
                        AddVertexData(attr, vertices.SelectMany(e => new float[] { e.CLR0.R, e.CLR0.G, e.CLR0.B, e.CLR0.A }));
                        break;
                    case GXAttribName.GX_VA_CLR1:
                        AddVertexData(attr, vertices.SelectMany(e => new float[] { e.CLR1.R, e.CLR1.G, e.CLR1.B, e.CLR1.A }));
                        break;
                    case GXAttribName.GX_VA_TEX0:
                        AddVertexData(attr, vertices.SelectMany(e => new float[] { e.TEX0.X, e.TEX0.Y }));
                        break;
                    case GXAttribName.GX_VA_TEX1:
                        AddVertexData(attr, vertices.SelectMany(e => new float[] { e.TEX1.X, e.TEX1.Y }));
                        break;
                    case GXAttribName.GX_VA_TEX2:
                        AddVertexData(attr, vertices.SelectMany(e => new float[] { e.TEX2.X, e.TEX2.Y }));
                        break;
                    case GXAttribName.GX_VA_TEX3:
                        AddVertexData(attr, vertices.SelectMany(e => new float[] { e.TEX3.X, e.TEX3.Y }));
                        break;
                    case GXAttribName.GX_VA_TEX4:
                        AddVertexData(attr, vertices.SelectMany(e => new float[] { e.TEX4.X, e.TEX4.Y }));
                        break;
                    case GXAttribName.GX_VA_TEX5:
                        AddVertexData(attr, vertices.SelectMany(e => new float[] { e.TEX5.X, e.TEX5.Y }));
                        break;
                    case GXAttribName.GX_VA_TEX6:
                        AddVertexData(attr, vertices.SelectMany(e => new float[] { e.TEX6.X, e.TEX6.Y }));
                        break;
                    case GXAttribName.GX_VA_TEX7:
                        AddVertexData(attr, vertices.SelectMany(e => new float[] { e.TEX7.X, e.TEX7.Y }));
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        private void AddVertexData(GXAttribName name, IEnumerable<float> data)
        {
            if (!Data.ContainsKey(name))
                Data.Add(name, new List<float>());

            Data[name].AddRange(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal HSD_DOBJ ToDObj(HsdImportHelper imp)
        {
            var dobj = new HSD_DOBJ()
            {
                Mobj = Material.ToMObj(imp)
            };

            GX_Vertex[] vertices = new GX_Vertex[Triangles.Max() + 1]; 
            Span<GX_Vertex> verts = vertices;
            for (int i = 0; i < verts.Length; i++)
            {
                ref GX_Vertex v = ref verts[i];
                foreach (var (attrib, data) in Data)
                {
                    switch (attrib)
                    {
                        case GXAttribName.GX_VA_PNMTXIDX:
                            {
                                v.PNMTXIDX = (ushort)data[i];
                                break;
                            }
                        case GXAttribName.GX_VA_POS:
                            {
                                v.POS.X = data[i * 3];
                                v.POS.Y = data[i * 3 + 1];
                                v.POS.Z = data[i * 3 + 2];
                                break;
                            }
                        case GXAttribName.GX_VA_NRM:
                            {
                                v.NRM.X = data[i * 3];
                                v.NRM.Y = data[i * 3 + 1];
                                v.NRM.Z = data[i * 3 + 2];
                                break;
                            }
                        case GXAttribName.GX_VA_CLR0:
                            {
                                v.CLR0.R = data[i * 4];
                                v.CLR0.G = data[i * 4 + 1];
                                v.CLR0.B = data[i * 4 + 2];
                                v.CLR0.A = data[i * 4 + 3];
                                break;
                            }
                        case GXAttribName.GX_VA_CLR1:
                            {
                                v.CLR1.R = data[i * 4];
                                v.CLR1.G = data[i * 4 + 1];
                                v.CLR1.B = data[i * 4 + 2];
                                v.CLR1.A = data[i * 4 + 3];
                                break;
                            }
                        case GXAttribName.GX_VA_TEX0:
                            {
                                v.TEX0.X = data[i * 2];
                                v.TEX0.Y = data[i * 2 + 1];
                                break;
                            }
                        case GXAttribName.GX_VA_TEX1:
                            {
                                v.TEX1.X = data[i * 2];
                                v.TEX1.Y = data[i * 2 + 1];
                                break;
                            }
                        case GXAttribName.GX_VA_TEX2:
                            {
                                v.TEX2.X = data[i * 2];
                                v.TEX2.Y = data[i * 2 + 1];
                                break;
                            }
                        case GXAttribName.GX_VA_TEX3:
                            {
                                v.TEX3.X = data[i * 2];
                                v.TEX3.Y = data[i * 2 + 1];
                                break;
                            }
                        case GXAttribName.GX_VA_TEX4:
                            {
                                v.TEX4.X = data[i * 2];
                                v.TEX4.Y = data[i * 2 + 1];
                                break;
                            }
                        case GXAttribName.GX_VA_TEX5:
                            {
                                v.TEX5.X = data[i * 2];
                                v.TEX5.Y = data[i * 2 + 1];
                                break;
                            }
                        case GXAttribName.GX_VA_TEX6:
                            {
                                v.TEX6.X = data[i * 2];
                                v.TEX6.Y = data[i * 2 + 1];
                                break;
                            }
                        case GXAttribName.GX_VA_TEX7:
                            {
                                v.TEX7.X = data[i * 2];
                                v.TEX7.Y = data[i * 2 + 1];
                                break;
                            }
                    }
                }
            }

            List<GX_Vertex> tri = Triangles.Select(e => vertices[e]).ToList();

            List<HSD_JOBJ[]> bones = null;
            List<float[]> weights = null;

            if (Data.ContainsKey(GXAttribName.GX_VA_PNMTXIDX))
            {
                bones = Triangles.Select(e => Skin.Bones[vertices[e].PNMTXIDX].Select(i => imp.Root.TreeList[i]).ToArray()).ToList();
                weights = Triangles.Select(e => Skin.Weights[vertices[e].PNMTXIDX].ToArray()).ToList();
            }

            dobj.Pobj = imp.PObjGenerator.CreatePOBJsFromTriangleList(tri, 
                Data.Select(e=>e.Key).ToArray(),
                bones,
                weights);

            foreach (var p in dobj.Pobj.List)
                if (Material.Culling == HsdCullMode.FRONT)
                    p.Flags |= POBJ_FLAG.CULLFRONT;
                else
                if (Material.Culling == HsdCullMode.BACK)
                    p.Flags |= POBJ_FLAG.CULLBACK;

            return dobj;
        }
    }
}
