using HSDRaw.GX;
using HSDRaw.Tools;
using System.Collections.Generic;

namespace HSDRaw.Common
{
    /// <summary>
    /// WIP: Missing Editing features
    /// </summary>
    public class HSD_TEXGraphic : HSDAccessor
    {
        public int Length { get => _s.GetInt32(0x00); internal set => _s.SetInt32(0x00, value); }

        public HSD_ParticleImage[] ParticleImages
        {
            get
            {
                HSD_ParticleImage[] images = new HSD_ParticleImage[Length];
                for(int i = 0; i < Length - 1; i++)
                {
                    var offset = _s.GetInt32(0x04 * i);
                    var length = _s.GetInt32(0x04 * (i + 1)) - offset;
                    if (i > 0)
                    {
                        images[i] = new HSD_ParticleImage();
                        images[i]._s = _s.GetEmbeddedStruct(offset, length);
                        images[i].SubtractOffset(offset);
                    }
                }
                if(Length > 0)
                {
                    var offset = _s.GetInt32(0x04 * (Length - 1));
                    var length = _s.Length - offset;
                    images[Length - 1] = new HSD_ParticleImage();
                    images[Length - 1]._s = _s.GetEmbeddedStruct(offset, length);
                    images[Length - 1].SubtractOffset(offset);
                }
                return images;
            }
        }
    }

    public class HSD_ParticleImage : HSDAccessor
    {
        public int ImageCount { get => _s.GetInt32(0x00); internal set => _s.SetInt32(0x00, value); }

        public GXTexFmt ImageFormat { get => (GXTexFmt)_s.GetInt32(0x04); internal set => _s.SetInt32(0x04, (int)value); }

        public GXTlutFmt PaletteFormat { get => (GXTlutFmt)_s.GetInt32(0x08); internal set => _s.SetInt32(0x08, (int)value); }

        public int Width { get => _s.GetInt32(0x0C); internal set => _s.SetInt32(0x0C, value); }

        public int Height { get => _s.GetInt32(0x10); internal set => _s.SetInt32(0x10, value); }

        public void SubtractOffset(int amt)
        {
            bool isPaletted = TPLConv.IsPalettedFormat(ImageFormat);
            for (int i = 0; i < ImageCount; i++)
            {
                _s.SetInt32(i * 4 + 0x18, _s.GetInt32(i * 4 + 0x18) - amt);
                if (isPaletted)
                    _s.SetInt32((i + ImageCount) * 4 + 0x18, _s.GetInt32((i + ImageCount) * 4 + 0x18) - amt);
            }
        }

        public void AddOffset(int amt)
        {
            bool isPaletted = TPLConv.IsPalettedFormat(ImageFormat);
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

            bool isPaletted = TPLConv.IsPalettedFormat(ImageFormat);
            int imageSize = TPLConv.GetImageSize(ImageFormat, Width, Height);
            int paletteSize = 0x200;

            for (int i = 0; i < ImageCount; i++)
            {
                var offset = _s.GetInt32(i * 4 + 0x18);

                var imageData = _s.GetBytes(offset, imageSize);

                if (isPaletted)
                {
                    var paloffset = _s.GetInt32((i + ImageCount) * 4 + 0x18);
                    var palData = _s.GetBytes(paloffset, paletteSize);

                    images.Add(TPLConv.DecodeTPL(ImageFormat, Width, Height, imageData, PaletteFormat, 0x100, palData));
                }
                else
                {
                    images.Add(TPLConv.DecodeTPL(ImageFormat, Width, Height, imageData));
                }
            }
            
            return images;
        }
    }
}
