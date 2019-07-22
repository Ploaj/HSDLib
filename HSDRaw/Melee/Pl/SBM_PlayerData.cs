using HSDRaw.Common;

namespace HSDRaw.Melee.Pl
{
    public class SBM_PlayerData : HSDAccessor
    {
        public override int TrimmedSize => 0x60;

        public SBM_CommonFighterAttributes Attributes { get => _s.GetReference<SBM_CommonFighterAttributes>(0x00); set => _s.SetReference(0x00, value); }

        public HSDAccessor Attributes2 { get => _s.GetReference<HSDAccessor>(0x04); set => _s.SetReference(0x04, value); }
        
        public HSDAccessor UnknownNode { get => _s.GetReference<HSDAccessor>(0x08); set => _s.SetReference(0x08, value); }

        public HSDAccessor SubActionTable { get => _s.GetReference<HSDAccessor>(0x0C); set => _s.SetReference(0x0C, value); }

        public HSDAccessor Unknown0x10 { get => _s.GetReference<HSDAccessor>(0x10); set => _s.SetReference(0x10, value); }

        public HSDAccessor WinSubAction { get => _s.GetReference<HSDAccessor>(0x14); set => _s.SetReference(0x14, value); }

        public HSDAccessor Unknown0x18 { get => _s.GetReference<HSDAccessor>(0x18); set => _s.SetReference(0x18, value); }

        public HSDAccessor Unknown0x1C { get => _s.GetReference<HSDAccessor>(0x1C); set => _s.SetReference(0x1C, value); }

        public HSD_JOBJ ShieldPoseJOBJ  { get => _s.GetReference<HSDAccessor>(0x20)._s.GetReference<HSD_JOBJ>(0x00);
            set
            {
                var re = _s.GetCreateReference<HSDAccessor>(0x20);
                re._s.Resize(4);
                re._s.SetReference(0x00, value);
            }
        }

        public HSDAccessor Unknown0x24 { get => _s.GetReference<HSDAccessor>(0x24); set => _s.SetReference(0x24, value); }

        public HSDAccessor Unknown0x28 { get => _s.GetReference<HSDAccessor>(0x28); set => _s.SetReference(0x28, value); }

        public HSDAccessor Unknown0x2C { get => _s.GetReference<HSDAccessor>(0x2C); set => _s.SetReference(0x2C, value); }

        public HSDAccessor Unknown0x30 { get => _s.GetReference<HSDAccessor>(0x30); set => _s.SetReference(0x30, value); }

        public HSDAccessor Unknown0x34 { get => _s.GetReference<HSDAccessor>(0x34); set => _s.SetReference(0x34, value); }

        public HSDAccessor Unknown0x38 { get => _s.GetReference<HSDAccessor>(0x38); set => _s.SetReference(0x38, value); }

        public HSDAccessor Unknown0x3C { get => _s.GetReference<HSDAccessor>(0x3C); set => _s.SetReference(0x3C, value); }

        public HSDAccessor Unknown0x40 { get => _s.GetReference<HSDAccessor>(0x40); set => _s.SetReference(0x40, value); }

        public HSDAccessor Unknown0x44 { get => _s.GetReference<HSDAccessor>(0x44); set => _s.SetReference(0x44, value); }

        public HSDAccessor Articles { get => _s.GetReference<HSDAccessor>(0x48); set => _s.SetReference(0x48, value); }

        public HSDAccessor Unknown0x4C { get => _s.GetReference<HSDAccessor>(0x4C); set => _s.SetReference(0x4C, value); }

        public HSDAccessor Unknown0x50 { get => _s.GetReference<HSDAccessor>(0x50); set => _s.SetReference(0x50, value); }

        public HSDAccessor Unknown0x54 { get => _s.GetReference<HSDAccessor>(0x54); set => _s.SetReference(0x54, value); }

        public HSDAccessor Unknown0x58 { get => _s.GetReference<HSDAccessor>(0x58); set => _s.SetReference(0x58, value); }

        public HSD_JOBJ ShadowModel { get => _s.GetReference<HSD_JOBJ>(0x5C); set => _s.SetReference(0x5C, value); }

    }
}
