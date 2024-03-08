namespace HSDRaw.AirRide.Gr.Data
{
    public class KAR_grRailCollNode : HSDAccessor
    {
        public override int TrimmedSize => 0x8;

        public HSDFixedLengthPointerArrayAccessor<KAR_grRailColl> Animations { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<KAR_grRailColl>>(0x00); set => _s.SetReference(0x00, value); }

        public int Count { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }
    }

    public class KAR_grRailColl : HSDAccessor
    {
        public override int TrimmedSize => 0x34;

        public KAR_grRailParam Param { get => _s.GetReference<KAR_grRailParam>(0x00); set => _s.SetReference(0x00, value); }

        /// <summary>
        /// Spline to reference for collision
        /// </summary>
        public int StartSplineIndex { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        /// <summary>
        /// Spline to reference for total length
        /// </summary>
        public int SplineLengthIndex { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public int SubAnimIndex { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

        public int x10 { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        public int x14 { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }

        public int x18 { get => _s.GetInt32(0x18); set => _s.SetInt32(0x18, value); }

        public int x1C { get => _s.GetInt32(0x1C); set => _s.SetInt32(0x1C, value); }

        public int x20 { get => _s.GetInt32(0x20); set => _s.SetInt32(0x20, value); }

        public int x24 { get => _s.GetInt32(0x24); set => _s.SetInt32(0x24, value); }

        public int x28 { get => _s.GetInt32(0x28); set => _s.SetInt32(0x28, value); }

        public int x2C { get => _s.GetInt32(0x2C); set => _s.SetInt32(0x2C, value); }

        public int x30 { get => _s.GetInt32(0x30); set => _s.SetInt32(0x30, value); }
    }

    public class KAR_grRailParam : HSDAccessor
    {
        public override int TrimmedSize => 0x34;

        public int x00 { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public int Flags { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public float x08 { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public int AltRail1 { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

        public int AltRail2 { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        public HSDArrayAccessor<KAR_grRailDataParam> Data { get => _s.GetReference<HSDArrayAccessor<KAR_grRailDataParam>>(0x14); set => _s.SetReference(0x14, value); }

        public int DataCount { get => _s.GetInt32(0x18); set => _s.SetInt32(0x18, value); }

        public HSDArrayAccessor<KAR_grRailDashParam> Dash { get => _s.GetReference<HSDArrayAccessor<KAR_grRailDashParam>>(0x1C); set => _s.SetReference(0x1C, value); }

        public int DashCount { get => _s.GetInt32(0x20); set => _s.SetInt32(0x20, value); }

        public HSDArrayAccessor<KAR_grRailDashParam> Dash2 { get => _s.GetReference<HSDArrayAccessor<KAR_grRailDashParam>>(0x24); set => _s.SetReference(0x24, value); }

        public int Dash2Count { get => _s.GetInt32(0x28); set => _s.SetInt32(0x28, value); }

        public HSDArrayAccessor<KAR_grRailLeapParam> Leap { get => _s.GetReference<HSDArrayAccessor<KAR_grRailLeapParam>>(0x2C); set => _s.SetReference(0x2C, value); }


        public int LeapCount { get => _s.GetInt32(0x30); set => _s.SetInt32(0x30, value); }
    }

    public class KAR_grRailDataParam : HSDAccessor
    {
        public override int TrimmedSize => 0x0C;

        /// <summary>
        /// 
        /// </summary>
        public float Offset { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }

        /// <summary>
        /// 
        /// </summary>
        public float Speed1 { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        /// <summary>
        /// 
        /// </summary>
        public float Speed2 { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }
    }

    public class KAR_grRailDashParam : HSDAccessor
    {
        public override int TrimmedSize => 0x8;

        /// <summary>
        /// 
        /// </summary>
        public float Offset { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }

        /// <summary>
        /// GrCommon (0x584 + index * 4) // boost acceleration
        /// GrCommon (0x14 + index * 0x1C) // usually 0x1A
        /// </summary>
        public int Index { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }
    }

    public class KAR_grRailLeapParam : HSDAccessor
    {
        public override int TrimmedSize => 0x0C;

        /// <summary>
        /// 
        /// </summary>
        public float Offset { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }

        /// <summary>
        /// 
        /// </summary>
        public int RailIndex1 { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        /// <summary>
        /// 
        /// </summary>
        public int RailIndex2 { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }
    }

}
