namespace HSDRaw.GX
{
    public class GX_Attribute : HSDAccessor
    {
        public override int TrimmedSize { get; } = 0x18;

        public GXAttribName AttributeName { get => (GXAttribName)_s.GetInt32(0); set => _s.SetInt32(0, (int)value); }

        public GXAttribType AttributeType { get => (GXAttribType)_s.GetInt32(4); set => _s.SetInt32(4, (int)value); }

        public GXCompCnt CompCount { get => (GXCompCnt)_s.GetInt32(0x08); set => _s.SetInt32(0x08, (int)value); }

        public GXCompType CompType { get => (GXCompType)_s.GetInt32(0x0C); set => _s.SetInt32(0x0C, (int)value); }

        public byte Scale { get => _s.GetByte(0x10); set => _s.SetByte(0x10, value); }

        public short Stride { get => _s.GetInt16(0x12); set => _s.SetInt16(0x12, value); }

        public byte[] Buffer
        {
            get
            {
                var re = _s.GetReference<HSDAccessor>(0x14);

                if (re == null)
                    return null;

                return re._s.GetData();
            }
            set
            {
                var re = _s.GetReference<HSDAccessor>(0x14);

                if (re == null)
                    re = _s.GetCreateReference<HSDAccessor>(0x14);

                re._s.SetData(value);
            }
        }

        public override bool Equals(object obj)
        {
            var attribute = obj as GX_Attribute;
            return attribute != null &&
                   AttributeType == attribute.AttributeType &&
                   CompCount == attribute.CompCount &&
                   CompType == attribute.CompType &&
                   Scale == attribute.Scale &&
                   Stride == attribute.Stride;
        }

        public override int GetHashCode()
        {
            var hashCode = -2053284073;
            hashCode = hashCode * -1521134295 + AttributeType.GetHashCode();
            hashCode = hashCode * -1521134295 + CompCount.GetHashCode();
            hashCode = hashCode * -1521134295 + CompType.GetHashCode();
            hashCode = hashCode * -1521134295 + Scale.GetHashCode();
            hashCode = hashCode * -1521134295 + Stride.GetHashCode();
            return hashCode;
        }
    }
}
