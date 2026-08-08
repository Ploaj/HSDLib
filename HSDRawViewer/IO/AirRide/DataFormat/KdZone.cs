using HSDRaw;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    public enum ZoneKind
    {
        GroundedBoost,
        GroundedBoostForce,
        DashGate1,
        DashGate2,
        DashRing,
        WarpIn,
        WarpOut,
        SuperJump,
        SuperJumpCameraTrailer,
        Leap,
        Spin,
        Airflow,
        SwitchGrounded,
        SwitchRing,
        SwitchArea,
        RandomAbility,
        FreeMovement,
        DownForce,
        ClawStart,
        ClawEnd,
        Unknown20,
        Unknown21,
        Canon,
        ClawTarget,
        Unknown24,
        DeathPlane,
        Unknown26, // shield?
        Unknown27,
        Unknown28,
        Unknown29,
        Unknown30,
        PlaySound,
        LightArea,
        Heal,
        Reverb,
        Unknown35,
    }

    public class KdZone // : ICustomTypeDescriptor
    {
        [Browsable(false)]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Category("0 - General")]
        [JsonPropertyName("type")]
        public ZoneKind Type 
        { 
            get => type; 
            set
            {
                type = value;
                UpdateParam();
            }
        }

        private ZoneKind type;

        [Category("0 - General")]
        [JsonPropertyName("flags")]
        public uint Flags { get; set; }

        [Category("0 - General")]
        [DisplayName("Parent Joint")]
        [JsonPropertyName("parent")]
        public int Parent { get; set; } = -1;

        [Category("0 - General")]
        [DisplayName("Linked Zone Index")]
        [Description("Specifies the index of the destination or connected zone. Used by warps and connected movement zones.")]
        public int LinkedZone { get; set; }


        [Category("1 - Params")]
        [DisplayName("Param")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public object Param { get; set; } = null;


        [Category("2 - Data")]
        [JsonPropertyName("vertices")]
        public List<List<float>> Vertices { get; set; } = new List<List<float>>();

        [Category("2 - Data")]
        [JsonPropertyName("triangles")]
        public List<KdZoneTriangle> Triangles { get; set; } = new List<KdZoneTriangle>();

        public float[] Matrix { get; set; } = new float[12];


        #region Params

        private Dictionary<ZoneKind, KdZoneParam> ZoneParams = new Dictionary<ZoneKind, KdZoneParam>()
        {
            { ZoneKind.GroundedBoost, new KdZoneParamGroundBoost() },
            { ZoneKind.GroundedBoostForce, new KdZoneParamGroundBoost() },

            { ZoneKind.DashGate1, new KdZoneParamDashGate() },
            { ZoneKind.DashGate2, new KdZoneParamDashGate() },

            { ZoneKind.DashRing, new KdZoneParamDashRing() },

            { ZoneKind.SuperJump, new KdZoneParamSuperJump() },
            { ZoneKind.Leap, new KdZoneParamLeap() },

            { ZoneKind.Airflow, new KdZoneParamAirFlow() },

            { ZoneKind.SwitchGrounded, new KdZoneParamSwitch() },
            { ZoneKind.SwitchRing, new KdZoneParamSwitch() },
            { ZoneKind.SwitchArea, new KdZoneParamSwitch() },

            { ZoneKind.DeathPlane, new KdZoneParamDeath() },

            { ZoneKind.Unknown26, new KdZoneParam26() },

            { ZoneKind.PlaySound, new KdZoneParamSound() },

            { ZoneKind.LightArea, new KdZoneParamLight() },
        };

        private int Value30;

        #endregion

        private void UpdateParam()
        {
            if (Param is int i)
            {
                Value30 = i;
            }
            if (ZoneParams.TryGetValue(Type, out KdZoneParam param))
            {
                Param = param;
            }
            else
            {
                Param = null;
            }
        }

        public void SetParam(int value, HSDAccessor accessor)
        {
            if (Type == ZoneKind.Unknown30)
            {
                Value30 = value;
                Param = value;
            }
            if (ZoneParams.TryGetValue(Type, out KdZoneParam param))
            {
                param.SetParam(accessor);
                Param = param;
            }
        }

        public object GetParam()
        {
            if (Param is int i)
                return i;

            if (ZoneParams.TryGetValue(Type, out KdZoneParam param))
            {
                return param.GetParam();
            }
            return null;
        }
    }
}
