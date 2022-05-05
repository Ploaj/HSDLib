using HSDRaw.Common;
using HSDRaw.Melee.Pl;
using HSDRawViewer.Rendering.Models;
using OpenTK;
using System;
using System.Collections.Generic;

namespace HSDRawViewer.Rendering.Animation
{

    public class PhysicsJobj
    {
        public HSD_JOBJ Jobj;
        public LiveJObj Live;
        public Vector3 Translation;
        public Vector4 Rotation;
        public Vector3 Scale;
    }

    public class DynamicsParamHeap
    {
        public PhysicsJobj Joint;
        public Vector4 Rotation;
        public Vector3 Translation;
        public Vector3 Scale;
        public Vector3 WorldPos;
        public Vector3 RotAxis;
        public float RotMomentum { get; set; }
        public float Length { get; set; }
        public float x4C { get; set; }
        public float x50 { get; set; }
        public int Direction;
        public Vector4 rot_orig;
        public float RotationLimit { get; set; }
        public float x6C { get; set; }
        public float x70 { get; set; }
        public float x74 { get; set; }
        public float x78 { get; set; }
        public float x7C { get; set; }
        public float x80 { get; set; }
        public float RotMomentumSpeed { get; set; }
        public float MaxAngleChange { get; set; }
        public float length_scale;

        private void PrintFormattedFloat(float f)
        {
            Console.WriteLine("{0:X8} {1}", BitConverter.ToInt32(BitConverter.GetBytes(f), 0), f);
        }

        public void Reset(JOBJManager m)
        {
            //var jobj = Joint.Jobj;
            //Joint.Translation = new Vector3(jobj.TX, jobj.TY, jobj.TZ);
            //Joint.Rotation = new Vector4(jobj.RX, jobj.RY, jobj.RZ, 0);
            //Joint.Scale = new Vector3(jobj.SX, jobj.SY, jobj.SZ);

            //Translation = new Vector3(jobj.TX, jobj.TY, jobj.TZ);
            //Rotation = new Vector4(jobj.RX, jobj.RY, jobj.RZ, 0);
            //Scale = new Vector3(jobj.SX, jobj.SY, jobj.SZ);
            //m.SetOverrideLocalTransform(jobj, Joint.LocalMatrix);
            //m.UpdateNoRender();
            //WorldPos = m.GetWorldTransform(jobj).ExtractTranslation();
            //RotAxis = Vector3.UnitX;
            //RotMomentum = 0;
        }

        public void PrintData()
        {
            PrintFormattedFloat(Rotation.X);
            PrintFormattedFloat(Rotation.Y);
            PrintFormattedFloat(Rotation.Z);
            PrintFormattedFloat(Rotation.W);

            PrintFormattedFloat(Translation.X);
            PrintFormattedFloat(Translation.Y);
            PrintFormattedFloat(Translation.Z);

            PrintFormattedFloat(Scale.X);
            PrintFormattedFloat(Scale.Y);
            PrintFormattedFloat(Scale.Z);

            PrintFormattedFloat(WorldPos.X);
            PrintFormattedFloat(WorldPos.Y);
            PrintFormattedFloat(WorldPos.Z);

            PrintFormattedFloat(RotAxis.X);
            PrintFormattedFloat(RotAxis.Y);
            PrintFormattedFloat(RotAxis.Z);
            PrintFormattedFloat(RotMomentum);

            PrintFormattedFloat(Length);

            PrintFormattedFloat(x4C);
            PrintFormattedFloat(x50);

            Console.WriteLine("{0:X8} {0}", Direction);

            PrintFormattedFloat(rot_orig.X);
            PrintFormattedFloat(rot_orig.Y);
            PrintFormattedFloat(rot_orig.Z);
            PrintFormattedFloat(rot_orig.W);

            PrintFormattedFloat(RotationLimit);
            PrintFormattedFloat(x6C);
            PrintFormattedFloat(x70);
            PrintFormattedFloat(x74);
            PrintFormattedFloat(x78);
            PrintFormattedFloat(x7C);
            PrintFormattedFloat(x80);
            PrintFormattedFloat(RotMomentumSpeed);
            PrintFormattedFloat(MaxAngleChange);

            PrintFormattedFloat(length_scale);
        }
    }

    public class Dynamics
    {
        public int ApplyNum;

        public List<DynamicsParamHeap> PhysicsBones;

        public int BoneNum { get; set; }

        public float x8 { get; set; }

        public float xc { get; set; }

        public float x10 { get; set; }

        public Vector3 GravityVector { get; } = new Vector3(0, -1, 0);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <param name="index"></param>
        /// <param name="bone_count"></param>
        public void InitBones(JOBJManager m, int index, int bone_count)
        {
            var jobj = m.GetJOBJ(index).Desc;

            if (jobj == null)
            {
                PhysicsBones = null;
                return;
            }

            BoneNum = 0;
            //var pointer_x00 = 0x804D63A4; // some kind of custom heap?

            PhysicsBones = new List<DynamicsParamHeap>();

            while (BoneNum < bone_count)
            {
                var param = new DynamicsParamHeap();
                PhysicsBones.Add(param);

                // Make matrix dirty 8000fe10
                // if (jobj != 0) // USER_DEFINED_MTX && jobj.Flags.HasFlag(JOBJ_FLAG.MTX_DIRTY))

                var worldTransform = m.GetWorldTransform(jobj);

                param.Joint = new PhysicsJobj()
                {
                    Jobj = jobj,
                    Live = m.GetLiveJOBJ(jobj),
                    Translation = new Vector3(jobj.TX, jobj.TY, jobj.TZ),
                    Rotation = new Vector4(jobj.RX, jobj.RY, jobj.RZ, 0),
                    Scale = new Vector3(jobj.SX, jobj.SY, jobj.SZ),
                };
                param.Rotation = new Vector4(jobj.RX, jobj.RY, jobj.RZ, 0);
                param.Translation = new Vector3(jobj.TX, jobj.TY, jobj.TZ);
                param.Scale = new Vector3(jobj.SX, jobj.SY, jobj.SZ);
                param.WorldPos = worldTransform.ExtractTranslation();
                param.rot_orig = new Vector4(param.Rotation.X, param.Rotation.Y, param.Rotation.Z, param.Rotation.W);
                param.RotAxis = Vector3.UnitX;
                param.RotMomentum = 0;
                param.Length = 0;
                param.length_scale = 0;

                // go to child
                jobj = jobj.Child;
                BoneNum++;
            }

            // now calculate direction and lengths
            for (int i = 0; i < PhysicsBones.Count - 1; i++)
            {
                var current = PhysicsBones[i];
                var next = PhysicsBones[i + 1];

                current.Length = Vector3.Distance(current.WorldPos, next.WorldPos);

                var xabs = Math.Abs(next.Translation.X);
                var yabs = Math.Abs(next.Translation.Y);
                var zabs = Math.Abs(next.Translation.Z);

                if (zabs <= yabs)
                {
                    if (yabs <= xabs)
                    {
                        current.Direction = 0;
                    }
                    else
                    {
                        current.Direction = 1;
                    }
                }
                else if (zabs <= xabs)
                {
                    current.Direction = 0;
                }
                else
                {
                    current.Direction = -1;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="desc"></param>
        public void InitParams(SBM_DynamicDesc desc)
        {
            x8 = desc.PARAM1;
            xc = desc.PARAM2;
            x10 = desc.PARAM3;

            for (int i = 0; i < desc.Parameters.Length; i++)
            {
                var p = PhysicsBones[i];
                var d = desc.Parameters[i];
                p.x4C = d.PARAM1;
                p.x50 = d.PARAM2;
                p.rot_orig = new Vector4(d.RotX, d.RotY, d.RotZ, d.RotW);
                p.RotationLimit = d.RotLimit;

                p.x6C = d.PARAM8;
                p.x70 = d.PARAM9;
                p.x74 = d.PARAM10;

                p.x78 = d.PARAM11;
                p.x7C = d.PARAM12;
                p.x80 = d.PARAM13;

                p.RotMomentumSpeed = d.RotMomentumSpeed;
                p.MaxAngleChange = d.MaxAngleChange;

                if (p.Length == 0)
                    p.length_scale = 0;
                else
                    p.length_scale = desc.PARAM3 / p.Length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void PrintData()
        {
            Console.WriteLine("{0:X8} {0}", BoneNum);
            Console.WriteLine("{0:X8} {1}", BitConverter.ToInt32(BitConverter.GetBytes(x8), 0), x8);
            Console.WriteLine("{0:X8} {1}", BitConverter.ToInt32(BitConverter.GetBytes(xc), 0), xc);
            Console.WriteLine("{0:X8} {1}", BitConverter.ToInt32(BitConverter.GetBytes(x10), 0), x10);

            foreach (var v in PhysicsBones)
            {
                Console.WriteLine();

                v.PrintData();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="param_1"></param>
        /// <param name="param_2"></param>
        private float zz_00101c8_(Vector3 param_1, Vector3 param_2)
        {
            return 0;
            //float fVar1;
            //char* pcVar2;
            //double dVar3;
            //double dVar4;
            //double dVar5;
            //double dVar6;
            //double dVar7;
            //double dVar8;
            //Vec3 local_50;
            //undefined4 local_40;
            //uint uStack60;

            //fVar1 = FLOAT_804d7ba4;
            //pcVar2 = DAT_804d63b0;
            //param_2->X = FLOAT_804d7ba4;
            //param_2->Y = fVar1;
            //param_2->Z = fVar1;
            //dVar5 = DOUBLE_804d7bc0;
            //dVar6 = DOUBLE_804d7bb8;
            //dVar7 = DOUBLE_804d7bc8;
            //dVar8 = DOUBLE_804d7ba8;
            //for (; pcVar2 != (char*)0x0; pcVar2 = *(char**)(pcVar2 + 0x34))
            //{
            //    uStack60 = *(uint*)(pcVar2 + 0x30) ^ 0x80000000;
            //    local_40 = 0x43300000;
            //    dVar3 = cos((double)((float)((double)CONCAT44(0x43300000, uStack60) - dVar7) *
            //                        *(float*)(pcVar2 + 0x2c)));
            //    dVar3 = (double)(float)(dVar8 * (double)*(float*)(pcVar2 + 0x20) * (dVar6 + dVar3));
            //    if (*pcVar2 == '\x01')
            //    {
            //        if ((((*(float*)(pcVar2 + 0x10) < *param_1) && (*param_1 < *(float*)(pcVar2 + 0x18))) &&
            //            (param_1[1] < *(float*)(pcVar2 + 0x14))) && (*(float*)(pcVar2 + 0x1c) < param_1[1]))
            //        {
            //            param_2->X = (float)((double)*(float*)(pcVar2 + 4) * dVar3 + (double)param_2->X);
            //            param_2->Y = (float)((double)*(float*)(pcVar2 + 8) * dVar3 + (double)param_2->Y);
            //            param_2->Z = (float)((double)*(float*)(pcVar2 + 0xc) * dVar3 + (double)param_2->Z);
            //        }
            //    }
            //    else
            //    {
            //        local_50.X = *param_1 - *(float*)(pcVar2 + 4);
            //        local_50.Y = param_1[1] - *(float*)(pcVar2 + 8);
            //        local_50.Z = param_1[2] - *(float*)(pcVar2 + 0xc);
            //        dVar4 = (double)Vec3_Normalize(&local_50);
            //        dVar4 = (double)(float)(dVar5 * dVar4);
            //        if (dVar4 < dVar6)
            //        {
            //            dVar4 = (double)FLOAT_804d7ba0;
            //        }
            //        fVar1 = (float)(dVar6 / (double)(float)(dVar4 * dVar4));
            //        param_2->X = fVar1 * (float)((double)local_50.X * dVar3) + param_2->X;
            //        param_2->Y = fVar1 * (float)((double)local_50.Y * dVar3) + param_2->Y;
            //        param_2->Z = fVar1 * (float)((double)local_50.Z * dVar3) + param_2->Z;
            //    }
            //}
            //Vec3_Normalize(param_2);
            //return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        private bool CheckDistance(Vector3 v1, Vector3 v2, Vector3 v3, float distance)
        {
            if (v1.X <= v2.X)
            {
                if (v1.X - distance > v3.X)
                {
                    return false;
                }
                if (v2.X + distance < v3.X)
                {
                    return false;
                }
            }
            else
            {
                if (v1.X + distance < v3.X)
                {
                    return false;
                }
                if (v2.X - distance > v3.X)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thres"></param>
        /// <param name="size"></param>
        /// <param name="world_pos"></param>
        /// <param name="param_4"></param>
        /// <param name="hit_pos"></param>
        /// <returns></returns>
        private bool zz_0005c44_(float thres, float size, Vector3 world_pos, Vector3 param_4, Vector3 hit_pos) //, Vec3* param_6)
        {
            // final distance
            float distance = thres + size;

            if (world_pos.X <= param_4.X)
            {
                if (hit_pos.X < world_pos.X - distance)
                {
                    return false;
                }
                if (param_4.X + distance < hit_pos.X)
                {
                    return false;
                }
            }
            else
            {
                if (world_pos.X + distance < hit_pos.X)
                {
                    return false;
                }
                if (hit_pos.X < param_4.X - distance)
                {
                    return false;
                }
            }
            if (world_pos.Y <= param_4.Y)
            {
                if (hit_pos.Y < world_pos.Y - distance)
                {
                    return false;
                }
                if (param_4.Y + distance < hit_pos.Y)
                {
                    return false;
                }
            }
            else
            {
                if (world_pos.Y + distance < hit_pos.Y)
                {
                    return false;
                }
                if (hit_pos.Y < param_4.Y - distance)
                {
                    return false;
                }
            }
            if (world_pos.Z <= param_4.Z)
            {
                if (hit_pos.Z < world_pos.Z - distance)
                {
                    return false;
                }
                if (param_4.Z + distance < hit_pos.Z)
                {
                    return false;
                }
            }
            else
            {
                if (world_pos.Z + distance < hit_pos.Z)
                {
                    return false;
                }
                if (hit_pos.Z < param_4.Z - distance)
                {
                    return false;
                }
            }

            var dVar11 = param_4.Y - world_pos.Y;
            var dVar10 = param_4.Z - world_pos.Z;
            var dVar7 = param_4.X - world_pos.X;

            float dVar9 = 0;
            var fVar1 = (dVar10 * dVar10 + (dVar7 * dVar7 + (dVar11 * dVar11)));
            if ((1.0E-5 <= fVar1) || (fVar1 <= -1.0E-5))
            {
                dVar9 = (-(dVar10 * (world_pos.Z - hit_pos.Z) + (dVar7 * (world_pos.X - hit_pos.X) + (dVar11 * (world_pos.Y - hit_pos.Y)))) / fVar1);

                // clamp between 0 and 1
                if (dVar9 < 0)
                    dVar9 = 0;
                if (dVar9 > 1)
                    dVar9 = 1;
            }

            // salted distance
            Vector3 param_6 = new Vector3(
                dVar7 * dVar9 + world_pos.X,
                dVar11 * dVar9 + world_pos.Y,
                dVar10 * dVar9 + world_pos.Z
                );

            float fVar3 = param_6.Y - hit_pos.Y;
            float fVar4 = param_6.Z - hit_pos.Z;
            float fVar5 = param_6.X - hit_pos.X;

            // fast distance between param_6 and hit_pos
            return fVar4 * fVar4 + fVar5 * fVar5 + fVar3 * fVar3 <= distance * distance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <param name="x1648"></param>
        /// <param name="x1644"></param>
        /// <param name="enable_ground_collision"></param>
        /// <param name="apply_phys_num"></param>
        /// <param name="max_apply_physics"></param>
        /// <param name="air_state"></param>
        public void Think(JOBJManager m, List<SBM_DynamicHitBubble> hitbubbles, bool enable_ground_collision, int max_apply_physics, int air_state)
        {
            // special flag to skip processing
            if (ApplyNum > 0x100)
                return;

            // 800104e0 - 80010558 not sure what this section does

            DynamicsParamHeap dynamic_params = PhysicsBones[0];

            // start transform at parent
            var matrix = m.GetLiveJOBJ(dynamic_params.Joint.Jobj).Parent.WorldTransform;

            // get joint
            PhysicsJobj joint;
            DynamicsParamHeap child;
            Matrix4 final_matrix;
            Matrix4 rot_matrix;

            var bVar2 = false;

            for (int i = ApplyNum; i < PhysicsBones.Count - 1; i++)
            {
                dynamic_params = PhysicsBones[i];
                joint = dynamic_params.Joint;
                child = PhysicsBones[i + 1];

                var mtx_trans = Matrix4.CreateTranslation(joint.Translation);
                final_matrix = mtx_trans * matrix;
                rot_matrix = Math3D.CreateMatrix4FromEuler(dynamic_params.rot_orig.Xyz) * final_matrix;
                final_matrix = Math3D.CreateMatrix4FromEuler(joint.Rotation.Xyz) * final_matrix;
                var scale_matrix = Matrix4.CreateScale(joint.Scale);
                final_matrix = scale_matrix * final_matrix;
                rot_matrix = scale_matrix * rot_matrix;

                // update world position
                dynamic_params.WorldPos = final_matrix.ExtractTranslation();

                // calculate vectors
                var rot_vector = Vector3.TransformPosition(new Vector3(child.Translation), rot_matrix) - dynamic_params.WorldPos;
                var final_vector = Vector3.TransformPosition(new Vector3(child.Translation), final_matrix) - dynamic_params.WorldPos;

                // vector from child to self
                var axis_vector = child.WorldPos - dynamic_params.WorldPos;

                // normalize all the vectors
                rot_vector.Normalize();
                final_vector.Normalize();
                axis_vector.Normalize();

                // original axis vector
                var org_axis_vector = new Vector3(axis_vector.X, axis_vector.Y, axis_vector.Z);

                // 800107d8 - 80010878
                // not sure
                // peach's hair seems to use it?
                // stability related?
                if (dynamic_params.x4C * x8 < 1)
                {
                    var a = Math.Abs(Vector3.CalculateAngle(final_vector, axis_vector));
                    if (a != 0)
                    {
                        var VStack604 = Vector3.Cross(axis_vector, final_vector);
                        if (VStack604.Length != 0)
                        {
                            VStack604.Normalize();
                            axis_vector.RotateAboutUnitAxis(a * (1 - dynamic_params.x4C * x8), VStack604); // lbvector_RotateAboutUnitAxis(a * (1 - dynamic_params.x4C * x8), ref axis_vector, VStack604);
                        }
                    }
                }

                // 80010888 - 800108d0
                // gravity
                var ang = Vector3.CalculateAngle(GravityVector, axis_vector);
                if (ang != 0)
                {
                    float gravity_angle = (float)Math.Abs(Math.Sin(ang) * dynamic_params.length_scale);
                    var gravity_direction = Vector3.Cross(axis_vector, GravityVector).Normalized();
                    axis_vector.RotateAboutUnitAxis(gravity_angle, gravity_direction); // lbvector_RotateAboutUnitAxis(gravity_angle, ref axis_vector, gravity_direction);
                }

                // 800108e8 - 8001096c
                // TODO: global wind physics?
                //if (max_apply_physics <= i) // some global 804d63b0 != 0
                //{
                //    //var pJVar6 = UnkParams[i + 1].Joint;
                //    var local_280 = child.WorldPos;
                //    Vector3 auStack652 = Vector3.Zero;
                //    var dVar9 = zz_00101c8_(local_280, auStack652); // this function uses global info
                //    var dVar10 = Vector3.CalculateAngle(axis_vector, auStack652);
                //    if (0 != dVar10)
                //    {
                //        dVar10 = (float)Math.Abs((dVar9 * Math.Sin(dVar10)) / dynamic_params.Length); //(&dynamic_params->dvar14_x44)[1]);
                //        var VStack664 = Vector3.Cross(axis_vector, auStack652).Normalized();
                //        lbvector_RotateAboutUnitAxis(dVar10, ref axis_vector, VStack664); 
                //    }
                //}

                // 80010978 - 80010980
                // apply swing momentum
                if (dynamic_params.RotMomentum != 0)
                    axis_vector.RotateAboutUnitAxis(dynamic_params.RotMomentum, dynamic_params.RotAxis); // lbvector_RotateAboutUnitAxis(dynamic_params.RotMomentum, ref axis_vector, dynamic_params.RotAxis);

                // 80010998 - 80010a08
                // corrects the rotation based on MaxAngleChange
                if ((dynamic_params.length_scale * dynamic_params.Length) > 0)
                {
                    var local_2a4 = new Vector3(org_axis_vector.X, org_axis_vector.Y, org_axis_vector.Z);
                    if (dynamic_params.MaxAngleChange < Vector3.CalculateAngle(axis_vector, local_2a4))
                    {
                        var VStack688 = Vector3.Cross(local_2a4, axis_vector).Normalized();
                        local_2a4.RotateAboutUnitAxis(dynamic_params.MaxAngleChange, VStack688); // lbvector_RotateAboutUnitAxis(dynamic_params.MaxAngleChange, ref local_2a4, VStack688);
                        axis_vector = local_2a4;
                    }
                }

                // 80010a14 - 80010a48
                // seems to make model have a static shape that is affected by drag
                if (dynamic_params.x50 > 0)
                {
                    if (Vector3.CalculateAngle(axis_vector, rot_vector) >= dynamic_params.x50)
                    {
                        var VStack700 = Vector3.Cross(axis_vector, rot_vector).Normalized();
                        axis_vector.RotateAboutUnitAxis(dynamic_params.x50, VStack700); // lbvector_RotateAboutUnitAxis(dynamic_params.x50, ref axis_vector, VStack700);
                    }
                    else
                    {
                        axis_vector.X = rot_vector.X;
                        axis_vector.Y = rot_vector.Y;
                        axis_vector.Z = rot_vector.Z;
                    }
                }

                // 80010a8c - 80010aac
                // limits the amount of rotation a joint can have
                var rot_axis_angle = Vector3.CalculateAngle(axis_vector, rot_vector);
                if (rot_axis_angle > dynamic_params.RotationLimit)
                {
                    var VStack712 = Vector3.Cross(axis_vector, rot_vector).Normalized();
                    axis_vector.RotateAboutUnitAxis(rot_axis_angle - dynamic_params.RotationLimit, VStack712); // lbvector_RotateAboutUnitAxis(rot_axis_angle - dynamic_params.RotationLimit, ref axis_vector, VStack712);
                }

                // 80010abc - 80010ca0
                // hitbubble collision
                if (hitbubbles != null && hitbubbles.Count != 0)
                {
                    // normalize axis_vector before correcting?
                    axis_vector.Normalize();

                    // process each hitbubble
                    foreach (var hb in hitbubbles)
                    {
                        var local_2d4 = axis_vector * dynamic_params.Length + dynamic_params.WorldPos;
                        var hbPos = (Matrix4.CreateTranslation(hb.X, hb.Y, hb.Z) * m.GetWorldTransform(hb.BoneIndex)).ExtractTranslation();
                        var hitDistance = hbPos - dynamic_params.WorldPos;

                        var distance = hitDistance.LengthFast;

                        if (hb.Size < distance)//&&
                                               //zz_0005c44_(0.1f, hb.Size * 2, dynamic_params.WorldPos, local_2d4, hbPos))
                        {
                            float angle = Vector3.CalculateAngle(axis_vector, hitDistance);
                            if (angle != 0)
                            {
                                var __y = 0.1f + hb.Size;
                                var thresDistance = new Vector2(distance, __y).LengthFast;
                                var reflect_angle = (float)Math.Abs(Math.Atan2(__y, thresDistance));
                                reflect_angle -= angle;
                                if (reflect_angle > 0)
                                {
                                    var VStack760 = Vector3.Cross(hitDistance, axis_vector).Normalized();
                                    axis_vector.RotateAboutUnitAxis(reflect_angle, VStack760); // lbvector_RotateAboutUnitAxis(reflect_angle, ref axis_vector, VStack760);
                                }
                            }
                        }
                    }
                }

                // TODO: air_state != 0 80010cb0 - 80010ea4
                // stage collision

                var final_angle = Vector3.CalculateAngle(axis_vector, final_vector);
                var VStack324 = Vector3.Cross(final_vector, axis_vector).Normalized();
                var local_12c = new Vector3(final_vector.X, final_vector.Y, final_vector.Z);
                local_12c.RotateAboutUnitAxis(final_angle, VStack324); // lbvector_RotateAboutUnitAxis(final_angle, ref local_12c, VStack324); //dVar11, dVar12, 
                axis_vector = local_12c;

                // calculate rotation momentum angle by getting the 
                // angle between the original axis and the newly calculate one
                if (bVar2)
                    dynamic_params.RotMomentum = 0;
                else
                    dynamic_params.RotMomentum = Vector3.CalculateAngle(org_axis_vector, axis_vector);

                // update rot axis only if there is momentum
                if (Math.Abs(dynamic_params.RotMomentum) > 0)
                    dynamic_params.RotAxis = Vector3.Cross(org_axis_vector, axis_vector).Normalized();

                // calculate new rotation momentum speed
                if (dynamic_params.RotMomentum != 0)
                {
                    var momCurrent = dynamic_params.RotMomentum;
                    var momSpeed = dynamic_params.RotMomentumSpeed;

                    if (momCurrent > momSpeed)
                        dynamic_params.RotMomentum = momCurrent - momSpeed;
                    else
                    if (momCurrent < -momSpeed)
                        dynamic_params.RotMomentum = momCurrent + momSpeed;
                    else
                        dynamic_params.RotMomentum = 0;
                }

                // 80011128 - 800112bc
                // update joint rotation
                if (((1.0E-5 <= final_angle) || (final_angle <= -1.0E-5)))
                {
                    final_matrix = new Matrix4(matrix.Row0, matrix.Row1, matrix.Row2, matrix.Row3);
                    final_matrix.Transpose();
                    var local_214 = Vector3.TransformPosition(VStack324, final_matrix);

                    if (((1.0E-5 <= local_214.X) || (local_214.X <= -1.0E-5)) ||
                        ((1.0E-5 <= local_214.Y) || (local_214.Y <= -1.0E-5)) ||
                        ((1.0E-5 <= local_214.Z) || (local_214.Z <= -1.0E-5)))
                    {
                        Quaternion auStack560 = Quaternion.FromAxisAngle(local_214, final_angle);
                        Quaternion quaternion = Math3D.EulerToQuat(joint.Rotation.X, joint.Rotation.Y, joint.Rotation.Z);
                        Quaternion auStack592 = auStack560 * quaternion; // zz_037ec4c_(auStack560, quaternion, auStack592);
                        final_matrix = Matrix4.CreateFromQuaternion(auStack592); // PSMTXQuat(final_matrix, auStack592);
                        joint.Rotation.Xyz = final_matrix.ExtractRotationEuler(); // zz_037eb28_(final_matrix, local_3b4);
                    }
                }

                // 800112c8 - 800114dc
                //apply friction to rotation?
                if (dynamic_params.Direction == -float.NaN)
                    joint.Rotation.Z *= 0.9f;
                else if (dynamic_params.Direction == 0.0)
                    joint.Rotation.X *= 0.9f;
                else
                    joint.Rotation.Y *= 0.9f;

                // update matrix for next child
                matrix = mtx_trans * matrix;
                matrix = Math3D.CreateMatrix4FromEuler(joint.Rotation.Xyz) * matrix;
                matrix = scale_matrix * matrix;
            }

            // update final bone position
            if (PhysicsBones.Count > 1)
            {
                joint = PhysicsBones[PhysicsBones.Count - 1].Joint;
                matrix = Matrix4.CreateTranslation(joint.Translation) * matrix;
                matrix = Math3D.CreateMatrix4FromEuler(joint.Rotation.Xyz) * matrix;
                matrix = Matrix4.CreateScale(joint.Scale) * matrix;
                PhysicsBones[PhysicsBones.Count - 1].WorldPos = matrix.ExtractTranslation();
            }

            // update matricies
            for (int i = 0; i < PhysicsBones.Count - 1; i++)
            {
                var v = PhysicsBones[i];
                if (i >= ApplyNum)
                {
                    v.Joint.Live.Rotation = v.Joint.Rotation;
                }
            }
        }
    }

}