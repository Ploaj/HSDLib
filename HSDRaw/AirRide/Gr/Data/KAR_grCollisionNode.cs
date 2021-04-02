
using HSDRaw.GX;

namespace HSDRaw.AirRide.Gr.Data
{
    public class KAR_CollisionTriangle : HSDAccessor
    {
        public override int TrimmedSize => 0x14;

        public int V1 { get => _s.GetInt32(0x0); set => _s.SetInt32(0x0, value); }

        public int V2 { get => _s.GetInt32(0x4); set => _s.SetInt32(0x4, value); }

        public int V3 { get => _s.GetInt32(0x8); set => _s.SetInt32(0x8, value); }

        public int Flags { get => _s.GetInt32(0xc); set => _s.SetInt32(0xc, value); }

        public int Unknown { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }
    }

    public class KAR_CollisionJoint : HSDAccessor
    {
        public override int TrimmedSize => 0x1C;

        public int BoneID { get => _s.GetInt32(0x0); set => _s.SetInt32(0x0, value); }

        public int VertexStart { get => _s.GetInt32(0x4); set => _s.SetInt32(0x4, value); }

        public int VertexSize { get => _s.GetInt32(0x8); set => _s.SetInt32(0x8, value); }

        public int FaceStart { get => _s.GetInt32(0xc); set => _s.SetInt32(0xc, value); }

        public int FaceSize { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        public int Unknown1 { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }

        public int Unknown2 { get => _s.GetInt32(0x18); set => _s.SetInt32(0x18, value); }
    }

    public class KAR_ZoneCollisionTriangle : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public int UnknownZone { get => _s.GetInt32(0x0); set => _s.SetInt32(0x0, value); }

        public int V1 { get => _s.GetInt32(0x4); set => _s.SetInt32(0x4, value); }

        public int V2 { get => _s.GetInt32(0x8); set => _s.SetInt32(0x8, value); }

        public int V3 { get => _s.GetInt32(0xc); set => _s.SetInt32(0xc, value); }

        public int Color { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        public int Unknown { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }
    }

    public class KAR_ZoneCollisionJoint : HSDAccessor
    {
        public override int TrimmedSize => 0x4C;

        public int BoneID { get => _s.GetInt32(0x0); set => _s.SetInt32(0x0, value); }

        public int ZoneVertexStart { get => _s.GetInt32(0x4); set => _s.SetInt32(0x4, value); }

        public int ZoneVertexSize { get => _s.GetInt32(0x8); set => _s.SetInt32(0x8, value); }

        public int ZoneFaceStart { get => _s.GetInt32(0xc); set => _s.SetInt32(0xc, value); }

        public int ZoneFaceSize { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        //public int UnknownPointer { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }

        //public int Pointer { get => _s.GetInt32(0x18); set => _s.SetInt32(0x18, value); }

        public float UnknownStart1 { get => _s.GetFloat(0x1c); set => _s.SetFloat(0x1c, value); }

        public float UnknownSize1 { get => _s.GetFloat(0x20); set => _s.SetFloat(0x20, value); }

        public float UnknownStart2 { get => _s.GetFloat(0x24); set => _s.SetFloat(0x24, value); }

        public float UnknownSize2 { get => _s.GetFloat(0x28); set => _s.SetFloat(0x28, value); }

        public float UnknownStart3 { get => _s.GetFloat(0x2c); set => _s.SetFloat(0x2c, value); }

        public float UnknownSize3 { get => _s.GetFloat(0x30); set => _s.SetFloat(0x30, value); }

        public float UnknownStart4 { get => _s.GetFloat(0x34); set => _s.SetFloat(0x34, value); }

        public float UnknownSize4 { get => _s.GetFloat(0x38); set => _s.SetFloat(0x38, value); }

        public float UnknownStart5 { get => _s.GetFloat(0x3c); set => _s.SetFloat(0x3c, value); }

        public float UnknownSize5 { get => _s.GetFloat(0x40); set => _s.SetFloat(0x40, value); }

        public float UnknownStart6 { get => _s.GetFloat(0x44); set => _s.SetFloat(0x44, value); }

        public float UnknownSize6 { get => _s.GetFloat(0x48); set => _s.SetFloat(0x48, value); }
    }

    public class KAR_grCollisionNode : HSDAccessor
    {
        public override int TrimmedSize => 0x30;

        public GXVector3[] Vertices
        {
            get
            {
                if (VertexCount == 0)
                    return null;

                var str = _s.GetReference<HSDAccessor>(0x00)._s;

                var v = new GXVector3[VertexCount];

                for (int i = 0; i < v.Length; i++)
                    v[i] = new GXVector3() { X = str.GetFloat(i * 12), Y = str.GetFloat(i * 12 + 4), Z = str.GetFloat(i * 12 + 8) };

                return v;
            }
            set
            {
                if(value == null)
                {
                    VertexCount = 0;
                    _s.SetReference(0x00, null);
                }
                else
                {
                    VertexCount = value.Length;

                    var re = _s.GetCreateReference<HSDAccessor>(0x00)._s;
                    re.Resize(12 * value.Length);

                    for(int i = 0; i < value.Length; i++)
                    {
                        re.SetFloat(i * 12, value[i].X);
                        re.SetFloat(i * 12 + 4, value[i].Y);
                        re.SetFloat(i * 12 + 8, value[i].Z);
                    }
                }
            }
        }
        
        public int VertexCount { get => _s.GetInt32(0x04); internal set => _s.SetInt32(0x04, value); }

        public KAR_CollisionTriangle[] Triangles
        {
            get
            {
                var r = _s.GetReference<HSDAccessor>(0x08);
                if (r == null)
                    return null;
                return r._s.GetEmbeddedAccessorArray<KAR_CollisionTriangle>(0x00, TriangleCount);
            }
            set
            {
                if(value == null)
                {
                    TriangleCount = 0;
                    _s.SetReference(0x08, null);
                }
                else
                {
                    TriangleCount = value.Length;
                    var r = _s.GetCreateReference<HSDAccessor>(0x08);
                    r._s.SetEmbeddedAccessorArray(0x00, value);
                }
            }
        }

        public int TriangleCount { get => _s.GetInt32(0x0C); internal set => _s.SetInt32(0x0C, value); }
        
        public KAR_CollisionJoint[] Joints
        {
            get
            {
                var r = _s.GetReference<HSDAccessor>(0x10);
                if (r == null)
                    return null;
                return r._s.GetEmbeddedAccessorArray<KAR_CollisionJoint>(0x00, JointCount);
            }
            set
            {
                if (value == null)
                {
                    JointCount = 0;
                    _s.SetReference(0x10, null);
                }
                else
                {
                    JointCount = value.Length;
                    var r = _s.GetCreateReference<HSDAccessor>(0x10);
                    r._s.SetEmbeddedAccessorArray(0x00, value);
                }
            }
        }

        public int JointCount { get => _s.GetInt32(0x14); internal set => _s.SetInt32(0x14, value); }
        
        public GXVector3[] ZoneVertices
        {
            get
            {
                if (ZoneVertexCount == 0)
                    return null;

                var str = _s.GetReference<HSDAccessor>(0x18)._s;

                var v = new GXVector3[ZoneVertexCount];

                for (int i = 0; i < v.Length; i++)
                    v[i] = new GXVector3() { X = str.GetFloat(i * 12), Y = str.GetFloat(i * 12 + 4), Z = str.GetFloat(i * 12 + 8) };

                return v;
            }
            set
            {
                if (value == null)
                {
                    ZoneVertexCount = 0;
                    _s.SetReference(0x18, null);
                }
                else
                {
                    ZoneVertexCount = value.Length;

                    var re = _s.GetCreateReference<HSDAccessor>(0x18)._s;
                    re.Resize(12 * value.Length);

                    for (int i = 0; i < value.Length; i++)
                    {
                        re.SetFloat(i * 12, value[i].X);
                        re.SetFloat(i * 12 + 4, value[i].Y);
                        re.SetFloat(i * 12 + 8, value[i].Z);
                    }
                }
            }
        }

        public int ZoneVertexCount { get => _s.GetInt32(0x1C); internal set => _s.SetInt32(0x1C, value); }
        
        public KAR_ZoneCollisionTriangle[] ZoneTriangles
        {
            get
            {
                var r = _s.GetReference<HSDAccessor>(0x20);
                if (r == null)
                    return null;
                return r._s.GetEmbeddedAccessorArray<KAR_ZoneCollisionTriangle>(0x00, ZoneTriangleCount);
            }
            set
            {
                if (value == null)
                {
                    ZoneTriangleCount = 0;
                    _s.SetReference(0x20, null);
                }
                else
                {
                    ZoneTriangleCount = value.Length;
                    var r = _s.GetCreateReference<HSDAccessor>(0x20);
                    r._s.SetEmbeddedAccessorArray(0x00, value);
                }
            }
        }

        public int ZoneTriangleCount { get => _s.GetInt32(0x24); internal set => _s.SetInt32(0x24, value); }
        
        public KAR_ZoneCollisionJoint[] ZoneJoints
        {
            get
            {
                var r = _s.GetReference<HSDAccessor>(0x28);
                if (r == null)
                    return null;
                return r._s.GetEmbeddedAccessorArray<KAR_ZoneCollisionJoint>(0x00, ZoneJointCount);
            }
            set
            {
                if (value == null)
                {
                    ZoneJointCount = 0;
                    _s.SetReference(0x28, null);
                }
                else
                {
                    ZoneJointCount = value.Length;
                    var r = _s.GetCreateReference<HSDAccessor>(0x28);
                    r._s.SetEmbeddedAccessorArray(0x00, value);
                }
            }
        }

        public int ZoneJointCount { get => _s.GetInt32(0x2C); internal set => _s.SetInt32(0x2C, value); }
    }
}
