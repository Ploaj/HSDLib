namespace HSDRaw.MEX
{
    public enum MEXEffectType
    {
        PARTICLE,
        SPAWN_POSITION,
        JOINT_POSITION,
        JOINT_POSITION_AND_ROTATION,
    }

    public class MEX_Effect : HSDAccessor
    {
        public override int TrimmedSize => 0x08;
        
        public int ExternalID { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public MEXEffectType Type { get => (MEXEffectType)(_s.GetInt32(0x04)); set => _s.SetInt32(0x04, (int)value); }

        public override string ToString()
        {
            return $"{ExternalID} {Type}";
        }
    }
}
