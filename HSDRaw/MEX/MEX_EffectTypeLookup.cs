namespace HSDRaw.MEX
{
    public enum MEXEffectType
    {
        DefinePosRot,
        UseJointPos,
        UseJointPos_GroundOrientation,
        UseJointPosRot,
        UseJointPosFtDir,
        FollowJointPos,
        FollowJointPosRot,
    }

    public enum MEXParticleType
    {
        UseJointPos,
        UseJointPosRot,
        UseJointPosRot_Ground,
        UseJointPosFtDir,
        UseJointPosFtDir_Ground,
        FollowJointPos,
        FollowJointPos_FtDir,
    }

    public class MEX_EffectTypeLookup : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public int ModelEffectCount { get => _s.GetInt32(0x00); internal set => _s.SetInt32(0x00, value); }

        public MEXEffectType[] ModelEffectEntries
        {
            get
            {
                var ar = _s.GetReference<HSDAccessor>(0x04);

                if (ar == null)
                    return new MEXEffectType[0];

                var arr = ar._s.GetData();

                MEXEffectType[] types = new MEXEffectType[ModelEffectCount];
                for (int i = 0; i < ModelEffectCount; i++)
                    types[i] = (MEXEffectType)arr[i];
                return types;
            }
            set
            {
                if(value == null || value.Length == 0)
                {
                    ModelEffectCount = 0;
                    _s.SetReference(0x04, null);
                }
                else
                {
                    ModelEffectCount = value.Length;
                    byte[] data = new byte[ModelEffectCount];
                    for (int i = 0; i < value.Length; i++)
                        data[i] = (byte)value[i];
                    _s.GetCreateReference<HSDAccessor>(0x04)._s.SetData(data);
                }
            }
        }

        public int PtclGenCount { get => _s.GetInt32(0x08); internal set => _s.SetInt32(0x08, value); }

        public MEXParticleType[] PtclGenEntries
        {
            get
            {
                var ar = _s.GetReference<HSDAccessor>(0x0C);

                if (ar == null)
                    return new MEXParticleType[0];

                var arr = ar._s.GetData();

                MEXParticleType[] types = new MEXParticleType[PtclGenCount];
                for (int i = 0; i < PtclGenCount; i++)
                    types[i] = (MEXParticleType)arr[i];
                return types;
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    PtclGenCount = 0;
                    _s.SetReference(0x0C, null);
                }
                else
                {
                    PtclGenCount = value.Length;
                    byte[] data = new byte[PtclGenCount];
                    for (int i = 0; i < value.Length; i++)
                        data[i] = (byte)value[i];
                    _s.GetCreateReference<HSDAccessor>(0x0C)._s.SetData(data);
                }
            }
        }
    }
}
