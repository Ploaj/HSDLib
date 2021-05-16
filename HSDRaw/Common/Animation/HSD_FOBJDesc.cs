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

        public float  StartFrame { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public byte TrackType { get => _s.GetByte(0x0C); set => _s.SetByte(0x0C, value); }

        public JointTrackType JointTrackType { get => (JointTrackType)TrackType; set => TrackType = (byte)value; }

        public MatTrackType MatTrackType { get => (MatTrackType)TrackType; set => TrackType = (byte)value; }

        public TexTrackType TexTrackType { get => (TexTrackType)TrackType; set => TrackType = (byte)value; }

        private byte ValueFlag { get => _s.GetByte(0x0D); set => _s.SetByte(0x0D, value); }

        private byte TangentFlag { get => _s.GetByte(0x0E); set => _s.SetByte(0x0E, value); }

        public byte[] Buffer
        {
            get => _s.GetReference<HSDAccessor>(0x10)?._s.GetData();
            set
            {
                /*var re = _s.GetReference<HSDAccessor>(0x04);
                if (re == null)
                {
                    re = new HSDAccessor();
                    _s.SetReference(0x04, re);
                }*/
                //Always make new buffer
                var re = new HSDAccessor();
                re._s.SetData(value);
                _s.SetReference(0x10, re);
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
            TrackType = fobj.TrackType;
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
            fobj.TrackType = TrackType;
            fobj.ValueScale = ValueScale;
            fobj.ValueFormat = ValueFormat;
            fobj.TanFormat = TanFormat;
            fobj.TanScale = TanScale;
            fobj.Buffer = Buffer;
            return fobj;
        }

        public List<FOBJKey> GetDecodedKeys()
        {
            return FOBJFrameDecoder.GetKeys(ToFOBJ(), StartFrame);
        }

        public void SetKeys(List<FOBJKey> keys, byte trackType)
        {
            var fobj = FOBJFrameEncoder.EncodeFrames(keys, trackType);
            FromFOBJ(fobj);
        }

        /// <summary>
        /// Adds a key frame to the collection
        /// If a key with given frame already exists, it is overrwritten
        /// </summary>
        /// <param name="key"></param>
        public void SetKey(FOBJKey key)
        {
            var keys = GetDecodedKeys();

            var dup = keys.Find(e => e.Frame == key.Frame);
            keys.Remove(dup);

            keys.Insert(keys.FindIndex(e => e.Frame > key.Frame) - 1, key);

            SetKeys(keys, this.TrackType);
        }

        /// <summary>
        /// Inserts Key into track pushing other key frames down a frame
        /// </summary>
        /// <param name="key"></param>
        public void InsertKey(FOBJKey key)
        {
            var keys = GetDecodedKeys();

            foreach (var k in keys)
                if (k.Frame >= key.Frame)
                    k.Frame++;
                    
            var index = keys.FindIndex(e => e.Frame > key.Frame);
            if (index < 0)
                index = keys.Count;

            keys.Insert(index, key);
            
            SetKeys(keys, TrackType);
        }

        public override string ToString()
        {
            return TrackType.ToString() + " " + JointTrackType + " " + MatTrackType + " " + TexTrackType;
        }
    }
}
