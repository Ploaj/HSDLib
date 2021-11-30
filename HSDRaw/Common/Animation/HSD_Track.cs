using HSDRaw.Tools;
using System;
using System.Collections.Generic;

namespace HSDRaw.Common.Animation
{
    /// <summary>
    /// Frame object with additional data length field
    /// used in <see cref="HSD_FigaTree"/>
    /// </summary>
    public class HSD_Track : HSDAccessor
    {
        public override int TrimmedSize => 0x0C;

        public ushort DataLength { get => _s.GetUInt16(0x00); set => _s.SetUInt16(0x00, value); }

        public short StartFrame { get => _s.GetInt16(0x02); set => _s.SetInt16(0x02, value); }

        public byte TrackType { get => _s.GetByte(0x04); set => _s.SetByte(0x04, value); }

        public JointTrackType JointTrackType { get => (JointTrackType)TrackType; set => TrackType = (byte)value; }

        public MatTrackType MatTrackType { get => (MatTrackType)TrackType; set => TrackType = (byte)value; }

        public TexTrackType TexTrackType { get => (TexTrackType)TrackType; set => TrackType = (byte)value; }

        private byte ValueFlag { get => _s.GetByte(0x05); set => _s.SetByte(0x05, value); }

        private byte TangentFlag { get => _s.GetByte(0x6); set => _s.SetByte(0x06, value); }

        public byte[] Buffer
        {
            get => _s.GetReference<HSDAccessor>(0x08)?._s.GetData();
            set
            {
                //Always make new buffer
                var re = new HSDAccessor();
                re._s.SetData(value);
                _s.SetReference(0x08, re);
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

        public HSD_Track()
        {

        }

        public HSD_Track(HSD_FOBJ fobj)
        {
            FromFOBJ(fobj);
        }

        public void FromFOBJ(HSD_FOBJ fobj)
        {
            TrackType = fobj.TrackType;
            ValueFormat = fobj.ValueFormat;
            ValueScale = fobj.ValueScale;
            TanFormat = fobj.TanFormat;
            TanScale = fobj.TanScale;
            Buffer = fobj.Buffer;
            DataLength = (ushort)Buffer.Length;
        }

        public HSD_FOBJ ToFOBJ()
        {
            var fobj = new HSD_FOBJ();
            fobj.TrackType = TrackType;
            fobj.ValueScale = ValueScale;
            fobj.ValueFormat = ValueFormat;
            fobj.TanFormat = TanFormat;
            fobj.TanScale = TanScale;
            fobj.Buffer = Buffer;
            return fobj;
        }


        public List<FOBJKey> GetKeys()
        {
            return FOBJFrameDecoder.GetKeys(ToFOBJ(), StartFrame);
        }
    }
}
