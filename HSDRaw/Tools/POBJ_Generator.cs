using HSDRaw.Common;
using HSDRaw.GX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HSDRaw.Tools
{

    public enum GenCullMode
    {
        None,
        Front,
        Back,
        FrontAndBack
    }

    /// <summary>
    /// Special class used for generating POBJs
    /// </summary>
    public class POBJ_Generator
    {
        public GXCompType VertexColorFormat { get; set; } = (GXCompType)GXCompTypeClr.RGB565;

        public bool UseTriangleStrips { get; set; } = true;

        public bool RoundWeights { get; set; } = false;

        public float Tolerance { get; set; } = 0.0f;

        public GenCullMode CullMode { get; set; } = GenCullMode.Front;

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
                    var attr = new GX_Attribute()
                    {
                        AttributeName = names[i],
                        AttributeType = GXAttribType.GX_DIRECT,
                        CompType = (GXCompType)GXCompTypeClr.RGB565,
                        Stride = 4,
                        CompCount = GXCompCnt.ClrRGB
                    };

                    if (names[i].Equals(GXAttribName.GX_VA_CLR0) || names[i].Equals(GXAttribName.GX_VA_CLR1))
                    {
                        attr.CompType = VertexColorFormat;

                        switch (attr.CompType)
                        {
                            case (GXCompType)GXCompTypeClr.RGBA6:
                            case (GXCompType)GXCompTypeClr.RGBA8:
                            case (GXCompType)GXCompTypeClr.RGBA4:
                            case (GXCompType)GXCompTypeClr.RGBX8:
                                attr.CompCount = GXCompCnt.ClrRGBA;
                                break;
                        }
                    }

                    System.Diagnostics.Debug.WriteLine(names[i] + " " + attr.CompType);

                    nameToAttr.Add(names[i], attr);
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

            var hash = string.Join("", o).GetHashCode();
            
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

        class UnorderedTupleArrayComparer : IEqualityComparer<(int, float)[]>
        {
            public bool Equals((int, float)[] x, (int, float)[] y)
            {
                return AreArraysEqual(x, y);
            }

            public int GetHashCode((int, float)[] obj)
            {
                return GetUnorderedArrayHashCode(obj);
            }

            private bool AreArraysEqual((int, float)[] a, (int, float)[] b)
            {
                if (a == null || b == null) return a == b;
                if (a.Length != b.Length) return false;

                // Sort both arrays by their tuple values (Item1 and Item2)
                var sortedA = a.OrderBy(t => t.Item1).ThenBy(t => t.Item2).ToArray();
                var sortedB = b.OrderBy(t => t.Item1).ThenBy(t => t.Item2).ToArray();

                // Compare sorted arrays element by element
                for (int i = 0; i < sortedA.Length; i++)
                {
                    if (!sortedA[i].Equals(sortedB[i])) return false;
                }

                return true;
            }

            private int GetUnorderedArrayHashCode((int, float)[] arr)
            {
                if (arr == null) return 0;

                // Sort and generate the hash code based on sorted tuples
                var sortedArr = arr.OrderBy(t => t.Item1).ThenBy(t => t.Item2).ToArray();
                int hash = 0;
                foreach (var item in sortedArr)
                {
                    hash ^= item.Item1.GetHashCode() ^ item.Item2.GetHashCode();
                }
                return hash;
            }
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
            if(Bones == null)
            {
                Bones = new List<HSD_JOBJ[]>();
                Weights = new List<float[]>();
                foreach(var v in triList)
                {
                    Bones.Add(new HSD_JOBJ[0]);
                    Weights.Add(new float[0]);
                }
            }

            Dictionary<HSDStruct, int> jobjToIndex = new Dictionary<HSDStruct, int>();
            List<HSD_JOBJ> jobjs = new List<HSD_JOBJ>();
            List<int[]> BonesI = new List<int[]>();
            foreach (var bone in Bones)
            {
                int[] bonei = new int[bone.Length];
                int i = 0;
                foreach(var b in bone)
                {
                    if (!jobjToIndex.ContainsKey(b._s))
                    {
                        jobjToIndex.Add(b._s, jobjs.Count);
                        jobjs.Add(b);
                    }
                    bonei[i++] = jobjToIndex[b._s];
                }
                BonesI.Add(bonei);
            }

            // length checking
            if(triList.Count != Bones.Count || triList.Count != Weights.Count)
            {
                throw new IndexOutOfRangeException("Bone and Weight list must have same count as Triangle List");
            }

            // create a weight list
            List<int> pmtx = new List<int>();
            List<HSD_Envelope> weights = new List<HSD_Envelope>();
            Dictionary<(int, float)[], int> env = new Dictionary<(int, float)[], int>(new UnorderedTupleArrayComparer());
            for (int i = 0; i < triList.Count; i++)
            {
                var w = Weights[i];
                var b = BonesI[i];

                if (w.Length != b.Length)
                    throw new IndexOutOfRangeException("Bone and Weight must have the same lengths");

                var a = new (int, float)[w.Length];
                for (int j = 0; j < w.Length; j++)
                    a[j] = (b[j], w[j]);

                a = a.OrderBy(e => e.Item1).ToArray();

                // optional round weights
                if (RoundWeights)
                    a = RoundWeight(a);

                // optional delta tolerance
                if (Tolerance != 0.0f)
                {
                    var r = env.Keys.FirstOrDefault(d => IsClose(a, d, Tolerance));
                    if (r != null)
                        a = r;
                }

                // check to create new envelope
                if (!env.ContainsKey(a))
                {
                    env.Add(a, weights.Count);

                    HSD_Envelope e = new HSD_Envelope();
                    for (int j = 0; j < a.Length; j++)
                        e.Add(jobjs[a[j].Item1], a[j].Item2);
                    weights.Add(e);
                }

                var v = triList[i];
                v.PNMTXIDX = (ushort)(env[a] * 3);
                triList[i] = v;
            }

            //System.Diagnostics.Debug.WriteLine(string.Join("\n", env.Keys.Select(e=>string.Join(", ", e))));

            return CreatePOBJsFromTriangleList(triList, attributes, weights);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private static (int, float)[] RoundWeight((int, float)[] a)
        {
            double sum = 0;
            for (int j = 0; j < a.Length; j++)
            {
                if (j == a.Length - 1)
                {
                    a[j].Item2 = (float)Math.Round(1 - sum, 2);
                }
                else
                {
                    a[j].Item2 = (float)Math.Round(a[j].Item2, 2);
                    sum += a[j].Item2;
                }
            }
            return a;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        private static bool IsClose((int, float)[] a, (int, float)[] b, float tolerance)
        {
            if (a.Length != b.Length)
                return false;

            float error = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i].Item1 != b[i].Item1)
                    return false;

                error += Math.Abs(a[i].Item2 - b[i].Item2);
            }

            if (error > tolerance)
                return false;

            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private static (int, float)[] ClampWeight(List<(int, float)> a)
        {
            for (int i = a.Count - 1; i >= 0; i--)
            {
                if (a[i].Item2 <= 0.1)
                {
                    // remove this weight...
                    var weight = a[i].Item2;
                    a.RemoveAt(i); 
                    
                    int minIndex = a.Select((value, index) => (value, index))
                                    .OrderBy(pair => pair.value.Item2)
                                    .First().index;
                    a[minIndex] = (a[minIndex].Item1, a[minIndex].Item2 + weight);
                    break;
                }
            }

            return a.ToArray();
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

            var groups = converter.GroupPrimitives(triList.ToArray(), out int pointCount, out int faceCount);

            HSD_POBJ rootPOBJ = null;
            HSD_POBJ prevPOBJ = null;

            foreach (var g in groups)
            {
                var jobjweights = new List<HSD_Envelope>();
                var pmidToNewID = new Dictionary<ushort, ushort>();

                if (HasPositionNormalMatrix == true && weights.Count > 0)
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

                switch (CullMode)
                {
                    case GenCullMode.Front:
                        newpobj.Flags |= POBJ_FLAG.CULLFRONT;
                        break;
                    case GenCullMode.Back:
                        newpobj.Flags |= POBJ_FLAG.CULLBACK;
                        break;
                    case GenCullMode.FrontAndBack:
                        newpobj.Flags |= POBJ_FLAG.CULLFRONT | POBJ_FLAG.CULLBACK;
                        break;
                }

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
                        case GXAttribName.GX_VA_TEX2: ig.Indices[i] = GetIndex(b, new float[] { v.TEX2.X, v.TEX2.Y }); break;
                        case GXAttribName.GX_VA_TEX3: ig.Indices[i] = GetIndex(b, new float[] { v.TEX3.X, v.TEX3.Y }); break;
                        case GXAttribName.GX_VA_TEX4: ig.Indices[i] = GetIndex(b, new float[] { v.TEX4.X, v.TEX4.Y }); break;
                        case GXAttribName.GX_VA_TEX5: ig.Indices[i] = GetIndex(b, new float[] { v.TEX5.X, v.TEX5.Y }); break;
                        case GXAttribName.GX_VA_TEX6: ig.Indices[i] = GetIndex(b, new float[] { v.TEX6.X, v.TEX6.Y }); break;
                        case GXAttribName.GX_VA_TEX7: ig.Indices[i] = GetIndex(b, new float[] { v.TEX7.X, v.TEX7.Y }); break;
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
