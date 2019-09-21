namespace HSDRaw.AirRide.Vc
{
    public class KAR_vcDataStar : HSDAccessor
    {
        public override int TrimmedSize => 0x1C;

        public KAR_vcAttributes VehicleAttributes { get => _s.GetReference<KAR_vcAttributes>(0x0); set => _s.SetReference(0x0, value); }
        public KAR_vcModelData ModelData { get => _s.GetReference<KAR_vcModelData>(0x4); set => _s.SetReference(0x4, value); }
        //TODO: Unknown structure at 0x08
        public KAR_vcCollisionAttributes CollisionAttributes { get => _s.GetReference<KAR_vcCollisionAttributes>(0xc); set => _s.SetReference(0xc, value); }
        public KAR_vcCollisionSphere CollisionSphere { get => _s.GetReference<KAR_vcCollisionSphere>(0x10); set => _s.SetReference(0x10, value); }
        public KAR_vcHandlingAttributes HandlingAttributes { get => _s.GetReference<KAR_vcHandlingAttributes>(0x14); set => _s.SetReference(0x14, value); }
        public KAR_vcAnimationStar AnimationBank { get => _s.GetReference<KAR_vcAnimationStar>(0x18); set => _s.SetReference(0x18, value); }
    }
}
