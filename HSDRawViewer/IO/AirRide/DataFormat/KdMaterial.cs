using HSDRaw.AirRide.Gr.Data;
using System;
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

    public class KdMaterial
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
                Restitution = t.PlayerRestituionIndex,
                Restitution2 = t.ItemRestitutionIndex,
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
            v.PlayerRestituionIndex = Restitution;
            v.ItemRestitutionIndex = Restitution2;
            v.ConveyorDirection = GetConveyorFlag();
            v.SegmentMove = SegmentMove;
        }

        public override string ToString()
        {
            return string.Join("_",
                "Kd",
                $"T{Type}",
                $"Cmn{CommonType}",
                $"Fr{Friction}",
                $"R1{Restitution}",
                $"R2{Restitution2}",
                $"Seg{(SegmentMove ? 1 : 0)}",
                $"CV{ConveyorVertical}",
                $"CH{ConveyorHorizontal}"
            );
        }

        public static KdMaterial Parse(string name)
        {
            var parts = name.Split('_');

            if (parts.Length != 9 || parts[0] != "Kd")
                throw new FormatException($"Invalid KdMaterial name: {name}");

            return new KdMaterial
            {
                Type = Enum.Parse<KdType>(parts[1][1..]),
                CommonType = byte.Parse(parts[2][3..]),
                Friction = byte.Parse(parts[3][2..]),
                Restitution = byte.Parse(parts[4][2..]),
                Restitution2 = byte.Parse(parts[5][2..]),
                SegmentMove = parts[6][3..] == "1",
                ConveyorVertical = Enum.Parse<KdConveyor>(parts[7][2..]),
                ConveyorHorizontal = Enum.Parse<KdConveyor>(parts[8][2..])
            };
        }
    }
}
