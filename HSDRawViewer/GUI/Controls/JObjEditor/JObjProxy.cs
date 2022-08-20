using HSDRaw.Common;
using System.ComponentModel;

namespace HSDRawViewer.GUI.Controls.JObjEditor
{
    public enum JObjBillboardType
    {
        None,
        Both,
        Horizontal,
        Vertical,
        View,
    }

    public class JObjSplineProxy : JObjProxy
    {
        public JObjSplineProxy(HSD_JOBJ jobj) : base(jobj)
        {
        }

    }

    public class JObjParticleProxy : JObjProxy
    {
        [DisplayName("Particle Bank"), Category("General")]
        public byte ParticleBank { get => jobj.ParticleJoint.ParticleBank; set => jobj.ParticleJoint.ParticleBank = value; }


        [DisplayName("Particle ID"), Category("General")]
        public int ParticleID { get => jobj.ParticleJoint.ParticleID; set => jobj.ParticleJoint.ParticleID = value; }

        public JObjParticleProxy(HSD_JOBJ jobj) : base(jobj)
        {
        }

    }

    public class JObjProxy
    {
        public HSD_JOBJ jobj;

        [DisplayName("Class Name"), Category("General")]
        public string Name { get => jobj.ClassName; set => jobj.ClassName = value; }

        [DisplayName("Use Classical Scale"), Category("General")]
        public bool ClassicalScale
        {
            get => jobj.Flags.HasFlag(JOBJ_FLAG.CLASSICAL_SCALING);
            set
            {
                if (value)
                    jobj.Flags |= JOBJ_FLAG.CLASSICAL_SCALING;
                else
                    jobj.Flags &= ~JOBJ_FLAG.CLASSICAL_SCALING;
            }
        }

        [DisplayName("Billboard Type"), Category("General")]
        public JObjBillboardType Billboard
        {
            get => (JObjBillboardType)(((int)jobj.Flags >> 9) & 0x7);
            set
            {
                jobj.Flags &= (JOBJ_FLAG)~(0x7 << 9);
                jobj.Flags |= (JOBJ_FLAG)(((int)value & 0x7) << 9);
            }
        }

        [DisplayName("Hidden"), Category("General")]
        public bool Hidden
        {
            get => jobj.Flags.HasFlag(JOBJ_FLAG.HIDDEN);
            set
            {
                if (value)
                    jobj.Flags |= JOBJ_FLAG.HIDDEN;
                else
                    jobj.Flags &= ~JOBJ_FLAG.HIDDEN;
            }
        }

        // custom
        // MTX_SCALE_COMPENSATE = (1 << 26),

        [DisplayName("Translation X"), Category("Transforms")]
        public float X { get => jobj.TX; set => jobj.TX = value; }

        [DisplayName("Translation Y"), Category("Transforms")]
        public float Y { get => jobj.TY; set => jobj.TY = value; }

        [DisplayName("Translation Z"), Category("Transforms")]
        public float Z { get => jobj.TZ; set => jobj.TZ = value; }


        [DisplayName("Rotation X"), Category("Transforms")]
        public float RX { get => jobj.RX; set => jobj.RX = value; }

        [DisplayName("Rotation Y"), Category("Transforms")]
        public float RY { get => jobj.RY; set => jobj.RY = value; }

        [DisplayName("Rotation Z"), Category("Transforms")]
        public float RZ { get => jobj.RZ; set => jobj.RZ = value; }


        [DisplayName("Scale X"), Category("Transforms")]
        public float SX { get => jobj.SX; set => jobj.SX = value; }

        [DisplayName("Scale Y"), Category("Transforms")]
        public float SY { get => jobj.SY; set => jobj.SY = value; }

        [DisplayName("Scale Z"), Category("Transforms")]
        public float SZ { get => jobj.SZ; set => jobj.SZ = value; }


        public JObjProxy(HSD_JOBJ jobj)
        {
            this.jobj = jobj;
        }
    }
}
