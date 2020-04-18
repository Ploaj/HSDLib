using HSDRaw.GX;
using HSDRaw.Tools;
using System.Collections.Generic;

namespace HSDRaw.Common.Animation
{
    public class HSD_TexAnim : HSDListAccessor<HSD_TexAnim>
    {
        public override int TrimmedSize => 0x18;

        public override HSD_TexAnim Next { get => _s.GetReference<HSD_TexAnim>(0x00); set => _s.SetReference(0x00, value); }
        
        public GXTexMapID GXTexMapID { get => (GXTexMapID)_s.GetInt32(0x04); set => _s.SetInt32(0x04, (int)value); }

        public HSD_AOBJ AnimationObject { get => _s.GetReference<HSD_AOBJ>(0x08); set => _s.SetReference(0x08, value); }

        public HSDArrayAccessor<HSD_TexBuffer> ImageBuffers
        {
            get => _s.GetReference<HSDArrayAccessor<HSD_TexBuffer>>(0x0C);
            set
            {
                ImageCount = value != null ? (short)value.Length : (short)0;
                _s.SetReference(0x0C, value);
            }
        }
        
        public HSDArrayAccessor<HSD_TlutBuffer> TlutBuffers
        {
            get => _s.GetReference<HSDArrayAccessor<HSD_TlutBuffer>>(0x10);
            set
            {
                TlutCount = value != null ? (short)value.Length : (short)0;
                _s.SetReference(0x10, value);
            }
        }

        public short ImageCount
        {
            get => _s.GetInt16(0x14);
            internal set => _s.SetInt16(0x14, value);
        }

        public short TlutCount
        {
            get => _s.GetInt16(0x16);
            internal set => _s.SetInt16(0x16, value);
        }

        /// <summary>
        /// Adds a new <see cref="HSD_TOBJ"/> too image bank
        /// </summary>
        /// <param name="tobj"></param>
        /// <returns>-1 is TOBJ is inavlid format and new index otherwise</returns>
        public int AddImage(HSD_TOBJ tobj)
        {
            if (tobj == null)
                return -1;

            if (ImageCount > 0 && tobj.ImageData.Format != ImageBuffers[0].Data.Format)
                return -1;

            if (TlutCount > 0 && tobj.TlutData != null && tobj.TlutData.Format != TlutBuffers[0].Data.Format)
                return -1;

            if (ImageBuffers == null)
                ImageBuffers = new HSDArrayAccessor<HSD_TexBuffer>();
            ImageBuffers.Add(new HSD_TexBuffer() { Data = tobj.ImageData });
            ImageCount++;

            if(tobj.TlutData != null)
            {
                if (TlutBuffers == null)
                    TlutBuffers = new HSDArrayAccessor<HSD_TlutBuffer>();
                TlutBuffers.Add(new HSD_TlutBuffer() { Data = tobj.TlutData });
                TlutCount++;
            }

            return ImageBuffers.Length - 1;
        }

        /// <summary>
        /// Removes image at given index
        /// Adjust Key Frames to remove image as well
        /// </summary>
        /// <param name="index"></param>
        public void RemoveImageAt(int index)
        {
            if(index >= 0 && index < ImageCount)
            {
                ImageBuffers.RemoveAt(index);
                ImageCount--;
            }
            if (index >= 0 && index < TlutCount)
            {
                TlutBuffers.RemoveAt(index);
                TlutCount--;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HSD_TOBJ[] ToTOBJs()
        {
            HSD_TOBJ[] tobj = new HSD_TOBJ[ImageCount];

            for(int i = 0; i < tobj.Length; i++)
            {
                tobj[i] = new HSD_TOBJ()
                {
                    ImageData = ImageBuffers[i].Data,
                };
                if (i < TlutCount)
                    tobj[i].TlutData = TlutBuffers[i].Data;
            }

            return tobj;
        }

        /// <summary>
        /// 
        /// </summary>
        public void FromTOBJs(IEnumerable<HSD_TOBJ> tobjs, bool generateAnimation)
        {
            ImageBuffers = null;
            TlutBuffers = null;
            List<FOBJKey> keys = new List<FOBJKey>();
            int index = 0;
            foreach (var t in tobjs)
            {
                AddImage(t);
                keys.Add(new FOBJKey() { Frame = index, Value = index, InterpolationType = GXInterpolationType.HSD_A_OP_CON });
                index++;
            }

            if(generateAnimation)
            {
                AnimationObject = new HSD_AOBJ();
                AnimationObject.EndFrame = index;
                AnimationObject.Flags = AOBJ_Flags.ANIM_LOOP;
                AnimationObject.FObjDesc = new HSD_FOBJDesc();
                AnimationObject.FObjDesc.SetKeys(keys, (byte)TexTrackType.HSD_A_T_TIMG);
                if (TlutCount != 0)
                {
                    AnimationObject.FObjDesc.Next = new HSD_FOBJDesc();
                    AnimationObject.FObjDesc.Next.SetKeys(keys, (byte)TexTrackType.HSD_A_T_TCLT);
                }
            }
        }
    }

    public class HSD_TexBuffer : HSDAccessor
    {
        public override int TrimmedSize => 4;

        public HSD_Image Data { get => _s.GetReference<HSD_Image>(0); set => _s.SetReference(0, value); }
    }

    public class HSD_TlutBuffer : HSDAccessor
    {
        public override int TrimmedSize => 4;

        public HSD_Tlut Data { get => _s.GetReference<HSD_Tlut>(0); set => _s.SetReference(0, value); }
    }
}
