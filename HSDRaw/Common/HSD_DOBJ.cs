namespace HSDRaw.Common
{
    public class HSD_DOBJ : HSDListAccessor<HSD_DOBJ>
    {
        public override int TrimmedSize { get; } = 0x10;

        //public uint NameOffset { get; set; }

        public override HSD_DOBJ Next { get => _s.GetReference<HSD_DOBJ>(0x04); set => _s.SetReference(0x04, value); }

        public HSD_MOBJ Mobj { get => _s.GetReference<HSD_MOBJ>(0x08); set => _s.SetReference(0x08, value); }

        public HSD_POBJ Pobj { get => _s.GetReference<HSD_POBJ>(0x0C); set => _s.SetReference(0x0C, value); }
    }
}
