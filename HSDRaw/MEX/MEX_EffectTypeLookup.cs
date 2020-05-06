namespace HSDRaw.MEX
{
    public enum MEXEffectType
    {
        Effect_Particle,
        Effect_DefinePosRot,
        Effect_UseJointPos,
        Effect_UseJointPosRot,
        Effect_UseJointPos_GroundOrientation,
        Effect_FollowJointPos,
        Effect_FollowJointPosRot,
        Effect_FollowJointPos_GroundOrientation,
    }

    public class MEX_EffectTypeLookup : HSDAccessor
    {
        public override int TrimmedSize => 0x8;

        public int Count { get => _s.GetInt32(0x00); internal set => _s.SetInt32(0x00, value); }

        public MEXEffectType[] Entries
        {
            get
            {
                var ar = _s.GetReference<HSDAccessor>(0x04);

                if (ar == null)
                    return new MEXEffectType[0];

                var arr = ar._s.GetData();

                MEXEffectType[] types = new MEXEffectType[Count];
                for (int i = 0; i < Count; i++)
                    types[i] = (MEXEffectType)arr[i];
                return types;
            }
            set
            {
                if(value == null || value.Length == 0)
                {
                    Count = 0;
                    _s.SetReference(0x04, null);
                }
                else
                {
                    Count = value.Length;
                    byte[] data = new byte[Count];
                    for (int i = 0; i < value.Length; i++)
                        data[i] = (byte)value[i];
                    _s.GetCreateReference<HSDAccessor>(0x04)._s.SetData(data);
                }
            }
        }
    }
}
