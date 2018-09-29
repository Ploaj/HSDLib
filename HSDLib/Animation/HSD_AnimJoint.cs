using System;

namespace HSDLib.Animation
{
    public class HSD_AnimJoint : IHSDTree<HSD_AnimJoint>
    {
        [FieldData(typeof(HSD_AnimJoint))]
        public override HSD_AnimJoint Child { get; set; }

        [FieldData(typeof(HSD_AnimJoint))]
        public override HSD_AnimJoint Next { get; set; }

        [FieldData(typeof(HSD_AOBJ))]
        public HSD_AOBJ AOBJ { get; set; }

        [FieldData(typeof(uint))]
        public uint ROBJPointer { get; set; }

        [FieldData(typeof(uint))]
        public uint Flags { get; set; }

        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);

            if (ROBJPointer != 0)
                Console.WriteLine("TODO: ROBJ in AnimJoint 0x" + ROBJPointer.ToString("X"));
        }
    }
}
