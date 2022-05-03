using HSDRaw.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HSDRawViewer.Rendering.Renderers
{
    public class ParticleSystem : IDisposable
    {
        public int GeneratorCount { get => _generators.Count; }

        public int ParticleCount { get => Particles.Count; }

        public int LiveGeneratorCount { get => Generators.Count; }

        private List<ParticleGenerator> Generators { get; } = new List<ParticleGenerator>();

        private List<Particle> Particles { get; } = new List<Particle>();

        private List<HSD_ParticleGenerator> _generators { get; } = new List<HSD_ParticleGenerator>();

        private TextureManager _manager { get; } = new TextureManager();

        private List<TexGroup> TexGs { get; } = new List<TexGroup>();


        private class TexGroup
        {
            public List<int> Indexes { get; } = new List<int>();

            private List<int> GLCache = null;

            public List<int> GetGLIndices(TextureManager m)
            {
                if (GLCache == null)
                {
                    GLCache = new List<int>();

                    foreach (var i in Indexes)
                        GLCache.Add(m.GetGLID(i));
                }

                return GLCache;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="bank"></param>
        public void LoadParticleGroup(IEnumerable<HSD_ParticleGenerator> generators, HSD_TEXGraphicBank bank)
        {
            _generators.AddRange(generators);

            if (_manager != null)
                _manager.ClearTextures();

            TexGs.Clear();
            foreach (var p in bank.ParticleImages)
            {
                TexGroup tg = new TexGroup();
                foreach (var tobj in p.ConvertToTOBJs())
                    tg.Indexes.Add(_manager.Add(tobj));
                TexGs.Add(tg);
            }
        }

        /// <summary>
        /// Spawn particle from currently loaded bank
        /// </summary>
        /// <param name="id"></param>
        public ParticleGenerator SpawnGenerator(int id)
        {
            var gen = new ParticleGenerator(_generators[id % 1000]) { Parent = this };

            Generators.Add(gen);
            return gen;
        }

        /// <summary>
        /// Spawn particle from currently loaded bank
        /// </summary>
        /// <param name="id"></param>
        public ParticleGenerator SpawnGenerator(int id, float x, float y, float z)
        {
            var gen = new ParticleGenerator(GetDescriptor(id))
            {
                Parent = this
            };

            gen.JointPosition = new OpenTK.Vector3(x, y, z);

            Generators.Add(gen);
            return gen;
        }

        /// <summary>
        /// Spawn any particle using this loaded bank's resources
        /// </summary>
        /// <param name="gen"></param>
        public void SpawnGenerator(HSD_ParticleGenerator gen)
        {
            var g = new ParticleGenerator(gen) { Parent = this };

            Generators.Add(g);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public void AttachParticle(Particle p)
        {
            Particles.Add(p);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DestroyAllGenerators()
        {
            Generators.Clear();
            Particles.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public HSD_ParticleGenerator GetDescriptor(int id)
        {
            return _generators[id % 1000];
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            // update the generators
            for (int i = Generators.Count - 1; i >= 0; i--) 
            {
                Generators[i].Update();

                // remove dead particles
                if (Generators[i].Dead)
                    Generators.Remove(Generators[i]);
            }

            // process loose particles
            for (int i = Particles.Count - 1; i >= 0; i--)
            {
                // process particle
                Particles[i].Process();

                // remove dead particles
                if (Particles[i].Life <= 0)
                    Particles.Remove(Particles[i]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        public void Render(Camera c)
        {
            // render particles
            var pos = c.ModelViewMatrix.ExtractTranslation();
            foreach (var p in Particles.OrderBy(e => (e.Pos - pos).LengthSquared))
            {
                if (p.TexG >= 0)
                    p.Render(c, TexGs[p.TexG].GetGLIndices(_manager));
                else
                    p.Render(c, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _manager.ClearTextures();
        }
    }
}
