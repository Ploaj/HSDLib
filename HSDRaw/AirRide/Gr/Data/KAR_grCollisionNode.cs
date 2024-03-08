
using HSDRaw.GX;
using System;

namespace HSDRaw.AirRide.Gr.Data
{
    [Flags]
    public enum KCCollFlag
    {
        None,
        Floor       = 0x1,
        Wall        = 0x2,
        Ceiling     = 0x4,
        Unknown     = 0x8,
    }

    [Flags]
    public enum KCConveyorDirection
    {
        DirBack     = 0x1,
        DirFront    = 0x2,
        DirRight    = 0x4,
        DirLeft     = 0x8,
    }

    public class KAR_CollisionTriangle : HSDAccessor
    {
        public override int TrimmedSize => 0x14;

        public int V1 { get => _s.GetInt32(0x0); set => _s.SetInt32(0x0, value); }

        public int V2 { get => _s.GetInt32(0x4); set => _s.SetInt32(0x4, value); }

        public int V3 { get => _s.GetInt32(0x8); set => _s.SetInt32(0x8, value); }

        /// <summary>
        /// Determintes the type of collision
        /// </summary>
        public KCCollFlag Flags 
        { 
            get => (KCCollFlag)(_s.GetInt32(0xc) & 0xF); 
            set => _s.SetInt32(0xc, (_s.GetInt32(0xc) & ~0xF) | ((int)value & 0xF)); 
        }

        /// <summary>
        /// References data from 0x00 of GrCommon.dat
        /// </summary>
        public byte GrCommonIndex 
        { 
            get => (byte)((_s.GetInt32(0xc) >> 4) & 0xFF); 
            set => _s.SetInt32(0xc, (_s.GetInt32(0xc) & ~0xFF0) | ((value & 0xFF) << 4)); 
        }

        private int _materialflag { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        /// <summary>
        /// Indicates collision is rough
        /// This is usually used so you can stop quickly
        /// </summary>
        public byte Rough
        {
            get => (byte)(_materialflag & 0x3);
            set => _materialflag = (_materialflag & ~0x3) | (value & 0x3);
        }

        /// <summary>
        /// starts at 0x20 in stage node
        /// </summary>
        public byte StageNodeReflectIndex
        {
            get => (byte)((_materialflag >> 2) & 0x7);
            set => _materialflag = (_materialflag & ~0x1C) | ((value & 0x7) << 2);
        }

        /// <summary>
        /// Indicates to not use tree lookup for this
        /// </summary>
        public bool SegmentMove
        {
            get => (_materialflag & 0x20) != 0;
            set => _materialflag = (_materialflag & ~0x20) | (value ? 0x20 : 0);
        }

        /// <summary>
        /// Back and Front cannot be used together
        /// Left and Right cannot be used together
        /// </summary>
        public KCConveyorDirection ConveyorDirection
        {
            get => (KCConveyorDirection)((_materialflag >> 6) & 0xF);
            set => _materialflag = (_materialflag & ~0x3C0) | (((int)value & 0xF) << 6);
        }

        /// <summary>
        /// starts at 0x40 in stage node
        /// </summary>
        public byte StageNodeForceReflectIndex
        {
            get => (byte)((_materialflag >> 10) & 0x7);
            set => _materialflag = (_materialflag & ~0x1C00) | ((value & 0x7) << 10);
        }

        // TODO:
        /*
            00002000 - 800d190c
            00004000 - 800d1928
            00008000 - ?? unused ??

            00010000 - ?? unused ??
            00020000 - 800d1944
            00040000 - ?? unused ??
            00080000 - 800d19c8

            00100000 - 800d19e4
            00200000 - 800d1a00
            00400000 - 800d1a1c
            00800000 - 800d1a38
         */

    }

    public class KAR_CollisionJoint : HSDAccessor
    {
        public override int TrimmedSize => 0x1C;

        public int BoneID { get => _s.GetInt32(0x0); set => _s.SetInt32(0x0, value); }

        public int VertexStart { get => _s.GetInt32(0x4); set => _s.SetInt32(0x4, value); }

        public int VertexSize { get => _s.GetInt32(0x8); set => _s.SetInt32(0x8, value); }

        public int FaceStart { get => _s.GetInt32(0xc); set => _s.SetInt32(0xc, value); }

        public int FaceSize { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        /// <summary>
        /// Values between 0-4
        /// 0 - seems to be used on bones that are animated, such as the spiny thing at top of city trial
        /// </summary>
        public int Flags { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }

        /// <summary>
        /// When Flag is set to 2 this exists
        /// Seems to point to a Vec3
        /// </summary>
        public int Pointer { get => _s.GetInt32(0x18); set => _s.SetInt32(0x18, value); }
    }

    public class KAR_ZoneCollisionTriangle : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public KCCollFlag CollFlags { get => (KCCollFlag)_s.GetInt32(0x0); set => _s.SetInt32(0x0, (int)value); }

        public int V1 { get => _s.GetInt32(0x4); set => _s.SetInt32(0x4, value); }

        public int V2 { get => _s.GetInt32(0x8); set => _s.SetInt32(0x8, value); }

        public int V3 { get => _s.GetInt32(0xc); set => _s.SetInt32(0xc, value); }

        /// <summary>
        /// F0000000 - Index
        /// 01ffffff
        /// 2/3 - Dash Gate	
        /// 	- 0x9C of stage
        /// 	- index< 2
        /// 4 - Dash Ring 
        /// 	- 0xB4 of stage
        /// 	- Index< 2
        /// 7 - SuperJump
        /// 9 - Jump
        /// 10 - Spin
        /// 11 - AirFlow
        /// 12/13 - Switch 800d2a1c
        /// 25 - Dead
        /// 32 - 800eefd4 - also checks flag 0x2000020
        /// </summary>
        public int TypeFlags { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        public int UnkFlags { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }
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

        public float Mtx00 { get => _s.GetFloat(0x1c); set => _s.SetFloat(0x1c, value); }

        public float Mtx10 { get => _s.GetFloat(0x20); set => _s.SetFloat(0x20, value); }

        public float Mtx20 { get => _s.GetFloat(0x24); set => _s.SetFloat(0x24, value); }

        public float Mtx30 { get => _s.GetFloat(0x28); set => _s.SetFloat(0x28, value); }

        public float Mtx01 { get => _s.GetFloat(0x2c); set => _s.SetFloat(0x2c, value); }

        public float Mtx11 { get => _s.GetFloat(0x30); set => _s.SetFloat(0x30, value); }

        public float Mtx21 { get => _s.GetFloat(0x34); set => _s.SetFloat(0x34, value); }

        public float Mtx31 { get => _s.GetFloat(0x38); set => _s.SetFloat(0x38, value); }

        public float Mtx02 { get => _s.GetFloat(0x3c); set => _s.SetFloat(0x3c, value); }

        public float Mtx12 { get => _s.GetFloat(0x40); set => _s.SetFloat(0x40, value); }

        public float Mtx22 { get => _s.GetFloat(0x44); set => _s.SetFloat(0x44, value); }

        public float Mtx32 { get => _s.GetFloat(0x48); set => _s.SetFloat(0x48, value); }
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
