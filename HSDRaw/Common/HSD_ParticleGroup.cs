namespace HSDRaw.Common
{
    public class HSD_ParticleGroup : HSDAccessor
    {
        public short Unknown1 { get => _s.GetInt16(0x00); set => _s.SetInt16(0x00, value); }

        public short Unknown2 { get => _s.GetInt16(0x02); set => _s.SetInt16(0x02, value); }

        public int Unknown3 { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int ParticleCount { get => _s.GetInt32(0x08); internal set => _s.SetInt32(0x08, value); }

        public HSD_Particle[] Particles
        {
            get
            {
                HSD_Particle[] p = new HSD_Particle[ParticleCount];

                int prevSize = 0;
                for(int i = 0; i < p.Length; i++)
                {
                    var size = _s.GetInt32(0x0C + i * 4);
                    if (i > 0)
                    {
                        p[i - 1] = new HSD_Particle();
                        p[i - 1]._s = _s.GetEmbeddedStruct(prevSize, size - prevSize);
                    }
                    prevSize = size;
                }
                p[p.Length - 1] = new HSD_Particle();
                p[p.Length - 1]._s = _s.GetEmbeddedStruct(prevSize, _s.Length - prevSize);

                return p;
            }

            set
            {
                var size = 0x0C + 4 * value.Length;

                _s.Resize(size);

                int i = 0;
                foreach (var v in value)
                {
                    if (size % 0x8 != 0)
                        size += 0x8 - (size % 0x8);

                    _s.SetInt32(0x0C + 4 * i++, size);

                    size += v._s.Length;
                }

                if (size % 0x8 != 0)
                    size += 0x8 - (size % 0x8);

                _s.Resize(size);

                size = 0x0C + 4 * value.Length;
                foreach (var v in value)
                {
                    if (size % 0x8 != 0)
                        size += 0x8 - (size % 0x8);

                    _s.SetBytes(size, v._s.GetData());

                    size += v._s.Length;
                }

                ParticleCount = value.Length;
            }
        }
    }

    public enum ParticleType
    {
        Disc,
        Line,
        Tornado,
        DiscCT,
        DiscCD,
        Rect,
        Cone,
        Cylinder,
        Sphere
    }

    public class HSD_Particle : HSDAccessor
    {
        public ParticleType Type { get => (ParticleType)_s.GetInt16(0x00); set => _s.SetInt16(0x00, (short)value); }

        public short TexGroup { get => _s.GetInt16(0x02); set => _s.SetInt16(0x02, value); }

        public short GenLife { get => _s.GetInt16(0x04); set => _s.SetInt16(0x04, value); }

        public short Life { get => _s.GetInt16(0x06); set => _s.SetInt16(0x06, value); }

        public int Kind { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public float Gravity { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        public float Friction { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }

        public float VX { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }

        public float VY { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }

        public float VZ { get => _s.GetFloat(0x1C); set => _s.SetFloat(0x1C, value); }

        public float Radius { get => _s.GetFloat(0x20); set => _s.SetFloat(0x20, value); }

        public float Angle { get => _s.GetFloat(0x24); set => _s.SetFloat(0x24, value); }

        public float Random { get => _s.GetFloat(0x28); set => _s.SetFloat(0x28, value); }

        public float Size { get => _s.GetFloat(0x2C); set => _s.SetFloat(0x2C, value); }

        public float Param1 { get => _s.GetFloat(0x30); set => _s.SetFloat(0x30, value); }

        public float Param2 { get => _s.GetFloat(0x34); set => _s.SetFloat(0x34, value); }

        public float Param3 { get => _s.GetFloat(0x38); set => _s.SetFloat(0x38, value); }

        public byte[] TrackData
        {
            get
            {
                return _s.GetBytes(0x3C, _s.Length - 0x3C);
            }
            set
            {
                _s.Resize(0x3C + value.Length);
                _s.SetBytes(0x3C, value);
            }
        }
    }
}
