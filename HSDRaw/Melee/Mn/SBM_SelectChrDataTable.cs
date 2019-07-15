using HSDRaw.Common;
using HSDRaw.Common.Animation;

namespace HSDRaw.Melee.Mn
{
    /// <summary>
    /// 
    /// </summary>
    public class SBM_SelectChrDataTable : HSDAccessor
    {
        public override int TrimmedSize => 0x100;

        //TODO: 2 unknowns structs and 2 lighting stucts

        public HSD_JOBJ BackgroundModel { get => _s.GetReference<HSD_JOBJ>(0x10); set => _s.SetReference(0x10, value); }

        public HSD_AnimJoint BackgroundAnimation { get => _s.GetReference<HSD_AnimJoint>(0x14); set => _s.SetReference(0x14, value); }

        public HSD_MatAnimJoint BackgroundMaterialAnimation { get => _s.GetReference<HSD_MatAnimJoint>(0x18); set => _s.SetReference(0x18, value); }

        public HSD_JOBJ HandModel { get => _s.GetReference<HSD_JOBJ>(0x20); set => _s.SetReference(0x20, value); }

        public HSD_AnimJoint HandAnimation { get => _s.GetReference<HSD_AnimJoint>(0x24); set => _s.SetReference(0x24, value); }

        public HSD_MatAnimJoint HandMaterialAnimation { get => _s.GetReference<HSD_MatAnimJoint>(0x28); set => _s.SetReference(0x28, value); }

        public HSD_JOBJ TokenModel { get => _s.GetReference<HSD_JOBJ>(0x30); set => _s.SetReference(0x30, value); }

        public HSD_AnimJoint TokenAnimation { get => _s.GetReference<HSD_AnimJoint>(0x34); set => _s.SetReference(0x34, value); }

        public HSD_MatAnimJoint TokenMaterialAnimation { get => _s.GetReference<HSD_MatAnimJoint>(0x38); set => _s.SetReference(0x38, value); }

        public HSD_JOBJ MenuModel { get => _s.GetReference<HSD_JOBJ>(0x40); set => _s.SetReference(0x40, value); }

        public HSD_AnimJoint MenuAnimation { get => _s.GetReference<HSD_AnimJoint>(0x44); set => _s.SetReference(0x44, value); }

        public HSD_MatAnimJoint MenuMaterialAnimation { get => _s.GetReference<HSD_MatAnimJoint>(0x48); set => _s.SetReference(0x48, value); }

        public HSD_JOBJ PressStartModel { get => _s.GetReference<HSD_JOBJ>(0x50); set => _s.SetReference(0x50, value); }

        public HSD_AnimJoint PressStartAnimation { get => _s.GetReference<HSD_AnimJoint>(0x54); set => _s.SetReference(0x54, value); }

        public HSD_MatAnimJoint PressStartMaterialAnimation { get => _s.GetReference<HSD_MatAnimJoint>(0x58); set => _s.SetReference(0x58, value); }

        public HSD_JOBJ DebugCameraModel { get => _s.GetReference<HSD_JOBJ>(0x60); set => _s.SetReference(0x60, value); }

        public HSD_AnimJoint DebugCameraAnimation { get => _s.GetReference<HSD_AnimJoint>(0x64); set => _s.SetReference(0x64, value); }

        public HSD_MatAnimJoint DebugCameraMaterialAnimation { get => _s.GetReference<HSD_MatAnimJoint>(0x68); set => _s.SetReference(0x68, value); }

        public HSD_JOBJ SingleMenuModel { get => _s.GetReference<HSD_JOBJ>(0x70); set => _s.SetReference(0x70, value); }

        public HSD_AnimJoint SingleMenuAnimation { get => _s.GetReference<HSD_AnimJoint>(0x74); set => _s.SetReference(0x74, value); }

        public HSD_MatAnimJoint SingleMenuMaterialAnimation { get => _s.GetReference<HSD_MatAnimJoint>(0x78); set => _s.SetReference(0x78, value); }

        public HSD_JOBJ SingleOptionsModel { get => _s.GetReference<HSD_JOBJ>(0x80); set => _s.SetReference(0x80, value); }

        public HSD_AnimJoint SingleOptionsAnimation { get => _s.GetReference<HSD_AnimJoint>(0x84); set => _s.SetReference(0x84, value); }

        public HSD_MatAnimJoint SingleOptionsMaterialAnimation { get => _s.GetReference<HSD_MatAnimJoint>(0x88); set => _s.SetReference(0x88, value); }

        public HSD_JOBJ PortraitModel { get => _s.GetReference<HSD_JOBJ>(0x90); set => _s.SetReference(0x90, value); }

        public HSD_AnimJoint PortraitAnimation { get => _s.GetReference<HSD_AnimJoint>(0x94); set => _s.SetReference(0x94, value); }

        public HSD_MatAnimJoint PortraitMaterialAnimation { get => _s.GetReference<HSD_MatAnimJoint>(0x98); set => _s.SetReference(0x98, value); }


    }
}
