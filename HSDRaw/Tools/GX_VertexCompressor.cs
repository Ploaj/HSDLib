using HSDRaw.Common;
using HSDRaw.GX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace HSDRaw.Tools
{
    /// <summary>
    /// Helper class for creating the vertex buffer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GX_BufferMaker<T>
    {
        public List<T> Collection = new List<T>();
        private Dictionary<T, int> QuickIndex = new Dictionary<T, int>();

        public ushort GetIndex(T o)
        {
            int index = 0;
            if (!QuickIndex.ContainsKey(o))
            {
                index = Collection.Count;
                Collection.Add(o);
                QuickIndex.Add(o, index);
            }

            return (ushort)QuickIndex[o];
        }

        public byte[] GenerateBuffer(GX_Attribute Attribute)
        {
            MemoryStream o = new MemoryStream();

            using (BinaryWriterExt Writer = new BinaryWriterExt(o))
            {
                Writer.BigEndian = true;
                foreach (object ob in Collection)
                {
                    if (ob is GXVector2 vc2)
                    {
                        WriteData(Writer, vc2.X, Attribute.CompType, Attribute.Scale);
                        WriteData(Writer, vc2.Y, Attribute.CompType, Attribute.Scale);
                    }
                    else
                    if (ob is GXVector3)
                    {
                        WriteData(Writer, ((GXVector3)ob).X, Attribute.CompType, Attribute.Scale);
                        WriteData(Writer, ((GXVector3)ob).Y, Attribute.CompType, Attribute.Scale);
                        WriteData(Writer, ((GXVector3)ob).Z, Attribute.CompType, Attribute.Scale);
                    }
                    else
                        Console.WriteLine("Error Writing: " + ob.GetType());
                }
                Writer.Align(0x20);
            }

            byte[] data = o.ToArray();
            o.Dispose();

            return data;
        }

        private void WriteData(BinaryWriterExt Writer, float Value, GXCompType Type, float Scale)
        {
            float Scaled = Value * (float)Math.Pow(2, Scale);
            switch (Type)
            {
                case GXCompType.UInt8:
                    Writer.Write((byte)Scaled);
                    break;
                case GXCompType.Int8:
                    Writer.Write((sbyte)Scaled);
                    break;
                case GXCompType.UInt16:
                    Writer.Write((ushort)Scaled);
                    break;
                case GXCompType.Int16:
                    Writer.Write((short)Scaled);
                    break;
                case GXCompType.Float:
                    Writer.Write(Scaled);
                    break;
                default:
                    Writer.Write((byte)Scaled);
                    break;
            }
        }
    }

    /// <summary>
    /// Special class used for generating POBJs
    /// </summary>
    public class GX_VertexCompressor
    {
        private Dictionary<GX_Attribute, GX_BufferMaker<object>> BufferLinker = new Dictionary<GX_Attribute, GX_BufferMaker<object>>();

        // Creates the new buffers
        public void SaveChanges()
        {
            //Dictionary<GX_BufferMaker<object>, byte[]> BufferData = new Dictionary<GX_BufferMaker<object>, byte[]>();
            foreach (GX_Attribute b in BufferLinker.Keys)
            {
                /*if (BufferData.ContainsKey(BufferLinker[b]))
                {
                    b.Buffer = BufferData[BufferLinker[b]];
                    continue;
                }*/

                byte[] data = BufferLinker[b].GenerateBuffer(b);

                //TODO, check if null?
                b.Buffer = data;

                //BufferData.Add(BufferLinker[b], data);

                Console.WriteLine(b.AttributeName + " " + b.Buffer.Length.ToString("X"));
            }
        }

        /// <summary>
        /// Gets the index of a value in the buffer
        /// </summary>
        /// <param name="v"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        private ushort GetIndex(GX_Attribute v, object o)
        {
            if (!BufferLinker.ContainsKey(v))
            {
                /*foreach (GX_Attribute buf in BufferLinker.Keys)
                {
                    if (buf.AttributeName == v.AttributeName
                        && buf.Scale == v.Scale
                        && buf.AttributeType == v.AttributeType
                        && buf.CompType == v.CompType
                        && buf.CompCount == v.CompCount)
                    {
                        BufferLinker.Add(v, BufferLinker[buf]);
                        return BufferLinker[buf].GetIndex(o);
                    }
                }*/
                BufferLinker.Add(v, new GX_BufferMaker<object>());
            }
            return BufferLinker[v].GetIndex(o);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="triList"></param>
        /// <param name="attrGroup"></param>
        /// <param name="Bones"></param>
        /// <param name="Weights"></param>
        /// <returns></returns>
        public HSD_POBJ CreatePOBJsFromTriangleList(List<GX_Vertex> triList, GX_Attribute[] attrGroup, List<HSD_JOBJ[]> Bones, List<float[]> Weights)
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

            Console.WriteLine("WeightList Created: " + weights.Count + " " + triList.Count);

            return CreatePOBJsFromTriangleList(triList, attrGroup, weights);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="triList"></param>
        /// <param name="attrGroup"></param>
        /// <param name="weights"></param>
        public HSD_POBJ CreatePOBJsFromTriangleList(List<GX_Vertex> triList, GX_Attribute[] attrGroup, List<HSD_Envelope> weights)
        {
            TriangleConverter.TriangleConverter converter = new TriangleConverter.TriangleConverter(true, 100, 3, true);
            int pointCount, faceCount;

            var groups = converter.GroupPrimitives(triList.ToArray(), out pointCount, out faceCount);

            HSD_POBJ rootPOBJ = null;
            HSD_POBJ prevPOBJ = null;

            foreach (var g in groups)
            {
                var jobjweights = new List<HSD_Envelope>();
                var pmidToNewID = new Dictionary<ushort, ushort>();

                foreach (var n in g._nodes)
                {
                    pmidToNewID.Add(n, (ushort)(jobjweights.Count * 3));
                    jobjweights.Add(weights[n / 3]);
                }

                GX_DisplayList newdl = new GX_DisplayList();

                foreach (var t in g._triangles)
                {
                    var newVert = new List<GX_Vertex>();
                    for (int p = 0; p < t.Points.Count; p++)
                    {
                        var point = t.Points[p];
                        point.PNMTXIDX = pmidToNewID[point.PNMTXIDX];
                        t.Points[p] = point;
                        newVert.Add(point);
                    }

                    newdl.Primitives.Add(Compress(GXPrimitiveType.Triangles, newVert.ToArray(), attrGroup));
                }
                foreach (var t in g._tristrips)
                {
                    var newVert = new List<GX_Vertex>();
                    for (int p = 0; p < t.Points.Count; p++)
                    {
                        var point = t.Points[p];
                        point.PNMTXIDX = pmidToNewID[point.PNMTXIDX];
                        t.Points[p] = point;
                        newVert.Add(point);
                    }
                    newdl.Primitives.Add(Compress(GXPrimitiveType.TriangleStrip, newVert.ToArray(), attrGroup));
                }

                newdl.Envelopes = jobjweights;
                newdl.Attributes.AddRange(attrGroup);

                var newpobj = new HSD_POBJ();
                newpobj.FromDisplayList(newdl);

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
        public GX_PrimitiveGroup Compress(GXPrimitiveType type, GX_Vertex[] Vertices, GX_Attribute[] Attributes)
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
                    switch (b.AttributeType)
                    {
                        case GXAttribType.GX_DIRECT:
                            if (b.AttributeName == GXAttribName.GX_VA_CLR0)
                                ig.Clr0 = new byte[] { (byte)(v.CLR0.R * 0xFF), (byte)(v.CLR0.G * 0xFF), (byte)(v.CLR0.B * 0xFF), (byte)(v.CLR0.A * 0xFF) };
                            else
                            if (b.AttributeName == GXAttribName.GX_VA_CLR1)
                                ig.Clr1 = new byte[] { (byte)(v.CLR1.R * 0xFF), (byte)(v.CLR1.G * 0xFF), (byte)(v.CLR1.B * 0xFF), (byte)(v.CLR1.A * 0xFF) };
                            else
                            if (b.AttributeName == GXAttribName.GX_VA_PNMTXIDX)
                                ig.Indices[i] = v.PNMTXIDX;
                            if (b.AttributeName == GXAttribName.GX_VA_TEX0MTXIDX)
                                ig.Indices[i] = v.TEX0MTXIDX;
                            break;
                        default:
                            switch (b.AttributeName)
                            {
                                case GXAttribName.GX_VA_NULL: break;
                                case GXAttribName.GX_VA_POS: ig.Indices[i] = GetIndex(b, v.POS); break;
                                case GXAttribName.GX_VA_NRM: ig.Indices[i] = GetIndex(b, v.NRM); break;
                                case GXAttribName.GX_VA_TEX0: ig.Indices[i] = GetIndex(b, v.TEX0); break;
                                case GXAttribName.GX_VA_TEX1: ig.Indices[i] = GetIndex(b, v.TEX1); break;
                                case GXAttribName.GX_VA_CLR0: ig.Indices[i] = GetIndex(b, v.CLR0); break;
                                default:
                                    throw new Exception("Error Building " + b.AttributeName);
                            }

                            break;
                    }
                    i++;
                }
                g.Indices[IndexGroupIndex++] = ig;
            }

            return g;
        }

    }
}
