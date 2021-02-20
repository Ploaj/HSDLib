using HSDRaw.Tools;
using System;
using System.Collections.Generic;

namespace HSDRaw.Common.Animation
{
    public enum GXAnimDataFormat
    {
        HSD_A_FRAC_FLOAT = 0x00,
        HSD_A_FRAC_S16 = 0x20,
        HSD_A_FRAC_U16 = 0x40,
        HSD_A_FRAC_S8 = 0x60,
        HSD_A_FRAC_U8 = 0x80
    }

    public enum MatTrackType
    {
        HSD_A_M_NONE = 0,
        HSD_A_M_AMBIENT_R = 1,
        HSD_A_M_AMBIENT_G,
        HSD_A_M_AMBIENT_B,
        HSD_A_M_DIFFUSE_R,
        HSD_A_M_DIFFUSE_G,
        HSD_A_M_DIFFUSE_B,
        HSD_A_M_SPECULAR_R,
        HSD_A_M_SPECULAR_G,
        HSD_A_M_SPECULAR_B,
        HSD_A_M_ALPHA,
        HSD_A_M_PE_REF0,
        HSD_A_M_PE_REF1,
        HSD_A_M_PE_DSTALPHA
    }

    public enum TexTrackType
    {
        HSD_A_T_NONE = 0,
        HSD_A_T_TIMG = 1,
        HSD_A_T_TRAU = 2,
        HSD_A_T_TRAV = 3,
        HSD_A_T_SCAU = 4,
        HSD_A_T_SCAV = 5,
        HSD_A_T_ROTX = 6,
        HSD_A_T_ROTY = 7,
        HSD_A_T_ROTZ = 8,
        HSD_A_T_BLEND = 9,
        HSD_A_T_TCLT = 10,
        HSD_A_T_LOD_BIAS = 11,
        HSD_A_T_KONST_R = 12,
        HSD_A_T_KONST_G = 13,
        HSD_A_T_KONST_B = 14,
        HSD_A_T_KONST_A = 15,
        HSD_A_T_TEV0_R = 16,
        HSD_A_T_TEV0_G = 17,
        HSD_A_T_TEV0_B = 18,
        HSD_A_T_TEV0_A = 19,
        HSD_A_T_TEV1_R = 20,
        HSD_A_T_TEV1_G = 21,
        HSD_A_T_TEV1_B = 22,
        HSD_A_T_TEV1_A = 23,
        HSD_A_T_TS_BLEND = 24
    }

    public enum JointTrackType
    {
        HSD_A_J_NONE = 0,
        HSD_A_J_ROTX = 1,
        HSD_A_J_ROTY,
        HSD_A_J_ROTZ,
        HSD_A_J_PATH,
        HSD_A_J_TRAX,
        HSD_A_J_TRAY,
        HSD_A_J_TRAZ,
        HSD_A_J_SCAX,
        HSD_A_J_SCAY,
        HSD_A_J_SCAZ,
        HSD_A_J_NODE,
        HSD_A_J_BRANCH,
        HSD_A_J_SETBYTE0,
        HSD_A_J_SETBYTE1,
        HSD_A_J_SETBYTE2,
        HSD_A_J_SETBYTE3,
        HSD_A_J_SETBYTE4,
        HSD_A_J_SETBYTE5,
        HSD_A_J_SETBYTE6,
        HSD_A_J_SETBYTE7,
        HSD_A_J_SETBYTE8,
        HSD_A_J_SETBYTE9,
        HSD_A_J_SETFLOAT0,
        HSD_A_J_SETFLOAT1,
        HSD_A_J_SETFLOAT2,
        HSD_A_J_SETFLOAT3,
        HSD_A_J_SETFLOAT4,
        HSD_A_J_SETFLOAT5,
        HSD_A_J_SETFLOAT6,
        HSD_A_J_SETFLOAT7,
        HSD_A_J_SETFLOAT8,
        HSD_A_J_SETFLOAT9
    }

    public enum ShapeTrackType
    {
        HSD_A_S_BLEND = 1
    }

    public enum GXInterpolationType
    {
        HSD_A_OP_NONE = 0,
        HSD_A_OP_CON = 0x01,
        HSD_A_OP_LIN = 0x02,
        HSD_A_OP_SPL0 = 0x03,
        HSD_A_OP_SPL = 0x04,
        HSD_A_OP_SLP = 0x05,
        HSD_A_OP_KEY = 0x06
    }

    /// <summary>
    /// A key frame descriptor
    /// </summary>
    public class HSD_FOBJ : HSDAccessor
    {
        public override int TrimmedSize => 8;

        public byte TrackType { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }

        public JointTrackType JointTrackType { get => (JointTrackType)TrackType; set => TrackType = (byte)value; }

        public MatTrackType MatTrackType { get => (MatTrackType)TrackType; set => TrackType = (byte)value; }

        public TexTrackType TexTrackType { get => (TexTrackType)TrackType; set => TrackType = (byte)value; }

        private byte ValueFlag { get => _s.GetByte(0x01); set => _s.SetByte(0x01, value); }

        private byte TangentFlag { get => _s.GetByte(0x02); set => _s.SetByte(0x02, value); }

        public byte[] Buffer
        {
            get => _s.GetReference<HSDAccessor>(0x04)?._s.GetData();
            set
            {
                if (value == null)
                {
                    _s.SetReference(0x04, null);
                    return;
                }

                /*var re = _s.GetReference<HSDAccessor>(0x04);
                if (re == null)
                {
                    re = new HSDAccessor();
                    _s.SetReference(0x04, re);
                }*/
                //Always make new buffer
                var re = new HSDAccessor();
                re._s.SetData(value);
                _s.SetReference(0x04, re);
            }
        }

        public uint ValueScale
        {
            get
            {
                return (uint)(1 << (ValueFlag & 0x1F));
            }
            set
            {
                ValueFlag = (byte)((ValueFlag & 0xE0) | (byte)Math.Log(value, 2));
            }
        }

        public uint TanScale
        {
            get
            {
                return (uint)(1 << (TangentFlag & 0x1F));
            }
            set
            {
                TangentFlag = (byte)((TangentFlag & 0xE0) | (byte)Math.Log(value, 2));
            }
        }

        public GXAnimDataFormat ValueFormat
        {
            get
            {
                return (GXAnimDataFormat)(ValueFlag & 0xE0);
            }
            set
            {
                ValueFlag = (byte)((ValueFlag & 0x1F) | (byte)value);
            }
        }

        public GXAnimDataFormat TanFormat
        {
            get
            {
                return (GXAnimDataFormat)(TangentFlag & 0xE0);
            }
            set
            {
                TangentFlag = (byte)((TangentFlag & 0x1F) | (byte)value);
            }
        }

        public List<FOBJKey> GetDecodedKeys()
        {
            return FOBJFrameDecoder.GetKeys(this);
        }

        public void SetKeys(List<FOBJKey> keys, JointTrackType type)
        {
            var fobj = FOBJFrameEncoder.EncodeFrames(keys, type);

            JointTrackType = type;
            ValueFlag = fobj.ValueFlag;
            TangentFlag = fobj.TangentFlag;
            Buffer = fobj.Buffer;
        }

    }
}
