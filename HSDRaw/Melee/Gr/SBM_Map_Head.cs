using HSDRaw.Common;

namespace HSDRaw.Melee.Gr
{
    public class SBM_Map_Head : HSDAccessor
    {
        public override int TrimmedSize => 0x30;

        public SBM_GeneralPoints[] GeneralPoints
        {
            get => _s.GetArray<SBM_GeneralPoints>(0x00);
            set => _s.SetArray(0x00, 0x04, value);
        }

        public Map_GOBJ[] ModelGroups
        {
            get => _s.GetArray<Map_GOBJ>(0x08);
            set => _s.SetArray(0x08, 0x0C, value);
        }

        public HSD_Spline[] Splines
        {
            get => _s.GetArray<HSD_Spline>(0x10);
            set => _s.SetArray(0x10, 0x14, value);
        }

        public SBM_MapLight[] Lights
        {
            get => _s.GetArray<SBM_MapLight>(0x18);
            set => _s.SetArray(0x18, 0x1C, value);
        }

        // Section 5

        // Section 6
    }

    public class SBM_MapLight : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public HSD_LOBJ LightObject { get => _s.GetReference<HSD_LOBJ>(0x00); set => _s.SetReference(0x00, value); }

        public int Flags { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }
    }

}
