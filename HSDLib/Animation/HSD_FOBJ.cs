using System;

namespace HSDLib.Animation
{
    public enum GXAnimDataFormat
    {
        Float = 0x00,
        Short = 0x20,
        UShort = 0x40,
        SByte = 0x60,
        Byte = 0x80
    }

    public enum JointTrackType
    {
        HSD_A_J_ROTX = 1
, HSD_A_J_ROTY
, HSD_A_J_ROTZ
, HSD_A_J_PATH
, HSD_A_J_TRAX
, HSD_A_J_TRAY
, HSD_A_J_TRAZ
, HSD_A_J_SCAX
, HSD_A_J_SCAY
, HSD_A_J_SCAZ
, HSD_A_J_NODE
, HSD_A_J_BRANCH
, HSD_A_J_SETBYTE0
, HSD_A_J_SETBYTE1
, HSD_A_J_SETBYTE2
, HSD_A_J_SETBYTE3
, HSD_A_J_SETBYTE4
, HSD_A_J_SETBYTE5
, HSD_A_J_SETBYTE6
, HSD_A_J_SETBYTE7
, HSD_A_J_SETBYTE8
, HSD_A_J_SETBYTE9
, HSD_A_J_SETFLOAT0
, HSD_A_J_SETFLOAT1
, HSD_A_J_SETFLOAT2
, HSD_A_J_SETFLOAT3
, HSD_A_J_SETFLOAT4
, HSD_A_J_SETFLOAT5
, HSD_A_J_SETFLOAT6
, HSD_A_J_SETFLOAT7
, HSD_A_J_SETFLOAT8
, HSD_A_J_SETFLOAT9
    }

    public enum InterpolationType
    {
        Step = 1,
        Linear = 2,
        HermiteValue = 3,
        Hermite = 4,
        HermiteCurve = 5,
        Constant = 6
    }
    
    public class HSD_FOBJ : IHSDNode
    {
        public byte AnimationType { get; set; }

        [System.ComponentModel.Browsable(false)]
        public byte Flag1 { get; internal set; }

        [System.ComponentModel.Browsable(false)]
        public byte Flag2 { get; internal set; }

        [System.ComponentModel.Browsable(false)]
        public byte Padding2 { get; internal set; }
        
        public uint DataOffset { get; set; }

        [HSDParseIgnore]
        public uint ValueScale
        {
            get
            {
                return (uint)Math.Pow(2, Flag1 & 0x1F);
            }
            set
            {
                Flag1 = (byte)((Flag1 & 0xF0) | (byte)Math.Log(value, 2));
            }
        }

        [HSDParseIgnore]
        public uint TanScale
        {
            get
            {
                return (uint)Math.Pow(2, Flag2 & 0x1F);
            }
            set
            {
                Flag2 = (byte)((Flag2 & 0xF0) | (byte)Math.Log(value, 2));
            }
        }

        [HSDParseIgnore]
        public GXAnimDataFormat ValueFormat
        {
            get
            {
                return (GXAnimDataFormat)(Flag1 & 0xF0);
            }
            set
            {
                Flag1 = (byte)((Flag1 & 0x1F) | (byte)value);
            }
        }

        [HSDParseIgnore]
        public GXAnimDataFormat TanFormat
        {
            get
            {
                return (GXAnimDataFormat)(Flag2 & 0xF0);
            }
            set
            {
                Flag2 = (byte)((Flag2 & 0x1F) | (byte)value);
            }
        }

        public byte[] Data;

        public override void Save(HSDWriter Writer)
        {
            Writer.WriteBuffer(Data, 0x4);

            int start = (int)Writer.BaseStream.Position;
            base.Save(Writer);

            Writer.WritePointerAt(start + 4, Data);
        }
    }
}
