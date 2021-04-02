namespace HSDRaw.AirRide.Gr.Data
{
    public class KAR_grCollisionTreeNode : HSDAccessor
    {
        public override int TrimmedSize => 4;
        public KAR_grCollisionTree Partition { get => _s.GetReference<KAR_grCollisionTree>(0x0); set => _s.SetReference(0x0, value); }
    }

    public class KAR_grCollisionTree : HSDAccessor
    {
        public override int TrimmedSize => 0x5C;


        public KAR_grPartitionBucket[] Buckets
        {
            get
            {
                var s = _s.GetReference<HSDFixedLengthPointerArrayAccessor<KAR_grPartitionBucket>>(0x00);
                return s != null ? s.Array : null;
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    BucketsCount = 0;
                    _s.SetReference(0x00, null);
                }
                else
                {
                    BucketsCount = (ushort)value.Length;
                    _s.SetReference(0x00, new HSDFixedLengthPointerArrayAccessor<KAR_grPartitionBucket>() { Array = value });
                }
            }
        }

        public ushort BucketsCount { get => (ushort)_s.GetInt16(0x4); internal set => _s.SetInt16(0x4, (short)value); }

        public byte CollidableTriangleDataType { get => _s.GetByte(0x8); set => _s.SetByte(0x8, value); }

        public ushort[] CollidableTriangles
        {
            get
            {
                var s = _s.GetReference<HSDUShortArray>(0x0C);
                return s != null ? s.Array : null;
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    CollidableTriangleCount = 0;
                    _s.SetReference(0x0C, null);
                }
                else
                {
                    CollidableTriangleCount = (ushort)value.Length;
                    _s.SetReference(0x0C, new HSDUShortArray() { Array = value });
                }
            }
        }

        public ushort CollidableTriangleCount { get => (ushort)_s.GetInt16(0x10); internal set => _s.SetInt16(0x10, (short)value); }


        public int BitTableDataType { get => _s.GetInt32(0x50); set => _s.SetInt32(0x50, value); }

        /*public bool[] BitTable
        {
            get
            {
                var s = _s.GetReference<HSDUShortArray>(0x54);
                return s != null ? s.Array : null;
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    BitTableCount = 0;
                    _s.SetReference(0x54, null);
                }
                else
                {
                    BitTableCount = (ushort)value.Length;
                    _s.SetReference(0x54, new HSDUShortArray() { Array = value });
                }
            }
        }*/

        public ushort BitTableCount { get => (ushort)_s.GetInt16(0x58); set => _s.SetInt16(0x58, (short)value); }
    }

    public class KAR_grPartitionBucket : HSDAccessor
    {
        public override int TrimmedSize => 0x50;

        public float MinX { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }
        public float MinY { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }
        public float MinZ { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }
        public float MaxX { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }
        public float MaxY { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }
        public float MaxZ { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }

        public short BucketStart { get => _s.GetInt16(0x18); set => _s.SetInt16(0x18, value); }

        public short BucketCount { get => _s.GetInt16(0x1A); set => _s.SetInt16(0x1A, value); }

        public short CollTriangleStart { get => _s.GetInt16(0x1C); set => _s.SetInt16(0x1C, value); }

        public short CollTriangleCount { get => _s.GetInt16(0x1E); set => _s.SetInt16(0x1E, value); }

        public byte Depth { get => _s.GetByte(0x4C); set => _s.SetByte(0x4C, value); }
    }
}
