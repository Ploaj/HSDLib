using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDLib.MaterialAnimation
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
        [FieldData(typeof(AOBJ_Flags))]
        public AOBJ_Flags Flags { get; set; }

        [FieldData(typeof(float))]
        public float EndFrame { get; set; }

        [FieldData(typeof(FOBJDesc))]
        public FOBJDesc FObjDesc { get; set; }

        [FieldData(typeof(uint))]
        public uint PathAnimJoint { get; set; }

        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);
            if (PathAnimJoint != 0)
                throw new Exception("Error Reading AOBJ Path Detected");
        }
    }
}
