using HSDRaw.Common;
using System.Collections.Generic;
using System.Linq;

namespace HSDRaw.MEX.Akaneia
{
    public class AK_Shape : HSDAccessor
    {
        public override int TrimmedSize => 0x20;

        public int Width { get => _s.GetInt32(0x00); internal set => _s.SetInt32(0x00, value); }

        public int Height { get => _s.GetInt32(0x04); internal set => _s.SetInt32(0x04, value); }

        public HSDByteArray IndexData { get => _s.GetReference<HSDByteArray>(0x08); internal set => _s.SetReference(0x08, value); }

        public HSDIntArray PaletteData { get => _s.GetReference<HSDIntArray>(0x0C); internal set => _s.SetReference(0x0C, value); }

        public int ColorCount { get => _s.GetInt32(0x10); internal set => _s.SetInt32(0x10, value); }

        public int Non0Count { get => _s.GetInt32(0x14); internal set => _s.SetInt32(0x14, value); }

        public float Size { get => _s.GetFloat(0x18); internal set => _s.SetFloat(0x18, value); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pixels"></param>
        public void FromTOBJ(HSD_TOBJ tobj)
        {
            if (tobj.ImageData == null)
                return;

            Width = tobj.ImageData.Width;
            Height = tobj.ImageData.Height;

            List<int> indices = new List<int>();
            List<byte> ids = new List<byte>();
            indices.Add(0xFF);

            var data = tobj.GetDecodedImageData();
            int non0 = 0;
            for (int i = 0; i < data.Length; i += 4)
            {
                var b = data[i];
                var g = data[i + 1];
                var r = data[i + 2];
                var a = data[i + 3];
                var color = ((r & 0xFF) << 24) | ((g & 0xFF) << 16) | ((b & 0xFF) << 8) | (a & 0xFF);
                var index = indices.IndexOf(color);

                if (index == -1)
                {
                    index = indices.Count;
                    indices.Add(color);
                }

                if (index != 0)
                    non0++;

                ids.Add((byte)index);
            }
            Non0Count = non0;
            ColorCount = indices.Count;

            IndexData = new HSDByteArray();
            IndexData.Array = ids.ToArray();

            PaletteData = new HSDIntArray();
            PaletteData.Array = indices.ToArray();
        }
    }
}
