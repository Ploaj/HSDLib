using System;

namespace HSDRaw.Melee.Gr
{
    public enum CollMaterial
    {
        Basic       = 0,
        Rock        = 1,
        Grass       = 2,
        Dirt        = 3,
        Wood        = 4,
        LightMetal  = 5,
        HeavyMetal  = 6,
        UnkFlatZone = 7,
        AlienGoop   = 8,
        Unknown9    = 9,
        Water       = 10,
        Unknown11   = 11,
        Glass       = 12,
        GreatBay    = 13,
        Unknown14   = 14,
        Unknown15   = 15,
        FlatZone    = 16,
        Unknown17   = 17,
        Checkered   = 18
    }

    [Flags]
    public enum CollPhysics
    {
        Top             = 1,
        Bottom          = 2,
        Right           = 4,
        Left            = 8,
        Disabled        = 16
    }

    [Flags]
    public enum CollProperty
    {
        DropThrough = 1,
        LedgeGrab   = 2,
        Unknown     = 4 // used on some dynamic collisions (PS and RC)
    }

    public class SBM_CollVertex : HSDAccessor
    {
        public override int TrimmedSize => 8;
        public float X { get => _s.GetFloat(0); set => _s.SetFloat(0, value); }
        public float Y { get => _s.GetFloat(4); set => _s.SetFloat(4, value); }
    }

    public class SBM_CollLine : HSDAccessor
    {
        public override int TrimmedSize => 0x10;
        public short VertexIndex1 { get => _s.GetInt16(0x00); set => _s.SetInt16(0x00, value); }
        public short VertexIndex2 { get => _s.GetInt16(0x02); set => _s.SetInt16(0x02, value); }
        public short NextLine { get => _s.GetInt16(0x04); set => _s.SetInt16(0x04, value); }
        public short PreviousLine { get => _s.GetInt16(0x06); set => _s.SetInt16(0x06, value); }
        public short NextLineAltGroup { get => _s.GetInt16(0x08); set => _s.SetInt16(0x08, value); } // -1
        public short PreviousLineAltGroup { get => _s.GetInt16(0x0A); set => _s.SetInt16(0x0A, value); } // -1
        public CollPhysics CollisionFlag { get => (CollPhysics)_s.GetInt16(0x0C); set => _s.SetInt16(0x0C, (short)value); }
        public CollProperty Flag { get => (CollProperty)_s.GetByte(0x0E); set => _s.SetByte(0x0E, (byte)value); }
        public CollMaterial Material { get => (CollMaterial)_s.GetByte(0x0F); set => _s.SetByte(0x0F, (byte)value); }
    }

    public class SBM_CollLineGroup : HSDAccessor
    {
        public override int TrimmedSize => 0x28;

        public short TopLineIndex { get => _s.GetInt16(0x00); set => _s.SetInt16(0x00, value); }
        public short TopLineCount { get => _s.GetInt16(0x02); set => _s.SetInt16(0x02, value); }

        public short BottomLineIndex { get => _s.GetInt16(0x04); set => _s.SetInt16(0x04, value); }
        public short BottomLineCount { get => _s.GetInt16(0x06); set => _s.SetInt16(0x06, value); }

        public short RightLineIndex { get => _s.GetInt16(0x08); set => _s.SetInt16(0x08, value); }
        public short RightLineCount { get => _s.GetInt16(0x0A); set => _s.SetInt16(0x0A, value); }

        public short LeftLineIndex { get => _s.GetInt16(0x0C); set => _s.SetInt16(0x0C, value); }
        public short LeftLineCount { get => _s.GetInt16(0x0E); set => _s.SetInt16(0x0E, value); }

        public short DynamicLineIndex { get => _s.GetInt16(0x10); set => _s.SetInt16(0x10, value); }
        public short DynamicLineCount { get => _s.GetInt16(0x12); set => _s.SetInt16(0x12, value); }

        public float XMin { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }
        public float YMin { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }
        public float XMax { get => _s.GetFloat(0x1C); set => _s.SetFloat(0x1C, value); }
        public float YMax { get => _s.GetFloat(0x20); set => _s.SetFloat(0x20, value); }

        public short VertexStart { get => _s.GetInt16(0x24); set => _s.SetInt16(0x24, value); }
        public short VertexCount { get => _s.GetInt16(0x26); set => _s.SetInt16(0x26, value); }
    }

    public class SBM_Coll_Data : HSDAccessor
    {
        public override int TrimmedSize => 0x2C;
        
        public SBM_CollVertex[] Vertices
        {
            get
            {
                var re = _s.GetReference<HSDArrayAccessor<SBM_CollVertex>>(0x00);
                if (re == null)
                    return null;
                return re.Array;
            }
            set
            {
                if(value == null)
                {
                    _s.SetInt32(0x04, 0);
                    _s.SetReference(0x00, null);
                }
                else
                {
                    _s.SetInt32(0x04, value.Length);
                    var re = _s.GetCreateReference<HSDArrayAccessor<SBM_CollVertex>>(0x00);
                    re.Array = value;
                }
            }
        }

        public SBM_CollLine[] Links
        {
            get
            {
                var re = _s.GetReference<HSDArrayAccessor<SBM_CollLine>>(0x08);
                if (re == null)
                    return null;
                return re.Array;
            }
            set
            {
                if (value == null)
                {
                    _s.SetInt32(0x0C, 0);
                    _s.SetReference(0x08, null);
                }
                else
                {
                    _s.SetInt32(0x0C, value.Length);
                    var re = _s.GetCreateReference<HSDArrayAccessor<SBM_CollLine>>(0x08);
                    re.Array = value;
                }
            }
        }

        public short TopLinksOffset { get => _s.GetInt16(0x10); set => _s.SetInt16(0x10, value); }
        public short TopLinksCount { get => _s.GetInt16(0x12); set => _s.SetInt16(0x12, value); }
        public short BottomLinksOffset { get => _s.GetInt16(0x14); set => _s.SetInt16(0x14, value); }
        public short BottomLinksCount { get => _s.GetInt16(0x16); set => _s.SetInt16(0x16, value); }
        public short RightLinksOffset { get => _s.GetInt16(0x18); set => _s.SetInt16(0x18, value); }
        public short RightLinksCount { get => _s.GetInt16(0x1A); set => _s.SetInt16(0x1A, value); }
        public short LeftLinksOffset { get => _s.GetInt16(0x1C); set => _s.SetInt16(0x1C, value); }
        public short LeftLinksCount { get => _s.GetInt16(0x1E); set => _s.SetInt16(0x1E, value); }
        public short DynamicLinksOffset { get => _s.GetInt16(0x20); set => _s.SetInt16(0x20, value); }
        public short DynamicLinksCount { get => _s.GetInt16(0x22); set => _s.SetInt16(0x22, value); }
        
        public SBM_CollLineGroup[] LineGroups
        {
            get
            {
                var re = _s.GetReference<HSDArrayAccessor<SBM_CollLineGroup>>(0x24);
                if (re == null)
                    return null;
                return re.Array;
            }
            set
            {
                if (value == null)
                {
                    _s.SetInt32(0x28, 0);
                    _s.SetReference(0x24, null);
                }
                else
                {
                    _s.SetInt32(0x28, value.Length);
                    var re = _s.GetCreateReference<HSDArrayAccessor<SBM_CollLineGroup>>(0x24);
                    re.Array = value;
                }
            }
        }
        
    }
}
