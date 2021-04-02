namespace HSDRaw.AirRide.Gr
{
    public class KAR_grModelBounding : HSDAccessor
    {
        public override int TrimmedSize => 0x20;

        public KAR_grViewRegion[] ViewRegions
        {
            get
            {
                var str = _s.GetReference<HSDAccessor>(0x00);
                if (str == null)
                    return null;

                var regions = str._s.GetEmbeddedAccessorArray<KAR_grViewRegion>(0x00, ViewRegionCount);

                return regions;
            }
            set
            {
                if(value == null || value.Length == 0)
                {
                    ViewRegionCount = 0;
                    _s.SetReference(0x00, null);
                }
                else
                {
                    ViewRegionCount = (short)value.Length;
                    _s.GetCreateReference<HSDAccessor>(0x00)._s.SetEmbeddedAccessorArray(0x00, value);
                }
            }
        }

        public short ViewRegionCount { get => _s.GetInt16(0x04); internal set => _s.SetInt16(0x04, value); }

        public KAR_grDynamicBoundingBoxes[] DynamicBoundingBoxes
        {
            get
            {
                var str = _s.GetReference<HSDAccessor>(0x08);
                if (str == null)
                    return null;

                var regions = str._s.GetEmbeddedAccessorArray<KAR_grDynamicBoundingBoxes>(0x00, UnknownCount);

                return regions;
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    UnknownCount = 0;
                    _s.SetReference(0x08, null);
                }
                else
                {
                    UnknownCount = (short)value.Length;
                    _s.GetCreateReference<HSDAccessor>(0x08)._s.SetEmbeddedAccessorArray(0x00, value);
                }
            }
        }

        public short UnknownCount { get => _s.GetInt16(0x0C); internal set => _s.SetInt16(0x0C, value); }

        public KAR_grStaticBoundingBox[] StaticBoundingBoxes
        {
            get
            {
                var str = _s.GetReference<HSDAccessor>(0x10);
                if (str == null)
                    return null;

                var regions = str._s.GetEmbeddedAccessorArray<KAR_grStaticBoundingBox>(0x00, BoundingBoxCount);

                return regions;
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    BoundingBoxCount = 0;
                    _s.SetReference(0x10, null);
                }
                else
                {
                    BoundingBoxCount = (short)value.Length;
                    _s.GetCreateReference<HSDAccessor>(0x10)._s.SetEmbeddedAccessorArray(0x00, value);
                }
            }
        }

        public short BoundingBoxCount { get => _s.GetInt16(0x14); internal set => _s.SetInt16(0x14, value); }


        public ushort[] Indices
        {
            get
            {
                var s = _s.GetReference<HSDAccessor>(0x18);

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
                if (value == null || value.Length == 0)
                {
                    Count = 0;
                    _s.SetReference(0x18, null);
                }
                else
                {
                    Count = (ushort)value.Length;

                    var r = _s.GetCreateReference<HSDAccessor>(0x18);
                    r._s.Resize(value.Length * 2);
                    for (int i = 0; i < value.Length; i++)
                        r._s.SetInt16(i * 2, (short)value[i]);
                }
            }
        }

        public ushort Count { get => (ushort)_s.GetInt16(0x1C); internal set => _s.SetInt16(0x1C, (short)value); }
    }
}
