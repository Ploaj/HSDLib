namespace HSDRaw.MEX.Stages
{
    public class MEX_StageCollision : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public int InternalID { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        //public HSDFixedLengthPointerArrayAccessor<MEX_StageCollisionData> Data { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<MEX_StageCollisionData>>(0x04); set => _s.SetReference(0x04, value); }

    }
    
    public class MEX_StageCollisionData : HSDAccessor
    {
        public override int TrimmedSize => 0x58;



    }
}
