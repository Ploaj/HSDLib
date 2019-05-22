namespace HSDLib.KAR
{
    public class KAR_VcStarVehicle : IHSDNode
    {
        public KAR_VcAttributes VehicleAttributes { get; set; }
        
        public KAR_VcModelData ModelData { get; set; }
        
        public KAR_VcUnk Unk1 { get; set; }
        
        public KAR_VcCollisionAttributes CollisionAttributes { get; set; }
        
        public KAR_VcCollisionSphere CollisionSphere { get; set; }
        
        public KAR_VcHandlingAttributes HandlingAttributes { get; set; }
        
        public KAR_VcStarAnimation AnimationBank { get; set; }
    }

    public class KAR_WheelVehicle : IHSDNode
    {
        public KAR_VcAttributes VehicleAttributes { get; set; }
        
        public KAR_VcModelData ModelData { get; set; }
        
        public KAR_VcUnk Unk1 { get; set; }
        
        public KAR_VcCollisionAttributes CollisionAttributes { get; set; }
        
        public KAR_VcCollisionSphere CollisionSphere { get; set; }
        
        public KAR_VcHandlingAttributes HandlingAttributes { get; set; }
        
        public KAR_VcWheelAnimation AnimationBank { get; set; }
    }
}
