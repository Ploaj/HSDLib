using System;

namespace HSDLib.Animation
{
    [Flags]
    public enum AOBJ_Flags
    {
        ANIM_REWINDED = (1<<26),
        FIRST_PLAY = (1<<27),
        NO_UPDATE = (1<<28),
        ANIM_LOOP = (1<<29),
        NO_ANIM = (1<<30)
    }

    public class HSD_AOBJ : IHSDNode
    {
        public AOBJ_Flags Flags { get; set; }
        
        public float EndFrame { get; set; }
        
        public FOBJDesc FObjDesc { get; set; }
        
        public uint PathAnimJoint { get; set; }

        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);
            if (PathAnimJoint != 0)
                throw new Exception("Error Reading AOBJ Path Detected");
        }
    }
}
