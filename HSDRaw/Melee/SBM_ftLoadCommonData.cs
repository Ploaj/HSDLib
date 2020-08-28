using HSDRaw.Common;
using HSDRaw.Common.Animation;
using System.Drawing;
using HSDRaw.Melee.Cmd;

namespace HSDRaw.Melee
{
    public class ftLoadCommonData : HSDAccessor
    {
        public ftLoadCommandDataCommonAttributes CommonAttributes { get => _s.GetReference<ftLoadCommandDataCommonAttributes>(0x00); set => _s.SetReference(0x00, value); }


        public HSDFixedLengthPointerArrayAccessor<SBM_BoneLookupTable> BoneTables { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<SBM_BoneLookupTable>>(0x10); set => _s.SetReference(0x10, value); }

        public HSDFixedLengthPointerArrayAccessor<SBM_PlCoUnknownFighterTable> FighterTable { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<SBM_PlCoUnknownFighterTable>>(0x14); set => _s.SetReference(0x14, value); }

        public HSDFixedLengthPointerArrayAccessor<SBM_ColorSubactionData> ColorSATable { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<SBM_ColorSubactionData>>(0x18); set => _s.SetReference(0x18, value); }


        public SBM_RespawnPlatform RespawnPlatform { get => _s.GetReference<SBM_RespawnPlatform>(0x20); set => _s.SetReference(0x20, value); }


        public HSD_JOBJ TrophyStandModel { get => _s.GetReference<HSD_JOBJ>(0x40); set => _s.SetReference(0x40, value); }

        public ftLoadCommonDataColors PlayerColors { get => _s.GetReference<ftLoadCommonDataColors>(0x44); set => _s.SetReference(0x44, value); }

        public ftLoadCommonDataColors ShieldColors { get => _s.GetReference<ftLoadCommonDataColors>(0x48); set => _s.SetReference(0x48, value); }

        public ftLoadCommonDataColors DeathFlashColors { get => _s.GetReference<ftLoadCommonDataColors>(0x4C); set => _s.SetReference(0x4C, value); }

        public HSD_JOBJ CubeModel { get => _s.GetReference<HSD_JOBJ>(0x50); set => _s.SetReference(0x50, value); }
    }


    public class ftLoadCommonDataColors : HSDAccessor
    {
        public override int TrimmedSize => 0x14;

        public Color Player1 { get => _s.GetColorRGB(0x00); set => _s.SetColorRGB(0x00, value); }

        public Color Player2 { get => _s.GetColorRGB(0x04); set => _s.SetColorRGB(0x04, value); }

        public Color Player3 { get => _s.GetColorRGB(0x08); set => _s.SetColorRGB(0x08, value); }

        public Color Player4 { get => _s.GetColorRGB(0x0C); set => _s.SetColorRGB(0x0C, value); }

        public Color CPU { get => _s.GetColorRGB(0x10); set => _s.SetColorRGB(0x10, value); }
    }

    public class SBM_RespawnPlatform : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public HSD_JOBJ Model { get => _s.GetReference<HSD_JOBJ>(0x00); set => _s.SetReference(0x00, value); }

        public HSD_AnimJoint Animation { get => _s.GetReference<HSD_AnimJoint>(0x04); set => _s.SetReference(0x04, value); }
    }

    public class ftLoadCommandDataCommonAttributes : HSDAccessor
    {

    }
}
