using System;
using HSDLib.Common;
using HSDLib.Animation;

namespace HSDLib.MaterialAnimation
{
    public class HSD_TexAnim : IHSDList<HSD_TexAnim>
    {
        public override HSD_TexAnim Next { get; set; }
        
        public uint GXTexMapID { get; set; }
        
        public HSD_AOBJ AnimationObject { get; set; }
        
        public uint ImageArrayOffset { get; set; }
        
        public uint TlutArrayOffset { get; set; }
        
        public ushort ImageCount {
            get
            {
                if (ImageArray.Elements == null)
                    return 0;
                return (ushort)ImageArray.Elements.Length;
            } set
            {
                _imagecount = value;
            }
        }
        private ushort _imagecount = 0;
        
        public ushort TlutCount
        {
            get
            {
                if (TlutArray.Elements == null)
                    return 0;
                return (ushort)TlutArray.Elements.Length;
            }
            set
            {
                _tlutcount = value;
            }
        }
        private ushort _tlutcount = 0;

        public HSD_PointerArray<HSD_IOBJ> ImageArray;
        public HSD_PointerArray<HSD_Tlut> TlutArray;

        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);

            ImageArray = new HSD_PointerArray<HSD_IOBJ>() { SetSize = _imagecount};
            Reader.Seek(ImageArrayOffset);
            ImageArray.Open(Reader);
            TlutArray = new HSD_PointerArray<HSD_Tlut>() { SetSize = _tlutcount };
            Reader.Seek(TlutArrayOffset);
            TlutArray.Open(Reader);
        }

        public override void Save(HSDWriter Writer)
        {
            Writer.WriteObject(TlutArray);
            Writer.WriteObject(ImageArray);

            base.Save(Writer);

            Writer.WritePointerAt((int)(Writer.BaseStream.Position - 12), ImageArray);
            Writer.WritePointerAt((int)(Writer.BaseStream.Position - 8), TlutArray);
        }
    }
}
