using System;
using System.Collections.Generic;

namespace HSDRaw.GX
{
    public class GX_Attribute : HSDAccessor
    {
        public override int TrimmedSize { get; } = 0x18;

        public GXAttribName AttributeName { get => (GXAttribName)_s.GetInt32(0); set => _s.SetInt32(0, (int)value); }

        public GXAttribType AttributeType { get => (GXAttribType)_s.GetInt32(4); set => _s.SetInt32(4, (int)value); }

        public GXCompCnt CompCount { get => (GXCompCnt)_s.GetInt32(0x08); set => _s.SetInt32(0x08, (int)value); }

        public GXCompType CompType { get => (GXCompType)_s.GetInt32(0x0C); set => _s.SetInt32(0x0C, (int)value); }

        public byte Scale { get => _s.GetByte(0x10); set => _s.SetByte(0x10, value); }

        public short Stride { get => _s.GetInt16(0x12); set => _s.SetInt16(0x12, value); }

        public HSDAccessor Buffer
        {
            get => _s.GetReference<HSDAccessor>(0x14);
            set => _s.SetReference(0x14, value);
        }

        /// <summary>
        /// Gets all decoded data from this Attribute
        /// Warning: this process is slow
        /// </summary>
        public List<float[]> DecodedData
        {
            get
            {
                if (Buffer == null || AttributeType == GXAttribType.GX_DIRECT || AttributeType == GXAttribType.GX_NONE)
                    return null;
                
                List<float[]> output = new List<float[]>();

                var count = Buffer._s.Length / Stride;

                for (int i = 0; i < count; i++)
                {
                    output.Add(GetDecodedDataAt(i));
                }

                return output;
            }
            set
            {
                Tools.GX_AttributeCompressor.GenerateBuffer(this, value);
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessor"></param>
        /// <param name="attribute"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public float[] GetDecodedDataAt(int index)
        {
            if (AttributeName == GXAttribName.GX_VA_CLR0 || AttributeName == GXAttribName.GX_VA_CLR1)
                return GetColorAt(index);

            var size = Stride;

            switch (CompType)
            {
                case GXCompType.UInt16: size /= 2; break;
                case GXCompType.Int16: size /= 2; break;
                case GXCompType.Float: size /= 4; break;
            }

            float[] a = new float[size];
            var accessor = Buffer;

            int offset = Stride * index;

            switch (CompType)
            {
                case GXCompType.UInt8:
                    for (int i = 0; i < size; i++)
                        a[i] = accessor._s.GetByte(offset + i);
                    break;
                case GXCompType.Int8:
                    for (int i = 0; i < size; i++)
                        a[i] = (sbyte)accessor._s.GetByte(offset + i);
                    break;
                case GXCompType.UInt16:
                    for (int i = 0; i < size; i++)
                        a[i] = (ushort)accessor._s.GetInt16(offset + (i * 2));
                    break;
                case GXCompType.Int16:
                    for (int i = 0; i < size; i++)
                        a[i] = accessor._s.GetInt16(offset + (i * 2));
                    break;
                case GXCompType.Float:
                    for (int i = 0; i < size; i++)
                        a[i] = accessor._s.GetFloat(offset + (i * 4));
                    break;
                default:
                    for (int i = 0; i < size; i++)
                        a[i] = accessor._s.GetByte(offset + i);
                    break;
            }

            for (int i = 0; i < a.Length; i++)
                a[i] = a[i] / (1 << Scale);

            return a;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessor"></param>
        /// <param name="attribute"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private float[] GetColorAt(int index)
        {
            var c = new float[4] { 1, 1, 1, 1 };

            int offset = index * Stride;

            var accessor = Buffer;

            // TODO: are these in the right order?
            switch (CompType)
            {
                case GXCompType.RGB565:
                    var pixel = accessor._s.GetInt16(offset);
                    c[0] = ((((pixel >> 0) & 0x1F) << 3) & 0xff) / 255f;
                    c[1] = ((((pixel >> 5) & 0x3F) << 2) & 0xff) / 255f;
                    c[2] = ((((pixel >> 11) & 0x1F) << 3) & 0xff) / 255f;
                    c[3] = 1;
                    break;
                case GXCompType.RGB8:
                    c[0] = accessor._s.GetByte(offset) / 255f;
                    c[1] = accessor._s.GetByte(offset + 1) / 255f;
                    c[2] = accessor._s.GetByte(offset + 2) / 255f;
                    c[3] = 1;
                    break;
                case GXCompType.RGBA4:
                    c[0] = (accessor._s.GetByte(offset) >> 4) * 16 / 255f;
                    c[1] = (accessor._s.GetByte(offset) & 0xF) * 16 / 255f;
                    c[2] = (accessor._s.GetByte(offset + 1) >> 4) * 16 / 255f;
                    c[3] = (accessor._s.GetByte(offset + 1) & 0xF) * 16 / 255f;
                    break;
                case GXCompType.RGBA6: //TODO: this is approximate
                    var p = accessor._s.GetInt32(offset) & 0xFFFFFF;
                    c[0] = ((p >> 18) & 0x3F) / 0x3F;
                    c[1] = ((p >> 12) & 0x3F) / 0x3F;
                    c[2] = ((p >> 6) & 0x3F) / 0x3F;
                    c[3] = ((p) & 0x3F) / 0x3F;
                    break;
                case GXCompType.RGBA8:
                    c[0] = accessor._s.GetByte(offset) / 255f;
                    c[1] = accessor._s.GetByte(offset + 1) / 255f;
                    c[2] = accessor._s.GetByte(offset + 2) / 255f;
                    c[3] = accessor._s.GetByte(offset + 3) / 255f;
                    break;
                case GXCompType.RGBX8:
                    c[0] = accessor._s.GetByte(offset) / 255f;
                    c[1] = accessor._s.GetByte(offset + 1) / 255f;
                    c[2] = accessor._s.GetByte(offset + 2) / 255f;
                    c[3] = accessor._s.GetByte(offset + 3) / 255f;
                    break;
            }

            return c;
        }
        
    }
}
