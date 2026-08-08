using HSDRaw;
using HSDRaw.Common;
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

    public class KdZoneParamLight : KdZoneParam
    {
        // Priority?
        public int x00 { get; set; } 

        public float x04 { get; set; }

        public Color x08 { get; set; }

        public Color x0C { get; set; }

        public int x10 { get; set; }

        public int x14 { get; set; }

        public int x18 { get; set; }

        public Color x1C { get; set; }

        public int x20 { get; set; }

        public int x24 { get; set; }

        public int x28 { get; set; }

        public float x2C { get; set; }

        public int x30 { get; set; }

        public int x34 { get; set; }

        public int x38 { get; set; }

        public int x3C { get; set; }

        public void SetParam(HSDAccessor acc)
        {
            if (acc == null) return;
            x00 = acc._s.GetInt32(0x00);
            x04 = acc._s.GetFloat(0x04);
            x08 = acc._s.GetColorRGBA(0x08);
            x0C = acc._s.GetColorRGBA(0x0C);

            x10 = acc._s.GetInt32(0x10);
            x14 = acc._s.GetInt32(0x14);
            x18 = acc._s.GetInt32(0x18);
            x1C = acc._s.GetColorRGBA(0x1C);

            x20 = acc._s.GetInt32(0x20);
            x24 = acc._s.GetInt32(0x24);
            x28 = acc._s.GetInt32(0x28);
            x2C = acc._s.GetFloat(0x2C);

            x30 = acc._s.GetInt32(0x30);
            x34 = acc._s.GetInt32(0x34);
            x38 = acc._s.GetInt32(0x38);
            x3C = acc._s.GetInt32(0x3C);
        }

        public HSDAccessor GetParam()
        {
            HSDAccessor acc = new();
            acc._s.Resize(0x40);

            acc._s.SetInt32(0x00, x00);
            acc._s.SetFloat(0x04, x04);
            acc._s.SetColorRGBA(0x08, x08);
            acc._s.SetColorRGBA(0x0C, x0C);

            acc._s.SetInt32(0x10, x10);
            acc._s.SetInt32(0x14, x14);
            acc._s.SetInt32(0x18, x18);
            acc._s.SetColorRGBA(0x1C, x1C);

            acc._s.SetInt32(0x20, x20);
            acc._s.SetInt32(0x24, x24);
            acc._s.SetInt32(0x28, x28);
            acc._s.SetFloat(0x2C, x2C);

            acc._s.SetInt32(0x30, x30);
            acc._s.SetInt32(0x34, x34);
            acc._s.SetInt32(0x38, x38);
            acc._s.SetInt32(0x3C, x3C);
            return acc;
        }

        public override string ToString()
        {
            return $"Light Param";
        }
    }
}
