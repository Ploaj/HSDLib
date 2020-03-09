namespace HSDRaw.MEX
{
    public enum MEXEffectType
    {
        Effect_Particle,
        Effect_UseJointPos,
        Effect_UseJointPos_GroundOrientation,
        Effect_FollowJointPos,
        Effect_FollowJointPosRot,
        Effect_FollowJointPos_GroundOrientation,
    }

    public class MEX_FighterEffect : HSDAccessor
    {
        public override int TrimmedSize => 0x8;

        public int Count { get => _s.GetInt32(0x00); internal set => _s.SetInt32(0x00, value); }

        public MEXEffectType[] Entries
        {
            get
            {
                var arr = _s.GetCreateReference<HSDAccessor>(0x04)._s.GetData();
                MEXEffectType[] types = new MEXEffectType[Count];
                for (int i = 0; i < Count; i++)
                    types[i] = (MEXEffectType)arr[i];
                return types;
            }
            set
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
