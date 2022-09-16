using HSDRaw.Common;
using OpenTK.Mathematics;
using System;

namespace HSDRawViewer.Rendering.Models
{
    internal class IKSolver
    {

        /// <summary>
        /// 
        /// </summary>
        public static void ResolveIKJoint1(LiveJObj jobj)
        {
            float dot;
            float length_squared;
            float axis_scale;
            float scale_squared = 0;
            float var_f27 = 0;
            Vector3 sp74 = Vector3.Zero;

            Vector3 axis;
            Vector3 rvec;
            Vector3 fvec = Vector3.Zero;

            bool has_flag = false;
            float x_scale = 0.0F;
            Vector3 trans = Vector3.Zero;
            Vector3 joint_scale = Vector3.One;

            // get parent scale
            if (jobj.Parent != null)
                joint_scale = jobj.Parent.WorldTransform.ExtractScale();

            // get robj ikhint
            HSD_ROBJ robj = RObjGetByType(jobj, REFTYPE.IKHINT, 0);
            var ik_hint = robj.Ref_IkHint;
            float rotate_angle = ik_hint.RotateX;
            float bone_length = ik_hint.BoneLength * joint_scale.X;

            // get this joint effector
            LiveJObj effector_joint;
            LiveJObj joint2 = Joint_GetIKJoint2(jobj.Child);
            if (joint2 != null)
            {
                robj = RObjGetByType(joint2, REFTYPE.IKHINT, 0);
                x_scale = robj.Ref_IkHint.BoneLength * joint2.Scale.X * joint_scale.X;
                has_flag = (robj.LimitFlag & 4) != 0;
                effector_joint = jobj_get_effector_checked(joint2.Child);
            }
            else
            {
                effector_joint = jobj_get_effector_checked(jobj.Child);
            }

            // if effector joint was found then apply ik
            if (effector_joint != null)
            {
                Vector3 up = Vector3.Zero;
                Vector3 forward = Vector3.Zero;

                // update reftype jobj if it exists
                // TODO:
                //if (RObjGetByType(jobj, REFTYPE.JOBJ, 3) == null)
                //{
                //    if (jobj.Desc.ROBJ != NULL)
                //    {
                //        RObjUpdateAll(jobj->RObj, jobj, JObjUpdateFunc);
                //        if (HSD_JObjMtxIsDirty(jobj))
                //        {
                //            HSD_JOBJ_METHOD(jobj)->make_mtx(jobj);
                //            jobj->flags &= 0xFFFFFFBF;
                //        }
                //    }
                //}

                // get parent translation
                LiveJObj parent = jobj.Parent;
                if (parent != null)
                    trans = parent.WorldTransform.ExtractTranslation();

                // get effector robj's global position
                HSD_RObjGetGlobalPosition(effector_joint, effector_joint.Desc.ROBJ, 1, ref effector_joint.Translation);

                // get axis and magnitude squared
                axis = effector_joint.Translation - trans;
                dot = Vector3.Dot(axis, axis);

                // if magnitude is not 0
                if (dot > 1e-8F)
                {
                    if (HSD_RObjGetGlobalPosition(jobj, jobj.Desc.ROBJ, 3, ref up) != 0)
                    {
                        up = up - trans;
                        if (rotate_angle != 0.0F)
                        {
                            up = Vector3.TransformPosition(up, Matrix4.CreateFromAxisAngle(axis, rotate_angle));
                        }
                        forward = Vector3.Cross(axis, up);
                        up = Vector3.Cross(forward, axis);
                    }
                    else
                    {
                        forward = jobj.WorldTransform.Row2.Xyz;
                        up = Vector3.Cross(forward, axis);
                        forward = Vector3.Cross(axis, up);
                    }

                    // normalize forward vector
                    //float var_f4 = Math.sqrt(1.0F / (1e-10F + PSVECDotProduct(&forward, &forward)));
                    //PSVECScale(&forward, &fvec, var_f4);
                    fvec = forward.Normalized();

                    // normalize uvec vector
                    //f32 var_f4_2 = sqrtf(1.0F / (1e-10F + PSVECDotProduct(&up, &up)));
                    //PSVECScale(&up, &sp74, var_f4_2);
                    sp74 = up.Normalized();

                    // some math
                    length_squared = bone_length * bone_length;
                    scale_squared = x_scale * x_scale;
                    float lengthtoscale = length_squared - scale_squared;
                    var_f27 = 0.25F * (((2.0F * (length_squared + scale_squared)) - dot) - ((lengthtoscale * lengthtoscale) / dot));

                    // absolute value
                    if (var_f27 < 0.0F)
                        var_f27 = 0.0F;

                    // calculate axis_scale
                    float temp_f5_2 = (length_squared - var_f27) / dot;
                    //float var_f4_3 = (float)Math.Sqrt(1.0F / (1e-10F + temp_f5_2));
                    //axis_scale = temp_f5_2 * var_f4_3;
                    axis_scale = (float)Math.Sqrt(temp_f5_2);

                    // calculate bone_length
                    //float var_f5 = (float)Math.Sqrt(1.0F / (1e-10F + var_f27));
                    //bone_length = var_f27 * var_f5;
                    bone_length = (float)Math.Sqrt(var_f27);
                }
                else
                {
                    // not scaling applied
                    axis_scale = 0.0F;
                    // bone_length = bone_length;
                }

                // invert bone flag?
                if (has_flag)
                    bone_length = -bone_length;

                // scale up vector by bone length
                //PSVECScale(&sp74, &up, bone_length);
                up = sp74 * bone_length;

                // right vector is axis * axis_scale
                if ((scale_squared - var_f27) < dot)
                    //PSVECScale(&axis, &rvec, axis_scale);
                    rvec = axis * axis_scale;
                else
                    //PSVECScale(&axis, &rvec, -axis_scale);
                    rvec = axis * -axis_scale;

                // calculate right vector
                // PSVECAdd(&rvec, &up, &rvec);
                rvec = rvec + up;

                // normalize right vector
                //f32 rscale = sqrtf(1.0F / (1e-10F + PSVECDotProduct(&rvec, &rvec)));
                //PSVECScale(&rvec, &rvec, rscale);
                rvec = rvec.Normalized();

                // calculate up vector
                // PSVECCrossProduct(&fvec, &rvec, &uvec2);
                Vector3 uvec2 = Vector3.Cross(fvec, rvec);

                // generate matrix
                jobj.WorldTransform = new Matrix4(
                    new Vector4(rvec * joint_scale.X, 0),
                    new Vector4(uvec2 * joint_scale.Y, 0),
                    new Vector4(fvec * joint_scale.Z, 0),
                    new Vector4(trans, 1));
                //jobj->mtx[0][0] = rvec.x * joint_scale.x;
                //jobj->mtx[1][0] = rvec.y * joint_scale.x;
                //jobj->mtx[2][0] = rvec.z * joint_scale.x;
                //jobj->mtx[0][1] = uvec2.x * joint_scale.y;
                //jobj->mtx[1][1] = uvec2.y * joint_scale.y;
                //jobj->mtx[2][1] = uvec2.z * joint_scale.y;
                //jobj->mtx[0][2] = fvec.x * joint_scale.z;
                //jobj->mtx[1][2] = fvec.y * joint_scale.z;
                //jobj->mtx[2][2] = fvec.z * joint_scale.z;
                //jobj->mtx[0][3] = trans.x;
                //jobj->mtx[1][3] = trans.y;
                //jobj->mtx[2][3] = trans.z;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ResolveIKJoint2(LiveJObj jobj)
        {
            Vector3 trans;
            Vector3 parent_position;
            Vector3 rvec;
            Vector3 uvec;
            Vector3 fvec;
            LiveJObj temp_r5;
            float var_f4;
            bool has_flag;
            HSD_ROBJ robj;

            float x_scale = 1.0F;
            Vector3 joint_scale = Vector3.One;

            // get effector
            LiveJObj effector_joint = jobj_get_effector_checked(jobj.Child);

            // make sure effector and parent are not null
            if (effector_joint == null || jobj.Parent == null)
                return;

            // get parent scale
            if (jobj.Parent != null)
                joint_scale = jobj.Parent.WorldTransform.ExtractScale();

            // get parent translation
            {
                //MtxPtr mtx = jobj->parent->mtx;
                //parent_position.x = mtx[0][3];
                //parent_position.y = mtx[1][3];
                //parent_position.z = mtx[2][3];
                parent_position = jobj.Parent.WorldTransform.ExtractTranslation();
            }

            // get parent right vector
            {
                //MtxPtr mtx = jobj->parent->mtx;
                //rvec.x = mtx[0][0];
                //rvec.y = mtx[1][0];
                //rvec.z = mtx[2][0];
                rvec = jobj.Parent.WorldTransform.Row0.Xyz;
            }

            // normalize right vector
            //var_f4 = sqrtf(1.0F / (1e-10F + PSVECDotProduct(&rvec, &rvec)));
            //PSVECScale(&rvec, &rvec, var_f4);
            rvec.Normalize();

            // get parent x scale
            if (jobj.Parent != null)
                x_scale = jobj.Parent.WorldTransform.ExtractScale().X;

            // get parent ik hint
            robj = RObjGetByType(jobj.Parent, REFTYPE.IKHINT, 0);
            // assert_line(0x8FC, robj);

            // scale right vector by bone_length hint and parent x scale
            //PSVECScale(&rvec, &rvec, robj->u.ik_hint.bone_length * x_scale);
            rvec *= robj.Ref_IkHint.BoneLength * x_scale;

            // add parent position to right vecotr
            // PSVECAdd(&parent_position, &rvec, &trans);
            trans = parent_position + rvec;

            // calculate relative translation?
            //PSVECSubtract(&effector_joint->trans, &trans, &rvec);
            rvec = effector_joint.Translation - trans;

            // normalize rvec
            // PSVECScale(&rvec, &rvec, sqrtf(1.0F / (1e-10F + PSVECDotProduct(&rvec, &rvec))));
            rvec.Normalize();

            // apply ik limits
            HSD_ROBJ lower_limit = RObjGetByType(jobj, REFTYPE.LIMIT, 5);
            HSD_ROBJ upper_limit = RObjGetByType(jobj, REFTYPE.LIMIT, 6);
            if ((lower_limit != null) || (upper_limit != null))
            {
                bool apply_rotation = false;

                // get ik hint (not necessary since we already gave it)
                robj = RObjGetByType(jobj, REFTYPE.IKHINT, 0);
                // assert_line(0x91E, robj);

                // determine if flag is set
                has_flag = (robj.Flags & 4) != 0;

                // get parent right vector and normalize
                Vector3 sp28;
                {
                    //MtxPtr mtx = jobj->parent->mtx;
                    //sp28.x = mtx[0][0];
                    //sp28.y = mtx[1][0];
                    //sp28.z = mtx[2][0];
                    sp28 = jobj.Parent.WorldTransform.Row0.Xyz;
                }
                //PSVECNormalize(&sp28, &sp28);
                sp28.Normalize();

                // calculate dot between parent right vector and projected right vector
                float rotation;
                float bone_length_squared = Vector3.Dot(sp28, rvec); // PSVECDotProduct(&sp28, &rvec);
                if (bone_length_squared >= 1.0F)
                    rotation = 0.0F;
                else if (bone_length_squared <= -1.0F)
                    rotation = (float)Math.PI;
                else
                    rotation = (float)Math.Acos(bone_length_squared);

                // flag inverts angle
                if (!has_flag)
                    rotation = -rotation;

                // apply limits
                if (lower_limit != null && rotation < lower_limit.Ref_Limit)
                {
                    rotation = lower_limit.Ref_Limit;
                    apply_rotation = true;
                }
                else
                if (upper_limit != null)
                {
                    if (upper_limit.Ref_Limit < rotation)
                    {
                        rotation = upper_limit.Ref_Limit;
                        apply_rotation = true;
                    }
                }

                // apply rotation if necessary
                if (apply_rotation)
                {
                    Vector3 rotAxis;
                    {
                        //MtxPtr mtx = jobj->parent->mtx;
                        //rotAxis.x = mtx[0][2];
                        //rotAxis.y = mtx[1][2];
                        //rotAxis.z = mtx[2][2];
                        rotAxis = jobj.Parent.WorldTransform.Row2.Xyz;
                    }
                    //Mtx rotMtx;
                    //PSMTXRotAxisRad(rotMtx, &rotAxis, rotation);
                    //PSMTXMUltiVec(rotMtx, &sp28, &rvec);
                    rvec = Vector3.TransformPosition(sp28, Matrix4.CreateFromAxisAngle(rotAxis, rotation));
                }
            }

            // calculate up vector
            {
                //MtxPtr mtx = jobj->parent->mtx;
                //fvec.x = mtx[0][2];
                //fvec.y = mtx[1][2];
                //fvec.z = mtx[2][2];
                fvec = jobj.Parent.WorldTransform.Row2.Xyz;
            }
            //PSVECCrossProduct(&fvec, &rvec, &uvec);
            uvec = Vector3.Cross(fvec, rvec);

            // normalize up vector
            //f32 var_f4_2 = sqrtf(1.0F / (1e-10F + PSVECDotProduct(&uvec, &uvec)));
            //PSVECScale(&uvec, &uvec, var_f4_2);
            uvec.Normalize();

            // calculate forward vector
            //PSVECCrossProduct(&rvec, &uvec, &fvec);
            fvec = Vector3.Cross(rvec, uvec);

            // calculate final matrix
            jobj.WorldTransform = new Matrix4(
                new Vector4(rvec * joint_scale.X, 0),
                new Vector4(uvec * joint_scale.Y, 0),
                new Vector4(fvec * joint_scale.Z, 0),
                new Vector4(trans, 1));
            //jobj->mtx[0][0] = rvec.x * joint_scale.x;
            //jobj->mtx[1][0] = rvec.y * joint_scale.x;
            //jobj->mtx[2][0] = rvec.z * joint_scale.x;
            //jobj->mtx[0][1] = uvec.x * joint_scale.y;
            //jobj->mtx[1][1] = uvec.y * joint_scale.y;
            //jobj->mtx[2][1] = uvec.z * joint_scale.y;
            //jobj->mtx[0][2] = fvec.x * joint_scale.z;
            //jobj->mtx[1][2] = fvec.y * joint_scale.z;
            //jobj->mtx[2][2] = fvec.z * joint_scale.z;
            //jobj->mtx[0][3] = trans.x;
            //jobj->mtx[1][3] = trans.y;
            //jobj->mtx[2][3] = trans.z;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ResolveIKEffector(LiveJObj jobj)
        {
            LiveJObj parent = jobj.Parent;
            float x_scale = 1.0F;
            if (parent != null)
            {
                HSD_ROBJ robj = RObjGetByType(parent, REFTYPE.IKHINT, 0);
                if (robj != null)
                {
                    // get and normalize position
                    Vector3 position = parent.WorldTransform.Row3.Xyz;
                    //position.x = parent->mtx[0][3];
                    //position.y = parent->mtx[1][3];
                    //position.z = parent->mtx[2][3];

                    // get and normalize rvec
                    Vector3 rvec = parent.WorldTransform.Row0.Xyz.Normalized();
                    //rvec.x = parent->mtx[0][0];
                    //rvec.y = parent->mtx[1][0];
                    //rvec.z = parent->mtx[2][0];
                    //PSVECScale(&rvec, &rvec, sqrtf(1.0F / (1e-10F + PSVECDotProduct(&rvec, &rvec))));

                    // apply scale
                    //if (parent->VEC != NULL)
                    x_scale = parent.WorldTransform.ExtractScale().X; // parent->VEC->X;
                    //PSVECScale(&rvec, &rvec, robj.Ref_IkHint.BoneLength * x_scale);
                    rvec *= robj.Ref_IkHint.BoneLength * x_scale;

                    // adjust final position
                    //Vec3 final_position;
                    //PSVECAdd(&position, &rvec, &final_position);
                    //jobj->mtx[0][3] = final_position.x;
                    //jobj->mtx[1][3] = final_position.y;
                    //jobj->mtx[2][3] = final_position.z;
                    jobj.WorldTransform = new Matrix4(
                        jobj.WorldTransform.Row0,
                        jobj.WorldTransform.Row1,
                        jobj.WorldTransform.Row2,
                        new Vector4(position + rvec, 1));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        /// <returns></returns>
        private static LiveJObj Joint_GetIKJoint2(LiveJObj jobj)
        {
            foreach (var j in jobj.Enumerate)
            {
                if ((j.Desc.Flags & JOBJ_FLAG.EFFECTOR) == JOBJ_FLAG.JOINT2)
                {
                    return j;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        /// <returns></returns>
        private static LiveJObj jobj_get_effector(LiveJObj jobj)
        {
            foreach (var j in jobj.Enumerate)
            {
                if ((j.Desc.Flags & JOBJ_FLAG.EFFECTOR) == JOBJ_FLAG.EFFECTOR)
                {
                    return j;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eff"></param>
        /// <returns></returns>
        private static LiveJObj jobj_get_effector_checked(LiveJObj eff)
        {
            eff = jobj_get_effector(eff);
            if (RObjGetByType(eff, REFTYPE.JOBJ, 1) != null)
            {
                return eff;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void RObjUpdateAll(LiveJObj jobj)
        {
            if (jobj.Desc.ROBJ == null)
                return;

            //resolveCnsPos(robj, obj, obj_update);
            //resolveCnsDirUp(robj, obj, obj_update);
            //resolveCnsOrientation(robj, obj, obj_update);
            //resolveLimits(robj, obj);
            //for (HSD_RObj* r = robj; r != NULL; r = r->next)
            //{
            //    if ((r->flags & 0x70000000) == 0 && (r->flags & 0x80000000) != 0)
            //    {
            //        //expEvaluate(robj->u.exp.rvalue, r->flags & 0xFFFFFFF, obj, obj_update);
            //    }
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static HSD_ROBJ RObjGetByType(LiveJObj jobj, REFTYPE type, int subtype)
        {
            if (jobj == null || jobj.Desc.ROBJ == null)
                return null;

            foreach (var r in jobj.Desc.ROBJ.List)
            {
                if (r.RefType == type)
                {
                    if (subtype == 0 || r.LimitFlag == subtype)
                        return r;
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="robj"></param>
        /// <param name="flag"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        private static int HSD_RObjGetGlobalPosition(LiveJObj jobj, HSD_ROBJ robj, int flag, ref Vector3 pos)
        {
            int pos_count = 0;
            Vector3 global_position = Vector3.Zero;

            if (robj == null)
            {
                return 0;
            }
            else
            {
                for (HSD_ROBJ i = robj; i != null; i = i.Next)
                {
                    int flags = i.Flags;
                    if (i.RefType == REFTYPE.JOBJ && (flags & 0x80000000) != 0)
                    {
                        flags &= 0xfffffff;
                        if (flags == flag)
                        {
                            HSD_JOBJ j = i.Ref_Joint;
                            // assert(jobj != NULL);
                            var live = jobj.Root.GetJObjFromDesc(j);
                            if (live != null)
                            {
                                live.RecalculateTransforms(null, false);
                                //HSD_JObjSetupMatrix(jobj);
                                pos_count += 1;
                                global_position += live.WorldTransform.ExtractTranslation();
                                //global_position.x += guMtxRowCol(jobj->mtx, 0, 3);
                                //global_position.y += guMtxRowCol(jobj->mtx, 1, 3);
                                //global_position.z += guMtxRowCol(jobj->mtx, 2, 3);
                            }
                        }
                    }
                }
                if (pos_count > 0)
                {
                    pos = global_position;
                }
            }
            return pos_count;
        }
    }
}
