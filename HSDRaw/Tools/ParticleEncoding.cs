using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HSDRaw.Tools
{
    public class ParticleEncoding
    {
        /// <summary>
        /// Op code descriptor
        /// </summary>
        private class ParticleOpCode
        {
            public byte CodeStart { get; set; }

            public byte CodeRange { get; set; }

            public char[] Parameters { get; set; }
            
            /// <summary>
            /// 
            /// </summary>
            /// <param name="code"></param>
            /// <param name="name"></param>
            /// <param name="description"></param>
            /// <param name="parameters"></param>
            /// <param name="paramNames"></param>
            /// <param name="paramDescriptions"></param>
            public ParticleOpCode(byte code, byte coderange, string parameters)
            {
                CodeStart = code;
                CodeRange = coderange;
                Parameters = parameters.ToCharArray();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="code"></param>
            /// <param name="r"></param>
            /// <returns></returns>
            public void Encode(byte code, object[] p, BinaryWriterExt w)
            {
                if(Parameters.Length > p.Length 
                    && (code & 0xF0) != 0xC0 
                    && (code & 0xF0) != 0xD0
                    && (code & 0xF8) != 0x80
                    && (code & 0xF8) != 0x88
                    && (code & 0xF8) != 0x90
                    && (code & 0xF8) != 0x98)
                    throw new ArgumentOutOfRangeException($"Op Code 0x{code.ToString("X")} expected {Parameters.Length} argument(s) and recieved {p.Length}");

                var codepos = w.BaseStream.Position;
                w.Write((byte)0);

                for (int i = 0; i < Parameters.Length; i++)
                {
                    var k = Parameters[i];

                    switch (k)
                    {
                        case 'e':
                            {
                                var param = (short)p[i];

                                if(param >= 0x80)
                                {
                                    w.Write((byte)(((param >> 8) & 0x7F) | 0x80));
                                    w.Write((byte)(param & 0xFF));
                                }
                                else
                                {
                                    w.Write((byte)param);
                                }
                            }
                            break;
                        case 'b':
                            w.Write((byte)p[i]);
                            break;
                        case 'f':
                            w.Write((float)p[i]);
                            break;
                        case 's':
                            w.Write((short)p[i]);
                            break;
                        case 'c':
                            {
                                byte param_count = 0;

                                for (int j = 0; j < p.Length - i; j++)
                                    param_count |= (byte)(1 << j);

                                code = ((byte)((code & 0xF0) | (param_count & 0x0F)));

                                for (int j = i; j < p.Length; j++)
                                    w.Write((byte)p[j]);
                            }
                            break;
                        case 'p':
                        case 'v':
                            {
                                byte param_count = 0;

                                for (int j = 0; j < p.Length; j++)
                                    param_count |= (byte)(1 << j);

                                code = ((byte)((code & 0xF8) | param_count));
                                for (int j = 0; j < p.Length; j++)
                                    w.Write((float)p[j]);
                            }
                            break;
                        case 'r':
                            {
                                var beh = 0;

                                if ((bool)p[0])
                                    beh |= 0x10;

                                if ((bool)p[1])
                                    beh |= 0x20;

                                byte param_count = 0;

                                for (int j = 0; j < p.Length - 3; j++)
                                    param_count |= (byte)(1 << j);

                                w.Write((byte)(beh | param_count));
                                w.Write((byte)p[2]);

                                for (int j = 3; j < p.Length; j++)
                                    w.Write((byte)p[j]);
                            }
                            break;
                        case 'm':
                            {
                                w.Write((byte)0x09);
                                w.Write((byte)p[0]);
                                w.Write((byte)p[0]);
                            }
                            break;
                    }
                }
                
                var temp = w.BaseStream.Position;
                w.Seek((uint)codepos);
                w.Write(code);
                w.Seek((uint)temp);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public object[] Decode(byte code, BinaryReaderExt r)
            {
                var output = new List<object>(Parameters.Length);

                for(int i = 0; i < Parameters.Length ;i++)
                {
                    var k = Parameters[i];

                    switch(k)
                    {
                        case 'e':
                            // extended byte
                            int e = r.ReadByte();

                            if((e & 0x80) > 0)
                                e = ((e & 0x7F) << 8 | r.ReadByte());

                            output.Add((short)e);
                            break;
                        case 'b':
                            output.Add(r.ReadByte());
                            break;
                        case 'f':
                            output.Add(r.ReadSingle());
                            break;
                        case 's':
                            output.Add(r.ReadInt16());
                            break;
                        case 'c':
                            {
                                // custom parameters
                                //var behavior = code >> 4;
                                var param_count = code & 0xF;

                                for (int j = 0; j < 4; j++)
                                    if (((param_count >> i) & 1) == 1)
                                        output.Add(r.ReadByte());
                            }
                            break;
                        case 'p':
                        case 'v':
                            {
                                if ((code & 0x01) != 0)
                                    output.Add(r.ReadSingle());
                                else
                                    output.Add(0);

                                if ((code & 0x02) != 0)
                                    output.Add(r.ReadSingle());
                                else
                                    output.Add(0);

                                if ((code & 0x04) != 0)
                                    output.Add(r.ReadSingle());
                                else
                                    output.Add(0);
                            }
                            break;
                        case 'r':
                            {
                                var behavior = r.ReadByte();

                                output.Add((behavior & 0x10) != 0);
                                output.Add((behavior & 0x20) != 0);

                                output.Add(r.ReadByte());

                                if ((behavior & 0x01) != 0)
                                    output.Add(r.ReadByte());
                                else
                                    output.Add(0);

                                if ((behavior & 0x02) != 0)
                                    output.Add(r.ReadByte());
                                else
                                    output.Add(0);

                                if ((behavior & 0x04) != 0)
                                    output.Add(r.ReadByte());
                                else
                                    output.Add(0);

                                if ((behavior & 0x08) != 0)
                                    output.Add(r.ReadByte());
                                else
                                    output.Add(0);
                            }
                            break;
                        case 'm':
                            {
                                var behavior = r.ReadByte();

                                if ((behavior & 0x01) != 0)
                                    output.Add(r.ReadByte());
                                else
                                    output.Add(0);

                                if ((behavior & 0x08) != 0)
                                    output.Add(r.ReadByte());
                                else
                                    output.Add(0);
                            }
                            break;
                    }
                }

                return output.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static readonly ParticleOpCode[] OpCodes = new ParticleOpCode[]
        {
            new ParticleOpCode(0x80, 8, "p"),
            new ParticleOpCode(0x88, 8, "p"),
            new ParticleOpCode(0x90, 8, "v"),
            new ParticleOpCode(0x98, 8, "v"),
            new ParticleOpCode(0xA0, 1, "ef"),
            new ParticleOpCode(0xA1, 1, ""),
            new ParticleOpCode(0xA2, 1, "f"),
            new ParticleOpCode(0xA3, 1, "f"),
            new ParticleOpCode(0xA4, 1, "bb"),
            new ParticleOpCode(0xA5, 1, "bb"),
            new ParticleOpCode(0xA6, 1, "ss"),
            new ParticleOpCode(0xA7, 1, "b"),
            new ParticleOpCode(0xA8, 1, "fff"),
            new ParticleOpCode(0xA9, 1, "f"),
            new ParticleOpCode(0xAA, 1, "ss"),
            new ParticleOpCode(0xAB, 1, "f"),
            new ParticleOpCode(0xAC, 1, "eff"),
            new ParticleOpCode(0xAD, 1, ""),
            new ParticleOpCode(0xAE, 1, ""),
            new ParticleOpCode(0xAF, 1, ""),
            new ParticleOpCode(0xB0, 1, ""),
            new ParticleOpCode(0xB1, 1, ""),
            new ParticleOpCode(0xB2, 1, ""),
            new ParticleOpCode(0xB3, 1, "ebbb"),
            new ParticleOpCode(0xB4, 1, ""),
            new ParticleOpCode(0xB5, 1, ""),
            new ParticleOpCode(0xB6, 1, "ef"),
            new ParticleOpCode(0xB6, 1, "b"),
            new ParticleOpCode(0xB8, 1, "bff"),
            new ParticleOpCode(0xB9, 1, "bb"),
            new ParticleOpCode(0xBA, 1, "bbbb"),
            new ParticleOpCode(0xBB, 1, "bbbb"),
            new ParticleOpCode(0xBC, 1, "bb"),
            new ParticleOpCode(0xBD, 1, "ff"),
            new ParticleOpCode(0xBE, 1, "fff"),
            new ParticleOpCode(0xBF, 1, "b"),
            new ParticleOpCode(0xC0, 16, "ec"),
            new ParticleOpCode(0xD0, 16, "ec"),
            new ParticleOpCode(0xE0, 1, "bbbb"),
            new ParticleOpCode(0xE1, 1, "b"),
            new ParticleOpCode(0xE2, 1, ""),
            new ParticleOpCode(0xE3, 1, "b"),
            new ParticleOpCode(0xE4, 1, "b"),
            new ParticleOpCode(0xE5, 1, "b"),
            new ParticleOpCode(0xE6, 1, ""),
            new ParticleOpCode(0xE7, 1, ""),
            new ParticleOpCode(0xE8, 1, "f"),
            new ParticleOpCode(0xE9, 1, "r"),
            new ParticleOpCode(0xEA, 1, "em"),
            new ParticleOpCode(0xEB, 1, "em"),
            new ParticleOpCode(0xEC, 1, "bf"),
            new ParticleOpCode(0xED, 2, "ffb"),
            new ParticleOpCode(0xEF, 1, "sb"),
            new ParticleOpCode(0xF0, 1, "sb"),
            new ParticleOpCode(0xF1, 1, "sm"),
            new ParticleOpCode(0xF2, 8, "sm"),
            new ParticleOpCode(0xFA, 1, "b"),
            new ParticleOpCode(0xFB, 1, ""),
            new ParticleOpCode(0xFC, 1, ""),
            new ParticleOpCode(0xFD, 1, ""),
            new ParticleOpCode(0xFE, 1, ""),
            new ParticleOpCode(0xFF, 1, ""),
        };
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static ParticleOpCode GetOpCode(byte code)
        {
            foreach(var op in OpCodes)
                if (code >= op.CodeStart && code < op.CodeStart + op.CodeRange)
                    return op;

            return null;
        }

        /*
         * 
            new ParticleOpCode(0x00, 31, "w"),
            new ParticleOpCode(0x20, 1, "s"), // idk
            new ParticleOpCode(0x40, 1, "b"), // interpolation type
         */
         

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        public static byte[] EncodeParticleCodes(IEnumerable<Tuple<byte, object[]>> codes)
        {
            // process data
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriterExt w = new BinaryWriterExt(stream))
            {
                w.BigEndian = true;
                
                foreach(var c in codes)
                {
                    var code = c.Item1;
                    var p = c.Item2;

                    System.Diagnostics.Debug.WriteLine(stream.Position.ToString("X8") + " " + code.ToString("X"));

                    if (code == 0x00)
                    {
                        if (p.Length != 1)
                            throw new ArgumentOutOfRangeException($"Op Code 0x{code.ToString("X")} expected 1 argument(s) and recieved {p.Length}");

                        var wait = (short)p[0];

                        if (wait >= 0x2000)
                        {
                            throw new IndexOutOfRangeException("Particle can only wait 8192 frames");
                        }
                        if (wait > 31)
                        {
                            w.Write((byte)((wait >> 8) | 0x20));
                            w.Write((byte)wait);
                        }
                        else
                            w.Write((byte)wait);
                    }
                    else
                    if(code == 0x40)
                    {
                        w.Write((byte)0x40);
                        w.Write((byte)p[0]);
                    }
                    else
                    {
                        // get descriptor
                        var opcode = GetOpCode(code);

                        // op code not found
                        if (opcode == null)
                            throw new NotSupportedException("Unknown op code 0x" + code.ToString("X"));

                        // process code
                        opcode.Encode(code, p, w);

                        // terminate script
                        if (code == 0xFF || code == 0xFE || code == 0xFD)
                            break;
                    }
                }

                return stream.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codes"></param>
        public static Tuple<byte, object[]>[] DecodeParticleOpCodes(byte[] codes)
        {
            var output = new List<Tuple<byte, object[]>>();

            // process data
            using (MemoryStream stream = new MemoryStream(codes))
            using (BinaryReaderExt r = new BinaryReaderExt(stream))
            {
                r.BigEndian = true;

                while(r.Position < r.Length)
                {
                    // read code
                    var code = r.ReadByte();

                    if(code < 128)
                    {

                        if ((code & 0xC0) == 0x40)
                            output.Add(new Tuple<byte, object[]>(0x40, new object[] { r.ReadByte()}));
                        else
                        {
                            var wait = code & 0x1F;
                            if ((code & 0x20) != 0)
                                wait = (wait << 8) | r.ReadByte();
                            output.Add(new Tuple<byte, object[]>(0x00, new object[] { (short)wait }));
                        }

                        continue;
                    }

                    // get descriptor
                    var opcode = GetOpCode(code);

                    // op code not found
                    if (opcode == null)
                        throw new NotSupportedException("Unknown op code 0x" + code.ToString("X"));
                    
                    // process code
                    output.Add(new Tuple<byte, object[]>(code, opcode.Decode(code, r)));

                    // terminate script
                    if (code == 0xFF || code == 0xFE || code == 0xFD)
                        break;
                }
            }

            return output.ToArray();
        }

    }
}
