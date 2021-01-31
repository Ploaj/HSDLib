using HSDRaw.GX;
using HSDRaw.Tools;
using System.Collections.Generic;

namespace HSDRaw.Common
{
    /// <summary>
    /// WIP: Missing Editing features
    /// </summary>
    public class HSD_TEXGraphicBank : HSDAccessor
    {
        public int Length { get => _s.GetInt32(0x00); internal set => _s.SetInt32(0x00, value); }

        public HSD_TexGraphic[] ParticleImages
        {
            get
            {
                HSD_TexGraphic[] images = new HSD_TexGraphic[Length];
                for(int i = 0; i <= Length; i++)
                {
                    var offset = _s.GetInt32(0x04 * i);
                    var length = _s.GetInt32(0x04 * (i + 1)) - offset;
                    if(i == Length)
                        length = _s.Length - offset;
                    if (i > 0)
                    {
                        images[i - 1] = new HSD_TexGraphic();
                        images[i - 1]._s = _s.GetEmbeddedStruct(offset, length);
                        images[i - 1].SubtractOffset(offset);
                    }
                }
                return images;
            }
            set
            {
                Length = value.Length;

                int length = 4;

                length += 4 * value.Length;

                if (length % 0x20 != 0)
                    length += 0x20 - (length % 0x20);
                
                var offset = length;

                foreach (var v in value)
                    length += v._s.Length;

                _s.Resize(length);
                for(int i = 0; i < value.Length; i++)
                {
                    _s.SetInt32(4 + 4 * i, offset);
                    value[i].AddOffset(offset);
                    _s.SetBytes(offset, value[i]._s.GetData());
                    value[i].SubtractOffset(offset);
                    offset += value[i]._s.Length;
                }
            }
        }
    }

    public class HSD_TexGraphic : HSDAccessor
    {
        public override int TrimmedSize => 0x14;

        public int ImageCount { get => _s.GetInt32(0x00); internal set => _s.SetInt32(0x00, value); }

        public GXTexFmt ImageFormat { get => (GXTexFmt)_s.GetInt32(0x04); internal set => _s.SetInt32(0x04, (int)value); }

        public GXTlutFmt PaletteFormat { get => (GXTlutFmt)_s.GetInt32(0x08); internal set => _s.SetInt32(0x08, (int)value); }

        public int Width { get => _s.GetInt32(0x0C); internal set => _s.SetInt32(0x0C, value); }

        public int Height { get => _s.GetInt32(0x10); internal set => _s.SetInt32(0x10, value); }

        public void SubtractOffset(int amt)
        {
            bool isPaletted = GXImageConverter.IsPalettedFormat(ImageFormat);
            for (int i = 0; i < ImageCount; i++)
            {
                _s.SetInt32(i * 4 + 0x18, _s.GetInt32(i * 4 + 0x18) - amt);
                if (isPaletted)
                    _s.SetInt32((i + ImageCount) * 4 + 0x18, _s.GetInt32((i + ImageCount) * 4 + 0x18) - amt);
            }
        }

        public void AddOffset(int amt)
        {
            bool isPaletted = GXImageConverter.IsPalettedFormat(ImageFormat);
            for (int i = 0; i < ImageCount; i++)
            {
                _s.SetInt32(i * 4 + 0x18, _s.GetInt32(i * 4 + 0x18) + amt);
                if (isPaletted)
                    _s.SetInt32((i + ImageCount) * 4 + 0x18, _s.GetInt32((i + ImageCount) * 4 + 0x18) + amt);
            }
        }
        
        /// <summary>
        /// Decodes the image data into a list of rgba8 byte arrays of the images
        /// </summary>
        /// <returns></returns>
        public List<byte[]> GetRGBAImageData()
        {
            List<byte[]> images = new List<byte[]>();

            bool isPaletted = GXImageConverter.IsPalettedFormat(ImageFormat);
            int imageSize = GXImageConverter.GetImageSize(ImageFormat, Width, Height);
            int paletteSize = 0x200;

            for (int i = 0; i < ImageCount; i++)
            {
                var offset = _s.GetInt32(i * 4 + 0x18);

                var imageData = _s.GetBytes(offset, imageSize);

                if (isPaletted)
                {
                    var paloffset = _s.GetInt32((i + ImageCount) * 4 + 0x18);
                    var palData = _s.GetBytes(paloffset, paletteSize);

                    images.Add(GXImageConverter.DecodeTPL(ImageFormat, Width, Height, imageData, PaletteFormat, 0x100, palData));
                }
                else
                {
                    images.Add(GXImageConverter.DecodeTPL(ImageFormat, Width, Height, imageData));
                }
            }
            
            return images;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HSD_TOBJ[] ConvertToTOBJs()
        {
            HSD_TOBJ[] tobjs = new HSD_TOBJ[ImageCount];

            bool isPaletted = GXImageConverter.IsPalettedFormat(ImageFormat);
            int imageSize = GXImageConverter.GetImageSize(ImageFormat, Width, Height);
            int paletteSize = 0x200;

            for (int i = 0; i < ImageCount; i++)
            {
                tobjs[i] = new HSD_TOBJ();

                var offset = _s.GetInt32(i * 4 + 0x18);

                var imageData = _s.GetBytes(offset, imageSize);

                tobjs[i].ImageData = new HSD_Image();
                tobjs[i].ImageData.Width = (short)Width;
                tobjs[i].ImageData.Height = (short)Height;
                tobjs[i].ImageData.Format = ImageFormat;
                tobjs[i].ImageData.ImageData = imageData;

                if (isPaletted)
                {
                    var paloffset = _s.GetInt32((i + ImageCount) * 4 + 0x18);
                    var palData = _s.GetBytes(paloffset, paletteSize);

                    tobjs[i].TlutData = new HSD_Tlut();
                    tobjs[i].TlutData.ColorCount = 0x100;
                    tobjs[i].TlutData.Format = PaletteFormat;
                    tobjs[i].TlutData.TlutData = palData;
                }
            }

            return tobjs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tobjs"></param>
        public void SetFromTOBJs(HSD_TOBJ[] tobjs)
        {
            if (tobjs.Length == 0)
            {
                _s.Resize(0x18);
                return;
            }

            // set texture information
            ImageCount = tobjs.Length;
            ImageFormat = tobjs[0].ImageData.Format;
            Width = tobjs[0].ImageData.Width;
            Height = tobjs[0].ImageData.Height;
            if (tobjs[0].TlutData != null)
                PaletteFormat = tobjs[0].TlutData.Format;
            
            //
            bool isPaletted = GXImageConverter.IsPalettedFormat(ImageFormat);
            int imageSize = GXImageConverter.GetImageSize(ImageFormat, Width, Height);

            // header length 
            var newLength = 0x18;

            // add offsets
            newLength += tobjs.Length * 4;
            if (isPaletted)
                newLength += tobjs.Length * 4;

            // align buffer
            if(newLength % 0x20 != 0)
                newLength += 0x20 - (newLength % 0x20);

            var offset = newLength;

            // add padding for data
            newLength += imageSize * tobjs.Length;

            if (isPaletted)
                newLength += 0x200 * tobjs.Length;

            // resize buffer
            _s.Resize(newLength);
            
            // now inject offsets to palette and image data
            
            for(int i = 0; i < ImageCount; i++)
            {
                _s.SetInt32(0x18 + i * 4, offset);
                _s.SetBytes(offset, tobjs[i].ImageData.ImageData);
                offset += imageSize;

                if (isPaletted)
                {
                    _s.SetInt32(0x18 + (ImageCount + i) * 4, offset);
                    _s.SetBytes(offset, tobjs[i].TlutData.TlutData);
                    offset += 0x200;
                }
            }
        }
    }
}
