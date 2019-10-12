namespace HSDRaw.AirRide.Gr
{
    public class KAR_grViewRegion : HSDAccessor
    {
        public override int TrimmedSize => 0x20;

        public ushort[] Indices
        {
            get
            {
                var s = _s.GetReference<HSDAccessor>(0x00);

                if (s == null)
                    return null;

                ushort[] indices = new ushort[Count];
                for(int i = 0; i < indices.Length; i++)
                {
                    indices[i] = (ushort)s._s.GetInt16(i * 2);
                }
                return indices;
            }
            set
            {
                if(value == null)
                {
                    Count = 0;
                    _s.SetReference(0x00, null);
                }
                else
                {
                    Count = (ushort)value.Length;

                    var r = _s.GetCreateReference<HSDAccessor>(0x00);
                    r._s.Resize(value.Length * 2);
                    for (int i = 0; i < value.Length; i++)
                        r._s.SetInt16(i * 2, (short)value[i]);
                }
            }
        }

        public ushort Count { get => (ushort)_s.GetInt16(0x04); internal set => _s.SetInt16(0x04, (short)value); }

        public float MinX { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public float MinY { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        public float MinZ { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }

        public float MaxX { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }

        public float MaxY { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }

        public float MaxZ { get => _s.GetFloat(0x1C); set => _s.SetFloat(0x1C, value); }
    }

    public class KAR_grUnknownBounding : HSDAccessor
    {
        public override int TrimmedSize => 0x24;

        public ushort[] Indices
        {
            get
            {
                var s = _s.GetReference<HSDAccessor>(0x00);

                if (s == null)
                    return null;

                ushort[] indices = new ushort[Count];
                for (int i = 0; i < indices.Length; i++)
                {
                    indices[i] = (ushort)s._s.GetInt16(i * 2);
                }
                return indices;
            }
            set
            {
                if (value == null)
                {
                    Count = 0;
                    _s.SetReference(0x00, null);
                }
                else
                {
                    Count = (ushort)value.Length;

                    var r = _s.GetCreateReference<HSDAccessor>(0x00);
                    r._s.Resize(value.Length * 2);
                    for (int i = 0; i < value.Length; i++)
                        r._s.SetInt16(i * 2, (short)value[i]);
                }
            }
        }

        public ushort Count { get => (ushort)_s.GetInt16(0x04); internal set => _s.SetInt16(0x04, (short)value); }

        public float MinX { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public float MinY { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        public float MinZ { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }

        public float MaxX { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }

        public float MaxY { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }

        public float MaxZ { get => _s.GetFloat(0x1C); set => _s.SetFloat(0x1C, value); }

        public int Unknown { get => _s.GetInt32(0x20); set => _s.SetInt32(0x20, value); }
    }

    public class KAR_grBoundingBox : HSDAccessor
    {
        public override int TrimmedSize => 0x18;
        
        public float MinX { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }

        public float MinY { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public float MinZ { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public float MaxX { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        public float MaxY { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }

        public float MaxZ { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }
    }
}
