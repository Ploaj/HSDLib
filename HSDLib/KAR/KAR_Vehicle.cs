
namespace HSDLib.KAR
{
    public class KAR_Vehicle : IHSDNode
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
}
