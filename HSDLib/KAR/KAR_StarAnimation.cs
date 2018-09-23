using HSDLib.Animation;
using HSDLib.MaterialAnimation;

namespace HSDLib.KAR
{
    public class KAR_StarAnimation : IHSDNode
    {
        [FieldData(typeof(HSD_FigaTree))]
        public HSD_FigaTree MovingAnim { get; set; }

        [FieldData(typeof(HSD_MatAnimJoint))]
        public HSD_MatAnimJoint MovingMatAnim { get; set; }

        [FieldData(typeof(HSD_FigaTree))]
        public HSD_FigaTree Unk1Anim { get; set; }

        [FieldData(typeof(HSD_MatAnimJoint))]
        public HSD_MatAnimJoint Unk1MatAnim { get; set; }

        [FieldData(typeof(HSD_FigaTree))]
        public HSD_FigaTree Unk2Anim { get; set; }

        [FieldData(typeof(HSD_MatAnimJoint))]
        public HSD_MatAnimJoint Unk2MatAnim { get; set; }

        [FieldData(typeof(HSD_FigaTree))]
        public HSD_FigaTree Unk3Anim { get; set; }

        [FieldData(typeof(HSD_MatAnimJoint))]
        public HSD_MatAnimJoint Unk3MatAnim { get; set; }

        [FieldData(typeof(HSD_FigaTree))]
        public HSD_FigaTree BoostAnim { get; set; }

        [FieldData(typeof(HSD_MatAnimJoint))]
        public HSD_MatAnimJoint BoostMatAnim { get; set; }
        
        [FieldData(typeof(HSD_FigaTree))]
        public HSD_FigaTree StopAnim { get; set; }

        [FieldData(typeof(HSD_MatAnimJoint))]
        public HSD_MatAnimJoint StopMatAnim { get; set; }
        
        [FieldData(typeof(int))]
        public int Unk1 { get; set; }

        [FieldData(typeof(int))]
        public int Unk2 { get; set; }

        [FieldData(typeof(int))]
        public int Unk3 { get; set; }

        [FieldData(typeof(int))]
        public int Unk4 { get; set; }

        [FieldData(typeof(int))]
        public int Unk5 { get; set; }

        [FieldData(typeof(int))]
        public int Unk6 { get; set; }

        [FieldData(typeof(int))]
        public int Unk7 { get; set; }

        [FieldData(typeof(int))]
        public int Unk8 { get; set; }

        [FieldData(typeof(int))]
        public int Unk9 { get; set; }

        [FieldData(typeof(int))]
        public int Unk10 { get; set; }

        [FieldData(typeof(int))]
        public int Unk11 { get; set; }

        [FieldData(typeof(float))]
        public float AnimFloat1 { get; set; }

        [FieldData(typeof(float))]
        public float AnimFloat2 { get; set; }

        [FieldData(typeof(float))]
        public float AnimFloat3 { get; set; }

        [FieldData(typeof(float))]
        public float AnimFloat4 { get; set; }

        [FieldData(typeof(float))]
        public float AnimFloat5 { get; set; }

        [FieldData(typeof(float))]
        public float AnimFloat6 { get; set; }

        [FieldData(typeof(int))]
        public int Unk12 { get; set; }

        [FieldData(typeof(int))]
        public int Unk13 { get; set; }
    }
}
