using HSDRaw.Common.Animation;

namespace HSDRaw.Melee.Pl
{
    public class SBM_ModelPartTable : HSDAccessor
    {
        public override int TrimmedSize => 0x0C;

        public SBM_ModelPart LHand { get => _s.GetReference<SBM_ModelPart>(0x00); set => _s.SetReference(0x00, value); }

        public SBM_ModelPart RHand { get => _s.GetReference<SBM_ModelPart>(0x04); set => _s.SetReference(0x04, value); }

        public SBM_ModelPart Head { get => _s.GetReference<SBM_ModelPart>(0x08); set => _s.SetReference(0x08, value); }
    }

    public class SBM_ModelPart : HSDAccessor
    {
        public override int TrimmedSize => 0x0C;

        public short Unknown { get => _s.GetInt16(0x00); set => _s.SetInt16(0x00, value); }

        public short Count { get => _s.GetInt16(0x02); set => _s.SetInt16(0x02, value); }

        public byte[] Entries
        {
            get
            {
                var a = _s.GetReference<HSDAccessor>(0x04);
                if (a == null)
                    return null;
                return a._s.GetBytes(0x00, Count);
            }
            set
            {
                if (value == null)
                {
                    Count = 0;
                    _s.SetReference(0x04, null);
                }
                else
                {
                    Count = (short)value.Length;
                    var a = _s.GetCreateReference<HSDAccessor>(0x04);
                    var size = (int)Count;
                    if (size % 4 != 0)
                        size += 4 - (size % 4);
                    a._s.SetData(value);
                    a._s.Resize(size);
                }
            }
        }

        public HSDFixedLengthPointerArrayAccessor<HSD_AnimJoint> Anims { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSD_AnimJoint>>(0x08); set => _s.SetReference(0x08, value); }
    }
}
