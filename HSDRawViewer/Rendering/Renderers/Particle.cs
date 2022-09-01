using HSDRaw.Common;
using HSDRaw.GX;
using HSDRawViewer.Rendering.GX;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSDRawViewer.Rendering.Shaders;

namespace HSDRawViewer.Rendering.Renderers
{
    public class Particle
    {
        public struct GXColor
        {
            public byte R, G, B, A;
        }

        public Particle Next;

        public ParticleKind Kind;

        public byte Bank;
        public byte TexG;
        public byte poseNum;
        public sbyte palNum;
        public short SizeCount;
        public short primColCount;
        public short envColCount;
        public GXColor primColor;
        public GXColor envCol;
        public short cmdWait;
        public byte loopCount;
        public byte linkNo;
        public short idNum;
        public byte[] cmdList;
        public short cmdPtr;
        public short cmdMarkPtr;
        public short cmdLoopPtr;
        public short Life;
        public Vector3 Velocity;
        public float Gravity;
        public float Friction;
        public Vector3 Pos;
        public float Size;
        public float Rotate;
        public short aCmpCount;
        public byte aCmpMode;
        public byte aCmpParam1;
        public byte aCmpParam2;
        public byte pJObjOfs;
        public short MatColCount;
        public short AmbColCount;
        public ushort RotateCount;
        public float SizeTarget;
        public float RotateTarget;
        public short PrimColRemain;
        public short EnvColRemain;
        public GXColor PrimColorTarget;
        public GXColor EnvColorTarget;
        public short MatColRemain;
        public short AmbColRemain;
        public short aCmpRemain;

        public byte aCmpParam1Target;
        public byte aCmpParam2Target;
        public byte matRGB;
        public byte matA;
        public byte ambRGB;
        public byte ambA;
        public byte matRGBTarget;
        public byte matATarget;
        public byte ambRGBTarget;
        public byte ambATarget;

        public float TrailAlpha;
        public ParticleGenerator Gen;
        public AppSRT AppSRT { get; set; }
        public int x90;

        public static Particle SpawnParticle(Vector3 pos, Vector3 vel, float size, float gravity, float link_no, byte bank, ParticleKind kind, byte tex_group, byte[] cmdList, short life, int param_15, ParticleGenerator gen, float friction)
        {
            return SpawnParticle0(
                pos, vel,
                size, gravity, null, link_no,
                bank, kind, tex_group, cmdList,
                life, param_15, friction, gen, 1);
        }

        private static Particle SpawnParticle0(
            Vector3 pos, Vector3 vel,
            float size, float gravity,
            Particle param_9,
            float link_no, byte bank, ParticleKind kind, byte tex_group,
            byte[] cmdList,
            short life, int param_16,
            float friction,
            ParticleGenerator gen,
            int param_19)
        {
            Particle p = new Particle()
            {
                idNum = gen == null ? (short)0 : gen.IDNum,
                AppSRT = gen == null ? null : gen.AppSRT,

                Bank = bank,
                linkNo = (byte)link_no,
                Kind = kind,
                TexG = tex_group,
                Pos = pos,
                Velocity = vel,
                Size = size,
                Gravity = gravity,
                Friction = friction,
                Life = (short)(life + 1),
                cmdList = cmdList,
                cmdMarkPtr = 0,
                cmdPtr = 0,
                cmdWait = 1,
                poseNum = 0,
                palNum = -1,
                primColor = new GXColor() { A = 255, B = 255, G = 255, R = 255 },
                envCol = new GXColor(),
                aCmpMode = 0x33,
                aCmpParam1 = 0,
                aCmpParam2 = 255,
                Gen = gen,
                matRGB = 255,
                matA = 255,
                ambRGB = 255,
                ambA = 255,
                TrailAlpha = 1,
            };

            if (gen != null)
                gen.ParticleNum += 1;

            if (param_16 != 0)
                p.Kind |= ParticleKind.ComTLUT;

            if (param_19 != 0)
                p.Process();

            return p;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private float Randf()
        {
            if (Gen != null)
                return (float)Gen.RandomGen.NextDouble();
            else
                return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ProcessRemains()
        {
            if (SizeCount != 0)
            {
                Size += (SizeTarget - Size) / (float)SizeCount;
                SizeCount -= 1;
            }

            if (primColCount != 0)
            {
                PrimColRemain -= 1;
                if (PrimColRemain == 0)
                {
                    primColCount = 0;
                    primColor.R = PrimColorTarget.R;
                    primColor.G = PrimColorTarget.G;
                    primColor.B = PrimColorTarget.B;
                    primColor.A = PrimColorTarget.A;
                }
            }

            if (envColCount != 0)
            {
                EnvColRemain -= 1;
                if (EnvColRemain == 0)
                {
                    envColCount = 0;
                    envCol.R = EnvColorTarget.R;
                    envCol.G = EnvColorTarget.G;
                    envCol.B = EnvColorTarget.B;
                    envCol.A = EnvColorTarget.A;
                }
            }

            if (MatColCount != 0)
            {
                MatColRemain -= 1;
                if (MatColRemain == 0)
                {
                    MatColCount = 0;
                    matRGB = matRGBTarget;
                    matA = matATarget;
                }
            }

            if (AmbColCount != 0)
            {
                AmbColRemain -= 1;
                if (AmbColRemain == 0)
                {
                    AmbColCount = 0;
                    ambRGB = ambRGBTarget;
                    ambA = ambATarget;
                }
            }

            if (aCmpCount != 0)
            {
                aCmpRemain -= 1;
                if (aCmpRemain == 0)
                {
                    aCmpCount = 0;
                    aCmpParam1 = aCmpParam1Target;
                    aCmpParam2 = aCmpParam2Target;
                }
            }

            if (RotateCount != 0)
            {
                Rotate += (RotateTarget - Rotate) / (float)RotateCount;
                RotateCount -= 1;
            }
        }

        private short ReadExtendedByte()
        {
            short b = cmdList[cmdPtr++];
            if ((b & 0x80) != 0)
            {
                return (short)((b & 0x7F) * 0x100 + cmdList[cmdPtr++]);
            }
            return b;
        }

        private float ReadFloat()
        {
            var d = new byte[]
                {
                    cmdList[cmdPtr + 3],
                    cmdList[cmdPtr + 2],
                    cmdList[cmdPtr + 1],
                    cmdList[cmdPtr],
                };
            cmdPtr += 4;
            return BitConverter.ToSingle(d, 0);
        }

        private short ReadShort()
        {
            return (short)(cmdList[cmdPtr++] * 0x100 + cmdList[cmdPtr++]);
        }

        private ushort ReadUShort()
        {
            return (ushort)(cmdList[cmdPtr++] * 0x100 + cmdList[cmdPtr++]);
        }

        private byte ReadByteRandomize(byte input)
        {
            input += (byte)(cmdList[cmdPtr++] * (float)Gen.RandomGen.NextDouble());

            if (input < 0)
                input = 0;

            if (input > 255)
                input = 255;

            return input;
        }

        private byte ClampAddByte(byte value, float amt)
        {
            int dVar45 = (int)(value + amt);
            if (dVar45 < 0)
                dVar45 = 0;
            if (dVar45 > 255)
                dVar45 = 255;
            return (byte)dVar45;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void RotateVelocity(float value)
        {
            float vx = Velocity.X;
            float vy = Velocity.Y;
            float vz = Velocity.Z;

            float yz;

            if (Math.Abs(Velocity.Z) > 0)
                yz = (float)Math.Atan2(vy, vz);
            else if (vy < 0)
                yz = -(float)Math.PI / 2;
            else
                yz = (float)Math.PI / 2;

            float dVar2 = (float)Math.Sin(yz);
            float dVar4 = (float)Math.Cos(yz);

            float fVar1 = vy * dVar2 + (vz * dVar4);
            float yza;
            if (Math.Abs(fVar1) > 0)
                yza = (float)Math.Atan2(vx, fVar1);
            else if (vx < 0)
                yza = -(float)Math.PI / 2;
            else
                yza = (float)Math.PI / 2;

            float dVar3 = (float)Math.Sin(yza);
            float dVar5 = (float)Math.Cos(yza);

            vz = Velocity.LengthFast;

            float __x = (float)(2 * Math.PI * Randf());

            vy = (float)(vz * Math.Sin(value));
            float dVar6 = (float)(vy * Math.Cos(__x));
            vy *= (float)Math.Sin(__x);
            vz *= (float)Math.Cos(value);

            Velocity.X = (dVar6 * dVar5 + (vz * dVar3));
            Velocity.Y = (dVar5 * (vz * dVar2) + (dVar3 * (-dVar6 * dVar2) + (vy * dVar4)));
            Velocity.Z = (dVar5 * (vz * dVar4) + (dVar3 * (-dVar6 * dVar4) - (vy * dVar2)));
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <param name="range"></param>
        /// <param name="speed"></param>
        private bool MoveToMatrix(Matrix4 m, float range, float speed)
        {
            if (range < 0)
                return false;

            var vec = m.ExtractTranslation() - Pos;

            if (range * range < vec.LengthSquared)
            {
                if (vec.LengthSquared == 0)
                {
                    return false;
                }
                else
                {
                    Velocity += vec * speed / vec.Length;
                    return false;
                }
            }
            else
                return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ProcessCommandList()
        {
            if (cmdWait == 0)
                return;

            cmdWait -= 1;

            if (cmdWait != 0)
                return;

            do
            {
                byte cmd = cmdList[cmdPtr++];

                if (cmd < 0x80)
                {
                    cmdWait = (short)(cmd & 0x1F);

                    if ((cmd & 0x20) != 0)
                    {
                        cmdWait = (short)(cmdWait * 0x100 + cmdList[cmdPtr++]);
                    }
                    if ((cmd & 0xC0) == 0x40)
                    {
                        poseNum = cmdList[cmdPtr++];
                        Kind |= ParticleKind.HasTexture;
                    }
                }
                else
                {
                    cmdWait = 0;
                    var final_cmd = cmd & 0xF8;

                    if ((cmd & 0xF8) > 0x98)
                    {
                        final_cmd = cmd & 0xF0;

                        if ((cmd & 0xF0) != 0xC0 && (cmd & 0xF0) != 0xD0)
                            final_cmd = cmd;
                    }

                    switch(final_cmd)
                    {
                        case 0x80: // Set Position
                            if ((cmd & 1) != 0)
                                Pos.X = ReadFloat();
                            if ((cmd & 2) != 0)
                                Pos.Y = ReadFloat();
                            if ((cmd & 4) != 0)
                                Pos.Z = ReadFloat();
                            break;
                        case 0x88: // Add Position
                            if ((cmd & 1) != 0)
                                Pos.X += ReadFloat();
                            if ((cmd & 2) != 0)
                                Pos.Y += ReadFloat();
                            if ((cmd & 4) != 0)
                                Pos.Z += ReadFloat();
                            break;
                        case 0x90: // set velocity
                            if ((cmd & 1) != 0)
                                Velocity.X = ReadFloat();
                            if ((cmd & 2) != 0)
                                Velocity.Y = ReadFloat();
                            if ((cmd & 4) != 0)
                                Velocity.Z = ReadFloat();
                            break;
                        case 0x98: // Add velocity
                            if ((cmd & 1) != 0)
                                Velocity.X += ReadFloat();
                            if ((cmd & 2) != 0)
                                Velocity.Y += ReadFloat();
                            if ((cmd & 4) != 0)
                                Velocity.Z += ReadFloat();
                            break;
                        case 0xA0: // Set Size Count and Target
                            SizeCount = ReadExtendedByte();
                            SizeTarget = ReadFloat();
                            if (SizeCount == 0)
                                Size = SizeTarget;
                            break;
                        case 0xA1: // Disable Texture
                            Kind &= ~ParticleKind.HasTexture;
                            break;
                        case 0xA2: // Set Gravity
                            Gravity = ReadFloat();
                            if (Gravity == 0)
                                Kind &= ~ParticleKind.Gravity;
                            else
                                Kind |= ParticleKind.Gravity;
                            break;
                        case 0xA3: // Set Friction
                            Friction = ReadFloat();
                            if (Friction == 0)
                                Kind &= ~ParticleKind.Friction;
                            else
                                Kind |= ParticleKind.Friction;
                            break;
                        case 0xA4: // spawns particle at position
                            {
                                int ptclId = ReadUShort();

                                var desc = Gen.Parent.GetDescriptor(ptclId);

                                var ptcl = SpawnParticle0(Vector3.Zero,
                                    new Vector3(desc.VX, desc.VY, desc.VZ),
                                    desc.Size, desc.Gravity, this, linkNo, Bank,
                                    desc.Kind, (byte)desc.TexGroup, desc.TrackData, desc.Life, 0,
                                    desc.Friction, null, 0);

                                if (ptcl != null)
                                {
                                    ptcl.idNum = idNum;
                                    ptcl.Gen = Gen;
                                    if (Gen != null)
                                        Gen.ParticleNum += 1;
                                    ptcl.AppSRT = AppSRT;
                                    ptcl.Pos.X = Pos.X;
                                    ptcl.Pos.Y = Pos.Y;
                                    ptcl.Pos.Z = Pos.Z;
                                    Gen.Parent.AttachParticle(ptcl);
                                    ptcl.Process();
                                }
                            }
                            break;
                        case 0xA5: // Spawn Generator at position
                            {
                                int ptclId = ReadUShort();

                                int bankid = ptclId / 1000;
                                int ptcli = ptclId % 1000;

                                if (Gen != null && Gen.Parent != null)
                                    Gen.Parent.SpawnGenerator(ptcli, Pos.X, Pos.Y, Pos.Z);
                            }
                            break;
                        case 0xA6: // Set Life
                            Life = (short)(ReadShort() + (short)(Randf() * ReadShort()));
                            break;
                        case 0xA7: // Chance to destroy
                            if (cmdList[cmdPtr++] >= 100 * Randf())
                            {
                                Life = 1;
                                return;
                            }
                            break;
                        case 0xA8: // randomize position
                            {
                                float v = ReadFloat();
                                Pos.X += 2 * v * Randf() - v;
                            }
                            {
                                float v = ReadFloat();
                                Pos.Y += 2 * v * Randf() - v;
                            }
                            {
                                float v = ReadFloat();
                                Pos.Z += 2 * v * Randf() - v;
                            }
                            break;
                        case 0xA9: // Rotate Velocty
                            RotateVelocity(ReadFloat());
                            break;
                        case 0xAA: // spawn random particle
                            {
                                var id = ReadShort() + (int)(Randf() * ReadShort());

                                var desc = Gen.Parent.GetDescriptor(id);

                                var ptcl = SpawnParticle0(Vector3.Zero,
                                    new Vector3(desc.VX, desc.VY, desc.VZ),
                                    desc.Size, desc.Gravity, this, linkNo, Bank,
                                    desc.Kind, (byte)desc.TexGroup, desc.TrackData, desc.Life, 0,
                                    desc.Friction, null, 0);

                                if (ptcl != null)
                                {
                                    ptcl.idNum = idNum;
                                    ptcl.Gen = Gen;
                                    if (Gen != null)
                                        Gen.ParticleNum += 1;
                                    ptcl.AppSRT = AppSRT;
                                    ptcl.Pos.X = Pos.X;
                                    ptcl.Pos.Y = Pos.Y;
                                    ptcl.Pos.Z = Pos.Z;
                                    Gen.Parent.AttachParticle(ptcl);
                                    ptcl.Process();
                                }
                            }
                            break;
                        case 0xAB:
                            Velocity *= ReadFloat();
                            break;
                        case 0xAC: // Set Size Count and Target Range
                            SizeCount = ReadExtendedByte();
                            SizeTarget = ReadFloat();
                            SizeTarget += ReadFloat() * Randf();
                            if (SizeCount == 0)
                                Size = SizeTarget;
                            break;
                        case 0xAD: // Enable PrimEnv
                            Kind |= ParticleKind.PrimEnv;
                            break;
                        case 0xAE: // No Mirror
                            Kind &= ~(ParticleKind.MirrorS|ParticleKind.MirrorT);
                            break;
                        case 0xAF: // Mirror S
                            Kind &= ~ParticleKind.MirrorT;
                            Kind |= ParticleKind.MirrorS;
                            break;
                        case 0xB0: // Mirror T
                            Kind &= ~ParticleKind.MirrorS;
                            Kind |= ParticleKind.MirrorT;
                            break;
                        case 0xB1: // Mirror Both
                            Kind |= ParticleKind.MirrorS | ParticleKind.MirrorT;
                            break;
                        case 0xB3: // Set Alpha Compare
                            if (aCmpCount != 0)
                            {
                                aCmpParam1 = (byte)BlendByte(aCmpParam1, aCmpParam1Target, aCmpRemain, aCmpCount);
                                aCmpParam2 = (byte)BlendByte(aCmpParam2, aCmpParam2Target, aCmpRemain, aCmpCount);
                            }

                            aCmpCount = ReadExtendedByte();
                            aCmpMode = cmdList[cmdPtr++];
                            aCmpParam1Target = cmdList[cmdPtr++];
                            aCmpParam2Target = cmdList[cmdPtr++];

                            if (aCmpCount == 0)
                            {
                                aCmpParam1 = aCmpParam1Target;
                                aCmpParam2 = aCmpParam2Target;
                                aCmpRemain = 0;
                                aCmpCount = 0;
                            }
                            else
                            {
                                aCmpRemain = aCmpCount;
                            }

                            break;
                        case 0xB4:
                            Kind |= ParticleKind.NearestTex;
                            break;
                        case 0xB5:
                            Kind &= ~ParticleKind.NearestTex;
                            break;
                        case 0xB6: // Set Rotate Count and Target
                            RotateCount = (ushort)ReadExtendedByte();
                            RotateTarget += ReadFloat();
                            if (RotateCount == 0)
                                Rotate = RotateTarget;
                            break;
                        case 0xB7: // Move toward JObj
                            {
                                Matrix4 jobjMatrix = Matrix4.Identity;
                                var fVar7 = (jobjMatrix.ExtractTranslation() - Pos);
                                float scale = Velocity.LengthFast / fVar7.LengthFast;
                                Velocity = fVar7 * scale;
                            }
                            break;
                        case 0xB8: // Move to JOBJ
                            {
                                byte jointindex = cmdList[cmdPtr++];
                                float range = ReadFloat();
                                float speed = ReadFloat();
                                if (MoveToMatrix(Matrix4.Identity, range, speed))
                                {
                                    Life = 1;
                                    return;
                                }
                            }
                            break;
                        case 0xB9: // spawns particle using current generator
                            {
                                int ptclId = ReadUShort();

                                var curr = Gen.BankGen;
                                var desc = Gen.Parent.GetDescriptor(ptclId);

                                var ptcl = SpawnParticle0(Vector3.Zero,
                                    new Vector3(curr.VX, curr.VY, curr.VZ),
                                    desc.Size, desc.Gravity, this, linkNo, Bank,
                                    desc.Kind, (byte)desc.TexGroup, desc.TrackData, desc.Life, (int)desc.Kind,
                                    desc.Friction, null, 0);

                                if (ptcl != null)
                                {
                                    ptcl.idNum = idNum;
                                    ptcl.Gen = Gen;
                                    if (Gen != null)
                                        Gen.ParticleNum += 1;
                                    ptcl.AppSRT = AppSRT;
                                    ptcl.Pos.X = Pos.X;
                                    ptcl.Pos.Y = Pos.Y;
                                    ptcl.Pos.Z = Pos.Z;
                                    Gen.Parent.AttachParticle(ptcl);
                                    ptcl.Process();
                                }
                            }
                            break;
                        case 0xBA: // Random prim color target
                            if (primColCount != 0)
                            {
                                primColor.R = (byte)BlendByte(primColor.R, PrimColorTarget.R, PrimColRemain, primColCount);
                                primColor.G = (byte)BlendByte(primColor.G, PrimColorTarget.G, PrimColRemain, primColCount);
                                primColor.B = (byte)BlendByte(primColor.B, PrimColorTarget.B, PrimColRemain, primColCount);
                                primColor.A = (byte)BlendByte(primColor.A, PrimColorTarget.A, PrimColRemain, primColCount);
                            }

                            PrimColorTarget.R = ReadByteRandomize(PrimColorTarget.R);
                            PrimColorTarget.G = ReadByteRandomize(PrimColorTarget.G);
                            PrimColorTarget.B = ReadByteRandomize(PrimColorTarget.B);
                            PrimColorTarget.A = ReadByteRandomize(PrimColorTarget.A);

                            if (primColCount == 0)
                            {
                                primColor.R = PrimColorTarget.R;
                                primColor.G = PrimColorTarget.G;
                                primColor.B = PrimColorTarget.B;
                                primColor.A = PrimColorTarget.A;
                                PrimColRemain = 0;
                            }
                            else
                            {
                                PrimColRemain = primColCount;
                            }

                            break;
                        case 0xBB:  // Random env color target
                            if (envColCount != 0)
                            {
                                envCol.R = (byte)BlendByte(envCol.R, EnvColorTarget.R, EnvColRemain, envColCount);
                                envCol.G = (byte)BlendByte(envCol.G, EnvColorTarget.G, EnvColRemain, envColCount);
                                envCol.B = (byte)BlendByte(envCol.B, EnvColorTarget.B, EnvColRemain, envColCount);
                                envCol.A = (byte)BlendByte(envCol.A, EnvColorTarget.A, EnvColRemain, envColCount);
                            }

                            EnvColorTarget.R = ReadByteRandomize(EnvColorTarget.R);
                            EnvColorTarget.G = ReadByteRandomize(EnvColorTarget.G);
                            EnvColorTarget.B = ReadByteRandomize(EnvColorTarget.B);
                            EnvColorTarget.A = ReadByteRandomize(EnvColorTarget.A);

                            if (primColCount == 0)
                            {
                                envCol.R = EnvColorTarget.R;
                                envCol.G = EnvColorTarget.G;
                                envCol.B = EnvColorTarget.B;
                                envCol.A = EnvColorTarget.A;
                                EnvColRemain = 0;
                            }
                            else
                            {
                                EnvColRemain = envColCount;
                            }

                            break;
                        case 0xBC: // Set Pose Num Random
                            poseNum = (byte)(cmdList[cmdPtr++] + (cmdList[cmdPtr++] * Randf()));
                            Kind |= ParticleKind.HasTexture;
                            break;
                        case 0xBD: // Randomize Velocity
                            {
                                var value = ReadFloat();
                                var value_rand = ReadFloat();
                                var random_num = Randf();

                                var velocity_mag = Velocity.LengthFast;

                                if (velocity_mag > 1.0E-10)
                                    Velocity *= (value_rand * random_num + value) / velocity_mag;
                            }
                            break;
                        case 0xBE:
                            Velocity.X *= ReadFloat();
                            Velocity.Y *= ReadFloat();
                            Velocity.Z *= ReadFloat();
                            break;
                        case 0xBF:
                            Kind |= ParticleKind.PNTJOBJ | (ParticleKind)((pJObjOfs + cmdList[cmdPtr++]) * 0x1000 & 0x7000);
                            break;
                        case 0xC0: // Set Prim Color and Count
                            if (primColCount != 0)
                            {
                                primColor.R = (byte)BlendByte(primColor.R, PrimColorTarget.R, PrimColRemain, primColCount);
                                primColor.G = (byte)BlendByte(primColor.G, PrimColorTarget.G, PrimColRemain, primColCount);
                                primColor.B = (byte)BlendByte(primColor.B, PrimColorTarget.B, PrimColRemain, primColCount);
                                primColor.A = (byte)BlendByte(primColor.A, PrimColorTarget.A, PrimColRemain, primColCount);
                            }

                            primColCount = ReadExtendedByte();

                            if ((cmd & 1) != 0)
                                PrimColorTarget.R = cmdList[cmdPtr++];
                            if ((cmd & 2) != 0)
                                PrimColorTarget.G = cmdList[cmdPtr++];
                            if ((cmd & 4) != 0)
                                PrimColorTarget.B = cmdList[cmdPtr++];
                            if ((cmd & 8) != 0)
                                PrimColorTarget.A = cmdList[cmdPtr++];

                            if (primColCount == 0)
                            {
                                primColor.R = PrimColorTarget.R;
                                primColor.G = PrimColorTarget.G;
                                primColor.B = PrimColorTarget.B;
                                primColor.A = PrimColorTarget.A;
                                PrimColRemain = 0;
                            }
                            else
                            {
                                PrimColRemain = primColCount;
                            }

                            break;
                        case 0xD0: // Set Env Color and Count
                            if (envColCount != 0)
                            {
                                envCol.R = (byte)BlendByte(envCol.R, EnvColorTarget.R, EnvColRemain, envColCount);
                                envCol.G = (byte)BlendByte(envCol.G, EnvColorTarget.G, EnvColRemain, envColCount);
                                envCol.B = (byte)BlendByte(envCol.B, EnvColorTarget.B, EnvColRemain, envColCount);
                                envCol.A = (byte)BlendByte(envCol.A, EnvColorTarget.A, EnvColRemain, envColCount);
                            }

                            envColCount = ReadExtendedByte();

                            if ((cmd & 1) != 0)
                                EnvColorTarget.R = cmdList[cmdPtr++];
                            if ((cmd & 2) != 0)
                                EnvColorTarget.G = cmdList[cmdPtr++];
                            if ((cmd & 4) != 0)
                                EnvColorTarget.B = cmdList[cmdPtr++];
                            if ((cmd & 8) != 0)
                                EnvColorTarget.A = cmdList[cmdPtr++];

                            if (envColCount == 0)
                            {
                                envCol.R = EnvColorTarget.R;
                                envCol.G = EnvColorTarget.G;
                                envCol.B = EnvColorTarget.B;
                                envCol.A = EnvColorTarget.A;
                                EnvColRemain = 0;
                            }
                            else
                            {
                                EnvColRemain = envColCount;
                            }

                            break;
                        case 0xE0:
                            if (primColCount != 0)
                            {
                                primColor.R = (byte)BlendByte(primColor.R, PrimColorTarget.R, PrimColRemain, primColCount);
                                primColor.G = (byte)BlendByte(primColor.G, PrimColorTarget.G, PrimColRemain, primColCount);
                                primColor.B = (byte)BlendByte(primColor.B, PrimColorTarget.B, PrimColRemain, primColCount);
                                primColor.A = (byte)BlendByte(primColor.A, PrimColorTarget.A, PrimColRemain, primColCount);
                            }

                            if (envColCount != 0)
                            {
                                envCol.R = (byte)BlendByte(envCol.R, EnvColorTarget.R, EnvColRemain, envColCount);
                                envCol.G = (byte)BlendByte(envCol.G, EnvColorTarget.G, EnvColRemain, envColCount);
                                envCol.B = (byte)BlendByte(envCol.B, EnvColorTarget.B, EnvColRemain, envColCount);
                                envCol.A = (byte)BlendByte(envCol.A, EnvColorTarget.A, EnvColRemain, envColCount);
                            }

                            {
                                var amt = (byte)(cmdList[cmdPtr++] * Randf());
                                PrimColorTarget.R = ClampAddByte(PrimColorTarget.R, amt);
                                EnvColorTarget.R = ClampAddByte(EnvColorTarget.R, amt);
                            }
                            {
                                var amt = (byte)(cmdList[cmdPtr++] * Randf());
                                PrimColorTarget.G = ClampAddByte(PrimColorTarget.G, amt);
                                EnvColorTarget.G = ClampAddByte(EnvColorTarget.G, amt);
                            }
                            {
                                var amt = (byte)(cmdList[cmdPtr++] * Randf());
                                PrimColorTarget.B = ClampAddByte(PrimColorTarget.B, amt);
                                EnvColorTarget.B = ClampAddByte(EnvColorTarget.B, amt);
                            }
                            {
                                var amt = (byte)(cmdList[cmdPtr++] * Randf());
                                PrimColorTarget.A = ClampAddByte(PrimColorTarget.A, amt);
                                EnvColorTarget.A = ClampAddByte(EnvColorTarget.A, amt);
                            }

                            if (primColCount == 0)
                            {
                                primColor.R = PrimColorTarget.R;
                                primColor.G = PrimColorTarget.G;
                                primColor.B = PrimColorTarget.B;
                                primColor.A = PrimColorTarget.A;
                                PrimColRemain = 0;
                            }
                            else
                            {
                                PrimColRemain = primColCount;
                            }

                            if (envColCount == 0)
                            {
                                envCol.R = EnvColorTarget.R;
                                envCol.G = EnvColorTarget.G;
                                envCol.B = EnvColorTarget.B;
                                envCol.A = EnvColorTarget.A;
                                EnvColRemain = 0;
                            }
                            else
                            {
                                EnvColRemain = envColCount;
                            }

                            break;
                        case 0xE1: // Set callback
                            int cbIndex = cmdList[cmdPtr++];
                            if (cbIndex == 0)
                            {

                            }
                            else
                            {
                                // TODO: what do these callbacks do?
                            }
                            break;
                        case 0xE2: // set bit4
                            Kind |= ParticleKind.Bit4;
                            break;
                        case 0xE3: // set palNum
                            palNum = (sbyte)cmdList[cmdPtr++];
                            break;
                        case 0xE4: // Set FlipS
                            {
                                var flip = cmdList[cmdPtr++];
                                switch (flip)
                                {
                                    case 0:
                                        Kind &= ~ParticleKind.FlipS;
                                        break;
                                    case 1:
                                        Kind |= ParticleKind.FlipS;
                                        break;
                                    case 2:
                                        Kind ^= ParticleKind.FlipS;
                                        break;
                                    case 3:
                                        if (Randf() > 0.5)
                                            Kind &= ~ParticleKind.FlipS;
                                        else
                                            Kind |= ParticleKind.FlipS;
                                        break;
                                }
                            }
                            break;
                        case 0xE5: // Set FlipT
                            {
                                var flip = cmdList[cmdPtr++];
                                switch (flip)
                                {
                                    case 0:
                                        Kind &= ~ParticleKind.FlipT;
                                        break;
                                    case 1:
                                        Kind |= ParticleKind.FlipT;
                                        break;
                                    case 2:
                                        Kind ^= ParticleKind.FlipT;
                                        break;
                                    case 3:
                                        if (Randf() > 0.5)
                                            Kind &= ~ParticleKind.FlipT;
                                        else
                                            Kind |= ParticleKind.FlipT;
                                        break;
                                }
                            }
                            break;
                        case 0xE6: // Enable DirVec
                            Kind |= ParticleKind.DirVec;
                            break;
                        case 0xE7: // Disable DirVec
                            Kind &= ~ParticleKind.DirVec;
                            break;
                        case 0xE8: // Set Trail
                            var a = ReadFloat();
                            if (a > 0)
                            {
                                Kind |= ParticleKind.Trail;
                                TrailAlpha = a;
                            }
                            else
                            {
                                Kind &= ~ParticleKind.Trail;
                            }
                            break;
                        case 0xE9:
                            {
                                if (primColCount != 0)
                                {
                                    primColor.R = (byte)BlendByte(primColor.R, PrimColorTarget.R, PrimColRemain, primColCount);
                                    primColor.G = (byte)BlendByte(primColor.G, PrimColorTarget.G, PrimColRemain, primColCount);
                                    primColor.B = (byte)BlendByte(primColor.B, PrimColorTarget.B, PrimColRemain, primColCount);
                                    primColor.A = (byte)BlendByte(primColor.A, PrimColorTarget.A, PrimColRemain, primColCount);
                                }
                                if (envColCount != 0)
                                {
                                    envCol.R = (byte)BlendByte(envCol.R, EnvColorTarget.R, EnvColRemain, envColCount);
                                    envCol.G = (byte)BlendByte(envCol.G, EnvColorTarget.G, EnvColRemain, envColCount);
                                    envCol.B = (byte)BlendByte(envCol.B, EnvColorTarget.B, EnvColRemain, envColCount);
                                    envCol.A = (byte)BlendByte(envCol.A, EnvColorTarget.A, EnvColRemain, envColCount);
                                }

                                byte flag = cmdList[cmdPtr++];
                                byte step = cmdList[cmdPtr++];
                                float rand = Randf();
                                float value;
                                if (step == 0)
                                    value = rand;
                                else
                                    value = ((step + 1) * rand) / step;

                                if ((flag & 1) != 0)
                                {
                                    float amt = value * cmdList[cmdPtr++];

                                    if ((flag & 0x10) != 0)
                                        PrimColorTarget.R = ClampAddByte(PrimColorTarget.R, amt);

                                    if ((flag & 0x20) != 0)
                                        EnvColorTarget.R = ClampAddByte(EnvColorTarget.R, amt);
                                }

                                if ((flag & 2) != 0)
                                {
                                    float amt = value * cmdList[cmdPtr++];

                                    if ((flag & 0x10) != 0)
                                        PrimColorTarget.G = ClampAddByte(PrimColorTarget.G, amt);

                                    if ((flag & 0x20) != 0)
                                        EnvColorTarget.G = ClampAddByte(EnvColorTarget.G, amt);
                                }

                                if ((flag & 4) != 0)
                                {
                                    float amt = value * cmdList[cmdPtr++];

                                    if ((flag & 0x10) != 0)
                                        PrimColorTarget.B = ClampAddByte(PrimColorTarget.B, amt);

                                    if ((flag & 0x20) != 0)
                                        EnvColorTarget.B = ClampAddByte(EnvColorTarget.B, amt);
                                }

                                if ((flag & 8) != 0)
                                {
                                    float amt = value * cmdList[cmdPtr++];

                                    if ((flag & 0x10) != 0)
                                        PrimColorTarget.A = ClampAddByte(PrimColorTarget.A, amt);

                                    if ((flag & 0x20) != 0)
                                        EnvColorTarget.A = ClampAddByte(EnvColorTarget.A, amt);
                                }

                                if (primColCount == 0)
                                    primColor = PrimColorTarget;
                                PrimColRemain = primColCount;

                                if (envColCount == 0)
                                    envCol = EnvColorTarget;
                                EnvColRemain = envColCount;
                            }
                            break;
                        case 0xEA: // mat col
                            {
                                if (MatColCount != 0)
                                {
                                    matRGB = (byte)BlendByte(matRGB, matRGBTarget, MatColRemain, MatColCount);
                                    matA = (byte)BlendByte(matA, matATarget, MatColRemain, MatColCount);
                                }

                                MatColCount = ReadExtendedByte();

                                var flag = cmdList[cmdPtr++];

                                matRGBTarget = matRGB;

                                if ((flag & 1) != 0)
                                    matRGBTarget = cmdList[cmdPtr++];

                                if ((flag & 8) != 0)
                                    matATarget = cmdList[cmdPtr++];

                                if (MatColCount == 0)
                                {
                                    matRGB = matRGBTarget;
                                    MatColRemain = 0;
                                }
                                else
                                    MatColRemain = MatColCount;

                            }
                            break;
                        case 0xEB: // amb col
                            {
                                if (AmbColCount != 0)
                                {
                                    ambRGB = (byte)BlendByte(ambRGB, ambRGBTarget, AmbColRemain, AmbColCount);
                                    ambA = (byte)BlendByte(ambA, ambATarget, AmbColRemain, AmbColCount);
                                }

                                AmbColCount = ReadExtendedByte();

                                var flag = cmdList[cmdPtr++];

                                ambRGBTarget = ambRGB;

                                if ((flag & 1) != 0)
                                    ambRGBTarget = cmdList[cmdPtr++];

                                if ((flag & 8) != 0)
                                    ambATarget = cmdList[cmdPtr++];

                                if (AmbColCount == 0)
                                {
                                    ambRGB = ambRGBTarget;
                                    AmbColRemain = 0;
                                }
                                else
                                    AmbColRemain = AmbColCount;

                            }
                            break;
                        case 0xED: // Add Random Rotation
                            {
                                var base_value = ReadFloat();
                                var random_range = ReadFloat();
                                var step = cmdList[cmdPtr++];
                                float rand = Randf();
                                if (step == 0)
                                {
                                    base_value += random_range * rand;
                                }
                                else
                                {
                                    base_value += (random_range * ((step + 1) * rand)) / step;
                                }
                                RotateTarget += base_value;
                                Rotate += base_value;
                            }
                            break;
                        case 0xEF: // Create Generate with flag
                            {
                                var particleId = ReadUShort();
                                var flag = cmdList[cmdPtr++];

                                if (Gen != null && Gen.Parent != null)
                                {
                                    var gen = Gen.Parent.SpawnGenerator(particleId, Pos.X, Pos.Y, Pos.Z);

                                    gen.Kind &= ~(ParticleKind.BitGroup);
                                    gen.Kind |= (ParticleKind)((flag & 7) << 0x19);
                                }
                            }
                            break;
                        case 0xFA: // Set Loop
                            loopCount = cmdList[cmdPtr++];
                            cmdLoopPtr = cmdPtr;
                            break;
                        case 0xFB: // Execute Loop
                            loopCount -= 1;
                            if (loopCount != 0)
                                cmdPtr = cmdLoopPtr;
                            break;
                        case 0xFC: // Set Mark Pointer
                            cmdMarkPtr = cmdPtr;
                            break;
                        case 0xFD: // Goto Mark Pointer
                            cmdPtr = cmdMarkPtr;
                            break;
                        case 0xFE: // End
                        case 0xFF:
                            Life = 1;
                            return;

                        //-------------------------------------------------------------------------
                        // The following op codes are not functional currently
                        // -------------------------------------------------------------------------

                        case 0xB2: // remove appsrt
                            {
                                // transforms by appsrt before removing
                            }
                            break;       
                        case 0xEC: // set userdata value
                            {
                                int userdata_index = cmdList[cmdPtr++];
                                float value = ReadFloat();
                            }
                            break;
                        case 0xF0: // spawns particle
                            {
                                int ptclId = ReadShort();
                            }
                            break;
                        case 0xF1: // spawns particle
                            {
                                int ptclId = ReadShort();
                            }
                            break;
                        case 0xF2: // spawns particle
                            {
                                int ptclId = ReadShort();
                            }
                            break;
                        
                        default:
                            throw new NotSupportedException($"Particle Command {final_cmd.ToString("X")} not supported");
                    }
                }

            } while (cmdWait == 0);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Process()
        {
            if (Kind.HasFlag(ParticleKind.ExecPause))
                return;

            // process counts and remains
            ProcessRemains();

            // process cmd list
            ProcessCommandList();

            // process life
            Life -= 1;

            // process physics
            if (Life != 0)
            {
                if (Kind.HasFlag(ParticleKind.Tornado))
                {
                    // TODO: tornado physics
                    var gen = Gen;
                    float dVar36 = (float)Math.Sin(Gravity);
                    float dVar37 = (float)Math.Sin(Friction);
                    float dVar38 = (float)Math.Cos(Gravity);
                    float dVar39 = (float)Math.Cos(Friction);

                    Velocity.Z += gen.TornadoParam;
                    Velocity.X += gen.Gravity;

                    float dVar44 = (Velocity.Z * (float)Math.Tan(Math.Abs(gen.Angle)) + Math.Abs(gen.Radius)) * Velocity.Y;
                    float dVar41 = dVar44 * (float)Math.Cos(Velocity.X);
                    float dVar43 = (float)Math.Sin(Velocity.X);
                    float dVar40 = Velocity.Z;

                    Pos.X = gen.JointPosition.X +
                        dVar41 * dVar39 + dVar40 * dVar37;
                    Pos.Y = gen.JointPosition.Y +
                        (dVar39 * (dVar40 * dVar36) +
                        (dVar37 * (-dVar41 * dVar36) +
                        ((dVar44 * dVar43) * dVar38)));
                    Pos.Z = gen.JointPosition.Z +
                        (dVar39 * (dVar40 * dVar38) +
                        (dVar37 * (-dVar41 * dVar38) -
                        ((dVar44 * dVar43) * dVar36)));
                }
                else
                {
                    // normal physics
                    if (Kind.HasFlag(ParticleKind.Gravity))
                        Velocity.Y = Velocity.Y - Gravity;

                    if (Kind.HasFlag(ParticleKind.Friction))
                        Velocity *= Friction;

                    Pos += Velocity;
                }

                // TODO: pntjobj stuff...
            }
        }

        private Matrix4 psViewMatrix;
        private Matrix4 InvertedViewMatrix;
        private Matrix4 psProjViewMatrix;

        private static byte[,] TexFlipCoords = new byte[,]
        {
            { 0x00, 0x01, 0x00, 0x00, 0x01, 0x00, 0x01, 0x01, },
            { 0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x01, },
            { 0x00, 0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x00, },
            { 0x01, 0x00, 0x01, 0x01, 0x00, 0x01, 0x00, 0x00, }
        };

        private float StartX;
        private float EndX;
        private float StartY;
        private float EndY;
        private float StartZ;
        private float EndZ;

        /// <summary>
        /// 
        /// </summary>
        private void UpdatePosition(Camera cam)
        {
            psViewMatrix = cam.ModelViewMatrix;
            var inv = psViewMatrix.Inverted();
            psProjViewMatrix = cam.MvpMatrix;

            StartX = inv.M11 + inv.M21;
            EndX = inv.M11 - inv.M21;
            StartY = inv.M12 + inv.M22;
            EndY = inv.M12 - inv.M22;
            StartZ = inv.M13 + inv.M23;
            EndZ = inv.M13 - inv.M23;

            InvertedViewMatrix = inv;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="target"></param>
        /// <param name="remain"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private static int BlendByte(byte param, byte target, short remain, short count)
        {
            if (count <= 0)
                return param;

            return (target * 0x10000 + ((remain << 0x10) / count) * (param - target)) >> 0x10;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        public void Render(Camera cam, ParticleShader _shader, List<int> texg, int piVar34 = 0)
        {
            _shader.MVP = cam.MvpMatrix;

            UpdatePosition(cam);

            GL.PushAttrib(AttribMask.AllAttribBits);

            if (texg != null && poseNum < texg.Count)
            {
                GL.Enable(EnableCap.Texture2D);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, texg[poseNum]);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.MirroredRepeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.MirroredRepeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                    Kind.HasFlag(ParticleKind.NearestTex) 
                    ? (int)TextureMagFilter.Nearest : (int)TextureMagFilter.Linear);

                // set wrap modes
                if (Kind.HasFlag(ParticleKind.MirrorS))
                    _shader.TexScaleX = 2;
                else
                    _shader.TexScaleX = 1;

                if (Kind.HasFlag(ParticleKind.MirrorT))
                    _shader.TexScaleY = 2;
                else
                    _shader.TexScaleY = 1;

            }

            // 803a02cc - 803a0780
            // initialize GX

            // 803a0784 - 803a07ec
            // setup blend mode
            GL.Enable(EnableCap.Blend);
            switch (((int)Kind >> 0x16) & 3)
            {
                case 0:
                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                    break;
                case 1:
                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
                    break;
                default:
                    GL.Disable(EnableCap.Blend);
                    break;
            }

            // 803a07f0 - 803a08a0
            // setup alpha compare
            // GL.Enable(EnableCap.AlphaTest);
            int ref1;
            int ref2;
            if (aCmpCount == 0)
            {
                ref1 = aCmpParam1;
                ref2 = aCmpParam2;
            }
            else
            {
                ref1 = BlendByte(aCmpParam1, aCmpParam1Target, aCmpRemain, aCmpCount);
                ref2 = BlendByte(aCmpParam2, aCmpParam2Target, aCmpRemain, aCmpCount);
            }
            _shader.AlphaOp = (GXAlphaOp)(aCmpMode >> 6);
            _shader.AlphaComp0 = (GXCompareType)(aCmpMode >> 3 & 7);
            _shader.AlphaComp1 = (GXCompareType)(aCmpMode & 7);
            _shader.AlphaRef0 = ref1 / 255f;
            _shader.AlphaRef1 = ref2 / 255f;

            // 803a08a4 - 803a091c
            // init tev and lighting

            // 803a0a00 - 803a0b60
            // init lighting

            // 803a0b64 - 803a0c64
            // prim env setup
            _shader.PrimitiveColor.X = BlendByte(primColor.R, PrimColorTarget.R, PrimColRemain, primColCount) / 255f;
            _shader.PrimitiveColor.Y = BlendByte(primColor.G, PrimColorTarget.G, PrimColRemain, primColCount) / 255f;
            _shader.PrimitiveColor.Z = BlendByte(primColor.B, PrimColorTarget.B, PrimColRemain, primColCount) / 255f;
            _shader.PrimitiveColor.W = BlendByte(primColor.A, PrimColorTarget.A, PrimColRemain, primColCount) / 255f;

            // 803a0cc8 - 803a0d54
            // environment setup
            _shader.EnvironmentColor.X = BlendByte(envCol.R, EnvColorTarget.R, EnvColRemain, envColCount) / 255f;
            _shader.EnvironmentColor.Y = BlendByte(envCol.G, EnvColorTarget.G, EnvColRemain, envColCount) / 255f;
            _shader.EnvironmentColor.Z = BlendByte(envCol.B, EnvColorTarget.B, EnvColRemain, envColCount) / 255f;
            _shader.EnvironmentColor.W = BlendByte(envCol.A, EnvColorTarget.A, EnvColRemain, envColCount) / 255f;
            _shader.EnablePrimEnv = Kind.HasFlag(ParticleKind.PrimEnv);

            // 803a0dd0 - 803a0e00
            // z mode setup
            GL.Disable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            // GL.DepthMask(true);// Kind.HasFlag(ParticleKind.Bit4));

            // 803a0e14 - 803a0e34
            // TODO: fog for particles

            // 803a0e40 - 803a0e88
            // texgroup setup

            // 803a0e90 - 803a11c4
            // load texture

            // 803a11c8 - 803a2c04
            // TODO: point light for particles
            if (!Kind.HasFlag(ParticleKind.Point))
            {
                //if (AppSRT == null)
                {
                    float pos_x = Pos.X;
                    float pos_y = Pos.Y;
                    float pos_z = Pos.Z;

                    Vector3 s = Vector3.Zero;
                    Vector3 e = Vector3.Zero;

                    if (piVar34 == 0)
                    {
                        s.X = StartX * Size;
                        e.X = EndX * Size;
                        s.Y = StartY * Size;
                        e.Y = EndY * Size;
                        s.Z = StartZ * Size;
                        e.Z = EndZ * Size;
                    }
                    else
                    {
                        s.X = InvertedViewMatrix.M11 * Size;
                        s.Y = InvertedViewMatrix.M12 * Size;
                        s.Z = InvertedViewMatrix.M13 * Size;
                        e.X = -InvertedViewMatrix.M21 * Size;
                        e.Y = -InvertedViewMatrix.M22 * Size;
                        e.Z = -InvertedViewMatrix.M23 * Size;
                    }

                    var rotation = Rotate;

                    // todo 803a2e88 - 803a30ec
                    if (Kind.HasFlag(ParticleKind.Trail) || Kind.HasFlag(ParticleKind.DirVec))
                    {
                        float prev_x = 0, prev_y = 0, prev_z = 0;
                        if (!Kind.HasFlag(ParticleKind.Tornado))
                        {
                            prev_x = pos_x - Velocity.X;
                            prev_y = pos_y - Velocity.Y;
                            prev_z = pos_z - Velocity.Z;
                        }
                        else
                        {
                            // TODO: tornado
                        }

                        float tz_pos = psViewMatrix.M43 + (psViewMatrix.M33 * pos_z + (psViewMatrix.M13 * pos_x + (psViewMatrix.M23 * pos_y)));
                        float tz_prev = psViewMatrix.M43 + (psViewMatrix.M33 * prev_z + (psViewMatrix.M13 * prev_x + (psViewMatrix.M23 * prev_y)));
                        if (0 != tz_pos && 0 != tz_prev)
                        {
                            var vpos = Vector3.TransformPosition(Pos, psProjViewMatrix);
                            var vprev = Vector3.TransformPosition(new Vector3(prev_x, prev_y, prev_z), psProjViewMatrix);
                            var cfin = vpos.Xy / -tz_pos - vprev.Xy / -tz_prev;

                            if (Math.Abs(cfin.Y) >= 0)
                                rotation = (float)Math.Atan2(cfin.X, cfin.Y);
                            else if (cfin.X < 0)
                                rotation = -(float)Math.PI / 2;
                            else
                                rotation = (float)Math.PI / 2;
                        }

                        if (Kind.HasFlag(ParticleKind.DirVec))
                            rotation += Rotate;
                    }

                    if (Math.Abs(rotation) > 0.01) 
                    {
                        Vector3 local_714 = new Vector3(
                            s.Y * e.Z - (s.Z * e.Y),
                            s.Z * e.X - (s.X * e.Z),
                            s.X * e.Y - (s.Y * e.X));

                        var axis = Matrix4.CreateFromAxisAngle(local_714, rotation); // PSMTXRotAxisRad(&local_744, &local_714);

                        s = Vector3.TransformNormal(s, axis);
                        e = Vector3.TransformNormal(e, axis);
                    }

                    if (Kind.HasFlag(ParticleKind.Trail))
                    {
                        var local_6ec = Vector3.Zero;
                        if (Kind.HasFlag(ParticleKind.Tornado))
                        {
                            // TODO: calcTornadoLastPos
                        }
                        else
                        {
                            local_6ec.X = pos_x - Velocity.X;
                            local_6ec.Y = pos_y - Velocity.Y;
                            local_6ec.Z = pos_z - Velocity.Z;
                        }

                        // some lighting stuff

                        //if (piVar34 == 0x0)
                        {
                            int flip = ((int)Kind >> 0x12) & 0x3;
                            _shader.HasTexture = Kind.HasFlag(ParticleKind.HasTexture);

                            float[] v = new float[]
                            {
                                local_6ec.X - s.X, local_6ec.Y - s.Y, local_6ec.Z - s.Z,
                                TexFlipCoords[flip, 0], TexFlipCoords[flip, 1],
                                1, 1, 1, TrailAlpha,

                                pos_x - e.X, pos_y - e.Y, pos_z - e.Z,
                                TexFlipCoords[flip, 2], TexFlipCoords[flip, 3],
                                1, 1, 1, 1,

                                pos_x + s.X, pos_y + s.Y, pos_z + s.Z,
                                TexFlipCoords[flip, 4], TexFlipCoords[flip, 5],
                                1, 1, 1, 1,

                                local_6ec.X + e.X, local_6ec.Y + e.Y, local_6ec.Z + e.Z,
                                TexFlipCoords[flip, 6], TexFlipCoords[flip, 7],
                                1, 1, 1, TrailAlpha,
                            };

                            _shader.DrawDynamicPOSTEXCLR(4, ref v);
                        }
                    }
                    else
                    {
                        //if (piVar34 == 0x0)
                        {
                            int flip = ((int)Kind >> 0x12) & 0x3;
                            _shader.HasTexture = Kind.HasFlag(ParticleKind.HasTexture);

                            float[] v = new float[]
                            {
                                pos_x - s.X, pos_y - s.Y, pos_z - s.Z,
                                TexFlipCoords[flip, 0], TexFlipCoords[flip, 1],

                                pos_x - e.X, pos_y - e.Y, pos_z - e.Z,
                                TexFlipCoords[flip, 2], TexFlipCoords[flip, 3],

                                pos_x + s.X, pos_y + s.Y, pos_z + s.Z,
                                TexFlipCoords[flip, 4], TexFlipCoords[flip, 5],

                                pos_x + e.X, pos_y + e.Y, pos_z + e.Z,
                                TexFlipCoords[flip, 6], TexFlipCoords[flip, 7],
                            };

                            _shader.DrawDynamicPOSTEX(4, ref v);
                        }
                    }
                    
                }
            }

            // 803a11d8
            // no appsrt

            // 803a11e4
            // appsrt

            GL.PopAttrib();
        }
    
    }
}
