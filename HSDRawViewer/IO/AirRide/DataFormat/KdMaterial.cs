using HSDRaw.AirRide.Gr.Data;
using System;
using System.ComponentModel;
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

        [DisplayName("Common Kind")]
        [Description("Index of surface material to use. Surface materials can be found in GrCommon.dat")]
        [JsonPropertyName("cmn")]
        public byte CommonType { get; set; }

        [DisplayName("Friction")]
        [Description("Index of friction data to use for this surface.")]
        [JsonPropertyName("fric")]
        public byte Friction { get; set; }

        [DisplayName("Player Restitution Index")]
        [Description("Index of restitution in the Stage Node to use for this surface.")]
        [JsonPropertyName("r1")]
        public byte PlayerRestitutionIndex { get; set; }

        [DisplayName("Item Restitution Index")]
        [Description("Index of restitution in the Stage Node to use for this surface.")]
        [JsonPropertyName("r2")]
        public byte ItemRestitutionIndex { get; set; }

        [DisplayName("Segmented")]
        [Description("If enabled this surface will not use static lookup and will be able to be moved around in game.")]
        [JsonPropertyName("seg")]
        public bool SegmentMove { get; set; }

        public bool Flag00002000 { get; set; }

        public bool Flag00004000 { get; set; }

        public bool Flag00008000 { get; set; }

        public bool Flag00010000 { get; set; }

        public bool Flag00020000 { get; set; }

        public bool Flag00040000 { get; set; }

        public bool Flag00080000 { get; set; }

        public bool Flag00100000 { get; set; }

        public bool Flag00200000 { get; set; }

        public bool Flag00400000 { get; set; }

        public bool Flag00800000 { get; set; }


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

        public static bool GetBitFlag(int value, int bit)
        {
            return ((value >> bit) & 0x1) != 0;
        }
        public static void SetBitFlag(ref int value, int bit, bool set)
        {
            int mask = 1 << bit;
            value = set ? (value | mask) : (value & ~mask);
        }

        public static KdMaterial FromTriangle(KAR_CollisionTriangle t)
        {
            var flag = t._s.GetInt32(0x10);

            var m = new KdMaterial()
            {
                CommonType = t.GrCommonIndex,
                Friction = t.Rough,
                PlayerRestitutionIndex = t.PlayerRestituionIndex,
                ItemRestitutionIndex = t.ItemRestitutionIndex,
                SegmentMove = t.SegmentMove,
                Flag00002000 = GetBitFlag(flag, 13),
                Flag00004000 = GetBitFlag(flag, 14),
                Flag00008000 = GetBitFlag(flag, 15),
                Flag00010000 = GetBitFlag(flag, 16),
                Flag00020000 = GetBitFlag(flag, 17),
                Flag00040000 = GetBitFlag(flag, 18),
                Flag00080000 = GetBitFlag(flag, 19),
                Flag00100000 = GetBitFlag(flag, 20),
                Flag00200000 = GetBitFlag(flag, 21),
                Flag00400000 = GetBitFlag(flag, 22),
                Flag00800000 = GetBitFlag(flag, 23),
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
            v.PlayerRestituionIndex = PlayerRestitutionIndex;
            v.ItemRestitutionIndex = ItemRestitutionIndex;
            v.ConveyorDirection = GetConveyorFlag();
            v.SegmentMove = SegmentMove;

            var flag = v._s.GetInt32(0x10);

            SetBitFlag(ref flag, 13, Flag00002000);
            SetBitFlag(ref flag, 14, Flag00004000);
            SetBitFlag(ref flag, 15, Flag00008000);
            SetBitFlag(ref flag, 16, Flag00010000);
            SetBitFlag(ref flag, 17, Flag00020000);
            SetBitFlag(ref flag, 18, Flag00040000);
            SetBitFlag(ref flag, 19, Flag00080000);
            SetBitFlag(ref flag, 20, Flag00100000);
            SetBitFlag(ref flag, 21, Flag00200000);
            SetBitFlag(ref flag, 22, Flag00400000);
            SetBitFlag(ref flag, 23, Flag00800000);

            v._s.SetInt32(0x10, flag);
        }

        public override string ToString()
        {
            return string.Join("_",
                "Kd",
                $"T{Type}",
                $"Cmn{CommonType}",
                $"Fr{Friction}",
                $"R1{PlayerRestitutionIndex}",
                $"R2{ItemRestitutionIndex}",
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
                PlayerRestitutionIndex = byte.Parse(parts[4][2..]),
                ItemRestitutionIndex = byte.Parse(parts[5][2..]),
                SegmentMove = parts[6][3..] == "1",
                ConveyorVertical = Enum.Parse<KdConveyor>(parts[7][2..]),
                ConveyorHorizontal = Enum.Parse<KdConveyor>(parts[8][2..])
            };
        }
    }
}
