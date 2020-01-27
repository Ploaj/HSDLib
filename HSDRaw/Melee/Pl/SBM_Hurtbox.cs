namespace HSDRaw.Melee.Pl
{
    public class SBM_HurtboxBank : HSDAccessor
    {
        public SBM_Hurtbox[] Hurtboxes
        {
            get
            {
                var re = _s.GetReference<HSDArrayAccessor<SBM_Hurtbox>>(0x04);
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
                    var re = _s.GetCreateReference<HSDArrayAccessor<SBM_Hurtbox>>(0x04);
                    re.Array = value;
                }
            }
        }
    }

    public enum HurtboxPositionType
    {
        Low, Mid, High
    }

    public class SBM_Hurtbox : HSDAccessor
    {
        public override int TrimmedSize => 0x28;

        public int BoneIndex { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public HurtboxPositionType Type { get => (HurtboxPositionType)_s.GetInt32(0x04); set => _s.SetInt32(0x04, (int)value); }

        public int Grabbable { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public float X1 { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }
        public float Y1 { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }
        public float Z1 { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }
        public float X2 { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }
        public float Y2 { get => _s.GetFloat(0x1C); set => _s.SetFloat(0x1C, value); }
        public float Z2 { get => _s.GetFloat(0x20); set => _s.SetFloat(0x20, value); }
        public float Size { get => _s.GetFloat(0x24); set => _s.SetFloat(0x24, value); }

        public override string ToString()
        {
            return $"JOBJ_{BoneIndex} R: {Size} ({X1}, {Y1}, {Z1}) ({X2}, {Y2}, {Z2}) Grabbable: {Grabbable}";
        }
    }
}
