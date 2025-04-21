using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace HSDRawViewer.Tools
{
    [Flags]
    public enum MaterialSetFlag
    {
        RGB = 1,
        A = 8,
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MaterialSet
    {
        public MaterialSetFlag Flag { get; set; }
        public byte RGB { get; set; }
        public byte A { get; set; }

        public override string ToString()
        {
            return $"({RGB}, {A})";
        }
    }

    [Flags]
    public enum VectorSetFlag
    {
        X = 0x01,
        Y = 0x02,
        Z = 0x04,
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class VectorSet
    {
        public VectorSetFlag Flag { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
    }

    [Flags]
    public enum ColorSetFlag
    {
        R = 0x01,
        G = 0x02,
        B = 0x04,
        A = 0x08
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ColorSet
    {
        public ColorSetFlag Enabled { get; set; }

        [DisplayName("Color R")]
        public byte R { get; set; }
        [DisplayName("Color G")]
        public byte G { get; set; }
        [DisplayName("Color B")]
        public byte B { get; set; }
        [DisplayName("Color A")]
        public byte A { get; set; }

        public override string ToString()
        {
            return $"rgba({R}, {G}, {B}, {A})";
        }
    }

    [Flags]
    public enum PrimEnvFlag
    {
        R = 0x01,
        G = 0x02,
        B = 0x04,
        A = 0x08,

        Prim = 0x10,
        Env = 0x20,
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ColorStep
    {
        public PrimEnvFlag Flag { get; set; }

        [DisplayName("Color R")]
        public byte R { get; set; }
        [DisplayName("Color G")]
        public byte G { get; set; }
        [DisplayName("Color B")]
        public byte B { get; set; }
        [DisplayName("Color A")]
        public byte A { get; set; }

        public byte Step { get; set; }

        public override string ToString()
        {
            return $"{Step} {R} {G} {B} {A}";
        }
    }

    public class ParticleEvent : CustomClass
    {
        public byte Code { get => _code; }

        private byte _code;

        /// <summary>
        /// 
        /// </summary>
        public ParticleEvent()
        {
            SetCode(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        public void SetCode(byte code)
        {
            _code = code;

            ParticleDescriptor desc = ParticleManager.GetParticleDescriptor(code);

            Clear();

            if (desc == null)
                return;

            if (desc.ParamDesc == null)
                return;

            for (int i = 0; i < desc.ParamDesc.Length; i++)
            {
                switch (desc.ParamDesc[i])
                {
                    case 'w':
                        Add(desc.Params[i], (ushort)0);
                        break;
                    case 'b':
                        Add(desc.Params[i], (byte)0);
                        break;
                    case 's':
                        Add(desc.Params[i], (ushort)0);
                        break;
                    case 'f':
                        Add(desc.Params[i], 0f);
                        break;
                    case 'e':
                        Add(desc.Params[i], (ushort)0);
                        break;
                    case 'v':
                        Add(desc.Params[i], new VectorSet());
                        break;
                    case 'c':
                        Add(desc.Params[i], new ColorSet());
                        break;
                    case 'l':
                        Add(desc.Params[i], new ColorStep());
                        break;
                    case 'm':
                        Add(desc.Params[i], new MaterialSet());
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdList"></param>
        /// <param name="cmdPtr"></param>
        public void LoadCode(byte[] cmdList, ref int cmdPtr)
        {
            // load description and code byte
            byte code = GetByteCode(cmdList, cmdPtr);
            ParticleDescriptor desc = ParticleManager.GetParticleDescriptor(code);

            // get normal byte
            byte cmd = cmdList[cmdPtr++];

            // setup code
            SetCode(code);

            if (desc == null)
                return;

            // 
            if (desc.ParamDesc == null)
                return;

            // load params from data
            for (int i = 0; i < desc.ParamDesc.Length; i++)
            {
                switch (desc.ParamDesc[i])
                {
                    case 'w':
                        ushort cmdWait = (ushort)(cmd & 0x1F);

                        if ((cmd & 0x20) != 0)
                            cmdWait = (ushort)(cmdWait * 0x100 + cmdList[cmdPtr++]);

                        this[i].Value = cmdWait;
                        break;
                    case 'b':
                        this[i].Value = cmdList[cmdPtr++];
                        break;
                    case 's':
                        this[i].Value = (ushort)(cmdList[cmdPtr++] * 0x100 + cmdList[cmdPtr++]);
                        break;
                    case 'f':
                        {
                            this[i].Value = BitConverter.ToSingle(new byte[]
                                {
                                    cmdList[cmdPtr + 3],
                                    cmdList[cmdPtr + 2],
                                    cmdList[cmdPtr + 1],
                                    cmdList[cmdPtr],
                                }, 0);
                            cmdPtr += 4;
                        }
                        break;
                    case 'e':
                        {
                            ushort b = cmdList[cmdPtr++];
                            if ((b & 0x80) != 0)
                                b = (ushort)((b & 0x7F) * 0x100 + cmdList[cmdPtr++]);
                            this[i].Value = b;
                        }
                        break;
                    case 'v':
                        {
                            VectorSet v = (VectorSet)this[i].Value;
                            v.Flag = (VectorSetFlag)(cmd & 0xF);
                            if ((cmd & 1) != 0)
                            {
                                v.X = BitConverter.ToSingle(new byte[]
                                {
                                    cmdList[cmdPtr + 3],
                                    cmdList[cmdPtr + 2],
                                    cmdList[cmdPtr + 1],
                                    cmdList[cmdPtr],
                                }, 0);
                                cmdPtr += 4;
                            }
                            if ((cmd & 2) != 0)
                            {
                                v.Y = BitConverter.ToSingle(new byte[]
                                {
                                    cmdList[cmdPtr + 3],
                                    cmdList[cmdPtr + 2],
                                    cmdList[cmdPtr + 1],
                                    cmdList[cmdPtr],
                                }, 0);
                                cmdPtr += 4;
                            }
                            if ((cmd & 4) != 0)
                            {
                                v.Z = BitConverter.ToSingle(new byte[]
                                {
                                    cmdList[cmdPtr + 3],
                                    cmdList[cmdPtr + 2],
                                    cmdList[cmdPtr + 1],
                                    cmdList[cmdPtr],
                                }, 0);
                                cmdPtr += 4;
                            }
                        }
                        break;
                    case 'c':
                        {
                            ColorSet v = (ColorSet)this[i].Value;

                            v.Enabled = (ColorSetFlag)(cmd & 0xF);

                            if ((cmd & 1) != 0)
                                v.R = cmdList[cmdPtr++];
                            if ((cmd & 2) != 0)
                                v.G = cmdList[cmdPtr++];
                            if ((cmd & 4) != 0)
                                v.B = cmdList[cmdPtr++];
                            if ((cmd & 8) != 0)
                                v.A = cmdList[cmdPtr++];
                        }
                        break;
                    case 'l':
                        {
                            ColorStep v = (ColorStep)this[i].Value;

                            byte flag = cmdList[cmdPtr++];
                            v.Step = cmdList[cmdPtr++];
                            v.Flag = (PrimEnvFlag)(flag);

                            if ((flag & 1) != 0)
                                v.R = cmdList[cmdPtr++];
                            if ((flag & 2) != 0)
                                v.G = cmdList[cmdPtr++];
                            if ((flag & 4) != 0)
                                v.B = cmdList[cmdPtr++];
                            if ((flag & 8) != 0)
                                v.A = cmdList[cmdPtr++];
                        }
                        break;
                    case 'm':
                        {
                            MaterialSet v = (MaterialSet)this[i].Value;

                            byte flag = cmdList[cmdPtr++];
                            v.Flag = (MaterialSetFlag)(flag & 0xF);

                            if ((flag & 1) != 0)
                                v.RGB = cmdList[cmdPtr++];

                            if ((flag & 8) != 0)
                                v.A = cmdList[cmdPtr++];
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdList"></param>
        /// <param name="cmdPtr"></param>
        /// <returns></returns>
        public static bool ReadEvent(byte[] cmdList, ref int cmdPtr, out ParticleEvent ptclEvent)
        {
            // null by default
            ptclEvent = null;

            if (cmdPtr >= cmdList.Length)
                return false;

            // check end of data cmd
            byte code = GetByteCode(cmdList, cmdPtr);
            if (code == 0xFF || code == 0xFE)
                return false;

            // load particle code
            ptclEvent = new ParticleEvent();
            ptclEvent.LoadCode(cmdList, ref cmdPtr);

            // GOTO at the end
            if (code == 0xFD)
                return false;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdList"></param>
        /// <param name="cmdPtr"></param>
        /// <returns></returns>
        private static byte GetByteCode(byte[] cmdList, int cmdPtr)
        {
            byte cmd = cmdList[cmdPtr];

            if (cmd < 0x80)
            {
                ushort cmdWait = (ushort)(cmd & 0x1F);

                if ((cmd & 0x20) != 0)
                    cmdWait = (ushort)(cmdWait * 0x100 + cmdList[cmdPtr]);

                if ((cmd & 0xC0) == 0x40)
                    return 0x40;

                if (cmdWait != 0)
                    return 0x00;

            }
            else
            {
                int final_cmd = cmd & 0xF8;

                if ((cmd & 0xF8) > 0x98)
                {
                    final_cmd = cmd & 0xF0;

                    if ((cmd & 0xF0) != 0xC0 && (cmd & 0xF0) != 0xD0)
                        final_cmd = cmd;
                }

                cmd = (byte)final_cmd;
            }

            return cmd;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] CompileCode()
        {
            // load description and code byte
            ParticleDescriptor desc = ParticleManager.GetParticleDescriptor(_code);

            if (desc == null)
                return new byte[] { _code };

            // 
            if (desc.ParamDesc == null)
                return new byte[] { _code };

            List<byte> o = new();
            o.Add(_code);

            // load params from data
            for (int i = 0; i < desc.ParamDesc.Length; i++)
            {
                switch (desc.ParamDesc[i])
                {
                    case 'w':
                        if ((ushort)this[i].Value < 31)
                        {
                            o[i] |= (byte)(ushort)this[i].Value;
                        }
                        else
                        {
                            o[i] |= (byte)(0x20 | ((ushort)this[i].Value >> 8));
                            o.Add((byte)((ushort)this[i].Value & 0xFF));
                        }
                        break;
                    case 'b':
                        o.Add((byte)this[i].Value);
                        break;
                    case 's':
                        o.Add((byte)(((ushort)this[i].Value >> 8) & 0xFF));
                        o.Add((byte)((ushort)this[i].Value & 0xFF));
                        break;
                    case 'f':
                        {
                            o.AddRange(BitConverter.GetBytes((float)this[i].Value).Reverse());
                        }
                        break;
                    case 'e':
                        {
                            ushort b = (ushort)this[i].Value;

                            if (b > 0xFF || (b & 0x80) > 0)
                            {
                                o.Add((byte)(((b >> 8) & 0x7F) | 0x80));
                                o.Add((byte)(b & 0xFF));
                            }
                            else
                            {
                                o.Add((byte)b);
                            }
                        }
                        break;
                    case 'v':
                        {
                            VectorSet v = (VectorSet)this[i].Value;

                            o[0] |= (byte)((byte)v.Flag & 0xF);

                            if (v.Flag.HasFlag(VectorSetFlag.X))
                                o.AddRange(BitConverter.GetBytes(v.X).Reverse());

                            if (v.Flag.HasFlag(VectorSetFlag.Y))
                                o.AddRange(BitConverter.GetBytes(v.Y).Reverse());

                            if (v.Flag.HasFlag(VectorSetFlag.Z))
                                o.AddRange(BitConverter.GetBytes(v.Z).Reverse());

                        }
                        break;
                    case 'c':
                        {
                            ColorSet v = (ColorSet)this[i].Value;

                            o[0] |= (byte)((byte)v.Enabled & 0xF);

                            if (v.Enabled.HasFlag(ColorSetFlag.R))
                                o.Add(v.R);
                            if (v.Enabled.HasFlag(ColorSetFlag.G))
                                o.Add(v.G);
                            if (v.Enabled.HasFlag(ColorSetFlag.B))
                                o.Add(v.B);
                            if (v.Enabled.HasFlag(ColorSetFlag.A))
                                o.Add(v.A);
                        }
                        break;
                    case 'l':
                        {
                            ColorStep v = (ColorStep)this[i].Value;

                            o.Add((byte)v.Flag);
                            o.Add(v.Step);

                            if (v.Flag.HasFlag(PrimEnvFlag.R))
                                o.Add(v.R);
                            if (v.Flag.HasFlag(PrimEnvFlag.G))
                                o.Add(v.G);
                            if (v.Flag.HasFlag(PrimEnvFlag.B))
                                o.Add(v.B);
                            if (v.Flag.HasFlag(PrimEnvFlag.A))
                                o.Add(v.A);
                        }
                        break;
                    case 'm':
                        {
                            MaterialSet v = (MaterialSet)this[i].Value;

                            o.Add((byte)v.Flag);

                            if (v.Flag.HasFlag(MaterialSetFlag.RGB))
                                o.Add(v.RGB);

                            if (v.Flag.HasFlag(MaterialSetFlag.A))
                                o.Add(v.A);
                        }
                        break;
                }
            }

            return o.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            ParticleDescriptor desc = ParticleManager.GetParticleDescriptor(_code);

            StringBuilder b = new();

            bool first = true;
            foreach (CustomProperty v in base.List)
                if (first)
                {
                    b.Append(v.Value);
                    first = false;
                }
                else
                {
                    b.Append(", " + v.Value);
                }

            return $"{desc.Name} ({b})"; // e.Name + " = " + 
        }
    }

}
