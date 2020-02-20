using HSDRaw.Common;

namespace HSDRaw.Melee.Gr
{
    public class SBM_Map_Head : HSDAccessor
    {
        public override int TrimmedSize => 0x30;

        public HSDArrayAccessor<SBM_GeneralPoints> GeneralPoints
        {
            get => _s.GetReference<HSDArrayAccessor<SBM_GeneralPoints>>(0x00);
            set
            {
                _s.SetInt32(0x04, value.Length);
                _s.SetReference(0x00, value);
            }
        }

        public HSDArrayAccessor<Map_GOBJ> ModelGroups
        {
            get => _s.GetReference<HSDArrayAccessor<Map_GOBJ>>(0x08);
            set
            {
                _s.SetInt32(0x0C, value.Length);
                _s.SetReference(0x08, value);
            }
        }

        public HSDFixedLengthPointerArrayAccessor<HSD_Spline> Splines
        {
            get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSD_Spline>>(0x10);
            set
            {
                _s.SetInt32(0x14, value.Length);
                _s.SetReference(0x10, value);
            }
        }

        public HSDArrayAccessor<SBM_MapLight> Lights
        {
            get => _s.GetReference<HSDArrayAccessor<SBM_MapLight>>(0x18);
            set
            {
                _s.SetInt32(0x1C, value.Length);
                _s.SetReference(0x18, value);
            }
        }

        // Section 5
        
        public HSDFixedLengthPointerArrayAccessor<HSD_MOBJ> MOBJs
        {
            get
            {
                return _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSD_MOBJ>>(0x28);
            }
            set
            {
                _s.SetInt32(0x2C, value.Length);
                _s.SetReference(0x28, value);
            }
        }
    }
    
    public class SBM_MapLight : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public HSD_LOBJ LightObject { get => _s.GetReference<HSD_LOBJ>(0x00); set => _s.SetReference(0x00, value); }

        public int Flags { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }
    }

}
