using HSDRaw.Common;
using HSDRaw.Common.Animation;

namespace HSDRaw.Melee.Mn
{
    public class SBM_MnSelectStageDataTable : HSDAccessor
    {
        public override int TrimmedSize => 0xD0;

        //TODO: 2 unknowns structs and 2 lighting stucts

        public HSD_JOBJ IconModel { get => _s.GetReference<HSD_JOBJ>(0x10); set => _s.SetReference(0x10, value); }

        public HSD_AnimJoint IconAnimation { get => _s.GetReference<HSD_AnimJoint>(0x14); set => _s.SetReference(0x14, value); }

        public HSD_MatAnimJoint IconMaterialAnimation { get => _s.GetReference<HSD_MatAnimJoint>(0x18); set => _s.SetReference(0x18, value); }

        public HSD_JOBJ RandomIconModel { get => _s.GetReference<HSD_JOBJ>(0x20); set => _s.SetReference(0x20, value); }

        public HSD_AnimJoint RandomIconAnimation { get => _s.GetReference<HSD_AnimJoint>(0x24); set => _s.SetReference(0x24, value); }

        public HSD_MatAnimJoint RandomIconMaterialAnimation { get => _s.GetReference<HSD_MatAnimJoint>(0x28); set => _s.SetReference(0x28, value); }

        public HSD_JOBJ Icon2Model { get => _s.GetReference<HSD_JOBJ>(0x30); set => _s.SetReference(0x30, value); }

        public HSD_AnimJoint Icon2Animation { get => _s.GetReference<HSD_AnimJoint>(0x34); set => _s.SetReference(0x34, value); }

        public HSD_MatAnimJoint Icon2MaterialAnimation { get => _s.GetReference<HSD_MatAnimJoint>(0x38); set => _s.SetReference(0x38, value); }

        public HSD_JOBJ StageSelectModel { get => _s.GetReference<HSD_JOBJ>(0x40); set => _s.SetReference(0x40, value); }

        public HSD_AnimJoint StageSelectAnimation { get => _s.GetReference<HSD_AnimJoint>(0x44); set => _s.SetReference(0x44, value); }

        public HSD_MatAnimJoint StageSelectMaterialAnimation { get => _s.GetReference<HSD_MatAnimJoint>(0x48); set => _s.SetReference(0x48, value); }

        public HSD_JOBJ Icon3Model { get => _s.GetReference<HSD_JOBJ>(0x50); set => _s.SetReference(0x50, value); }

        public HSD_AnimJoint Icon3Animation { get => _s.GetReference<HSD_AnimJoint>(0x54); set => _s.SetReference(0x54, value); }

        public HSD_MatAnimJoint Icon3MaterialAnimation { get => _s.GetReference<HSD_MatAnimJoint>(0x58); set => _s.SetReference(0x58, value); }

        public HSD_JOBJ StageSelect2Model { get => _s.GetReference<HSD_JOBJ>(0x60); set => _s.SetReference(0x60, value); }

        public HSD_AnimJoint StageSelect2Animation { get => _s.GetReference<HSD_AnimJoint>(0x64); set => _s.SetReference(0x64, value); }

        public HSD_MatAnimJoint StageSelect2MaterialAnimation { get => _s.GetReference<HSD_MatAnimJoint>(0x68); set => _s.SetReference(0x68, value); }

        public HSD_JOBJ StagePreviewModel { get => _s.GetReference<HSD_JOBJ>(0x70); set => _s.SetReference(0x70, value); }

        public HSD_AnimJoint StagePreviewAnimation { get => _s.GetReference<HSD_AnimJoint>(0x74); set => _s.SetReference(0x74, value); }

        public HSD_MatAnimJoint StagePreviewMaterialAnimation { get => _s.GetReference<HSD_MatAnimJoint>(0x78); set => _s.SetReference(0x78, value); }

        public HSD_JOBJ SelectBoxModel { get => _s.GetReference<HSD_JOBJ>(0x80); set => _s.SetReference(0x80, value); }

        public HSD_AnimJoint SelectBoxAnimation { get => _s.GetReference<HSD_AnimJoint>(0x84); set => _s.SetReference(0x84, value); }

        public HSD_MatAnimJoint SelectBoxMaterialAnimation { get => _s.GetReference<HSD_MatAnimJoint>(0x88); set => _s.SetReference(0x88, value); }

        public HSD_JOBJ SelectCursorModel { get => _s.GetReference<HSD_JOBJ>(0x90); set => _s.SetReference(0x90, value); }

        public HSD_AnimJoint SelectCursorAnimation { get => _s.GetReference<HSD_AnimJoint>(0x94); set => _s.SetReference(0x94, value); }

        public HSD_MatAnimJoint SelectCursorMaterialAnimation { get => _s.GetReference<HSD_MatAnimJoint>(0x98); set => _s.SetReference(0x98, value); }

        public HSD_JOBJ PositionModel { get => _s.GetReference<HSD_JOBJ>(0xA0); set => _s.SetReference(0xA0, value); }

        public HSD_AnimJoint PositionAnimation { get => _s.GetReference<HSD_AnimJoint>(0xA4); set => _s.SetReference(0xA4, value); }

        public HSD_MatAnimJoint PositionMaterialAnimation { get => _s.GetReference<HSD_MatAnimJoint>(0xA8); set => _s.SetReference(0xA8, value); }

        public HSD_JOBJ BackgroundModel { get => _s.GetReference<HSD_JOBJ>(0xB0); set => _s.SetReference(0xB0, value); }

        public HSD_AnimJoint BackgroundAnimation { get => _s.GetReference<HSD_AnimJoint>(0xB4); set => _s.SetReference(0xB4, value); }

        public HSD_MatAnimJoint BackgroundMaterialAnimation { get => _s.GetReference<HSD_MatAnimJoint>(0xB8); set => _s.SetReference(0xB8, value); }

        public HSD_JOBJ NowLoadingModel { get => _s.GetReference<HSD_JOBJ>(0xC0); set => _s.SetReference(0xC0, value); }

        public HSD_AnimJoint NowLoadingAnimation { get => _s.GetReference<HSD_AnimJoint>(0xC4); set => _s.SetReference(0xC4, value); }

        public HSD_MatAnimJoint NowLoadingMaterialAnimation { get => _s.GetReference<HSD_MatAnimJoint>(0xC8); set => _s.SetReference(0xC8, value); }
    }
}
