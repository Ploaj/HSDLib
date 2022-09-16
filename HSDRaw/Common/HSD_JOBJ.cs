using HSDRaw.Common.Animation;
using System;
using System.ComponentModel;
using System.Text;

namespace HSDRaw.Common
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
        MTX_INDEPEND_SRT = (1 << 25),
        ROOT_OPA = (1 << 28),
        ROOT_XLU = (1 << 29),
        ROOT_TEXEDGE = (1 << 30),

        // custom
        MTX_SCALE_COMPENSATE = (1 << 26),
    }

    public class HSD_JOBJ : HSDTreeAccessor<HSD_JOBJ>
    {
        public override int TrimmedSize { get; } = 0x40;

        /// <summary>
        /// Used for class lookup, but you can put whatever you want here
        /// </summary>
        public string ClassName
        {
            get => _s.GetString(0x00);
            set => _s.SetString(0x00, value);
        }

        public JOBJ_FLAG Flags 
        { 
            get => (JOBJ_FLAG)_s.GetInt32(0x04); 
            set => _s.SetInt32(0x04, (int)value);
        }

        public override HSD_JOBJ Child { get => _s.GetReference<HSD_JOBJ>(0x08); set => _s.SetReference(0x08, value); }

        public override HSD_JOBJ Next { get => _s.GetReference<HSD_JOBJ>(0x0C); set => _s.SetReference(0x0C, value); }
        
        public HSD_DOBJ Dobj { get => !Flags.HasFlag(JOBJ_FLAG.SPLINE) && !Flags.HasFlag(JOBJ_FLAG.PTCL) ? _s.GetReference<HSD_DOBJ>(0x10) : null; set { _s.SetReference(0x10, value); Flags &= ~JOBJ_FLAG.SPLINE; Flags &= ~JOBJ_FLAG.PTCL; } }

        
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public HSD_Spline Spline { get => Flags.HasFlag(JOBJ_FLAG.SPLINE) ? _s.GetReference<HSD_Spline>(0x10) : null; set { _s.SetReference(0x10, value); Flags |= JOBJ_FLAG.SPLINE; } }

        
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public HSD_ParticleJoint ParticleJoint { get => Flags.HasFlag(JOBJ_FLAG.PTCL) ? _s.GetReference<HSD_ParticleJoint>(0x10) : null; set { _s.SetReference(0x10, value); Flags |= JOBJ_FLAG.PTCL; } }

        public float RX { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }
        public float RY { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }
        public float RZ { get => _s.GetFloat(0x1C); set => _s.SetFloat(0x1C, value); }
        public float SX { get => _s.GetFloat(0x20); set => _s.SetFloat(0x20, value); }
        public float SY { get => _s.GetFloat(0x24); set => _s.SetFloat(0x24, value); }
        public float SZ { get => _s.GetFloat(0x28); set => _s.SetFloat(0x28, value); }
        public float TX { get => _s.GetFloat(0x2C); set => _s.SetFloat(0x2C, value); }
        public float TY { get => _s.GetFloat(0x30); set => _s.SetFloat(0x30, value); }
        public float TZ { get => _s.GetFloat(0x34); set => _s.SetFloat(0x34, value); }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public HSD_Matrix4x3 InverseWorldTransform { get => _s.GetReference<HSD_Matrix4x3>(0x38); set => _s.SetReference(0x38, value); }

        public HSD_ROBJ ROBJ { get => _s.GetReference<HSD_ROBJ>(0x3C); set => _s.SetReference(0x3C, value); }

        protected override int Trim()
        {
            // quit optimizing these away
            _s.CanBeBuffer = false;

            return base.Trim();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public float GetDefaultValue(JointTrackType type)
        {
            switch (type)
            {
                case JointTrackType.HSD_A_J_TRAX: return TX;
                case JointTrackType.HSD_A_J_TRAY: return TY;
                case JointTrackType.HSD_A_J_TRAZ: return TZ;
                case JointTrackType.HSD_A_J_ROTX: return RX;
                case JointTrackType.HSD_A_J_ROTY: return RY;
                case JointTrackType.HSD_A_J_ROTZ: return RZ;
                case JointTrackType.HSD_A_J_SCAX: return SX;
                case JointTrackType.HSD_A_J_SCAY: return SY;
                case JointTrackType.HSD_A_J_SCAZ: return SZ;
            }
            return 0;
        }

        /// <summary>
        /// Autometically sets needed flags for self and all children
        /// </summary>
        public void UpdateFlags()
        {
            UpdateFlags(true);
        }

        /// <summary>
        /// Autometically sets needed flags for self and all children
        /// </summary>
        private void UpdateFlags(bool isRoot)
        {
            // process child
            if (Child != null)
                Child.UpdateFlags(false);

            // process sibling
            if (Next != null)
                Next.UpdateFlags(false);

            // check dobj flags
            if (Dobj != null)
            {
                bool xlu = false;
                bool opa = false;
                bool lighting = false;
                bool specular = false;
                bool envelope = false;

                foreach (var dobj in Dobj.List)
                {
                    var mobj = dobj.Mobj;
                    if (mobj != null)
                    {
                        // get xlu or opa
                        if (mobj.RenderFlags.HasFlag(RENDER_MODE.XLU))
                            xlu = true;
                        else
                            opa = true;

                        // check if lighting is enabled
                        if (mobj.RenderFlags.HasFlag(RENDER_MODE.DIFFUSE))
                            lighting = true;

                        // check if specular is enabled
                        if (mobj.RenderFlags.HasFlag(RENDER_MODE.SPECULAR))
                            specular = true;
                    }

                    // check if model is enveloped
                    if (dobj.Pobj != null)
                    {
                        foreach (var pobj in dobj.Pobj.List)
                        {
                            if (pobj.Flags.HasFlag(POBJ_FLAG.ENVELOPE))
                                envelope = true;
                        }
                    }
                }

                SetFlag(JOBJ_FLAG.OPA, opa);
                SetFlag(JOBJ_FLAG.XLU | JOBJ_FLAG.TEXEDGE, xlu);
                SetFlag(JOBJ_FLAG.SPECULAR, specular);
                SetFlag(JOBJ_FLAG.LIGHTING, lighting);
                SetFlag(JOBJ_FLAG.ENVELOPE_MODEL, envelope);

                //if (xlu)
                //    Flags |= JOBJ_FLAG.XLU | JOBJ_FLAG.TEXEDGE;
                //else
                //    Flags &= ~JOBJ_FLAG.XLU;
            }


            // check if this joint is part of the skeleton
            SetFlag(JOBJ_FLAG.SKELETON, InverseWorldTransform != null);

            // set root flags
            SetFlag(JOBJ_FLAG.ROOT_XLU, ChildHasFlag(Child, JOBJ_FLAG.XLU));
            SetFlag(JOBJ_FLAG.ROOT_OPA, ChildHasFlag(Child, JOBJ_FLAG.OPA));
            SetFlag(JOBJ_FLAG.ROOT_TEXEDGE, ChildHasFlag(Child, JOBJ_FLAG.TEXEDGE));

            // TODO: if this joint has any mesh that are not single bound
            if (isRoot && ChildHasFlag(Child, JOBJ_FLAG.SKELETON))
                SetFlag(JOBJ_FLAG.SKELETON_ROOT, true);
            // SetFlag(JOBJ_FLAG.SKELETON_ROOT, isRoot && ChildHasFlag(Child, JOBJ_FLAG.SKELETON));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        private static bool ChildHasFlag(HSD_JOBJ jobj, JOBJ_FLAG flag)
        {
            // joint is null
            if (jobj == null)
                return false;

            // joint has flag
            if (jobj.Flags.HasFlag(flag))
                return true;

            // check if child has flag
            if (jobj.Child != null && ChildHasFlag(jobj.Child, flag))
                return true;

            // check if sibling has flag
            if (jobj.Next != null && ChildHasFlag(jobj.Next, flag))
                return true;

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="value"></param>
        private void SetFlag(JOBJ_FLAG flag, bool value)
        {
            if (value)
                Flags |= flag;
            else
                Flags &= ~flag;
        }
    }
}
