using HSDRaw.Common;
using HSDRaw.GX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HSDRaw.Tools
{
    /// <summary>
    /// Special class used for generating POBJs
    /// </summary>
    public class POBJ_Generator
    {
        public bool UseTriangleStrips { get; set; } = true;

        private Dictionary<GXAttribName, Dictionary<int, int>> nameToIndexHash = new Dictionary<GXAttribName, Dictionary<int, int>>();
        private Dictionary<GXAttribName, GX_Attribute> nameToAttr = new Dictionary<GXAttribName, GX_Attribute>();
        private Dictionary<GX_Attribute, List<float[]>> attrToNewData = new Dictionary<GX_Attribute, List<float[]>>();
        
        private List<HSD_POBJ> CreatedPOBJs = new List<HSD_POBJ>();

        private Dictionary<HSD_POBJ, GXAttribName[]> pobjToAttributes = new Dictionary<HSD_POBJ, GXAttribName[]>();

        private Dictionary<HSD_POBJ, GX_DisplayList> pobjToDisplayList = new Dictionary<HSD_POBJ, GX_DisplayList>();

        /// <summary>
        /// Generates the buffer data and writes it to all given <see cref="GX_Attribute"/>
        /// </summary>
        public void SaveChanges()
        {
            // Generate the buffers
            foreach (var b in attrToNewData)
            {
                b.Key.DecodedData = b.Value;
            }
            
            // Set the attributes for all pobjs created by this object
            foreach (var pobj in CreatedPOBJs)
            {
                var names = pobjToAttributes[pobj];
                var attrs = GetAttributes(names);

                var dl = pobjToDisplayList[pobj];
                
                dl.Attributes.Clear();
                dl.Attributes.AddRange(attrs);
                
                pobj.FromDisplayList(dl);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private GX_Attribute[] GetAttributes(GXAttribName[] names)
        {
            var attrs = new GX_Attribute[names.Length + 1];

            for (int i = 0; i < names.Length; i++)
            {
                if(!nameToAttr.ContainsKey(names[i]))
                {
                    nameToAttr.Add(names[i], new GX_Attribute()
                    {
                        AttributeName = names[i],
                        AttributeType = GXAttribType.GX_DIRECT,
                        CompType = GXCompType.RGB8,
                        Stride = 4,
                        CompCount = GXCompCnt.ClrRGB
                    });
                }
                attrs[i] = nameToAttr[names[i]];
            }

            // null buffer to indicate end
            attrs[attrs.Length - 1] = new GX_Attribute();
            attrs[attrs.Length - 1].AttributeName = GXAttribName.GX_VA_NULL;

            return attrs;
        }

        /// <summary>
        /// Gets the index of a value in the buffer
        /// </summary>
        /// <param name="v"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        private ushort GetIndex(GXAttribName attributeName, float[] o)
        {
            if (!nameToAttr.ContainsKey(attributeName))
            {
                GX_Attribute a = new GX_Attribute();
                a.AttributeName = attributeName;
                nameToIndexHash.Add(attributeName, new Dictionary<int, int>());
                nameToAttr.Add(attributeName, a);
                attrToNewData.Add(a, new List<float[]>());
            }

            var hashToIndex = nameToIndexHash[attributeName];

            var index = 0;

            var hash = string.Join("", o).GetHashCode();// ((IStructuralEquatable)o).GetHashCode(EqualityComparer<float>.Default);
            
            if (hashToIndex.ContainsKey(hash))
            {
                index = hashToIndex[hash];
            }
            else
            {
                var vals = attrToNewData[nameToAttr[attributeName]];
                index = vals.Count;
                vals.Add(o);
                hashToIndex.Add(hash, index);
            }
            
            return (ushort)index;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="triList"></param>
        /// <param name="attrGroup"></param>
        /// <param name="Bones"></param>
        /// <param name="Weights"></param>
        /// <returns></returns>
        public HSD_POBJ CreatePOBJsFromTriangleList(List<GX_Vertex> triList, GXAttribName[] attributes, List<HSD_JOBJ[]> Bones, List<float[]> Weights)
        {
            List<HSD_Envelope> weights = new List<HSD_Envelope>();
            Dictionary<int, int> evToIndex = new Dictionary<int, int>();

            Dictionary<HSDStruct, int> jobjToIndex = new Dictionary<HSDStruct, int>();
            List<HSD_JOBJ> jobjs = new List<HSD_JOBJ>();
            foreach(var bone in Bones)
            {
                foreach(var b in bone)
                    if (!jobjToIndex.ContainsKey(b._s))
                    {
                        jobjToIndex.Add(b._s, jobjs.Count);
                        jobjs.Add(b);
                    }
            }

            // length checking
            if(triList.Count != Bones.Count || triList.Count != Weights.Count)
            {
                throw new IndexOutOfRangeException("Bone and Weight list must have same count as Triangle List");
            }
            
            // create a weight list
            for(int i = 0; i < triList.Count; i++)
            {
                var bone = Bones[i];
                var weight = Weights[i];

                if (bone.Length != weight.Length)
                    throw new IndexOutOfRangeException("Bone and Weight must have the same lengths");

                var hash = ((IStructuralEquatable)weight).GetHashCode(EqualityComparer<float>.Default);

                int[] bonei = new int[bone.Length];
                for (int j = 0; j < bone.Length; j++)
                    bonei[j] = jobjToIndex[bone[j]._s];

                var hash2 = ((IStructuralEquatable)bonei).GetHashCode(EqualityComparer<int>.Default);
                hash += hash2;
                
                if (!evToIndex.ContainsKey(hash))
                {
                    evToIndex.Add(hash, weights.Count);

                    HSD_Envelope e = new HSD_Envelope();
                    for (int j = 0; j < bone.Length; j++)
                        e.Add(bone[j], weight[j]);

                    weights.Add(e);
                }

                var v = triList[i];
                v.PNMTXIDX = (ushort)(evToIndex[hash] * 3);
                triList[i] = v;
            }

            //Console.WriteLine("WeightList Created: " + weights.Count + " " + triList.Count);

            return CreatePOBJsFromTriangleList(triList, attributes, weights);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="triList"></param>
        /// <param name="attrGroup"></param>
        /// <param name="weights"></param>
        public HSD_POBJ CreatePOBJsFromTriangleList(List<GX_Vertex> triList, GXAttribName[] attributes, List<HSD_Envelope> weights)
        {
            TriangleConverter.TriangleConverter converter = new TriangleConverter.TriangleConverter(UseTriangleStrips, 32, 3, true);
            int pointCount, faceCount;

            HSD_JOBJ singleBind = null;
            
            bool HasPositionNormalMatrix = attributes.Contains(GXAttribName.GX_VA_PNMTXIDX);

            // Optimize single bind
            /*if (weights != null && weights.Count == 1 && weights[0].EnvelopeCount == 1)
            {
                singleBind = weights[0].GetJOBJAt(0);
                var al = attributes.ToList();
                al.Remove(GXAttribName.GX_VA_PNMTXIDX);
                attributes = al.ToArray();
            }*/

            var groups = converter.GroupPrimitives(triList.ToArray(), out pointCount, out faceCount);

            HSD_POBJ rootPOBJ = null;
            HSD_POBJ prevPOBJ = null;

            foreach (var g in groups)
            {
                var jobjweights = new List<HSD_Envelope>();
                var pmidToNewID = new Dictionary<ushort, ushort>();

                if (HasPositionNormalMatrix)
                {
                    foreach (var n in g._nodes)
                    {
                        pmidToNewID.Add(n, (ushort)(jobjweights.Count * 3));
                        jobjweights.Add(weights[n / 3]);
                    }
                }

                GX_DisplayList newdl = new GX_DisplayList();

                foreach (var t in g._triangles)
                {
                    var newVert = new List<GX_Vertex>();
                    for (int p = 0; p < t.Points.Count; p++)
                    {
                        var point = t.Points[p];
                        if (HasPositionNormalMatrix)
                        {
                            point.PNMTXIDX = pmidToNewID[point.PNMTXIDX];
                            point.TEX0MTXIDX = (ushort)(point.PNMTXIDX + 30);
                            point.TEX1MTXIDX = point.TEX0MTXIDX;
                        }
                        t.Points[p] = point;
                        newVert.Add(point);
                    }

                    newdl.Primitives.Add(Compress(GXPrimitiveType.Triangles, newVert.ToArray(), attributes));
                }
                foreach (var t in g._tristrips)
                {
                    var newVert = new List<GX_Vertex>();
                    for (int p = 0; p < t.Points.Count; p++)
                    {
                        var point = t.Points[p];
                        if (HasPositionNormalMatrix)
                        {
                            point.PNMTXIDX = pmidToNewID[point.PNMTXIDX];
                            point.TEX0MTXIDX = (ushort)(point.PNMTXIDX + 30);
                            point.TEX1MTXIDX = point.TEX0MTXIDX;
                        }
                        t.Points[p] = point;
                        newVert.Add(point);
                    }
                    newdl.Primitives.Add(Compress(GXPrimitiveType.TriangleStrip, newVert.ToArray(), attributes));
                }

                if(singleBind == null)
                    newdl.Envelopes = jobjweights;

                var newpobj = new HSD_POBJ();
                CreatedPOBJs.Add(newpobj);

                if(singleBind != null)
                    newpobj.SingleBoundJOBJ = singleBind;

                pobjToDisplayList.Add(newpobj, newdl);
                pobjToAttributes.Add(newpobj, attributes);

                if (prevPOBJ == null)
                {
                    rootPOBJ = newpobj;
                }
                else
                {
                    prevPOBJ.Next = newpobj;
                }
                prevPOBJ = newpobj;
            }

            //Console.WriteLine(rootPOBJ.List.Count + " POBJs generated");


            return rootPOBJ;
        }

        /// <summary>
        /// Creates a primitive group for the vertex buffer
        /// </summary>
        /// <param name="type"></param>
        /// <param name="Vertices"></param>
        /// <param name="Attributes"></param>
        /// <returns></returns>
        private GX_PrimitiveGroup Compress(GXPrimitiveType type, GX_Vertex[] Vertices, GXAttribName[] Attributes)
        {
            GX_PrimitiveGroup g = new GX_PrimitiveGroup();
            g.PrimitiveType = type;
            g.Indices = new GX_IndexGroup[Vertices.Length];
            int IndexGroupIndex = 0;
            foreach (GX_Vertex v in Vertices)
            {
                GX_IndexGroup ig = new GX_IndexGroup();
                ig.Indices = new ushort[Attributes.Length];

                int i = 0;
                foreach (var b in Attributes)
                {
                    switch (b)
                    {
                        case GXAttribName.GX_VA_CLR0:
                            ig.Clr0 = v.CLR0.ToBytes();
                            break;
                        case GXAttribName.GX_VA_CLR1:
                            ig.Clr1 = v.CLR1.ToBytes();
                            break;
                        case GXAttribName.GX_VA_PNMTXIDX:
                            ig.Indices[i] = v.PNMTXIDX;
                            break;
                        case GXAttribName.GX_VA_TEX0MTXIDX:
                            ig.Indices[i] = v.TEX0MTXIDX;
                            break;
                        case GXAttribName.GX_VA_TEX1MTXIDX:
                            ig.Indices[i] = v.TEX1MTXIDX;
                            break;
                        case GXAttribName.GX_VA_NULL: break;
                        case GXAttribName.GX_VA_POS: ig.Indices[i] = GetIndex(b, new float[] { v.POS.X, v.POS.Y, v.POS.Z }); break;
                        case GXAttribName.GX_VA_NRM: ig.Indices[i] = GetIndex(b, new float[] { v.NRM.X, v.NRM.Y, v.NRM.Z }); break;
                        case GXAttribName.GX_VA_NBT: ig.Indices[i] = GetIndex(b, new float[] { v.NRM.X, v.NRM.Y, v.NRM.Z, v.BITAN.X, v.BITAN.Y, v.BITAN.Z, v.TAN.X, v.TAN.Y, v.TAN.Z }); break;
                        case GXAttribName.GX_VA_TEX0: ig.Indices[i] = GetIndex(b, new float[] { v.TEX0.X, v.TEX0.Y }); break;
                        case GXAttribName.GX_VA_TEX1: ig.Indices[i] = GetIndex(b, new float[] { v.TEX1.X, v.TEX1.Y }); break;
                        //case GXAttribName.GX_VA_CLR0: ig.Indices[i] = GetIndex(b, v.CLR0); break;
                        default:
                            throw new Exception("Error Building " + b);
                    }
                    i++;
                }
                g.Indices[IndexGroupIndex++] = ig;
            }

            return g;
        }

    }
}
