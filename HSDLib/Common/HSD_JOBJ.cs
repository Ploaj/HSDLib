using System;

namespace HSDLib.Common
{
    [Flags]
    public enum JOBJ_FLAG
    {
        SKELETON = (1 << 0),
        SKELETON_ROOT = (1 << 1),
        ENVELOPE_MODEL = (1 << 2),
        CLASSICAL_SCALING = (1 << 3),
        HIDDEN = (1 << 4),
        PTCL = (1 << 5),
        MTX_DIRTY = (1 << 6),
        LIGHTING = (1 << 7),
        TEXGEN = (1 << 8),
        BILLBOARD = (1 << 9),
        VBILLBOARD = (2 << 9),
        HBILLBOARD = (3 << 9),
        RBILLBOARD = (4 << 9),
        INSTANCE = (1 << 12),
        PBILLBOARD = (1 << 13),
        SPLINE = (1 << 14),
        FLIP_IK = (1 << 15),
        SPECULAR = (1 << 16),
        USE_QUATERNION = (1 << 17),
        OPA = (1 << 18),
        XLU = (1 << 19),
        TEXEDGE = (1 << 20),
        NULL = (0 << 21),
        JOINT1 = (1 << 21),
        JOINT2 = (2 << 21),
        EFFECTOR = (3 << 21),
        USER_DEFINED_MTX = (1 << 23),
        MTX_INDEPEND_PARENT = (1 << 24),
        MTS_INDEPEND_SRT = (1 << 25),
        ROOT_OPA = (1 << 28),
        ROOT_XLU = (1 << 29),
        ROOT_TEXEDGE = (1 << 30)
    }

    public class HSD_JOBJ : IHSDTree<HSD_JOBJ>
    {
        public uint NameOffset { get; set; }
        
        public JOBJ_FLAG Flags { get; set; }
        
        public override HSD_JOBJ Child { get; set; }
        
        public override HSD_JOBJ Next { get; set; }
        
        public HSD_DOBJ DOBJ { get; set; }

        [HSDInLine()]
        public HSD_Transforms Transforms { get; set; }
        
        public HSD_Matrix4x3 InverseMatrix { get; set; }
        
        public uint ROBJOffset { get; set; }

        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);
            if (ROBJOffset != 0)
                throw new Exception("Path in joint detected");
        }
    }
}
