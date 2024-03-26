using System;
using System.ComponentModel;

namespace HSDRaw.Common
{
    public class HSD_ParticleJoint : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public int ParticleID
        {
            get => _s.GetInt32(0x04) >> 6;
            set => _s.SetInt32(0x04, (value << 6) | (_s.GetInt32(0x04) & 0b111111));
        }

        public byte ParticleBank
        {
            get => (byte)(_s.GetInt32(0x04) & 0b111111);
            set => _s.SetInt32(0x04, value | (_s.GetInt32(0x04) & ~0b111111));
        }
}

    public class HSD_ParticleGroup : HSDAccessor
    {
        public short Unknown1 { get => _s.GetInt16(0x00); set => _s.SetInt16(0x00, value); }

        public short Unknown2 { get => _s.GetInt16(0x02); set => _s.SetInt16(0x02, value); }

        public int EffectIDStart { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int GeneratorCount { get => _s.GetInt32(0x08); internal set => _s.SetInt32(0x08, value); }

        public HSD_ParticleGenerator[] Generators
        {
            get
            {
                HSD_ParticleGenerator[] p = new HSD_ParticleGenerator[GeneratorCount];

                int prevSize = 0;
                for(int i = 0; i < p.Length; i++)
                {
                    var size = _s.GetInt32(0x0C + i * 4);

                    if(size == 0)
                    {
                        p[i - 1] = new HSD_ParticleGenerator();
                    }
                    else
                    {
                        if (i > 0)
                        {
                            p[i - 1] = new HSD_ParticleGenerator();
                            p[i - 1]._s = _s.GetEmbeddedStruct(prevSize, size - prevSize);
                        }

                        prevSize = size;
                    }
                }
                p[p.Length - 1] = new HSD_ParticleGenerator();
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
                    if(v == null || v.IsBlank())
                    {
                        _s.SetInt32(0x0C + 4 * i++, 0);
                    }
                    else
                    {
                        if (size % 0x8 != 0)
                            size += 0x8 - (size % 0x8);

                        _s.SetInt32(0x0C + 4 * i++, size);

                        size += v._s.Length;
                    }
                }

                if (size % 0x8 != 0)
                    size += 0x8 - (size % 0x8);

                _s.Resize(size);

                size = 0x0C + 4 * value.Length;
                foreach (var v in value)
                {
                    if (v == null || v.IsBlank())
                    {

                    }
                    else
                    {
                        if (size % 0x8 != 0)
                            size += 0x8 - (size % 0x8);

                        _s.SetBytes(size, v._s.GetData());

                        size += v._s.Length;
                    }
                }

                GeneratorCount = value.Length;
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
        Sphere,
    }

    public enum GeneratorFlags
    {
        HasChild = 0x80,

        x100 = 0x100,
        BillboardA = 0x800,

        Bit26 = 0x02000000,
        Bit27 = 0x04000000,
        Bit28 = 0x08000000,
    }

    [Flags]
    public enum ParticleKind : uint
    {
        None        = 0x00000000,

        Gravity     = 0x00000001,
        Friction    = 0x00000002,
        Tornado     = 0x00000004,
        Bit4        = 0x00000008,

        ComTLUT     = 0x00000010,
        MirrorS     = 0x00000020,
        MirrorT     = 0x00000040,
        PrimEnv     = 0x00000080,

        IMMRND      = 0x00000100,
        NearestTex   = 0x00000200,
        HasTexture  = 0x00000400,
        ExecPause   = 0x00000800,

        //          = 0x00001000, pnt jobj index for these 3 bits
        //          = 0x00002000,
        //          = 0x00004000,
        PNTJOBJ     = 0x00008000,

        BillboardG  = 0x00010000,
        BillboardA  = 0x00020000,
        FlipS       = 0x00040000,
        FlipT       = 0x00080000,

        Trail       = 0x00100000,
        DirVec      = 0x00200000,
        BlendOne    = 0x00400000,
        //          = 0x00800000,

        Fog         = 0x01000000,
        Bit26       = 0x02000000, // Bit 26 - 27 set by particle event 0xEF
        Bit27       = 0x04000000,
        Bit28       = 0x08000000,
        BitGroup = Bit26 | Bit27 | Bit28,

        Bit29       = 0x10000000,
        Bit30       = 0x20000000,
        Point       = 0x40000000,
        Lighting    = 0x80000000
    }

    public class HSD_ParticleGenerator : HSDAccessor
    {
        public ParticleType TypeShape { get => (ParticleType)(_s.GetInt16(0x00) & 0xF); set => _s.SetInt16(0x00, (short)((_s.GetInt16(0x00) & ~0xF) | (int)value)); }

        public GeneratorFlags Flags { get => (GeneratorFlags)(_s.GetInt16(0x00) & ~0xF); set => _s.SetInt16(0x00, (short)((_s.GetInt16(0x00) & 0xF) | (int)value)); }

        public short TexGroup { get => _s.GetInt16(0x02); set => _s.SetInt16(0x02, value); }

        public short GenLife { get => _s.GetInt16(0x04); set => _s.SetInt16(0x04, value); }

        public short Life { get => _s.GetInt16(0x06); set => _s.SetInt16(0x06, value); }

        private int _kind { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public ParticleKind Kind { get => (ParticleKind)_kind; set => _kind = (int)value; }

        public int JobjNumber
        {
            get => (_kind >> 13) & 0x7;
        }

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

        [Browsable(false)]
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

        public override void New()
        {
            _s = new HSDStruct(0x40);
            TrackData = new byte[] { 0xFF };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Particle: {TypeShape} TexG: {TexGroup} ";
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsBlank()
        {
            return TrackData.Length == 0 || TrackData.Length == 1;
        }
    }
}
