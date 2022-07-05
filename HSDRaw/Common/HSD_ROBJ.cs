namespace HSDRaw.Common
{
    public enum REFTYPE
    {
        EXP = 0x00000000,
        JOBJ = 0x10000000,
        LIMIT = 0x20000000,
        BYTECODE = 0x30000000,
        IKHINT = 0x40000000,
    }

    public enum ROBJ_LIMIT
    {
        MIN_ROTX = 1,
        MAX_ROTX = 2,
        MIN_ROTY = 3,
        MAX_ROTY = 4,
        MIN_ROTZ = 5,
        MAX_ROTZ = 6,
        MIN_TRAX = 7,
        MAX_TRAX = 8,
        MIN_TRAY = 9,
        MAX_TRAY = 10,
        MIN_TRAZ = 11,
        MAX_TRAZ = 12,

    }

    /// <summary>
    /// Reference Object
    /// </summary>
    public class HSD_ROBJ : HSDListAccessor<HSD_ROBJ>
    {
        public override int TrimmedSize => 0x0C;

        public override HSD_ROBJ Next { get => _s.GetReference<HSD_ROBJ>(0x00); set => _s.SetReference(0x00, value); }

        public int Flags { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public REFTYPE RefType { get => (REFTYPE)(Flags & 0x70000000); set => Flags = (Flags & ~0x70000000) | ((int)value & 0x70000000); }

        public int LimitFlag { get => Flags & ~0x70000000; set => Flags = (Flags & 0x70000000) | (value & ~0x70000000); }

        // union

        public HSD_Exp Ref_Exp
        {
            get
            {
                if (RefType == REFTYPE.EXP)
                    return _s.GetReference<HSD_Exp>(0x08);

                return null;
            }
            set
            {
                if(value != null)
                {
                    RefType = REFTYPE.EXP;
                    _s.SetReference(0x08, value);
                }
            }
        }

        public HSD_ByteCodeExp Ref_ByteCodeExp
        {
            get
            {
                if (RefType == REFTYPE.BYTECODE)
                    return _s.GetReference<HSD_ByteCodeExp>(0x08);

                return null;
            }
            set
            {
                if (value != null)
                {
                    RefType = REFTYPE.BYTECODE;
                    _s.SetReference(0x08, value);
                }
            }
        }

        public HSD_IKHint Ref_IkHint
        {
            get
            {
                if (RefType == REFTYPE.IKHINT)
                    return _s.GetReference<HSD_IKHint>(0x08);

                return null;
            }
            set
            {
                if (value != null)
                {
                    RefType = REFTYPE.IKHINT;
                    _s.SetReference(0x08, value);
                }
            }
        }

        public HSD_JOBJ Ref_Joint
        {
            get
            {
                if (RefType == REFTYPE.JOBJ)
                    return _s.GetReference<HSD_JOBJ>(0x08);

                return null;
            }
            set
            {
                if (value != null)
                {
                    RefType = REFTYPE.JOBJ;
                    _s.SetReference(0x08, value);
                }
            }
        }
        
        public float Ref_Limit
        {
            get
            {
                if (RefType == REFTYPE.LIMIT)
                    return _s.GetFloat(0x08);

                return 0;
            }
            set
            {
                RefType = REFTYPE.LIMIT;
                _s.SetFloat(0x08, value);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HSD_Exp : HSDListAccessor<HSD_RValueList>
    {
        public override int TrimmedSize => 0x08;

        public float func { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }

        public HSD_RValueList rvalue { get => _s.GetReference<HSD_RValueList>(0x04); set => _s.SetReference(0x04, value); }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HSD_ByteCodeExp : HSDListAccessor<HSD_RValueList>
    {
        public override int TrimmedSize => 0x08;

        public byte ByteCode { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }

        public HSD_RValueList rvalue { get => _s.GetReference<HSD_RValueList>(0x04); set => _s.SetReference(0x04, value); }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HSD_RValueList : HSDListAccessor<HSD_RValueList>
    {
        public override int TrimmedSize => 0x0C;

        public int Flags { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }
        
        public HSD_JOBJ JOBJ { get => _s.GetReference<HSD_JOBJ>(0x04); set => _s.SetReference(0x04, value); }

        public override HSD_RValueList Next { get => _s.GetReference<HSD_RValueList>(0x08); set => _s.SetReference(0x08, value); }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HSD_IKHint : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public float BoneLength { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }

        public float RotateX { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }
    }
}
