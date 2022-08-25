using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace HSDRawViewer.Rendering.Models
{
    // Live Object
    public class LiveJObj
    {
        public int Index { get; internal set; }

        public bool PhysicsEnabled { get; set; } = false;

        public bool Visible { get; set; } = true;

        public bool BranchVisible
        {
            get
            {
                if (Visible)
                    return true;

                if (Parent != null)
                    return Parent.BranchVisible;

                return Visible;
            }
        }

        public HSD_JOBJ Desc { get; internal set; }

        public LiveJObj Parent;
        public LiveJObj Child;
        public LiveJObj Sibling;

        public LiveJObj Root
        {
            get
            {
                if (Parent != null)
                    return Parent.Root;

                return this;
            }
        }

        public Vector3 Translation;
        public Vector4 Rotation;
        public Vector3 Scale;

        public Matrix4 LocalTransform;
        public Matrix4 WorldTransform { get => _worldTransform; set { _worldTransform = value; BindTransform = InvertedTransform * value; } }
        private Matrix4 _worldTransform;
        private Matrix4 InvertedTransform;
        public Matrix4 BindTransform;

        public int JointCount
        {
            get
            {
                int i = 1;

                if (Child != null)
                    i += Child.JointCount;

                if (Sibling != null)
                    i += Sibling.JointCount;

                return i;
            }
        }

        public IEnumerable<LiveJObj> Enumerate
        {
            get
            {
                yield return this;

                if (Child != null)
                    foreach (var c in Child.Enumerate)
                        yield return c;

                if (Sibling != null)
                    foreach (var c in Sibling.Enumerate)
                        yield return c;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="desc"></param>
        public LiveJObj(HSD_JOBJ desc)
        {
            int i = 0;
            Init(desc, null, ref i);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="desc"></param>
        public LiveJObj(HSD_JOBJ desc, LiveJObj parent, ref int i)
        {
            Init(desc, parent, ref i);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="parent"></param>
        private void Init(HSD_JOBJ desc, LiveJObj parent, ref int i)
        {
            Index = i++;
            Parent = parent;

            Desc = desc;
            SetDefaultSRT();
            Visible = !desc.Flags.HasFlag(JOBJ_FLAG.HIDDEN);

            // recalcuate transforms
            RecalculateTransforms(null, false);

            // inverse bind
            InvertedTransform = WorldTransform.Inverted();

            // inverse bind can be set from jobj as well
            if ((desc.Flags.HasFlag(JOBJ_FLAG.SKELETON) || desc.Flags.HasFlag(JOBJ_FLAG.SKELETON_ROOT)) &&
                desc.InverseWorldTransform != null)
            {
                InvertedTransform = desc.InverseWorldTransform.ToTKMatrix();
            }

            // initialize children
            if (desc.Child != null)
            {
                Child = new LiveJObj(desc.Child, this, ref i);
            }
            if (desc.Next != null)
            {
                Sibling = new LiveJObj(desc.Next, Parent, ref i);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        public LiveJObj AddChild(HSD_JOBJ desc)
        {
            if (Child == null)
            {
                Child = new LiveJObj(desc);
                this.Desc.Child = desc;
                return Child;
            }
            else
            {
                return Child.AddSibling(desc);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        public LiveJObj AddSibling(HSD_JOBJ desc)
        {
            if (Sibling == null)
            {
                Sibling = new LiveJObj(desc);
                this.Desc.Next = desc;
                return Sibling;
            }
            else
            {
                return Sibling.AddSibling(desc);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public LiveJObj GetJObjFromDesc(HSD_JOBJ jobj)
        {
            if (Desc == jobj)
                return this;

            if (Child != null)
            {
                var c = Child.GetJObjFromDesc(jobj);
                if (c != null)
                    return c;
            }

            if (Sibling != null)
            {
                var s = Sibling.GetJObjFromDesc(jobj);
                if (s != null)
                    return s;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public LiveJObj GetJObjAtIndex(int index)
        {
            if (Index == index)
                return this;

            if (Child != null)
            {
                var c = Child.GetJObjAtIndex(index);
                if (c != null)
                    return c;
            }

            if (Sibling != null)
            {
                var s = Sibling.GetJObjAtIndex(index);
                if (s != null)
                    return s;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="desc"></param>
        /// <returns></returns>
        public int GetIndexOfDesc(HSD_JOBJ desc)
        {
            int i = 0;
            foreach (var v in Enumerate)
            {
                if (v.Desc == desc)
                    return i;
                i++;
            }
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetDefaultSRT()
        {
            Translation = new Vector3(Desc.TX, Desc.TY, Desc.TZ);
            Rotation = new Vector4(Desc.RX, Desc.RY, Desc.RZ, 0);
            Scale = new Vector3(Desc.SX, Desc.SY, Desc.SZ);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void ResetTransforms()
        {
            foreach (var v in Enumerate)
                v.SetDefaultSRT();

            RecalculateTransforms(null, true);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RecalculateInverseBinds()
        {
            foreach (var v in Enumerate)
                v.SetDefaultSRT();

            RecalculateTransforms(null, true);

            foreach (var v in Enumerate)
                if (v.Desc.Flags.HasFlag(JOBJ_FLAG.SKELETON) || v.Desc.Flags.HasFlag(JOBJ_FLAG.SKELETON_ROOT))
                    v.Desc.InverseWorldTransform = v.WorldTransform.Inverted().ToHsdMatrix();
        }
        /// <summary>
        /// 
        /// </summary>
        public void RecalculateTransforms(Camera c, bool updateChildren)
        {
            // calculate local transform
            LocalTransform = Matrix4.CreateScale(Scale) *
                Math3D.CreateMatrix4FromEuler(Rotation.Xyz) *
                Matrix4.CreateTranslation(Translation);

            // parent transform processing
            if (Parent != null)
            {
                // custom matrix compensate
                if (Desc.Flags.HasFlag(JOBJ_FLAG.MTX_SCALE_COMPENSATE))
                {
                    // get parent scale
                    var parentScale = Parent.LocalTransform.ExtractScale();

                    // get animated vectors
                    // GetLocalTransformValues(root, out Vector3 trans, out Quaternion rot, out Vector3 scale, index);

                    // scale the position only
                    var trans = Translation * parentScale;

                    // build the local matrix
                    LocalTransform = Matrix4.CreateScale(Scale) *
                                    Math3D.CreateMatrix4FromEuler(Rotation.Xyz) *
                                    Matrix4.CreateTranslation(trans);

                    // calculate the compensate scale
                    var scaleCompensation = Matrix4.CreateScale(1f / parentScale.X, 1f / parentScale.Y, 1f / parentScale.Z);

                    // apply scale compensate
                    WorldTransform = LocalTransform * scaleCompensation * Parent.WorldTransform;
                }
                else
                {
                    // multiply parent transform
                    WorldTransform = LocalTransform * Parent.WorldTransform;
                }
            }
            else
            {
                WorldTransform = LocalTransform;
            }

            // calculate billboarding
            if (c != null)
            {
                var pos = Vector3.TransformPosition(Vector3.Zero, WorldTransform);
                var campos = c.TransformedPosition;

                int billboard_type = (((int)Desc.Flags >> 9) & 0x7);

                switch (billboard_type)
                {
                    case 1:
                        {
                            mkBillBoardMtx(c.ModelViewMatrix);
                        }
                        break;
                    case 2:
                        {
                            mkVBillBoardMtx(c.ModelViewMatrix);
                        }
                        break;
                    case 3:
                        {
                            mkHBillBoardMtx(c.ModelViewMatrix);
                        }
                        break;
                    case 4:
                        {
                            mkRBillBoardMtx(c.ModelViewMatrix);
                        }
                        break;
                }
            }

            // calculate the bind matrix
            // BindTransform = InvertedTransform * WorldTransform;

            // process children
            if (Child != null && updateChildren)
                Child.RecalculateTransforms(c, updateChildren);

            // process siblings
            if (Sibling != null && updateChildren)
                Sibling.RecalculateTransforms(c, updateChildren);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mtx"></param>
        /// <param name="pmtx"></param>
        private void mkBillBoardMtx(Matrix4 mv)
        {
            var mtx = WorldTransform * mv;

            Vector3 ax;
            Vector3 ay = new Vector3(mtx.M21, mtx.M22, mtx.M23);
            Vector3 az;

            float sx = new Vector3(mtx.M11, mtx.M12, mtx.M13).LengthFast;
            float sz = new Vector3(mtx.M31, mtx.M32, mtx.M33).LengthFast;
            float sy = ay.LengthFast;

            Vector3 pos = new Vector3(mtx.M41, mtx.M42, mtx.M43);

            if (Desc.Flags.HasFlag(JOBJ_FLAG.PBILLBOARD))
            {
                az = pos;
                ax = Vector3.Cross(az, ay);
                ay = Vector3.Cross(ax, az);
                sz /= az.LengthFast;
            }
            else
            {
                az = Vector3.UnitZ;
                ax = Vector3.Cross(ay, az);
                ay = Vector3.Cross(az, ax);
                sz /= az.LengthFast;
            }

            sx = sx / ax.LengthFast;
            sy = sy / ay.LengthFast;

            WorldTransform = new Matrix4()
            {
                M11 = sx * ax.X,
                M12 = sx * ax.Y,
                M13 = sx * ax.Z,

                M21 = sy * ay.X,
                M22 = sy * ay.Y,
                M23 = sy * ay.Z,

                M31 = sz * az.X,
                M32 = sz * az.Y,
                M33 = sz * az.Z,

                M41 = pos.X,
                M42 = pos.Y,
                M43 = pos.Z,
                M44 = 1,
            } * mv.Inverted();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mtx"></param>
        /// <param name="pmtx"></param>
        private void mkHBillBoardMtx(Matrix4 mv)
        {
            var mtx = WorldTransform * mv;

            Vector3 pos = new Vector3(mtx.M41, mtx.M42, mtx.M43);

            Vector3 ax = new Vector3(mtx.M11, mtx.M12, mtx.M13);
            Vector3 ay;
            Vector3 az;

            float sy = new Vector3(mtx.M21, mtx.M22, mtx.M23).LengthFast;
            float sz = new Vector3(mtx.M31, mtx.M32, mtx.M33).LengthFast;

            if (Desc.Flags.HasFlag(JOBJ_FLAG.PBILLBOARD))
            {
                float pos_mag = pos.LengthFast;
                Vector3 l = new Vector3(
                    pos.X * (-pos.Y / pos_mag),
                    pos_mag,
                    pos.Z * (-pos.Y / pos_mag));
                az = Vector3.Cross(ax, l);
            }
            else
            {
                az = Vector3.Cross(ax, Vector3.UnitY);
            }

            ay = Vector3.Cross(az, ax);
            sy = sy / ay.LengthFast;
            sz = sz / az.LengthFast;

            WorldTransform = new Matrix4()
            {
                M11 = ax.X,
                M12 = ax.Y,
                M13 = ax.Z,

                M21 = sy * ay.X,
                M22 = sy * ay.Y,
                M23 = sy * ay.Z,

                M31 = sz * az.X,
                M32 = sz * az.Y,
                M33 = sz * az.Z,

                M41 = pos.X,
                M42 = pos.Y,
                M43 = pos.Z,
                M44 = 1,
            } * mv.Inverted();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mtx"></param>
        /// <param name="pmtx"></param>
        private void mkVBillBoardMtx(Matrix4 mv)
        {
            var mtx = WorldTransform * mv;

            Vector3 pos = new Vector3(mtx.M41, mtx.M42, mtx.M43);

            Vector3 ax;
            Vector3 ay = new Vector3(mtx.M21, mtx.M22, mtx.M23);
            Vector3 az;

            float sx = new Vector3(mtx.M11, mtx.M12, mtx.M13).LengthFast;
            float sz = new Vector3(mtx.M31, mtx.M32, mtx.M33).LengthFast;

            if (Desc.Flags.HasFlag(JOBJ_FLAG.PBILLBOARD))
            {
                ax = Vector3.Cross(pos, ay);
            }
            else
            {
                ax = Vector3.Cross(ay, Vector3.UnitZ);
            }

            az = Vector3.Cross(ax, ay);

            sx = sx / ax.LengthFast;
            sz = sz / az.LengthFast;


            WorldTransform = new Matrix4()
            {
                M11 = sx * ax.X,
                M12 = sx * ax.Y,
                M13 = sx * ax.Z,

                M21 = ay.X,
                M22 = ay.Y,
                M23 = ay.Z,

                M31 = sz * az.X,
                M32 = sz * az.Y,
                M33 = sz * az.Z,

                M41 = pos.X,
                M42 = pos.Y,
                M43 = pos.Z,
                M44 = 1,
            } * mv.Inverted();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mtx"></param>
        /// <param name="pmtx"></param>
        private void mkRBillBoardMtx(Matrix4 mv)
        {
            var mtx = WorldTransform * mv;

            float x = new Vector3(mtx.M11, mtx.M12, mtx.M13).LengthFast;
            float y = new Vector3(mtx.M21, mtx.M22, mtx.M23).LengthFast;
            float z = new Vector3(mtx.M31, mtx.M32, mtx.M33).LengthFast;

            var scamtx = Matrix4.CreateScale(x, y, z);

            var rotmtx = Matrix4.CreateRotationZ(Rotation.Z);
            rotmtx.M41 = mtx.M41;
            rotmtx.M42 = mtx.M42;
            rotmtx.M43 = mtx.M43;

            WorldTransform = rotmtx * scamtx * mv.Inverted();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tracks"></param>
        /// <param name="frame"></param>
        public void ApplyAnimation(List<FOBJ_Player> tracks, float frame)
        {
            foreach (FOBJ_Player t in tracks)
            {
                switch (t.JointTrackType)
                {
                    case JointTrackType.HSD_A_J_ROTX: Rotation.X = t.GetValue(frame); break;
                    case JointTrackType.HSD_A_J_ROTY: Rotation.Y = t.GetValue(frame); break;
                    case JointTrackType.HSD_A_J_ROTZ: Rotation.Z = t.GetValue(frame); break;
                    case JointTrackType.HSD_A_J_TRAX: Translation.X = t.GetValue(frame); break;
                    case JointTrackType.HSD_A_J_TRAY: Translation.Y = t.GetValue(frame); break;
                    case JointTrackType.HSD_A_J_TRAZ: Translation.Z = t.GetValue(frame); break;
                    case JointTrackType.HSD_A_J_SCAX: Scale.X = t.GetValue(frame); break;
                    case JointTrackType.HSD_A_J_SCAY: Scale.Y = t.GetValue(frame); break;
                    case JointTrackType.HSD_A_J_SCAZ: Scale.Z = t.GetValue(frame); break;
                }
            }
        }
    }
}
