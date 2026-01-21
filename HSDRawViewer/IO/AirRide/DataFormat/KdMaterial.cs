using HSDRaw.AirRide.Gr.Data;
using System.Text.Json.Serialization;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    public enum KdType
    {
        NONE,
        FLOOR,
        CEILING,
        WALL,
        UNKNOWN,
    }

    public enum KdConveyor
    {
        NONE,
        FORWARD,
        BACKWARD,
        LEFT,
        RIGHT,
    }

    internal class KdMaterial
    {
        //[JsonPropertyName("name")]
        //public string Name { get; set; }

        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public KdType Type { get; set; }

        [JsonPropertyName("cmn")]
        public byte CommonType { get; set; }

        [JsonPropertyName("fric")]
        public byte Friction { get; set; }

        [JsonPropertyName("r1")]
        public byte Restitution { get; set; }

        [JsonPropertyName("r2")]
        public byte Restitution2 { get; set; }

        [JsonPropertyName("seg")]
        public bool SegmentMove { get; set; }

        [JsonPropertyName("conv")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public KdConveyor ConveyorVertical { get; set; }

        [JsonPropertyName("conh")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public KdConveyor ConveyorHorizontal { get; set; }

        public KCCollFlag GetRealFlag()
        {
            switch (Type)
            {
                case KdType.FLOOR: return KCCollFlag.Floor;
                case KdType.CEILING: return KCCollFlag.Ceiling;
                case KdType.WALL: return KCCollFlag.Wall;
                case KdType.UNKNOWN: return KCCollFlag.Unknown;
                default: return KCCollFlag.None;
            }
        }

        public KCConveyorDirection GetConveyorFlag()
        {
            KCConveyorDirection dir = (KCConveyorDirection)0;

            switch (ConveyorHorizontal)
            {
                case KdConveyor.LEFT: dir |= KCConveyorDirection.DirLeft; break;
                case KdConveyor.RIGHT: dir |= KCConveyorDirection.DirRight; break;
            }

            switch (ConveyorVertical)
            {
                case KdConveyor.FORWARD: dir |= KCConveyorDirection.DirFront; break;
                case KdConveyor.BACKWARD: dir |= KCConveyorDirection.DirBack; break;
            }

            return dir;
        }

        public void SetRealFlag(KCCollFlag f)
        {
            switch (f)
            {
                case KCCollFlag.Floor: Type = KdType.FLOOR; break;
                case KCCollFlag.Ceiling: Type = KdType.CEILING; break;
                case KCCollFlag.Wall: Type = KdType.WALL; break;
                case KCCollFlag.Unknown: Type = KdType.UNKNOWN; break;
                default: Type = KdType.NONE; break;
            }
        }

        public void SetConveyorFlag(KCConveyorDirection dir)
        {
            if (dir.HasFlag(KCConveyorDirection.DirBack))
                ConveyorVertical = KdConveyor.BACKWARD;
            else
            if (dir.HasFlag(KCConveyorDirection.DirFront))
                ConveyorVertical = KdConveyor.FORWARD;
            else
                ConveyorVertical = KdConveyor.NONE;

            if (dir.HasFlag(KCConveyorDirection.DirLeft))
                ConveyorHorizontal = KdConveyor.LEFT;
            else
            if (dir.HasFlag(KCConveyorDirection.DirRight))
                ConveyorHorizontal = KdConveyor.RIGHT;
            else
                ConveyorHorizontal = KdConveyor.NONE;
        }

        public static KdMaterial FromTriangle(KAR_CollisionTriangle t)
        {
            var m = new KdMaterial()
            {
                CommonType = t.GrCommonIndex,
                Friction = t.Rough,
                Restitution = t.StageNodeReflectIndex,
                Restitution2 = t.StageNodeForceReflectIndex,
                SegmentMove = t.SegmentMove,
            };
            m.SetRealFlag(t.Flags);
            m.SetConveyorFlag(t.ConveyorDirection);
            return m;
        }

        public void SetMaterial(KAR_CollisionTriangle v)
        {
            v.Flags = GetRealFlag();
            v.GrCommonIndex = CommonType;
            v.Rough = Friction;
            v.StageNodeReflectIndex = Restitution;
            v.StageNodeForceReflectIndex = Restitution2;
            v.ConveyorDirection = GetConveyorFlag();
            v.SegmentMove = SegmentMove;
        }
    }
}
