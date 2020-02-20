using HSDRaw.Tools;
using System;
using System.Collections.Generic;

namespace HSDRaw.Common.Animation
{
    /// <summary>
    /// An extended header for a <see cref="HSD_FOBJ"/> that is a linked list 
    /// containing a starting frame and a data length
    /// </summary>
    public class HSD_FOBJDesc : HSDListAccessor<HSD_FOBJDesc>
    {
        public override int TrimmedSize => 0x14;

        public override HSD_FOBJDesc Next { get => _s.GetReference<HSD_FOBJDesc>(0x00); set => _s.SetReference(0x00, value); }

        public int DataLength { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int StartFrame { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }
        
        public JointTrackType AnimationType { get => (JointTrackType)_s.GetByte(0x0C); set => _s.SetByte(0x0C, (byte)value); }

        private byte ValueFlag { get => _s.GetByte(0x0D); set => _s.SetByte(0x0D, value); }

        private byte TangentFlag { get => _s.GetByte(0x0E); set => _s.SetByte(0x0E, value); }

        public byte[] Buffer
        {
            get => _s.GetReference<HSDAccessor>(0x10)?._s.GetData();
            set
            {
                if (value == null)
                {
                    _s.SetReference(0x10, null);
                    return;
                }

                var re = _s.GetReference<HSDAccessor>(0x10);
                if (re == null)
                {
                    re = new HSDAccessor();
                    _s.SetReference(0x10, re);
                }

                re._s.SetData(value);
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

        public void FromFOBJ(HSD_FOBJ fobj)
        {
            AnimationType = fobj.JointTrackType;
            ValueFormat = fobj.ValueFormat;
            ValueScale = fobj.ValueScale;
            TanFormat = fobj.TanFormat;
            TanScale = fobj.TanScale;
            Buffer = fobj.Buffer;
            DataLength = Buffer.Length;
        }

        public HSD_FOBJ ToFOBJ()
        {
            var fobj = new HSD_FOBJ();
            fobj.JointTrackType = AnimationType;
            fobj.ValueScale = ValueScale;
            fobj.ValueFormat = ValueFormat;
            fobj.TanFormat = TanFormat;
            fobj.TanScale = TanScale;
            fobj.Buffer = Buffer;
            return fobj;
        }

        public List<FOBJKey> GetDecodedKeys()
        {
            return FOBJFrameDecoder.GetKeys(ToFOBJ());
        }

        public void SetKeys(List<FOBJKey> keys, JointTrackType type)
        {
            var fobj = FOBJFrameEncoder.EncodeFrames(keys, type);
            FromFOBJ(fobj);
        }

        public override string ToString()
        {
            return AnimationType.ToString();
        }
    }
}
