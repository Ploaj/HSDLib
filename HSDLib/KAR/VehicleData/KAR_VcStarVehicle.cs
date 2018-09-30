
namespace HSDLib.KAR
{
    public class KAR_VcStarVehicle : IHSDNode
    {
        [FieldData(typeof(KAR_VcAttributes))]
        public KAR_VcAttributes VehicleAttributes { get; set; }

        [FieldData(typeof(KAR_VcModelData))]
        public KAR_VcModelData ModelData { get; set; }

        [FieldData(typeof(KAR_VcUnk))]
        public KAR_VcUnk Unk1 { get; set; }

        [FieldData(typeof(KAR_VcCollisionAttributes))]
        public KAR_VcCollisionAttributes CollisionAttributes { get; set; }

        [FieldData(typeof(KAR_VcCollisionSphere))]
        public KAR_VcCollisionSphere CollisionSphere { get; set; }

        [FieldData(typeof(KAR_VcHandlingAttributes))]
        public KAR_VcHandlingAttributes HandlingAttributes { get; set; }

        [FieldData(typeof(KAR_VcStarAnimation))]
        public KAR_VcStarAnimation AnimationBank { get; set; }
    }

    public class KAR_WheelVehicle : IHSDNode
    {
        [FieldData(typeof(KAR_VcAttributes))]
        public KAR_VcAttributes VehicleAttributes { get; set; }

        [FieldData(typeof(KAR_VcModelData))]
        public KAR_VcModelData ModelData { get; set; }

        [FieldData(typeof(KAR_VcUnk))]
        public KAR_VcUnk Unk1 { get; set; }

        [FieldData(typeof(KAR_VcCollisionAttributes))]
        public KAR_VcCollisionAttributes CollisionAttributes { get; set; }

        [FieldData(typeof(KAR_VcCollisionSphere))]
        public KAR_VcCollisionSphere CollisionSphere { get; set; }

        [FieldData(typeof(KAR_VcHandlingAttributes))]
        public KAR_VcHandlingAttributes HandlingAttributes { get; set; }
        
        [FieldData(typeof(KAR_VcWheelAnimation))]
        public KAR_VcWheelAnimation AnimationBank { get; set; }
    }
}
