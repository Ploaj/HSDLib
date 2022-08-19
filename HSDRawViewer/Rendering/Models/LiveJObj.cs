using HSDRaw.Common;
using OpenTK.Mathematics;
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
        public Matrix4 WorldTransform;
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
        private void SetDefaultSRT()
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
            if (c != null &&
                (Desc.Flags & (JOBJ_FLAG.BILLBOARD | JOBJ_FLAG.VBILLBOARD | JOBJ_FLAG.HBILLBOARD | JOBJ_FLAG.PBILLBOARD)) != 0)
            {
                var pos = Vector3.TransformPosition(Vector3.Zero, WorldTransform);
                var campos = (c.RotationMatrix * new Vector4(c.Translation, 1)).Xyz;

                if (Desc.Flags.HasFlag(JOBJ_FLAG.BILLBOARD))
                    WorldTransform = Matrix4.LookAt(pos, campos, Vector3.UnitY).Inverted();

                if (Desc.Flags.HasFlag(JOBJ_FLAG.VBILLBOARD))
                {
                    var temp = pos.Y;
                    pos.Y = 0;
                    campos.Y = 0;
                    WorldTransform = Matrix4.LookAt(pos, campos, Vector3.UnitY).Inverted() * Matrix4.CreateTranslation(0, temp, 0);
                }

                if (Desc.Flags.HasFlag(JOBJ_FLAG.HBILLBOARD))
                {
                    var temp = pos.X;
                    pos.X = 0;
                    campos.X = 0;
                    WorldTransform = Matrix4.LookAt(pos, campos, Vector3.UnitY).Inverted() * Matrix4.CreateTranslation(temp, 0, 0);
                }

                if (Desc.Flags.HasFlag(JOBJ_FLAG.RBILLBOARD))
                {
                    var temp = pos.Z;
                    pos.Z = 0;
                    campos.Z = 0;
                    WorldTransform = Matrix4.LookAt(pos, campos, Vector3.UnitY).Inverted() * Matrix4.CreateTranslation(0, 0, temp);
                }
            }

            // calculate the bind matrix
            BindTransform = InvertedTransform * WorldTransform;

            // process children
            if (Child != null && updateChildren)
                Child.RecalculateTransforms(c, updateChildren);

            // process siblings
            if (Sibling != null && updateChildren)
                Sibling.RecalculateTransforms(c, updateChildren);
        }
    }
}
