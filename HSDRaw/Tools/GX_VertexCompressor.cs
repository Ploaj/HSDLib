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
    public class GX_BufferMaker
    {
        public static float Epsilon = 0.0001f;

        public GX_Attribute Attribute { get; internal set; } = new GX_Attribute();

        public List<object> Collection = new List<object>();
        private Dictionary<object, int> QuickIndex = new Dictionary<object, int>();

        public GX_BufferMaker(GXAttribName name)
        {
            Attribute.AttributeName = name;
            switch (name)
            {
                case GXAttribName.GX_VA_PNMTXIDX:
                case GXAttribName.GX_VA_TEX0MTXIDX:
                case GXAttribName.GX_VA_CLR0:
                case GXAttribName.GX_VA_CLR1:
                    Attribute.AttributeType = GXAttribType.GX_DIRECT;
                    break;
            }
        }
        
        /// <summary>
        /// TODO:
        /// </summary>
        private void OptimizeCompression()
        {
            // no need to optimize direct
            if (Attribute.AttributeType == GXAttribType.GX_DIRECT)
                return;

            switch (Attribute.AttributeName)
            {
                case GXAttribName.GX_VA_POS:
                    Attribute.CompCount = GXCompCnt.PosXYZ;
                    break;
                case GXAttribName.GX_VA_NRM:
                    Attribute.CompCount = GXCompCnt.NrmXYZ;
                    break;
                case GXAttribName.GX_VA_TEX0:
                case GXAttribName.GX_VA_TEX1:
                case GXAttribName.GX_VA_TEX2:
                case GXAttribName.GX_VA_TEX3:
                case GXAttribName.GX_VA_TEX4:
                case GXAttribName.GX_VA_TEX5:
                case GXAttribName.GX_VA_TEX6:
                case GXAttribName.GX_VA_TEX7:
                    Attribute.CompCount = GXCompCnt.TexST;
                    break;
                default:
                    throw new NotSupportedException($"{Attribute.AttributeName} not supported for optimizing");
            }

            // get normalized value range
            float max = 0;
            bool signed = false;
            foreach(var v in Collection)
            {
                if (v is GXVector2 vec2)
                {
                    max = Math.Max(max, Math.Abs(vec2.X));
                    max = Math.Max(max, Math.Abs(vec2.Y));
                    if (vec2.X < 0 || vec2.Y < 0)
                        signed = true;
                }
                if (v is GXVector3 vec3)
                {
                    max = Math.Max(max, Math.Abs(vec3.X));
                    max = Math.Max(max, Math.Abs(vec3.Y));
                    max = Math.Max(max, Math.Abs(vec3.Z));
                    if (vec3.X < 0 || vec3.Y < 0 || vec3.Z < 0)
                        signed = true;
                }
            }

            // get best scale for 8

            byte scale = 1;
            byte byteScale = 1;
            byte sbyteScale = 1;
            byte shortScale = 1;
            byte ushortScale = 1;

            while (max != 0 && max * Math.Pow(2, scale) < ushort.MaxValue && scale < byte.MaxValue)
            {
                var val = max * Math.Pow(2, scale);
                if (val < byte.MaxValue)
                    byteScale = scale;
                if (val < sbyte.MaxValue)
                    sbyteScale = scale;
                if (val < short.MaxValue)
                    shortScale = scale;
                if (val < ushort.MaxValue)
                    ushortScale = scale;

                scale++;
            }

            double error = 0;
            if (!signed)
                // byte or ushort
                error = (byte)(max * Math.Pow(2, byteScale)) / Math.Pow(2, byteScale);
            else
                // sbyte or short
                error = (sbyte)(max * Math.Pow(2, sbyteScale)) / Math.Pow(2, sbyteScale);


            if (Math.Abs(max - error) < Epsilon)
            {
                if (signed)
                {
                    Attribute.CompType = GXCompType.Int8;
                    Attribute.Scale = sbyteScale;
                }
                else
                {
                    Attribute.CompType = GXCompType.UInt8;
                    Attribute.Scale = byteScale;
                }
            }
            else
            {
                if (signed)
                {
                    Attribute.CompType = GXCompType.Int16;
                    Attribute.Scale = shortScale;
                }
                else
                {
                    Attribute.CompType = GXCompType.UInt16;
                    Attribute.Scale = ushortScale;
                }
            }

            //Console.WriteLine($"Scale {scale} {byteScale} {sbyteScale} {shortScale} {ushortScale}");
            //Console.WriteLine(signed + " " + max + " " + error + " " + Epsilon + " " + Math.Abs(max - error));
            //Console.WriteLine(Attribute.AttributeName + " " + Attribute.CompType + " " + Attribute.Scale);

            // calculate stride
            Attribute.Stride = AttributeStride(Attribute);
        }

        private short AttributeStride(GX_Attribute attribute)
        {
            return (short)(CompTypeToInt(attribute.AttributeName, attribute.CompType) * CompCountToInt(attribute.AttributeName, attribute.CompCount));
        }

        private int CompTypeToInt(GXAttribName name, GXCompType type)
        {
            switch (name)
            {
                case GXAttribName.GX_VA_CLR0:
                case GXAttribName.GX_VA_CLR1:
                    switch (type)
                    {
                        case GXCompType.RGBA4:
                        case GXCompType.RGB565:
                            return 2;
                        case GXCompType.RGB8:
                        case GXCompType.RGBA6:
                            return 3;
                        case GXCompType.RGBX8:
                        case GXCompType.RGBA8:
                            return 4;
                        default:
                            return 0;
                    }
                default:
                    switch (type)
                    {
                        case GXCompType.Int8:
                        case GXCompType.UInt8:
                            return 1;
                        case GXCompType.Int16:
                        case GXCompType.UInt16:
                            return 2;
                        case GXCompType.Float:
                            return 4;
                        default:
                            return 0;
                    }
            }
        }

        private int CompCountToInt(GXAttribName name, GXCompCnt cc)
        {
            switch (name)
            {
                case GXAttribName.GX_VA_POS:
                    switch (cc)
                    {
                        case GXCompCnt.PosXY:
                            return 2;
                        case GXCompCnt.PosXYZ:
                            return 3;
                    }
                    break;
                case GXAttribName.GX_VA_NRM:
                    switch (cc)
                    {
                        case GXCompCnt.NrmXYZ:
                        case GXCompCnt.NrmNBT:
                            return 3;
                        case GXCompCnt.NrmNBT3: // ??
                            return 3;
                    }
                    break;
                case GXAttribName.GX_VA_TEX0:
                case GXAttribName.GX_VA_TEX1:
                case GXAttribName.GX_VA_TEX2:
                case GXAttribName.GX_VA_TEX3:
                case GXAttribName.GX_VA_TEX4:
                case GXAttribName.GX_VA_TEX5:
                case GXAttribName.GX_VA_TEX6:
                case GXAttribName.GX_VA_TEX7:
                    switch (cc)
                    {
                        case GXCompCnt.TexST:
                            return 2;
                        case GXCompCnt.TexS:
                            return 1;
                    }
                    break;
            }

            return 0;
        }

        public ushort GetIndex(object o)
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

        public byte[] GenerateBuffer()
        {
            OptimizeCompression();

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
                        throw new NotSupportedException("Error Writing: " + ob.GetType());
                }
                Writer.Align(0x20);
            }

            byte[] data = o.ToArray();
            o.Dispose();

            Attribute.Buffer = data;

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
        private List<GX_BufferMaker> Attributes = new List<GX_BufferMaker>();
        
        private List<HSD_POBJ> CreatedPOBJs = new List<HSD_POBJ>();

        private Dictionary<HSD_POBJ, GXAttribName[]> pobjToAttributes = new Dictionary<HSD_POBJ, GXAttribName[]>();

        private Dictionary<HSD_POBJ, GX_DisplayList> pobjToDisplayList = new Dictionary<HSD_POBJ, GX_DisplayList>();

        /// <summary>
        /// Generates the buffer data and writes it to all given <see cref="GX_Attribute"/>
        /// </summary>
        public void SaveChanges()
        {
            // Generate the buffers
            foreach (var b in Attributes)
                if (b != null)
                    b.GenerateBuffer();
            
            // Set the attributes for all pobjs created by this object
            foreach (var pobj in CreatedPOBJs)
            {
                var names = pobjToAttributes[pobj];
                var dl = pobjToDisplayList[pobj];
                var attrs = GetAttributes(names);
                
                dl.Attributes.Clear();
                dl.Attributes.AddRange(attrs);

                pobj.Attributes = attrs;
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
                var att = Attributes.Find(e => e.Attribute.AttributeName == names[i]);
                attrs[i] = att.Attribute;
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
        private ushort GetIndex(GXAttribName attributeName, object o)
        {
            GX_BufferMaker buf = Attributes.Find(e=>e.Attribute.AttributeName == attributeName);

            if (buf == null)
            {
                buf = new GX_BufferMaker(attributeName);
                Attributes.Add(buf);
            }
            
            return buf.GetIndex(o);
        }

        private void AddAttribute(GXAttribName attributeName)
        {
            if(Attributes.Find(e => e.Attribute.AttributeName == attributeName) == null)
                Attributes.Add(new GX_BufferMaker(attributeName));
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
            foreach (var a in attributes)
                AddAttribute(a);

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

                    newdl.Primitives.Add(Compress(GXPrimitiveType.Triangles, newVert.ToArray(), attributes));
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
                    newdl.Primitives.Add(Compress(GXPrimitiveType.TriangleStrip, newVert.ToArray(), attributes));
                }

                newdl.Envelopes = jobjweights;

                var newpobj = new HSD_POBJ();
                CreatedPOBJs.Add(newpobj);
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
                            ig.Clr0 = new byte[] { (byte)(v.CLR0.R * 0xFF), (byte)(v.CLR0.G * 0xFF), (byte)(v.CLR0.B * 0xFF), (byte)(v.CLR0.A * 0xFF) };
                            break;
                        case GXAttribName.GX_VA_CLR1:
                            ig.Clr1 = new byte[] { (byte)(v.CLR1.R * 0xFF), (byte)(v.CLR1.G * 0xFF), (byte)(v.CLR1.B * 0xFF), (byte)(v.CLR1.A * 0xFF) };
                            break;
                        case GXAttribName.GX_VA_PNMTXIDX:
                            ig.Indices[i] = v.PNMTXIDX;
                            break;
                        case GXAttribName.GX_VA_TEX0MTXIDX:
                            ig.Indices[i] = v.TEX0MTXIDX;
                            break;
                        case GXAttribName.GX_VA_NULL: break;
                        case GXAttribName.GX_VA_POS: ig.Indices[i] = GetIndex(b, v.POS); break;
                        case GXAttribName.GX_VA_NRM: ig.Indices[i] = GetIndex(b, v.NRM); break;
                        case GXAttribName.GX_VA_TEX0: ig.Indices[i] = GetIndex(b, v.TEX0); break;
                        case GXAttribName.GX_VA_TEX1: ig.Indices[i] = GetIndex(b, v.TEX1); break;
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
