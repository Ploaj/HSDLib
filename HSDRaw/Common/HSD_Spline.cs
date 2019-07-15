using HSDRaw.GX;

namespace HSDRaw.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class HSD_Spline : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public short Type { get => _s.GetInt16(0x00); set => _s.SetInt16(0x00, value); }

        public short PointCount{ get => _s.GetInt16(0x02); set => _s.SetInt16(0x02, value); }

        public float Tension { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public GXVector3[] Points
        {
            get
            {
                var r = _s.GetReference<HSDAccessor>(0x08);
                if (r == null)
                    return null;
                GXVector3[] o = new GXVector3[PointCount];
                for(int i = 0; i < o.Length; i++)
                {
                    o[i] = new GXVector3() { X = r._s.GetFloat(i * 0xC), Y = r._s.GetFloat(i * 0xC + 4) , Z = r._s.GetFloat(i * 0xC + 8) };
                }
                return o;
            }
        }

        public float TotalLength { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        public float[] Lengths
        {
            get
            {
                var r = _s.GetReference<HSDAccessor>(0x10);
                if (r == null)
                    return null;
                float[] o = new float[PointCount];
                for (int i = 0; i < o.Length; i++)
                {
                    o[i] = r._s.GetFloat(i * 4);
                }
                return o;
            }
            set
            {
                if(value == null)
                {
                    _s.SetReference(0x10, null);
                    return;
                }
                var r = _s.GetCreateReference<HSDAccessor>(0x10);
                _s.Resize(4 * value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    r._s.SetFloat(i * 4, value[i]);
                }
            }
        }

        //TODO: 0x14 has a pointer to some unknown structure

    }
}
