
namespace HSDLib.KAR
{
    public class KAR_StarVehicle : IHSDNode
    {
        [FieldData(typeof(KAR_VehicleAttributes))]
        public KAR_VehicleAttributes VehicleAttributes { get; set; }

        [FieldData(typeof(KAR_ModelData))]
        public KAR_ModelData ModelData { get; set; }

        [FieldData(typeof(KAR_VehicleUnk))]
        public KAR_VehicleUnk Unk1 { get; set; }

        [FieldData(typeof(KAR_VehicleCollisionAttributes))]
        public KAR_VehicleCollisionAttributes CollisionAttributes { get; set; }

        [FieldData(typeof(CollisionSphere))]
        public CollisionSphere CollisionSphere { get; set; }

        [FieldData(typeof(KAR_HandlingAttributes))]
        public KAR_HandlingAttributes HandlingAttributes { get; set; }

        [FieldData(typeof(KAR_StarAnimation))]
        public KAR_StarAnimation AnimationBank { get; set; }
    }

    public class KAR_WheelVehicle : IHSDNode
    {
        [FieldData(typeof(KAR_VehicleAttributes))]
        public KAR_VehicleAttributes VehicleAttributes { get; set; }

        [FieldData(typeof(KAR_ModelData))]
        public KAR_ModelData ModelData { get; set; }

        [FieldData(typeof(KAR_VehicleUnk))]
        public KAR_VehicleUnk Unk1 { get; set; }

        [FieldData(typeof(KAR_VehicleCollisionAttributes))]
        public KAR_VehicleCollisionAttributes CollisionAttributes { get; set; }

        [FieldData(typeof(CollisionSphere))]
        public CollisionSphere CollisionSphere { get; set; }

        [FieldData(typeof(KAR_HandlingAttributes))]
        public KAR_HandlingAttributes HandlingAttributes { get; set; }
        
        [FieldData(typeof(KAR_WheelAnimation))]
        public KAR_WheelAnimation AnimationBank { get; set; }
    }
}
