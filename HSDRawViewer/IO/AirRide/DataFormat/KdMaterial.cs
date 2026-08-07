using HSDRaw.AirRide.Gr.Data;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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

        [Category("0 - General")]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public KdType Type { get; set; }

        [Category("0 - General")]
        [DisplayName("Common Kind")]
        [Description("Index of surface material to use. Surface materials can be found in GrCommon.dat")]
        [JsonPropertyName("cmn")]
        public byte CommonType { get; set; }

        [Category("0 - General")]
        [DisplayName("Rough")]
        [Description("Some index related to rough.")]
        [Range(0, 3)]
        [JsonPropertyName("rough")]
        public byte Rough { get; set; }

        [Category("0 - General")]
        [DisplayName("Player Restitution Index")]
        [Description("Index of restitution in the Stage Node to use for this surface.")]
        [Range(0, 9)]
        [JsonPropertyName("r1")]
        public byte PlayerRestitutionIndex { get; set; }

        [Category("0 - General")]
        [DisplayName("Item Restitution Index")]
        [Description("Index of restitution in the Stage Node to use for this surface.")]
        [Range(0, 9)]
        [JsonPropertyName("r2")]
        public byte ItemRestitutionIndex { get; set; }

        [Category("1 - Flags")]
        [DisplayName("Segmented Move")]
        [Description("If enabled this surface will not use static lookup and will be able to be moved around in game.")]
        [JsonPropertyName("seg")]
        public bool SegmentMove { get; set; }

        [Category("1 - Flags")]
        public bool Flag00002000 { get; set; }

        [Category("1 - Flags")]
        public bool Flag00004000 { get; set; }

        [Category("1 - Flags")]
        public bool Flag00008000 { get; set; }

        [Category("1 - Flags")]
        public bool Flag00010000 { get; set; }

        [Category("1 - Flags")]
        public bool Flag00020000 { get; set; }

        [Category("1 - Flags")]
        public bool Flag00040000 { get; set; }

        [Category("1 - Flags")]
        public bool Flag00080000 { get; set; }

        [Category("1 - Flags")]
        public bool Flag00100000 { get; set; }

        [Category("1 - Flags")]
        public bool Flag00200000 { get; set; }

        [Category("1 - Flags")]
        public bool Flag00400000 { get; set; }

        [Category("1 - Flags")]
        public bool Flag00800000 { get; set; }


        [Category("2 - Conveyor")]
        [DisplayName("Vertical")]
        [JsonPropertyName("conv")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public KdConveyor ConveyorVertical { get; set; }

        [Category("2 - Conveyor")]
        [DisplayName("Horizontal")]
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

        public void SetCollFlag(KCCollFlag f)
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

        private void SetFlags(KAR_CollisionTriangle t)
        {
            Rough = t.Rough;
            PlayerRestitutionIndex = t.PlayerRestituionIndex;
            ItemRestitutionIndex = t.ItemRestitutionIndex;

            var flag = t._s.GetInt32(0x10);

            SegmentMove = t.SegmentMove;
            Flag00002000 = GetBitFlag(flag, 13);
            Flag00004000 = GetBitFlag(flag, 14);
            Flag00008000 = GetBitFlag(flag, 15);
            Flag00010000 = GetBitFlag(flag, 16);
            Flag00020000 = GetBitFlag(flag, 17);
            Flag00040000 = GetBitFlag(flag, 18);
            Flag00080000 = GetBitFlag(flag, 19);
            Flag00100000 = GetBitFlag(flag, 20);
            Flag00200000 = GetBitFlag(flag, 21);
            Flag00400000 = GetBitFlag(flag, 22);
            Flag00800000 = GetBitFlag(flag, 23);

            SetConveyorFlag(t.ConveyorDirection);
        }

        public static KdMaterial FromTriangle(KAR_CollisionTriangle t)
        {
            var flag = t._s.GetInt32(0x10);

            var m = new KdMaterial()
            {
                CommonType = t.GrCommonIndex,
            };
            m.SetCollFlag(t.Flags);
            m.SetFlags(t);
            return m;
        }

        public void SetMaterial(KAR_CollisionTriangle v)
        {
            v.Flags = GetRealFlag();
            v.GrCommonIndex = CommonType;
            v.Rough = Rough;
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
            var temp = new KAR_CollisionTriangle();
            SetMaterial(temp);

            return string.Join("_",
                "Kd",
                $"T{Type}",
                $"C{CommonType}",
                $"F{temp._s.GetInt32(0x10):X8}");
        }

        public static KdMaterial Parse(string name)
        {
            var m = new KdMaterial();

            foreach (var part in name.Split('_'))
            {
                if (part.StartsWith("T"))
                {
                    if (Enum.TryParse(part[1..], out KdType result))
                    {
                        m.Type = result;
                    }
                }
                else if (part.StartsWith("C"))
                {
                    if (byte.TryParse(part[1..], out byte result))
                    {
                        m.CommonType = result;
                    }
                }
                else if (part.StartsWith("F"))
                {
                    if (int.TryParse(part[1..], 
                        NumberStyles.HexNumber, 
                        CultureInfo.InvariantCulture, 
                        out int flags))
                    {
                        var tri = new KAR_CollisionTriangle();
                        tri._s.SetInt32(0x10, flags);
                        m.SetFlags(tri);
                    }
                }
            }

            return m;
        }
    }
}
