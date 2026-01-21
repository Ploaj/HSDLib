using System;

namespace HSDRaw.AirRide.Gr.Data
{
    public class KAR_grDataCommon : HSDAccessor
    {
        public KAR_grMaterialNode Materials { get => _s.GetReference<KAR_grMaterialNode>(0x00); set => _s.SetReference(0x00, value); }
    }

    public class KAR_grMaterialNode : HSDAccessor
    {
        public override int TrimmedSize => 0x594;

        public KAR_grMaterial[] Materials 
        {
            get
            {
                return _s.GetEmbeddedAccessorArray<KAR_grMaterial>(0x00, 0x2F);
            }
            set
            {
                var arr = value;
                Array.Resize(ref arr, 0x2F);
                _s.SetEmbeddedAccessorArray(0x00, arr);
            }
        }

        public KAR_grMaterial2[] Materials2
        {
            get
            {
                return _s.GetEmbeddedAccessorArray<KAR_grMaterial2>(0x524, 8);
            }
            set
            {
                var arr = value;
                Array.Resize(ref arr, 8);
                _s.SetEmbeddedAccessorArray(0x524, arr);
            }
        }

        public float x584 { get => _s.GetFloat(0x584); set => _s.SetFloat(0x584, value); }
        public float x588 { get => _s.GetFloat(0x588); set => _s.SetFloat(0x588, value); }
        public float x58C { get => _s.GetFloat(0x58C); set => _s.SetFloat(0x58C, value); }
        public float x590 { get => _s.GetFloat(0x590); set => _s.SetFloat(0x590, value); }
    }

    public class KAR_grMaterial : HSDAccessor
    {
        public override int TrimmedSize => 0x1C;

        public float x00 { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }
        public float x04 { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }
        public float x08 { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }
        public float x0C { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        public int x10 { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }
        public int x14 { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }
        public int x18 { get => _s.GetInt32(0x18); set => _s.SetInt32(0x18, value); }
    }

    public class KAR_grMaterial2 : HSDAccessor
    {
        public override int TrimmedSize => 0xC;

        public float x00 { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }

        public float x04 { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public int x08 { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }
    }
}
