using System.ComponentModel;

namespace HSDRaw.Melee.Pl
{
    public class SBM_HurtboxBank<T> : HSDAccessor where T : HSDAccessor
    {
        public T[] Hurtboxes
        {
            get
            {
                var re = _s.GetReference<HSDArrayAccessor<T>>(0x04);
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
                    var re = _s.GetCreateReference<HSDArrayAccessor<T>>(0x04);
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

        [DisplayName("Bone Index"), Description("Index of Bone to attach hurtbox to"), Category("Parameters")]
        public int BoneIndex { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        [DisplayName("Hurtbox Type"), Description("Indicates position of the body this hurtbox corresponds to"), Category("Parameters")]
        public HurtboxPositionType Type { get => (HurtboxPositionType)_s.GetInt32(0x04); set => _s.SetInt32(0x04, (int)value); }
        
        [DisplayName("Grabbable"), Description("Indicates if the hurtbox is grabbable"), Category("Parameters")]
        public int Grabbable { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        [Category("Position")]
        public float X1 { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        [Category("Position")]
        public float Y1 { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }

        [Category("Position")]
        public float Z1 { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }

        [Category("Position")]
        public float X2 { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }

        [Category("Position")]
        public float Y2 { get => _s.GetFloat(0x1C); set => _s.SetFloat(0x1C, value); }

        [Category("Position")]
        public float Z2 { get => _s.GetFloat(0x20); set => _s.SetFloat(0x20, value); }
        
        [Category("Parameters")]
        public float Size { get => _s.GetFloat(0x24); set => _s.SetFloat(0x24, value); }

        public override string ToString()
        {
            return $"JOBJ_{BoneIndex} R: {Size} ({X1}, {Y1}, {Z1}) ({X2}, {Y2}, {Z2}) Grabbable: {Grabbable}";
        }
    }

    public class SBM_ItemHurtbox : HSDAccessor
    {
        public override int TrimmedSize => 0x20;

        [DisplayName("Bone Index"), Description("Index of Bone to attach hurtbox to"), Category("Parameters")]
        public int BoneIndex { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }
        
        [Category("Position")]
        public float X1 { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        [Category("Position")]
        public float Y1 { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        [Category("Position")]
        public float Z1 { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        [Category("Position")]
        public float X2 { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }

        [Category("Position")]
        public float Y2 { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }

        [Category("Position")]
        public float Z2 { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }

        [Category("Parameters")]
        public float Size { get => _s.GetFloat(0x1C); set => _s.SetFloat(0x1C, value); }

        public override string ToString()
        {
            return $"JOBJ_{BoneIndex} R: {Size} ({X1}, {Y1}, {Z1}) ({X2}, {Y2}, {Z2})";
        }
    }
}
