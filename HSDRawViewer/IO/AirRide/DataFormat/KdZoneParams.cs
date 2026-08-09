using HSDRaw;
using HSDRaw.Common;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    public interface KdZoneParam
    {
        public void SetParam(HSDAccessor acc);
        public HSDAccessor GetParam();
    }

    public class KdZoneParamGroundBoost : KdZoneParam
    {
        [DisplayName("Boost Index")]
        [Description("Index (0 - 2) of ground boost in Stage node.")]
        [Range(0, 2)]
        public int Index { get; set; }

        public void SetParam(HSDAccessor acc)
        {
            if (acc == null) return;
            HSDIntArray arr = new HSDIntArray() { _s = acc._s };
            Index = arr[0];
        }

        public HSDAccessor GetParam()
        {
            HSDIntArray arr = new HSDIntArray();
            arr.Array = new int[] { Index };
            return arr;
        }

        public override string ToString()
        {
            return $"Boost Param";
        }
    }

    public class KdZoneParamDashGate : KdZoneParam
    {
        [DisplayName("Gate Index")]
        [Description("Index (0 - 1) of gate boost in Stage node.")]
        [Range(0, 1)]
        public int Index { get; set; }

        public void SetParam(HSDAccessor acc)
        {
            if (acc == null) return;
            Index = acc._s.GetInt32(0x00);
        }

        public HSDAccessor GetParam()
        {
            HSDIntArray arr = new HSDIntArray();
            arr.Array = new int[] { Index };
            return arr;
        }
        public override string ToString()
        {
            return $"Gate Param";
        }
    }

    public class KdZoneParamDashRing : KdZoneParam
    {
        [DisplayName("Unknown")]
        [Description("Index (-1 to 9)")]
        public int[] Index { get; set; }

        public void SetParam(HSDAccessor acc)
        {
            if (acc == null) return;
            HSDIntArray arr = new HSDIntArray() { _s = acc._s };
            Index = arr.Array;
        }

        public HSDAccessor GetParam()
        {
            HSDIntArray arr = new HSDIntArray();
            arr.Array = Index;
            return arr;
        }
        public override string ToString()
        {
            return $"Dash Ring Param";
        }
    }

    public class KdZoneParamSuperJump : KdZoneParam
    {
        [DisplayName("Animation Index")]
        [Description("Index of SubAnimNode -> SuperJump")]
        public int AnimationIndex { get; set; }

        //[DisplayName("")]
        //[Description("")]
        public short x04 { get; set; }

        //[DisplayName("")]
        //[Description("")]
        public short x06 { get; set; }

        //[DisplayName("")]
        //[Description("")]
        public float x08 { get; set; }

        //[DisplayName("")]
        //[Description("")]
        public bool Flag { get; set; }

        public void SetParam(HSDAccessor acc)
        {
            if (acc == null) return;
            AnimationIndex = acc._s.GetInt32(0x00);
            x04 = acc._s.GetInt16(0x04);
            x06 = acc._s.GetInt16(0x06);
            x08 = acc._s.GetFloat(0x08);
            Flag = acc._s.GetUInt32(0x0C) == 0x80000000;
        }

        public HSDAccessor GetParam()
        {
            HSDAccessor acc = new();
            acc._s.Resize(0x10);
            acc._s.SetInt32(0x00, AnimationIndex);
            acc._s.SetInt16(0x04, x04);
            acc._s.SetInt16(0x06, x06);
            acc._s.SetFloat(0x08, x08);
            acc._s.SetUInt32(0x0C, Flag ? 0x80000000 : 0);
            return acc;
        }
        public override string ToString()
        {
            return $"Super Jump Param";
        }
    }

    public class KdZoneParamLeap : KdZoneParam
    {
        [DisplayName("Animation Index")]
        [Description("Index of SubAnimNode -> Leap")]
        public int AnimationIndex { get; set; }

        //[DisplayName("")]
        //[Description("")]
        public short x04 { get; set; }

        //[DisplayName("")]
        //[Description("")]
        public short x06 { get; set; }

        //[DisplayName("")]
        //[Description("")]
        public int x08 { get; set; }

        public void SetParam(HSDAccessor acc)
        {
            if (acc == null) return;
            AnimationIndex = acc._s.GetInt32(0x00);
            x04 = acc._s.GetInt16(0x04);
            x06 = acc._s.GetInt16(0x06);
            x08 = acc._s.GetInt32(0x08);
        }

        public HSDAccessor GetParam()
        {
            HSDAccessor acc = new();
            acc._s.Resize(0xC);
            acc._s.SetInt32(0x00, AnimationIndex);
            acc._s.SetInt16(0x04, x04);
            acc._s.SetInt16(0x06, x06);
            acc._s.SetInt32(0x08, x08);
            return acc;
        }
        public override string ToString()
        {
            return $"Leap Param";
        }
    }

    public class KdZoneParamAirFlow : KdZoneParam
    {
        public class AirFlowEntry : HSDAccessor
        {
            public override int TrimmedSize => 0xC;

            public float x00 { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }

            public float x04 { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

            public int x08 { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }
        }

        public class Param : HSDAccessor
        {
            public override int TrimmedSize => 0x0C;

            public List<AirFlowEntry> Params { get; set; } = new List<AirFlowEntry>();


            [Browsable(false)]
            public HSDArrayAccessor<AirFlowEntry> _points
            {
                get => _s.GetReference<HSDArrayAccessor<AirFlowEntry>>(0x00);
                set => _s.SetReference(0x00, value);
            }

            [Browsable(false)]
            public int PointCount
            {
                get => _s.GetInt32(0x04);
                set => _s.SetInt32(0x04, value);
            }

            public int Index
            {
                get => _s.GetInt32(0x08);
                set => _s.SetInt32(0x08, value);
            }
        }

        public List<Param> Entries { get; set; } = new List<Param>();

        public void SetParam(HSDAccessor acc)
        {
            if (acc == null) return;
            HSDArrayAccessor<Param> arr = new HSDArrayAccessor<Param>() { _s = acc._s };
            Entries.AddRange(arr.Array);
            foreach (var e in Entries)
            {
                e.Params = e._points.Array.ToList();
            }
        }

        public HSDAccessor GetParam()
        {
            HSDArrayAccessor<Param> arr = new HSDArrayAccessor<Param>();
            foreach (var e in Entries)
            {
                e._points.Array = e.Params.ToArray();
                e.PointCount = e.Params.Count;
            }
            arr.Array = Entries.ToArray();
            return arr;
        }

        public override string ToString()
        {
            return $"Air Flow Param";
        }
    }

    public class KdZoneParamSwitch : KdZoneParam
    {
        public int x00 { get; set; }

        public int x04 { get; set; }

        public int x08 { get; set; }

        public int x0C { get; set; }

        public void SetParam(HSDAccessor acc)
        {
            if (acc == null) return;
            x00 = acc._s.GetInt32(0x00);
            x04 = acc._s.GetInt32(0x04);
            x08 = acc._s.GetInt32(0x08);
            x0C = acc._s.GetInt32(0x0C);
        }

        public HSDAccessor GetParam()
        {
            HSDAccessor acc = new();
            acc._s.Resize(0x10);
            acc._s.SetInt32(0x00, x00);
            acc._s.SetInt32(0x04, x04);
            acc._s.SetInt32(0x08, x08);
            acc._s.SetInt32(0x0C, x0C);
            return acc;
        }
        public override string ToString()
        {
            return $"Switch Param";
        }
    }

    public class KdZoneParamDeath : KdZoneParam
    {
        [DisplayName("LocalDeadPos Index")]
        [Description("LocalDeadPos index to respawn player at after voiding out.")]
        public int LocalDeadPos { get; set; }

        public void SetParam(HSDAccessor acc)
        {
            if (acc == null) return;
            LocalDeadPos = acc._s.GetInt32(0x00);
        }

        public HSDAccessor GetParam()
        {
            HSDAccessor acc = new();
            acc._s.Resize(0x4);
            acc._s.SetInt32(0x00, LocalDeadPos);
            return acc;
        }
        public override string ToString()
        {
            return $"Death Plane Param";
        }
    }


    public class KdZoneParam26 : KdZoneParam
    {
        //[DisplayName("")]
        //[Description("")]
        public List<int> Entries { get; set; } = new List<int>();

        public void SetParam(HSDAccessor acc)
        {
            if (acc == null) return;
            var arr = acc._s.GetReference<HSDIntArray>(0x00);
            Entries = new List<int>();
            Entries.AddRange(arr.Array);
        }

        public HSDAccessor GetParam()
        {
            HSDAccessor acc = new();
            acc._s.Resize(0x8);
            acc._s.SetReference(0x00, new HSDIntArray() { Array = Entries.ToArray() });
            acc._s.SetInt32(0x04, Entries.Count);
            return acc;
        }
        public override string ToString()
        {
            return $"Zone 26 Param";
        }
    }

    public class KdZoneParamSound : KdZoneParam
    {
        [DisplayName("FGM Node Entry2 Index")]
        public int FGMNodeEntry { get; set; }

        public float Unknown { get; set; }

        public void SetParam(HSDAccessor acc)
        {
            if (acc == null) return;
            FGMNodeEntry = acc._s.GetInt32(0x00);
            Unknown = acc._s.GetFloat(0x04);
        }

        public HSDAccessor GetParam()
        {
            HSDAccessor acc = new();
            acc._s.Resize(0x8);
            acc._s.SetInt32(0x00, FGMNodeEntry);
            acc._s.SetFloat(0x04, Unknown);
            return acc;
        }

        public override string ToString()
        {
            return $"Sound Param";
        }
    }

    public class KdZoneVector
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public KdZoneVector()
        {
        }

        public KdZoneVector(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return "Vector";
        }
    }

    public class KdZoneParamLight : KdZoneParam
    {
        [DisplayName("Priority")]
        [Description("High values increase prioity of light to affect objects. There are a limited number of lights that can affect each object at one time.")]
        public int Priority { get; set; }

        [DisplayName("Unknown (x04)")]
        [Description("")]
        public byte x04 { get; set; }


        [DisplayName("Flag0")]
        [Description("")]
        public bool AmbientEnabled { get; set; }

        [DisplayName("Flag1")]
        [Description("")]
        public bool UnknownEnabled { get; set; }

        [DisplayName("Flag2")]
        [Description("")]
        public bool DiffuseEnabled { get; set; }


        [DisplayName("Light[0] Enabled")]
        [Description("Enables the use of this lighting.")]
        public bool Light0Enabled { get; set; }


        [DisplayName("Light[1] Enabled")]
        [Description("Enables the use of this lighting.")]
        public bool Light1Enabled { get; set; }


        [DisplayName("Light[2] Enabled")]
        [Description("Enables the use of this lighting.")]
        public bool Light2Enabled { get; set; }


        [DisplayName("Light[0] Color")]
        [Description("Base color of this light.")]
        public Color Color0 { get; set; }

        [DisplayName("Light[1] Color")]
        [Description("Base color of this light.")]
        public Color Color1 { get; set; }

        [DisplayName("Light[0] Direction")]
        [Description("")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KdZoneVector Direction0 { get; } = new KdZoneVector();

        [DisplayName("Light[2] Color")]
        [Description("Base color of this light.")]
        public Color Color2 { get; set; }

        [DisplayName("Light[1] Direction")]
        [Description("")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KdZoneVector Direction1 { get; } = new KdZoneVector();

        [DisplayName("Unknown Interpolation Value")]
        [Description("")]
        public float x2C { get; set; }

        [DisplayName("Light[2] Direction")]
        [Description("")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KdZoneVector Direction2 { get; } = new KdZoneVector();

        [DisplayName("Direction Enabled")]
        [Description("")]
        public byte UseDirectionVectors { get; set; }


        public void SetParam(HSDAccessor acc)
        {
            if (acc == null) return;
            Priority = acc._s.GetInt32(0x00);

            x04 = acc._s.GetByte(0x04);

            var enable_flag = acc._s.GetByte(0x05);
            Light0Enabled   = (enable_flag & 0b00000001) != 0;
            Light1Enabled   = (enable_flag & 0b00000010) != 0;
            Light2Enabled   = (enable_flag & 0b00000100) != 0;
            AmbientEnabled  = (enable_flag & 0b00001000) != 0;
            UnknownEnabled  = (enable_flag & 0b00010000) != 0;
            DiffuseEnabled  = (enable_flag & 0b00100000) != 0;

            Color0 = acc._s.GetColorRGBA(0x08);
            Color1 = acc._s.GetColorRGBA(0x0C);

            Direction0.X = acc._s.GetFloat(0x10);
            Direction0.Y = acc._s.GetFloat(0x14);
            Direction0.Z = acc._s.GetFloat(0x18);

            Color2 = acc._s.GetColorRGBA(0x1C);

            Direction1.X = acc._s.GetFloat(0x20);
            Direction1.Y = acc._s.GetFloat(0x24);
            Direction1.Z = acc._s.GetFloat(0x28);

            x2C = acc._s.GetFloat(0x2C);

            Direction2.X = acc._s.GetFloat(0x20);
            Direction2.Y = acc._s.GetFloat(0x24);
            Direction2.Z = acc._s.GetFloat(0x28);

            UseDirectionVectors = acc._s.GetByte(0x3C);

            var flag = acc._s.GetUInt32(0x3C);
            if ((flag & 0x00FFFFFF) != 0)
            {
                throw new NotSupportedException();
            }
        }

        public HSDAccessor GetParam()
        {
            HSDAccessor acc = new();
            acc._s.Resize(0x40);

            acc._s.SetInt32(0x00, Priority);

            acc._s.SetByte(0x04, x04);

            int enable_flag = 0;
            enable_flag |= (Light0Enabled ? 1 : 0) << 0;
            enable_flag |= (Light1Enabled ? 1 : 0) << 1;
            enable_flag |= (Light2Enabled ? 1 : 0) << 2;
            enable_flag |= (AmbientEnabled ? 1 : 0) << 3;
            enable_flag |= (UnknownEnabled ? 1 : 0) << 4;
            enable_flag |= (DiffuseEnabled ? 1 : 0) << 5;
            acc._s.SetByte(0x05, (byte)enable_flag);

            acc._s.SetColorRGBA(0x08, Color0);
            acc._s.SetColorRGBA(0x0C, Color1);

            acc._s.SetFloat(0x10, Direction0.X);
            acc._s.SetFloat(0x14, Direction0.Y);
            acc._s.SetFloat(0x18, Direction0.Z);

            acc._s.SetColorRGBA(0x1C, Color2);

            acc._s.SetFloat(0x20, Direction1.X);
            acc._s.SetFloat(0x24, Direction1.X);
            acc._s.SetFloat(0x28, Direction1.Z);

            acc._s.SetFloat(0x2C, x2C);

            acc._s.SetFloat(0x30, Direction2.X);
            acc._s.SetFloat(0x34, Direction2.Y);
            acc._s.SetFloat(0x38, Direction2.Z);

            acc._s.SetByte(0x3C, UseDirectionVectors);
            return acc;
        }

        public override string ToString()
        {
            return $"Light Param";
        }
    }
}
