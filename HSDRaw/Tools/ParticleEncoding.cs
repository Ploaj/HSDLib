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

                            output.Add(e);
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
            new ParticleOpCode(0x00, 32, "w"),
            new ParticleOpCode(0x80, 7, "p"),
            new ParticleOpCode(0x88, 7, "p"),
            new ParticleOpCode(0x90, 7, "v"),
            new ParticleOpCode(0x98, 7, "v"),
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
            new ParticleOpCode(0xB3, 1, "ebs"),
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
            new ParticleOpCode(0xF1, 1, "s"),
            new ParticleOpCode(0xF2, 8, "?"),
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

                    // get descriptor
                    var opcode = GetOpCode(code);

                    // op code not found
                    if (opcode == null)
                        throw new NotSupportedException("Unknown op code 0x" + code.ToString("X"));

                    // process code
                    output.Add(new Tuple<byte, object[]>(code, opcode.Decode(code, r)));

                    // terminate script
                    if (code == 0xFF || code == 0xFE)
                        break;
                }
            }

            return output.ToArray();
        }

    }
}
