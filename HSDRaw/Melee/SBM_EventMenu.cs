using HSDRaw.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace HSDRaw.Melee
{
    public class SBM_EventMenu : HSDAccessor
    {
        public HSD_JOBJ JOBJ_1 { get => _s.GetReference<HSD_JOBJ>(0x00); set => _s.SetReference(0x00, value); }
        public HSD_JOBJ JOBJ_2 { get => _s.GetReference<HSD_JOBJ>(0x04); set => _s.SetReference(0x04, value); }
        public HSD_JOBJ JOBJ_3 { get => _s.GetReference<HSD_JOBJ>(0x08); set => _s.SetReference(0x08, value); }
        public HSD_JOBJ JOBJ_4 { get => _s.GetReference<HSD_JOBJ>(0x0C); set => _s.SetReference(0x0C, value); }
        public HSD_JOBJ JOBJ_5 { get => _s.GetReference<HSD_JOBJ>(0x10); set => _s.SetReference(0x10, value); }
    }
}
