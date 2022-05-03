using HSDRaw.Common;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HSDRawViewer.Rendering.Renderers
{
    public class ParticleGenerator
    {
        public ParticleSystem Parent;

        public class ParticleJoint
        {
            public HSD_JOBJ Jobj = null;
        }

        public ParticleKind Kind { get; set; }
        public float Random { get; set; }
        public float NumToSpawn { get; set; }
        public ParticleJoint Joint { get; set; }

        public short GenLife { get; set; }
        public ParticleType Shape { get; set; }
        public GeneratorFlags Flags { get; set; }

        public byte Bank;
        public byte LinkNo;
        public byte TexG;
        public byte x1b;
        public short IDNum;
        public short Life;
        public byte[] cmdList;
        public Vector3 JointPosition { get; set; }
        public Vector3 Velocity;
        public float Gravity;
        public float Friction;
        public float Size;
        public float Radius;
        public float Angle;

        public int ParticleNum;

        public AppSRT AppSRT;
        // SpawnParticleCallback
        // DestroyParticleCallback

        public Vector2 DiscParam;

        public Vector3 LineParam;

        public Vector3 ConeParam;

        public float TornadoParam;

        public Vector3 SphereParam1;
        public Vector2 SphereParam2;

        public Vector3 RectParam1;
        public Vector3 RectParam2;
        public Vector3 RectParam3;
        public Vector3 RectParam4;

        public int RectFlags;


        public bool Dead { get; internal set; } = false;

        public Random RandomGen { get; set; } = new Random();

        public HSD_ParticleGenerator BankGen { get; set; }

        public static bool RandomSeed = true;

        public static int SetRandomSeed = 0;


        public ParticleGenerator(HSD_ParticleGenerator gen)
        {
            Shape = gen.TypeShape;
            Flags = gen.Flags;
            // Bank = bankid;
            // LinkNo = 
            BankGen = gen;
            Kind = gen.Kind;
            TexG = (byte)gen.TexGroup;
            Life = gen.Life;
            GenLife = gen.GenLife;
            JointPosition = Vector3.Zero;
            Velocity = new Vector3(gen.VX, gen.VY, gen.VZ);
            Gravity = gen.Gravity;
            Friction = gen.Friction;
            Size = gen.Size;
            cmdList = gen.TrackData;
            Radius = gen.Radius;
            Angle = gen.Angle;
            Random = gen.Random;

            if (!RandomSeed)
                RandomGen = new Random(SetRandomSeed);

            if (Kind.HasFlag(ParticleKind.IMMRND))
            {
                if (Random >= 0)
                    NumToSpawn = (float)RandomGen.NextDouble();
                else
                    NumToSpawn = 0;
            }
            else if (Random >= 0)
            {
                NumToSpawn = 0.99999999f;
            }
            else
            {
                if (Random + 1 > 1.1920929E-7)
                    NumToSpawn = 1;
                else
                    NumToSpawn = 0;
            }

            // 8039f270-8039f290 sets the ComTLUT based on if paletted format is used

            Joint = null;
            ParticleNum = 0;

            switch (Shape)
            {
                case ParticleType.Disc:
                case ParticleType.DiscCT:
                case ParticleType.DiscCD:
                    if ((0 == gen.Param1) &&
                       (0 == gen.Param2))
                    {
                        DiscParam.X = 0;
                        DiscParam.Y = (float)Math.PI * 2;
                    }
                    else
                    {
                        DiscParam.X = gen.Param1;
                        DiscParam.Y = gen.Param2;
                    }
                    break;
                case ParticleType.Line:
                    LineParam.X = gen.Param1;
                    LineParam.Y = gen.Param2;
                    LineParam.Z = gen.Param3;
                    break;
                case ParticleType.Tornado:
                    break;
                case ParticleType.Rect:
                    RectParam1.X = gen.Param1;
                    RectParam1.Y = gen.Param2;
                    RectParam1.Z = gen.Param3;

                    RectParam3.X = 0;
                    RectParam3.Y = gen.Param2;
                    RectParam3.Z = 0;

                    RectParam4.X = 0;
                    RectParam4.Y = 0;
                    RectParam4.Z = gen.Param3;

                    RectParam2.X = gen.Param1;
                    RectParam2.Y = 0;
                    RectParam2.Z = 0;

                    RectFlags = 0;
                    if (gen.Param1 < 0)
                        RectFlags |= 1;
                    if (gen.Param2 < 0)
                        RectFlags |= 2;
                    if (gen.Param3 < 0)
                        RectFlags |= 4;
                    break;
                case ParticleType.Cone:
                case ParticleType.Cylinder:
                    if ((0 == gen.Param1) &&
                       (0 == gen.Param2))
                    {
                        ConeParam.X = 0;
                        ConeParam.Y = (float)Math.PI * 2;
                    }
                    else
                    {
                        ConeParam.X = gen.Param1;
                        ConeParam.Y = gen.Param2;
                    }
                    ConeParam.Z = gen.Param3;
                    break;
                case ParticleType.Sphere:
                    SphereParam1.X = Velocity.Length;

                    var dVar6 = Velocity.Xz.Length;
                    if (1.17549435E-38 <= dVar6)
                        SphereParam1.Y = (float)Math.Atan2(Velocity.Y, dVar6);
                    else if (Velocity.Y < 0)
                        SphereParam1.Y = -(float)Math.PI / 2;
                    else
                        SphereParam1.Y = (float)Math.PI / 2;

                    if (1.17549435E-38 <= Math.Abs(Velocity.X)) //(float)((uint)Velocity.X & 0x7fffffff))
                        SphereParam2.X = (float)Math.Atan2(Velocity.Z, Velocity.X);
                    else if (Velocity.Z < 0)
                        SphereParam2.X = -(float)Math.PI / 2;
                    else
                        SphereParam2.X = (float)Math.PI / 2;

                    SphereParam1.Z = gen.Param1;
                    if (SphereParam1.Z < 0)
                    {
                        SphereParam1.Z = -SphereParam1.Z;
                        SphereParam1.X = -SphereParam1.X;
                    }
                    SphereParam2.Y = gen.Param2;

                    break;
            }

            if (Kind.HasFlag(ParticleKind.BillboardA))
            {
                Flags |= GeneratorFlags.BillboardA;
                AppSRT = new AppSRT();
                AppSRT.BillboardA = 1;
                AppSRT.Parent = this;
            }
        }

        private void PositionUpdate()
        {
            // TODO: 8039d214
        }

        private float generateParticle()
        {
            if (NumToSpawn < 1)
                return NumToSpawn;

            var local_c8 = new Vector3(Velocity);
            var vel_mag = local_c8.Length;

            // transform matrix
            var matrix = Matrix4.Identity;

            // 8039dc10 - 8039dce0
            // initial matrix stuff
            if (Flags.HasFlag(GeneratorFlags.x100) && 
                (Joint != null) &&
                Flags.HasFlag(GeneratorFlags.BillboardA) && 
                !Kind.HasFlag(ParticleKind.BillboardA | ParticleKind.BillboardG))
            {
                // gets rotation matrix from Joint

                //PSMTXCopy(param_1->joint->rotMtx, local_150);
                //spawn_vel.X = local_150[0][0];
                //spawn_vel.Y = local_150[1][0];
                //spawn_vel.Z = local_150[2][0];
                //PSVECNormalize(&spawn_vel, &spawn_vel);
                //local_120[0][0] = spawn_vel.X;
                //local_120[1][0] = spawn_vel.Y;
                //local_120[2][0] = spawn_vel.Z;
                //spawn_vel.X = local_150[0][1];
                //spawn_vel.Y = local_150[1][1];
                //spawn_vel.Z = local_150[2][1];
                //PSVECNormalize(&spawn_vel, &spawn_vel);
                //local_120[0][1] = spawn_vel.X;
                //local_120[1][1] = spawn_vel.Y;
                //local_120[2][1] = spawn_vel.Z;
                //spawn_vel.X = local_150[0][2];
                //spawn_vel.Y = local_150[1][2];
                //spawn_vel.Z = local_150[2][2];
                //PSVECNormalize(&spawn_vel, &spawn_vel);
                //local_120[0][2] = spawn_vel.X;
                //local_120[1][2] = spawn_vel.Y;
                //local_120[2][2] = spawn_vel.Z;
                //local_120[2][3] = FLOAT_804de9a8;
                //local_120[1][3] = FLOAT_804de9a8;
                //local_120[0][3] = FLOAT_804de9a8;
            }

            // 8039dce4 - 8039dde0 
            // billboard to camera
            if (Kind.HasFlag(ParticleKind.BillboardG))
            {
                // generate lookat matrix from camera eye to joint position

                //local_15c.X = (PTR_804d78f0->eye_position->pos).Y - (param_1->joint_position).X;
                //local_15c.Y = (PTR_804d78f0->eye_position->pos).Z - (param_1->joint_position).Y;
                //local_15c.Z = (float)PTR_804d78f0->eye_position->aobj - (param_1->joint_position).Z;
                //PSVECNormalize(&local_15c, &local_15c);
                //HSD_CObjGetUpVector(PTR_804d78f0, &local_168);
                //PSVECNormalize(&local_168, &local_168);
                //PSVECCrossProduct(&local_168, &local_15c, &local_174);
                //PSVECCrossProduct(&local_15c, &local_174, &local_168);
                //matrix.M11 = local_174.X;
                //matrix.M12 = local_174.Y;
                //matrix.M13 = local_174.Z;
                //matrix.M21 = local_168.X;
                //matrix.M22 = local_168.Y;
                //matrix.M23 = local_168.Z;
                //matrix.M31 = local_15c.X;
                //matrix.M32 = local_15c.Y;
                //matrix.M33 = local_15c.Z;
            }

            // 8039dde4 - 8039df60
            // calculate velocity rotation matrix
            if (Shape != ParticleType.Line && 
                vel_mag > 0)
            {
                var vn = new Vector3(Velocity).Normalized();

                float angle;

                if (Math.Abs(vn.Z) >= 0)
                    angle = (float)Math.Atan2(vn.Y, vn.Z);
                else if (vn.Y < 0)
                    angle = -(float)Math.PI / 2;
                else
                    angle = (float)Math.PI / 2;

                var dVar8 = (float)Math.Sin(angle);
                var dVar14 = (float)Math.Cos(angle);

                var fVar1 = (vn.Y * dVar8) + (vn.Z * dVar14);

                if (Math.Abs(fVar1) >= 0)
                    angle = (float)Math.Atan2(vn.X, fVar1);
                else if (vn.X < 0)
                    angle = -(float)Math.PI / 2;
                else
                    angle = (float)Math.PI / 2;

                var dVar9 = (float)Math.Sin(angle);
                var dVar16 = (float)Math.Cos(angle);

                float M11 = dVar16;
                float M12 = -dVar8 * dVar9;
                float M13 = -dVar14 * dVar9;

                float M21 = 0;
                float M22 = dVar14;
                float M23 = -dVar8;

                float M31 = dVar9;
                float M32 = dVar8 * dVar16;
                float M33 = dVar14 * dVar16;

                float M41 = 0;
                float M42 = 0;
                float M43 = 0;

                var rotMtx = new Matrix4(
                    M11, M12, M13, 0,
                    M21, M22, M23, 0,
                    M31, M32, M33, 0,
                    M41, M42, M43, 1);

                matrix = rotMtx * matrix;
            }

            // 8039df64 - 8039e024
            // special processing for tornado
            if (Shape == ParticleType.Tornado)
            {
                //float angle;
                //if (Math.Abs(matrix.M33) >= 0)
                //    angle = (float)Math.Atan2(matrix.M32, matrix.M33);
                //else if (matrix.M32 < 0)
                //    angle = -(float)Math.PI / 2;
                //else
                //    angle = (float)Math.PI / 2;

                //var dVar14 = matrix.M33 * (float)Math.Cos(angle);
                //var fVar1 = matrix.M32 * (float)Math.Sin(angle) + dVar14;

                //if (Math.Abs(fVar1) >= 0)
                //    dVar12 = (float)Math.Atan2(matrix.M31, fVar1);
                //else if (matrix.M31 < 0)
                //    dVar12 = -(float)Math.PI / 2;
                //else
                //    dVar12 = (float)Math.PI / 2;
            }

            //float in_f18 = 0;
            //float angle_y = 0;

            //// 8039e03c - 8039e168
            //if (Angle < 0)
            //{
            //    switch ((ParticleType)((int)Type & 0xF))
            //    {
            //        case ParticleType.Disc:
            //        case ParticleType.DiscCT:
            //        case ParticleType.DiscCD:
            //            in_f18 = (x60.Y - x60.X) / NumToSpawn;
            //            angle_y = in_f18 * (float)RandomGen.NextDouble() + x60.X;
            //            break;
            //        case ParticleType.Rect:
            //            in_f18 = (float)((Math.PI * 2) / NumToSpawn);
            //            angle_y = (float)((Math.PI * 2) * (float)RandomGen.NextDouble());
            //            break;
            //        case ParticleType.Cone:
            //        case ParticleType.Cylinder:
            //            in_f18 = (x60.Y - x60.X) / NumToSpawn;
            //            angle_y = in_f18 * (float)RandomGen.NextDouble() + x60.X;
            //            break;
            //    }
            //}

            // 8039e16c - 8039edac
            // process spawn type
            while (NumToSpawn >= 1)
            {
                switch (Shape)
                {
                    case ParticleType.Disc:
                    case ParticleType.DiscCT:
                    case ParticleType.DiscCD:
                    case ParticleType.Cone:
                    case ParticleType.Cylinder:
                        float radius;
                        float range;
                        float angle_x, angle_y;
                        var kind = Shape;

                        // generate spawn radius and range
                        if (Radius >= 0)
                        {
                            range = (float)RandomGen.NextDouble();
                            if (((kind == ParticleType.DiscCT) || (kind == ParticleType.DiscCD)) &&
                                range > 0)
                                range = (float)Math.Sqrt(range);
                            radius = range * Radius;
                        }
                        else
                        {
                            radius = -Radius;
                            range = 1;
                        }

                        // calculate angles to generate spawn position
                        switch (kind)
                        {
                            case ParticleType.Cylinder:
                                if (Angle >= 0)
                                {
                                    angle_y = ((ConeParam.Y - ConeParam.X) * (float)RandomGen.NextDouble() + ConeParam.X);
                                    angle_x = ((float)Math.PI / 2 + Angle);
                                }
                                else
                                {
                                    var in_f18 = (ConeParam.Y - ConeParam.X) / NumToSpawn;
                                    angle_y = in_f18 * (float)RandomGen.NextDouble() + ConeParam.X;

                                    angle_y += in_f18;
                                    angle_x = ((float)Math.PI / 2 - Angle);
                                }
                                break;
                            case ParticleType.Cone:
                                if (Angle >= 0)
                                {
                                    angle_y = (ConeParam.Y - ConeParam.X) * (float)RandomGen.Next() + ConeParam.X;

                                    if (Math.Abs(radius) > 0)
                                        angle_x = Angle + ((float)Math.PI / 2 - (float)Math.Atan2(ConeParam.Z, radius));
                                    else if (ConeParam.Z < 0)
                                        angle_x = (float)Math.PI + Angle;
                                    else
                                        angle_x = Angle;
                                }
                                else
                                {
                                    var in_f18 = (ConeParam.Y - ConeParam.X) / NumToSpawn;
                                    angle_y = in_f18 * (float)RandomGen.NextDouble() + ConeParam.X;

                                    angle_y += in_f18;

                                    if (Math.Abs(radius) > 0)
                                        angle_x = ((float)Math.PI / 2 - (float)Math.Atan2(ConeParam.Z, radius)) - Angle;
                                    else if (ConeParam.Z < 0)
                                        angle_x = (float)Math.PI - Angle;
                                    else
                                        angle_x = -Angle;
                                }
                                break;
                            default:
                                if (Angle >= 0)
                                {
                                    angle_x = (range * Angle);
                                    angle_y = (DiscParam.Y - DiscParam.X) * (float)RandomGen.NextDouble() + DiscParam.X;
                                }
                                else
                                {
                                    var in_f18 = (DiscParam.Y - DiscParam.X) / NumToSpawn;
                                    angle_y = in_f18 * (float)RandomGen.NextDouble() + DiscParam.X;
                                    angle_y += in_f18;
                                    angle_x = range * -Angle;
                                }
                                break;
                        }

                        Vector3 spawn_pos = new Vector3(
                            (float)(radius * Math.Cos(angle_y)),
                            (float)(radius * Math.Sin(angle_y)),
                            0);

                        // calculate z spawn position
                        if (kind == ParticleType.Cone || kind == ParticleType.Cylinder)
                        {
                            var rand = (float)RandomGen.NextDouble();
                            if (kind == ParticleType.Cone)
                            {
                                spawn_pos.X *= 1 - rand;
                                spawn_pos.Y *= 1 - rand;
                            }
                            spawn_pos.Z = rand * ConeParam.Z;
                        }
                        else
                        {
                            spawn_pos.Z = 0;
                        }

                        // calculate spawn velocity
                        var dVar10 = vel_mag * (float)Math.Sin(angle_x);
                        var spawn_vel = new Vector3(
                            (float)(dVar10 * Math.Cos(angle_y)),
                            (float)(dVar10 * Math.Sin(angle_y)),
                            (float)(vel_mag * Math.Cos(angle_x))
                            );

                        // more range for discCT?
                        if (kind == ParticleType.DiscCT)
                            spawn_vel *= range;

                        // transform by matrix
                        spawn_pos = Vector3.TransformPosition(spawn_pos, matrix);
                        spawn_pos += JointPosition;
                        spawn_vel = Vector3.TransformPosition(spawn_vel, matrix);

                        // spawn particle
                        Parent.AttachParticle(Particle.SpawnParticle(spawn_pos, spawn_vel,
                            Size, Gravity, LinkNo, Bank, Kind,
                            TexG, cmdList, Life, 0, this, Friction));
                        //psGenerateParticle((double)spawn_pos.X, (double)spawn_pos.Y, (double)spawn_pos.Z,
                        //                   (double)spawn_vel.X, (double)spawn_vel.Y, (double)spawn_vel.Z,
                        //                   (double)param_1->size, (double)param_1->gravity, param_1->link_no,
                        //                   param_1->bank, param_1->kind, param_1->texg, param_1->cmdList, param_1->life, 0,
                        //                   param_1, param_1->friction);
                        break;
                    case ParticleType.Line:
                        spawn_pos = LineParam * (float)RandomGen.NextDouble();
                        spawn_pos = Vector3.TransformPosition(spawn_pos, matrix) + JointPosition;
                        spawn_vel = Vector3.TransformPosition(Velocity, matrix);

                        // spawn particle
                        Parent.AttachParticle(Particle.SpawnParticle(spawn_pos, spawn_vel,
                            Size, Gravity, LinkNo, Bank, Kind,
                            TexG, cmdList, Life, 0, this, Friction));

                        //psGenerateParticle((double)spawn_pos.X, (double)spawn_pos.Y, (double)spawn_pos.Z,
                        //                   (double)spawn_vel.X, (double)spawn_vel.Y, (double)spawn_vel.Z,
                        //                   (double)param_1->size, (double)param_1->gravity, param_1->link_no,
                        //                   param_1->bank, param_1->kind, param_1->texg, param_1->cmdList, param_1->life, 0,
                        //                   param_1, param_1->friction);
                        break;
                    case ParticleType.Tornado:

                        float y_vel;
                        if (Radius >= 0)
                            y_vel = (float)RandomGen.NextDouble();
                        else
                            y_vel = 1;

                        float in_f25 = (float)((Math.PI * 2) * (float)RandomGen.NextDouble());
                        if (Angle >= 0)
                            in_f25 = (float)(2 * Math.PI * RandomGen.NextDouble());
                        else
                            in_f25 += (float)((Math.PI * 2) / NumToSpawn);

                        TornadoParam = vel_mag;

                        spawn_pos = new Vector3(0);
                        spawn_vel = new Vector3(in_f25, y_vel, 0);

                        Parent.AttachParticle(Particle.SpawnParticle(spawn_pos, spawn_vel,
                            Size, Gravity, LinkNo, Bank, Kind | ParticleKind.Tornado,
                            TexG, cmdList, Life, 0, this, Friction));

                        //psGenerateParticle(dVar6, dVar6, dVar6, in_f25, dVar5, dVar6, (double)param_1->size, dVar7,
                        //                   param_1->link_no, param_1->bank, param_1->kind | Tornado, param_1->texg,
                        //                   param_1->cmdList, param_1->life, 0, param_1, (float)dVar12);
                        break;
                    case ParticleType.Rect:
                        spawn_pos.X = (float)RandomGen.NextDouble();
                        spawn_pos.Y = (float)RandomGen.NextDouble();
                        spawn_pos.Z = (float)RandomGen.NextDouble();

                        switch (RectFlags)
                        {
                            case 1:
                                spawn_pos.X = spawn_pos.X > 0.5f ? 1 : 0;
                                break;
                            case 2:
                                spawn_pos.Y = spawn_pos.Y > 0.5f ? 1 : 0;
                                break;
                            case 3:
                                if (RandomGen.NextDouble() <= RectParam1.X / (RectParam1.X + RectParam1.Y))
                                    spawn_pos.X = spawn_pos.X > 0.5f ? 1 : 0;
                                else
                                    spawn_pos.Y = spawn_pos.Y > 0.5f ? 1 : 0;
                                break;
                            case 4:
                                spawn_pos.Z = spawn_pos.Z > 0.5f ? 1 : 0;
                                break;
                            case 5:
                                if (RandomGen.NextDouble() <= RectParam1.X / (RectParam1.X + RectParam1.Z))
                                    spawn_pos.X = spawn_pos.X > 0.5f ? 1 : 0;
                                else
                                    spawn_pos.Z = spawn_pos.Z > 0.5f ? 1 : 0;
                                break;
                            case 6:
                                if (RandomGen.NextDouble() <= RectParam1.Y / (RectParam1.Y + RectParam1.Z))
                                    spawn_pos.Y = spawn_pos.Y > 0.5f ? 1 : 0;
                                else
                                    spawn_pos.Z = spawn_pos.Z > 0.5f ? 1 : 0;
                                break;
                            case 7:
                                float dVar5 = (float)RandomGen.NextDouble();
                                float dVar6 = 1 / (RectParam1.X * (RectParam1.Y + RectParam1.Z) + RectParam1.Y * RectParam1.Z);
                                if ((dVar6 * (RectParam1.X * RectParam1.Y)) <= dVar5)
                                {
                                    if (dVar5 <= -(dVar6 * (RectParam1.X * RectParam1.Z) - 1))
                                        spawn_pos.X = spawn_pos.X > 0.5f ? 1 : 0;
                                    else
                                        spawn_pos.Y = spawn_pos.Y > 0.5f ? 1 : 0;
                                }
                                else
                                    spawn_pos.Z = spawn_pos.Z > 0.5f ? 1 : 0;
                                break;
                        }

                        spawn_pos -= new Vector3(0.5f);

                        Vector3 base_pos = new Vector3(
                            RectParam4.X * spawn_pos.Z +
                            RectParam2.X * spawn_pos.X +
                            RectParam3.X * spawn_pos.Y,

                            RectParam4.Y * spawn_pos.Z +
                            RectParam2.Y * spawn_pos.X +
                            RectParam3.Y * spawn_pos.Y,

                            RectParam4.Z * spawn_pos.Z +
                            RectParam2.Z * spawn_pos.X +
                            RectParam3.Z * spawn_pos.Y
                        );

                        spawn_pos = Vector3.TransformPosition(base_pos, matrix);
                        spawn_pos += JointPosition;

                        // calculate spawn_velocity
                        spawn_vel = RectParam4 * (vel_mag / RectParam4.LengthFast);

                        // transform by matrix
                        spawn_vel = Vector3.TransformPosition(spawn_vel, matrix);

                        // spawn particle
                        Parent.AttachParticle(Particle.SpawnParticle(spawn_pos, spawn_vel,
                            Size, Gravity, LinkNo, Bank, Kind,
                            TexG, cmdList, Life, 0, this, Friction));
                        //psGenerateParticle((double)spawn_pos.X, (double)spawn_pos.Y, (double)spawn_pos.Z,
                        //                   (double)spawn_vel.X, (double)spawn_vel.Y, (double)spawn_vel.Z,
                        //                   (double)param_1->size, (double)param_1->gravity, param_1->link_no,
                        //                   param_1->bank, param_1->kind, param_1->texg, param_1->cmdList, param_1->life, 0,
                        //                   param_1, param_1->friction);
                        break;
                    case ParticleType.Sphere:

                        // generate spawn radius
                        if ((0 == SphereParam1.Z) || (Math.Abs(SphereParam1.Z - (float)Math.PI) < 0.001f))
                        {
                            radius = (float)Math.PI / 2 * (float)Math.Sqrt(RandomGen.NextDouble());

                            if (RandomGen.NextDouble() < 0.5)
                                radius = (float)Math.PI - radius;
                        }
                        else
                        {
                            radius = SphereParam1.Z * (float)Math.Sqrt(RandomGen.NextDouble());
                        }

                        // idk
                        float dVar15 = radius;
                        if (dVar15 >= 0)
                            dVar15 = dVar15 * (float)Math.Sqrt(RandomGen.NextDouble());
                        else
                            dVar15 = -dVar15;
                        
                        // calculate spawn direction
                        float spawn_angle = (float)(2 * Math.PI * RandomGen.NextDouble());
                        Vector3 spawn_dir = new Vector3(
                            (float)(Math.Sin(radius) * Math.Cos(spawn_angle)),
                            (float)(Math.Sin(radius) * Math.Sin(spawn_angle)),
                            (float)Math.Cos(radius)
                            );

                        // multiple by matrix
                        spawn_dir = Vector3.TransformPosition(spawn_dir, matrix);

                        // calculate spawn velocity
                        spawn_vel = spawn_dir * SphereParam1.X;
                        if (radius >= 0 && SphereParam1.X < 0)
                            spawn_vel *= dVar15 / Radius;

                        // calculate spawn position
                        spawn_pos = dVar15 * spawn_dir + JointPosition;

                        // spawn particle
                        Parent.AttachParticle(Particle.SpawnParticle(spawn_pos, spawn_vel,
                            Size, Gravity, LinkNo, Bank, Kind,
                            TexG, cmdList, Life, 0, this, Friction));

                        //psGenerateParticle((double)spawn_pos.X, (double)spawn_pos.Y, (double)spawn_pos.Z,
                        //                   (double)spawn_vel.X, (double)spawn_vel.Y, (double)spawn_vel.Z,
                        //                   (double)param_1->size, (double)param_1->gravity, param_1->link_no,
                        //                   param_1->bank, param_1->kind, param_1->texg, param_1->cmdList, param_1->life, 0,
                        //                   param_1, param_1->friction);
                        break;
                    default:
                        //if (DAT_804d78e8 != (code*)0x0)
                        //{
                        //    (*DAT_804d78e8)(param_1, &local_120);
                        //}
                        break;
                }
                NumToSpawn -= 1;
            }

            return NumToSpawn;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {

            // check if still active
            if (Dead || Kind.HasFlag(ParticleKind.ExecPause))
                return;

            PositionUpdate();

            if (Random >= 0)
                NumToSpawn += Random * (float)RandomGen.NextDouble();
            else
                NumToSpawn -= Random;

            if (NumToSpawn >= 1)
                NumToSpawn = generateParticle();

            if (GenLife != 0)
            {
                GenLife -= 1;
                if (GenLife <= 1)
                    Dead = true;
            }
        }
    }

    public class AppSRT
    {
        public ParticleGenerator Parent;
        public byte BillboardA;
    }
}
