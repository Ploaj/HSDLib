namespace HSDRaw.Melee.Pl
{
    public class SBM_PhysicsGroup : HSDAccessor
    {
        public override int TrimmedSize => 0x14;
        
        public SBM_DynamicDesc[] Physics
        {
            get
            {
                var re = _s.GetReference<HSDArrayAccessor<SBM_DynamicDesc>>(0x04);
                if (re == null)
                    return null;
                return re.Array;
            }
            set
            {
                if (value == null)
                {
                    _s.SetInt32(0x00, 0);
                    _s.SetReference(0x04, null);
                }
                else
                {
                    _s.SetInt32(0x00, value.Length);
                    var re = _s.GetCreateReference<HSDArrayAccessor<SBM_DynamicDesc>>(0x04);
                    re.Array = value;
                }
            }
        }

        public int DynamicHitBubbleCount { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public SBM_DynamicHitBubble[] DynamicHitBubbles
        {
            get => _s.GetReference<HSDArrayAccessor<SBM_DynamicHitBubble>>(0x0C)?.Array;
            set => _s.GetCreateReference<HSDAccessor>(0x0C)._s.SetEmbeddedAccessorArray(0, value);
        }
    }

    public class ItemDynamics : HSDAccessor
    {
        public int DynamicsNum { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }
        public SBM_DynamicDesc[] DynamicsDesc
        {
            get
            {
                var re = _s.GetReference<HSDArrayAccessor<SBM_DynamicDesc>>(0x04);
                if (re == null)
                    return null;
                return re.Array;
            }
            set
            {
                if (value == null)
                {
                    _s.SetInt32(0x00, 0);
                    _s.SetReference(0x04, null);
                }
                else
                {
                    _s.SetInt32(0x00, value.Length);
                    var re = _s.GetCreateReference<HSDArrayAccessor<SBM_DynamicDesc>>(0x04);
                    re.Array = value;
                }
            }
        }

    }


    public class SBM_DynamicHitBubble : HSDAccessor
    {
        public override int TrimmedSize => 0x14;

        public int BoneIndex { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }
        public float Z { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }
        public float Y { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }
        public float X { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }
        public float Size { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }
    }

    public class SBM_DynamicDesc : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public int BoneIndex { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public float PARAM1 { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }
        public float PARAM2 { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }
        public float PARAM3 { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }

        public SBM_DynamicParams[] Parameters
        {
            get
            {
                var re = _s.GetReference<HSDArrayAccessor<SBM_DynamicParams>>(0x04);
                if (re == null)
                    return null;
                return re.Array;
            }
            set
            {
                if (value == null)
                {
                    _s.SetInt32(0x08, 0);
                    _s.SetReference(0x04, null);
                }
                else
                {
                    _s.SetInt32(0x08, value.Length);
                    var re = _s.GetCreateReference<HSDArrayAccessor<SBM_DynamicParams>>(0x04);
                    re.Array = value;
                }
            }
        }
    }

    public class SBM_DynamicParams : HSDAccessor
    {
        public override int TrimmedSize => 0x3C;

        public float PARAM1 { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }
        public float PARAM2 { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }
        public float PARAM3 { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }
        public float PARAM4 { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }
        public float PARAM5 { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }
        public float PARAM6 { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }
        public float PARAM7 { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }
        public float PARAM8 { get => _s.GetFloat(0x1C); set => _s.SetFloat(0x1C, value); }
        public float PARAM9 { get => _s.GetFloat(0x20); set => _s.SetFloat(0x20, value); }
        public float PARAM10 { get => _s.GetFloat(0x24); set => _s.SetFloat(0x24, value); }
        public float PARAM11 { get => _s.GetFloat(0x28); set => _s.SetFloat(0x28, value); }
        public float PARAM12 { get => _s.GetFloat(0x2C); set => _s.SetFloat(0x2C, value); }
        public float PARAM13 { get => _s.GetFloat(0x30); set => _s.SetFloat(0x30, value); }
        public float PARAM14 { get => _s.GetFloat(0x34); set => _s.SetFloat(0x34, value); }
        public float PARAM15 { get => _s.GetFloat(0x38); set => _s.SetFloat(0x38, value); }
    }
}
