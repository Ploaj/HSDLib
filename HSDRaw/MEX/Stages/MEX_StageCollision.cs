namespace HSDRaw.MEX.Stages
{
    public class MEX_StageCollision : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public int InternalID { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public int CollisionFunction { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }
        
    }
    
}
