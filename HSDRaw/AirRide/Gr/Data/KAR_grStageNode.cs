using HSDRaw.Common;
using HSDRaw.GX;
using System;
using System.ComponentModel;
using System.Linq;

namespace HSDRaw.AirRide.Gr.Data
{
    public class KAR_grStageNode : HSDAccessor
    {
        public override int TrimmedSize => 0xE8;

        [Category("0 - General")]
        [DisplayName("Shadow Alpha (Unused)")]
        [Description("Unused constant for shadow alpha.")]
        public int Unk1 { get => _s.GetInt32(0x0); set => _s.SetInt32(0x0, value); }

        [Category("0 - General")]
        [DisplayName("Machine Acceleration Scale")]
        [Description("Global stage acceleration constant")]
        public float MachineAccel { get => _s.GetFloat(0x4); set => _s.SetFloat(0x4, value); }

        [Category("0 - General")]
        [DisplayName("Stage Scale")]
        [Description("Amount to scale entire stage.")]
        public float StageScale { get => _s.GetFloat(0x8); set => _s.SetFloat(0x8, value); }

        [Category("0 - General")]
        [DisplayName("Gravity")]
        [Description("Magnitude of the gravity force.")]
        public float GravityStrength { get => _s.GetFloat(0xc); set => _s.SetFloat(0xc, value); }

        [Category("0 - General")]
        [DisplayName("Gravity Direction X")]
        [Description("X component of the gravity direction vector.")]
        public float GravityDirectionX { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }

        [Category("0 - General")]
        [DisplayName("Gravity Direction Y")]
        [Description("Y component of the gravity direction vector.")]
        public float GravityDirectionY { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }

        [Category("0 - General")]
        [DisplayName("Gravity Direction Z")]
        [Description("Z component of the gravity direction vector.")]
        public float GravityDirectionZ { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }

        [Category("0 - General")]
        [DisplayName("Map Fog Enabled")]
        [Description("Enables fog rendering on the map model.")]
        public bool MapFogEnabled { get => (_s.GetByte(0x1c) & 0x01) != 0; set => _s.SetByte(0x1c, (byte)((_s.GetByte(0x1c) & ~0x01) | (value ? 0x01 : 0x00))); }

        [Category("0 - General")]
        [DisplayName("Player Fog Enabled")]
        [Description("Enables fog rendering on the player and effect models.")]
        public bool PlayerFogEnabled { get => (_s.GetByte(0x1c) & 0x02) != 0; set => _s.SetByte(0x1c, (byte)((_s.GetByte(0x1c) & ~0x02) | (value ? 0x02 : 0x00))); }


        [Category("1 - Restitution")]
        [DisplayName("Item Restitution")]
        [Description("The coefficient of restitution for items.")]
        public HSDBinaryArray<float> ItemRestitution { get; }

        [Category("1 - Restitution")]
        [DisplayName("Player Restitution")]
        [Description("The coefficient of restitution for players.")]
        public HSDBinaryArray<float> PlayerRestitution { get; }


        [Category("2 - Boost Params")]
        [DisplayName("Pads")]
        [Description("Params referenced by boost zones to control boost speed.")]
        public HSDBinaryArray<BoostAccessor> BoostPads { get; }

        [Category("2 - Boost Params")]
        [DisplayName("Gates")]
        [Description("Params referenced by boost zones to control boost speed.")]
        public HSDBinaryArray<BoostAccessor> BoostGates { get; }

        [Category("2 - Boost Params")]
        [DisplayName("Rings")]
        [Description("Params referenced by boost zones to control boost speed.")]
        public HSDBinaryArray<BoostAccessor> BoostRings { get; }


        [Category("3 - Minimap")]
        [DisplayName("Scale")]
        public float MinimapScale { get => _s.GetFloat(0x60); set => _s.SetFloat(0x60, value); }

        [Category("3 - Minimap")]
        [DisplayName("Offset X")]
        public float MinimapPlayerX { get => _s.GetFloat(0x64); set => _s.SetFloat(0x64, value); }

        [Category("3 - Minimap")]
        [DisplayName("Offset Y")]
        public float MinimapPlayerY { get => _s.GetFloat(0x68); set => _s.SetFloat(0x68, value); }

        [Category("3 - Minimap")]
        [DisplayName("Offset Z")]
        public float MinimapPlayerZ { get => _s.GetFloat(0x6c); set => _s.SetFloat(0x6c, value); }


        [Category("4 - Audio Flags")]
        [DisplayName("Flag 1")]
        [Description("Unknown flag")]
        public byte AudioFlag1 { get => _s.GetByte(0x80); set => _s.SetByte(0x80, value); }

        [Category("4 - Audio Flags")]
        [DisplayName("Flag 2")]
        [Description("Unknown flag")]
        public byte AudioFlag2 { get => _s.GetByte(0x81); set => _s.SetByte(0x81, value); }

        [Category("4 - Audio Flags")]
        [DisplayName("Flag 3")]
        [Description("Unknown Audio Flag used in Machine only.")]
        public byte AudioFlag3 { get => _s.GetByte(0x82); set => _s.SetByte(0x82, value); }


        [Category("5 - Bounding")]
        [DisplayName("Min X")]
        public float OoBMinXArea { get => _s.GetFloat(0xcc); set => _s.SetFloat(0xcc, value); }

        [Category("5 - Bounding")]
        [DisplayName("Min Y")]
        public float OoBMinYArea { get => _s.GetFloat(0xd0); set => _s.SetFloat(0xd0, value); }

        [Category("5 - Bounding")]
        [DisplayName("Min Z")]
        public float OoBMinZArea { get => _s.GetFloat(0xd4); set => _s.SetFloat(0xd4, value); }

        [Category("5 - Bounding")]
        [DisplayName("Max X")]
        public float OoBMaxXArea { get => _s.GetFloat(0xd8); set => _s.SetFloat(0xd8, value); }

        [Category("5 - Bounding")]
        [DisplayName("Max Y")]
        public float OoBMaxYArea { get => _s.GetFloat(0xdc); set => _s.SetFloat(0xdc, value); }

        [Category("5 - Bounding")]
        [DisplayName("Max Z")]
        public float OoBMaxZArea { get => _s.GetFloat(0xe0); set => _s.SetFloat(0xe0, value); }


        [Category("6 - Unused Params")]
        [DisplayName("Air Flow 1")]
        [Description("These params are only used for testing and are generally unused.")]
        public HSDArrayAccessor<HSD_Vector3> AirflowParam1 { get => _s.GetReference<HSDArrayAccessor<HSD_Vector3>>(0x70); set => _s.SetReference(0x70, value); }

        [Category("6 - Unused Params")]
        [DisplayName("Spline 1")]
        [Description("These params are only used for testing and are unused.")]
        public HSDArrayAccessor<HSD_Vector3> SplineParam1 { get => _s.GetReference<HSDArrayAccessor<HSD_Vector3>>(0x74); set => _s.SetReference(0x74, value); }

        [Category("6 - Unused Params")]
        [DisplayName("Air Flow 2")]
        [Description("These params are only used for testing and are generally unused.")]
        public HSDArrayAccessor<HSD_Vector3> AirflowParam2 { get => _s.GetReference<HSDArrayAccessor<HSD_Vector3>>(0x78); set => _s.SetReference(0x78, value); }

        [Category("6 - Unused Params")]
        [DisplayName("Spline 2")]
        [Description("These params are only used for testing and are unused.")]
        public HSDArrayAccessor<HSD_Vector3> SplineParam2 { get => _s.GetReference<HSDArrayAccessor<HSD_Vector3>>(0x7C); set => _s.SetReference(0x7C, value); }


        [Category("7 - Misc")]
        // TODO:
        public KAR_StagePadCountPointer PointerToBoostPad { get => _s.GetReference<KAR_StagePadCountPointer>(0xe4); set => _s.SetReference(0xe4, value); }


        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class BoostAccessor : HSDAccessor
        {
            public float AccelerationBoostPad1 { get => _s.GetFloat(offset); set => _s.SetFloat(offset, value); }

            public float AccelerationBoostPad2 { get => _s.GetFloat(offset + 4); set => _s.SetFloat(offset + 4, value); }

            public float AccelerationTimeL { get => _s.GetFloat(offset + 8); set => _s.SetFloat(offset + 8, value); }

            private HSDStruct _s;
            private int offset;

            public BoostAccessor(HSDStruct s, int offset)
            {
                _s = s;
                this.offset = offset;
            }

            public override string ToString()
            {
                return $"{AccelerationBoostPad1} {AccelerationBoostPad2} {AccelerationTimeL}";
            }
        }

        public KAR_grStageNode()
        {
            BoostPads = new HSDBinaryArray<BoostAccessor>(
                2,
                i => new BoostAccessor(_s, 0x84 + i * 0xC),
                (i, v) => _s.SetEmbededStruct(0x84 + i * 0xC, v._s));

            BoostGates = new HSDBinaryArray<BoostAccessor>(
                2,
                i => new BoostAccessor(_s, 0x9C + i * 0xC),
                (i, v) => _s.SetEmbededStruct(0x9C + i * 0xC, v._s));

            BoostRings = new HSDBinaryArray<BoostAccessor>(
                2,
                i => new BoostAccessor(_s, 0xB4 + i * 0xC),
                (i, v) => _s.SetEmbededStruct(0xB4 + i * 0xC, v._s));

            ItemRestitution = new HSDBinaryArray<float>(
                8,
                i => _s.GetFloat(0x20 + i * 4),
                (i, v) => _s.SetFloat(0x20 + i * 4, v));

            PlayerRestitution = new HSDBinaryArray<float>(
                8,
                i => _s.GetFloat(0x40 + i * 4),
                (i, v) => _s.SetFloat(0x40 + i * 4, v));
        }
    }

    public class KAR_StagePadCountPointer : HSDAccessor
    {
        public override int TrimmedSize => 0x04;

        public KAR_StagePadCount PadCount { get => _s.GetReference<KAR_StagePadCount>(0x0); set => _s.SetReference(0x0, value); }
    }

    public class KAR_StagePadCount : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public int Index0 { get => _s.GetInt32(0x0); set => _s.SetInt32(0x0, value); }

        public int Index1 { get => _s.GetInt32(0x4); set => _s.SetInt32(0x4, value); }
    }
}
