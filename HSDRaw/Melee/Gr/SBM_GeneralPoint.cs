using HSDRaw.Common;

namespace HSDRaw.Melee.Gr
{
    public enum PointType
    {
        Player1Spawn = 0,
        Player2Spawn = 1,
        Player3Spawn = 2,
        Player4Spawn = 3,
        GlobalPlayerRespawn = 4,
        Unknown1 = 6,
        Unknown2 = 7,
        ItemSpawn1 = 127,
        ItemSpawn2 = 128,
        ItemSpawn3 = 129,
        ItemSpawn4 = 130,
        ItemSpawn5 = 131,
        ItemSpawn6 = 132,
        ItemSpawn7 = 133,
        ItemSpawn8 = 134,
        ItemSpawn9 = 135,
        ItemSpawn10 = 136,
        DeltaAngleCamera = 148,
        TopLeftBoundary = 149,
        BottomRightBoundary = 150,
        TopLeftBlastZone = 151,
        BottomRightBlastZone = 152,
        Target1 = 199,
        Target2 = 200,
        Target3 = 201,
        Target4 = 202,
        Target5 = 203,
        Target6 = 204,
        Target7 = 205,
        Target8 = 206,
        Target9 = 207,
        Target10 = 208,
    }

    public class SBM_GeneralPointInfo : HSDAccessor
    {
        public override int TrimmedSize => 0x4;

        public short JOBJIndex { get => _s.GetInt16(0x0); set => _s.SetInt16(0x0, value); }

        public PointType Type { get => (PointType)_s.GetInt16(0x2); set => _s.SetInt16(0x2, (short)value); }
    }

    public class SBM_GeneralPoints : HSDAccessor
    {
        public override int TrimmedSize => 0x0C;

        public HSD_JOBJ Points { get => _s.GetReference<HSD_JOBJ>(0x00); set => _s.SetReference(0x00, value); }

        public SBM_GeneralPointInfo[] Lights
        {
            get
            {
                var re = _s.GetReference<HSDArrayAccessor<SBM_GeneralPointInfo>>(0x4);
                if (re == null)
                    return null;
                return re.Array;
            }
            set
            {
                if (value == null)
                {
                    _s.SetInt32(0x8, 0);
                    _s.SetReference(0x4, null);
                }
                else
                {
                    _s.SetInt32(0x8, value.Length);
                    var re = _s.GetCreateReference<HSDArrayAccessor<SBM_GeneralPointInfo>>(0x4);
                    re.Array = value;
                }
            }
        }
    }
}
