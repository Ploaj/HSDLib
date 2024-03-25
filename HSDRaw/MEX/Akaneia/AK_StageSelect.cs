using HSDRaw.Common;
using HSDRaw.Common.Animation;
using System;

namespace HSDRaw.MEX.Akaneia
{
    public class AK_StagePages : HSDNullPointerArrayAccessor<AK_StagePage>
    {

    }

    public class AK_StagePage : HSDAccessor
    {
        public override int TrimmedSize => 0x20;

        public HSD_JOBJ PositionJoint { get => _s.GetReference<HSD_JOBJ>(0x00); set => _s.SetReference(0x00, value); }

        public HSD_AnimJoint PositionAnimEnter { get => _s.GetReference<HSD_AnimJoint>(0x04); set => _s.SetReference(0x04, value); }

        public HSD_MatAnimJoint IconTextures { get => _s.GetReference<HSD_MatAnimJoint>(0x08); set => _s.SetReference(0x08, value); }

        public HSD_MatAnimJoint NameTextures { get => _s.GetReference<HSD_MatAnimJoint>(0x0C); set => _s.SetReference(0x0C, value); }

        public int Count { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        public HSDArrayAccessor<AK_StageIcon> Icons { get => _s.GetReference<HSDArrayAccessor<AK_StageIcon>>(0x14); set => _s.SetReference(0x14, value); }

        // 0x18 GOBJ

    }

    [Flags]
    public enum StageIconFlags
    {
        Locked = 1,
        RandomEnabled,
    }

    public enum AkStageType
    {
        Normal,
        TargetTest,
    }

    public class AK_StageIcon : HSDAccessor
    {
        public override int TrimmedSize => 0x20;

        public short JointIndex { get => _s.GetInt16(0x00); set => _s.SetInt16(0x00, value); }

        public short ExternalID { get => _s.GetInt16(0x02); set => _s.SetInt16(0x02, value); }

        public AkStageType StageType { get => (AkStageType)_s.GetByte(0x04); set => _s.SetByte(0x04, (byte)value); }

        public StageIconFlags Flags { get => (StageIconFlags)_s.GetInt24(0x05); set => _s.SetInt24(0x05, (int)value); }

        public float Width { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public float Height { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        public float ScaleX { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }

        public float ScaleY { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }

        public ushort IconIndex { get => _s.GetUInt16(0x18); set => _s.SetUInt16(0x18, value); }

        public ushort NameIndex { get => _s.GetUInt16(0x1A); set => _s.SetUInt16(0x1A, value); }

        public ushort EmblemIndex { get => _s.GetUInt16(0x1C); set => _s.SetUInt16(0x1C, value); }

        public short PreviewIndex { get => _s.GetInt16(0x1E); set => _s.SetInt16(0x1E, value); }
    }
}
