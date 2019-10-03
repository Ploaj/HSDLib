namespace HSDRaw.Common
{
    public class HSD_ParticleGroup : HSDAccessor
    {
        public int ParticleCount { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

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
        }
    }

    public class HSD_Particle : HSDAccessor
    {

    }
}
