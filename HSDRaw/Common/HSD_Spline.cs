namespace HSDRaw.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class HSD_Spline : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public byte Type { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }

        public short PointCount { get => _s.GetInt16(0x02); set => _s.SetInt16(0x02, value); }

        public float Tension { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public HSD_Vector3[] Points
        {
            get
            {
                return _s.GetCreateReference<HSDArrayAccessor<HSD_Vector3>>(0x08).Slice(PointCount);
            }
            set
            {
                if(value == null || value.Length == 0)
                {
                    _s.SetReference(0x08, null);
                    PointCount = 0;
                }
                else
                {
                    _s.GetCreateReference<HSDArrayAccessor<HSD_Vector3>>(0x08).Array = value;
                    PointCount = (short)value.Length;
                }
            }
        }

        public float TotalLength { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        public HSDFloatArray Lengths
        {
            get => _s.GetReference<HSDFloatArray>(0x10);
            set => _s.SetReference(0x10, value);
        }

        public HSDArrayAccessor<HSD_SegPoly> SegPolys
        {
            get => _s.GetReference<HSDArrayAccessor<HSD_SegPoly>>(0x14);
            set => _s.SetReference(0x14, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public void GetPointOnPath(float path, out float X, out float Y, out float Z)
        {
            X = 0;
            Y = 0;
            Z = 0;

            for (int i = 0; i < PointCount; i++)
            {
                if (path < Lengths[i])
                {
                    i--;

                    float prevLength = Lengths[i];
                    float nextLength = Lengths[i + 1];

                    HSD_Vector3 prevPoint = Points[i];
                    HSD_Vector3 nextPoint;

                    if (i >= PointCount)
                    {
                        nextPoint = Points[0];
                    }
                    else
                    {
                        nextPoint = Points[i + 1];
                    }

                    var weight = (path - prevLength) / (nextLength - prevLength);

                    X = prevPoint.X + (nextPoint.X - prevPoint.X) * weight;
                    Y = prevPoint.Y + (nextPoint.Y - prevPoint.Y) * weight;
                    Z = prevPoint.Z + (nextPoint.Z - prevPoint.Z) * weight;
                    return;
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HSD_SegPoly : HSDAccessor
    {
        public override int TrimmedSize => 0x14;

        public float Value1 { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }
        public float Value2 { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }
        public float Value3 { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }
        public float Value4 { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }
        public float Value5 { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }
    }
}
